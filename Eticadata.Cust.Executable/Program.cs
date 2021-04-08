
using Eticadata.Common;
using Eticadata.Cust.Executable.Helpers;
using Eticadata.Cust.Executable.Models;
using Eticadata.ERP;
using Eticadata.ERP.EtiEnums;
using Eticadata.Infrastruct;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace Eticadata.Cust.Executable
{
    public static class Program
    {
        static void Main(string[] args)
        {
            EtiAppAuthentication authentication = new EtiAppAuthentication()
            {
                serviceAddress = IniFile.IniRead(Path.Combine(IniFile.GetBasePath(), "ERPV19.eInic.ini"), "Geral", "ServerUrl", ""),
                SQLServerName = @"PT-ALFREDOA",
                SQLUser = "sa",
                SQLPassword = "Pl@tinum",
                SystemDatabase = "SIS_CUST19",
                Login = "demo",
                Password = "1",
                Company = "T19PT",
                FiscalYearCode = "EX 2020",
                SectionCode = "1",
                Language = "pt-PT"
            };

            try
            {
                //Obter o objeto EtiAplicacão. è necessário ter licença extended ou modulo de customziação
                EtiAplicacao etiApp = Functions.GetNewEtiAplicacao(authentication);
                //Tendo etiApp podemos utilziar a api do ERP

                //Chamar um webService, necessitando de inicilizar o etiAplicacao por webService
                //List<EntitiesCategory> entitiesCategory = Functions.GetEntitiesCategory(authentication);

                //Criar documento de venda
                MovVenda venda = etiApp.Movimentos.MovVendas.GetNew("FATU");

                venda.Cabecalho.CodEntidade = 1;
                venda.AlteraEntidade(ERP.EtiEnums.TpEntidade.Cliente, 1, true, true);

                int linha = 1;
                string codArtigo = "001";
                bool blnAfectaOutrasLinhas = false, blnAssociacoesFixas = false, blnAssociacoesLivres = false, blnStockDisponivel = false, blnFamPararQtd = false, blnFamPararPreco = false;
                TpProcuraArtigo pProcuraArtigo = TpProcuraArtigo.Encontrou;
                venda.AddLin(ref linha);

                venda.Lines[linha].CodArtigo = codArtigo;
                venda.AlteraArtigo(linha, ref codArtigo, ref blnAfectaOutrasLinhas, ref blnAssociacoesFixas, ref blnAssociacoesLivres, ref pProcuraArtigo, false, ref blnStockDisponivel, ref blnFamPararQtd, ref blnFamPararPreco);

                venda.CalculaTotais();

                venda.Cabecalho.Anulado = true;
                venda.AlteraStatusAnulado(true);
                venda.IntegracaoOffLine = true;
                var validateVenda = venda.Validate(true);

                if (!validateVenda)
                {
                    Console.WriteLine(venda.EtiErrorDescription);
                } else {
                    bool blnSTKImpeditivo = false;
                    etiApp.Movimentos.MovVendas.Update(ref venda, ref blnSTKImpeditivo, true, TpLigacaoExtra.SemLigacao, string.Empty);
                }

                //Criar Recibo
                MovLiquidacao mySettlement = etiApp.Movimentos.MovLiquidacoes.GetNew("REC", "1");
                mySettlement.IntegracaoOffLine = true;
                mySettlement.Cabecalho.Anulado = true;

                mySettlement.Cabecalho.CodEntidade = 1;
                mySettlement.AlteraEntidade(1, true);

                //Obtem venda que está a ser liquidada
                venda = etiApp.Movimentos.MovVendas.Find(venda.Cabecalho.AbrevTpDoc, venda.Cabecalho.CodExercicio, venda.Cabecalho.Numero, venda.Cabecalho.CodSeccao);
                bool anuladoAnt = venda.Cabecalho.Anulado;
                
                //Se o documento de venda estiver anulado, remove o anulado e grava, para poder fazer a ligação
                if (venda.Cabecalho.Anulado)
                {
                    venda.Cabecalho.Anulado = false;
                    venda.AlteraStatusAnulado(false);
                    bool blnSTKImpeditivo = false;
                    etiApp.Movimentos.MovVendas.Update(ref venda, ref blnSTKImpeditivo, true, TpLigacaoExtra.SemLigacao, string.Empty);
                }

                mySettlement.FindDocs(0, 0, 0, 0, 1, false, String.Empty, 1, 1, true, venda.Cabecalho.CodSeccao, venda.Cabecalho.AbrevTpDoc, venda.Cabecalho.CodExercicio, venda.Cabecalho.Numero, 1);
                var linePend = mySettlement.get_LinhaOfPendente(venda.Cabecalho.CodSeccao, venda.Cabecalho.AbrevTpDoc, venda.Cabecalho.CodExercicio, venda.Cabecalho.Numero);
                var lineSettlement = mySettlement.get_LinhaOfDocumento(venda.Cabecalho.CodSeccao, venda.Cabecalho.AbrevTpDoc, venda.Cabecalho.CodExercicio, venda.Cabecalho.Numero);

                short IRS = 0;
                mySettlement.AlteraConfirmacao(lineSettlement.NumLinha, true, ref IRS);
                linePend.Confirmacao = 1;
                
                var validate = mySettlement.Validate();
                if (!validate)
                {
                    Console.WriteLine(mySettlement.EtiErrorDescription); 
                } 
                else
                {
                    etiApp.Movimentos.MovLiquidacoes.Update(ref mySettlement);
                }

                //Se o documento de venda estava anulado antes da liquidação, volta a anular.
                if (anuladoAnt)
                {
                    venda = etiApp.Movimentos.MovVendas.Find(venda.Cabecalho.AbrevTpDoc, venda.Cabecalho.CodExercicio, venda.Cabecalho.Numero, venda.Cabecalho.CodSeccao);
                    venda.Cabecalho.Anulado = true;
                    venda.AlteraStatusAnulado(true);
                    bool blnSTKImpeditivo = false;
                    etiApp.Movimentos.MovVendas.Update(ref venda, ref blnSTKImpeditivo, true, TpLigacaoExtra.SemLigacao, string.Empty);
                }
            }
            catch (Exception ex)
            {
                //tratar erro
                //escrever lo log                
            }
        }
    }
}
