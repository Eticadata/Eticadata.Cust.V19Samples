using Eticadata.Cust.WebServices.Helpers;
using Eticadata.Cust.WebServices.Models.CustomerOrders;
using Eticadata.ERP;
using Eticadata.ERP.EtiEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;

namespace Eticadata.Cust.WebServices.Controllers
{
    public class CustomerOrdersTAController : ApiController
    {
        public myCustomerOrder GetNewCustomerOrder()
        {
            //Apenas com uma linha
            CustomerOrderLine line = new CustomerOrderLine()
            {
                LineNumber = 1,
                ItemCode = "ART1",
                ItemDescription = "Artigo cust",
                Quantity = 3,
                VATTax = 23,
                UnitPriceExcludedVAT = 100,
                Discount1 = 1,
                Discount2 = 2,
                Discount3 = 3,
                DiscountValue = 4,
            };

            myCustomerOrder myOrder = new myCustomerOrder()
            {
                FiscalYear = "2018",
                SectionCode = "1",
                DocTypeAbbrev = "ENCCL",
                EntityCode = 1,
                Date = DateTime.Now,
                ExpirationDate = DateTime.Now,
                CurrencyCode = "EUR",
                Lines = new List<CustomerOrderLine>() { line },
                GetReportBytes = true,
            };

            return myOrder;
        }


        [HttpPost]
        [Authorize]
        public IHttpActionResult GenerateCustomerOrder([FromBody] myCustomerOrder pOrder)
        {
            MovEncomenda myOrder;
            MovEncomendaLin myOrderLine;

            var byRefFalse = false;
            var stockAvailable = true;
            var affectsOtherLines = true;
            var fixedAssociation = true;
            var freeAssociation = true;
            var checkStock = false;
            var famForQty = false;
            var famForPrice = false;
            TpProcuraArtigo searchItem = 0;
            int numberLine;
            string itemCode;

            var errorDescription = "";
            var result = new DocumentResult();

            try
            {
                //pOrder = GetNewCustomerOrder();

                myOrder = Eti.Aplicacao.Movimentos.MovEncomendasCli.GetNew(pOrder.DocTypeAbbrev, pOrder.SectionCode);
                myOrder.Cabecalho.CodExercicio = pOrder.FiscalYear;

                myOrder.Cabecalho.AplicacaoOrigem = "WS";

                myOrder.Cabecalho.Data = pOrder.Date.Date;
                myOrder.Cabecalho.DataVencimento = pOrder.ExpirationDate;
                myOrder.Cabecalho.Hora = pOrder.Date.Hour.ToString("00") + ":" + pOrder.Date.Minute.ToString("00");

                myOrder.Cabecalho.CodEntidade = pOrder.EntityCode;
                myOrder.AlteraEntidade((byte)TpEntidade.Cliente, myOrder.Cabecalho.CodEntidade, true);

                myOrder.Cabecalho.AbrevMoeda = pOrder.CurrencyCode;
                myOrder.AlteraMoeda(pOrder.CurrencyCode, ref byRefFalse, false);

                foreach (CustomerOrderLine line in pOrder.Lines.OrderBy(p => p.LineNumber))
                {
                    itemCode = line.ItemCode;

                    numberLine = myOrder.Lines.Count + 1;
                    myOrder.AddLin(ref numberLine);
                    myOrderLine = myOrder.Lines[numberLine];
                    myOrderLine.TipoLinha = TpLinha.Artigo;

                    myOrderLine.CodArtigo = itemCode;
                    myOrder.AlteraArtigo(numberLine, ref itemCode, ref affectsOtherLines, ref fixedAssociation, ref freeAssociation, ref searchItem, checkStock, ref stockAvailable, ref famForQty, ref famForPrice);
                    myOrderLine.DescArtigo = line.ItemDescription;

                    myOrderLine.Quantidade = line.Quantity;
                    myOrder.AlteraQuantidade(numberLine, myOrderLine.Quantidade, ref affectsOtherLines, false, ref stockAvailable);

                    if (myOrder.TDocIvaIncluido)
                    {
                        line.UnitPriceExcludedVAT = line.UnitPriceExcludedVAT * (1 + line.VATTax / 100.0);
                    }

                    myOrderLine.PrecoUnitario = Convert.ToDouble(line.UnitPriceExcludedVAT);
                    myOrder.AlteraPrecoUnitario(numberLine, myOrderLine.PrecoUnitario, ref byRefFalse);

                    myOrderLine.TaxaIva = Convert.ToDouble(line.VATTax);
                    myOrderLine.CodTaxaIva = Eti.Aplicacao.Tabelas.TaxasIvas.GetTaxaIva(Convert.ToDecimal(myOrderLine.TaxaIva));
                    myOrder.AlteraTaxaIVA(numberLine, myOrderLine.CodTaxaIva);

                    myOrderLine.Desconto1 = line.Discount1;
                    myOrder.AlteraDesconto(1, numberLine, myOrderLine.Desconto1);

                    myOrderLine.Desconto2 = line.Discount2;
                    myOrder.AlteraDesconto(2, numberLine, myOrderLine.Desconto2);

                    myOrderLine.Desconto3 = line.Discount3;
                    myOrder.AlteraDesconto(3, numberLine, myOrderLine.Desconto3);

                    myOrderLine.DescontoValorLinha = line.DiscountValue;
                    myOrder.AlteraDesconto(4, numberLine, myOrderLine.DescontoValorLinha);
                }

                var validate = myOrder.Validate(true);
                if (validate)
                {                    
                    Eti.Aplicacao.Movimentos.MovEncomendasCli.Update(ref myOrder);
                }

                if (myOrder.EtiErrorDescription == "")
                {
                    result = new DocumentResult()
                    {
                        ErrorDescription = myOrder.EtiErrorDescription,
                        DocumentKey = new Helpers.DocumentKey()
                        {
                            SectionCode = myOrder.Cabecalho.CodSeccao,
                            DocTypeAbbrev = myOrder.Cabecalho.AbrevTpDoc,
                            FiscalYear = myOrder.Cabecalho.CodExercicio,
                            Number = myOrder.Cabecalho.Numero
                        }
                    };

                    if (pOrder.GetReportBytes)
                    {
                        result.reportBytes = Functions.GetReportBytes(TpDocumentoAEmitir.Encomendas, result.DocumentKey);
                    }
                }
                else
                {
                    result.ErrorDescription = myOrder.EtiErrorDescription;
                }
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
        //GET api/CustomerOrdersTA/GetReportBytes?fiscalYear=2018&section=1&docType=ENCCL&number=1
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

                byte[] reportBytes = Functions.GetReportBytes(TpDocumentoAEmitir.Encomendas, docKey);

                return Ok(reportBytes);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}