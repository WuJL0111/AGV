using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace AGV
{
   
    public partial class Form1 : Form
    {
        // 控制器IP地址与端口号
        EndPoint point = new IPEndPoint(IPAddress.Parse("192.168.100.200"), 17800);
        // TCP相关配置
        public Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);

        static Socket client;

        int AutoBootup = 1;
        int time = 0;

        double fyX;
        double fyY;
        double fyTheta;
        double Atan;
        double angle;

        string msg = "";
        string[] strArr = { };

        byte[] radioMes = { };

        double vl = 0.4;
        double va = 0.2;
        double ta = 0;
        double tl = 0;
        string ctime = "";
        double Theta = 0;
        double s = 0;
        double Al = 0.4;
        double[] robotPoint = new double[2];
        
        double[] udpPoint = new double[5];
        List<double> PointsX = new List<double>();
        List<double> PointsY = new List<double>();

        string order00 = " 00 00";
        string order01 = " 01 00";
        string order02 = " 02 00";
        string order03 = " 03 00";
        
        string NaviControl = "05 62 16 FE 9E E4 DF 42 89 63 87 AE 12 9B 0B 47 01 00 00 03 10 00 00 00 01 0B 00 00 4E 61 76 69 43 6F 6E 74 72 6F 6C 00 00 00 00 00";
        string DispatchMode = "05 62 16 FE 9E E4 DF 42 89 63 87 AE 12 9B 0B 47 01 00 00 03 10 00 00 00 01 0B 00 00 44 69 73 70 61 74 63 68 4D 6F 64 65 00 00 00 00";
        string STOP = "05 62 16 FE 9E E4 DF 42 89 63 87 AE 12 9B 0B 47 01 00 00 03 10 00 00 00 01 0B 00 00 53 54 4F 50 00 00 00 00 00 00 00 00 00 00 00 00";
        string v_lin = "05 62 16 FE 9E E4 DF 42 89 63 87 AE 12 9B 0B 47 01 00 00 03 10 00 00 00 01 0B 00 00 76 5F 6C 69 6E 00 00 00 00 00 00 00 00 00 00 00";
        string v_ang = "05 62 16 FE 9E E4 DF 42 89 63 87 AE 12 9B 0B 47 01 00 00 03 10 00 00 00 01 0B 00 00 76 5F 61 6E 67 00 00 00 00 00 00 00 00 00 00 00";
        string cTime = "05 62 16 FE 9E E4 DF 42 89 63 87 AE 12 9B 0B 47 01 00 00 03 10 00 00 00 01 0B 00 00 63 54 69 6D 65 00 00 00 00 00 00 00 00 00 00 00";
        string W = "05 62 16 FE 9E E4 DF 42 89 63 87 AE 12 9B 0B 47 01 00 00 03 10 00 00 00 01 0B 00 00 57 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00";
        string A = "05 62 16 FE 9E E4 DF 42 89 63 87 AE 12 9B 0B 47 01 00 00 03 10 00 00 00 01 0B 00 00 41 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00";
        string S = "05 62 16 FE 9E E4 DF 42 89 63 87 AE 12 9B 0B 47 01 00 00 03 10 00 00 00 01 0B 00 00 53 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00";
        string D = "05 62 16 FE 9E E4 DF 42 89 63 87 AE 12 9B 0B 47 01 00 00 03 10 00 00 00 01 0B 00 00 44 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00";
        string mon_Block = "05 62 16 FE 9E E4 DF 42 89 63 87 AE 12 9B 0B 47 01 00 00 03 10 01 00 00 01 0B 00 00 6D 6F 6E 5F 42 6C 6F 63 6B 00 00 00 00 00 00 00";
        string cur_X = "05 62 16 FE 9E E4 DF 42 89 63 87 AE 12 9B 0B 47 01 00 00 03 10 01 00 00 01 0B 00 00 63 75 72 5F 58 00 00 00 00 00 00 00 00 00 00 00";
        string cur_Y = "05 62 16 FE 9E E4 DF 42 89 63 87 AE 12 9B 0B 47 01 00 00 03 10 01 00 00 01 0B 00 00 63 75 72 5F 59 00 00 00 00 00 00 00 00 00 00 00";
        string cur_Theta = "05 62 16 FE 9E E4 DF 42 89 63 87 AE 12 9B 0B 47 01 00 00 03 10 01 00 00 01 0B 00 00 63 75 72 5F 54 68 65 74 61 00 00 00 00 00 00 00";

        List<string> ff = new List<string>();
        List<string> pathPointX = new List<string>();
        List<string> pathPointY = new List<string>();

        string[] sArrayX = { };
        string[] sArrayY = { };

        public LogWriter LOG;
        List<string> ans = new List<string>();

        IUdpClient udp = new UdpClient();
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            LOG = new LogWriter("start logging ");
            LOG.runLogName = String.Format("{0:yyyy-MM-dd-hh-mm}", DateTime.Now) + ".txt";
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                LOG.WirteLOG1("建立UDP通信");
                udp.Init("192.168.100.200", 17800,"192.168.100.6",17800);   //  建立UDP通信

                Thread udpMonitor = new Thread(MonitorBlock);
                udpMonitor.Start();

                LOG.WirteLOG1("切换手动模式");
                sendNaviControl();
                LOG.WirteLOG1("发送停止信息");
                sendStop();
                LOG.WirteLOG1("小车信息初始化");
                Init();
                LOG.WirteLOG1("UDP连接成功");
            }
            catch
            {
                LOG.WirteLOG1("UDP连接失败");
            }
            // 调用readX，readY，readTheda读取当前位置坐标点            
            LOG.WirteLOG1("readInformationX");
            ans.Clear();
            readInformationX();
            LOG.WirteLOG1("readInformationX 完成" + fyX);
            
            LOG.WirteLOG1("readInformationY");
            ans.Clear();
            readInformationY();
            LOG.WirteLOG1("readInformationY 完成" + fyY);
            
            LOG.WirteLOG1("readInformationTheta");
            ans.Clear();
            readInformationTheta();
            LOG.WirteLOG1("readInformationTheta 完成" + fyTheta);

            // 合并X,Y的值  
            robotPoint[0] = fyX; robotPoint[1] = fyY;
            udpPoint[0] = (int)((fyX + 0.43) * 100); udpPoint[1] = (int)((fyY + 1.17) * 100);
            udpPoint[2] = 1000; udpPoint[3] = 680;
            try
            {
                LOG.WirteLOG1("客户端已经开启");
                //建立与服务端的TCP通信，检测障碍物，并接收返回值
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //服务端链接ip与端口号
                clientSocket.Connect(new IPEndPoint(IPAddress.Parse("192.168.100.10"), 8001));
                Thread.Sleep(5000);

                string[] ss = new string[5];

                for (int i = 0; i < 2; i++)
                {
                    if (i == 0)
                    {
                        udpPoint[4] = 101;
                        for (int j = 0; j < 5; j++)
                        {
                            ss[j] = udpPoint[j].ToString();
                        }
                        string str = string.Empty;
                        str = string.Join(", ", ss);//数组转成字符串
                        byte[] byteDataX = Encoding.ASCII.GetBytes("[" + str + "]");
                        clientSocket.BeginSend(byteDataX, 0, byteDataX.Length, 0, new AsyncCallback(SendCallback), clientSocket);
                        sendDone.WaitOne();
                        byte[] data1 = new byte[1024];
                        int count1 = clientSocket.Receive(data1);
                        LOG.WirteLOG1("接收到所有X坐标");
                        string msg1 = Encoding.UTF8.GetString(data1, 0, count1);
                        msg1 = msg1.Substring(1, msg1.Length - 2);
                        sArrayX = msg1.Split(',');
                    }
                    if (i == 1)
                    {
                        udpPoint[4] = 102;
                        for (int j = 0; j < 5; j++)
                        {
                            ss[j] = udpPoint[j].ToString();
                        }
                        string str = string.Empty;
                        str = string.Join(", ", ss);//数组转成字符串
                        byte[] byteDataY = Encoding.ASCII.GetBytes("[" + str + "]");
                        clientSocket.BeginSend(byteDataY, 0, byteDataY.Length, 0, new AsyncCallback(SendCallback), clientSocket);
                        sendDone.WaitOne();
                        byte[] data2 = new byte[1024];
                        int count2 = clientSocket.Receive(data2);
                        LOG.WirteLOG1("接收到所有Y坐标");
                        string msg2 = Encoding.UTF8.GetString(data2, 0, count2);
                        msg2 = msg2.Substring(1, msg2.Length - 2);
                        sArrayY = msg2.Split(',');                             
                    }                    
                }
                for (int j = 0; j < sArrayX.Length; j++)
                {
                    LOG.WirteLOG1("X: " + sArrayX[j] + "Y: " + sArrayY[j]);
                }
            }
            catch
            {
                LOG.WirteLOG1("TCP连接失败");
            }

            PointsX = sArrayX.ToList().ConvertAll(s => Convert.ToDouble(s));
            PointsY = sArrayY.ToList().ConvertAll(s => Convert.ToDouble(s));
            //double X1 = 1.8; double Y1 = 5.23;

            for (int i = 1; i < PointsX.Count ; i++)
            {
                LOG.WirteLOG1("角度运算");
                LOG.WirteLOG1(PointsX[i].ToString());
                LOG.WirteLOG1(PointsY[i].ToString());
                // 将X，Y与TCP列表中的数值进行求值和角度的运算
                double X1 = PointsX[i]; double Y1 = PointsY[i];
                LOG.WirteLOG1("readInformationX");
                ans.Clear();
                readInformationX();
                LOG.WirteLOG1("移动前X点：" + fyX);

                LOG.WirteLOG1("readInformationY");
                ans.Clear();
                readInformationY();
                LOG.WirteLOG1("移动前Y点：" + fyY);

                LOG.WirteLOG1("readInformationTheta");
                ans.Clear();
                readInformationTheta();
                LOG.WirteLOG1("移动前角度：" + fyTheta);

                LOG.WirteLOG1("readInformationTheta 完成");
                Atan = Math.Atan(Math.Abs(Y1 - robotPoint[1])/ Math.Abs(X1 - robotPoint[0])) * 180 / Math.PI;
                Atan = Atan / 180 * 3.14;
                LOG.WirteLOG1("当前机器人方向：" + fyTheta);
                if (Y1 - robotPoint[1] > 0)
                {
                    // 第一象限
                    if (X1 - robotPoint[0] > 0)
                    {
                        angle = Atan - fyTheta;
                        if (angle > 3.14)
                        {
                            angle = angle - (Math.PI * 2);
                        }
                        else if (angle < -3.14)
                        {
                            angle = angle + (Math.PI * 2);
                        }
                        LOG.WirteLOG1("转到第一象限角度是：" + angle.ToString() + "初始角度" + fyTheta.ToString() + "计算角度" + Atan.ToString());
                    }
                    // 第二象限
                    else
                    {
                        angle = Math.PI - Atan - fyTheta;
                        if (angle > 3.14)
                        {
                            angle = angle - (Math.PI * 2);
                        }
                        else if (angle < -3.14)
                        {
                            angle = angle + (Math.PI * 2);
                        }
                        LOG.WirteLOG1("转到第二象限角度是：" + angle.ToString() + "初始角度" + fyTheta.ToString() + "计算角度" + Atan.ToString());
                    }
                }
                else
                {
                    // 第四象限
                    if (X1 - robotPoint[0] > 0) 
                    {
                        angle = -Atan - fyTheta;
                        if (angle > 3.14)
                        {
                            angle = angle - (Math.PI * 2);
                        }
                        else if (angle < -3.14)
                        {
                            angle = angle + (Math.PI * 2);
                        }
                        LOG.WirteLOG1("转到第四象限角度是：" + angle.ToString() + "初始角度" + fyTheta.ToString() + "计算角度" + Atan.ToString());
                    }
                    // 第三象限
                    else
                    {
                        angle = Atan - fyTheta - Math.PI;
                        if (angle > 3.14)
                        {
                            angle = angle - (Math.PI * 2);
                        }
                        else if (angle < -3.14)
                        {
                            angle = angle + (Math.PI * 2);
                        }
                        LOG.WirteLOG1("转到第三象限角度是：" + angle.ToString() + "初始角度" + fyTheta.ToString() + "计算角度" + Atan.ToString());
                    }
                }
                LOG.WirteLOG1("距离运算");
                s = Math.Sqrt(Math.Abs(X1 - robotPoint[0]) * Math.Abs(X1 - robotPoint[0]) + Math.Abs(Y1 - robotPoint[1]) * Math.Abs(Y1 - robotPoint[1]));
                LOG.WirteLOG1("距离运算完成" + s);

                // 根据距离和角度计算线速度和角速度运行的时间,合成相关报文并基于UDP通信发送
                ff.Clear();
                double Ta = Math.Abs(angle) / va * 1000;
                time = (int)Convert.ToDouble(Ta);
                LOG.WirteLOG1("角速度时间运算" + time);
                ctime = time.ToString("x8");

                for (int j = 0; j < 8; j++)
                {
                    if (ctime.Length < 8)
                    {
                        ctime = 0 + ctime;
                    }
                    else
                    {
                        break;
                    }
                }

                for (int index = ctime.Length / 2; index >0; index--)
                {
                    string ss = ctime.Substring(2 * (index - 1), 2);
                    ss = ' ' + ss;
                    ff.Add(ss);
                }
                LOG.WirteLOG1("角速度时间报文发送");
                writeTime();
                LOG.WirteLOG1("角速度时间报文发送成功并开始旋转");
                judgeDirection();
                Thread.Sleep(time + 2000);

                ff.Clear();
                //  未设置龟速
                //double Tl1 = Al / vl;
                //double s1 = Al * Tl1 * Tl1 * 2;
                //double Tl = (s - s1) / vl * 1000;

                //  设置龟速
                double vGui = 0.05;
                double Tl1 = Al / vl;
                double s1 = Al * Tl1 * Tl1;
                double s2 = (vGui * 2) + ((vGui * vGui) / Al) + (((vGui + vl) * ((vl - vGui) / Al)) / 2);
                double Tl = (s - s1 - s2) / vl * 1000;
                LOG.WirteLOG1("线速度时间运算"+ Tl);
                time = (int)Convert.ToDouble(Tl);
                ctime = time.ToString("x8");

                for (int j = 0; j < 8; j++)
                {
                    if (ctime.Length < 8)
                    {
                        ctime = 0 + ctime;
                    }
                }
                
                for (int index = ctime.Length / 2; index> 0; index--)
                {
                    string ss = ctime.Substring(2 * (index - 1), 2);
                    ss = ' ' + ss;
                    ff.Add(ss);
                }
                LOG.WirteLOG1("线速度时间报文发送");
                writeTime();
                LOG.WirteLOG1("线速度时间报文发送成功并开始运动");
                writeGo();
                Thread.Sleep(time + 2000);
                Init();
                
                LOG.WirteLOG1("readInformationX");
                ans.Clear();
                readInformationX();
                LOG.WirteLOG1("机器人当前点X："+ fyX.ToString());

                LOG.WirteLOG1("readInformationY");
                ans.Clear();
                readInformationY();
                LOG.WirteLOG1("机器人当前点Y：" + fyY.ToString());

                LOG.WirteLOG1("readInformationTheta");
                ans.Clear();
                readInformationTheta();
                LOG.WirteLOG1("readInformationTheta 完成");
                LOG.WirteLOG1("机器人当前角度：" + fyTheta.ToString());
                // 合并X,Y的值            
                robotPoint[0] = fyX; robotPoint[1] = fyY;

                if (robotPoint[0] < PointsX[i] - 0.05 && robotPoint[0] < PointsX[i] + 0.05)
                {
                    LOG.WirteLOG1("进行下一次位移");
                }                
                else
                {
                    if (robotPoint[1] < PointsY[i] - 0.05 && robotPoint[1] < PointsY[i] - 0.05)
                    {
                        LOG.WirteLOG1("误差过大或无法读取机器人当前状态 X 误差" + (Math.Abs(PointsX[i] - robotPoint[0])).ToString());
                    }
                    else
                    {
                        LOG.WirteLOG1("误差过大或无法读取机器人当前状态 Y 误差" + (Math.Abs(PointsY[i] - robotPoint[0])).ToString());
                    }
                }
            }            
            Thread.Sleep(10);
        }
        public void MonitorBlock()
        {
            while (true)
            {
                msg = mon_Block;
                strArr = msg.Trim().Split(' ');
                radioMes = new byte[strArr.Length];

                for (int i = 0; i < strArr.Length; i++)
                {
                    radioMes[i] = Convert.ToByte(strArr[i], 16);
                }
                byte[] buffer = new byte[1024 * 2];
                string[] rec = new string[buffer.Length];
                buffer = udp.SendMsgwait(radioMes);

                for (int i = 0; i < buffer.Length; i++)
                {
                    rec[i] = Convert.ToString(buffer[i], 16);

                    ans.Add(rec[i]);
                }

                if (ans[44] != "0")
                {
                    LOG.WirteLOG1("AGV遇到障碍物，准备切换手动模式");
                    break;
                }
                else
                {
                    LOG.WirteLOG1("AGV在自动导航中");
                }
            }
        }
        private static void SendCallback(IAsyncResult ar)
        {
            try
            {  
                Socket handler = (Socket)ar.AsyncState;   
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void sendNaviControl()
        {
            msg = NaviControl + order00;
            strArr = msg.Trim().Split(' ');
            radioMes = new byte[strArr.Length];

            for (int i = 0; i < strArr.Length; i++)
            {
                radioMes[i] = Convert.ToByte(strArr[i], 16);
            }

            byte[] buffer = new byte[1024 * 2];
            string[] rec = new string[buffer.Length];
            buffer = udp.SendMsgwait(radioMes);
            for (int i = 0; i < buffer.Length; i++)
            {
                rec[i] = Convert.ToString(buffer[i], 16);

                ans.Add(rec[i]);
            }
            if (ans[22] !="0")
            {
                LOG.WirteLOG1("接收数据错误");
            }
            LOG.WirteLOG1("发送完成切换手动模式");
            Thread.Sleep(1000);
        }
        public void sendStop()
        {
            // 将报文字节转换成十六进制发送到控制器
            msg = STOP + order00;
            strArr = msg.Trim().Split(' ');
            radioMes = new byte[strArr.Length];

            for (int i = 0; i < strArr.Length; i++)
            {
                radioMes[i] = Convert.ToByte(strArr[i], 16);
            }
            byte[] buffer = new byte[1024 * 2];
            string[] rec = new string[buffer.Length];
            buffer = udp.SendMsgwait(radioMes);
            for (int i = 0; i < buffer.Length; i++)
            {
                rec[i] = Convert.ToString(buffer[i], 16);

                ans.Add(rec[i]);
            }
            if (ans[22] != "0")
            {
                LOG.WirteLOG1("接收数据错误");
            }
            LOG.WirteLOG1("发送完成STOP");
            Thread.Sleep(10);   
        }
        public void readInformationX()
        {
            msg = cur_X;
            strArr = msg.Trim().Split(' ');
            radioMes = new byte[strArr.Length];

            for (int i = 0; i < strArr.Length; i++)
            {
                radioMes[i] = Convert.ToByte(strArr[i], 16);
            }
            byte[] buffer = new byte[1024 * 2];
            string[] rec = new string[buffer.Length];
            buffer = udp.SendMsgwait(radioMes);

            for (int i = 0; i < buffer.Length; i++)
            {
                rec[i] = Convert.ToString(buffer[i], 16);

                ans.Add(rec[i]);
            }

            for (int i = 44; i < 48; i++)
            {
                if (ans[i].Length % 2 != 0)
                {
                    ans[i] = 0 + ans[i];
                }
            }
            uint x = Convert.ToUInt32(ans[47] + ans[46] + ans[45] + ans[44], 16);//字符串转16进制32位无符号整数
            float transformX = BitConverter.ToSingle(BitConverter.GetBytes(x), 0);
            fyX = Convert.ToDouble(transformX);
            Thread.Sleep(10);            
            LOG.WirteLOG1("接收X点坐标报文");                  
        }
        public void readInformationY()
        {
            msg = cur_Y;
            strArr = msg.Trim().Split(' ');
            radioMes = new byte[strArr.Length];

            for (int i = 0; i < strArr.Length; i++)
            {
                radioMes[i] = Convert.ToByte(strArr[i], 16);
            }
            byte[] buffer = new byte[1024 * 2];
            string[] rec = new string[buffer.Length];
            buffer = udp.SendMsgwait(radioMes);

            for (int i = 0; i < buffer.Length; i++)
            {
                rec[i] = Convert.ToString(buffer[i], 16);

                ans.Add(rec[i]);
            }

            for (int i = 44; i < 48; i++)
            {
                if (ans[i].Length % 2 != 0)
                {
                    ans[i] = 0 + ans[i];
                }
            }
            uint y = Convert.ToUInt32(ans[47] + ans[46] + ans[45] + ans[44], 16);//字符串转16进制32位无符号整数
            float transformY = BitConverter.ToSingle(BitConverter.GetBytes(y), 0);
            fyY = Convert.ToDouble(transformY);
            Thread.Sleep(10);
            LOG.WirteLOG1("接收Y点坐标报文");            
        }
        public void readInformationTheta()
        {
            msg = cur_Theta;
            strArr = msg.Trim().Split(' ');
            radioMes = new byte[strArr.Length];

            for (int i = 0; i < strArr.Length; i++)
            {
                radioMes[i] = Convert.ToByte(strArr[i], 16);
            }
            byte[] buffer = new byte[1024 * 2];
            string[] rec = new string[buffer.Length];
            buffer = udp.SendMsgwait(radioMes);

            for (int i = 0; i < buffer.Length; i++)
            {
                rec[i] = Convert.ToString(buffer[i], 16);

                ans.Add(rec[i]);
            }

            for (int i = 44; i < 48; i++)
            {
                if (ans[i].Length % 2 != 0)
                {
                    ans[i] = 0 + ans[i];
                }
            }
            uint thetaz = Convert.ToUInt32(ans[47] + ans[46] + ans[45] + ans[44], 16);//字符串转16进制32位无符号整数
            float transformThetaz = BitConverter.ToSingle(BitConverter.GetBytes(thetaz), 0);
            fyTheta = Convert.ToDouble(transformThetaz);
            Thread.Sleep(10);
            LOG.WirteLOG1("接收Theta点坐标报文");           
        }
        public void writeTime()
        {
            msg = cTime + ff[0]+ff[1]+ff[2]+ff[3];
            strArr = msg.Trim().Split(' ');
            radioMes = new byte[strArr.Length];

            for (int i = 0; i < strArr.Length; i++)
            {
                radioMes[i] = Convert.ToByte(strArr[i], 16);
            }
            bool if_write_success= udp.SendMsg(radioMes);
            LOG.WirteLOG1("发送时间完成 ：" + if_write_success.ToString());
            Thread.Sleep(10);
        }
        public void Init()
        {
            msg = A + order00;
            strArr = msg.Trim().Split(' ');
            radioMes = new byte[strArr.Length];
            for (int i = 0; i < strArr.Length; i++)
            {
                radioMes[i] = Convert.ToByte(strArr[i], 16);
            }
            bool if_write_success = udp.SendMsg(radioMes);
            LOG.WirteLOG1("发送初始化A完成 ：" + if_write_success.ToString());

            Thread.Sleep(10);
            msg = D + order00;
            strArr = msg.Trim().Split(' ');
            radioMes = new byte[strArr.Length];
            for (int i = 0; i < strArr.Length; i++)
            {
                radioMes[i] = Convert.ToByte(strArr[i], 16);
            }           
            if_write_success = udp.SendMsg(radioMes);
            LOG.WirteLOG1("发送初始化D完成 ：" + if_write_success.ToString());
            Thread.Sleep(10);

            msg = W + order00;
            strArr = msg.Trim().Split(' ');
            radioMes = new byte[strArr.Length];
            for (int i = 0; i < strArr.Length; i++)
            {
                radioMes[i] = Convert.ToByte(strArr[i], 16);
            }
            if_write_success = udp.SendMsg(radioMes);
            LOG.WirteLOG1("发送初始化W完成 ：" + if_write_success.ToString());
            Thread.Sleep(10);

            msg = S + order00;
            strArr = msg.Trim().Split(' ');
            radioMes = new byte[strArr.Length];
            for (int i = 0; i < strArr.Length; i++)
            {
                radioMes[i] = Convert.ToByte(strArr[i], 16);
            }
            if_write_success = udp.SendMsg(radioMes);
            LOG.WirteLOG1("发送初始化S完成 ：" + if_write_success.ToString());
            Thread.Sleep(10);

            msg = cTime + 00 + 00 + 00 + 00;
            strArr = msg.Trim().Split(' ');
            radioMes = new byte[strArr.Length];
            for (int i = 0; i < strArr.Length; i++)
            {
                radioMes[i] = Convert.ToByte(strArr[i], 16);
            }
            if_write_success = udp.SendMsg(radioMes);
            LOG.WirteLOG1("发送初始化TIME完成 ：" + if_write_success.ToString());
            Thread.Sleep(10);
        }
        public void judgeDirection()
        {
            if (angle >= 0 && angle <= 3.14)
            {
                msg = A + order02;
                strArr = msg.Trim().Split(' ');
                radioMes = new byte[strArr.Length];

                for (int i = 0; i < strArr.Length; i++)
                {
                    radioMes[i] = Convert.ToByte(strArr[i], 16);
                }
                bool if_write_success = udp.SendMsg(radioMes);
                LOG.WirteLOG1("发送A完成 ：" + if_write_success.ToString());
                Thread.Sleep(10);
            }
            else
            {
                msg = D + order02;
                strArr = msg.Trim().Split(' ');
                radioMes = new byte[strArr.Length];

                for (int i = 0; i < strArr.Length; i++)
                {
                    radioMes[i] = Convert.ToByte(strArr[i], 16);
                }
                bool if_write_success = udp.SendMsg(radioMes);
                LOG.WirteLOG1("发送初始化D完成 ：" + if_write_success.ToString());
                Thread.Sleep(10);
            }
        }
        public void writeGo()
        {
            if (s >= 0)
            {
                msg = W + order02;
                strArr = msg.Trim().Split(' ');
                radioMes = new byte[strArr.Length];

                for (int i = 0; i < strArr.Length; i++)
                {
                    radioMes[i] = Convert.ToByte(strArr[i], 16);
                }
                bool if_write_success = udp.SendMsg(radioMes);
                LOG.WirteLOG1("发送初始化W完成 ：" + if_write_success.ToString());
                Thread.Sleep(10);
            }
            else
            {
                msg = S + order02;
                strArr = msg.Trim().Split(' ');
                radioMes = new byte[strArr.Length];

                for (int i = 0; i < strArr.Length; i++)
                {
                    radioMes[i] = Convert.ToByte(strArr[i], 16);
                }
                bool if_write_success = udp.SendMsg(radioMes);
                LOG.WirteLOG1("发送初始化S完成 ：" + if_write_success.ToString());
                Thread.Sleep(10);
            }
        }       
    }
    public class LogWriter
    {
        private string m_exePath = string.Empty;
        public string runLogName;
        public LogWriter(string logMessage)
        {
            WirteLOG1(logMessage);
        }
        public void WirteLOG1(string logMessage)
        {
            m_exePath = "./log/";
            try
            {
                using (StreamWriter w = File.AppendText(m_exePath + runLogName))
                {
                    Log(w, logMessage);
                }
            }
            catch (Exception ex)
            {

            }
        }
        public void Log(TextWriter txtWriter, string logMessage, [CallerLineNumber] int lineNumber = 0)
        {
            try
            {
                string line = String.Format("[{0:yyyy-MM-dd hh:mm:ss:fff}] {1}行 {2}\r\n", DateTime.Now, lineNumber.ToString(), logMessage);
                txtWriter.WriteLine("{0}", line);
            }
            catch (Exception ex)
            {

            }
        }
    }
}