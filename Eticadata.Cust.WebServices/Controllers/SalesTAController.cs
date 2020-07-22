using Eticadata.Cust.WebServices.Helpers;
using Eticadata.Cust.WebServices.Models;
using Eticadata.ERP;
using Eticadata.ERP.EtiEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;

namespace Eticadata.Cust.WebServices.Controllers
{
    public class SalesTAController : ApiController
    {        
        [HttpPost]
        [Authorize]        
        public IHttpActionResult GenerateSalesDoc([FromBody] Models.mySale pSale)
        {
            MovVenda mySale;
            MovVendaLin saleLine;
            var byRefFalse = false;
            var stockAvailable = true;
            var affectsOtherLines = true;
            var fixedAssociation = true;
            var freeAssociation = true;
            var checkStock = false;
            var famForQty = false;
            var famForPrice = false;
            TpProcuraArtigo searchItem = TpProcuraArtigo.NaoEncontrou;
            int numberLine;
            string itemCode;

            var errorDescription = "";
            var result = new DocumentResult();

            try
            {                   
                mySale = Eti.Aplicacao.Movimentos.MovVendas.GetNew(pSale.DocTypeAbbrev, pSale.SectionCode);
                mySale.Cabecalho.CodExercicio = pSale.FiscalYear;

                mySale.Cabecalho.AplicacaoOrigem = "WS";

                mySale.Cabecalho.Data = pSale.Date.Date;
                mySale.Cabecalho.DataVencimento = pSale.ExpirationDate;
                mySale.Cabecalho.Hora = pSale.Date.Hour.ToString("00") + ":" + pSale.Date.Minute.ToString("00");

                mySale.Cabecalho.CodEntidade = pSale.EntityCode;
                mySale.AlteraEntidade(TpEntidade.Cliente, mySale.Cabecalho.CodEntidade, true, true);

                mySale.Cabecalho.AbrevMoeda = pSale.CurrencyCode;
                mySale.AlteraMoeda(pSale.CurrencyCode, ref byRefFalse, false);

                foreach (SalesLine line in pSale.Lines.OrderBy(p => p.LineNumber))
                {
                    itemCode = line.ItemCode;

                    numberLine = mySale.Lines.Count + 1;
                    mySale.AddLin(ref numberLine);
                    saleLine = mySale.Lines[numberLine];
                    saleLine.TipoLinha = TpLinha.Artigo;

                    saleLine.CodArtigo = itemCode;
                    mySale.AlteraArtigo(numberLine, ref itemCode, ref affectsOtherLines, ref fixedAssociation, ref freeAssociation, ref searchItem, checkStock, ref stockAvailable, ref famForQty, ref famForPrice);
                    saleLine.DescArtigo = line.ItemDescription;

                    saleLine.Quantidade = line.Quantity;
                    mySale.AlteraQuantidade(numberLine, saleLine.Quantidade, ref affectsOtherLines, false, ref stockAvailable);

                    if (mySale.TDocIvaIncluido)
                    {
                        line.UnitPriceExcludedVAT = line.UnitPriceExcludedVAT * (1 + line.VATTax / 100.0);
                    }

                    saleLine.PrecoUnitario = Convert.ToDouble(line.UnitPriceExcludedVAT);
                    mySale.AlteraPrecoUnitario(numberLine, saleLine.PrecoUnitario, ref byRefFalse);

                    saleLine.TaxaIva = Convert.ToDouble(line.VATTax);
                    saleLine.CodTaxaIva = Eti.Aplicacao.Tabelas.TaxasIvas.GetTaxaIva(Convert.ToDecimal(saleLine.TaxaIva));
                    mySale.AlteraTaxaIVA(numberLine, saleLine.CodTaxaIva);

                    saleLine.Desconto1 = line.Discount1;
                    mySale.AlteraDesconto(1, numberLine, saleLine.Desconto1);

                    saleLine.Desconto2 = line.Discount2;
                    mySale.AlteraDesconto(2, numberLine, saleLine.Desconto2);

                    saleLine.Desconto3 = line.Discount3;
                    mySale.AlteraDesconto(3, numberLine, saleLine.Desconto3);

                    saleLine.DescontoValorLinha = line.DiscountValue;
                    mySale.AlteraDesconto(4, numberLine, saleLine.DescontoValorLinha);
                }


                //redefinir meios de pagamento
                var linePayMovType = 1;

                if (mySale.LinesPag.Count > 0) mySale.LinesPag.Clear();

                foreach (SalePayment line in pSale.LinesPayment)
                {
                    var movType = line.PayMovTypeCodeTres;                    
                    mySale.LinesPag[linePayMovType].AbrevTpMovPagTes = movType;
                    mySale.AlteraPagamAbrevTpMov(linePayMovType, movType, ref byRefFalse, ref byRefFalse, ref byRefFalse, ref byRefFalse, ref byRefFalse, ref byRefFalse, ref byRefFalse, ref byRefFalse);
                    mySale.LinesPag[linePayMovType].Cambio = line.Exchange;
                    mySale.LinesPag[linePayMovType].Valor = line.Value;
                    mySale.AlteraPagamValor(linePayMovType, mySale.LinesPag[linePayMovType].Valor);
                    mySale.LinesPag[linePayMovType].AbrevMoeda = line.CurrencyCode;

                    mySale.LinesPag[linePayMovType].ValorMoedaDoc = mySale.LinesPag[linePayMovType].Valor;

                    linePayMovType++;
                }

                //documento externo certeficado
                //mySale.AlteraInfoCertificacao(certificationNumber, "1", hash, saftDocNo);

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

                    if (pSale.GetReportBytes)
                    {
                        result.reportBytes = Functions.GetReportBytes(TpDocumentoAEmitir.Vendas, result.DocumentKey);
                    }
                }

                return Ok(result);

            }
            catch (Exception ex)
            {
                errorDescription = string.Format("{0}.{1}.{2}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name, ex.Message);
                result.ErrorDescription = errorDescription;
            }

            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        //GET api/Invoices/PrintSalesDoc?fiscalYear=2018&section=1&docType=FAT&number=1
        public IHttpActionResult GetReportBytes([FromUri] string fiscalYear, [FromUri] string section, [FromUri] string docType, [FromUri] int number)
        {
            try
            {
                DocumentKey docKey = new Helpers.DocumentKey()
                {
                    SectionCode = section,
                    DocTypeAbbrev = docType,
                    FiscalYear = fiscalYear,
                    Number = number,
                };

                byte[] reportBytes = Functions.GetReportBytes(TpDocumentoAEmitir.Vendas , docKey);

                return Ok(reportBytes);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
