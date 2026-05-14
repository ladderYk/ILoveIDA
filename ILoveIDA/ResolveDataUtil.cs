using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ILoveIDA.Prots;

namespace ILoveIDA
{
    class ResolveDataUtil
    {
        public static void OnGetAGVState(DeviceModel agv)
        {
            Online(agv);
        }
        public static void Close(DeviceModel agv)
        {
            agv.Client?.Disconnect();
        }
        public static void OnGetData(DeviceModel agv)
        {
            GetData(agv);
        }
        private static void Online(DeviceModel agv)
        {
            Task.Run(delegate
            {
                if (!agv.IsConnected && !agv.IsReg)
                {
                    ProtModel type = MainWindow.findByName(agv.Type);
                    if (type != null)
                    {
                        if (agv.Client == null)
                        {
                            agv.Client = new TestClient(agv);
                        }
                        agv.Client.ConnectServer();
                    }
                    //if (agv.tcpClient == null)
                    //    agv.tcpClient = new AsyncTcpClient(agv);
                    agv.IsReg = true;
                    // agv.tcpClient.OnConnectedCallback += ResolveDataUtil.OnConnectedCallback;
                    //agv.tcpClient.ConnectServer();
                    //Utils.AddErr(agv.Name, "连接中", "100");
                }
                else if (agv.IsReg)
                {
                    //Utils.RemoveErr(agv.Name, "100");
                    agv.IsReg = false;
                }
            }).ContinueWith((Func<Task, Task>)async delegate
            {
                await Task.Delay(agv.Timeout);
                if (MainWindow.IsRun)
                    Online(agv);
            });
        }
        private static void GetData(DeviceModel agv)
        {
            Task.Run(delegate
            {
                if (agv.IsConnected && agv.Client != null)
                {
                    // TODO 变量读取周期
                    ProtModel type = MainWindow.findByName(agv.Type);
                    ProtSend protSend = type.FindSendByName("读取");
                    List<byte> sendByts = protSend.DataToBytes();
                    short addr = 100;
                    sendByts.InsertRange(8, BitConverter.GetBytes(addr).Reverse().ToArray());

                    byte[] bys = agv.Client.SendBytes(sendByts.ToArray());
                    ProtRecv protRecv = type.FindRecvByName(protSend.Recv);
                    if (protRecv != null)
                    {
                        foreach (ProtRecvVal s in protRecv.Vals)
                        {
                            object v = null;

                            #region 直接读byte
                            byte[] vals = bys.Skip(s.Index).Take(2).ToArray();
                            if (vals.Length < 1)
                                continue;

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
                            #endregion
                            if (agv.ValList.ContainsKey(s.Val))
                            {
                                agv.ValList[s.Val] = v;
                            }
                            else
                            {
                                agv.ValList.Add(s.Val, v);
                            }
                        }
                    }
                }
            }).ContinueWith((Func<Task, Task>)async delegate
            {
                await Task.Delay(agv.Cycle);
                if (MainWindow.IsRun)
                    GetData(agv);
            });
        }
        internal static void OnConnectedCallback(DeviceModel agv, string msg, bool result)
        {
            if (agv == null)
            {
                return;
            }
            bool showErr = agv.IsConnected && !result;

            agv.IsConnected = result;
            if (result)
            {
                //Utils.AddRtMsg("AGV" + agv.Name + "[" + agv.IP + "]连接成功！");
                //MagneticMapView.MapVM.PutAgvPosition(agv.Name, "#FF32cd32");
                //Utils.SaveErrorLog(DateTime.Now.ToString() + "  【" + agv.Name + "】 连接成功" + Constants.vbCrLf);
                //OnGetData(agv);
                return;
            }
            //Websocket.WebsocketVM.Instance.SendData("online", new JObject { { "online", false }, { "name", agv.Name } }.ToString());
            // Utils.AddRtMsg("AGV" + agv.Name + "[" + agv.IP + "]连接失败：" + msg + "！", 1);
            //MagneticMapView.MapVM.PutAgvPosition(agv.Name, "#FF808080");

            if (showErr)
            {
                //Utils.AddErr(agv.Name, DateTime.Now.ToString() + "  【" + agv.Name + "】 离线", "0");
                //agv.LogicalSite = "";
            }
        }

        private static char[] byteTo8BitArr(byte bye)
        {
            return Convert.ToString(bye, 2).PadLeft(8, '0').Reverse().ToArray();
        }
    }
}
