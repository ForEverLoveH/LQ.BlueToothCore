using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LQ.BlueToothCore.MLL.RspMessage
{
    /// 成绩：70, 72, 76,  0, 1, 0,   6, 5, 0, 0, 29, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 
    ///        F  H   L   (设备) 千   百 十 个 流 校验
    public struct RspFHLGetFinnallyScoreMessage
    {
        /// <summary>
        ///  头码 FHL
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] headerCode;
        /// <summary>
        /// 设备码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] equipMentCode;
        /// <summary>
        /// 分数
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] scoreCode;
        /// <summary>
        /// 流水号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte liuCode;
        /// <summary>
        /// 校验码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] crcCode;

    }
}
