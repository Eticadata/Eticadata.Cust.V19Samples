using Eticadata.Common;
using Eticadata.ERP.EtiEnums;
using Eticadata.Views.Reports;
using System;

namespace Eticadata.Cust.WebServices.Helpers
{
    public static class Functions
    {
        public static byte[] GetReportBytes(TpDocumentoAEmitir typeDocToPrint, DocumentKey docKey)
        {
            byte[] reportBytes = null;

            try
            {
                ReportsGcePOS report = new ReportsGcePOS(Eti.Aplicacao, string.Empty, ExportWebFormat.PDF);

                var inputParameters = new EtiReportProperties()
                {
                    TpDocAEmitir = typeDocToPrint,

                    AbrevTpDoc = docKey.DocTypeAbbrev,
                    CodExercicio = docKey.FiscalYear,
                    CodSeccao = docKey.SectionCode,
                    Numero = docKey.Number,
                    EtiApp = Eti.Aplicacao,
                    ExportaFicheiro = true,
                    SoExportacao = true,
                    ToPrinter = false,
                    IncrementPrintCount = true,
                    FrontOffBackOff = ReportApplication.BackOffice,
                    ExportaFormato = "1"
                };

               reportBytes = report.EmiteDocumentos(inputParameters);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message); 
            }

            return reportBytes;
        }
    }
}