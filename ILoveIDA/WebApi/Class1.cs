using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ILoveIDA.WebApi
{
    public class Class1 : NancyModule
    {
        public Class1(IRootPathProvider path)
        {
            string curDir = Directory.GetCurrentDirectory();

            After.AddItemToEndOfPipeline((ctx) => ctx.Response
            .WithHeader("Access-Control-Allow-Origin", "*")
            .WithHeader("Access-Control-Allow-Methods", "POST,GET,OPTIONS")
            .WithHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type"));

            Get("/data", d =>
            {
                
                return req("200", "成功",  MainWindow.client.IsConnect? JObject.FromObject(MainWindow.client.ValList): null);
                //return JsonConvert.SerializeObject(new { data = MainWindow.devices });
            });
        }
        private JObject reqBody(string code, string message, JToken data)
        {
            JObject reqData = new JObject();
            reqData.Add("code", code);
            reqData.Add("message", message);
            reqData.Add("data", data);
            return reqData;
        }
        private string req(string code, string message, JToken data)
        {
            return JsonConvert.SerializeObject(reqBody(code, message, data));
        }
    }
}
