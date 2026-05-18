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

                    // 根据类型查询协议
                    ProtModel type = MainWindow.findByName(agv.Type);
                    if (type != null)
                    {
                        // 遍历数据读取列表
                        foreach (DeviceData data in agv.Datas)
                        {
                            // 根据名称查询报文
                            ProtSend protSend = type.FindSendByName(data.SendPort);
                            List<byte> sendByts = protSend.DataToBytes();
                            // 根据参数配置报文
                            int len = 0;
                            foreach(ProtSendParam param in protSend.Params)
                            {
                                if(!data.Params.ContainsKey(param.Name))
                                {
                                    continue;
                                }
                                short val = Convert.ToInt16(data.Params[param.Name]);
                                sendByts.InsertRange(param.Index + len, BitConverter.GetBytes(val).Reverse().ToArray());
                                len += 1;
                            }
                            //foreach (string param in data.Params.Keys)
                            //{
                            //    ProtSendParam sendParam = protSend.FindParamByName(param);
                            //    if (sendParam != null)
                            //    {
                            //        short val = Convert.ToInt16(data.Params[param]);
                            //        sendByts.InsertRange(sendParam.Index + len, BitConverter.GetBytes(val).Reverse().ToArray());
                            //        len += 1;
                            //    }
                            //}
                            // 发送并接收数据
                            byte[] bys = agv.Client.SendBytes(sendByts.ToArray());
                            if (bys == null)
                                continue;
                            // 判断条件，解析结果 
                            ProtRecv protRecv = type.FindRecvByName(protSend.Recv);
                            if (protRecv != null)
                            {

                            }
                            // 遍历查询结果
                            foreach (DeviceRecvVal s in data.Vals)
                            {
                                object v = null;

                                #region 直接读byte
                                byte[] vals = bys.Skip(s.Index).Take(s.Len).ToArray();
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
                        //if (!agv.ParamList.ContainsKey(data.Params.Keys.First()))
                        //    agv.ParamList.Add(data.Params.Keys.First(), sendParam != null);
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

            if (result)
            {
                //Utils.AddRtMsg("AGV" + agv.Name + "[" + agv.IP + "]连接成功！");
                //MagneticMapView.MapVM.PutAgvPosition(agv.Name, "#FF32cd32");
                //Utils.SaveErrorLog(DateTime.Now.ToString() + "  【" + agv.Name + "】 连接成功" + Constants.vbCrLf);
                //OnGetData(agv);
                ProtModel type = MainWindow.findByName(agv.Type);
                // 判断是否有握手报文
                foreach (string send in type.Hands)
                {
                    ProtSend protSend = type.FindSendByName(send);
                    if (protSend == null)
                    {
                        agv.Client.Disconnect();
                        return;
                    }
                    byte[] bys = agv.Client.SendHandBytes(protSend.DataToBytes().ToArray());
                    ProtRecv protRecv = type.FindRecvByName(protSend.Recv);
                    if (protRecv != null)
                    {
                        if (bys.Length != protRecv.Len)
                        {
                            Console.WriteLine("响应长度不足");
                            agv.Client.Disconnect();
                            return;
                        }
                        foreach (ProtRecvCond s in protRecv.Conds)
                        {
                            if (s.Anal[0].ToString() == "!=" && bys[s.Index] != Convert.ToByte(s.Anal[1].ToString(), 16))
                            {
                                Console.WriteLine("错误" + s.Anal[2].ToString());
                                agv.Client.Disconnect();
                                return;
                            }
                            if (s.Anal[0].ToString() == "==" && bys[s.Index] == Convert.ToByte(s.Anal[1].ToString(), 16))
                            {
                                Console.WriteLine("错误" + s.Anal[2].ToString());
                                agv.Client.Disconnect();
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
                agv.IsConnected = result;

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
