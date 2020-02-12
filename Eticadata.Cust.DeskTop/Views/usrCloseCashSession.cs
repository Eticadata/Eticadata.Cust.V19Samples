using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Eticadata.ERP;

namespace Eticadata.Cust.DeskTop.Views
{
    public partial class usrCloseCashSession : Form
    {
        private EtiAplicacao etiApp;
        public usrCloseCashSession(EtiAplicacao etiApp)
        {
            InitializeComponent();
            this.etiApp = etiApp;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {


            int activePOS = etiApp.AmbientePOS.Posto;
            string activeUser = etiApp.ActiveUser.Nome;

            if (etiApp.AmbientePOS.SessaoCaixaMultiposto || etiApp.ActiveUser.SessaoMultiPosto) //If is multisession or not individual session
            {
                activeUser = "-1";
            }

            string activeSession = etiApp.ActiveSeccao.Codigo;
            string activeFiscalYear = etiApp.ActiveExercicio.Codigo;
            string openDocType = etiApp.AmbientePOS.TpDocAberturaCX;
            int intNumber = -1;
            string strNumber = "";
            DateTime date = new DateTime();
            string hour = "";
            bool sessionClosed = false;
            bool permissionEditDocs = false;
            bool provisoryClose = false;
            ERP.EtiEnums.enumSalesComissionType comissionType = ERP.EtiEnums.enumSalesComissionType.None;
            string docComission = "";
            string docTypeClose = etiApp.AmbientePOS.TpDocFechoCX;

            MovCaixas movCaixas = etiApp.Movimentos.get_MovCaixas(string.Empty); //Initialize movs session
            movCaixas.GetSessaoCaixa(activePOS, activeUser, activeSession, activeFiscalYear,
                                    ref openDocType, ref intNumber, ref strNumber, ref date, ref hour, ref sessionClosed,
                                    ref permissionEditDocs, ref provisoryClose, ref comissionType, ref docComission); //Get active session

            MovCaixa movCaixaOpen = movCaixas.Find(openDocType, activeFiscalYear, intNumber, activePOS, activeSession); //Find open session mov
            MovCaixa movCaixaClose = movCaixas.Find(docTypeClose, movCaixaOpen.Cabecalho.CodExercicio, movCaixaOpen.Cabecalho.Numero, movCaixaOpen.Cabecalho.Posto, movCaixaOpen.Cabecalho.CodSeccao); //Find close session mov

            if (movCaixaClose.IsNew) //If cash Session is open...
            {
                movCaixaClose.Cabecalho.Utilizador = movCaixaOpen.Cabecalho.Utilizador;
                movCaixaClose.Cabecalho.Posto = movCaixaOpen.Cabecalho.Posto;
                movCaixaClose.Cabecalho.FechoProvis = false;
                movCaixaClose.SetSessaoCaixa(movCaixaOpen.Cabecalho.AbrevTpDoc, movCaixaOpen.Cabecalho.Numero, movCaixaOpen.Cabecalho.SalesComissionType, movCaixaOpen.Cabecalho.DocComissionType);
                movCaixaClose.Cabecalho.SessaoFechada = true;

                for (int i = 1; i < movCaixaClose.CountLin; i++)
                {
                    if (movCaixaClose.Linha(i).CodContaTesMeioPag == "NUM")
                        movCaixaClose.AlteraValor(i, 50);
                }

                movCaixas.Update(ref movCaixaClose);

                if(!string.IsNullOrEmpty(movCaixaClose.EtiErrorCode)){
                    throw new Exception(movCaixaClose.EtiErrorDescription);
                }
            }
        }
    }
}
