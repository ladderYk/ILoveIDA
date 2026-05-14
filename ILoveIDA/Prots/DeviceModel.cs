using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ILoveIDA.Prots
{
    public class DeviceModel
    {
        public string Name;

        public string IP;
        public int Port;

        public string Type;
        public int Timeout = 1000;
        public int Cycle = 1000;

        public bool IsConnected;
        public string JsonData;

        public Dictionary<string, object> ValList = new Dictionary<string, object>();
     
        public bool IsReg;
        public TestClient Client;

        public Thread OnlineThread;
        public Thread GetDataThread;

        public void SetVal(DeviceModel device)
        {
            this.Name = device.Name;
            this.IP = device.IP;
            this.Port = device.Port;

            this.Type = device.Type;
            this.Timeout = device.Timeout;
            this.Cycle = device.Cycle;
        }
    }
}
