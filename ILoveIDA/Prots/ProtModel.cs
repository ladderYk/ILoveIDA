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
        // 握手报文列表
        public List<string> Hands;
        // 变量
        public List<ProtVal> Vals;
        // 发送报文
        public List<ProtSend> SendProts;
        // 返回报文
        public List<ProtRecv> RecvProts;
        // 通过名称查询发送报文
        public ProtSend FindSendByName(string name)
        {
            return SendProts.FirstOrDefault(send => send.Name == name);
        }
        // 通过名称查询接收报文
        public ProtRecv FindRecvByName(string name)
        {
            return RecvProts.FirstOrDefault(recv => recv.Name == name);
        }
    }

    // 变量
    public class ProtVal
    {
        // 名称
        public string Name;
        // 类型
        public string Type;
    }

    // 发送报文
    public class ProtSend
    {
        // 名称
        public string Name;
        // 报文数据
        public List<string[]> Datas;
        // 接收报文
        public string Recv;
        // 报文参数
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
    // 报文参数
    public class ProtSendParam
    {
        // 下标
        public int Index;
        // 长度
        public int Len;
        // 类型
        public string Type;
        // 名称
        public string Name;
    }
    // 接收报文
    public class ProtRecv
    {
        // 名称
        public string Name;
        // 长度
        public int Len;
        // 处理
        public List<ProtRecvData> Datas;
    }
    // 处理接收报文
    public class ProtRecvData
    {
        public int Index;
        public int Len;
        public string Type;
        public List<string> Anal;
    }
}
