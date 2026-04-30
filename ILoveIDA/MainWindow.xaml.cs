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
            return prots.FirstOrDefault(prots => prots.Name == name);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TestClient client = new TestClient();
            client.ConnectServer();
            Thread.Sleep(1000);
            // 根据类型查询报文
            ProtModel model = findByName("S7-1200");
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

        }
    }
}
