using Eticadata.Common;
using Eticadata.Cust.WebServices.Models.Stocks;
using Eticadata.ERP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Eticadata.Cust.WebServices.Controllers
{
    public class StockMovsController : ApiController
    {

        
        [HttpPost]
        [Authorize]
        public IHttpActionResult RecalcStock([FromBody] RecalcInput input)
        {
            try
            {
                var app = Eti.Aplicacao;
                //Efectuar recalculo

                System.DateTime DtmFecho = System.DateTime.Today;
                bool closeDate = false;
                if (input.CloseDate != null)
                {
                    closeDate = true;
                    DtmFecho = (DateTime)input.CloseDate;
                }

                DateTime until = DateTime.Today;
                bool processUntil = false;
                if (input.ProcessoUntil != null)
                {
                    processUntil = true;
                    until = (DateTime)input.ProcessoUntil;
                }

                int NcdTotais = app.MoedaBase.NcdTotais;
                int NcdCustosUnit = app.MoedaBase.NcdCustosUnit;
                app.Movimentos.MovRecalculo.GetValoresAmbiente(ref DtmFecho, ref NcdTotais, ref NcdCustosUnit);
                //Inicializações correspondem opções da janela 
                app.Movimentos.MovRecalculo.RecalculaStocksInic(closeDate, DtmFecho, false, false, processUntil, until, true, "", "");
                //Executar o recalculo
                bool Result = app.Movimentos.MovRecalculo.RecalculaStocks();

                if (!Result)
                    return BadRequest(app.Movimentos.MovRecalculo.GetErrorMessage);

                return Ok(Result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public IHttpActionResult CreateDoc([FromBody] Models.Stocks.Stock document)
        {
            try
            {
                MovStock movStock = Eti.Aplicacao.Movimentos.MovStocks.GetNew(document.DocTypeAbbrev, document.SectionCode);

                movStock.Cabecalho.Data = document.Date;

                int lineNumber = 1;
                foreach (var line in document.Lines)
                {
                    movStock.AddLin(ref lineNumber);
                    string code = line.ItemCode;
                    bool blnAfectaOutrasLinhas = false;
                    bool blnAssociacoesFixas = false;
                    bool blnAssociacoesLivres = false;
                    ERP.EtiEnums.TpProcuraArtigo pProcuraArtigo = ERP.EtiEnums.TpProcuraArtigo.Encontrou;
                    bool blnStockDisponivel = false;
                    bool blnImpeditivo = false;

                    movStock.Lines[lineNumber].CodArtigo = code;
                    movStock.AlteraArtigo(lineNumber, ref code, ref blnAfectaOutrasLinhas, ref blnAssociacoesFixas, ref blnAssociacoesLivres, ref pProcuraArtigo, false, ref blnStockDisponivel);

                    movStock.Lines[lineNumber].Quantidade = line.Quantity;
                    movStock.AlteraQuantidade(lineNumber, line.Quantity, ref blnAfectaOutrasLinhas, false, ref blnStockDisponivel);

                    movStock.AlteraValorUnitario(lineNumber, line.UnitCost);

                    movStock.Lines[lineNumber].CodArmazem = line.Warehouse;
                    movStock.AlteraArmazem(lineNumber, line.Warehouse, false, ref blnStockDisponivel);

                    movStock.CalculaTotais();

                    if (movStock.Validate(true)) {  Eti.Aplicacao.Movimentos.MovStocks.Update(ref movStock, ref blnImpeditivo, true); }
                    
                    if (!string.IsNullOrEmpty(movStock.EtiErrorCode))
                    {
                        return BadRequest(movStock.EtiErrorDescription);
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return Ok(document);
        }

    }
}