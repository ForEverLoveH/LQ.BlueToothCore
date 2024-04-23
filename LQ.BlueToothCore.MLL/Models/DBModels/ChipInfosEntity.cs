using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LQ.BlueToothCore.MLL.Models.DBModels
{
    public class ChipInfosEntity
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public int Id { get; set; }

        [Column(IsNullable = false)]
        public int ProjectID { get; set; }

        public string ChipLabel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int projectType { get; set; }

       

        /// <summary>
        ///
        /// </summary>
        public string GroupName { get; set; }

        public int ChipSort { get; set; }
    }
}
