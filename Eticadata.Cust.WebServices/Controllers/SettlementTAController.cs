using Eticadata.Cust.WebServices.Helpers;
using Eticadata.Cust.WebServices.Models.Settlements;
using Eticadata.ERP;
using System;
using System.Reflection;
using System.Web.Http;

namespace Eticadata.Cust.WebServices.Controllers
{
    public class SettlementTAController : ApiController
    {
        [HttpPost]
        [Authorize]
        public IHttpActionResult GenerateSettlement([FromBody] mySettlement pSettlement)
        {
            MovLiquidacao mySettlement;
            var result = new DocumentResult();

            var errorDescription = "";

            try
            {
                mySettlement = Eti.Aplicacao.Movimentos.MovLiquidacoes.GetNew(pSettlement.DocTypeAbbrev, pSettlement.SectionCode);

                mySettlement.Cabecalho.CodEntidade = 1;
                mySettlement.AlteraEntidade(mySettlement.Cabecalho.CodEntidade, true);
                mySettlement.FindDocs(0, 0, 0, 1);

                //mySettlement.Cabecalho.set_CampoAdicional(Constants.CA_LIQ_Auto, true);

                var linePend = mySettlement.get_LinhaOfPendente(pSettlement.PendingDocument.SectionCode, pSettlement.PendingDocument.DocTypeAbbrev, pSettlement.PendingDocument.FiscalYear, pSettlement.PendingDocument.Number);
                if (linePend.ValorPendente > 0)
                {
                    //atualizar linha de liquidação
                    var lineSettlement = mySettlement.get_LinhaOfDocumento(linePend.CodSeccaoPend, linePend.AbrevTpDocPend, linePend.CodExercicioPend, linePend.NumeroPend);

                    short IRS = 0;
                    mySettlement.AlteraConfirmacao(lineSettlement.NumLinha, true, ref IRS);
                                        
                    linePend.Confirmacao = 1;

                    var validate = mySettlement.Validate();
                    if (validate)
                    {
                        Eti.Aplicacao.Movimentos.MovLiquidacoes.Update(ref mySettlement);
                    }

                    if (!string.IsNullOrEmpty(mySettlement.EtiErrorCode))
                    {
                        result.ErrorDescription = mySettlement.EtiErrorDescription;
                    }
                    else
                    {
                        result = new DocumentResult()
                        {
                            ErrorDescription = mySettlement.EtiErrorDescription,
                            DocumentKey = new Helpers.DocumentKey()
                            {
                                SectionCode = mySettlement.Cabecalho.CodSeccao,
                                DocTypeAbbrev = mySettlement.Cabecalho.AbrevTpDoc,
                                FiscalYear = mySettlement.Cabecalho.CodExercicio,
                                Number = mySettlement.Cabecalho.Numero
                            }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                errorDescription = string.Format("{0}.{1}.{2}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name, ex.Message);
                return BadRequest(errorDescription);
            }

            return Ok(result);
        }
    }
}