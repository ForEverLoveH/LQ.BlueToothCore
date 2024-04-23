using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LQ.BlueToothCore.MLL.Models.DBModels
{
    public class StudentDataEntity
    {

        [Column(IsIdentity = true, IsPrimary = true)]
        public int Id { get; set; }

        public string CreateTime { get; set; }

        public int SortId { get; set; }

        public int IsRemoved { get; set; }

        public string ProjectId { get; set; }

        public string SchoolName { get; set; }

        public string GradeName { get; set; }

        public string ClassNumber { get; set; }

        public string GroupName { get; set; }

        public string Name { get; set; }

        public string IdNumber { get; set; }

        public int Sex { get; set; }
        

    }
}
