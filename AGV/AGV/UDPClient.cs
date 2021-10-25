using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;
//using CatLib;
using System.Collections.Generic;

namespace AGV
{
    public interface IUdpClient
    {
        bool Init(string serverAddress, int serverPort, string localIP, int localPort);
        void Close();
        bool IsConnect();
        bool SendMsg(byte[] msg);
        byte[] SendMsgwait(byte[] msg);
        void ReceiveMsgCallBack(Action<byte[]> action);
        void ReceiveMsgCallBackBuffer(Action<byte[],int> action);
        void ErrorInfo(Action<string> action);
    }
    /// <summary>
    /// UDP封装类     UDP被连接方断开本地是不知道啊，需要对返回的数据进行校验  断开返回空字符""
    /// </summary>
   public class UdpClient: IUdpClient
    {
        private Action<byte[]> receiveMsg;
        private Action<byte[],int> receiveMsgBuf;
        private Action<string> errorInfo;
        private Socket socket;
        private IPEndPoint localEndPort;
        private IPEndPoint serverEndPort;
        private bool isConnect = false;
        byte[] Recmsg =null;
        public bool Init(string serverAddress, int serverPort, string localIP, int localPort)
        {
            //if (!UdpClientCount(serverAddress, serverPort, localIP, localPort))
            //{
            //    return false;
            //}
            //  udpClient.receiveMsg += OnUpdateRecvMessage;
            if (isConnect)
            {
                return true;
            }
            
            var iplist = GetLocalIP();
            var flag = false;
            for (int i = 0; i < iplist.Count; i++)
            {
                if (iplist[i] == localIP)
                {
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    localEndPort = new IPEndPoint(IPAddress.Parse(localIP), localPort);
                    serverEndPort = new IPEndPoint(IPAddress.Parse(serverAddress), serverPort);
                    try
                    {
                        socket.Bind(localEndPort);
                        flag = true;
                    }
                    catch (Exception)
                    {
                      //  App.TriggerHalt(EventSys.ShowMessageBox, "端口:"+ localPort+"已被占用！", "警告", 16);
                        //throw;
                    }
                }
            }
            if (!flag)
            {
               // App.TriggerHalt(EventSys.ShowMessageBox,"本地IP设置错误","警告",16);
                return false;
            }
            Thread recvThread = new Thread(Receive);
            recvThread.IsBackground = true;
            recvThread.Start();
            isConnect = true;
            return true;
        }
    

        public bool SendMsg(byte[] msg)
        {
            try
            {
                socket.SendTo(msg, serverEndPort);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void Receive()
        {
            EndPoint s = serverEndPort;
            byte[] buffer = new byte[1024];
            byte[] Refbuffer;
            try
            {
                while (true)
                {
                    int length = socket.ReceiveFrom(buffer, ref s);
                    if (length > 0)
                    {
                        receiveMsgBuf?.Invoke(buffer,length);
                        // Recmsg = Encoding.UTF8.GetString(buffer, 0, length);
                        Refbuffer = new byte[length];
                        Array.Copy(buffer,Refbuffer,length);
                        receiveMsg?.Invoke(Refbuffer);
                        Recmsg = Refbuffer;
                    }
                    else
                        Thread.Sleep(50);
                }
            }
            catch (System.Exception e)
            {
                isConnect = false;
                Close();
                ////异常处理  UPDServer 已经被关闭
                errorInfo?.Invoke(e.Message);
            }
        }
       
        public void ReceiveMsgCallBack(Action<byte[]> action)
        {
          receiveMsg += action;
            //throw new NotImplementedException();
        }

        public void ReceiveMsgCallBackBuffer(Action<byte[],int> action)
        {
             receiveMsgBuf = action;
        }

        public void ErrorInfo(Action<string> action)
        {
            errorInfo = action;
        }




        public List<string> GetLocalIP()
        {
            List<string> Iplist = new List<string>();
            try
            {
                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                         Iplist.Add( IpEntry.AddressList[i].ToString());
                    }
                }
                return Iplist;
            }
            catch (Exception ex)
            {
              //  MessageBox.Show("获取本机IP出错:" + ex.Message);
                return Iplist;
            }
        }

        public void Close()
        {
            socket.Close();
            socket = null;
    }

        public bool IsConnect()
        {
            return isConnect;
        }

        public byte[]  SendMsgwait(byte[] msg)
        {
            Recmsg =null;
            SendMsg( msg);
            Thread.Sleep(300);
            return Recmsg;
        }
    }
}
