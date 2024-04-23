using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LQ.BlueToothCore.MLL.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class MainTreeViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string projectName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ExamTimeModel> examTimeModel { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ExamTimeModel
    {
        public string createTime { get; set; }
        public List<GroupsData > groupsDatas { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class GroupsData
    {
        public int isAllTest { get; set; }
        public string groupName { get; set; }
    }
    
}
