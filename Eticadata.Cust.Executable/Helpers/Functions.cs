using Eticadata.Common;
using Eticadata.Cust.Executable.Models;
using Eticadata.ERP;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Eticadata.Cust.Executable.Helpers
{
    public static class Functions
    {
        public static EtiAplicacao GetNewEtiAplicacao(EtiAppAuthentication authentication)
        {
            EtiAplicacao etiApp = new EtiAplicacao();

            try
            {
                if (!etiApp.InitializeEtiApp(EtiConstantes.cAplicBackOffice, authentication.SQLServerName, authentication.SystemDatabase, authentication.SQLUser, authentication.SQLPassword, string.Empty, string.Empty, string.Empty, string.Empty, authentication.serviceAddress, true, string.Empty))
                {
                    throw new Exception("Ocorreu um erro a inicializar etiAplicacao...");
                }

                EtiAplicacao.LoginResult result;
                if ((result = etiApp.Login(authentication.Login, authentication.Password)) != EtiAplicacao.LoginResult.Ok)
                {
                    throw new Exception(result == EtiAplicacao.LoginResult.InvalidUser ? "Invalid user..." : "Wrong user or password...");
                }

                if (!etiApp.OpenEmpresa(authentication.Company))
                {
                    throw new Exception("Empresa inválida ...");
                }

                if (!etiApp.OpenExercicio(authentication.FiscalYearCode))
                {
                    throw new Exception("Exercício inválido...");
                }

                if (!etiApp.OpenSeccao(authentication.SectionCode))
                {
                    throw new Exception("Seção inválida...");
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return etiApp;
        }

        public static List<EntitiesCategory> GetEntitiesCategory(EtiAppAuthentication authentication)
        {
            List<EntitiesCategory> entitiesCategory = new List<Models.EntitiesCategory>(); 
             
            dynamic loginUser = new JObject();
            loginUser.server = authentication.SQLServerName;
            loginUser.sistema = authentication.SystemDatabase;
            loginUser.idioma = authentication.Language;
            loginUser.login = authentication.Login;
            loginUser.password = authentication.Password;
            var login = GetRequest(authentication.serviceAddress + "api/Shell/LoginUser", "POST", loginUser);

            if ((bool)((Newtonsoft.Json.Linq.JValue)login.sucesso).Value)
            {
                dynamic openCompany = new JObject();
                openCompany.reabertura = true;
                openCompany.mostrarJanelaIniSessao = false;
                openCompany.codEmpresa = authentication.Company;
                openCompany.codExercicio = authentication.FiscalYearCode;
                openCompany.codSeccao = authentication.SectionCode;

                var resultOpenCompany = GetRequest(authentication.serviceAddress + "api/Shell/OpenCompany", "POST", openCompany);

                if ((bool)((Newtonsoft.Json.Linq.JValue)resultOpenCompany.sucesso).Value)
                {
                    var resultWS = GetRequest(authentication.serviceAddress + "api/EntitiesCategoryTA/GetEntitiesCategory", "GET", null);
                    var resultBytes = System.Text.Encoding.UTF8.GetBytes(resultWS.ToString());
                    entitiesCategory = Newtonsoft.Json.JsonConvert.DeserializeObject<List<EntitiesCategory>>(System.Text.Encoding.UTF8.GetString(resultBytes));
                }
            }

            return entitiesCategory;
        }


        public static CookieContainer container = new CookieContainer();
        public static dynamic GetRequest(string serverURI, string method, JObject json)
        {
            HttpWebRequest myWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(serverURI));

            myWebRequest.UseDefaultCredentials = true;
            myWebRequest.Method = method;
            myWebRequest.ContentType = "application/json;charset=UTF-8";
            myWebRequest.CookieContainer = container;

            if (method != "GET")
            {
                byte[] postBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(json));
                myWebRequest.ContentLength = postBytes.Length;
                Stream requestStream = myWebRequest.GetRequestStream();
                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Close();
            }

            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
            string result;
            using (StreamReader rdr = new StreamReader(myHttpWebResponse.GetResponseStream()))
            {
                result = rdr.ReadToEnd();
            }

            CookieCollection cookies = myHttpWebResponse.Cookies;
            container.Add(cookies);

            return JsonConvert.DeserializeObject(result);
        }
    }
}
