using Eticadata.ERP;
using Eticadata.ERP.EtiEnums;
using Eticadata.ErpAuthentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Eticadata.Cust.WebServices.Controllers
{
    public class DigitalAttachmentsController : ApiController
    {

        [HttpPost]
        [Authorize]
        public IHttpActionResult SaveAttachment([FromBody] string base64Document)
        {
            try
            {
                Eticadata.RiaServices.AuthenticationService authSvc = new RiaServices.AuthenticationService();
                string strCustomData = Eticadata.LoginData.GetCustomData("(LocalDB)\\MSSQLLocalDB", "sistema", string.Empty, true, true, "pt-PT", string.Empty);
                EtiAplicacao etiApp = new EtiAplicacao();

                var myEtiUser = authSvc.Login("demo", "demo", false, strCustomData);
                if (myEtiUser != null)
                {
                    switch (myEtiUser.loginResult)
                    {
                        case 0:
                            etiApp = Eti.Aplicacao;
                            break;
                        case 2:
                            throw new Exception("The user does not exists!");
                        case 3:
                            throw new Exception("The user is inactive!");
                        default:
                            throw new Exception("The user does not exists!");
                    }
                }

                bool initResult = etiApp.OpenEmpresa("D18");

                if (!initResult)
                    throw new Exception("OpenCompany: Não foi possivel efetuar a autenticação no ERP.");

                initResult = etiApp.OpenExercicio("EX 2018");

                if (!initResult)
                    throw new Exception("OpenFiscalYear: Não foi possivel efetuar a autenticação no ERP.");

                initResult = etiApp.OpenSeccao("SEC1");

                if (!initResult)
                    throw new Exception("OpenSection: Não foi possivel efetuar a autenticação no ERP.");

                RiaServices.Attachments.Services.AnexosDigitaisService attachSrv = new RiaServices.Attachments.Services.AnexosDigitaisService();

                attachSrv.UpdateAnexoDigital(new RiaServices.Attachments.Models.AnexoDigital()
                {
                    Entidades = new System.Data.Entity.Core.Objects.DataClasses.EntityCollection<RiaServices.Attachments.Models.Entidade>() { new RiaServices.Attachments.Models.Entidade()
                    {
                         TipoEntidade = (int)TpEntidade.Cliente,
                         Chave1 = "1"
                    }},
                    TipoDocumentacao = "DOC",
                    CodDocumentacao = "BI",
                    DataEmissao = DateTime.Now,
                    CodSituacao = 0,
                    DataSituacao = DateTime.Now,
                    Ficheiro = Convert.FromBase64String(base64Document),
                    Ref = "0001/2018",
                    Local = "SEDE",
                    Obs = "Documento anexado ao cliente 1"
                });

                return Ok();
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
