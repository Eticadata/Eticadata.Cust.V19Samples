using Eticadata.ERP;
using Eticadata.ERP.EtiEnums;
using Eticadata.Views.Reports;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace Eticadata.Cust.DeskTop.Helpers
{
    internal class EventsClass
    {

        /// <summary>
        /// Antes de gravar
        /// Guarda se o cliente é novo ou não, de forma a aceder no evento de depois de gravar
        /// </summary>
        /// <param name="etiApp"></param>
        /// <param name="customer"></param>
        internal void SetIsNew(EtiAplicacao etiApp, Cliente customer)
        {
            //Podemos na datarow colcoar uma nova coluna,com valores auxiliares, o equivalente ao uma variável global 
            Eticadata.ERP.modDataLib.ValueSet<bool>(customer.DataRow, "isNewCust", customer.IsNew);
        }

        /// <summary>
        /// Depois de gravar
        /// Neste evento o estado se é novo ou não é sempre não novo, pois já foi gravado, é existente
        /// Com a variável global isNew colocada no atarow no evento antes de gravar, vamos saber se o cliente é novo ou não.
        /// </summary>
        /// <param name="etiApp"></param>
        /// <param name="customer"></param>
        internal void GetIsNew(EtiAplicacao etiApp, Cliente customer)
        {
            //Podemos na datarow colocar uma nova coluna,com valores auxiliares, o equivalente ao uma variável global 
            //è sempre sobre o objeto singular.DataRow
            var isNew = Eticadata.ERP.modDataLib.ValueGet<bool>(customer.DataRow, "isNewCust") ? "novo" : "existente";
        }

        /// <summary>
        /// Depois de gravar a venda
        /// Imprimir o documento de venda alternativo
        /// </summary>
        /// <param name="etiApp"></param>
        /// <param name="sale"></param>
        internal void PrintAlternativeFile(EtiAplicacao etiApp, MovVenda sale)
        {
            ReportsGcePOS report = new ReportsGcePOS(etiApp, "", ExportWebFormat.PDF);

            var rptProp = new Eticadata.Common.EtiReportProperties()
            {
                FrontOffBackOff = ReportApplication.BackOffice,
                PerfilPerifericos = -1,
                TpDocAEmitir = TpDocumentoAEmitir.Vendas,
                ToPrinter = true,
                CodExercicio = sale.Cabecalho.CodExercicio,
                CodSeccao = sale.Cabecalho.CodSeccao,
                AbrevTpDoc = sale.Cabecalho.AbrevTpDoc,
                Numero = sale.Cabecalho.Numero,
                ConfigImpressao = 1,
                Gravacao = false,
                Movimento = 1,
                ReportName = "VNDDocumentoA5.rpt",
                Entidade = sale.Cabecalho.CodEntidade.ToString(),
                EtiApp = etiApp,
            };

            System.Threading.Thread th = new System.Threading.Thread(() => report.EmiteDocumentos(rptProp));
            th.IsBackground = true;
            th.Start();
        }

        /// <summary>
        /// Depois de Gravar Venda
        /// Imprimir um report personalizado
        /// </summary>
        /// <param name="etiApp"></param>
        /// <param name="sale"></param>
        internal void PrintReportPersonalized(EtiAplicacao etiApp, MovVenda sale)
        {
            var errors = "";
            var customerName = "Sou o cliente XPTO";

            var rptProp = new Eticadata.Common.EtiReportProperties()
            {
                FrontOffBackOff = ReportApplication.BackOffice,
                ReportServer_ReportPath = "Consultas",
                WindowTitle = "Titulo do report personalizado",
                ReportName = "USR_TESTE.rdl",
                ToPrinter = true,                 
                EtiApp = etiApp,               
            };

            //Adicionar parametros
            rptProp.AdicionarFormula($"CustomerName = {customerName}");

            System.Threading.Thread th = new System.Threading.Thread(() => Eticadata.Views.Reports.ReportsInit.MapaSimplesEmissao(rptProp, etiApp, null, null, ref errors));
            th.IsBackground = true;
            th.Start();
        }

        /// <summary>
        /// Depois de Gravar cliente
        /// Adicionar uma nova atividade do tipo tarefa
        /// </summary>
        /// <param name="etiApp"></param>
        /// <param name="customer"></param>
        internal void AddNewActivity(EtiAplicacao etiApp, Cliente customer)
        {
            var activityType = "102"; //tarefa
            var subject = "Subject ... ";
            var notes = "Notes ... ";

            Functions.NewActivity(etiApp, subject, notes, customer.Codigo, activityType, (int)NaturezaAtividade.Tarefa, null);
        }
    }

    public static class Events
    {
        public static void SetIsNew(EtiAplicacao etiApp, EtiConnNetBD conn, Cliente customer)
        {
            EventsClass e = null;
            try
            {
                e = new EventsClass();
                e.SetIsNew(etiApp, customer);
            }
            catch (Exception ex)
            {
                if (Environment.UserInteractive)
                {
                    var errorDescription = string.Format("{0}.{1}.{2}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name, ex.Message);
                    MessageBox.Show(errorDescription);
                }
                else
                {
                    throw ex;
                }
            }
            finally
            {
                e = null;
            }
        }

        public static void GetIsNew(EtiAplicacao etiApp, EtiConnNetBD conn, Cliente customer)
        {
            EventsClass e = null;
            try
            {
                e = new EventsClass();
                e.GetIsNew(etiApp, customer);

                Functions.NewActivity(etiApp, "Subject ... ", "Notes...", customer.Codigo, "1", (int)NaturezaAtividade.Tarefa, null);
            }
            catch (Exception ex)
            {
                if (Environment.UserInteractive)
                {
                    var errorDescription = string.Format("{0}.{1}.{2}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name, ex.Message);
                    MessageBox.Show(errorDescription);
                }
                else
                {
                    throw ex;
                }
            }
            finally
            {
                e = null;
            }
        }

        public static void AddNewActivity(EtiAplicacao etiApp, EtiConnNetBD conn, Cliente customer)
        {
            EventsClass e = null;
            try
            {
                e = new EventsClass();

                e.AddNewActivity(etiApp, customer);
            }
            catch (Exception ex)
            {
                if (Environment.UserInteractive)
                {
                    var errorDescription = string.Format("{0}.{1}.{2}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name, ex.Message);
                    MessageBox.Show(errorDescription);
                }
                else
                {
                    throw ex;
                }
            }
            finally
            {
                e = null;
            }
        }

        public static void PrintAlternativeFile(EtiAplicacao etiApp, MovVenda sale)
        {
            EventsClass e = null;
            try
            {
                e = new EventsClass();
                e.PrintAlternativeFile(etiApp, sale);
            }
            catch (Exception ex)
            {
                if (Environment.UserInteractive)
                {
                    var errorDescription = string.Format("{0}.{1}.{2}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name, ex.Message);
                    MessageBox.Show(errorDescription);
                }
                else
                {
                    throw ex;
                }
            }
            finally
            {
                e = null;
            }
        }

        public static void PrintReportPersonalized(EtiAplicacao etiApp, MovVenda sale)
        {
            EventsClass e = null;
            try
            {
                e = new EventsClass();
                e.PrintReportPersonalized(etiApp, sale);
            }
            catch (Exception ex)
            {
                if (Environment.UserInteractive)
                {
                    var errorDescription = string.Format("{0}.{1}.{2}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name, ex.Message);
                    MessageBox.Show(errorDescription);
                }
                else
                {
                    throw ex;
                }
            }
            finally
            {
                e = null;
            }
        }

    }
}
