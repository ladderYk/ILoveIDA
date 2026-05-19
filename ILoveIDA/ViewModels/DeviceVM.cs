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
    public class DeviceVM
    {
        public bool setOn()
        {
            if (MainWindow.IsRun)
                return false;
            MainWindow.IsRun = true;

            foreach (DeviceModel device in MainWindow.Devices)
            {
                ResolveDataUtil.OnGetAGVState(device);
                ResolveDataUtil.OnGetData(device);
            }
            return true;
        }
        public bool setOff()
        {
            if (!MainWindow.IsRun)
                return false;
            MainWindow.IsRun = false;

            foreach (DeviceModel device in MainWindow.Devices)
            {
                ResolveDataUtil.Close(device);
            }
            return true;
        }
        public string getDeviceList()
        {
            return req("200", "", JArray.FromObject(MainWindow.Devices));
        }
        public bool addDevice(string sbody)
        {
            DeviceModel device = JsonConvert.DeserializeObject<DeviceModel>(sbody);
            MainWindow.Devices.Add(device);
            ResolveDataUtil.OnGetAGVState(device);
            ResolveDataUtil.OnGetData(device);

            string jsonfile = MainWindow.DeviceFile;
            JArray jObject = JArray.FromObject(MainWindow.Devices);
            File.WriteAllText(jsonfile, jObject.ToString(Formatting.None));
            return true;
        }
        public bool editDevice(string sbody)
        {
            DeviceModel device = JsonConvert.DeserializeObject<DeviceModel>(sbody);
            int typeIndex = MainWindow.Devices.FindIndex(dev => dev.Name == device.Name);
            if (typeIndex > -1)
            {
                MainWindow.Devices[typeIndex].Client?.Disconnect();
                MainWindow.Devices[typeIndex].SetVal(device);
                string jsonfile = MainWindow.DeviceFile;
                JArray jObject = JArray.FromObject(MainWindow.Devices);
                File.WriteAllText(jsonfile, jObject.ToString(Formatting.None));
                return true;
            }
            return false;
        }
        public string getDeviceData(string name)
        {
            DeviceModel device = MainWindow.Devices.Find(dev => dev.Name == name);
            JArray socketData = new JArray();
            if (device != null)
            {
                socketData.Add(JArray.FromObject(device.ValList));
            }
            return req("200", "", JArray.FromObject(socketData));
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
