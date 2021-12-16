
using Eticadata.Common;
using Eticadata.Cust.Executable.Helpers;
using Eticadata.Cust.Executable.Models;
using Eticadata.ERP;
using Eticadata.ERP.EtiEnums;
using Eticadata.Infrastruct;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
                SQLServerName = @"PT-ALFREDOA\ETICADATA",
                SQLUser = "sa",
                SQLPassword = "Pl@tinum",
                SystemDatabase = "SIS_CUST19",
                Login = "demo",
                Password = "1",
                Company = "T19PT",
                FiscalYearCode = "EX 2021",
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
                MovVenda venda = etiApp.Movimentos.MovVendas.GetNew("FACT");

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

                var validateVenda = venda.Validate(true);

                if (!validateVenda)
                {
                    Console.WriteLine(venda.EtiErrorDescription);
                } else {
                    bool blnSTKImpeditivo = false;
                    etiApp.Movimentos.MovVendas.Update(ref venda, ref blnSTKImpeditivo, true, TpLigacaoExtra.SemLigacao, string.Empty);
                    if(venda.EtiErrorCode != "")
                    {
                        Console.WriteLine(venda.EtiErrorDescription);
                    }
                    Helpers.Functions.PrintToPrinter(etiApp, venda.Cabecalho.CodExercicio, venda.Cabecalho.CodSeccao, venda.Cabecalho.AbrevTpDoc, venda.Cabecalho.Numero);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }
    }
}
