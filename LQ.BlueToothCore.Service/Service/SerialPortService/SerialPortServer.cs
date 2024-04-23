using LQ.BlueToothCore.IService.IService.ISerialPortService;
using LQ.BlueToothCore.MLL.RspMessage;
using LQ.BlueToothCore.MLL.RspMessage.SGTZ;
using LQ.BlueToothCore.Service.Service.StructService;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web.UI;

namespace LQ.BlueToothCore.Service.Service.SerialPortService
{
    /// <summary>
    /// 
    /// </summary>
    public class SerialPortServer : ISerialPortServer
    {
        

        /// <summary>
        /// 
        /// </summary>
        private SerialPort _serialPort;
        /// <summary>
        /// 
        /// </summary>
        private System.Timers.Timer waitTimer;
        /// <summary>
        /// 
        /// </summary>
        private System.Timers.Timer handleTimer;
        /// <summary>
        /// 
        /// </summary>
        private byte[] s232Buffer = new byte[2048 * 6];
        /// <summary>
        /// 
        /// </summary>
        private int s232Buffersp = 0;
        /// <summary>
        /// 二次缓存数据
        /// </summary>
        public List<byte> _buffer = new List<byte>();
        /// <summary>
        /// 
        /// </summary>
        private bool IsHandleData = false;
        /// <summary>
        /// 
        /// </summary>
        private string qtype;
        /// <summary>
        /// 
        /// </summary>
        private int projectType;
        /// <summary>
        /// 
        /// </summary>
        public event RecieveDataCallBack _RecieveDataCallBack;
        /// <summary>
        /// 
        /// </summary>
        public event LogDataCallBack _LogDataCallBack;
        /// <summary>
        /// 
        /// </summary>
        public event SendDataCallBack _SendDataCallBack;
        /// <summary>
        /// 
        /// </summary>
        public event HandleFHLRspMatchSucessMessageCallBack _HandleFHLRspMatchSucessMessageCallBack;
        /// <summary>
        /// 
        /// </summary>
        public event HandleFHLRspStartSucessMessageCallBack _HandleFHLRspStartSucessMessageCallBack;
        /// <summary>
        /// 
        /// </summary>
        public event HandelFHLRspGetFinnallyScoreMessageCallBack _HandelFHLRspGetFinnallyScoreMessageCallBack;
        /// <summary>
        /// 
        /// </summary>
        public event HandelSGTZRspGetFinnallyScoreMessageCallBack _HandelSGTZRspGetFinnallyScoreMessageCallBack;
        /// <summary>
        /// 
        /// </summary>
        public event HandleSGTZRspMatchSucessMessageCallBack _HandleSGTZRspMatchSucessMessageCallBack;
        /// <summary>
        /// 
        /// </summary>
        public event HandleSGTZRspStartSucessMessageCallBack _HandleSGTZRspStartSucessMessageCallBack;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public void SetCurrentType(string type)
        {
            this.qtype = type;
        }

        private byte[] FirstCode;
        /// <summary>
        /// 
        /// </summary>
        public SerialPortServer()
        {
            _serialPort = new SerialPort();
            _serialPort.DataReceived += RecieveDataFromSerialPort;
            ReadFristCode();
        }

