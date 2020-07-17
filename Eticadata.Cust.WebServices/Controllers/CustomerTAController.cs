using Eticadata.Cust.WebServices.Models.Customers;
using Eticadata.ERP;
using Eticadata.ERP.EtiEnums;
using System;
using System.Collections.Generic;
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

        [HttpGet]
        [Authorize]
        public IHttpActionResult FindCustomer([FromUri] int Code)
        {
            try
            {
                Cliente cliente = Eti.Aplicacao.Tabelas.Clientes.Find(Code);
                myCustomer customer = new myCustomer()
                {
                    Code = Code,
                    Name = cliente.Nome,
                    AddressLine1 = cliente.MoradaLin1,
                    AddressLine2 = cliente.MoradaLin2,
                    PostalCode = cliente.Postal,
                    Locality = cliente.Localidade,
                    Email = cliente.Email,
                    PaymentTerm = cliente.CodCondPag,
                    SubZone = cliente.AbrevSubZona,
                    FiscalId = cliente.NumContrib,
                    GenerateNewCode = cliente.IsNew ? true : false
                };

                return Ok(customer);

            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [Authorize]
        public IHttpActionResult FindCustomers([FromBody] string codes)
        {
            try
            {
                string[] _codes = codes.Split(',');
                List<myCustomer> customers = new List<myCustomer>();
                foreach (var _code in _codes)
                {
                    Cliente cliente = Eti.Aplicacao.Tabelas.Clientes.Find(int.Parse(_code));
                    myCustomer customer = new myCustomer()
                    {
                        Code = int.Parse(_code),
                        Name = cliente.Nome,
                        AddressLine1 = cliente.MoradaLin1,
                        AddressLine2 = cliente.MoradaLin2,
                        PostalCode = cliente.Postal,
                        Locality = cliente.Localidade,
                        Email = cliente.Email,
                        PaymentTerm = cliente.CodCondPag,
                        SubZone = cliente.AbrevSubZona,
                        FiscalId = cliente.NumContrib,
                        GenerateNewCode = cliente.IsNew ? true : false
                    };

                    customers.Add(customer);
                }



                return Ok(customers);

            }
            catch (Exception)
            {

                throw;
            }
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
                customer.NumContrib = pCustomer.FiscalId;
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

            return Ok(pCustomer);
        }
    }
}