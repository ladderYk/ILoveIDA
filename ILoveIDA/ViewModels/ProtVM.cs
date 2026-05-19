using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ILoveIDA.Prots;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ILoveIDA.ViewModels
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class ProtVM
    {
        public string getProtList()
        {
            return req("200", "", JArray.FromObject(MainWindow.Prots));
        }
        public bool addProt(string sbody)
        {
            ProtModel device = JsonConvert.DeserializeObject<ProtModel>(sbody);
            MainWindow.Prots.Add(device);

            string jsonfile = MainWindow.ProtsFile;
            JArray jObject = JArray.FromObject(MainWindow.Prots);
            File.WriteAllText(jsonfile, jObject.ToString(Formatting.None));
            return true;
        }
        public bool editProt(string sbody)
        {
            ProtModel device = JsonConvert.DeserializeObject<ProtModel>(sbody);
            int typeIndex = MainWindow.Prots.FindIndex(dev => dev.Name == device.Name);
            if (typeIndex > -1)
            {
                string jsonfile = MainWindow.ProtsFile;
                JArray jObject = JArray.FromObject(MainWindow.Prots);
                File.WriteAllText(jsonfile, jObject.ToString(Formatting.None));
                return true;
            }
            return false;
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
