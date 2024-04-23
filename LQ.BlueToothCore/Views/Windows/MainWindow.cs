using LQ.BlueToothCore.MLL.Models;
using LQ.BlueToothCore.MLL.Models.DBModels;
using LQ.BlueToothCore.Server;
using LQ.BlueToothCore.Views.Windows;
using LQ.BlueToothCore.Views.WindowSys;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LQ.BlueToothCore.Views
{
    public partial class MainWindow : UIForm
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private readonly DataGridService _DataGridService= new DataGridService();
        private List<Students> _StudentsModel= new List<Students>();
        private List<ProjectInfosEntity>projectInfosEntities=new List<ProjectInfosEntity>();

        private string projectName;
        /// <summary>
        /// 
        /// </summary>
        private int roundCount;
        /// <summary>
        /// 
        /// </summary>
        private string createTime;
        /// <summary>
        /// 
        /// </summary>
        private int projectId;
        /// <summary>
        /// 
        /// </summary>
        private string groupName;

        /// <summary>
        /// 
        /// </summary>
        private int projectType = 0;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Load(object sender, EventArgs e)
        {
            string code = "程序集版本：" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string code1 = "文件版本：" + Application.ProductVersion.ToString();
            toolStripStatusLabel1.Text = code;
            InitData();
        }
        /// <summary>
        /// 
        /// </summary>
        private List<MainTreeViewModel> mainTreeViewModels = new List<MainTreeViewModel>();
        /// <summary>
        /// 
        /// </summary>
        private readonly MainWindowSys mainWindowSys = new MainWindowSys();
        /// <summary>
        /// 
        /// </summary>
        private void InitData()
        {
            projectInfosEntities = mainWindowSys.LoadingProjectData();
            var trees = mainWindowSys.LoadingMainTreeViewModel();
            if(trees != null )mainTreeViewModels.AddRange(trees);
            SetProjectTreeView();
        }

        private void SetProjectTreeView()
        {
            if(mainTreeViewModels.Count>0)
            {
                uiTreeView1.Nodes.Clear();
                for(int i = 0; i < mainTreeViewModels.Count; i++)
                {
                    TreeNode node= new TreeNode(mainTreeViewModels[i].projectName);
                    var examTimeModel = mainTreeViewModels[i].examTimeModel;
                    for(int j = 0; j < examTimeModel.Count; j++)
                    {
                        TreeNode treeNode = new TreeNode(examTimeModel[j].createTime);
                        var groups = examTimeModel[j].groupsDatas;
                        for(int k=0; k < groups.Count; k++)
                        {
                            TreeNode node1= new TreeNode(groups[k].groupName);
                            if(groups[k].isAllTest==0) node1.ForeColor=Color.Red;
                            else
                            {
                                node1.ForeColor=Color.MediumSpringGreen;
                            }
                            treeNode.Nodes.Add(node1);
                            
                        }
                        node.Nodes.Add(treeNode);

                    }
                    uiTreeView1.Nodes.Add(node);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="studentDataGridView"></param>
        private void ShowStudentDataInDataGridView(UIDataGridView studentDataGridView, List<Students> students )
        {
            studentDataGridView.Rows.Clear();
            if (students.Count > 0)
            {
                int len = students.Count;
                DataGridViewRow[] rows = new DataGridViewRow[len];
                int index = 1;
                foreach (var student in students)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.Cells.Add(_DataGridService.SetNewDataGridViewCell(student.personId.ToString(), Color.Black, Color.White));
                    row.Cells.Add(
                        _DataGridService.SetNewDataGridViewCell(student.schoolName, Color.Black, Color.White));
                    row.Cells.Add(_DataGridService.SetNewDataGridViewCell(student.gradeName, Color.Black, Color.White));
                    row.Cells.Add(_DataGridService.SetNewDataGridViewCell(student.className, Color.Black, Color.White));
                    row.Cells.Add(_DataGridService.SetNewDataGridViewCell(student.groupName, Color.Black, Color.White));
                    row.Cells.Add(_DataGridService.SetNewDataGridViewCell(student.studentName, Color.Black, Color.White));
                    row.Cells.Add(_DataGridService.SetNewDataGridViewCell(student.sex, Color.Black, Color.White));
                    row.Cells.Add(_DataGridService.SetNewDataGridViewCell(student.idNumber, Color.Black, Color.White));
                    if (projectType == 1)//SGTZ
                    {
                        List<SGTZScoreData> sGTZScoreDatas = student.SgtzScoreData;
                        for(int i = 0; i < sGTZScoreDatas.Count;   i++)
                        {
                            row.Cells.Add(_DataGridService.SetNewDataGridViewCell($"第{sGTZScoreDatas[i].roundCount}轮" , Color.Black, Color.White));
                            row.Cells.Add(_DataGridService.SetNewDataGridViewCell(sGTZScoreDatas[i].score1, sGTZScoreDatas[i].score1 == "未测试" ? Color.Red : Color.Black, Color.White));
                            row.Cells.Add(_DataGridService.SetNewDataGridViewCell(sGTZScoreDatas[i].score1, sGTZScoreDatas[i].score2== "未测试" ? Color.Red : Color.Black, Color.White));
                            row.Cells.Add(_DataGridService.SetNewDataGridViewCell(sGTZScoreDatas[i].score1, sGTZScoreDatas[i].score3 == "未测试" ? Color.Red : Color.Black, Color.White));

                        }

                    }
                    else
                    {
                        List<FHLScoreData>fHLScoreDatas = student.FhlScoreData;
                        for (int i = 0; i < fHLScoreDatas.Count; i++)
                        {
                            row.Cells.Add(_DataGridService.SetNewDataGridViewCell($"第{fHLScoreDatas[i].roundCount}轮", Color.Black,Color.White));
                            row.Cells.Add(_DataGridService.SetNewDataGridViewCell(fHLScoreDatas[i].score, fHLScoreDatas[i].score=="未测试"?Color.Red: Color.Black,Color.White));
                            
                        }

                    }
                    rows[index - 1] = row;
                    index++;
                }

                studentDataGridView.Rows.AddRange(rows);

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataGridView"></param>
        private void SetDataGridViewColumText(UIDataGridView dataGridView)
        {
            if (dataGridView.Columns.Count > 0) dataGridView.Columns.Clear();
            dataGridView.Columns.Add(_DataGridService.SetDataGridViewTextBoxColumn("序号", 100));
            dataGridView.Columns.Add(_DataGridService.SetDataGridViewTextBoxColumn("学校名", 100));
            dataGridView.Columns.Add(_DataGridService.SetDataGridViewTextBoxColumn("年级", 100));
            dataGridView.Columns.Add(_DataGridService.SetDataGridViewTextBoxColumn("班级", 100));
            dataGridView.Columns.Add(_DataGridService.SetDataGridViewTextBoxColumn("组号", 100));
            dataGridView.Columns.Add(_DataGridService.SetDataGridViewTextBoxColumn("姓名", 100));
            dataGridView.Columns.Add(_DataGridService.SetDataGridViewTextBoxColumn("性别", 100));
            dataGridView.Columns.Add(_DataGridService.SetDataGridViewTextBoxColumn("考号", 100));
            if (projectType == 1)//SGTZ
            {
                for (int i = 0; i < roundCount; i++)
                {
                    dataGridView.Columns.Add(_DataGridService.SetDataGridViewTextBoxColumn("轮次", 100));
                    dataGridView.Columns.Add(_DataGridService.SetDataGridViewTextBoxColumn("身高成绩", 100));
                    dataGridView.Columns.Add(_DataGridService.SetDataGridViewTextBoxColumn("体重成绩", 100));
                    dataGridView.Columns.Add(_DataGridService.SetDataGridViewTextBoxColumn("BMI成绩", 100));
                }

            }
            else
            {
                for (int i = 0; i < roundCount; i++)
                {
                    dataGridView.Columns.Add(_DataGridService.SetDataGridViewTextBoxColumn("轮次", 100));
                    dataGridView.Columns.Add(_DataGridService.SetDataGridViewTextBoxColumn("成绩", 100));
                }
            }

        }
        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiTreeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            projectName = "";
            createTime ="";
            groupName = "";
            uiDataGridView1.Rows.Clear();
            _StudentsModel.Clear();
           var tree=uiTreeView1.SelectedNode; 
            if (tree != null)
            {
                var fullPath=tree.FullPath;
                string[] fulls = fullPath.Split('\\');
                if (fulls.Length == 1) projectName = fulls[0];
                else if(fulls.Length == 2)
                {
                    projectName = fulls[0];
                    createTime = fulls[1];
                }else if (fulls.Length == 3)
                {
                    projectName = fulls[0];
                    createTime = fulls[1];
                    groupName = fulls[2];
                }
                if (!string.IsNullOrEmpty(projectName))
                {
                    ProjectInfosEntity projectInfosEntity= projectInfosEntities.Find(a=>a.projectName == projectName);
                    if (projectInfosEntity != null)
                    {
                        projectId=projectInfosEntity.ID;
                        projectType = projectInfosEntity.Type;
                        roundCount = projectInfosEntity.RoundCount;
                        SetDataGridViewColumText(uiDataGridView1);
                    }
                }
                 List<Students> studentDataModels=mainWindowSys. LoadingCurrentStudentData( projectType,  projectId,  groupName,roundCount);
                if (studentDataModels!=null&&studentDataModels.Count > 0)
                {
                    _StudentsModel= studentDataModels;
                    ShowStudentDataInDataGridView(uiDataGridView1, studentDataModels);  
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiTreeView1_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                TreeNode node=uiTreeView1.GetNodeAt(e.X, e.Y);
                if (node != null)
                {
                    uiTreeView1.SelectedNode = node;
                }
            }
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {
            
                this.Hide();
                RunningWindow runningWindow = new RunningWindow();
                runningWindow.projectName = projectName;
                runningWindow.Show();
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 项目信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 平台设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
