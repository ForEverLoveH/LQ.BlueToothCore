using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LQ.BlueToothCore.Service.Service.StructService
{
    public class StructServer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="structObj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static byte[] StructToBytes<T>(T structObj) where T : struct
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(structObj);
            //创建byte数组
            byte[] bytes = new byte[size];
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将结构体拷到分配好的内存空间
            Marshal.StructureToPtr(structObj, structPtr, false);
            //从内存空间拷到byte数组
            Marshal.Copy(structPtr, bytes, 0, size);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回byte数组
            return bytes;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="startIndex"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T BytesToStuct<T>(byte[] bytes, int startIndex) where T : struct
        {
            //得到结构体的大小
            int size = Marshal.SizeOf<T>();
            //byte数组长度小于结构体的大小
            if (size > bytes.Length)
            {
                //返回空
                return default(T);
            }

            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将byte数组拷到分配好的内存空间
            Marshal.Copy(bytes, startIndex, structPtr, size);
            //将内存空间转换为目标结构体
            var re = Marshal.PtrToStructure<T>(structPtr);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回结构体
            return re;

        }
        /// <summary>
        /// 16进制数组转string 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToHexString(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2") + " ";
                }
            }
            return returnStr;
        }
        /// <summary>
        /// 字符串转byte[]
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static byte[] ConvertStringToHexBytes(string message)
        {
            byte[] hexBytes = Encoding.ASCII.GetBytes(message);

            hexBytes.Select(b => Convert.ToString(b, 16).PadLeft(2, '0'))
                .SelectMany(hex => hex)
                .Select(hex => byte.Parse(hex.ToString(), System.Globalization.NumberStyles.HexNumber))
                .ToArray();
            return hexBytes;
        }
    }
}
