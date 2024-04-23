using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LQ.BlueToothCore.Service.Service.FreeSqlService
{
    public class BaseServer
    {
        static string connection;
        static BaseServer()
        {
            connection = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataBase/DBBlueCore.db");
        }
        /// <summary>
        /// 
        /// </summary>
        private static Lazy<IFreeSql> ServersqlLazy = new Lazy<IFreeSql>(() => new FreeSql.FreeSqlBuilder()
            .UseMonitorCommand(cmd => Trace.WriteLine($"Sql：{cmd.CommandText}"))//监听SQL语句,Trace在输出选项卡中查看
            .UseConnectionString(FreeSql.DataType.Sqlite, $"Data Source={connection}")
            .UseAutoSyncStructure(true) //自动同步实体结构到数据库，FreeSql不会扫描程序集，只有CRUD时才会生成表。
            .Build());
        /// <summary>
        /// 
        /// </summary>
        public static IFreeSql sqlite => ServersqlLazy.Value;
    }
}
