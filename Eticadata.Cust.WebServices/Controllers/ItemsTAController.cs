using System;
using System.Data;
using System.Reflection;
using System.Web.Http;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Eticadata.Cust.WebServices.Controllers
{
    public class ItemsTAController : ApiController
    {
        private Models.myItem GetNewItem()
        {
            var myFamilies = new List<Models.myFamilies>();
            myFamilies.Add(new Models.myFamilies() { Code = "F1" });
            myFamilies.Add(new Models.myFamilies() { Code = "F3" });

            Models.myItem item = new Models.myItem()
            {
                Code = "ART2",
                Category = "1",
                Description = "Artigo 2",
                Abbreviation = "AR 2",
                VATRateSale = 3,
                VATRatePurchase = 3,
                MeasureOfStock = "UN",
                MeasureOfSale = "UN",
                MeasureOfPurchase = "UN",
                Families = myFamilies,
            };

            return item;
        }


        [HttpPost]
        [Authorize]
        public IHttpActionResult GenerateItem([FromBody] Models.myItem pItem)
        {
            var items = Eti.Aplicacao.Tabelas.Artigos.Clone();
            var errorDescription = "";

            try
            {
                //string output = JsonConvert.SerializeObject(GetNewItem());

                var item = Eti.Aplicacao.Tabelas.Artigos.Find(pItem.Code);

                item.CodigoCategoria = pItem.Category;
                item.Descricao = pItem.Description;
                item.Abreviatura = pItem.Abbreviation;

                item.CodigoTaxaIvaVenda = pItem.VATRateSale;
                item.CodigoTaxaIvaVenda2 = pItem.VATRateSale;
                item.CodigoTaxaIvaCompra = pItem.VATRatePurchase;

                item.AbrevMedStk = pItem.MeasureOfStock;
                item.AbrevMedVnd = pItem.MeasureOfSale;
                item.AbrevMedCmp = pItem.MeasureOfPurchase;

                item.NaoAfectaIntrastat = true;

                if (item.IsNew)
                {
                    item.CodigoInterno = Eti.Aplicacao.Tabelas.Artigos.DaCodigoInterno();
                }

                pItem.Families.ForEach(f =>
                {

                    var familyCode = f.Code;
                    var family = Eti.Aplicacao.Tabelas.Familias.Find(familyCode);                    
                    var newLineNumber = item.get_LinhaCodigoTpNivel(family.CodTpFamilia);

                    item.set_Familia(newLineNumber, familyCode);
                });

                pItem.PricesLines.ForEach(l =>
                {
                    item.set_PrecoNumLin(l.Number, l.SalePrice);
                    item.set_MoedaNumLin(l.Number, l.Currency);
                    item.set_IvaNumLin(l.Number, l.VATIncluded);
                });

                if (item.Validate())
                {
                    items.Update(ref item);
                }

                if (item.EtiErrorDescription != "")
                {
                    errorDescription = $"Erro ao criar o artigo [{item.Codigo} - {item.Descricao}]: {item.EtiErrorDescription}";
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