using Eticadata.Cust.WebServices.Models.Accounting;
using Eticadata.ERP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Eticadata.Cust.WebServices.Controllers
{
    public class AccountingController : ApiController
    {
        

        [HttpPost]
        [Authorize]
        public IHttpActionResult CreateNewMov([FromBody] AccountingMov mov)
        {
            MovCte objMov = Eti.Aplicacao.Movimentos.MovCteS.GetNew(mov.Journal);

            objMov.Cabecalho.AbrevMoeda = "EUR";
            objMov.Cabecalho.DataMovimento = mov.Date;
            objMov.Cabecalho.Data = mov.Date.ToString("dd-MM");

            foreach (AccountingMovLines line in mov.Lines)
            {
                objMov.get_LinhasPlano(Eti.Aplicacao.Ambiente.BaseCodePlan)[line.Number].Conta = line.Account;
                objMov.get_LinhasPlano(Eti.Aplicacao.Ambiente.BaseCodePlan)[line.Number].DescConta = line.Description;
                objMov.get_LinhasPlano(Eti.Aplicacao.Ambiente.BaseCodePlan)[line.Number].DebCre = line.Signal.ToString();
                objMov.get_LinhasPlano(Eti.Aplicacao.Ambiente.BaseCodePlan)[line.Number].Valor = line.Value;
            }

            try
            {
                if (objMov.Validate(false, false, false))
                {
                    Eti.Aplicacao.Movimentos.MovCteS.Update(objMov);

                    if (!String.IsNullOrEmpty(objMov.EtiErrorCode))
                    {
                        return BadRequest("Erro de gravação:" + objMov.EtiErrorCode + " - " + objMov.EtiErrorDescription);
                    }
                    else
                    {
                        return Ok();
                    }
                } else { return BadRequest("Erro de gravação:" + objMov.EtiErrorCode + " - " + objMov.EtiErrorDescription); }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}