        private void ReadFristCode()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecieveDataFromSerialPort(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int nCount = _serialPort.BytesToRead;
                if (nCount == 0) return;
                byte[] data = new byte[nCount];
                _serialPort.Read(data, 0, nCount);
                for (int i = 0; i < nCount; i++)
                {
                    s232Buffer[s232Buffersp] = data[i];
                    if (s232Buffersp < s232Buffer.Length - 2) s232Buffersp++;
                }
            }catch (Exception ex)
            {
                _LogDataCallBack?.Invoke(ex.Message);
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void CloseSerialPortConnection()
        {
            if(IsSerialPortConnection())_serialPort.Close();
            if(waitTimer!=null&&waitTimer.Enabled){ waitTimer.Stop();waitTimer = null; }
            if (handleTimer!=null&&handleTimer.Enabled) { handleTimer.Stop(); handleTimer = null; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="nBaudrate"></param>
        /// <returns></returns>
        public bool OpenSerialPortConnection(string port, int nBaudrate = 115200)
        {
            try
            {
                if (IsSerialPortConnection()) _serialPort.Close();
                _serialPort.PortName = port;
                _serialPort.BaudRate = nBaudrate;
                _serialPort.StopBits = StopBits.One;
                _serialPort.Parity = Parity.None;
                _serialPort.ReadTimeout = 10;
                _serialPort.WriteTimeout = 1000;
                _serialPort.ReadBufferSize = 4096 * 10;
                _serialPort.Open();
                OpenWaitTimer();
                OpenHanleBytesTimers();
                return true;
            }
            catch (Exception e)
            {
                _LogDataCallBack?.Invoke(e.Message);
                return false;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        private void OpenHanleBytesTimers()
        {
            try
            {
                if (handleTimer != null)
                {
                    if (handleTimer.Enabled) handleTimer.Enabled = false;
                    handleTimer.Dispose();
                    handleTimer = null;
                }
                handleTimer = new Timer(10);
                handleTimer.Elapsed += new System.Timers.ElapsedEventHandler(HandleBytesData);
                handleTimer.AutoReset = true;
                handleTimer.Enabled = true;
                handleTimer.Start();
            }
            catch (Exception e)
            {
                _LogDataCallBack?.Invoke(e.Message);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleBytesData(object sender, ElapsedEventArgs e)
        {
            try
            {
                IsHandleData = true;
                if (handleTimer != null)
                    handleTimer.Stop();
                int  len = _buffer.Count;
                if(len==0) return;
                else
                {
                    if (projectType == 0)
                    {
                        while (len > 3)
                        {
                            
                            //第一步先判断是不是有效数据包
                            if (_buffer[0] == 0x46 && _buffer[1] == 0x48 && _buffer[2] == 0x4c)
                            {
                                if (qtype != null)
                                {
                                    if (qtype == "MATCH")
                                    {
                                        // 首先应该先去切割字符这个buffer 70, 72, 76, 0, 1, 109, 97, 116, 99, 104, 111, 107, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
                                        List<List<byte>> pps = ChunkList(_buffer, 23);
                                        List<List<byte>> realByte = new List<List<byte>>();
                                        if (pps.Count > 0)
                                        {
                                            foreach (var data in pps)
                                            {
                                                if (data[0] == 0x46 && data[1] == 0x48 && data[2] == 0x4c)
                                                {
                                                    if (!realByte.Contains(data)) realByte.Add(data);
                                                    else continue;
                                                }
                                                else continue;
                                            }
                                        }
                                        int lens = realByte.Count;
                                        if (lens > 0)
                                        {
                                            foreach (var data in realByte)
                                            {
                                                var messages = CutTail(data);
                                                if (messages != null && messages.Length == 12)
                                                {
                                                    if (VerficationCrcCode(messages))
                                                    {
                                                        RspFHLMatchSucessMessage mess = StructServer.BytesToStuct<RspFHLMatchSucessMessage>(messages, 0);
                                                        _HandleFHLRspMatchSucessMessageCallBack?.Invoke(mess);
                                                    }

                                                }
                                                else continue;
                                            }

                                        }
                                    }
                                    else if (qtype == "START")
                                    {
                                        List<List<byte>> pps = ChunkList(_buffer, 23);
                                        List<List<byte>> realByte = new List<List<byte>>();
                                        if (pps.Count > 0)
                                        {
                                            foreach (var data in pps)
                                            {
                                                if (data[0] == 0x46 && data[1] == 0x48 && data[2] == 0x4c)
                                                {
                                                    if (!realByte.Contains(data)) realByte.Add(data);
                                                    else continue;
                                                }
                                                else continue;
                                            }
                                        }
                                        int lens = realByte.Count;
                                        if (lens > 0)
                                        {

                                            foreach (var data in realByte)
                                            {
                                                var messages = CutTail(data);

                                                if (messages != null && messages.Length == 12)
                                                {
                                                    if (VerficationCrcCode(messages))
                                                    {
                                                        RspFHLStartSucessMessage mess = StructServer.BytesToStuct<RspFHLStartSucessMessage>(messages, 0);
                                                        _HandleFHLRspStartSucessMessageCallBack?.Invoke(mess);
                                                    }

                                                }
                                            }

                                        }
                                    }
                                    else
                                    {
                                        List<List<byte>> pps = ChunkList(_buffer, 22);
                                        List<List<byte>> realByte = new List<List<byte>>();
                                        if (pps.Count > 0)
                                        {
                                            foreach (var data in pps)
                                            {
                                                if (data[0] == 0x46 && data[1] == 0x48 && data[2] == 0x4c)
                                                {
                                                    if (!realByte.Contains(data)) realByte.Add(data);
                                                    else continue;
                                                }
                                                else continue;
                                            }
                                        }
                                        int lens = realByte.Count;
                                        if (lens > 0)
                                        {

                                            foreach (var data in realByte)
                                            {
                                                var messages = CutTail(data);
                                                if (messages != null && messages.Length == 11)
                                                {
                                                    if (VerficationCrcCode(messages))
                                                    {
                                                        RspFHLGetFinnallyScoreMessage mess = StructServer.BytesToStuct<RspFHLGetFinnallyScoreMessage>(messages, 0);
                                                        _HandelFHLRspGetFinnallyScoreMessageCallBack?.Invoke(mess);
                                                    }

                                                }
                                            }
                                        }
                                    }
                                }else
                                    return;
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                    else
                    {
                        while(len>4)
                        {
                            if (_buffer[0] == 0x83 && _buffer[1] == 0x71 && _buffer[2] == 0x84 && _buffer[3] == 0x90)
                            {
                                if (qtype != null)
                                {
                                    if (qtype == "MATCH")
                                    {
                                        List<List<byte>> pps = ChunkList(_buffer, 24);
                                        List<List<byte>> realByte = new List<List<byte>>();
                                        if (pps.Count > 0)
                                        {
                                            foreach (var data in pps)
                                            {
                                                if (data[0] == 0x83 && data[1] == 0x71 && data[2] == 0x84 && data[3] == 0x90)
                                                {
                                                    if (!realByte.Contains(data)) realByte.Add(data);
                                                    else continue;
                                                }
                                                else continue;
                                            }
                                        }
                                        int lens = realByte.Count;
                                        if (lens > 0)
                                        {

                                            foreach (var data in realByte)
                                            {
                                                var messages = CutTail(data);

                                                if (messages != null && messages.Length == 11)
                                                {
                                                    if (VerficationCrcCode(messages))
                                                    {
                                                        RspSGTZMatchSucessMessage mess = StructServer.BytesToStuct<RspSGTZMatchSucessMessage>(messages, 0);
                                                        _HandleSGTZRspMatchSucessMessageCallBack?.Invoke(mess);
                                                    }

                                                }
                                            }
                                        }
                                    }
                                    else if (qtype == "START")
                                    {
                                        List<List<byte>> pps = ChunkList(_buffer, 24);
                                        List<List<byte>> realByte = new List<List<byte>>();

                                        if (pps.Count > 0)
                                        {
                                            foreach (var data in pps)
                                            {
                                                if (data[0] == 0x83 && data[1] == 0x71 && data[2] == 0x84 && data[3] == 0x90)
                                                {
                                                    if (!realByte.Contains(data)) realByte.Add(data);
                                                    else continue;
                                                }
                                                else continue;
                                            }
                                        }
                                        int lens = realByte.Count;
                                        if (lens > 0)
                                        {

                                            foreach (var data in realByte)
                                            {
                                                var messages = CutTail(data);

                                                if (messages != null && messages.Length == 11)
                                                {
                                                    if (VerficationCrcCode(messages))
                                                    {
                                                        RspSGTZStartSucessMessage mess = StructServer.BytesToStuct<RspSGTZStartSucessMessage>(messages, 0);
                                                        _HandleSGTZRspStartSucessMessageCallBack?.Invoke(mess);
                                                    }
                                               
                                                }
                                            }


                                        }

                                    }
                                    else
                                    {
                                        List<List<byte>> pps = ChunkList(_buffer, 22);
                                        var messages = CutTail(_buffer);
                                        if (messages != null && messages.Length == 11)
                                        {
                                            RspSGTZGetFinnallyScoreMessage mess = StructServer.BytesToStuct<RspSGTZGetFinnallyScoreMessage>(messages, 0);
                                            _HandelSGTZRspGetFinnallyScoreMessageCallBack?.Invoke(mess);
                                            try
                                            {
                                                _buffer.RemoveRange(0, len);
                                                qtype = "";
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine(ex.Message);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                }

            }
            finally
            {

                if (handleTimer != null)handleTimer.Start();
                else
                {
                    OpenHanleBytesTimers();
                }
                _buffer.Clear();
                
                IsHandleData = false;
            }
        }
        /// <summary>
        /// 校验crc
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        private bool VerficationCrcCode(byte[] data)
        {
            
           if(data==null||data.Length==0) return false;
            byte checkSum = data[0];
            for (int i = 1;i< data.Length-1; i++)
            {
                checkSum += data[i];
            }
            byte sum =(byte)(checkSum + 0xaa);
            return   sum== data[data.Length - 1];

        }

        /// <summary>
        /// 除去字节数组后没有的0x00
        /// </summary>
        /// <param name="byList"></param>
        /// <returns></returns>
        private byte[] CutTail(List<byte> byList)
        {
            int j = 0;
            byte[] tempb = null;
            for (int i = byList.Count - 1; i >= 0; i--)
            {
                if (byList[i] != 0x00 & j == 0)
                {
                    j = i;
                    if (tempb == null) tempb = new byte[j + 1];  
                    tempb[j] = byList[i];
                    j--;
                }
                else
                {
                    if (tempb != null)
                    {
                        tempb[j] = byList[i];
                        j--;
                    }
                }
            }
            return tempb;
        }
        

        /// <summary>
        /// 
        /// </summary>
        private void OpenWaitTimer()
        {
            if (waitTimer != null)
            {
                if (waitTimer.Enabled) waitTimer.Stop();
                waitTimer.Dispose();
                waitTimer = null;
            }
            waitTimer = new Timer(1);
            waitTimer.Elapsed += new ElapsedEventHandler(AnalySerialPortReceivedData);
            waitTimer.AutoReset = true;
            waitTimer.Enabled = true;
            waitTimer.Start();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnalySerialPortReceivedData(object sender, ElapsedEventArgs e)
        {
            if (waitTimer != null) waitTimer.Stop();
            try
            {
                if (s232Buffersp != 0)
                {
                    byte[] btAryBuffer = new byte[s232Buffersp];
                    Array.Copy(s232Buffer, 0, btAryBuffer, 0, s232Buffersp);
                    Array.Clear(s232Buffer, 0, s232Buffersp);
                    s232Buffersp = 0;
                    _buffer.AddRange(btAryBuffer);
                    _RecieveDataCallBack?.Invoke(btAryBuffer);
                }
            }
            catch (Exception ex)
            {
                _LogDataCallBack?.Invoke(ex.Message);
                return;
            }
            finally
            {
                if (waitTimer != null) waitTimer.Start();
                else
                {
                    OpenWaitTimer();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsSerialPortConnection()
        {
            return _serialPort.IsOpen;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string[] GetPortBySerialPortName(string name)
        {
            List<string> sts = new List<string>();
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_PnPEntity where Name like '%(COM%'"))
            {
                var hardInfos = searcher.Get();
                foreach (var item in hardInfos)
                {
                    if (item.Properties["Name"].Value != null)
                    {
                        string deviceName = item.Properties["Name"].Value.ToString();
                        if (deviceName.Contains(name) || deviceName.Contains("Prolific"))
                        {
                            sts.Add(deviceName);
                        }
                    }
                }
            }
            return sts.ToArray();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        public string PortNameToPort(string deviceName)
        {
            string str = String.Empty;
            try
            {
                int a = deviceName.IndexOf("(COM") + 1;//a会等于1
                str = deviceName.Substring(a, deviceName.Length - a);
                a = str.IndexOf(")");//a会等于1
                str = str.Substring(0, a);
            }
            catch (Exception exception)
            {
                return null;
            }
            return str;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="len"></param>
        public void SendSerialPortMessage(byte[] message, int len = 0)
        {
            if (!IsSerialPortConnection()) return;
            string pp = StructServer.ToHexString(message);
            _SendDataCallBack?.Invoke(pp);
            if (len == 0) len = message.Length;
            _serialPort.Write(message, 0, len);

        }
        /// <summary>
        /// 0 表示FHL 1 表示身高高
        /// </summary>
        /// <param name="type"></param>
        public void SetCurrentProjectType(int type)
        {
            this.projectType = type;
        }
        /// <summary>
        /// 切割字节数组
        /// </summary>
        /// <param name="byteList"></param>
        /// <param name="chunkSize">需要切割的长度</param>
        /// <returns></returns>
        private List<List<byte>> ChunkList(List<byte> byteList, int chunkSize)
        {
            List<List<byte>> chunkedList = new List<List<byte>>();
            int index = 0;
            while (index < byteList.Count)
            {
                chunkedList.Add(byteList.GetRange(index, Math.Min(chunkSize, byteList.Count - index)));
                index += chunkSize;
            }
            return chunkedList;
        }
    }
}

        
    
