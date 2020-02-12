using Eticadata.Cust.WebServices.Helpers;
using Eticadata.Cust.WebServices.Models.RepairOrders;
using Eticadata.ERP;
using Eticadata.ERP.EtiEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;

namespace Eticadata.Cust.WebServices.Controllers
{
    public class RepairOrdersTAController : ApiController
    {
        [HttpPost]
        [Authorize]
        public IHttpActionResult GenerateRepairOrder([FromBody] myRepairOrder pRepairOrder)
        {
            MovOrdemReparacao myRepairOrder;
            MovOrdemReparacaoLin newLine;
            List<string> lstMsg = new List<string>();
            int searchItem = 0;

            var errorDescription = "";
            var result = new DocumentResult();

            try
            {
                myRepairOrder = Eti.Aplicacao.MovimentosAutoGest.MovOrdensReparacao.GetNew(pRepairOrder.FiscalYear, pRepairOrder.SectionCode, pRepairOrder.DocTypeAbbrev);

                //Á custa de um orçamento
                //ConversaoOficinas conv = new ConversaoOficinas(Eti.Aplicacao);
                //bool res = conv.GetOR_FromOrcamento(movOrcamento, ref movOR, false, ref msg, true, false, false, 0, 0);

                myRepairOrder.Cabecalho.CodEntidade = pRepairOrder.EntityCode;
                myRepairOrder.AlteraCliente(pRepairOrder.EntityCode, true);

                myRepairOrder.Cabecalho.DataMov = pRepairOrder.Date;
                myRepairOrder.Cabecalho.DataAbertura = pRepairOrder.Date;

                myRepairOrder.AlteraViatura(pRepairOrder.Vehicle, false);

                //inserir linhas de materiais
                foreach (MaterialsLine lineMaterial in pRepairOrder.LinesMaterials.OrderBy(p => p.LineNumber))
                {
                    newLine = myRepairOrder.NovaLinha(TpLinhaOficinas.Materiais, true);

                    newLine.CodArtigo = lineMaterial.ItemCode;
                    myRepairOrder.SetInfoArtigo(newLine, TpLinhaOficinas.Materiais, ref searchItem);
                    newLine.DescArtigo = lineMaterial.ItemDescription;


                    if (myRepairOrder.Cabecalho.IvaIncluido)
                    {
                        lineMaterial.UnitPriceExcludedVAT = lineMaterial.UnitPriceExcludedVAT * (1 + lineMaterial.VATTax / 100.0);
                    }
                                        
                    newLine.PrecoUnitario = lineMaterial.UnitPriceExcludedVAT;
                    newLine.Quantidade = lineMaterial.Quantity;

                    myRepairOrder.CalculaTotaisLinha(newLine);
                }

                //serviços internos
                foreach (InternalServicesLine lineInternalService in pRepairOrder.LinesInternalServices.OrderBy(p => p.LineNumber))
                {
                    newLine = myRepairOrder.NovaLinha(TpLinhaOficinas.ServInternos, true);

                    newLine.CodArtigo = lineInternalService.ItemCode;
                    myRepairOrder.SetInfoArtigo(newLine, TpLinhaOficinas.ServInternos, ref searchItem);
                    newLine.DescArtigo = lineInternalService.ItemDescription;

                    if (myRepairOrder.Cabecalho.IvaIncluido)
                    {
                        lineInternalService.UnitPriceExcludedVAT = lineInternalService.UnitPriceExcludedVAT * (1 + lineInternalService.VATTax / 100.0);
                    }

                    newLine.PrecoUnitario = lineInternalService.UnitPriceExcludedVAT;
                    newLine.Quantidade = lineInternalService.Quantity;

                    myRepairOrder.CalculaTotaisLinha(newLine);
                }

                //serviços externos
                foreach (ExternalServicesLine lineExternalService in pRepairOrder.LinesExternalServices.OrderBy(p => p.LineNumber))
                {
                    newLine = myRepairOrder.NovaLinha(TpLinhaOficinas.ServExternos, true);

                    newLine.CodArtigo = lineExternalService.ItemCode;
                    myRepairOrder.SetInfoArtigo(newLine, TpLinhaOficinas.ServExternos, ref searchItem);
                    newLine.DescArtigo = lineExternalService.ItemDescription;

                    if (myRepairOrder.Cabecalho.IvaIncluido)
                    {
                        lineExternalService.UnitPriceExcludedVAT = lineExternalService.UnitPriceExcludedVAT * (1 + lineExternalService.VATTax / 100.0);
                    }

                    newLine.PrecoUnitario = lineExternalService.UnitPriceExcludedVAT;
                    newLine.Quantidade = lineExternalService.Quantity;

                    myRepairOrder.CalculaTotaisLinha(newLine);
                }


                var validate = myRepairOrder.Validate(true, ref lstMsg, 0);
                if (validate)
                {
                    Eti.Aplicacao.MovimentosAutoGest.MovOrdensReparacao.Update(myRepairOrder, 0);
                }

                if (!string.IsNullOrEmpty(myRepairOrder.EtiErrorCode))
                {
                    result.ErrorDescription = myRepairOrder.EtiErrorDescription;
                }
                else
                {
                    result = new DocumentResult()
                    {
                        ErrorDescription = myRepairOrder.EtiErrorDescription,
                        DocumentKey = new Helpers.DocumentKey()
                        {
                            SectionCode = myRepairOrder.Cabecalho.CodSeccao,
                            DocTypeAbbrev = myRepairOrder.Cabecalho.AbrevTpDoc,
                            FiscalYear = myRepairOrder.Cabecalho.CodExercicio,
                            Number = myRepairOrder.Cabecalho.Numero
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