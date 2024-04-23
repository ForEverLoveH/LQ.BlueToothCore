using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LQ.BlueToothCore.MLL.Models.DBModels
{
    public class StudentScoreEntity
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public int Id { get; set; }

        public string CreateTime { get; set; }

        public int SortId { get; set; }

        public int ProjectID { get; set; }

        public int IsRemoved { get; set; }

        public int PersonId { get; set; }
        /// <summary>
        /// 0 身高 1 体重2 bmi 4 肺活量
        /// </summary>
        public int SportItemType { get; set; }

        public string PersonName { get; set; }

        public string PersonIdNumber { get; set; }

        public int RoundId { get; set; }
       
        public double Score { get; set; }
         

        public int State { get; set; }

        public int uploadState { get; set; }
    }
}
