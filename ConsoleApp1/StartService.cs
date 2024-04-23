using LQ.BlueToothCore.Service.Service.ManagerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LQ.BlueToothCore.Service.Service.LogService;
using LQ.BlueToothCore.Service.Service.StructService;
using LQ.BlueToothCore.MLL.RspMessage;
using LQ.BlueToothCore.MLL.RspMessage.SGTZ;

namespace ConsoleApp1
{
    public class StartService
    {
        public ServerManager ServerManager = null;
        private readonly LogServer logServer = new LogServer();
        /// <summary>
        /// 
        /// </summary>
        public void InitService()
        {
            InitManagerService();

            string[] ports = ServerManager.RefreshSerialPort();
            if (ports.Length > 0)
            {
                if (ServerManager.OpenSerialPortService(ServerManager.PortNameToPort(ports[0])))
                {
                    ServerManager.SendBlueToothName();
                    ServerManager.SendMatchMachineMessage(0);
                }
            }
        }

        private void InitManagerService()
        {
            ServerManager = new ServerManager();
            ServerManager._LogDataCallBack += ServerManager__LogDataCallBack;
            ServerManager._SendDataCallBack += ServerManager__SendDataCallBack;
            ServerManager._RecieveDataCallBack += ServerManager__RecieveDataCallBack;
            ServerManager._HandelFHLRspStartSucessCallBack += ServerManager__HandelFHLRspStartSucessCallBack;
            ServerManager._HandleFHLRspMatchSucessCallBack += ServerManager__HandleFHLRspMatchSucessCallBack;
            ServerManager._HandleFHLRspGetFinnallyScoreCallBack += ServerManager__HandleFHLRspGetFinnallyScoreCallBack;
            ServerManager._HandelSGTZRspStartSucessCallBack += ServerManager__HandelSGTZRspStartSucessCallBack;
            ServerManager._HandleSGTZRspGetFinnallyScoreCallBack += ServerManager__HandleSGTZRspGetFinnallyScoreCallBack;
            ServerManager._HandleSGTZRspMatchSucessCallBack += ServerManager__HandleSGTZRspMatchSucessCallBack;

        }

        private void ServerManager__HandleSGTZRspMatchSucessCallBack(RspSGTZMatchSucessMessage message)
        {

        }

        private void ServerManager__HandleSGTZRspGetFinnallyScoreCallBack(RspSGTZGetFinnallyScoreMessage message)
        {

        }

        private void ServerManager__HandelSGTZRspStartSucessCallBack(RspSGTZStartSucessMessage message)
        {

        }

        private void ServerManager__HandleFHLRspGetFinnallyScoreCallBack(RspFHLGetFinnallyScoreMessage message)
        {

        }

        private void ServerManager__HandleFHLRspMatchSucessCallBack(RspFHLMatchSucessMessage message)
        {

        }

        private void ServerManager__HandelFHLRspStartSucessCallBack(RspFHLStartSucessMessage message)
        {

        }



        /// <summary>
        /// 成绩：70, 72, 76,  0, 1, 0,   6, 5, 0, 0, 29, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 
        ///        F  H   L   (设备) 千   百 十 个 流 校验
        /// 匹配成功 70, 72, 76, 0, 1, 109, 97, 116, 99, 104, 111, 107, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        ///          F   H    L  
        /// </summary>
        /// <param name="message"></param>
        private void ServerManager__RecieveDataCallBack(byte[] message)
        {
            int len = message.Length;
            logServer.Info(message);


        }

        private string[] SpliteMessage(string message)
        {
            char[] delimiter = new char[] { '{', '}' };
            string[] parts = message.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            return parts;
        }

        private void ServerManager__SendDataCallBack(string message)
        {
            message = "正在往串口发送:" + message;
            logServer.Info(message);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        private void ServerManager__LogDataCallBack(string message)
        {
            logServer.Warn(message);
        }
    }

    
}
