using Eticadata.ERP;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;
using System.Web.Http;

namespace Eticadata.Cust.WebServices.Controllers
{
    public class QueryController : ApiController
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID">BD6CA420-CE6B-42A7-AA04-3815BDE34A4D</param>
        /// <param name="Parameters">1;2017/01/01;2017/10/19</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public IHttpActionResult GetFinancialData(string id, string[] Parameters)
        {
            object result = null;
            try
            {
                var consultaInput = new Queries.Structures.ConsultaInput()
                {
                    tipoExportacao = Eticadata.ERP.EtiEnums.QExportType.Excel,
                    idConsulta = id,
                    firstCall = false,
                    paramValores = Parameters,
                    paramNames = new string[] { },
                    initParams = "",
                    filteringInfo = new Eticadata.Queries.Structures.ConsultaInputFiltering[0] { },
                    sortingInfo = new Eticadata.Queries.Structures.ConsultaInputSorting[0] { },
                    hidingInfo = new Eticadata.Queries.Structures.ConsultaInputHiding()
                    {
                        hiddenKeys = "",
                        showedKeys = "",
                    },
                    sizingInfo = "",
                };

                var consultaInfo = new Eticadata.Services.Admin.QueriesController().GetConsultaInfo(consultaInput);

                if (consultaInfo.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var reply = consultaInfo.Content.ReadAsStringAsync().Result;
                    dynamic obj = JObject.Parse(reply);

                    if (obj != null && obj.consultaResult != null && obj.consultaResult.dataSource != null)
                    {
                        return Ok(obj.consultaResult.dataSource);
                    }
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