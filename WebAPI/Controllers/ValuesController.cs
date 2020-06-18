using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }



        /// <summary>
        /// Делаем web push уведомление через Firebase
        /// </summary>
        /// <returns>Возвращаем ответ типа string</returns>
        [Route("api/Values/PostNotification")]
        [HttpPost]
        public async Task<string> PostNotification(Notification notifi)
        {
           
            Logger logger = LogManager.GetCurrentClassLogger();
            logger.Info("Вызов PostNotification");

            if (string.IsNullOrEmpty(notifi.Token) || string.IsNullOrEmpty(notifi.Body) || string.IsNullOrEmpty(notifi.Title))
            {
                logger.Error($"Заполните Token{0} body{1} title{2}", notifi.Token, notifi.Body, notifi.Title);
                return "Заполните Token,body,title";
            }

            try
            {
                logger.Info("Попытка отправки уведомления");
                var auth = ConfigurationManager.AppSettings["Authorization"];
                var Sender = ConfigurationManager.AppSettings["Sender"];
                var priority = ConfigurationManager.AppSettings["priority"];

                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                //serverKey - Key from Firebase cloud messaging server  
                tRequest.Headers.Add(string.Format("Authorization: key={0}", auth));
                //Sender Id - From firebase project setting  
                tRequest.Headers.Add(string.Format("Sender: id={0}", Sender));
                tRequest.ContentType = "application/json";
                var payload = new
                {
                    to = notifi.Token,
                    priority = priority,
                    content_available = true,
                    notification = new
                    {
                        body = notifi.Body,
                        title = notifi.Title,
                        icon = notifi.Icon,
                        image = notifi.Image,
                        click_action = notifi.Url,
                        badge = 1
                    }
                    ,
                    data = new
                    {
                        body = notifi.Body,
                        title = notifi.Title,
                        icon = notifi.Icon,
                        image = notifi.Image,
                        click_action = notifi.Url,
                        // badge = 1
                    }

                };

                string postbody = JsonConvert.SerializeObject(payload).ToString();


                Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
                tRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = await tRequest.GetRequestStreamAsync())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = await tRequest.GetResponseAsync())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                    var sResponseFromServer = tReader.ReadToEnd();
                                    var ResponceFirebase = JsonConvert.DeserializeObject<FirebaseJson>(sResponseFromServer);
                                    if (ResponceFirebase.success == 1 && ResponceFirebase.failure == 0)
                                    {
                                        logger.Info("Увемдоление отправлено успешно: "+ postbody);
                                        return "OK";
                                       
                                    }
                                    else
                                    {
                                        logger.Error("Ошибка отправки:" + ResponceFirebase.failure);
                                        return "ResponceFirebase.success:" + ResponceFirebase.success + " ResponceFirebase.failure:" + ResponceFirebase.failure;
                                    }
                            }
                            else
                            {
                                logger.Error("dataStreamResponse is null");
                                return "dataStreamResponse is null";
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                logger.Error("Exception: " +ex.InnerException.ToString());
                return ex.Message.ToString();
            }
        }



        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
