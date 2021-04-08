using Eticadata.Common;
using Eticadata.ERP;
using Eticadata.ERP.EtiEnums;
using Eticadata.Views.Reports;
using System;

namespace Eticadata.Cust.WebServices.Helpers
{
    public static class Functions
    {
        public static byte[] GetReportBytes(EtiAplicacao EtiApp, TpDocumentoAEmitir typeDocToPrint, DocumentKey docKey)
        {
            byte[] reportBytes = null;
            byte[] emailBytes = null;
            bool isCFDoc;

            try
            {
                var inputParameters = new ElectronicSignature.ReportParameters()
                {
                    TpDocEmit = typeDocToPrint,
                    AbrevTpDoc = docKey.DocTypeAbbrev,
                    CodeFiscalYear = docKey.FiscalYear,
                    CodeSection = docKey.SectionCode,
                    Number = docKey.Number,
                    ToPrinter = false
                };

                Eticadata.Reporting.ReportProvider.EmitDocumentAndSendEmail(ref inputParameters, out reportBytes, ref emailBytes, out isCFDoc, EtiApp);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return reportBytes;
        }

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
                    //FrontOffBackOff = ReportApplication.BackOffice,
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