using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.IO;


namespace GCM_Sender_Service
{
    public class Sender
    {
       
        public bool SendMessage(string API_KEY,string Mge,List<string> redid)
        {      
            List<string> registrationIDList = new List<string>();
            foreach (string rid in redid)
            {
                registrationIDList.Add(rid);
            }
            return HttpPostToGCM(registrationIDList, API_KEY, Mge);
        }
        private bool HttpPostToGCM(List<string> regIds, string API_Key, string message)
        {
            bool result = true;
            string errorMessage = "";

            if (regIds.Count > 0)
            {
                try
                {
                    foreach (var regId in regIds)
                    {
                        //準備對GCM Server發出Http post
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://android.googleapis.com/gcm/send");
                        request.Method = "POST";
                        request.ContentType = "application/json;charset=utf-8;";
                        request.Headers.Add(string.Format("Authorization: key={0}", API_Key));

                        string RegistrationID = regId.ToString();
                        var postData =
                        new
                        {
                            data = new
                            {
                                message = message //Message這個tag要讓前端開發人員知道
                            },
                            registration_ids = new string[] { RegistrationID }
                        };

                        string p = JsonConvert.SerializeObject(postData);//使用Json.Net 將Linq to json轉為字串

                        byte[] byteArray = Encoding.UTF8.GetBytes(p);//要發送的字串轉為byte[]
                        request.ContentLength = byteArray.Length;

                        Stream dataStream = request.GetRequestStream();
                        dataStream.Write(byteArray, 0, byteArray.Length);
                        dataStream.Close();

                        //發出Request
                        WebResponse response = request.GetResponse();
                        Stream responseStream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(responseStream);
                        string responseStr = reader.ReadToEnd();
                        reader.Close();
                        responseStream.Close();
                        response.Close();

                    }//End foreach

                }
                catch (Exception ex)
                {
                    result = false;
                    errorMessage += ex.Message + ",";
                }

            }//End if

            return result;
        }
    }
}
