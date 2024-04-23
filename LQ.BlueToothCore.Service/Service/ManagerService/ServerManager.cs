using LQ.BlueToothCore.IService.IService.ISerialPortService;
using LQ.BlueToothCore.MLL.Messages;
using LQ.BlueToothCore.MLL.RspMessage;
using LQ.BlueToothCore.MLL.RspMessage.SGTZ;
using LQ.BlueToothCore.Service.Service.SerialPortService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LQ.BlueToothCore.Service.Service.ManagerService
{
    /// <summary>
    /// 
    /// </summary>
    public class ServerManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public delegate void SendDataCallBack(string message );
        /// <summary>
        /// 
        /// </summary>
        public event SendDataCallBack _SendDataCallBack;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public delegate void LogDataCallBack(string message);
        /// <summary>
        /// 
        /// </summary>
        public event LogDataCallBack _LogDataCallBack;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public delegate void RecieveDataCallBack(byte[] message);
        /// <summary>
        /// 
        /// </summary>
        public event RecieveDataCallBack _RecieveDataCallBack;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public delegate void HandleFHLRspMatchSucessMessageCallBack(RspFHLMatchSucessMessage message);
        /// <summary>
        /// 处理匹配成功的委托事件
        /// </summary>
        public event HandleFHLRspMatchSucessMessageCallBack _HandleFHLRspMatchSucessCallBack;
        /// <summary>
        /// 
        /// </summary>
        public delegate void HandelFHLRspGetFinnallyScoreMessageCode(RspFHLGetFinnallyScoreMessage message);
        /// <summary>
        /// 
        /// </summary>
        public event HandelFHLRspGetFinnallyScoreMessageCode _HandleFHLRspGetFinnallyScoreCallBack;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public delegate void HandleFHLRspStartSucessMessageCallBack(RspFHLStartSucessMessage message);
        /// <summary>
        /// 
        /// </summary>
        public event HandleFHLRspStartSucessMessageCallBack _HandelFHLRspStartSucessCallBack;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public delegate void HandleSGTZRspMatchSucessMessageCallBack(RspSGTZMatchSucessMessage message);
        /// <summary>
        /// 处理匹配成功的委托事件
        /// </summary>
        public event HandleSGTZRspMatchSucessMessageCallBack _HandleSGTZRspMatchSucessCallBack;
        /// <summary>
        /// 
        /// </summary>
        public delegate void HandelSGTZRspGetFinnallyScoreMessageCode(RspSGTZGetFinnallyScoreMessage message);
        /// <summary>
        /// 
        /// </summary>
        public event HandelSGTZRspGetFinnallyScoreMessageCode _HandleSGTZRspGetFinnallyScoreCallBack;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public delegate void HandleSGTZRspStartSucessMessageCallBack(RspSGTZStartSucessMessage message);
        /// <summary>
        /// 
        /// </summary>
        public event HandleSGTZRspStartSucessMessageCallBack _HandelSGTZRspStartSucessCallBack;
        /// <summary>
        /// 
        /// </summary>
        public ServerManager()
        {
            InitSerialPortService();
        }
        /// <summary>
        /// 
        /// </summary>
        private ISerialPortServer serialPortServer;
        /// <summary>
        /// 
        /// </summary>
        public  bool OpenSerialPortService(string port,int nBaudrate=115200) => serialPortServer.OpenSerialPortConnection(port, nBaudrate);  
        /// <summary>
        /// 
        /// </summary>
        private void InitSerialPortService()
        {
            serialPortServer = new SerialPortServer();
            serialPortServer._SendDataCallBack += message => _SendDataCallBack?.Invoke(message);
            serialPortServer._LogDataCallBack += message => _LogDataCallBack?.Invoke(message);
            serialPortServer._RecieveDataCallBack += message => _RecieveDataCallBack?.Invoke(message);
            serialPortServer._HandleFHLRspMatchSucessMessageCallBack += message => _HandleFHLRspMatchSucessCallBack?.Invoke(message);
            serialPortServer._HandelFHLRspGetFinnallyScoreMessageCallBack += message => _HandleFHLRspGetFinnallyScoreCallBack?.Invoke(message);
            serialPortServer._HandleFHLRspStartSucessMessageCallBack += message => _HandelFHLRspStartSucessCallBack?.Invoke(message);
            serialPortServer._HandelSGTZRspGetFinnallyScoreMessageCallBack += message => _HandleSGTZRspGetFinnallyScoreCallBack?.Invoke(message);
            serialPortServer._HandleSGTZRspMatchSucessMessageCallBack += message => _HandleSGTZRspMatchSucessCallBack?.Invoke(message);
            serialPortServer._HandleSGTZRspStartSucessMessageCallBack += message => _HandelSGTZRspStartSucessCallBack?.Invoke(message);

        }

        /// <summary>
        /// 当前流水号
        /// </summary>
        private byte[] currentCode;
        /// <summary>
        /// 随机2 个字节的数组
        /// </summary>
        /// <returns></returns>
        private byte[] RandomShortByte()
        {
            Random random = new Random();
            short randomShort = (short)random.Next(short.MinValue, short.MaxValue);
            byte[] data = BitConverter.GetBytes(randomShort);
            return data;
        }
        /// <summary>
        /// 流水号
        /// </summary>
        private void CreateRunningWaterCode()
        {
            if(currentCode == null) currentCode = RandomShortByte();
            else
            {
                byte[] code ;
                do
                {
                    code=RandomShortByte();
                }while(currentCode==code);
                currentCode = code;
            }
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ports"></param>
        /// <returns></returns>
        public string[] RefreshSerialPort(string ports = "USB Serial Port")
        {
            string[] portss = serialPortServer.GetPortBySerialPortName(ports);
            if (portss.Length == 0)
            {
                ports = "USB-SERIAL";
                portss = serialPortServer.GetPortBySerialPortName(ports);
            }
            if (portss.Length == 0)
            {
                ports = "USB-to-Serial";
                portss = serialPortServer.GetPortBySerialPortName(ports);
            }
            return portss;
        }
        /// <summary>
        /// COM6
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public string PortNameToPort(string port)=> serialPortServer.PortNameToPort(port);
        /// <summary>
        /// 
        /// </summary>
        public void CloseSerialPortService()=>serialPortServer.CloseSerialPortConnection();
        /// <summary>
        /// 串口是否连接
        /// </summary>
        /// <returns></returns>
        public bool IsConnectionSerialPort()=>serialPortServer.IsSerialPortConnection();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="length"></param>
        private void SendMessageBySerial(byte[] message,int length=0)=>serialPortServer.SendSerialPortMessage(message,length);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        private void SetCurrentType(string type)=>serialPortServer.SetCurrentType(type);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        private void SetCurrentProjectType(int type)=>serialPortServer.SetCurrentProjectType(type);
        /// <summary>
        ///  发送配对请求
        /// </summary>
        public void SendMatchMachineMessage(int type)
        {
            string cmd = "MATCH";
            SetCurrentProjectType(type);
            SetCurrentType(cmd);     
            byte[] cmdCode=Encoding.ASCII.GetBytes(cmd);
            byte typeCode = (byte)10;
            CreateRunningWaterCode();
            ReqBlueMessage reqBlueMessage= new ReqBlueMessage(type,currentCode,typeCode,cmdCode);
            byte[] finnallyCode=reqBlueMessage.FinnallyCode;
            SendMessageBySerial(finnallyCode ,0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="equipMentCode">设备码</param>
        public void SendStartMessage(int type,byte[] equipMentCode)
        {
            string cmd = "START";
            SetCurrentProjectType(type);
            SetCurrentType(cmd);
            byte[] cmdCode = Encoding.ASCII.GetBytes(cmd);
            byte typeCode = (byte)12;
            CreateRunningWaterCode();
            ReqBlueMessage reqBlueMessage = new ReqBlueMessage(type, currentCode, typeCode, cmdCode);
            byte[] finnallyCode = reqBlueMessage.FinnallyCode;
            SendMessageBySerial(finnallyCode, 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void SendBlueToothName( )
        {
            string cmd = "'setHostName:PLLZ'";
            byte[] data=Encoding.ASCII.GetBytes(cmd);
            SendMessageBySerial(data, 0);
        }
        
    }
}
