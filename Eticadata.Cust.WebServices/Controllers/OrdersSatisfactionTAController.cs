using Eticadata.Cust.WebServices.Helpers;
using Eticadata.Cust.WebServices.Models.OrdersSatisfaction;
using Eticadata.ERP;
using Eticadata.ERP.EtiEnums;
using System;
using System.Reflection;
using System.Web.Http;

namespace Eticadata.Cust.WebServices.Controllers
{
    public class OrdersSatisfactionTAController : ApiController
    {

        [HttpPost]
        [Authorize]
        public IHttpActionResult GenerateOrdersSatisfaction([FromBody] MyOrderSatisfaction pOrder)
        {
            MovVenda mySale;
            MovVendaLin saleLine;
            MovEncomenda myOrder;             
            var stockAvailable = true;
            var affectsOtherLines = true;
            var fixedAssociation = true;
            var freeAssociation = true;
            var checkStock = false;
            var famForQty = false;
            var famForPrice = false;
            var byRefFalse = false;
            TpProcuraArtigo searchItem = TpProcuraArtigo.NaoEncontrou;
            int numberLine;
            string itemCode;

            var errorDescription = "";
            var result = new DocumentResult();

            try
            {
                //Encomenda a satisfazer
                myOrder = Eti.Aplicacao.Movimentos.MovEncomendasCli.Find(pOrder.KeyOrder.DocTypeAbbrev, pOrder.KeyOrder.FiscalYear, pOrder.KeyOrder.Number, pOrder.KeyOrder.SectionCode);

                //Nova venda
                mySale = Eti.Aplicacao.Movimentos.MovVendas.GetNew(pOrder.DocTypeAbbrevSale, pOrder.KeyOrder.SectionCode);
                mySale.Cabecalho.CodExercicio = pOrder.FiscalYearSale;

                mySale.Cabecalho.AplicacaoOrigem = "WS";

                mySale.Cabecalho.Data = pOrder.DateSale.Date;
                mySale.Cabecalho.DataVencimento = pOrder.ExpirationDateSale;
                mySale.Cabecalho.Hora = pOrder.DateSale.Date.Hour.ToString("00") + ":" + pOrder.DateSale.Date.Minute.ToString("00");

                mySale.Cabecalho.CodEntidade = myOrder.Cabecalho.CodEntidade;
                mySale.AlteraEntidade(TpEntidade.Cliente, mySale.Cabecalho.CodEntidade, true, true);

                //irá satisfazer toda a encomenda.
                foreach(MovEncomendaLin line in myOrder.Lines)
                {
                    itemCode = line.CodArtigo;

                    numberLine = mySale.Lines.Count + 1;
                    mySale.AddLin(ref numberLine);
                    saleLine = mySale.Lines[numberLine];
                    saleLine.TipoLinha = TpLinha.Artigo;

                    saleLine.CodArtigo = itemCode;
                    mySale.AlteraArtigo(numberLine, ref itemCode, ref affectsOtherLines, ref fixedAssociation, ref freeAssociation, ref searchItem, checkStock, ref stockAvailable, ref famForQty, ref famForPrice);

                    saleLine.Quantidade = line.Quantidade;
                    mySale.AlteraQuantidade(numberLine, saleLine.Quantidade, ref affectsOtherLines, false, ref stockAvailable);

                    saleLine.CodArmazem = line.CodArmazem;
                    mySale.AlteraArmazem(numberLine, saleLine.CodArmazem, false, ref stockAvailable);

                    //Tratar a questão do IVA pois os tipos de documentos podem não ter o IVA incluido 
                    saleLine.PrecoUnitario = Convert.ToDouble(line.PrecoUnitario);
                    mySale.AlteraPrecoUnitario(numberLine, saleLine.PrecoUnitario, ref byRefFalse);

                    //Ligar a linha da venda á encomenda, 1 linha de venda para 1 linha de encomenda
                    var countLinIntgr = 1;
                    mySale.SetCountLinEncomIntegr(ref numberLine,ref countLinIntgr);

                    var numberLineIntgr = 1;
                    saleLine.EncomIntegr(numberLineIntgr).EncomCodSeccao = pOrder.KeyOrder.SectionCode;
                    saleLine.EncomIntegr(numberLineIntgr).EncomAbrevTpDoc = pOrder.KeyOrder.DocTypeAbbrev;
                    saleLine.EncomIntegr(numberLineIntgr).EncomCodExercicio = pOrder.KeyOrder.FiscalYear;
                    saleLine.EncomIntegr(numberLineIntgr).EncomNumero = pOrder.KeyOrder.Number;
                    saleLine.EncomIntegr(numberLineIntgr).EncomNumLinha = line.NumLinha;
                    saleLine.EncomIntegr(numberLineIntgr).QtdSatisf = line.QuantidadePend;
                    saleLine.EncomIntegr(numberLineIntgr).QtdPendente = 0;

                    saleLine.DocAQueReporta = pOrder.KeyOrder.DocTypeAbbrev + ' ' + pOrder.KeyOrder.SectionCode + '/' + pOrder.KeyOrder.Number.ToString();
                }

                var validate = mySale.Validate(true);
                if (validate)
                {
                    var blockingStock = false;
                    Eti.Aplicacao.Movimentos.MovVendas.Update(ref mySale, ref blockingStock, true, 0, "");
                }

                if (!string.IsNullOrEmpty(mySale.EtiErrorCode))
                {
                    result.ErrorDescription = mySale.EtiErrorDescription;
                }
                else
                {
                    result = new DocumentResult()
                    {
                        ErrorDescription = mySale.EtiErrorDescription,
                        DocumentKey = new Helpers.DocumentKey()
                        {
                            SectionCode = mySale.Cabecalho.CodSeccao,
                            DocTypeAbbrev = mySale.Cabecalho.AbrevTpDoc,
                            FiscalYear = mySale.Cabecalho.CodExercicio,
                            Number = mySale.Cabecalho.Numero
                        }
                    };
                }

            }
            catch (Exception ex)
            {
                errorDescription = string.Format("{0}.{1}.{2}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name, ex.Message);
                result.ErrorDescription = errorDescription;
            }

            return Ok(result);
        }

    }
}