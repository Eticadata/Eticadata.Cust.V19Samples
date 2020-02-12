
using Eticadata.Cust.WebServices.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;

namespace Eticadata.Cust.WebServices.Controllers
{
    public class EntitiesCategoryTAController : ApiController
    {

        [HttpGet]
        [Authorize]        
        public IHttpActionResult GetEntitiesCategory()
        {
            List<EntitiesCategory> EntitiesCategory = new List<Models.EntitiesCategory>();
            var conn = Eti.Aplicacao.ActiveEmpresa.GetConnectionBD();

            try
            {
                var qwy = @"SELECT intCodigo    AS [Code],
                                   strDescricao AS [Description]
                            FROM   [Tbl_Categoria_Entidade] WITH (nolock) 
                            ";
                var dt = conn.GetDataTable("Tbl_Categoria_Entidade", qwy);

                EntitiesCategory = dt.AsEnumerable().Select(x => new EntitiesCategory { Code = x.Field<int>("Code"), Description = x.Field<string>("Description") }).ToList();

                return Ok(EntitiesCategory);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            finally
            {
                conn.Close(); 
            } 
        }
    }
}