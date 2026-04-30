using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILoveIDA.Prots
{
    public class ProtModel
    {
        // 协议名称
        public string Name;
        // 分组
        public string Group;
        // 通讯方式
        public string ConnType;
        // 握手报文
        public List<string> Hands;
        // 变量
        public List<ProtVal> Vals;
        // 发送报文
        public List<ProtSend> SendProts;
        // 返回报文
        public List<ProtRecv> RecvProts;
        public ProtSend FindSendByName(string name)
        {
            return SendProts.FirstOrDefault(send => send.Name == name);
        }
    }

    public class ProtVal
    {
        public string Name;
        public int Len;
        public string Type;
    }

    public class ProtSend
    {
        public string Name;

        public List<string[]> Datas;
        //public int Recv;
        public List<ProtSendParam> Params;
        public byte[] DataToBytes()
        {
            byte[] array = new byte[Datas.Count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = Convert.ToByte(Datas[i].Length>0 ? Datas[i][0]: "0", 16);
            }
            return array;
        }
    }
    public class ProtSendParam
    {
        public int Index;
        public int Len;
        public string Type;
        public string Name;
    }
    public class ProtRecv
    {
        public string Name;
        public int Len;
        public List<ProtRecvData> Datas;
    }
    public class ProtRecvData
    {
        public int Index;
        public int Len;
        public string Type;
        public List<object> Anal;
    }
}
