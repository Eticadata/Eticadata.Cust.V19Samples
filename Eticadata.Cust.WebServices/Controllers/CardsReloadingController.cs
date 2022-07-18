using Eticadata.ERP;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;
using System.Web.Http;

namespace Eticadata.Cust.WebServices.Controllers
{
    public class InputPrepaidCardsReloading
    {
        public string DocumentType { set; get; }
        public string ReloadItem { set; get; }
        public string Card { set; get; }
        public double Value { set; get; }
    }

    public class CardsReloadingController : ApiController
    {
       
        [Authorize]
        [HttpPost]
        public IHttpActionResult GetFinancialData([FromBody] InputPrepaidCardsReloading input)
        {
            //var input = new InputPrepaidCardsReloading()
            //{
            //    DocumentType = "FACT",
            //    ReloadItem = "CARTOES",
            //    Card = "1",
            //    Value = 10
            //};

            object result = null;

            try
            {
                var byRefFalse = false;
                var myMov = Eti.Aplicacao.Movimentos.MovVendas.GetNew(input.DocumentType);

                myMov.AlteraCarregamentoCartao(1, input.ReloadItem, input.Card, input.Value, true);
                myMov.CalculaTotaisPag();

                if (myMov.Validate(false))
                {
                    Eti.Aplicacao.Movimentos.MovVendas.Update(ref myMov, ref byRefFalse, false, 0, String.Empty);
                }

                if (!string.IsNullOrEmpty(myMov.EtiErrorCode))
                {
                    throw new Exception($@"ErrorCode:{myMov.EtiErrorCode}{Environment.NewLine}EtiErrorDescription:{myMov.EtiErrorDescription}");
                }
            }
            catch (Exception ex)
            {
                string errorDescription = string.Format("{0}.{1}.{2}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name, ex.Message);
                return BadRequest(errorDescription);
            }

            return Ok(result);
        }

    }
}