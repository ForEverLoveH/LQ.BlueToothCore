using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LQ.BlueToothCore.MLL.Models.ViewModels
{
    public class ResultType
    {
        public static int NoTest = 0;//未测试
        public static int Test = 1;//已测试
        public static int Withdrawal = 2;//中退
        public static int MissTest = 3;//缺考
        public static int Foul = 4;//犯规 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static string ResultState2Str(int state)
        {
            switch (state)
            {
                case 0:
                    return "未测试";
                case 1:
                    return "已测试";
                case 2:
                    return "中退";
                case 3:
                    return "缺考";
                case 4:
                    return "犯规";
                default:
                    return "";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="state0"></param>
        /// <returns></returns>
        public static string ResultState2Str(string state0)
        {
            int.TryParse(state0, out int state);
            switch (state)
            {
                case 0:
                    return "未测试";
                case 1:
                    return "已测试";
                case 2:
                    return "中退";
                case 3:
                    return "缺考";
                case 4:
                    return "犯规";
                default:
                    return "";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static int ResultState2Int(string state)
        {
            switch (state)
            {
                case "未测试":
                    return NoTest;
                case "已测试":
                    return Test;
                case "中退":
                    return Withdrawal;
                case "缺考":
                    return MissTest;
                case "犯规":
                    return Foul;
                default:
                    return 0;
            }
        }
    }
}
