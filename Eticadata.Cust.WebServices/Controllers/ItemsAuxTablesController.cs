using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Eticadata.Cust.WebServices.Controllers
{
    [Authorize]
    public class ItemsAuxTablesController : ApiController
    {

        #region Units

        public class Unit
        {
            public string Code { get; set; }
            public string Description { get; internal set; }
            public int DecimalPlaces { get; internal set; }
            public string Formula { get; internal set; }
            public bool StandardFormula { get; internal set; }
            public int Mesures { get; internal set; }
            public byte ProjEquType { get; internal set; }
            public double ProjEquValue { get; internal set; }
        }

        [HttpGet]
        public IHttpActionResult GetUnit(string unitCode)
        {
            var etiApp = Eti.Aplicacao;

            var unidade = etiApp.Tabelas.Unidades.Find(unitCode);

            return Ok(new Unit
            {
                Code = unidade.Abreviatura,
                Description = unidade.Descricao,
                DecimalPlaces = unidade.CasasDecimais,
                Formula = unidade.Formula,
                StandardFormula = unidade.FormulaPadrao,
                Mesures = unidade.Medidas,
                ProjEquType = unidade.TipoEquivalenciaGop,
                ProjEquValue = unidade.ValorEquivalenciaGop
            });
        }

        [HttpPost]
        public IHttpActionResult SetUnit([FromBody]Unit unit)
        {
            var etiApp = Eti.Aplicacao;

            var unidades = etiApp.Tabelas.Unidades;

            var unidade = unidades.Find(unit.Code);

            if (unidade.IsNew)
            {
                //Unit does not exist in DB.
                unidade.Abreviatura = unit.Code;
            }
            else
            {
                //Unit allready in DB.
            }

            unidade.Descricao = unit.Description;
            unidade.CasasDecimais = unit.DecimalPlaces;
            unidade.Formula = unit.Formula;
            unidade.FormulaPadrao = unit.StandardFormula;
            unidade.Medidas = unit.Mesures;
            unidade.TipoEquivalenciaGop = unit.ProjEquType;
            unidade.ValorEquivalenciaGop = unit.ProjEquValue;

            var lst = new List<string>();
            if (unidades.Validate(unidade, lst))
            {
                unidades.Update(unidade);
                if (string.IsNullOrEmpty(unidade.EtiErrorCode))
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(unidade.EtiErrorDescription);
                }
            }
            else
            {
                return BadRequest(unidade.EtiErrorDescription + Environment.NewLine + string.Join(Environment.NewLine, lst));
            }
        }

        #endregion

        #region Units Conversion

        public struct UnitConversion
        {
            public string Unit1Code { get; set; }
            public string Unit2Code { get; set; }
            public double Factor { get; set; }
        }

        [HttpGet]
        public IHttpActionResult GetUnitConversions()
        {
            var etiApp = Eti.Aplicacao;

            var factors = etiApp.Tabelas.Unidades.ListaFactoresConversao();

            var res = new List<UnitConversion>();

            for (var i = 0; i < factors.Count; i++)
            {
                var elem = factors.Elem(i);
                res.Add(new UnitConversion { Unit1Code = elem.Attr("strabrevunidade1").Valor, Unit2Code = elem.Attr("strabrevunidade2").Valor, Factor = Convert.ToDouble(elem.Attr("fltfactor").Valor) });
            }

            return Ok(res);
        }

        [HttpPost]
        public IHttpActionResult SetUnitConversions([FromBody]IEnumerable<UnitConversion> conversionUnits)
        {
            var etiApp = Eti.Aplicacao;
            var units = etiApp.Tabelas.Unidades;

            var factors = units.ListaFactoresConversao();

            var res = new List<UnitConversion>();

            //Insert or Update 
            foreach (var item in conversionUnits)
            {
                var idx = factors.ExistElemVariosAttr(new[] { "strabrevunidade1", "strabrevunidade2" }, new[] { item.Unit1Code, item.Unit2Code });
                if (idx >= 0)
                {
                    factors.Elem(idx).Attr("fltFactor").Valor = item.Factor.ToString();
                }
                else
                {
                    var elem = factors.NewElem();
                    elem.Add("strabrevunidade1", item.Unit1Code);
                    elem.Add("strabrevunidade2", item.Unit2Code);
                    elem.Add("fltfactor", item.Factor.ToString());
                    factors.Add(elem);                    
                }
            }

            //Remove non Existing
            var idxsToRemove = new List<int>();            
            for (var i = 0; i < factors.Count; i++)
            {
                if (!conversionUnits.Any(x => x.Unit1Code == factors.Elem(i).Attr("strabrevunidade1").Valor && x.Unit2Code == factors.Elem(i).Attr("strabrevunidade2").Valor))
                {
                    idxsToRemove.Add(i);
                }
            }
            foreach (var idx in idxsToRemove)
            {
                factors.Del(idx);
            }

            //Validate && Update
            var fieldNumber = 0;
            var msg = string.Empty;
            if (units.ValidateFactoresConversao(ref factors, ref fieldNumber, ref msg))
            {
                units.ActualizaFactoresConversao(ref factors);                
                return Ok();               
            }
            else
            {
                return BadRequest(msg);
            }
        }
        #endregion

        #region ItemFamily

        public class ItemFamily
        {
            public string Code { get; set; }
            public string Description { get; internal set; }
            public int DecimalPlaces { get; internal set; }
            public string Formula { get; internal set; }
            public bool StandardFormula { get; internal set; }
            public int Mesures { get; internal set; }
            public byte ProjEquType { get; internal set; }
            public double ProjEquValue { get; internal set; }
            public string CodTpFamilia { get; internal set; }
            public string LigCte { get; internal set; }
            public string FichaReparticao { get; internal set; }
            public bool IvaIncluido { get; internal set; }
            public bool LimMargemLucro { get; internal set; }
            public int LimMargemLucroTP { get; internal set; }
            public double MargemLucroMin { get; internal set; }
            public bool GranelPreco { get; internal set; }
            public bool GranelPrecoTotal { get; internal set; }
            public string PrefixoPreco { get; internal set; }
            public string PrefixoQtd { get; internal set; }
            public short PrcDigitosInt { get; internal set; }
            public short PrcDigitosDec { get; internal set; }
            public short QtdDigitosInt { get; internal set; }
            public short QtdDigitosDec { get; internal set; }
            public short EdicoesDigTot { get; internal set; }
            public short EdicoesDigEd { get; internal set; }
            public bool PararQtd { get; internal set; }
            public bool PararPreco { get; internal set; }
        }

        [HttpGet]
        public IHttpActionResult GetItemFamily(string ItemFamilyCode)
        {
            var etiApp = Eti.Aplicacao;

            var familia = etiApp.Tabelas.Familias.Find(ItemFamilyCode);

            return Ok(new ItemFamily
            {
                Code = familia.Codigo,
                Description = familia.Descricao,
                CodTpFamilia = familia.CodTpFamilia,
                LigCte = familia.LigCte,
                FichaReparticao = familia.FichaReparticao,
                IvaIncluido = familia.IvaIncluido,
                LimMargemLucro = familia.LimMargemLucro,
                LimMargemLucroTP = familia.LimMargemLucroTP,
                MargemLucroMin = familia.MargemLucroMin,
                GranelPreco = familia.GranelPreco,
                GranelPrecoTotal = familia.GranelPrecoTotal,
                PrefixoPreco = familia.PrefixoPreco,
                PrefixoQtd = familia.PrefixoQtd,
                PrcDigitosInt = familia.PrcDigitosInt,
                PrcDigitosDec = familia.PrcDigitosDec,
                QtdDigitosInt = familia.QtdDigitosInt,
                QtdDigitosDec = familia.QtdDigitosDec,
                EdicoesDigTot = familia.EdicoesDigTot,
                EdicoesDigEd = familia.EdicoesDigEd,
                PararQtd = familia.PararQtd,
                PararPreco = familia.PararPreco
            });
        }

        [HttpPost]
        public IHttpActionResult SetItemFamily([FromBody]ItemFamily ItemFamily)
        {
            var etiApp = Eti.Aplicacao;

            var familias = etiApp.Tabelas.Familias;

            var familia = familias.Find(ItemFamily.Code);

            if (familia.IsNew)
            {
                //ItemFamily does not exist in DB.
                familia.Codigo = ItemFamily.Code;
            }
            else
            {
                //ItemFamily allready in DB.
            }

            familia.Descricao = ItemFamily.Description;
            familia.CodTpFamilia = ItemFamily.CodTpFamilia;
            familia.LigCte = ItemFamily.LigCte;
            familia.FichaReparticao = ItemFamily.FichaReparticao;
            familia.IvaIncluido = ItemFamily.IvaIncluido;
            familia.LimMargemLucro = ItemFamily.LimMargemLucro;
            familia.LimMargemLucroTP = ItemFamily.LimMargemLucroTP;
            familia.MargemLucroMin = ItemFamily.MargemLucroMin;
            familia.GranelPreco = ItemFamily.GranelPreco;
            familia.GranelPrecoTotal = ItemFamily.GranelPrecoTotal;
            familia.PrefixoPreco = ItemFamily.PrefixoPreco;
            familia.PrefixoQtd = ItemFamily.PrefixoQtd;
            familia.PrcDigitosInt = ItemFamily.PrcDigitosInt;
            familia.PrcDigitosDec = ItemFamily.PrcDigitosDec;
            familia.QtdDigitosInt = ItemFamily.QtdDigitosInt;
            familia.QtdDigitosDec = ItemFamily.QtdDigitosDec;
            familia.EdicoesDigTot = ItemFamily.EdicoesDigTot;
            familia.EdicoesDigEd = ItemFamily.EdicoesDigEd;
            familia.PararQtd = ItemFamily.PararQtd;
            familia.PararPreco = ItemFamily.PararPreco;

            var lst = new List<string>();
            if (familias.Validate(familia, lst))
            {
                familias.Update(familia);
                if (string.IsNullOrEmpty(familia.EtiErrorCode))
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(familia.EtiErrorDescription);
                }
            }
            else
            {
                return BadRequest(familia.EtiErrorDescription + Environment.NewLine + string.Join(Environment.NewLine, lst));
            }
        }

        #endregion

        #region Items

        public class Item
        {
            public string Code { get; set; }
            public string Description { get; set; }
            public string Category { get; set; }
            public string Abbreviation { get; set; }
            public int VATRateSale { get; set; }
            public int VATRatePurchase { get; set; }
            public string MeasureOfStock { get; set; }
            public string MeasureOfSale { get; set; }
            public string MeasureOfPurchase { get; set; }

            public List<ItemPrice> Prices { get; set; }

            public List<FamilyAssotiation> Families { get; set; }

            public List<string> SubGroups { get; set; }
        }

        public class ItemPrice
        {
            public int Number { get; set; }
            public double Value { get; set; }
            public DateTime Date { get; set; }
            public double Margin { get; set; }
            public string VATIncluded { get; set; }
            public string Currency { get; set; }
            public double Discount1 { get; set; }
            public double Discount2 { get; set; }
            public double Discount3 { get; set; }
            public double ValueDiscount { get; set; }

        }

        public class FamilyAssotiation
        {
            public string FamilyLevel { get; set; }
            public string FamilyCode { get; set; }
        }

        [HttpGet]
        public IHttpActionResult GetItem(string itemCode)
        {
            var etiApp = Eti.Aplicacao;

            var artigo = etiApp.Tabelas.Artigos.Find(itemCode);

            var item = new Item
            {
                Code = artigo.Codigo,
                Description = artigo.Descricao,
                Category = artigo.CodigoCategoria,
                Abbreviation = artigo.Abreviatura,
                VATRateSale = artigo.CodigoTaxaIvaVenda,
                VATRatePurchase = artigo.CodigoTaxaIvaCompra,
                MeasureOfStock = artigo.AbrevMedStk,
                MeasureOfSale = artigo.AbrevMedVnd,
                MeasureOfPurchase = artigo.AbrevMedCmp,
            };

            item.Prices = new List<ItemPrice>();
            for (var i = 1; i <= artigo.PrecosVenda.CountPrecosVenda(); i++)
            {
                var prc = artigo.PrecosVenda.get_Linha(i);

                item.Prices.Add(new ItemPrice
                {
                    Number = prc.Numero,
                    Value = prc.Preco,
                    Date = prc.Data,
                    Margin = prc.Margem,
                    VATIncluded = prc.Iva,
                    Currency = prc.Moeda,
                    Discount1 = prc.Desconto1,
                    Discount2 = prc.Desconto2,
                    Discount3 = prc.Desconto3,
                    ValueDiscount = prc.DescontoValor
                });
            }

            item.Families = new List<FamilyAssotiation>();
            for (var i = 1; i <= artigo.CountFamilias(); i++)
            {
                if (!string.IsNullOrEmpty(artigo.get_Familia(i)))
                {
                    item.Families.Add(new FamilyAssotiation
                    {
                        FamilyLevel = artigo.get_CodTpNivel(i),
                        FamilyCode = artigo.get_Familia(i)
                    });
                }
            }

            item.SubGroups = artigo.SubGroups.Select(x => x.SubGroup).ToList();

            return Ok(item);
        }

        [HttpPost]
        public IHttpActionResult SetItem([FromBody]Item item)
        {
            var etiApp = Eti.Aplicacao;

            var artigos = etiApp.Tabelas.Artigos;

            var artigo = artigos.Find(item.Code);

            if (artigo.IsNew)
            {
                //Unit does not exist in DB.
                artigo.Abreviatura = item.Code;
            }
            else
            {
                //Unit allready in DB.
            }

            artigo.Codigo = item.Code;
            artigo.Descricao = item.Description;
            artigo.CodigoCategoria = item.Category;
            artigo.Abreviatura = item.Abbreviation;
            artigo.CodigoTaxaIvaVenda = item.VATRateSale;
            artigo.CodigoTaxaIvaCompra = item.VATRatePurchase;
            artigo.AbrevMedStk = item.MeasureOfStock;
            artigo.AbrevMedVnd = item.MeasureOfSale;
            artigo.AbrevMedCmp = item.MeasureOfPurchase;

            for (var i = 1; i <= item.Prices.Count; i++)
            {
                var prc = artigo.PrecosVenda.get_Linha(i);
                var itemprice = item.Prices[i - 1];

                prc.Numero = itemprice.Number;
                prc.Preco = itemprice.Value;
                prc.Data = itemprice.Date;
                prc.Margem = itemprice.Margin;
                prc.Iva = itemprice.VATIncluded;
                prc.Moeda = itemprice.Currency;
                prc.Desconto1 = itemprice.Discount1;
                prc.Desconto2 = itemprice.Discount2;
                prc.Desconto3 = itemprice.Discount3;
                prc.DescontoValor = itemprice.ValueDiscount;
            }
            //Remove extra lines if they exist
            for (int i = artigo.PrecosVenda.CountPrecosVenda(); i > item.Prices.Count; i--)
            {
                artigo.PrecosVenda.RemLinPrecos(i);
            }

            foreach (var familyAssotiation in item.Families)
            {
                var idx = artigo.get_LinhaCodigoTpNivel(familyAssotiation.FamilyLevel);
                artigo.set_Familia(idx, familyAssotiation.FamilyCode);
            }

            artigo.SubGroups.RemoveAll();
            foreach (var subGroup in item.SubGroups)
            {
                var sg = artigo.SubGroups.CreateNew();
                sg.SubGroup = subGroup;
                artigo.SubGroups.Add(sg);
            }

            var field = 0;
            var msg = string.Empty;
            if (artigos.Validate(ref artigo, ref field, ref msg))
            {
                artigos.Update(ref artigo);
                if (string.IsNullOrEmpty(artigo.EtiErrorCode))
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(artigo.EtiErrorDescription);
                }
            }
            else
            {
                return BadRequest(artigo.EtiErrorDescription + Environment.NewLine + "Field:" + field.ToString() + " Msg:" + msg);
            }
        }
        #endregion  

    }
}