using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LQ.BlueToothCore.MLL.Models.DBModels
{
    /// <summary>
    /// 
    /// </summary>
    public class ProjectInfosEntity
    {
        [Column(IsPrimary =true,IsIdentity =true)]
        public int ID { get; set; }
        
        public int SortID { get; set; }
        public string CreateTime { get; set; }
        public int IsRemoved { get; set; }
        public string projectName { get; set; }
        //1 表示身高 2 表示肺活量
        public  int Type {  get; set; } 
        public int RoundCount { get; set; }
        public int BestScoreMode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int TestMethod { get; set; }

        public int FloatType { get; set; }

        public int TurnsNumber0 { get; set; }

        public int TurnsNumber1 { get; set; }
    }
}
