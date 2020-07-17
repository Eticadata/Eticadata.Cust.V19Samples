using Eticadata.ERP;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace Eticadata.Cust.DeskTop.Helpers
{
    internal class RulesClass
    {

        /// <summary>
        /// Caso o email não esteja definido, o utilizador será avisado mas consegue continuar com a gravação
        /// </summary>
        /// <param name="etiApp"></param>
        /// <param name="customer"></param>
        /// <returns></returns>
        internal bool VerifyEmail(EtiAplicacao etiApp, Cliente customer)
        {
            bool res = !String.IsNullOrEmpty(customer.Email);
            
            return res;
        }
    }

    public static class Rules
    {
        public static bool VerifyEmail(EtiAplicacao etiApp, Cliente customer)
        {
            RulesClass e = null;
            bool res = true;

            try
            {
                e = new RulesClass();
                res = e.VerifyEmail(etiApp, customer);
            }
            catch (Exception ex)
            {
                if (Environment.UserInteractive)
                {
                    var errorDescription = string.Format("{0}.{1}.{2}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name, ex.Message);
                    MessageBox.Show(errorDescription);
                }
                else
                {
                    throw ex;
                }
            }

            return res;
        }
    
    }
}
