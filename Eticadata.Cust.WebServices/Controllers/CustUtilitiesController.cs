using Eticadata.Cust.WebServices.Models.Utilities;
using Eticadata.ERP;
using Eticadata.ERP.EtiEnums;
using Eticadata.Views.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using static Eticadata.ERP.Etiquetas;

namespace Eticadata.Cust.WebServices.Controllers
{
    public class CustUtilitiesController : ApiController
    {
        [HttpPost]
        [Authorize]
        public IHttpActionResult PrintReport([FromBody] PrintInput docParams)
        {
            try
            {
                Eticadata.Views.Reports.ReportsGcePOS report = new Views.Reports.ReportsGcePOS(Eti.Aplicacao, "", ERP.EtiEnums.ExportWebFormat.PDF);
                EtiAplicacao etiapp = new EtiAplicacao();

                byte[] reportBytes;

                var rptProp = new Eticadata.Common.EtiReportProperties()
                {
                    TpDocAEmitir = TpDocumentoAEmitir.Vendas,
                    AbrevTpDoc = docParams.DocType,
                    CodExercicio = docParams.DocFiscalYearCode,
                    CodSeccao = docParams.DocSeccion,
                    Numero = docParams.DocNumber,
                    EtiApp = Eti.Aplicacao,
                    ExportaFicheiro = false,
                    SoExportacao = false,
                    ToPrinter = true,
                    IncrementPrintCount = true,
                    FrontOffBackOff = ReportApplication.BackOffice,
                    ExportaFormato = "1"
                };

                reportBytes = report.EmiteDocumentos(rptProp);

                if (!string.IsNullOrEmpty(rptProp.ErrorDescription))
                {
                    return BadRequest(rptProp.ErrorDescription);
                }

                return Ok(reportBytes);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}