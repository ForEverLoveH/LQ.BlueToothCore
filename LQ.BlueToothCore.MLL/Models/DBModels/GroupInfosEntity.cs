using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LQ.BlueToothCore.MLL.Models.DBModels
{
    public class GroupInfosEntity
    {
        [Column(IsPrimary = true, IsIdentity = true)]
        public int ID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int SortId { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public int IsRemoved { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ProjectId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int IsAllTested { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public int State { get; set; }
    }
}
