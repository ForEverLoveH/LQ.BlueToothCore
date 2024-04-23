using LQ.BlueToothCore.MLL.RspMessage;
using LQ.BlueToothCore.MLL.RspMessage.SGTZ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LQ.BlueToothCore.IService.IService.ISerialPortService
{
    public delegate void RecieveDataCallBack(byte[] message);
    public delegate void LogDataCallBack(string message);
    public delegate void SendDataCallBack(string message);
    #region 肺活量
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    public delegate void HandleFHLRspMatchSucessMessageCallBack(RspFHLMatchSucessMessage message);
    /// <summary>
    /// 
    /// </summary>
    public delegate void HandelFHLRspGetFinnallyScoreMessageCallBack(RspFHLGetFinnallyScoreMessage message);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    public delegate void HandleFHLRspStartSucessMessageCallBack(RspFHLStartSucessMessage message);
    #endregion
    #region 身高
    public delegate void HandleSGTZRspMatchSucessMessageCallBack(RspSGTZMatchSucessMessage message);
    /// <summary>
    /// 
    /// </summary>
    public delegate void HandelSGTZRspGetFinnallyScoreMessageCallBack(RspSGTZGetFinnallyScoreMessage message);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    public delegate void HandleSGTZRspStartSucessMessageCallBack(RspSGTZStartSucessMessage message);
    #endregion
    /// <summary>
    /// 
    /// </summary>
    public interface ISerialPortServer
    {
        /// <summary>
        /// 
        /// </summary>
        event RecieveDataCallBack _RecieveDataCallBack;
        /// <summary>
        /// 
        /// </summary>
        event LogDataCallBack _LogDataCallBack;
        /// <summary>
        /// 
        /// </summary>
        event SendDataCallBack _SendDataCallBack;
        /// <summary>
        /// 
        /// </summary>
        event HandleFHLRspMatchSucessMessageCallBack _HandleFHLRspMatchSucessMessageCallBack;
        /// <summary>
        /// 
        /// </summary>
        event HandleFHLRspStartSucessMessageCallBack _HandleFHLRspStartSucessMessageCallBack;
        /// <summary>
        /// 
        /// </summary>
        event HandelFHLRspGetFinnallyScoreMessageCallBack _HandelFHLRspGetFinnallyScoreMessageCallBack;
        /// <summary>
        /// 
        /// </summary>
        event HandelSGTZRspGetFinnallyScoreMessageCallBack _HandelSGTZRspGetFinnallyScoreMessageCallBack;
        /// <summary>
        /// 
        /// </summary>
        event HandleSGTZRspMatchSucessMessageCallBack _HandleSGTZRspMatchSucessMessageCallBack;
        /// <summary>
        /// 
        /// </summary>
        event HandleSGTZRspStartSucessMessageCallBack _HandleSGTZRspStartSucessMessageCallBack;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="nBaudrate"></param>
        /// <returns></returns>
        bool OpenSerialPortConnection(string port, int nBaudrate = 115200);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        void CloseSerialPortConnection();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool IsSerialPortConnection();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void SendSerialPortMessage(byte[] message, int len = 0);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string[] GetPortBySerialPortName(string name);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        string PortNameToPort(string deviceName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="qtype"></param>
        void SetCurrentType(string qtype);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        void SetCurrentProjectType(int type);
    }
}
