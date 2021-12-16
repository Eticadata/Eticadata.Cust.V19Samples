using Eticadata.ERP;
using System;
using System.Collections.Generic;
using System.Linq;
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
                var attach = new Attachment()
                {
                    AttachmentEntities = new List<AttachmentEntity>(){
                                    new AttachmentEntity()
                                    {
                                        TipoEntidade = (int)Eticadata.ERP.EtiEnums.CodTabelas.Clientes,
                                        Chave1 = "1"
                                    }
                                },

                    CodDocumentacao = "BI",
                    DataEmissao = DateTime.Now,
                    CodSituacao = 3,
                    DataSituacao = DateTime.Now,
                    Ficheiro = Convert.FromBase64String(base64Document),
                    WithElectronicSignature = false,
                    IdFicheiro = $"{DateTime.Now.Month}_{DateTime.Now.Year}.txt",
                    Obs = "Documento anexado ao cliente 1",
                    Local = "SEDE",
                    Ref = "0001/2018",
                };

                var validate = Eti.Aplicacao.TablesEntities.Attachments.Validate(attach).ToList();
                if (validate.Count == 0)
                {
                    var b = Eti.Aplicacao.TablesEntities.Attachments.Update(attach, true);
                }
                else
                {
                    var error1 = validate.FirstOrDefault();
                    var Message = error1.Message;
                    if (Message == "")
                    {
                        var lstErro = Eti.Aplicacao.TablesEntities.Attachments.GetErrorList();
                        Message = lstErro.Where(w => w.Key == error1.Code).FirstOrDefault().Value;
                    }

                    return BadRequest(error1.Code + ":" + Message);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
