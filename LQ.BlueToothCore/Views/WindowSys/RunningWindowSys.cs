using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Forms;
using FreeSql.DatabaseModel;
using LQ.BlueToothCore.IService.IService.IFreeSqlService;
using LQ.BlueToothCore.MLL.Models;
using LQ.BlueToothCore.MLL.Models.DBModels;
using LQ.BlueToothCore.Service.Service.FreeSqlService;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Spire.Xls;
using Sunny.UI;

namespace LQ.BlueToothCore.Views.WindowSys
{
    public class RunningWindowSys
    {
        private readonly IFreeSqlServer freeSqlServer = new FreeSqlServer();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public ProjectInfosEntity LoadingProjectInfos(string projectName)
        {
            return freeSqlServer.Find<ProjectInfosEntity>(A => A.projectName.Equals(projectName));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public List<string> LoadingProjectGroupData(int projectId)
        {
            List<string> LP = new List<string>();
            var groupInfos = freeSqlServer.FindList<GroupInfosEntity>(x=>x.ID,x => x.ProjectId.Equals(projectId));
            if (groupInfos.Count > 0)
            {
                foreach (var pp in groupInfos)
                {
                    LP.Add(pp.Name);
                }

                return LP;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectType"></param>
        /// <param name="projectId"></param>
        /// <param name="groupName"></param>
        /// <param name="currentRoundCount"></param>
        /// <returns></returns>
        public List<StudentDataModel> LoadingCurrentStudentData(int projectType, int projectId, string groupName, int currentRoundCount)
        {
            List<StudentDataModel> students = new List<StudentDataModel>();
            try
            {
                List<StudentDataEntity> studentDataEntities = freeSqlServer.FindList<StudentDataEntity>(x=>x.Id,x => x.ProjectId.Equals(projectId) && x.GroupName.Equals(groupName));
                if (studentDataEntities.Count > 0)
                {
                    foreach (var student in studentDataEntities)
                    {
                        StudentDataModel studentDataModel = new StudentDataModel();
                        studentDataModel. studentName = student.Name;
                        studentDataModel.idNumber = student.IdNumber;
                        studentDataModel.schoolName = student.SchoolName;
                        studentDataModel.sex = student.Sex == 0 ? "男" : "女";
                        studentDataModel.personId = student.Id.ToString();
                        studentDataModel.className = student.ClassNumber;
                        studentDataModel.gradeName = student.GradeName;
                        studentDataModel.groupName = groupName;
                        
                        if (projectType==1)//SGTZ
                        {
                            SGTZScoreData sgtzScoreData = new SGTZScoreData();
                            List<StudentScoreEntity> studentScoreEntities =
                                freeSqlServer.FindList<StudentScoreEntity>(x => x.Id, x => x.PersonIdNumber == student.IdNumber && x.ProjectID == projectId &&
                                         x.PersonName == student.Name && x.RoundId == currentRoundCount);
                            if (studentScoreEntities.Count > 0&&studentScoreEntities.Count==3)
                            {
                                var pp = studentScoreEntities.OrderBy(x =>x.SportItemType ).ToList();
                                foreach (var scoreEntity in pp)
                                {
                                    int sportInfos = scoreEntity.SportItemType;
                                    int state = scoreEntity.State;
                                    double score = scoreEntity.Score;
                                    if (state == 1)
                                    {
                                        if (sportInfos == 0) sgtzScoreData.score1 = score.ToString();
                                        else if (sportInfos == 1) sgtzScoreData.score2 = score.ToString();
                                        else if (sportInfos == 2) sgtzScoreData.score3 = score.ToString();
                                    }
                                    else
                                    {
                                        string res = ResultStateType.Match(state);
                                        if (sportInfos == 0) sgtzScoreData.score1 = res;
                                        else if (sportInfos == 1) sgtzScoreData.score2 = res;
                                        else if (sportInfos == 2) sgtzScoreData.score3 = res;
                                    }
                                }
                                sgtzScoreData.roundCount = currentRoundCount;
                                studentDataModel.SgtzScoreData = sgtzScoreData;
                                studentDataModel.isTest = true;

                            }
                            else
                            {
                                studentDataModel.SgtzScoreData = new SGTZScoreData()
                                {
                                    score1 = "未测试", score2 = "未测试 ", score3 = "未测试", roundCount = currentRoundCount
                                };
                                studentDataModel.isTest = false;
                            }
                        }
                        else //FHL
                        {
                            FHLScoreData scoreData = new FHLScoreData();
                            StudentScoreEntity  studentScoreEntities =
                                freeSqlServer.Find<StudentScoreEntity>(  x => x.PersonIdNumber == student.IdNumber && x.ProjectID == projectId &&
                                    x.PersonName == student.Name && x.RoundId == currentRoundCount&&x.SportItemType==4);
                            {
                                if (studentScoreEntities != null)
                                { 
                                    int state = studentScoreEntities.State;
                                    double score = studentScoreEntities.Score;
                                    if (state == 1)
                                    {
                                        scoreData.score = score.ToString();
                                        scoreData.roundCount = currentRoundCount;
                                    }
                                    else
                                    {
                                        string res = ResultStateType.Match(state);
                                        scoreData.score = res;
                                        scoreData.roundCount = currentRoundCount;
                                        
                                    }

                                    studentDataModel.FhlScoreData = scoreData;
                                    studentDataModel.isTest = true;
                                }
                                else
                                {
                                    studentDataModel.FhlScoreData = new FHLScoreData()
                                    {
                                        score = "未测试",
                                        roundCount = currentRoundCount,
                                    };
                                    studentDataModel.isTest = false;
                                }
                            }
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="projectType"></param>
        /// <param name="currentStudentDataModels"></param>
        /// <returns></returns>
        public bool WriteStudentDataToDataBase(int projectID,string group,int round,int projectType, List<StudentDataModel> currentStudentDataModels)
        {
            if (projectType == 0)// FHL
            {
                if (currentStudentDataModels.Count > 0)
                {
                    var students = currentStudentDataModels.FindAll(a => !string.IsNullOrEmpty(a.studentName) && !string.IsNullOrEmpty(a.idNumber) && string.IsNullOrEmpty(a.FhlScoreData.score) && a.equipmentCode == null);
                    if (students.Count > 0)
                    {
                        UIMessageBox.ShowWarning("当前还有部分考生未完成测试");
                        return false;
                    }
                }
                return WriteFHLStudentDataToDataBase(projectID,group,round,currentStudentDataModels);
            }
            else 
            {
                if (currentStudentDataModels.Count > 0)
                {
                    var students = currentStudentDataModels.FindAll(a => !string.IsNullOrEmpty(a.studentName) && !string.IsNullOrEmpty(a.idNumber) && string.IsNullOrEmpty(a.FhlScoreData.score) && a.equipmentCode == null);
                    if (students.Count > 0)
                    {
                        UIMessageBox.ShowWarning("当前还有部分考生未完成测试");
                        return false;
                    }
                }
                return WriteSGTZStudentDataToDataBase(projectID,group,round,currentStudentDataModels );
            }      
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="currentStudentDataModels"></param>
        /// <returns></returns>
        private bool WriteSGTZStudentDataToDataBase(int projectID,string groupName,int round,List<StudentDataModel> currentStudentDataModels)
        {
            try
            {
                if (currentStudentDataModels.Count > 0)
                {
                    List<StudentScoreEntity> studentScores = new List<StudentScoreEntity>();
                    foreach (var studentModel in currentStudentDataModels)
                    {
                        string studentName = studentModel.studentName;
                        string idNumber = studentModel.idNumber;
                        if (string.IsNullOrEmpty(studentName) || string.IsNullOrEmpty(idNumber)) continue;
                        StudentDataEntity studentDataEntity = freeSqlServer.Find<StudentDataEntity>(X => X.IdNumber == idNumber && X.Name.Equals(studentName) && X.ProjectId == projectID.ToString() && X.GroupName.Equals(groupName));
                        if (studentDataEntity != null)
                        {
                            int studentID = studentDataEntity.Id;

                            List<StudentScoreEntity> studentScoreEntities = freeSqlServer.FindList<StudentScoreEntity>(x => x.Id, x => x.ProjectID.Equals(projectID) && x.PersonName.Equals(groupName) && x.PersonIdNumber.Equals(idNumber) && x.PersonId.Equals(studentID) && x.RoundId.Equals(round));
                            if (studentScoreEntities.Count > 0) continue;
                            else
                            {
                                freeSqlServer.FindAggregate<StudentScoreEntity>(x => x.Max(x.Key.SortId), out object sord);
                                int index = (int)sord;
                                StudentScoreEntity studentScoreEntity = new StudentScoreEntity()
                                {
                                    Score = double.Parse(studentModel.SgtzScoreData.score1),
                                    SortId = index + 1,
                                    SportItemType = 0,
                                    State = 1,
                                    uploadState = 0,
                                    CreateTime = DateTime.Now.ToString("yyyy_mm_dd"),
                                    IsRemoved = 0,
                                    PersonId = studentID,
                                    PersonIdNumber = idNumber,
                                    PersonName = studentName,
                                    ProjectID = projectID,
                                    RoundId = round,
                                };
                                studentScores.Add(studentScoreEntity);
                                StudentScoreEntity studentScoreEntity1 = new StudentScoreEntity()
                                {
                                    Score = double.Parse(studentModel.SgtzScoreData.score2),
                                    SortId = index + 1,
                                    SportItemType = 1,
                                    State = 1,
                                    uploadState = 0,
                                    CreateTime = DateTime.Now.ToString("yyyy_mm_dd"),
                                    IsRemoved = 0,
                                    PersonId = studentID,
                                    PersonIdNumber = idNumber,
                                    PersonName = studentName,
                                    ProjectID = projectID,
                                    RoundId = round,
                                };
                                studentScores.Add(studentScoreEntity1);
                                StudentScoreEntity studentScoreEntity2 = new StudentScoreEntity()
                                {
                                    Score = double.Parse(studentModel.SgtzScoreData.score3),
                                    SortId = index + 1,
                                    SportItemType = 2,
                                    State = 1,
                                    uploadState = 0,
                                    CreateTime = DateTime.Now.ToString("yyyy_mm_dd"),
                                    IsRemoved = 0,
                                    PersonId = studentID,
                                    PersonIdNumber = idNumber,
                                    PersonName = studentName,
                                    ProjectID = projectID,
                                    RoundId = round,
                                };
                                studentScores.Add(studentScoreEntity2);
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    return freeSqlServer.InsertOrUpdate<StudentScoreEntity>(studentScores) > 0 ? true : false;
                }
                else { return false; }
            }
            catch (Exception ex) { return false; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentStudentDataModels"></param>
        /// <returns></returns>
        private bool WriteFHLStudentDataToDataBase(int projectID,string groupName, int round, List<StudentDataModel> currentStudentDataModels)
        {
            try
            {
                if (currentStudentDataModels.Count > 0)
                {
                    List<StudentScoreEntity> studentScores = new List<StudentScoreEntity>();
                    foreach (var studentModel in currentStudentDataModels)
                    {
                        string studentName = studentModel.studentName;
                        string idNumber = studentModel.idNumber;
                        if (string.IsNullOrEmpty(studentName) || string.IsNullOrEmpty(idNumber)) continue;
                        StudentDataEntity studentDataEntity = freeSqlServer.Find<StudentDataEntity>(X => X.IdNumber == idNumber && X.Name.Equals(studentName) && X.ProjectId == projectID.ToString() && X.GroupName.Equals(groupName));
                        if (studentDataEntity != null)
                        {
                            int studentID = studentDataEntity.Id;

                            List<StudentScoreEntity> studentScoreEntities = freeSqlServer.FindList<StudentScoreEntity>(x => x.Id, x => x.ProjectID.Equals(projectID) && x.PersonName.Equals(groupName) && x.PersonIdNumber.Equals(idNumber) && x.PersonId.Equals(studentID) && x.RoundId.Equals(round));
                            if (studentScoreEntities.Count > 0) continue;
                            else
                            {
                                freeSqlServer.FindAggregate<StudentScoreEntity>(x => x.Max(x.Key.SortId), out object sord);
                                int index = (int)sord;
                                StudentScoreEntity studentScoreEntity = new StudentScoreEntity()
                                {
                                    Score = double.Parse(studentModel.FhlScoreData.score),
                                    SortId = index + 1,
                                    SportItemType = 4,
                                    State = 1,
                                    uploadState = 0,
                                    CreateTime = DateTime.Now.ToString("yyyy_mm_dd"),
                                    IsRemoved = 0,
                                    PersonId = studentID,
                                    PersonIdNumber = idNumber,
                                    PersonName = studentName,
                                    ProjectID = projectID,
                                    RoundId = round,
                                };
                                studentScores.Add(studentScoreEntity);
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    return freeSqlServer.InsertOrUpdate<StudentScoreEntity>(studentScores) > 0 ? true : false;
                }
                else { return false; }
            }
            catch (Exception ex) { return false; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectType"></param>
        /// <param name="projectId"></param>
        public void ExportCurentProjectStudentScore(int projectType, int projectId,int roundCount)
        {
            List<Students> studentDataModels = LoadingAllStudentData(projectType, projectId,roundCount);
            if (studentDataModels.Count != 0)
            {
                string psth = Path.Combine(Application.StartupPath, $"导出成绩/{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
                ExportStudentScore(projectType,roundCount,psth,studentDataModels);
                System.Diagnostics.Process.Start(psth);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectType"></param>
        /// <param name="roundCount"></param>
        /// <param name="psth"></param>
        /// <param name="studentDataModels"></param>
        private void ExportStudentScore(int projectType, int roundCount, string psth, List<Students> studentDataModels)
        {
            IWorkbook workbook = new XSSFWorkbook();
            
            ISheet sheet = workbook.CreateSheet("Students");
            IRow headerRow = CreateFristRowTitle(projectType, roundCount,sheet);
            if (studentDataModels.Count > 0)
            {
                for (int i = 1; i <= studentDataModels.Count; i++)
                {
                    IRow dataRow = sheet.CreateRow(i);
                    var student = studentDataModels[i - 1];
                    dataRow.CreateCell(0).SetCellValue(i);
                    dataRow.CreateCell(1).SetCellValue(projectType==0?"肺活量":"身高体重");
                    dataRow.CreateCell(2).SetCellValue(student.schoolName);
                    dataRow.CreateCell(3).SetCellValue(student.gradeName);
                    dataRow.CreateCell(4).SetCellValue(student.className);
                    dataRow.CreateCell(5).SetCellValue(student.groupName);
                    dataRow.CreateCell(6).SetCellValue(student.studentName);
                    dataRow.CreateCell(7).SetCellValue(student.sex);
                    dataRow.CreateCell(8).SetCellValue(student.idNumber);
                    if (projectType == 0) // fh
                    {
                        int index = 9;
                        for (int j = 0; j < student.FhlScoreData.Count; j++)
                        {
                            dataRow.CreateCell(index).SetCellValue(student.FhlScoreData[j].roundCount);     
                            dataRow.CreateCell(index+1).SetCellValue(student.FhlScoreData[j].score);
                            index=index+2;

                        }
                    }
                    else
                    {
                        int index = 9;
                        for (int j = 0; j < student.SgtzScoreData .Count; j++)
                        {
                            dataRow.CreateCell(index).SetCellValue(student.SgtzScoreData[j].roundCount);
                          
                            dataRow.CreateCell(index+1).SetCellValue(student.SgtzScoreData[j].score1);
                            dataRow.CreateCell(index+2).SetCellValue(student.SgtzScoreData[j].score2);
                            dataRow.CreateCell(index+3).SetCellValue(student.SgtzScoreData[j].score3);
                            index+=4;

                        }
                    }
                }
            }
            if (File.Exists(psth))
            {
                File.Delete(psth);
            }
            else
            {
                using (FileStream file = new FileStream(psth, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(file);
                    workbook.Close();
                }
            }
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectType"></param>
        /// <param name="roundCount"></param>
        /// <param name="sheet"></param>
        /// <returns></returns>
        private IRow CreateFristRowTitle(int projectType,  int roundCount, ISheet sheet)
        {
            IRow headerRow = sheet.CreateRow(0);
            headerRow.CreateCell(0).SetCellValue("序号");
            headerRow.CreateCell(1).SetCellValue("项目");
            headerRow.CreateCell(2).SetCellValue("学校");
            headerRow.CreateCell(3).SetCellValue("年级");
            headerRow.CreateCell(4).SetCellValue("班级");
            headerRow.CreateCell(5).SetCellValue("组别");
            headerRow.CreateCell(6).SetCellValue("姓名");
            headerRow.CreateCell(7).SetCellValue("性别");
            headerRow.CreateCell(8).SetCellValue("准考证号");
            if (projectType == 0)
            {
                int index = 9;
                for (int i = 0; i <roundCount; i++)
                {
                    headerRow.CreateCell(index).SetCellValue($"第{i+1}轮");
                   
                    headerRow.CreateCell(index+1).SetCellValue("成绩");
                    index+=2;
                }
            }
            else
            {
                int index = 9;
                for (int i = 0; i < roundCount; i++)
                {
                    headerRow.CreateCell(index).SetCellValue($"第{i+1}轮");
                     
                    headerRow.CreateCell(index+1).SetCellValue("身高(cm)");
                    
                    headerRow.CreateCell(index+2).SetCellValue("体重(kg)");
                   
                    headerRow.CreateCell(index+3).SetCellValue("BMI");
                    index+=4;
                }
                
            }
            return headerRow;
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectType"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        private List<Students> LoadingAllStudentData(int projectType, int projectId,int roundCount)
        {
            List<Students> students = new List<Students>();
             try
            {
                List<StudentDataEntity> studentDataEntities = freeSqlServer.FindList<StudentDataEntity>(x=>x.Id,x => x.ProjectId.Equals(projectId)  );
                if (studentDataEntities.Count > 0)
                {
                    foreach (var student in studentDataEntities)
                    {
                        Students studentDataModel = new Students();
                        studentDataModel. studentName = student.Name;
                        studentDataModel.idNumber = student.IdNumber;
                        studentDataModel.schoolName = student.SchoolName;
                        studentDataModel.sex = student.Sex == 0 ? "男" : "女";
                        studentDataModel.personId = student.Id.ToString();
                        studentDataModel.className = student.ClassNumber;
                        studentDataModel.gradeName = student.GradeName;
                        studentDataModel.groupName = student.GroupName;
                        if (projectType==0) //fhl
                        {
                            List<FHLScoreData> scoreData = new List<FHLScoreData>();
                           List< StudentScoreEntity > studentScoreEntities =
                                freeSqlServer.FindList<StudentScoreEntity>(x=>x.Id,  x => x.PersonIdNumber == student.IdNumber && x.ProjectID == projectId &&
                                                                              x.PersonName == student.Name  &&x.SportItemType==4);
                            if (studentScoreEntities.Count>0)
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
                                for (int i = 0; i <roundCount; i++)
                                {
                                    scoreData.Add(new FHLScoreData()
                                    {
                                        roundCount = i+1,
                                        score ="未测试",
                                    });
                                }
                            }
                            studentDataModel.FhlScoreData = scoreData;
                        }
                        else
                        {
                            List<SGTZScoreData> scoreData = new List<SGTZScoreData>();
                            List< StudentScoreEntity > studentScoreEntities =
                                freeSqlServer.FindList<StudentScoreEntity>(x=>x.Id,  x => x.PersonIdNumber == student.IdNumber && x.ProjectID == projectId &&
                                    x.PersonName == student.Name  );
                            if (studentScoreEntities.Count > 0)
                            {
                                var data=  studentScoreEntities.Find(a => a.SportItemType == 4);
                                if (data != null) studentScoreEntities.Remove(data);
                                if (studentScoreEntities.Count > 0)
                                {
                                    var groups= studentScoreEntities.GroupBy(a => a.RoundId) ;
                                    List<int> roundss = new List<int>();
                                    foreach (var item in groups)
                                    { 
                                        roundss.Add(item.Key);
                                    }
                                    foreach (var key in roundss)
                                    {
                                         SGTZScoreData SGTZScoreData = new SGTZScoreData();
                                         SGTZScoreData.roundCount = key;
                                         var stu= studentScoreEntities.FindAll(a => a.RoundId == key);
                                         if (stu.Count > 0)
                                         {
                                             foreach (var st in stu)
                                             {
                                                 int state = st.State;
                                                 double score = st.Score;
                                                 if (state==1)
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
                                                          SGTZScoreData.score2 =  pp;
                                                      else
                                                      {
                                                          SGTZScoreData.score3 =  pp;
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
                                        roundCount = i+1,
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectType"></param>
        /// <param name="projectID"></param>
        /// <param name="roundCount"></param>
        /// <param name="groupName"></param>
        public void PriteCurrentChooseGroupData(int projectType, int projectID, int roundCount, string groupName)
        {
            List<Students> students = LoadingAllStudentData(projectType, projectID, roundCount);
            if (students.Count > 0)
            {
                List<Students >groups=students.FindAll(a=>a.groupName == groupName);
                string psth = Path.Combine(Application.StartupPath, $"导出成绩/{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
                ExportStudentScore(projectType, roundCount, psth, groups);
                PrintSetting(psth);

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        private void PrintSetting(string path)
        {
            Workbook workbook = new Workbook();
            // workbook.LoadFromFile(path);
            workbook.LoadFromFile(path);
            Worksheet sheet = workbook.Worksheets[0];
            sheet.PageSetup.Orientation = PageOrientationType.Portrait;
            sheet.PageSetup.PaperSize = PaperSizeType.PaperA4;
            sheet.PageSetup.HeaderMarginInch = 2;
            sheet.PageSetup.TopMargin = 0.5;
            sheet.PageSetup.BottomMargin = 0;
            sheet.PageSetup.LeftMargin = 0.5;
            ///页边距_右
            sheet.PageSetup.RightMargin = 0.5;
            PrintDialog dialog = new PrintDialog();
            dialog.AllowPrintToFile = true;
            dialog.AllowCurrentPage = true;
            dialog.AllowSomePages = true;
            dialog.AllowSelection = true;
            dialog.UseEXDialog = true;
            dialog.PrinterSettings.Duplex = Duplex.Simplex;
            dialog.PrinterSettings.FromPage = 0;
            dialog.PrinterSettings.ToPage = 8;
            dialog.PrinterSettings.PrintRange = PrintRange.SomePages;
            dialog.PrinterSettings.PrinterName = GetLocalDefaultPrinter();
            workbook.PrintDialog = dialog;
            PrintDocument pd = workbook.PrintDocument;
            if (dialog.ShowDialog() == DialogResult.OK) pd.Print();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetLocalDefaultPrinter()
        {
            PrintDocument document = new PrintDocument();
            return document.PrinterSettings.PrinterName;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="idnumber"></param>
        /// <param name="name"></param>
        /// <param name="v"></param>
        /// <param name="currentRoundCount"></param>
        /// <param name="groupName"></param>
        public void SetStudentErrorData(int projectID, int projectType, string idnumber, string name, string v, int currentRoundCount, string groupName)
        {
            int state = ResultStateType.ResultStateToInt(v);
            var studentEnity=freeSqlServer.Find<StudentDataEntity>(x=>x.IdNumber == idnumber&&x.Name == name&&x.ProjectId==projectID.ToString());
            if(studentEnity != null)
            {
                var studentScore=freeSqlServer.Find<StudentScoreEntity>(x=>x.PersonName ==name&&x.ProjectID==projectID&&x.PersonIdNumber==idnumber&&x.RoundId==currentRoundCount);
                if(studentScore != null)
                {
                    UIMessageBox.ShowWarning("当前考生成绩已经存在!!");return;
                }
                else
                {

                    freeSqlServer.FindAggregate<StudentScoreEntity>(x => x.Max(x.Key.SortId), out object keys);
                    int key = (int)keys;
                    List<StudentScoreEntity> list = new List<StudentScoreEntity>();
                    if(projectType==0)//fhl
                    {
                        StudentScoreEntity student = new StudentScoreEntity()
                        {
                            SortId = key + 1,
                            Score = 0,
                            State = state,
                            SportItemType = 4,
                            uploadState = 0,
                            CreateTime = DateTime.Now.ToString("yyyy_mm_dd"),
                            PersonName = name,
                            PersonIdNumber = idnumber,
                            IsRemoved = 0,
                            PersonId = projectID,
                            RoundId = currentRoundCount,
                            ProjectID = projectID,
                        };
                        list.Add(student);
                    }
                    else
                    {
                        StudentScoreEntity student = new StudentScoreEntity()
                        {
                            SortId = key + 1,
                            Score = 0,
                            State = state,
                            SportItemType = 0,
                            uploadState = 0,
                            CreateTime = DateTime.Now.ToString("yyyy_mm_dd"),
                            PersonName = name,
                            PersonIdNumber = idnumber,
                            IsRemoved = 0,
                            PersonId = projectID,
                            RoundId = currentRoundCount,
                            ProjectID = projectID,
                        };
                        list.Add(student);
                        StudentScoreEntity student1 = new StudentScoreEntity()
                        {
                            SortId = key + 1,
                            Score = 0,
                            State = state,
                            SportItemType = 1,
                            uploadState = 0,
                            CreateTime = DateTime.Now.ToString("yyyy_mm_dd"),
                            PersonName = name,
                            PersonIdNumber = idnumber,
                            IsRemoved = 0,
                            PersonId = projectID,
                            RoundId = currentRoundCount,
                            ProjectID = projectID,
                        };
                        list.Add(student1);
                        StudentScoreEntity student2 = new StudentScoreEntity()
                        {
                            SortId = key + 1,
                            Score = 0,
                            State = state,
                            SportItemType = 2,
                            uploadState = 0,
                            CreateTime = DateTime.Now.ToString("yyyy_mm_dd"),
                            PersonName = name,
                            PersonIdNumber = idnumber,
                            IsRemoved = 0,
                            PersonId = projectID,
                            RoundId = currentRoundCount,
                            ProjectID = projectID,
                        };
                        list.Add(student);
                    }

                    freeSqlServer.InsertOrUpdate<StudentScoreEntity>(list);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uiComboBox"></param>
        public void LoadingChipInfos(UIComboBox uiComboBox)
        {
            uiComboBox.Items.Clear();
            var data = freeSqlServer.FindList<ChipInfosEntity>(x => x.Id);
            if(data.Count>0)
            {
                foreach( var item in data)
                {
                    if(!uiComboBox.Items.Contains(item.GroupName))uiComboBox.Items.Add(item.GroupName);
                    else continue;
                }
            }
            if(uiComboBox.Items.Count>0) { uiComboBox.SelectedIndex = 0; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sl"></param>
        /// <param name="currentStudentDataModels"></param>
        /// <param name="projectType"></param>
        /// <returns></returns>
        public List<StudentDataModel> LoadingStudentChipData(string sl,  int projectType,int roundCount)
        {
            List<StudentDataModel> selectStudentDataModels = new List<StudentDataModel>();
            List<ChipInfosEntity> chipInfosEntities =
                freeSqlServer.FindList<ChipInfosEntity>(a => a.Id, x => x.GroupName == sl&&x.projectType==projectType);
            if (chipInfosEntities.Count > 0)
            {
                foreach (var chip in chipInfosEntities)
                {
                     int projectID = chip.ProjectID;
                     var idnum = chip.ChipLabel.ToString();
                     var data = SelectStudentDataByIDNumber(projectID, idnum, roundCount,projectType);
                     if (!selectStudentDataModels.Contains(data))
                         selectStudentDataModels.Add(data);
                }
            }

            return selectStudentDataModels;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="idnum"></param>
        /// <param name="roundCount"></param>
        /// <param name="projectType"></param>
        /// <returns></returns>
        public StudentDataModel SelectStudentDataByIDNumber(int projectId, string idnum, int roundCount, int projectType)
        {
            StudentDataModel SelectStudentDataModel = new StudentDataModel();
            StudentDataEntity studentDataEntity =
                freeSqlServer.Find<StudentDataEntity>(a => a.ProjectId == projectId.ToString() && a.IdNumber == idnum);
            if (studentDataEntity != null)
            {
                SelectStudentDataModel.studentName = studentDataEntity.Name;
                SelectStudentDataModel.idNumber = studentDataEntity.IdNumber;
                SelectStudentDataModel.sex = studentDataEntity.Sex == 0 ? "男" : "女";
                SelectStudentDataModel.className = studentDataEntity.ClassNumber;
                SelectStudentDataModel.schoolName = studentDataEntity.SchoolName;
                SelectStudentDataModel.personId = studentDataEntity.Id.ToString();
                SelectStudentDataModel.gradeName = studentDataEntity.GradeName;
                SelectStudentDataModel.groupName  =studentDataEntity.GroupName;
                if (projectType == 0) // fhl
                {
                    FHLScoreData scoreData = new FHLScoreData();
                    StudentScoreEntity  studentScoreEntities = freeSqlServer.Find<StudentScoreEntity>(  x => x.PersonIdNumber ==  studentDataEntity.IdNumber && x.ProjectID == projectId &&
                                                                      x.PersonName ==  studentDataEntity.Name && x.RoundId == roundCount&&x.SportItemType==4);
                    if (studentScoreEntities != null)
                    { 
                        int state = studentScoreEntities.State;
                        double score = studentScoreEntities.Score;
                        if (state == 1)
                        {
                            scoreData.score = score.ToString();
                            scoreData.roundCount = roundCount;
                        }
                        else
                        {
                            string res = ResultStateType.Match(state);
                            scoreData.score = res;
                            scoreData.roundCount = roundCount;
                                        
                        }
                        SelectStudentDataModel.FhlScoreData = scoreData;
                        SelectStudentDataModel.isTest = true;
                    }
                    else
                    {
                        SelectStudentDataModel.FhlScoreData = new FHLScoreData()
                        {
                            score = "未测试",
                            roundCount = roundCount
                        };
                        SelectStudentDataModel.isTest = false;
                    }
                }
                else if (projectType==1)//SGTZ
                {
                    SGTZScoreData sgtzScoreData = new SGTZScoreData();
                    List<StudentScoreEntity> studentScoreEntities =
                        freeSqlServer.FindList<StudentScoreEntity>(x => x.Id, x => x.PersonIdNumber == studentDataEntity.IdNumber && x.ProjectID == projectId &&
                            x.PersonName == studentDataEntity.Name && x.RoundId == roundCount);
                    if (studentScoreEntities.Count > 0&&studentScoreEntities.Count==3)
                    {
                        var pp = studentScoreEntities.OrderBy(x =>x.SportItemType ).ToList();
                        foreach (var scoreEntity in pp)
                        {
                            int sportInfos = scoreEntity.SportItemType;
                            int state = scoreEntity.State;
                            double score = scoreEntity.Score;
                            if (state == 1)
                            {
                                if (sportInfos == 0) sgtzScoreData.score1 = score.ToString();
                                else if (sportInfos == 1) sgtzScoreData.score2 = score.ToString();
                                else if (sportInfos == 2) sgtzScoreData.score3 = score.ToString();
                            }
                            else
                            {
                                string res = ResultStateType.Match(state);
                                if (sportInfos == 0) sgtzScoreData.score1 = res;
                                else if (sportInfos == 1) sgtzScoreData.score2 = res;
                                else if (sportInfos == 2) sgtzScoreData.score3 = res;
                            }
                        }
                        sgtzScoreData.roundCount =roundCount;
                        SelectStudentDataModel.SgtzScoreData = sgtzScoreData;
                        SelectStudentDataModel.isTest = true;

                    }
                    else
                    {
                        SelectStudentDataModel.SgtzScoreData = new SGTZScoreData()
                        {
                            score1 = "未测试", score2 = "未测试 ", score3 = "未测试", roundCount = roundCount
                        };
                        SelectStudentDataModel.isTest = false;
                    }
                }

                return SelectStudentDataModel;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tempStudentModels"></param>
        /// <param name="projectId"></param>
        /// <param name="name"></param>
        /// <param name="projectType"></param>
        public void SaveChipInfosData(List<StudentDataModel> tempStudentModels, int projectId, string name ,int projectType)
        {
            freeSqlServer.FindAggregate<ChipInfosEntity>(x => x.Max(x.Key.ChipSort), out object maxID);
            List<ChipInfosEntity> list = new List<ChipInfosEntity>();
            var ck = freeSqlServer.FindList<ChipInfosEntity>().Where(A => A.ProjectID.Equals(projectId) && A.GroupName.Equals(name)&&A.projectType==projectType).ToList();
            if (ck != null)
            {
                foreach (var ch in ck)
                {
                    freeSqlServer.Delete<ChipInfosEntity>(ch);
                }
            }
            foreach (var chip in tempStudentModels)
            {
                ChipInfosEntity  chips = new ChipInfosEntity()
                {
                    ChipSort =(int) maxID+1,
                    ChipLabel = chip.idNumber,
                    GroupName = name,
                    ProjectID = projectId,
                    projectType = projectType
                };
                list.Add(chips);
            }
            if (list.Count > 0)
            {
                freeSqlServer.InsertOrUpdate<ChipInfosEntity>(list);
            }
        }


       
}