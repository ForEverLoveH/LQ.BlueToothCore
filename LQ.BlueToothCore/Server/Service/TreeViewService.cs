using LQ.BlueToothCore.IService.IService.IFreeSqlService;
using LQ.BlueToothCore.MLL.Models;
using LQ.BlueToothCore.MLL.Models.DBModels;
using LQ.BlueToothCore.Service.Service.FreeSqlService;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LQ.BlueToothCore.Server.Service
{
    public class TreeViewService
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IFreeSqlServer freeSqlServer= new  FreeSqlServer();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<MainTreeViewModel> LoadingProjectDataModel()
        {
            try
            {
                List<MainTreeViewModel> mainTreeViewModels = new List<MainTreeViewModel>();
                List<ProjectInfosEntity> projectInfosEntities = freeSqlServer.FindListAsync<ProjectInfosEntity>(x => x.ID).Result;
                if (projectInfosEntities.Count > 0)
                {
                    foreach (var projectInfoEntity in projectInfosEntities)
                    {
                        string projectName = projectInfoEntity.projectName;
                        int ID = projectInfoEntity.ID;

                        MainTreeViewModel mainTreeViewModel = mainTreeViewModels.Find(a => a.projectName == projectName);
                        if (mainTreeViewModel == null)
                        {
                            mainTreeViewModel = new MainTreeViewModel()
                            {
                                projectName = projectInfoEntity.projectName,
                                examTimeModel = new List<ExamTimeModel>()
                            };
                            mainTreeViewModels.Add(mainTreeViewModel);
                        }
                        mainTreeViewModel = mainTreeViewModels.Find(a => a.projectName == projectName);
                        List<GroupInfosEntity> groupInfosEntities = freeSqlServer.FindList<GroupInfosEntity>(x => x.ID, x => x.ProjectId == ID);
                        if (groupInfosEntities.Count > 0)
                        {
                            foreach (var groupInfoEntity in groupInfosEntities)
                            {
                                string examTime = groupInfoEntity.CreateTime.ToString("yyyy_mm_dd HH:ss");
                                string groupName = groupInfoEntity.Name;
                                int isAllTest = groupInfoEntity.IsAllTested;
                                ExamTimeModel ExamTimeModel = mainTreeViewModel.examTimeModel.Find(a => a.createTime == examTime);
                                if (ExamTimeModel == null)
                                {
                                    mainTreeViewModel.examTimeModel.Add(new ExamTimeModel()
                                    {
                                        createTime = examTime,
                                        groupsDatas = new List<GroupsData>(),
                                    });
                                }
                                ExamTimeModel = mainTreeViewModel.examTimeModel.Find(a => a.createTime == examTime);
                                var groups = ExamTimeModel.groupsDatas.Find(a => a.groupName == groupName);
                                if (groups == null)
                                {
                                    ExamTimeModel.groupsDatas.Add(new GroupsData()
                                    {
                                        groupName = groupName,
                                        isAllTest = isAllTest
                                    });
                                }
                                else
                                {
                                    GroupsData groupsData = new GroupsData()
                                    {
                                        groupName = groupName,
                                        isAllTest = isAllTest
                                    };
                                    if (!ExamTimeModel.groupsDatas.Contains(groupsData)) ExamTimeModel.groupsDatas.Add(groupsData);

                                }
                            }
                        }
                    }
                    return mainTreeViewModels;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
