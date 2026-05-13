using ILoveIDA.Prots;
using Nancy.Hosting.Self;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace ILoveIDA
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<ProtModel> prots = new List<ProtModel>();
        // 根目录地址
        public static string BasePath = AppDomain.CurrentDomain.BaseDirectory;

        public static TestClient client = new TestClient();

        // 配置协议列表
        public static string ProtsFile
        {
            get
            {
                return Path.Combine(BasePath, "Prots", "Config.json");
                //return Path.Combine(BasePath, "Prots", "modbustcp.json");
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            // 解析协议
            using (System.IO.StreamReader file = File.OpenText(ProtsFile))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JArray arrs = (JArray)JToken.ReadFrom(reader);
                    foreach (JObject arr in arrs)
                    {
                        ProtModel prot = arr.ToObject<ProtModel>();
                        prots.Add(prot);
                    }
                }
            }
            var url = $"http://localhost";

            NancyHost host = new NancyHost(new Uri(url));
            host.Start();
        }
        private ProtModel findByName(string name)
        {
            return prots.FirstOrDefault(prot => prot.Name == name);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            client.ConnectServer();
            Thread.Sleep(1000);
            // 根据类型查询报文
            ProtModel model = findByName("S7-1200");
            if (model != null)
            {
                // 判断是否有握手报文
                foreach (string send in model.Hands)
                {
                    ProtSend protSend = model.FindSendByName(send);
                    if (protSend == null)
                    {
                        client.Disconnect();
                        return;
                    }
                    byte[] bys = client.SendBytes(protSend.DataToBytes().ToArray());
                    ProtRecv protRecv = model.FindRecvByName(protSend.Recv);
                    if (protRecv != null)
                    {
                        if (bys.Length != protRecv.Len)
                        {
                            Console.WriteLine("响应长度不足");
                            client.Disconnect();
                            return;
                        }
                        foreach (ProtRecvCond s in protRecv.Conds)
                        {
                            if (s.Anal[0].ToString() == "!=" && bys[s.Index] != Convert.ToByte(s.Anal[1].ToString(), 16))
                            {
                                Console.WriteLine("错误" + s.Anal[2].ToString());
                                client.Disconnect();
                                return;
                            }
                            if (s.Anal[0].ToString() == "==" && bys[s.Index] == Convert.ToByte(s.Anal[1].ToString(), 16))
                            {
                                Console.WriteLine("错误" + s.Anal[2].ToString());
                                client.Disconnect();
                                return;
                            }
                        }
                    }
                    //if (bys.Length != model.RecvProts[0].Len)
                    //{
                    //    Console.WriteLine("响应长度不足");
                    //    client.Disconnect();
                    //    return;
                    //}
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (client.IsConnect)
            {
                ProtModel model = findByName("S7-1200");

                ProtSend protSend = model.FindSendByName("读取db区");
                if (protSend == null)
                {
                    client.Disconnect();
                    return;
                }
                foreach (ProtSendParam param in protSend.Params)
                {

                }
                short addr = 100;
                List<byte> sendByts = protSend.DataToBytes();

                //sendByts.InsertRange(8, BitConverter.GetBytes(addr).Reverse().ToArray());
                // 参数
                sendByts.InsertRange(23, BitConverter.GetBytes((short)2).Reverse().ToArray());

                byte[] bys = client.SendBytes(sendByts.ToArray());
                //.CopyTo(bys, 8);


                ProtRecv protRecv = model.FindRecvByName(protSend.Recv);
                if (protRecv != null)
                {
                    foreach (ProtRecvVal s in protRecv.Vals)
                    {
                        object v = null;
                        if (s.Type == "bool")
                        {
                            // 取数组
                            byte[] vals1 = bys.Skip(s.Index).Take(s.Len / 8 + 1).ToArray();
                            if (vals1.Length < 1)
                                continue;
                            List<bool> bTemp = new List<bool>();

                            for (int i = 0; i < vals1.Length; i++)
                            {
                                char[] c = byteTo8BitArr(vals1[i]);

                                for (int j = 0; j < c.Length; j++)
                                {
                                    bTemp.Add(c[j] == '1');
                                }
                            }

                            if (s.Len > 1)
                            {
                                v = bTemp.Take(s.Len).ToList();
                            }
                            else
                            {
                                v = bTemp[0];
                            }
                        }
                        else
                        {
                            byte[] vals = bys.Skip(s.Index).Take(s.Len).ToArray();
                            if (vals.Length < 1)
                                continue;

                            if (s.Type == "byte")
                            {
                                byte v1 = vals[0];
                                if (vals.Length > 1)
                                {
                                    List<byte> vTemp = new List<byte>();
                                    vTemp.Add(v1);
                                    for (int i = 1; i < vals.Length; i++)
                                    {
                                        vTemp.Add(vals[i]);
                                    }
                                    v = vTemp;
                                }
                                else
                                {
                                    v = v1;
                                }
                            }
                            if (s.Type == "short")
                            {
                                if (vals.Length < 2)
                                    continue;

                                short v1 = BitConverter.ToInt16(vals.Take(2).Reverse().ToArray(), 0);
                                if (vals.Length > 2)
                                {
                                    List<short> vTemp = new List<short>();
                                    vTemp.Add(v1);
                                    for (int i = 1; i < vals.Length / 2; i++)
                                    {
                                        vTemp.Add(BitConverter.ToInt16(vals.Skip(i * 2).Take(2).Reverse().ToArray(), 0));
                                    }
                                    v = vTemp;
                                }
                                else
                                {
                                    v = v1;
                                }
                            }
                            if (s.Type == "ushort")
                            {
                                if (vals.Length < 2)
                                    continue;

                                ushort v1 = BitConverter.ToUInt16(vals.Take(2).Reverse().ToArray(), 0);
                                if (vals.Length > 2)
                                {
                                    List<ushort> vTemp = new List<ushort>();
                                    vTemp.Add(v1);
                                    for (int i = 1; i < vals.Length / 2; i++)
                                    {
                                        vTemp.Add(BitConverter.ToUInt16(vals.Skip(i * 2).Take(2).Reverse().ToArray(), 0));
                                    }
                                    v = vTemp;
                                }
                                else
                                {
                                    v = v1;
                                }
                            }
                            if (s.Type == "int")
                            {
                                if (vals.Length < 4)
                                    continue;
                                // modbustcp ABCD
                                int v1 = BitConverter.ToInt32(vals.Take(4).Reverse().ToArray(), 0);
                                // CDAB
                                //int v1 = BitConverter.ToInt32(vals.Take(2).Reverse().ToArray().Concat(vals.Skip(2).Take(2).Reverse().ToArray()).ToArray(), 0);
                                if (vals.Length > 4)
                                {
                                    List<int> vTemp = new List<int>();
                                    vTemp.Add(v1);
                                    for (int i = 1; i < vals.Length / 4; i++)
                                    {
                                        vTemp.Add(BitConverter.ToInt32(vals.Skip(i * 4).Take(4).Reverse().ToArray(), 0));
                                    }
                                    v = vTemp;
                                }
                                else
                                {
                                    v = v1;
                                }
                            }
                            if (s.Type == "uint")
                            {
                                if (vals.Length < 4)
                                    continue;
                                uint v1 = BitConverter.ToUInt32(vals.Take(4).Reverse().ToArray(), 0);
                                if (vals.Length > 4)
                                {
                                    List<uint> vTemp = new List<uint>();
                                    vTemp.Add(v1);
                                    for (int i = 1; i < vals.Length / 4; i++)
                                    {
                                        vTemp.Add(BitConverter.ToUInt32(vals.Skip(i * 4).Take(4).Reverse().ToArray(), 0));
                                    }
                                    v = vTemp;
                                }
                                else
                                {
                                    v = v1;
                                }
                            }
                            if (s.Type == "long")
                            {
                                if (vals.Length < 8)
                                    continue;
                                long v1 = BitConverter.ToInt64(vals.Take(8).Reverse().ToArray(), 0);
                                if (vals.Length > 8)
                                {
                                    List<long> vTemp = new List<long>();
                                    vTemp.Add(v1);
                                    for (int i = 1; i < vals.Length / 8; i++)
                                    {
                                        vTemp.Add(BitConverter.ToInt64(vals.Skip(i * 8).Take(8).Reverse().ToArray(), 0));
                                    }
                                    v = vTemp;
                                }
                                else
                                {
                                    v = v1;
                                }
                            }
                            if (s.Type == "ulong")
                            {
                                if (vals.Length < 8)
                                    continue;
                                ulong v1 = BitConverter.ToUInt64(vals.Take(8).Reverse().ToArray(), 0);
                                if (vals.Length > 8)
                                {
                                    List<ulong> vTemp = new List<ulong>();
                                    vTemp.Add(v1);
                                    for (int i = 1; i < vals.Length / 8; i++)
                                    {
                                        vTemp.Add(BitConverter.ToUInt64(vals.Skip(i * 8).Take(8).Reverse().ToArray(), 0));
                                    }
                                    v = vTemp;
                                }
                                else
                                {
                                    v = v1;
                                }
                            }
                        }

                        if (client.ValList.ContainsKey(s.Val))
                        {
                            client.ValList[s.Val] = v;
                        }
                        else
                        {
                            client.ValList.Add(s.Val, v);
                        }
                        //client.Disconnect();
                        //return;
                    }
                    //if (s.Anal[1].ToString() == "==" && bys[s.Index] == Convert.ToByte(s.Anal[2].ToString(), 16))
                    //{
                    //    Console.WriteLine("错误" + s.Anal[3].ToString());
                    //    client.Disconnect();
                    //    return;
                    //}
                }
            }
        }
        private static char[] byteTo8BitArr(byte bye)
        {
            return Convert.ToString(bye, 2).PadLeft(8, '0').Reverse().ToArray();
        }
    }
}
