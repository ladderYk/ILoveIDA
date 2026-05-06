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
                foreach(string send in model.Hands)
                {
                    ProtSend protSend = model.FindSendByName(send);
                    if (protSend == null)
                    {
                        client.Disconnect();
                        return;
                    }
                    byte[] bys = client.SendBytes(protSend.DataToBytes());
                    ProtRecv protRecv = model.FindRecvByName(protSend.Recv);
                    if(protRecv != null)
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
                                Console.WriteLine("错误"+s.Anal[3].ToString());
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

                ProtSend protSend = model.FindSendByName("读取");
                if (protSend == null)
                {
                    client.Disconnect();
                    return;
                }
                byte[] bys = client.SendBytes(protSend.DataToBytes());
            }
        }
    }
}
