using Eticadata.ERP;
using Eticadata.ERP.EtiEnums;
using Eticadata.Infrastruct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eticadata.Cust.DeskTop.Helpers
{
    public static class Functions
    {

        public static void NewActivity(EtiAplicacao etiApp, string subject, string notes, int customerCode, string activityType, int nature, Byte[] bytesToWrite)
        {
            AutomaticActivityInfo obj = new AutomaticActivityInfo();
            List<RelationInfo> myRelationInfo = new List<RelationInfo>();

            AttachmentInfo Attach = new AttachmentInfo();
            if (bytesToWrite != null)
            {
                Attach.Name = "Test.pdf";
                Attach.File = System.Convert.ToBase64String(bytesToWrite);
            }

            myRelationInfo.Add(new RelationInfo((int)CodTabelas.Clientes, customerCode.ToString(), "", "", ""));
            obj.CodExercise = "";
            obj.CodSection = "";
            obj.AbrevTpDoc = "";
            obj.Number = customerCode;
            obj.Natureza = nature;
            obj.CodTableDoc = -1;
            obj.CodActivityType = activityType;
            obj.Integration = false;
            obj.Message = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(notes));
            obj.CodTableEntity = -1;
            obj.CodEntity = new List<int>();
            obj.Relations = myRelationInfo;
            obj.Subject = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(subject));
            obj.Contacts = new List<ContactInfo>();

            if (bytesToWrite != null)
            {
                obj.Attachments = new List<AttachmentInfo>();
                if (bytesToWrite != null) obj.Attachments.Add(Attach);
            }

            Uri url = new Uri(new Uri(etiApp.Ambiente.ServerUrl), "api/Crm/AutomaticActivity");
            EtiWebClient wc = new EtiWebClient();
            wc.Headers.Add("Content-Type", "application/json");
            string reply = wc.UploadString(url, "POST", obj.Serialize());
        }
    }
}
