using Eticadata.Cust.WebServices.Helpers;
using Eticadata.Cust.WebServices.Models;
using Eticadata.ERP;
using Eticadata.ERP.EtiEnums;
using Eticadata.Views.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Eticadata.Cust.WebServices.Controllers
{
    public class PurchasesController : ApiController
    {
        [HttpPost]
        [Authorize]
        //api/Purchases/GeneratePurchaseDoc
        //data: {
        //"FiscalYear":"2018",
        //"SectionCode":"1",
        //"DocTypeAbbrev":"V/FAT",
        //"EntityCode":"502395028",
        //"Date":"2018/01/25",
        //"ExpirationDate":"2018/02/24",
        //"CurrencyCode":"EUR",
        //"Lines":[
        //	{
        //		"LineNumber": "1",
        //		"ItemCode": "AMORT",
        //		"ItemDescription": "AMORTECEDOR 001",
        //      "BatchCode" : "LOTE_1",
        //      "BatchDescription" : "Lote 0001".
        //      "EntryDate" : "2018-03-27"
        //      "ExpirationDate" : "2019-03-27"
        //		"Quantity": "2",
        //		"VATTax": "23",
        //		"UnitPriceExcludedVAT": "39.90",
        //		"Discount1": "5",
        //		"Discount2": "0",
        //		"Discount3": "0",
        //		"DiscountValue": "0"
        //  }
        //]}
        public IHttpActionResult GeneratePurchaseDoc([FromBody] Models.myPurchase document)
        {
            byte[] reportBytes;

            MovCompra purch;
            MovCompraLin purchLine;
            var byRefFalse = false;
            var stockAvailable = true;
            var affectsOtherLines = true;
            var fixedAssociation = true;
            var freeAssociation = true;
            var checkStock = false;
            TpProcuraArtigo searchItem = TpProcuraArtigo.NaoEncontrou;
            int numberLine;
            string itemCode;

            try
            {
                purch = Eti.Aplicacao.Movimentos.MovCompras.GetNew(document.DocTypeAbbrev, document.SectionCode);
                purch.Cabecalho().CodExercicio = document.FiscalYear;

                purch.Cabecalho().AplicacaoOrigem = "WS";

                purch.Cabecalho().Data = document.Date.Date;
                purch.Cabecalho().DataVencimento = document.ExpirationDate;

                purch.Cabecalho().CodEntidade = document.EntityCode;
                purch.AlteraEntidade(TpEntidade.Fornecedor, purch.Cabecalho().CodEntidade, true, true);

                purch.Cabecalho().AbrevMoeda = document.CurrencyCode;
                purch.AlteraMoeda(document.CurrencyCode, ref byRefFalse, false);

                foreach (PurchaseLine line in document.Lines.OrderBy(p => p.LineNumber))
                {
                    itemCode = line.ItemCode;

                    numberLine = purch.Lines.Count + 1;
                    purch.AddLin(ref numberLine);
                    purchLine = purch.Lines[numberLine];
                    purchLine.TipoLinha = TpLinha.Artigo;

                    purchLine.CodArtigo = itemCode;
                    purch.AlteraArtigo(numberLine, ref itemCode, ref affectsOtherLines, ref fixedAssociation, ref freeAssociation, ref searchItem, checkStock, ref stockAvailable);
                    purchLine.DescArtigo = line.ItemDescription;

                    //Cria lote caso não exista
                    var item = Eti.Aplicacao.Tabelas.Artigos.Find(itemCode);
                    if (item.Lotes)
                    {
                        var inactive = false;
                        var exists = false;

                        exists = Eti.Aplicacao.Tabelas.Artigos.ExisteLote(itemCode, line.BatchCode, ref inactive);

                        if (!exists)
                        {                            
                            Eti.Aplicacao.Tabelas.Artigos.GravaLote(itemCode, line.BatchCode, line.BatchDescription, line.EntryDate , line.ExpirationDate, "", "", "", "", "");

                            purchLine.Lote = line.BatchCode;

                            checkStock = true;
                            stockAvailable = false;

                            //exists : devolve se o lote existe ou não
                            //inactive: devolve se está inativo
                            //se checkStock = true devolve no stockAvailable se existe stock disponivel
                            purch.AlteraLote(numberLine, itemCode, line.BatchCode, ref exists, ref inactive, checkStock, ref stockAvailable);
                        }
                    }


                    purchLine.Quantidade = line.Quantity;
                    purch.AlteraQuantidade(numberLine, purchLine.Quantidade, ref affectsOtherLines, false, ref stockAvailable);

                    purchLine.PrecoUnitario = Convert.ToDouble(line.UnitPriceExcludedVAT);
                    purch.AlteraPrecoUnitario(numberLine, purchLine.PrecoUnitario);

                    purchLine.TaxaIva = Convert.ToDouble(line.VATTax);
                    purchLine.CodTaxaIva = Eti.Aplicacao.Tabelas.TaxasIvas.GetTaxaIva(Convert.ToDecimal(purchLine.TaxaIva));
                    purch.AlteraTaxaIVA(numberLine, purchLine.CodTaxaIva);

                    purchLine.Desconto1 = line.Discount1;
                    purch.AlteraDesconto(1, numberLine, purchLine.Desconto1);

                    purchLine.Desconto2 = line.Discount2;
                    purch.AlteraDesconto(2, numberLine, purchLine.Desconto2);

                    purchLine.Desconto3 = line.Discount3;
                    purch.AlteraDesconto(3, numberLine, purchLine.Desconto3);

                    purchLine.DescontoValorLinha = line.DiscountValue;
                    purch.AlteraDesconto(4, numberLine, purchLine.DescontoValorLinha);
                }

                var validate = purch.Validate(true);
                if (validate)
                {
                    var blockingStock = false;
                    Eti.Aplicacao.Movimentos.MovCompras.Update(ref purch, ref blockingStock, true, 0, "");
                }

                if (!string.IsNullOrEmpty(purch.EtiErrorCode))
                {
                    throw new Exception($@"ErrorCode:{purch.EtiErrorCode}{Environment.NewLine}
                                            EtiErrorDescription:{purch.EtiErrorDescription}");
                }
                else
                {
                    DocumentKey docKey = new Helpers.DocumentKey()
                    {
                        SectionCode = purch.Cabecalho().CodSeccao,
                        DocTypeAbbrev = purch.Cabecalho().AbrevTpDoc,
                        FiscalYear = purch.Cabecalho().CodExercicio,
                        Number = purch.Cabecalho().Numero
                    };

                    reportBytes = Functions.GetReportBytes(TpDocumentoAEmitir.Compras, docKey);
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
        //GET api/Invoices/PrintPurchaseDoc?fiscalYear=2018&section=1&docType=FAT&number=1
        public IHttpActionResult PrintPurchaseDoc([FromUri] string fiscalYear, [FromUri] string section, [FromUri] string docType, [FromUri] int number)
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

                byte[] reportBytes = Functions.GetReportBytes(TpDocumentoAEmitir.Compras, docKey);

                return Ok(reportBytes);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
