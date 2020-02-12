
using Eticadata.Common;
using Eticadata.Cust.DeskTop.Views;
using Eticadata.ERP;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.ObjectBuilder;
using System;
using System.Data;
using System.Windows.Forms;

namespace Eticadata.Cust.DeskTop
{
    //De forma a que ao abrir a empresa entre nesta class será necessário implementar :ModuleInit
    public class Init: Microsoft.Practices.CompositeUI.ModuleInit
    {
        public EtiAplicacao myEtiApp { get; set; }
        public Microsoft.Practices.CompositeUI.WorkItem myWorkItem { get; set; }
        public UIUtils myUiutils { get; set; }


        /// <summary>
        /// Após a abertura da empresa passa por aqui, injetando os objetos de apoio a API já construidos
        /// </summary>
        /// <param name="myWorkItem_"></param>
        /// <param name="myEtiApp_"></param>
        /// <param name="myUiutils_"></param>
        [InjectionConstructor()]
        public Init([ServiceDependency()] WorkItem myWorkItem_,
           [ServiceDependency()] EtiAplicacao myEtiApp_,
           [ServiceDependency()] UIUtils myUiutils_)
        {
            myEtiApp = myEtiApp_;
            myUiutils = myUiutils_;
            myWorkItem = myWorkItem_;
        }


        /// <summary>
        /// Após a abertura da empresa passa por aqui
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [EventSubscription(EventTopics.EmpresaAbertaFim)]
        public void OpenCompany(object sender, EventArgs e)
        {
            //1 - Após a abertura da empresa podemos aproveitar para fazer alguns procedimentos
            //  - Criar/ajustar a BD para a customização
            //  - Substituir uma janela da base desktop por uma customizada, ou seja, editar a janela base  

            //Criar / ajustar a BD para a customização
            UpdatedDatabase();

            //Substituir uma janela da base desktop por uma customizada            
            EditWindow();
        }

