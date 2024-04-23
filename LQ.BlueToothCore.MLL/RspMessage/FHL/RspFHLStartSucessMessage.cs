using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LQ.BlueToothCore.MLL.RspMessage
{
    public struct RspFHLStartSucessMessage
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
}
