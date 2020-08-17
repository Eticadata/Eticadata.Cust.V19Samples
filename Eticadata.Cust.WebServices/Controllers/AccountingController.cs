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


                objMov.get_LinhasPlano(Eti.Aplicacao.Ambiente.BaseCodePlan)[line.Number].LinhasCC.Insert(objMov.get_LinhasPlano(Eti.Aplicacao.Ambiente.BaseCodePlan)[line.Number].LinhasCC.Count + 1);

                var lineCC = objMov.get_LinhasPlano(Eti.Aplicacao.Ambiente.BaseCodePlan)[line.Number].LinhasCC[objMov.get_LinhasPlano(Eti.Aplicacao.Ambiente.BaseCodePlan)[line.Number].LinhasCC.Count];

                lineCC.CodePlan = Eti.Aplicacao.Ambiente.BaseCodePlan;
                lineCC.CodigoDiario = objMov.Cabecalho.CodigoDiario;
                lineCC.NumDiario = objMov.Cabecalho.NumDiario;
                lineCC.CodigoExercicio = objMov.Cabecalho.CodigoExercicio;
                lineCC.Conta = objMov.get_LinhasPlano(Eti.Aplicacao.Ambiente.BaseCodePlan)[line.Number].Conta;
                lineCC.Descricao = objMov.get_LinhasPlano(Eti.Aplicacao.Ambiente.BaseCodePlan)[line.Number].DescConta;
                lineCC.DebCre = objMov.get_LinhasPlano(Eti.Aplicacao.Ambiente.BaseCodePlan)[line.Number].DebCre;
                lineCC.Data = objMov.Cabecalho.Data;
                lineCC.Valor = objMov.get_LinhasPlano(Eti.Aplicacao.Ambiente.BaseCodePlan)[line.Number].Valor;
                lineCC.ValorMoeda = objMov.get_LinhasPlano(Eti.Aplicacao.Ambiente.BaseCodePlan)[line.Number].ValorMoeda;
                lineCC.Percentagem = 100.0;

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

        [HttpPost]
        [Authorize]
        public IHttpActionResult CreateNewAccount([FromBody] AccountCust accountData)
        {
            try
            {
                var account = Eti.Aplicacao.Tabelas.PlanoContas.GetNew();
                account.CodigoExercicio = Eti.Aplicacao.ActiveExercicio.Codigo;
                account.CodePlan = Eti.Aplicacao.Ambiente.BaseCodePlan;
                account.Conta = accountData.Code;
                account.Descricao = accountData.Description;
                account.CodFichRepart = accountData.DistributionFile;


                var errors = new List<string>();
                var res = Eti.Aplicacao.Tabelas.PlanoContas.Validate(account, errors);

                if (res)
                {
                    Eti.Aplicacao.Tabelas.PlanoContas.Update(account);

                    if (account.EtiErrorDescription != "")
                    {
                        return BadRequest("Update():" + account.EtiErrorCode + " - " + account.EtiErrorDescription);
                    }
                }
                else
                {
                    return BadRequest("Validate():" + account.EtiErrorCode + " - " + account.EtiErrorDescription);
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