using ILoveIDA.Prots;
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

        TestClient client = new TestClient();

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
            ProtModel model = findByName("modbusTcp");
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
                        foreach (ProtRecvData s in protRecv.Datas)
                        {
                            if (s.Anal[1].ToString() == "!=" && bys[s.Index] != Convert.ToByte(s.Anal[2].ToString(), 16))
                            {
                                Console.WriteLine("错误" + s.Anal[3].ToString());
                                client.Disconnect();
                                return;
                            }
                            if (s.Anal[1].ToString() == "==" && bys[s.Index] == Convert.ToByte(s.Anal[2].ToString(), 16))
                            {
                                Console.WriteLine("错误" + s.Anal[3].ToString());
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
                ProtModel model = findByName("modbusTcp");

                ProtSend protSend = model.FindSendByName("写入");
                if (protSend == null)
                {
                    client.Disconnect();
                    return;
                }
                short addr = 100;
                List<byte> sendByts = protSend.DataToBytes();
                sendByts.InsertRange(8, BitConverter.GetBytes(addr).Reverse().ToArray());
                sendByts.InsertRange(10, BitConverter.GetBytes((ushort)400).Reverse().ToArray());

                byte[] bys = client.SendBytes(sendByts.ToArray());
                    //.CopyTo(bys, 8);
                

                ProtRecv protRecv = model.FindRecvByName(protSend.Recv);
                if (protRecv != null)
                {
                    foreach (ProtRecvData s in protRecv.Datas)
                    {
                        // 赋值操作
                        if (s.Anal[0] == "=")
                        {
                            // 取数组
                            byte[] vals = bys.Skip(s.Index).Take(s.Len).ToArray();
                            if (vals.Length < 1)
                                continue;

                            object v = null;
                            if (s.Type == "bool")
                            {
                                bool v1 = vals[0] == 1;
                                if (vals.Length > 1)
                                {
                                    List<bool> vTemp = new List<bool>();
                                    vTemp.Add(v1);
                                    for (int i = 1; i < vals.Length; i++)
                                    {
                                        vTemp.Add(vals[i] == 1);
                                    }
                                    v = vTemp;
                                }
                                else
                                {
                                    v = v1;
                                }
                            }
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
                                // modbustcp ABCD
                                uint v1 = BitConverter.ToUInt32(vals.Take(4).Reverse().ToArray(), 0);
                                // CDAB
                                //int v1 = BitConverter.ToInt32(vals.Take(2).Reverse().ToArray().Concat(vals.Skip(2).Take(2).Reverse().ToArray()).ToArray(), 0);
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

                            if (client.ValList.ContainsKey(s.Anal[1]))
                            {
                                client.ValList[s.Anal[1]] = v;
                            }
                            else
                            {
                                client.ValList.Add(s.Anal[1], v);
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
        }
        public static string ToHexString(byte[] bytes)
        {
            string text = string.Empty;
            if (bytes != null)
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    stringBuilder.Append(bytes[i].ToString("X2") + " ");
                }
                text = stringBuilder.ToString();
            }
            return text.TrimEnd(new char[] { ' ' });
        }
        public static byte[] HexStrTobyte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if (hexString.Length % 2 != 0)
            {
                hexString += " ";
            }
            byte[] array = new byte[hexString.Length / 2];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = Convert.ToByte(hexString.Substring(i * 2, 2).Trim(), 16);
            }
            return array;
        }
    }
}
