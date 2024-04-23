using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LQ.BlueToothCore.MLL.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public class ReqBlueMessage
    {
        private int type;
        /// <summary>
        /// 头码
        /// </summary>
        private byte[] headerCode;

        public byte[] HeaderCode;
        /// <summary>
        /// 项目码
        /// </summary>
        private byte[] projectCode;
        /// <summary>
        /// 设备码
        /// </summary>
        private byte[] equipMentCode;
        /// <summary>
        /// 类型码
        /// </summary>
        private byte typeCode;
        /// <summary>
        /// 命令码
        /// </summary>
        private byte[]cmdCode;
        /// <summary>
        /// 流水号
        /// </summary>
        private byte[] currentRunningWaterCode;
        /// <summary>
        /// crc8 
        /// </summary>
        private byte[] crcCode;
        /// <summary>
        /// 
        /// </summary>
        private byte[] finnallyCode;
        /// <summary>
        /// 
        /// </summary>
        public byte[] FinnallyCode
        {
            get=>finnallyCode;
        }

        private FristHeaderCode fristHeaderCode;
        
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">0 表示身高 1 表示肺活量
        /// <param name="currentRunningWaterCode">流水号</param>
        /// <param name="typeCode">命令类型</param>
        /// <param name="cmdCode">命令码</param>
        /// <param name="headerCode">头码</param>
        /// <param name="projectCode">项目码</param>
        /// <param name="equipMentCode">设备码</param>
        public ReqBlueMessage(int type,byte[] currentRunningWaterCode, byte typeCode, byte[] cmdCode, byte[] headerCode = null, byte[] projectCode = null, byte[] equipMentCode = null)
        {
            this.type = type;
            this.currentRunningWaterCode = currentRunningWaterCode;
            this.cmdCode = cmdCode;
            this.typeCode = typeCode;
            ReadHeaderCode();
           // this.headerCode = headerCode==null ? new byte[] { (byte)'A', (byte)'D', 4, (byte)'P', (byte)'L', (byte)'L', (byte)'Z', typeCode } : headerCode;
            if (type == 0)this.projectCode = projectCode == null ?   Encoding.ASCII.GetBytes("FHL"):projectCode;
            else
            {
                this.projectCode = projectCode==null?  Encoding.ASCII.GetBytes("SGTZ"):projectCode;
            }
            this.equipMentCode = equipMentCode;
            CreteCrcCode();
        }

        private void ReadHeaderCode()
        {
            string path = @"Config/Log4Net.Config";
            if (!File.Exists(path))
            {
                string json = File.ReadAllText(path);
                FristHeaderCode headerCode = JsonConvert.DeserializeObject<FristHeaderCode>(json);
                
               


            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CreteCrcCode()
        {
            byte[] messages;
            bool flag=false;
            int len =0;
            if(equipMentCode==null)
            {
                len=headerCode.Length+ projectCode.Length + cmdCode.Length + currentRunningWaterCode.Length;
                messages = new byte[len];
            }
            else
            {
                len= headerCode.Length + projectCode.Length + equipMentCode.Length + cmdCode.Length + currentRunningWaterCode.Length;
                messages = new byte[len];
                flag = true;
            }
            Array.Copy(headerCode, 0, messages, 0, headerCode.Length);
            Array.Copy(projectCode, 0, messages, headerCode.Length, projectCode.Length);
            if (flag)
            {
                Array.Copy(equipMentCode, 0, messages, headerCode.Length + projectCode.Length, projectCode.Length);
                Array.Copy(cmdCode, 0, messages, headerCode.Length + projectCode.Length + equipMentCode.Length, cmdCode.Length);
                Array.Copy(currentRunningWaterCode, 0, messages, headerCode.Length + projectCode.Length + equipMentCode.Length + cmdCode.Length, currentRunningWaterCode.Length);
            }
            else
            {
                Array.Copy(cmdCode, 0, messages, headerCode.Length + projectCode.Length, cmdCode.Length);
                Array.Copy(currentRunningWaterCode, 0, messages, headerCode.Length + projectCode.Length + cmdCode.Length, currentRunningWaterCode.Length);
            }
            byte code = CalculatSum(messages);
            crcCode= new byte[] {code};
            finnallyCode= messages.Concat(crcCode).ToArray();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        private byte CalculatSum(byte[] messages)
        {
            int sum = 0x00;
           // if(messages.Length > 0) messages[messages.Length - 1] = 0;    
            for (int i = 0; i < messages.Length; i++)
            {
                sum += messages[i];
            }
            sum = sum + 0xaa;
           // sum = sum % 256;
            return (byte)sum;
        }
    }
}
