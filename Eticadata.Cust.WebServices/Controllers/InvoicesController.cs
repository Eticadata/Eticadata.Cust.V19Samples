using Eticadata.Cust.WebServices.Helpers;
using Eticadata.Cust.WebServices.Models;
using Eticadata.ERP;
using Eticadata.ERP.EtiEnums;
using Eticadata.Views.Reports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Eticadata.Cust.WebServices.Controllers
{
    public class InvoicesController : ApiController
    {
        [HttpPost]
        [Authorize]
         //api/Invoices/GenerateSalesDoc
         //data: {
	        //"FiscalYear":"2018",
	        //"SectionCode":"1",
	        //"DocTypeAbbrev":"FAT",
	        //"EntityCode":"502395028",
	        //"Date":"2018/01/25",
	        //"ExpirationDate":"2018/02/24",
	        //"CurrencyCode":"EUR",
	        //"Lines":[
	        //	{
	        //		"LineNumber": "1",
	        //		"ItemCode": "AMORT",
	        //		"ItemDescription": "AMORTECEDOR 001",
	        //		"Quantity": "2",
	        //		"VATTax": "23",
	        //		"UnitPriceExcludedVAT": "39.90",
	        //		"Discount1": "5",
	        //		"Discount2": "0",
	        //		"Discount3": "0",
	        //		"DiscountValue": "0"
            //  }
	        //]}
        public IHttpActionResult GenerateSalesDoc([FromBody] Models.mySale document)
        {
            byte[] reportBytes;

            MovVenda sale;
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

            try
            {
                sale = Eti.Aplicacao.Movimentos.MovVendas.GetNew(document.DocTypeAbbrev, document.SectionCode);
                sale.Cabecalho.CodExercicio = document.FiscalYear;

                sale.Cabecalho.AplicacaoOrigem = "WS";

                sale.Cabecalho.Data = document.Date.Date;
                sale.Cabecalho.DataVencimento = document.ExpirationDate;

                sale.Cabecalho.CodEntidade = document.EntityCode;
                sale.AlteraEntidade(TpEntidade.Cliente, sale.Cabecalho.CodEntidade, true, true);

                sale.Cabecalho.AbrevMoeda = document.CurrencyCode;
                sale.AlteraMoeda(document.CurrencyCode, ref byRefFalse, false);

                foreach (SalesLine line in document.Lines.OrderBy(p => p.LineNumber))
                {
                    itemCode = line.ItemCode;

                    numberLine = sale.Lines.Count + 1;
                    sale.AddLin(ref numberLine);
                    saleLine = sale.Lines[numberLine];
                    saleLine.TipoLinha = TpLinha.Artigo;

                    saleLine.CodArtigo = itemCode;
                    sale.AlteraArtigo(numberLine, ref itemCode, ref affectsOtherLines, ref fixedAssociation, ref freeAssociation, ref searchItem, checkStock, ref stockAvailable, ref famForQty, ref famForPrice);
                    saleLine.DescArtigo = line.ItemDescription;

                    saleLine.Quantidade = line.Quantity;
                    sale.AlteraQuantidade(numberLine, saleLine.Quantidade, ref affectsOtherLines, false, ref stockAvailable);

                    saleLine.PrecoUnitario = Convert.ToDouble(line.UnitPriceExcludedVAT);
                    sale.AlteraPrecoUnitario(numberLine, saleLine.PrecoUnitario, ref byRefFalse);

                    saleLine.TaxaIva = Convert.ToDouble(line.VATTax);
                    saleLine.CodTaxaIva = Eti.Aplicacao.Tabelas.TaxasIvas.GetTaxaIva(Convert.ToDecimal(saleLine.TaxaIva));
                    sale.AlteraTaxaIVA(numberLine, saleLine.CodTaxaIva);

                    saleLine.Desconto1 = line.Discount1;
                    sale.AlteraDesconto(1, numberLine, saleLine.Desconto1);

                    saleLine.Desconto2 = line.Discount2;
                    sale.AlteraDesconto(2, numberLine, saleLine.Desconto2);

                    saleLine.Desconto3 = line.Discount3;
                    sale.AlteraDesconto(3, numberLine, saleLine.Desconto3);

                    saleLine.DescontoValorLinha = line.DiscountValue;
                    sale.AlteraDesconto(4, numberLine, saleLine.DescontoValorLinha);
                }

                var validate = sale.Validate(true);
                if (validate)
                {
                    var blockingStock = false;
                    Eti.Aplicacao.Movimentos.MovVendas.Update(ref sale, ref blockingStock, true, 0, "");
                }

                if (!string.IsNullOrEmpty(sale.EtiErrorCode))
                {
                    throw new Exception($@"ErrorCode:{sale.EtiErrorCode}{Environment.NewLine}
                                            EtiErrorDescription:{sale.EtiErrorDescription}");
                }
                else
                {
                    DocumentKey docKey = new Helpers.DocumentKey()
                    {
                        SectionCode = sale.Cabecalho.CodSeccao,
                        DocTypeAbbrev = sale.Cabecalho.AbrevTpDoc,
                        FiscalYear = sale.Cabecalho.CodExercicio,
                        Number = sale.Cabecalho.Numero
                    };

                    reportBytes = Functions.GetReportBytes(TpDocumentoAEmitir.Vendas, docKey);
                }

                return Ok(reportBytes);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        //GET api/Invoices/PrintSalesDoc?fiscalYear=2018&section=1&docType=FAT&number=1
        public IHttpActionResult PrintSalesDoc([FromUri] string fiscalYear, [FromUri] string section, [FromUri] string docType, [FromUri] int number, [FromUri] bool toFile = false)
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

                byte[] reportBytes = Functions.GetReportBytes(TpDocumentoAEmitir.Encomendas , docKey);

                if (toFile)
                {
                    string docFileNAme = System.Web.Hosting.HostingEnvironment.MapPath($@"~/App_Data/{docType}_{number}.pdf");
                    File.WriteAllBytes(docFileNAme, reportBytes);
                    return Ok(docFileNAme);
                }

                return Ok(reportBytes);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
