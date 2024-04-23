using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LQ.BlueToothCore.MLL.RspMessage.SGTZ
{
    public struct RspSGTZMatchSucessMessage
    {
        /// <summary>
        ///  头码 SGTZ
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] headerCode;
        /// <summary>
        /// 设备码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] equipMentCode;
        /// <summary>
        ///  命令 码  match
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public byte[] cmdCode;
        /// <summary>
        /// ok
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] stateCode;
    }
    /// <summary>
    /// 
    /// </summary>
    public struct RspSGTZStartSucessMessage
    {
        /// <summary>
        ///  头码 SGTZ
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] headerCode;
        /// <summary>
        /// 设备码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] equipMentCode;
        /// <summary>
        ///  命令 码  start
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public byte[] cmdCode;
        /// <summary>
        /// ok
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] stateCode;
    }
    /// <summary>
    /// 
    /// </summary>
    public struct RspSGTZGetFinnallyScoreMessage
    {
        /// <summary>
        ///  头码 sgtz
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] headerCode;
        /// <summary>
        /// 设备码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] equipMentCode;
        /// <summary>
        /// 分数
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)] // 身高 五个字节 体重 4 个字节
        public byte[] scoreCode;
        /// <summary>
        /// 流水号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte liuCode;
        /// <summary>
        /// 校验码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] crcCode;

    }
}
