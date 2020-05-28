using Eticadata.ERP;
using Eticadata.RiaServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace Eticadata.Cust.WebServices.Controllers
{
    public class QueryController : ApiController
    {

        [Authorize]
        [HttpGet]
        public IHttpActionResult GetFinancialData(String ID, string Parameters)
        {
            JArray result = new JArray();
            try
            {
                string[] body = Parameters.Replace("*", "%").Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", "").Replace("'", "").Replace("\"", "").Split(';');
                dynamic filter = new JObject();
                
                filter.tipoExportacao = Eticadata.ERP.EtiEnums.QExportType.Excel;
                filter.idConsulta = ID;
                filter.firstCall = false;
                filter.paramValores = JArray.FromObject(body);
                filter.initParams = "";
                filter.pagingInfo = "{'pageSize':0,'pageIndex':0}";
                filter.filteringInfo = new JArray();
                filter.sortingInfo = new JArray();
                filter.hidingInfo = "{'hiddenKeys':'','showedKeys':''}";
                filter.sizingInfo = "";

                var query = RequestHelper.Request(serverURL + "/api/queries/GetConsultaInfo", "POST", filter);
                if (query != null && query.consultaResult != null)
                {
                    result = query.consultaResult.dataSource;
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