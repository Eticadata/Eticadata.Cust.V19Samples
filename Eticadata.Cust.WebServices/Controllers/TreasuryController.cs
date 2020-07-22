using Eticadata.Cust.WebServices.Models.DepositReceipt;
using Eticadata.Cust.WebServices.Models.Treasury;
using Eticadata.ERP;
using System;
using System.Linq;
using System.Web.Http;

namespace Eticadata.Cust.WebServices.Controllers
{
    public class TreasuryController : ApiController
    {
        //var input = new InputDepositReceipt()
        //{
        //    Currency = "EUR",
        //    StartDate = DateTime.Now,
        //    EndDate = DateTime.Now,
        //    FiscalPeriod = "2020",
        //    Notes = "Teste",
        //    SectionCode = "1",
        //    TargetAccount = "BPI",
        //    SourceAccount = "Caixa",
        //};
        //  CreateDepositReceipt(EtiApp, input);

        [HttpPost]
        [Authorize]
        public IHttpActionResult CreateNewAccount([FromBody] InputDepositReceipt input)
        {

            MovTesTalao receipt;
            Eticadata.ERP.EtiRecordset realTransactions;
            short transactionNumbersGenerate = 0;
            string errorDescription = string.Empty;
            byte filterByDate = 0;
            string filterAdditional = "";
            DateTime startDate = input.StartDate;
            DateTime endDate = input.EndDate;
            string currency = input.Currency;
            string sourceAccount = input.SourceAccount;
            string sectionCode = input.SectionCode;
            short NumMovPiscados = 0;
            EtiAplicacao etiApp = Eti.Aplicacao;

            try
            {
                //double totCheques = etiApp.Movimentos.MovTesTaloes.TotalValores(ref strContaOrigem, ref strMoeda);
                //double totNum = etiApp.Movimentos.MovTesTaloes.TotalNumerario(ref strContaOrigem, ref strMoeda);

                receipt = etiApp.Movimentos.MovTesTaloes.GetNew(input.TargetAccount, input.SectionCode);
                receipt.CodigoSeccao = input.SectionCode;
                receipt.CodigoExercicio = input.FiscalPeriod;
                receipt.CodigoContaTesOri = input.SourceAccount;
                receipt.DataInicial = System.DateTime.Today;
                receipt.DataFinal = System.DateTime.Today;
                receipt.DiscriminaValCheque = false;
                receipt.Obs = input.Notes;
                receipt.Cambio = 1;

                realTransactions = etiApp.Movimentos.MovTesTaloes.RcSetMovReais(ref currency, ref sourceAccount, ref filterByDate, ref startDate, ref endDate, ref filterAdditional, ref filterAdditional, ref sectionCode);
                if (realTransactions.RecordCount > 0)
                {
                    //Para efeitos de testes, apenas se insere um elemento à lista do tipo cheque              
                    Eticadata.ERP.Lista lst = new Eticadata.ERP.Lista();
                    Eticadata.ERP.ListaElem lstElem;

                    lstElem = lst.NewElem();
                    lstElem.Add("strcodseccaomov", sectionCode);
                    lstElem.Add("lngnummov", realTransactions.ValueGet<int>("intnumero").ToString());
                    lstElem.Add("lngtalaonum", receipt.Numero.ToString());
                    lstElem.Add("valor", realTransactions.ValueGet<double>("fltvalor").ToString());
                    lstElem.Add("strrubricamov", "");
                    lstElem.Add("strdescricao", "");
                    lstElem.Add("strdocumento", "Cheque Caixa (EUR)");
                    lstElem.Add("strbanco", "");
                    lstElem.Add("tpentidade", "0");
                    lstElem.Add("lngcodentidade", "0");
                    lstElem.Add("strentidadectbanc", "");
                    lstElem.Add("strmyupdatestamp", Eticadata.ERP.UpdateStamp.ConvertBytesToLong(realTransactions.ValueGet<byte[]>("myupdatestamp")).ToString());
                    lst.AddBefore(lstElem, 1);


                    receipt.ListaMovsPiscados = lst;
                    receipt.ValoresCheques = realTransactions.ValueGet<double>("fltvalor");
                    receipt.Numerario = 0;
                    receipt.TotalDeposito = receipt.ValoresCheques + receipt.Numerario;

                    // 'Calcular o nº de movimentos a gerar
                    if (receipt.Numerario > 0)
                    {
                        transactionNumbersGenerate += 1;
                    }
                    if (receipt.ValoresCheques > 0)
                    {
                        transactionNumbersGenerate += 1;
                    }
                    if (!receipt.DiscriminaValCheque) //'Não discrimina valores/cheques
                    {
                        if (receipt.TotalDeposito > 0)
                        {
                            transactionNumbersGenerate += 1;
                        }
                    }
                    else
                    {
                        //'Discriminando os Valores/Cheques
                        transactionNumbersGenerate += NumMovPiscados;
                        if (receipt.Numerario > 0)
                        {
                            transactionNumbersGenerate += 1;
                        }
                    }
                    receipt.NumMovsReaisAGerar = transactionNumbersGenerate;

                    //Criar Array de movimentos reais a gerar
                    errorDescription = receipt.GeraMovsReaisTaloes(currency);
                    if (errorDescription != "")
                    {
                        throw new Exception(errorDescription);
                    }
                    else
                    {
                        //Gravar Talão
                        etiApp.Movimentos.MovTesTaloes.Update(ref receipt);
                        if (receipt.EtiErrorDescription != "")
                        {
                            throw new Exception(receipt.EtiErrorDescription);
                        }
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //var lines = new List<WithdrawalTransferLines>();
        //var line1 = new WithdrawalTransferLines()
        //{
        //    PayTransfType = "CHQ",
        //    AccountCode = "Caixa",
        //    Currency = "EUR",
        //    ValueDeb = 300,
        //};
        //lines.Add(line1);

        //            var line2 = new WithdrawalTransferLines()
        //            {
        //                PayTransfType = "CHQ",
        //                AccountCode = "CaixaB",
        //                Currency = "EUR",
        //                ValueDeb = 200,
        //            };
        //lines.Add(line2);

        //            var input = new InputWithdrawalTransfer()
        //            {
        //                SectionCode = "1",
        //                AccountCode = "Caixa",
        //                Currency = "EUR",
        //                PayTransfType = "CHQ",
        //                Lines = lines,
        //            };
        //CreateWithdrawalTransfer(EtiApp, input);
        [HttpPost]
        [Authorize]
        public IHttpActionResult CreateWithdrawalTransfer([FromBody] InputWithdrawalTransfer input)
        {

            MovTesLevantamento withdrawalTransfer;
            EtiAplicacao etiApp = Eti.Aplicacao;

            try
            {
                //Criar um movimento real na conta origem, com natureza a crédito              
                withdrawalTransfer = etiApp.Movimentos.MovTesLevantamentos.GetNew(input.AccountCode, input.SectionCode);

                withdrawalTransfer.ContaOrigem.AlteraCodigoContaTes(input.AccountCode);
                withdrawalTransfer.ContaOrigem.AbrevTpMovPag = input.PayTransfType;
                withdrawalTransfer.ContaOrigem.AbrevMoeda = input.Currency;
                withdrawalTransfer.ContaOrigem.Cambio = 1;
                withdrawalTransfer.ContaOrigem.Valor = input.Lines.Sum(s => s.ValueDeb);

                var numberLine = 1;
                foreach (var line in input.Lines)
                {
                    withdrawalTransfer.AlteraDestinoConta(numberLine, line.AccountCode);
                    withdrawalTransfer.AlteraDestinoTipoMovPag(numberLine, line.PayTransfType);
                    withdrawalTransfer.AlteraDestinoValor(numberLine, line.ValueDeb);

                    numberLine++;
                }
                                
                etiApp.Movimentos.MovTesLevantamentos.Update(withdrawalTransfer);
                if (withdrawalTransfer.EtiErrorDescription != "")
                {
                    throw new Exception(withdrawalTransfer.EtiErrorDescription);
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