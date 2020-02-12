
using Eticadata.Common;
using Eticadata.Cust.Executable.Helpers;
using Eticadata.Cust.Executable.Models;
using Eticadata.ERP;
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
                serviceAddress = IniFile.IniRead(Path.Combine(IniFile.GetBasePath(), "ERPv17.eInic.ini"), "Geral", "ServerUrl", ""),
                SQLServerName = @"DDS-VICTORG\SQL2014",
                SQLUser = "sa",
                SQLPassword = "platinum",
                SystemDatabase = "sisCust",
                Login = "demo",
                Password = "demo",
                Company = "Cust",
                FiscalYearCode = "2018",
                SectionCode = "1",
                Language = "pt-PT"
            };

            try
            {
                //Obter o objeto EtiAplicacão. è necessário ter licença extended ou modulo de customziação
                EtiAplicacao etiApp = Functions.GetNewEtiAplicacao(authentication);
                //Tendo etiApp podemos utilziar a api do ERP

                //Chamar um webService, necessitando de inicilizar o etiAplicacao por webService
                List<EntitiesCategory> entitiesCategory = Functions.GetEntitiesCategory(authentication);
            }
            catch (Exception ex)
            {
                //tratar erro
                //escrever lo log                
            }
        }
    }
}
