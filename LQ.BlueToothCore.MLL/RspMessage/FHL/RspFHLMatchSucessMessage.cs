using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LQ.BlueToothCore.MLL.RspMessage
{
    //
    /// 匹配成功 70, 72, 76,  0, 1, 109, 97, 116, 99, 104, 111, 107, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
    //           F   H    L  设备码 M   A   T    C   H    O     K
    public struct RspFHLMatchSucessMessage
    {
        /// <summary>
        ///  头码 FHL
        /// </summary>
        [MarshalAs(UnmanagedType. ByValArray, SizeConst = 3)]
        public byte[] headerCode;
        /// <summary>
        /// 设备码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] equipMentCode;
        /// <summary>
        ///  命令 码  match
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst =5)]
        public byte[] cmdCode;
        /// <summary>
        /// ok
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray ,SizeConst =2)]
        public byte[] stateCode;
    }
}
