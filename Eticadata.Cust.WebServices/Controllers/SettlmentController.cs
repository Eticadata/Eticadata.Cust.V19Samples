using Eticadata.ERP;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Eticadata.Cust.WebServices.Controllers
{
    public class SettlmentController : ApiController
    {

        [HttpPost]
        [Authorize]
        public IHttpActionResult GenerateSettlments([FromBody] JObject data)
        {
            EtiConnNetBD etiConn = Eti.Aplicacao.ActiveEmpresa.GetConnectionBD();
            try
            {
                dynamic _data = data;
                DateTime From = _data.From;
                if (From <= DateTime.MinValue)
                {
                    From = (DateTime)SqlDateTime.MinValue;
                }

                JArray _paymts = _data.Payments;
                IEnumerable<SettlementInput> Payments = _paymts.ToObject<List<SettlementInput>>();

                //Alterar estado das encomendas que foram selecionadas;
                foreach (SettlementInput order in Payments.Where(w => w.Pay == true && w.et_DocType == "ORD"))
                {
                    MovEncomenda objOrder = Eti.Aplicacao.Movimentos.MovEncomendasCli.Find(order.et_Doc, order.et_FiscalYear, order.et_DocNumber, order.et_Section);

                    //inserir no historico de dados                    
                    Eticadata.ERP.HistEstadoDocumento statusesHistory = objOrder.HistoricoEstados.CreateNew();
                    statusesHistory.Tabela = 1;//janela de vendas
                    statusesHistory.Login = Eti.Aplicacao.ActiveUser.Login;
                    statusesHistory.Chave1 = objOrder.Cabecalho.CodSeccao;
                    statusesHistory.Chave2 = objOrder.Cabecalho.AbrevTpDoc;
                    statusesHistory.Chave3 = objOrder.Cabecalho.CodExercicio;
                    statusesHistory.Chave4 = objOrder.Cabecalho.Numero.ToString();

                    statusesHistory.CodEstado = configs.OrderPaidState;
                    statusesHistory.DataEstado = DateTime.Now;

                    objOrder.HistoricoEstados.Add(statusesHistory);

                    //atualizar na venda de forma a aparecer em 1ª lugar
                    objOrder.Cabecalho.EstadoDocum = configs.OrderPaidState;
                    objOrder.Cabecalho.DataEstado = statusesHistory.DataEstado;
                    objOrder.Cabecalho.LoginEstado = statusesHistory.Login;
                    objOrder.Cabecalho.DataAlteracaoEstado = statusesHistory.DataEstado;

                    Eti.Aplicacao.Movimentos.MovEncomendasCli.Update(ref objOrder);
                    if (!string.IsNullOrEmpty(objOrder.EtiErrorCode))
                    {
                        throw new Exception(objOrder.EtiErrorDescription);
                    }
                }


                //Emitir liquidação das faturas que foram pagas;
                foreach (SettlementInput invoice in Payments.Where(w => w.Pay == true && w.et_DocType == "INV"))
                {
                    MovLiquidacao objSettlement = Eti.Aplicacao.Movimentos.MovLiquidacoes.GetNew(configs.SettlementDocType, Eti.Aplicacao.ActiveSeccao.Codigo);
                    MovVenda objInvoice = Eti.Aplicacao.Movimentos.MovVendas.Find(invoice.et_Doc, invoice.et_FiscalYear, invoice.et_DocNumber, invoice.et_Section);

                    objSettlement.Cabecalho.CodEntidade = objInvoice.Cabecalho.CodEntidade;
                    objSettlement.AlteraEntidade(objInvoice.Cabecalho.CodEntidade, true);
                    objSettlement.FindDocs(0, 0, 0, 1);

                    var linePend = objSettlement.get_LinhaOfPendente(invoice.et_Section, invoice.et_Doc, invoice.et_FiscalYear, invoice.et_DocNumber);

                    if (linePend.ValorPendente > 0)
                    {
                        var lineSettlement = objSettlement.get_LinhaOfDocumento(linePend.CodSeccaoPend, linePend.AbrevTpDocPend, linePend.CodExercicioPend, linePend.NumeroPend);
                        short IRS = 0;
                        objSettlement.AlteraConfirmacao(lineSettlement.NumLinha, true, ref IRS);
                        objSettlement.AlteraValorAtribuido(lineSettlement.NumLinha, (double)invoice.ep_value, ref IRS);
                        linePend.Confirmacao = 1;

                        int pagLin = 1;
                        objSettlement.LinesPag[pagLin].AbrevTpMovPagTes = configs.SettlementType;

                        var valid = objSettlement.Validate();
                        if (valid)
                        {
                            Eti.Aplicacao.Movimentos.MovLiquidacoes.Update(ref objSettlement);
                        }

                        if (!string.IsNullOrEmpty(objSettlement.EtiErrorCode) || !valid)
                        {
                            throw new Exception(objSettlement.EtiErrorDescription);
                        }
                    }

                }

                DataTable dtPayments = getPendingPayments(etiConn, From);

                return Ok(dtPayments);
            }
            catch (Exception ex)
            {
                etiConn.RollBackTrans();
                return BadRequest(ex.Message);
            }
            finally
            {
                etiConn.Close();
            }
        }

    }
}
