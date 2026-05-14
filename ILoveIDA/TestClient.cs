using ILoveIDA.Prots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ILoveIDA
{
    public class TestClient
    {
        public delegate void ConnectedCallback(DeviceModel agv, string msg, bool result);
        public event ConnectedCallback OnConnectedCallback;

        private Socket _Client; // 客户端
        public bool IsConnect;
        private DeviceModel agv;
        public TestClient(DeviceModel _agv)
        {
            agv = _agv;
            OnConnectedCallback += ResolveDataUtil.OnConnectedCallback;

        }
        public void ConnectServer()
        {
            if (_Client != null)
            {
                _Client.Dispose();
            }
            _Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _Client.SendTimeout = 1000;
            _Client.BeginConnect(new IPEndPoint(IPAddress.Parse(agv.IP),agv.Port), AsyncConnectCallback, _Client);
        }
        public void Disconnect()
        {
            if (_Client != null && _Client.Connected)
            {
                _Client.Shutdown(SocketShutdown.Both);
                //_Client.Close();
                IsConnect = false;
            }
        }
        private void AsyncConnectCallback(IAsyncResult ar)
        {
            if (!ar.AsyncWaitHandle.WaitOne(1000))
            {
                return;
            }
            _Client = (Socket)ar.AsyncState;
            try
            {
                if (_Client.Connected)
                {
                    _Client.EndConnect(ar);
                    ConnectedCallback onConnectedCallback = this.OnConnectedCallback;
                    if (onConnectedCallback != null)
                    {
                        onConnectedCallback(agv, "连接成功:", true);
                    }
                    //Receive(_Client);
                    Console.WriteLine("连接成功!");
                    IsConnect = true;
                }
                else
                {
                    Console.WriteLine("连接失败!");
                    //ConnectedCallback onConnectedCallback2 = this.OnConnectedCallback;
                    //if (onConnectedCallback2 != null)
                    //{
                    //    onConnectedCallback2(agv, "连接失败!", false);
                    //}
                }
            }
            catch (SocketException ex)
            {
                //ConnectedCallback onConnectedCallback3 = this.OnConnectedCallback;
                //if (onConnectedCallback3 != null)
                //{
                //    onConnectedCallback3(agv, "连接失败：" + ex.Message, false);
                //}
                Disconnect();
                Console.WriteLine(ex.Message ?? "");
            }
            catch (Exception ex2)
            {
                //ErrTimes++;
                //ConnectedCallback onConnectedCallback4 = this.OnConnectedCallback;
                //if (onConnectedCallback4 != null)
                //{
                //    onConnectedCallback4(agv, "连接失败：" + ex2.Message, false);
                //}
                //IsConnect = false;
                //if (ErrTimes < 3)
                //{
                //    client.Dispose();
                //    Thread.Sleep(3000);
                //    ConnectServer();
                //}
            }
        }
        public byte[] SendBytes(byte[] request)
        {
            if (IsConnect)
            {
                _Client.Send(request);
                byte[] _buffer = new byte[1024];
                int bytesReceived = _Client.Receive(_buffer);
                byte[] response = new byte[bytesReceived];
                Array.Copy(_buffer, response, bytesReceived);
                return response;
            }
            return null;
        }
    }
}
