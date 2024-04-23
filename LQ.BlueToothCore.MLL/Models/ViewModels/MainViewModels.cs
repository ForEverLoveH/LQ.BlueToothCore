using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LQ.BlueToothCore.MLL.Models.ViewModels
{
    public class MainViewModels
    {
        public string projectName { get; set; }
        public string SchoolName { get; set; }
        public string GradeName { get; set; }
        public string ClassNumber { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public string IDNumber { get; set; }
        public string GroupName { get; set; }
        public SGResultData sGResultDatas { get; set; }
        public FHResultData fHResultData { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class FHResultData
    {
        public List<FHResultItem> fHResultItems { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class FHResultItem
    {
        public int type { get; set; }
        public int roundCount { get; set; }
        public object score { get; set; }
        public int upLoadState { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class SGResultData
    {
        public List<SGResultDataItem> sGResultDatas { get; set; }
    }

    public class SGResultDataItem
    {
        public int roundCount { get; set; }
        //0 表示身高成绩 1 表示体重成绩 2 表示BMI   
        public List<BaseScores> Scores { get; set; }
    }

    public class  BaseScores
    {
        public int type { get; set; }
        public object score { get; set; }
        public int upLoadState { get; set; }
    }
}

