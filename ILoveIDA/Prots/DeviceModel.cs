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
        // 变量
        public List<DeviceVal> Vals;

        public Dictionary<string, object> ValList = new Dictionary<string, object>();
        public Dictionary<string, object> ValLists = new Dictionary<string, object>();

        public bool IsReg;
        public TestClient Client;

        public void SetVal(DeviceModel device)
        {
            this.Name = device.Name;
            this.IP = device.IP;
            this.Port = device.Port;

            this.Type = device.Type;
            this.Timeout = device.Timeout;
            this.Cycle = device.Cycle;
        }
        public List<DeviceData> Datas;
    }
    // 变量
    public class DeviceData
    {
        // 报文名称
        public string SendPort;
        // 类型
        public Dictionary<string, object> Params;
        public List<DeviceRecvVal> Vals;

    }

    // 变量
    public class DeviceVal
    {
        // 名称
        public string Name;
        // 类型
        public string Type;
    }
    // 接收报文变量
    public class DeviceRecvVal
    {
        public int Index;
        public int Len;
        public string Type;
        public string Val;
    }
}
