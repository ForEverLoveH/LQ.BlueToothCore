using LQ.BlueToothCore.IService.IService.IFreeSqlService;
using LQ.BlueToothCore.MLL.Models;
using LQ.BlueToothCore.MLL.Models.DBModels;
 
using LQ.BlueToothCore.Server.Service;
using LQ.BlueToothCore.Service.Service.FreeSqlService;
using LQ.BlueToothCore.Service.Service.ManagerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LQ.BlueToothCore.Views.WindowSys
{
    public class MainWindowSys
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IFreeSqlServer freeSqlServer= new  FreeSqlServer();
         
        private TreeViewService TreeViewService= new TreeViewService();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<MainTreeViewModel> LoadingMainTreeViewModel()
        {
             return TreeViewService.LoadingProjectDataModel();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<ProjectInfosEntity> LoadingProjectData()
        {
            return freeSqlServer.FindList<ProjectInfosEntity>(X=>X.ID);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectType"></param>
        /// <param name="projectId"></param>
        /// <param name="groupName"></param>
        /// <param name="roundCount"></param>
        /// <returns></returns>
        public List<Students> LoadingCurrentStudentData(int projectType, int projectId, string groupName,int roundCount)
        {
            if (String.IsNullOrEmpty(groupName)) return null;
            List<Students> students = new List<Students>();
            try
            {
                List<StudentDataEntity> studentDataEntities = freeSqlServer.FindList<StudentDataEntity>(x => x.Id, x => x.ProjectId.Equals(projectId)&&x.GroupName==groupName);
                if (studentDataEntities.Count > 0)
                {
                    foreach (var student in studentDataEntities)
                    {
                        Students studentDataModel = new Students();
                        studentDataModel.studentName = student.Name;
                        studentDataModel.idNumber = student.IdNumber;
                        studentDataModel.schoolName = student.SchoolName;
                        studentDataModel.sex = student.Sex == 0 ? "男" : "女";
                        studentDataModel.personId = student.Id.ToString();
                        studentDataModel.className = student.ClassNumber;
                        studentDataModel.gradeName = student.GradeName;
                        studentDataModel.groupName = student.GroupName;
                        if (projectType == 0) //fhl
                        {
                            List<FHLScoreData> scoreData = new List<FHLScoreData>();
                            List<StudentScoreEntity> studentScoreEntities =
                                 freeSqlServer.FindList<StudentScoreEntity>(x => x.Id, x => x.PersonIdNumber == student.IdNumber && x.ProjectID == projectId &&
                                                                               x.PersonName == student.Name && x.SportItemType == 4);
                            if (studentScoreEntities.Count > 0)
                            {
                                foreach (var item in studentScoreEntities)
                                {
                                    int state = item.State;
                                    double score = item.Score;
                                    int roundCounts = item.RoundId;
                                    FHLScoreData fhlScoreData = new FHLScoreData();
                                    fhlScoreData.roundCount = roundCounts;
                                    if (state == 1) fhlScoreData.score = score.ToString();
                                    else
                                    {
                                        string pp = ResultStateType.Match(state);
                                        fhlScoreData.score = pp;
                                    }
                                    scoreData.Add(fhlScoreData);
                                }

                            }
                            else
                            {
                                for (int i = 0; i < roundCount; i++)
                                {
                                    scoreData.Add(new FHLScoreData()
                                    {
                                        roundCount = i + 1,
                                        score = "未测试",
                                    });
                                }
                            }
                            studentDataModel.FhlScoreData = scoreData;
                        }
                        else
                        {
                            List<SGTZScoreData> scoreData = new List<SGTZScoreData>();
                            List<StudentScoreEntity> studentScoreEntities =
                                freeSqlServer.FindList<StudentScoreEntity>(x => x.Id, x => x.PersonIdNumber == student.IdNumber && x.ProjectID == projectId &&
                                    x.PersonName == student.Name);
                            if (studentScoreEntities.Count > 0)
                            {
                                var data = studentScoreEntities.Find(a => a.SportItemType == 4);
                                if (data != null) studentScoreEntities.Remove(data);
                                if (studentScoreEntities.Count > 0)
                                {
                                    var groups = studentScoreEntities.GroupBy(a => a.RoundId);
                                    List<int> roundss = new List<int>();
                                    foreach (var item in groups)
                                    {
                                        roundss.Add(item.Key);
                                    }
                                    foreach (var key in roundss)
                                    {
                                        SGTZScoreData SGTZScoreData = new SGTZScoreData();
                                        SGTZScoreData.roundCount = key;
                                        var stu = studentScoreEntities.FindAll(a => a.RoundId == key);
                                        if (stu.Count > 0)
                                        {
                                            foreach (var st in stu)
                                            {
                                                int state = st.State;
                                                double score = st.Score;
                                                if (state == 1)
                                                {
                                                    if (st.SportItemType == 0) SGTZScoreData.score1 = score.ToString();
                                                    else if (st.SportItemType == 1)
                                                        SGTZScoreData.score2 = score.ToString();
                                                    else
                                                    {
                                                        SGTZScoreData.score3 = score.ToString();
                                                    }

                                                }
                                                else
                                                {
                                                    string pp = ResultStateType.Match(state);
                                                    if (st.SportItemType == 0) SGTZScoreData.score1 = pp;
                                                    else if (st.SportItemType == 1)
                                                        SGTZScoreData.score2 = pp;
                                                    else
                                                    {
                                                        SGTZScoreData.score3 = pp;
                                                    }

                                                }
                                            }
                                        }
                                        scoreData.Add(SGTZScoreData);
                                    }

                                }
                                else
                                {
                                    continue;
                                }

                            }
                            else
                            {
                                for (int i = 0; i < roundCount; i++)
                                {
                                    scoreData.Add(new SGTZScoreData()
                                    {
                                        roundCount = i + 1,
                                        score1 = "未测试",
                                        score2 = "未测试",
                                        score3 = "未测试",
                                    });
                                }
                            }
                            studentDataModel.SgtzScoreData = scoreData;
                        }
                        students.Add(studentDataModel);

                    }

                    return students;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e);
                return null;
            }
        }
    }
}