        private void EditWindow()
        {
            try
            {
                //CAso se pretenda apenas substituir a janela numa certa condição, exemplo: por utilizador, empresa, baseado em uma janela de permissões, será possivel.                
                //OBS: Se não for substituida será apresentada a janela de base

                //Neste caso apenas é substituida caso o utilziador é "demo"
                if (myEtiApp.ActiveUser.Login == "demo")
                {
                    //Substituir a janela de clientes
                    //Comando da janela base: CommandsEntidades.EditarClientes (pode-se ver através da ribbon)

                    Eticadata.Common.EtiDesignerHost edh = myWorkItem.Services.Get<Eticadata.Common.EtiDesignerHost>();
                    
                    edh.LstAssemblyCust.Add(CommandsEntidades.EditarClientes, System.Reflection.Assembly.GetExecutingAssembly());
                    //Nome do namespace + nome da janela que herda (térá que ser um usercontrol)
                    edh.Classes[CommandsEntidades.EditarClientes] = "Eticadata.Cust.DeskTop.Views.usrCustomers";
                    edh.DesignerHosts[CommandsEntidades.EditarClientes] = String.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdatedDatabase()
        {
            //Coneção á base de dados
            var conn = myEtiApp.ActiveEmpresa.GetConnectionBD();
            var tbl_USR_Version = "USR_Version";
            var tbl_USR_TrainingAction = "USR_TrainingAction";
            var tbl_USR_TrainingActionLine = "USR_TrainingActionLine";
            var intInstalledVersion = 0;

            try
            {
                conn.BeginTrans();

                //Tabela de versão de base de dados
                var qwy = $@"IF NOT EXISTS (SELECT 1 
                                           FROM   sys.tables 
                                           WHERE  NAME = '{tbl_USR_Version}') 
                              BEGIN 
                                  CREATE TABLE {tbl_USR_Version} 
                                    ( 
                                       [intVersion] [int] NOT NULL, 
                                       [dtmModified]  [smalldatetime] NOT NULL 
                                    ) 
                              END";
                conn.Execute(qwy);

                qwy = $@"SELECT TOP 1 intVersion
                        FROM   {tbl_USR_Version} WITH (nolock) 
                        ";
                var dt = conn.GetDataTable(tbl_USR_Version, qwy);

                intInstalledVersion = (dt.Rows.Count == 0) ? 0 : dt.Rows[0].Field<int>("intVersion");
                if (intInstalledVersion == 0)
                {
                    qwy = $@"CREATE TABLE {tbl_USR_TrainingAction} 
	                                       ( intCode        [INT] NOT NULL,
	                                        strDescription [varchar](200) NULL,
                                            dtmDate        [smalldatetime] NOT NULL,
                                            dtmModified    [smalldatetime] NOT NULL ,
                                            strLogin       [VARCHAR](50) NOT NULL,
                                            UpdateStamp    [TIMESTAMP] NOT NULL,
                                            CONSTRAINT [PK_EtiErrorBD_PKTrainingAction00001_TrainingActionInvalid] PRIMARY KEY CLUSTERED ( [intCode] ASC )                                        
                                        )
                                        ON [PRIMARY]";
                    conn.Execute(qwy);

                    qwy = $@"CREATE TABLE {tbl_USR_TrainingActionLine}
	                                     (   intCode        [INT] NOT NULL,
                                            intNumberLine  [INT] NOT NULL,
	                                        strTopic       [varchar](200) NULL,
                                            CONSTRAINT [PK_EtiErrorBD_PKTrainingActionLine00001_TrainingActionLineInvalid] PRIMARY KEY CLUSTERED ( [intCode], [intNumberLine] ASC )                                                                                 
                                        ) ON [PRIMARY]";
                    conn.Execute(qwy);

                    qwy = $@"ALTER TABLE {tbl_USR_TrainingActionLine}
                            WITH CHECK ADD CONSTRAINT[FK_EtiErrorBD_FKTrainingAction00001_TypeInvalid] FOREIGN KEY([intCode]) REFERENCES[dbo].[{tbl_USR_TrainingAction}] ([intCode]) ";
                    conn.Execute(qwy);

                    qwy = $@" INSERT INTO {tbl_USR_Version} (intVersion, dtmModified) 
                                VALUES      (1, 
                                                Getdate()) ";
                    conn.Execute(qwy);
                }

                conn.CommitTrans();
            }
            catch (Exception ex)
            {
                conn.RollBackTrans();
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        #region "Commands - Janela desktop"

        public void ShowWindow<TView>(bool blnModal, bool blnFixed) where TView : UserControl
        {
            if (myWorkItem.Services.Contains<EtiAplicacao>() && myWorkItem.Services.Contains<UIUtils>())
            {
                if (blnFixed)
                    GWorkItem.MostraWorkViewFixedSize<TView>(myWorkItem, blnModal);
                else
                    GWorkItem.MostraWorkView<TView>(myWorkItem, blnModal);
            }
        }

        /// <summary>
        /// Nome do comando, tem se ser único
        /// Ao colocar a nova entrada de menu, será este nome a colocar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [CommandHandler("cmdNewWindow")]
        public void OnShowIntegraDocs(object sender, EventArgs e)
        {
            //Nome do do userControl (nova janela a colocar na ribbon)
            ShowWindow<NewWindow>(false, true);
        }

        //eticadata:Examples.NewTicket/1/1/REPEQ/TITULO
        [CommandHandler("Examples.NewTicket")]
        public void NewTicket(object sender, EventArgs e)
        {
            try
            {
                object globalVar = myWorkItem.RootWorkItem.State[GlobalState.Posicao];

                if (globalVar == null)
                {
                    throw new ArgumentNullException("parameters must be defined.");
                }

                string[] parameters = (string[])globalVar;
                int customerCode = int.Parse(parameters[0]);
                int techCode = int.Parse(parameters[1]);
                string docType = parameters[2];
                string subject = parameters[3];

                Eticadata.ERP.MovAPVReparacao ticket = myEtiApp.MovimentosAPV.MovAPVRecepcoes.GetNew(myEtiApp.ActiveSeccao.Codigo, docType, myEtiApp.ActiveExercicio.Codigo);
                ticket.Cabecalho.CodEntidade = customerCode;
                ticket.AlteraCliente(customerCode);
                //ticket.Cabecalho.Responsavel = techCode;
                ticket.Cabecalho.CodFuncionario = techCode;

                ticket.Cabecalho.Titulo = subject;

                myWorkItem.RootWorkItem.State[GlobalState.Posicao] = new string[] { docType, "0", myEtiApp.ActiveExercicio.Codigo, myEtiApp.ActiveSeccao.Codigo };
                myWorkItem.RootWorkItem.State[GlobalState.PosicaoSingular] = ticket;
                myWorkItem.RootWorkItem.Commands[CommandsAPVMov.EditarReparacao].Execute();

            }
            catch (Exception ex)
            {
                throw new Exception("NewTicket", ex);
            }         
            
        }

        #endregion
    }
}
