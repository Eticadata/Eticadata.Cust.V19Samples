using Eticadata.Cust.WebServices.Models.Customers;
using System;
using System.Reflection;
using System.Web.Http;

namespace Eticadata.Cust.WebServices.Controllers
{
    public class CustomerTAController : ApiController
    {
        private myCustomer getNewCustomer()
        {
            myCustomer customer = new myCustomer()
            {
                GenerateNewCode = true,
                Code = 0,
                Name = "Cliente",
                AddressLine1 = "Morada linha 1",
                AddressLine2 = "Morada linha 2",
                Locality = "Localidade",
                PostalCode = "4170-505",
                Email = "email@eticadata.pt",
                SubZone = "BRG",
                PaymentTerm = "1",
            };

            return customer;
        }

        [HttpPost]
        [Authorize]
        public IHttpActionResult GenerateCustomer([FromBody] myCustomer pCustomer)
        {
            var customers = Eti.Aplicacao.Tabelas.Clientes.Clone();
            var errorDescription = "";

            try
            {                
                //pCustomer = getNewCustomer();

                if (pCustomer.GenerateNewCode)
                {
                    pCustomer.Code = customers.GetCodigoLivre();
                } 

                var customer = Eti.Aplicacao.Tabelas.Clientes.Find(pCustomer.Code);

                customer.ConsumidorFinal = true;

                customer.Nome = pCustomer.Name;
                customer.MoradaLin1 = pCustomer.AddressLine1;
                customer.MoradaLin2 = pCustomer.AddressLine2;
                customer.Localidade = pCustomer.Locality;
                customer.Postal = pCustomer.PostalCode;
                customer.Email = pCustomer.Email;
                customer.NumContrib = "502395028";
                customer.AbrevSubZona = pCustomer.SubZone;
                customer.ALteraSubZona(pCustomer.SubZone);

                customer.CodCondPag = pCustomer.PaymentTerm; 

                //No caso de ter regras ou eventos e esses estiverem codificados em dll, essa dll terá de estar na bin do site
                if (customer.Validate())
                {
                    customers.Update(ref customer);
                }

                if (customer.EtiErrorDescription != "")
                {
                    errorDescription = $"Erro ao criar o cliente [{customer.Codigo} - {customer.Nome}]: {customer.EtiErrorDescription}";
                    throw new Exception(errorDescription);
                }
            }
            catch (Exception ex)
            {
                errorDescription = string.Format("{0}.{1}.{2}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name, ex.Message);
                return BadRequest(errorDescription);
            }

            return Ok("");
        }
    }
}