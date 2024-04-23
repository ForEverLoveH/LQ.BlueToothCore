using System.Collections.Generic;

namespace LQ.BlueToothCore.MLL.Models
{
    public class StudentDataModel
    {
        public string personId { get; set; }
        public string schoolName { get; set; }
        public string gradeName { get; set; }
        public string className { get; set; }
        public string groupName { get; set; }
        public string studentName { get; set; }
        public string sex { get; set; }
        public string idNumber { get; set; }
        public SGTZScoreData SgtzScoreData { get; set; }
        public  FHLScoreData FhlScoreData { get; set; }
        public byte[] equipmentCode { get; set; }   
        public bool isTest { get; set; }
        
    }
    /// <summary>
    /// 
    /// </summary>
    public class SGTZScoreData
    {
        public int roundCount { get; set; }
        public string score1 { get; set; }
        public string score2 { get; set; }
        public string score3 { get; set; }
    }

    public class FHLScoreData
    {
        public int roundCount { get; set; }
        public string score { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class Students
    {
        public string personId { get; set; }
        public string schoolName { get; set; }
        public string gradeName { get; set; }
        public string className { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string groupName { get; set; }
        public string studentName { get; set; }
        public string sex { get; set; }
        public string idNumber { get; set; }
        public List<SGTZScoreData> SgtzScoreData { get; set; }
        public List<FHLScoreData> FhlScoreData { get; set; }
    }
}