using EC748.CombustiveisInfConsumidor.Extensions;
using Eticadata.Cust.WebServices.Helpers;
using Eticadata.Cust.WebServices.Models;
using Eticadata.ERP;
using Eticadata.ERP.EtiEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Eticadata.Cust.WebServices.Controllers
{
    public class FuelInformationController : ApiController
    {
        
            //{
            //	"FiscalYear":"EX 2020",
            //	"SectionCode":"1",
            //	"DocTypeAbbrev":"FTR",
            //	"EntityCode":"502395028",
            //	"Date":"2020/07/21",
            //	"ExpirationDate":"2020/07/21",
            //	"CurrencyCode":"EUR",
            //	"Lines":[
            //		{
            //			"LineNumber": "1",
            //	        "ItemCode": "GAS95",
            //	        "ItemDescription": "GASOLINA 95",
            //	        "Quantity": "19.29",
            //	        "VATTax": "23",
            //	        "UnitPriceExcludedVAT": "28.14",
            //	        "Discount1": "0",
            //	        "Discount2": "0",
            //	        "Discount3": "0",
            //	        "DiscountValue": "0"
            //        }
            //	],
            //	"CustomInfo":[{
            //		"ISPCateg": "A",
            //		"ISPTaxa": 0.667,
            //		"ISPUnid": 1.0,
            //		"Quantidade": 19.29,
            //		"TaxaCalculada": 12.87,
            //		"TipoComb": "Gasolina 95 Simples",
            //		"EmissaoUnidade": "kgCO2eq/l",
            //		"EmissaoCO2": 2.1,
            //		"FossilPerc": 90,
            //		"ISPNomenclatura": "E5",
            //		"RenovavelPerc": 10,
            //		"SobreCusto": 0.667
            //    }]
            //}

        [HttpPost]
        [Authorize]
        public IHttpActionResult SaveExternalInvoice([FromBody] Models.mySale document)
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

                /// Marca como documento externo, para não recalcular informação de combustiveis
                sale.IsDocExterno = true;
                /// sale.AlteraInfoCertificacao("999", "1", "sdfhkdfFASDGFSfgsfgjsklgfsk394wrkd", "FT 1/598");

                /// INFORMAÇÃO COMBUSTÍVEIS
                foreach (dynamic customInfo in document.CustomInfo)
                {
                    sale.AddISPLines(new List<EC748.CombustiveisInfConsumidor.Models.ISPCombustivel>()
                    {
                        new EC748.CombustiveisInfConsumidor.Models.ISPCombustivel()
                        {
                            
                            ISPCateg = customInfo.ISPCateg,
                            ISPTaxa = customInfo.ISPTaxa,
                            ISPUnid = customInfo.ISPUnid,
                            Quantidade = customInfo.Quantidade,
                            TaxaCalculada = customInfo.TaxaCalculada,
                            TipoComb = customInfo.TipoComb,
                            
                        }
                    });

                    sale.AddISPInfoLines(new List<EC748.CombustiveisInfConsumidor.Models.ISPCombustivelInfo>()
                    {
                        new EC748.CombustiveisInfConsumidor.Models.ISPCombustivelInfo()
                        {
                            EmissaoUnidade = customInfo.EmissaoUnidade,
                            EmissaoCO2 = customInfo.EmissaoCO2,
                            FossilPerc = customInfo.FossilPerc,
                            ISPCategoria = customInfo.ISPCateg,
                            ISPNomenclatura = customInfo.ISPNomenclatura,
                            RenovavelPerc = customInfo.RenovavelPerc,
                            SobreCusto = customInfo.SobreCusto,
                            tpCombustivel = customInfo.TipoComb
                        }
                    });
                }
                /// INFORMAÇÃO COMBUSTÍVEIS
                
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

    }
}
