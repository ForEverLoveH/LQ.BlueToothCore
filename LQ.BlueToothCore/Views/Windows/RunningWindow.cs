using LQ.BlueToothCore.MLL.Models.DBModels;
using LQ.BlueToothCore.MLL.RspMessage;
using LQ.BlueToothCore.MLL.RspMessage.SGTZ;
using LQ.BlueToothCore.Service.Service.LogService;
using LQ.BlueToothCore.Service.Service.ManagerService;
using LQ.BlueToothCore.Views.MyControlls;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web.UI;
using System.Windows.Forms;
using LQ.BlueToothCore.IService.IService.IFreeSqlService;
using LQ.BlueToothCore.MLL.Models;
using LQ.BlueToothCore.Server;
using LQ.BlueToothCore.Views.WindowSys;
using System.Diagnostics.Eventing.Reader;
using System.Speech.Synthesis;
using System.Threading;
using LQ.BlueToothCore.Service.Service.ScanerHookService;

namespace LQ.BlueToothCore.Views.Windows
{
    public partial class RunningWindow : UIForm
    {
        public RunningWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 0 表示 FHL 1 表示sgtz
        /// </summary>
        private int projectType = 0;
        /// <summary>
        /// 
        /// </summary>
        public string projectName = "肺活量测试";
        /// <summary>
        /// 
        /// </summary>

        private int projectID = 0;
        /// <summary>
        /// 
        /// </summary>
        private int type = 0;
        /// <summary>
        /// 
        /// </summary>
        private int roundCount;
        /// <summary>
        /// 
        /// </summary>
        private int currentRoundCount = 0;
        /// <summary>
        /// 
        /// </summary>
        private string groupName;
        /// <summary>
        /// 
        /// </summary>
        private List<SGTZControlls> _SGTZControlls = new List<SGTZControlls>();
        /// <summary>
        /// 
        /// </summary>
        private List<FHLControlls> _FHLControlls = new List<FHLControlls>();

        /// <summary>
        /// 
        /// </summary>
        private List<StudentDataModel> studentDataModels = new List<StudentDataModel>();
        /// <summary>
        /// 
        /// </summary>
        private List<StudentDataModel> currentStudentDataModels= new List<StudentDataModel>();
        /// <summary>
        /// 
        /// </summary>
        private List<StudentDataModel>tempStudentModels  = new List<StudentDataModel>();
        /// <summary>
        /// 用于存贮接收到开始测试的数据返回的设备地址
        /// 
        /// </summary>
        private List<byte[]> equipmentCodeList = new List<byte[]>();
        
        /// <summary>
        /// 
        /// </summary>
        private ServerManager _ServerManager;
        /// <summary>
        /// 是否已经开始测试
        /// </summary>
        private bool _isStart = false;
        /// <summary>
        /// 是否可以开始测试
        /// </summary>
        private bool _isCanStart = false;
        /// <summary>
        /// 是否可以写入
        /// </summary>
        private bool _isCanWrite=false;
        /// <summary>
        /// 
        /// </summary>
        private readonly LogServer _logServer = new LogServer();
        /// <summary>
        /// 设备数
        /// </summary>
        private int equipMentCount = 0;
        /// <summary>
        /// 
        /// </summary>
        private string ports = "";

        private string currentChoosePort;
        /// <summary>
        /// 波特
        /// </summary>
        private int nBaudrate = 115200;
        /// <summary>
        /// 当前项目对应的项目信息
        /// </summary>
        private  ProjectInfosEntity _projectInfosEntity;
        /// <summary>
        /// 
        /// </summary>
        private readonly RunningWindowSys runningWindowSys = new RunningWindowSys();
        /// <summary>
        /// 
        /// </summary>
        private readonly DataGridService _DataGridService = new DataGridService();   
        /// <summary>
        /// 
        /// </summary>
        private bool isTemp = false;
        /// <summary>
        /// 
        /// </summary>
        private ScanerHookServer scanerHook = new ScanerHookServer();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RunningWindow_Load(object sender, EventArgs e)
        {
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            string code = "程序集版本：" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string code1 = "文件版本：" + Application.ProductVersion.ToString();
            toolStripStatusLabel1.Text = code;
            RunningWindowInit();
        }
        /// <summary>
        /// 
        /// </summary>
        private void RunningWindowInit()
        {
            
            ShowEquipMentWindow();
            InitManagerService();
            CloseSerialPort();
            LoadingInitData();
            LoadingPortData();
            scanerHook.ScanerEvent += ScanerHook_ScanerEvent; ;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="codes"></param>
        private void ScanerHook_ScanerEvent(ScanerHookServer.ScanerCodes codes)
        {
            string code= codes.Result;
            if(!string.IsNullOrEmpty(code))
            {
                if (!string.IsNullOrEmpty(uiTextBox1.Text.Trim())) uiTextBox1.Text = "";
                uiTextBox1.Text= code;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadingPortData()
        {
            string[] ports = RefreshSerialPort();
            if (ports.Length > 0)
            {
                uiComboBox1.Items.Clear();
                foreach (var port in ports)
                {
                    uiComboBox1.Items.Add(PortNameToPort(port));
                }
                if (uiComboBox1.Items.Count > 0) uiComboBox1.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadingInitData()
        {
            _projectInfosEntity = runningWindowSys.LoadingProjectInfos(projectName);
            if (_projectInfosEntity != null)
            {
                projectID = _projectInfosEntity.ID;
                type = _projectInfosEntity.Type;
                roundCount = _projectInfosEntity.RoundCount;
                projectType = type;
                SetCurrentProjectGroupData();
                if (roundCount > 0)
                {
                    SetCurrentProjectRound();
                }
            }
            SetDataGridViewColumText(uiDataGridView1);
            SetDataGridViewColumText(uiDataGridView2);
        }
        /// <summary>
        /// 
        /// </summary>
        private void SetCurrentProjectRound()
        {
            if(uiComboBox4.Items.Count>0)uiComboBox4.Items.Clear();
            if(uiComboBox6.Items.Count>0)uiComboBox6.Items.Clear();
            for (int i = 0; i < roundCount; i++)
            {
                uiComboBox4.Items.Add($"第{i+1}轮");
                uiComboBox6.Items.Add($"第{i+1}轮");
            }
            if (isTemp)
            {
                if (uiComboBox6.Items.Count > 0) uiComboBox6.SelectedIndex = 0;
            }
            else
            {
                if (uiComboBox4.Items.Count > 0) uiComboBox4.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetCurrentProjectGroupData()
        {
            List<string> pp = runningWindowSys.LoadingProjectGroupData(projectID);
            if (pp != null && pp.Count > 0)
            {
                if(uiComboBox3.Items.Count>0)uiComboBox3.Items.Clear();
                foreach (var p in pp)
                {
                    uiComboBox3.Items.Add(p);
                }

                if (uiComboBox3.Items.Count > 0) uiComboBox3.SelectedIndex = 0;
            }
        }

        #region 自定义控件相关
        /// <summary>
        /// 
        /// </summary>
        private void SetIsNotMatchFHLControll()
        {
            if (_FHLControlls.Count > 0)
            {
                foreach (var con in _FHLControlls)
                {
                    con.p_Name = "";
                    con.p_IdNumber = "";
                    con.p_roundCbx_selectIndex = 0;
                    con.p_toolState = "未连接";
                    con.p_title_Color = Color.Red;
                    con.p_toolState_color = Color.Red;

                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void SetIsNotMatchSGTZControll()
        {
            if (_SGTZControlls.Count > 0)
            {
                foreach (var con in _SGTZControlls)
                {
                    con.p_Name = "";
                    con.p_IdNumber = "";
                    con.p_roundCbx_selectIndex = 0;
                    con.p_toolState = "未连接";
                    con.p_title_Color = Color.Red;
                    con.p_toolState_color = Color.Red;
                    con.p_Score = "";
                    con.p_Score1 = "";
                    con.p_Score2 = "";

                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        private void AddFHLControlls(int count)
        {
            _FHLControlls.Clear();
            for (int i = 0; i < count; i++)
            {
                FHLControlls con = new FHLControlls();
                con.p_Name = "";
                con.p_IdNumber = "";
                con.p_roundCbx_selectIndex = 0;
                con.p_toolState = "未连接";
                con.p_title_Color = Color.Red;
                con.p_toolState_color = Color.Red;
                con.p_title= $"第{i + 1}设备";
                con.p_title_Color=Color.Gray;
                if (roundCount != 0)
                {
                    List<string> pl = new List<string>();
                    for (int j = 0; j < roundCount; j++)
                    {
                        pl.Add($"第{i + 1}轮");
                    }

                    con.p_roundCbx_items = pl;
                }
                _FHLControlls.Add(con);
            }
            if (_FHLControlls.Count > 0)
            {
                if (flowLayoutPanel1.Controls.Count > 0) flowLayoutPanel1.Controls.Clear();
                foreach (var con in _FHLControlls)
                {
                    flowLayoutPanel1.Controls.Add(con);
                }
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        private void AddSGTZControlls(int count)
        {
            _SGTZControlls.Clear();
            for (int i = 0; i < count; i++)
            {
                SGTZControlls con = new SGTZControlls();
                con.p_Name = "";
                con.p_IdNumber = "";
                con.p_roundCbx_selectIndex = 0;
                con.p_toolState = "未连接";
                con.p_title_Color = Color.Red;
                con.p_toolState_color = Color.Red;
                con.p_Score = "";
                con.p_Score1 = "";
                con.p_Score2 = "";
                con.p_title = $"第{1 + 1}设备";
                con.p_title_Color = Color.Gray;
                if (roundCount != 0)
                {
                    List<string> pl = new List<string>();
                    for (int j = 0; j < roundCount; j++)
                    {
                        pl.Add($"第{i+1}轮");
                    }

                    con.p_roundCbx_items = pl;
                }
                _SGTZControlls.Add(con);
            }
            if (_SGTZControlls.Count > 0)
            {
                if (flowLayoutPanel1.Controls.Count > 0) flowLayoutPanel1.Controls.Clear();
                foreach (var con in _SGTZControlls)
                {
                    flowLayoutPanel1.Controls.Add(con);
                }
            }

        }
        /// <summary>
        /// 情况当前所有匹配
        /// </summary>
        public void ClearAllMatchControll()
        {
            if (projectType == 0) SetIsNotMatchFHLControll();
            if (projectType == 1) SetIsNotMatchSGTZControll();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="equipMentCount"></param>
        private void SetCurrentEquipMentCount(int equipMentCount)
        {
            if (projectType == 1) //SGTZ
            {
                AddSGTZControlls(equipMentCount);
            }
            if (projectType == 0)
            {
                AddFHLControlls(equipMentCount);
            }
        }

        #endregion
        #region 串口相关
        /// <summary>
        /// 
        /// </summary>
        private void InitManagerService()
        {

            _ServerManager = new ServerManager();
            _ServerManager._LogDataCallBack += ServerManager__LogDataCallBack;
            _ServerManager._SendDataCallBack += ServerManager__SendDataCallBack;
            _ServerManager._RecieveDataCallBack += ServerManager__RecieveDataCallBack;
            _ServerManager._HandelFHLRspStartSucessCallBack += ServerManager__HandelFHLRspStartSucessCallBack;
            _ServerManager._HandleFHLRspMatchSucessCallBack += ServerManager__HandleFHLRspMatchSucessCallBack;
            _ServerManager._HandleFHLRspGetFinnallyScoreCallBack += ServerManager__HandleFHLRspGetFinnallyScoreCallBack;
            _ServerManager._HandelSGTZRspStartSucessCallBack += ServerManager__HandelSGTZRspStartSucessCallBack;
            _ServerManager._HandleSGTZRspGetFinnallyScoreCallBack += ServerManager__HandleSGTZRspGetFinnallyScoreCallBack;
            _ServerManager._HandleSGTZRspMatchSucessCallBack += ServerManager__HandleSGTZRspMatchSucessCallBack;
            SendBlueToothName();

        }
        /// <summary>
        /// 收到身高体重匹配成功的委托实现
        /// </summary>
        /// <param name="message"></param>
        private void ServerManager__HandleSGTZRspMatchSucessCallBack(RspSGTZMatchSucessMessage message)
        {
            if (projectType != 1) return;
            if (_isStart == false)
            {
                byte[] cmdCode = message.cmdCode;
                byte[] stateCode = message.stateCode;
                string cmd = Encoding.ASCII.GetString(cmdCode);
                string st = Encoding.ASCII.GetString(stateCode);
                if (cmd == "match" && st == "ok")
                {
                    byte[] equipMentCode = message.equipMentCode;
                    if (_SGTZControlls.Count > 0)
                    {
                        var con = _SGTZControlls.Find(a => a.EquipMentCode == null);
                        if (con != null)
                        {
                            con.EquipMentCode = equipMentCode;
                            string equip = $"SGTZ设备:" + equipMentCode[0] + equipMentCode[1];
                            con.p_toolState = equip;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            } else return;
        }
        /// <summary>
        /// 收到升高体重最终成绩的委托实现
        /// </summary>
        /// <param name="message"></param>
        private void ServerManager__HandleSGTZRspGetFinnallyScoreCallBack(RspSGTZGetFinnallyScoreMessage message)
        {
            
        } 
        /// <summary>
        /// 收到身高体重开始测试成功的委托实现
        /// </summary>
        /// <param name="message"></param>
        private void ServerManager__HandelSGTZRspStartSucessCallBack(RspSGTZStartSucessMessage message)
        {
            if (projectType != 1) return;
            if (_isStart == false)
            {
                byte[] cmdCode = message.cmdCode;
                byte[] stateCode = message.stateCode;
                string cmd = Encoding.ASCII.GetString(cmdCode);
                string st = Encoding.ASCII.GetString(stateCode);
                if(cmd == "start" && cmd == "ok")
                {
                    byte[]equipMentCode= message.equipMentCode;
                    if (_SGTZControlls.Count > 0)
                    {
                        var con = _SGTZControlls.Find(a => a.EquipMentCode == equipMentCode);
                        if (con != null)
                        {
                            if (equipmentCodeList.Count > 0)
                            {
                                if (!equipmentCodeList.Contains(equipMentCode)) equipmentCodeList.Add(equipMentCode);
                                else { return; }
                            }
                            else
                            {
                                equipmentCodeList.Add(equipMentCode);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 收到肺活量获取成绩的回复的委托实现
        /// </summary>
        /// <param name="message"></param>
        private void ServerManager__HandleFHLRspGetFinnallyScoreCallBack(RspFHLGetFinnallyScoreMessage message)
        {
            if (projectType != 0) return;
            if (_isStart)
            {
                byte[] equipMentCode = message.equipMentCode;
                byte[] scoreCode = message.scoreCode;
                int score = 0;
                if (scoreCode.Length == 4)
                {
                    score = (int)scoreCode[0] * 1000 + (int)scoreCode[1] * 100 + (int)scoreCode[2] * 10 + (int)scoreCode[3];
                }
                else return;
                if (projectType != 0) return;
                if(_FHLControlls.Count > 0)
                {
                    var con =_FHLControlls.Find(A=>A.EquipMentCode== equipMentCode);
                    if (con != null)
                    {
                        con.p_Score=score.ToString();
                        string studentName=con.p_Name;
                        string IDNumber = con.p_IdNumber;
                        if (!string.IsNullOrEmpty(studentName) && !string.IsNullOrEmpty(IDNumber))
                        {
                            if (currentStudentDataModels.Count > 0)
                            {
                                var student = currentStudentDataModels.Find(a => a.studentName == studentName && a.idNumber.Equals(IDNumber)&&a.equipmentCode.Equals(equipMentCode));
                                if (student != null)
                                {
                                    student.FhlScoreData.score = score.ToString();
                                }
                                else
                                {
                                    return;
                                }
                            }
                        }

                    }
                }
            }
        }
        /// <summary>
        /// 收到肺活量匹配成功的委托实现
        /// </summary>
        /// <param name="message"></param>
        private void ServerManager__HandleFHLRspMatchSucessCallBack(RspFHLMatchSucessMessage message)
        {
            if (projectType != 0) return;
            if (_isStart == false)
            {
                byte[] cmdCode = message.cmdCode;
                byte[] stateCode = message.stateCode;
                string cmd = Encoding.ASCII.GetString(cmdCode);
                string st = Encoding.ASCII.GetString(stateCode);
                if(cmd == "start" && st == "ok")
                {
                    byte[]equipMentCode= message.equipMentCode;
                    if (_FHLControlls.Count > 0)
                    {
                        var con = _FHLControlls.Find(a => a.EquipMentCode == equipMentCode);
                        if (con != null)
                        {
                            Console.WriteLine("启动测试成功");
                        }
                    }
                }
            }
            if (_isStart == false)
            {
                byte[] cmdCode = message.cmdCode;
                byte[] stateCode = message.stateCode;
                string cmd = Encoding.ASCII.GetString(cmdCode);
                string st = Encoding.ASCII.GetString(stateCode);
                if (cmd == "match" && st == "ok")
                {
                    byte[] equipMentCode = message.equipMentCode;
                    if (_FHLControlls.Count > 0)
                    {
                        var con = _FHLControlls.Find(a => a.EquipMentCode == null);
                        if (con != null)
                        {
                            con.EquipMentCode = equipMentCode;
                            string equip = $"SGTZ设备:" + equipMentCode[0] + equipMentCode[1];
                            con.p_toolState = equip;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else return;
        }
        /// <summary>
        /// 收到肺活量开始测试的委托实现
        /// </summary>
        /// <param name="message"></param>
        private void ServerManager__HandelFHLRspStartSucessCallBack(RspFHLStartSucessMessage message)
        {
            if (projectType != 0) return;
            if (_isStart == false)
            {
                byte[] cmdCode = message.cmdCode;
                byte[] stateCode = message.stateCode;
                string cmd = Encoding.ASCII.GetString(cmdCode);
                string st = Encoding.ASCII.GetString(stateCode);
                if (cmd == "start" && cmd == "ok")
                {
                    byte[] equipMentCode = message.equipMentCode;
                    if (_FHLControlls.Count > 0)
                    {
                        var con = _FHLControlls.Find(a => a.EquipMentCode == equipMentCode);
                        if (con != null)
                        {
                            if(equipmentCodeList.Count > 0)
                            {
                                 if(!equipmentCodeList.Contains(equipMentCode)) equipmentCodeList.Add(equipMentCode);
                                 else { return; }
                            }
                            else
                            {
                                equipmentCodeList.Add(equipMentCode);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        private void ServerManager__RecieveDataCallBack(byte[] message)
        {
            int len = message.Length;
            _logServer.Info(message);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        private void ServerManager__SendDataCallBack(string message)
        {
            message = "正在往串口发送:" + message;
            _logServer.Info(message);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        private void ServerManager__LogDataCallBack(string message)
        {
            _logServer.Warn(message);
        }
        /// <summary>
        /// 刷新串口
        /// </summary>
        /// <returns></returns>
        private string[] RefreshSerialPort()=>_ServerManager.RefreshSerialPort();
        /// <summary>
        /// 打开串口
        /// </summary>
        /// <param name="port"></param>
        /// <param name="nBaudrate"></param>
        /// <returns></returns>
        private  bool OpenSerialPortService(string port,int nBaudrate = 115200)=>_ServerManager.OpenSerialPortService(port, nBaudrate);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        private bool IsOpenSerialService() => _ServerManager.IsConnectionSerialPort();
        /// <summary>
        /// 关闭串口
        /// </summary>
        private void CloseSerialPort()=>_ServerManager.CloseSerialPortService();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="portName"></param>
        private string PortNameToPort(string portName)=>_ServerManager.PortNameToPort(portName);
        /// <summary>
        /// 发送广播名
        /// </summary>
        private void SendBlueToothName()=>_ServerManager.SendBlueToothName();
        /// <summary>
        /// 发送匹配设备
        /// </summary>
        private void SendMatchEquipMentMessage() => _ServerManager.SendMatchMachineMessage(projectType);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="equipMentCode"></param>
        private void SendStartTestingMessage(byte[] equipMentCode) => _ServerManager.SendStartMessage(projectType, equipMentCode);
        /// <summary>
        /// 
        /// </summary>
        private void ShowEquipMentWindow()
        {
            EquipMentSettingWindow equipMentSetting = new EquipMentSettingWindow();
            if(equipMentSetting.ShowDialog() == DialogResult.OK)
            {
                equipMentCount=equipMentSetting.EquipMentCount;
                ports = equipMentSetting.Ports;
                SetCurrentEquipMentCount(equipMentCount);

            }
        }
       
        

        #endregion
        #region 计时器任务
        /// <summary>
        /// 检测是否可以开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_isCanStart==false)
            {
                if (projectType == 1)
                {
                    int lens = _SGTZControlls.Count;
                    if (lens >0)
                    {
                        var cons = _SGTZControlls.FindAll(A => A.EquipMentCode != null);
                        if(cons != null)
                        {
                            int len= cons.Count;
                            if (len < lens) _isCanStart=false;
                            else
                            {
                                _isCanStart=true;
                            }       
                        }
                        else
                        {
                            _isCanStart=false;
                        }
                    }
                    else
                    {
                        _isCanStart = false;
                        return;
                    }
                }
                else if(projectType == 0)  
                {
                    int lens = _FHLControlls.Count;
                    if(lens >0)
                    {
                        var cons = _FHLControlls.FindAll(A => A.EquipMentCode != null);
                        if (cons != null)
                        {
                            int len = cons.Count;
                            if (len < lens) _isCanStart = false;
                            else
                            {
                                _isCanStart = true;
                            }
                        }
                        else
                        {
                            _isCanStart=false;
                        }
                    }
                    else
                    {
                        _isCanStart = false;
                        return ;
                    }
                }
            }
        }
        /// <summary>
        /// 检测是否可以写入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer2_Tick(object sender, EventArgs e)
        {
            if(!isSendStart)
            {
                if (timer3.Enabled)
                {
                    timer3.Enabled = false;
                    timer3.Stop();
                }
            }
            if (_isCanWrite == false)
            {
                if(projectType==0)//肺活量
                {
                    int len = _FHLControlls.Count;
                    if(len > 0)
                    {
                        var controlls= _FHLControlls.FindAll(A => !string.IsNullOrEmpty(A.p_Score));
                        if(controlls != null)
                        {
                            int lens = controlls.Count;
                            if (lens!=len)
                            {
                                _isCanWrite=false;
                            }
                            else
                            {
                                _isCanWrite = true;
                            }
                        }
                        else
                        {
                            _isCanWrite=false;
                        }
                    }
                    else
                    {
                        _isCanWrite=false ;
                        return ;
                    }
                }
                else if(projectType==1)
                {
                    int len=_SGTZControlls.Count;
                    if (len > 0)
                    {
                        var cons =_SGTZControlls.FindAll(a=>!string.IsNullOrEmpty(a.p_Score)&&!string.IsNullOrEmpty(a.p_Score1)&&!string.IsNullOrEmpty(a.p_Score2)  );
                        if( cons != null )
                        {
                            int lens = cons.Count;
                            if(len!=lens)_isCanWrite=false;
                            else
                            {
                                _isCanWrite=true;
                            }
                        }
                        else
                        {
                            _isCanWrite= false;
                        }
                    }
                    else
                    {
                        _isCanWrite=false;
                        return ;
                    }
                }
            }
        }
        /// <summary>
        /// 是否还需要继续发开始测试
        /// </summary>
        private bool isSendStart= true;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer3_Tick(object sender, EventArgs e)
        {
            if (_isStart)
            {
                if (isSendStart)
                {
                    int len = equipmentCodeList.Count;
                    if (len > 0)
                    {
                        if (projectType == 0) //FHL
                        {
                            int lens = _FHLControlls.Count;
                            if (lens > 0) // 不相等继续发
                            {
                                if (lens != len)
                                {
                                    var equipCode = _FHLControlls[0].EquipMentCode;// 只需要第一台设备的设备号
                                    SendStartTestingMessage(equipCode);
                                }
                                else
                                {
                                    isSendStart= false;
                                }
                            }
                            else
                            {

                            }
                        }
                    }
                }
            }
            else
            {
                return ;
            }
        }

        #endregion

        #region  页面事件
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton1_Click(object sender, EventArgs e)
        {
           LoadingPortData();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton2_Click(object sender, EventArgs e)
        {
            if (IsOpenSerialService())
            {
                UIMessageBox.ShowWarning("串口已打开");
                return;
            }
            else
            {
                OpenSerialPortService(PortNameToPort(currentChoosePort));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton3_Click(object sender, EventArgs e)
        {
             CloseSerialPort();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton4_Click(object sender, EventArgs e)
        {
            SetCurrentProjectGroupData();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton5_Click(object sender, EventArgs e)
        {
            if(isTemp==false)
                AutoMatchStudent();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton6_Click(object sender, EventArgs e)
        {
            UIMessageBox.ShowWarning("请保证你选择的学生是未测试状态！！");
            ChooseMatchStudent();
        }

        #region  临时分组
        /// <summary>
        /// 临时分组的刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton7_Click(object sender, EventArgs e)
        {
            if (!_isStart)
            {
                if (IsMakeGroupSucess) IsMakeGroupSucess = false;
                if (tempStudentModels.Count > 0)
                {
                    tempStudentModels.Clear();
                   ShowStudentDataInDataGridView(uiDataGridView2, tempStudentModels);
                }
                string groupName = "DYL__" + DateTime.Now.ToString("yyyy_MMdd HH:ss:MM");
                uiComboBox2.Items.Add(groupName);
                uiComboBox2.SelectedItem = groupName;
                // uiButton12.Enabled = false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton8_Click(object sender, EventArgs e)
        {
            SelectStudentDataByIDNumber();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton9_Click(object sender, EventArgs e)
        {

            if (tempStudentModels.Count == 0)
            {
                UIMessageBox.ShowWarning("当前没有考生，无法完成编组,请重试");
                return;
            }
            else
            {
                string name = uiComboBox2.Text;
                if (!string.IsNullOrEmpty(name))
                {
                    IsMakeGroupSucess = true;
                    if(currentStudentDataModels.Count > 0)currentStudentDataModels.Clear();
                    runningWindowSys.SaveChipInfosData(tempStudentModels, projectID, name,projectType);
                  
                }
            }
        
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton10_Click(object sender, EventArgs e)
        {
            ClearAllMatchControll();
            CreateCurrentStudentData(tempStudentModels);
        }
        /// <summary>
        /// 匹配设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton12_Click(object sender, EventArgs e)
        {
             SendMatchEquipMentMessage();
        }

        private bool IsMakeGroupSucess = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiComboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
              if(uiDataGridView2.Rows.Count>0)uiDataGridView2.Rows.Clear();
              string sl = uiComboBox5.Text.Trim();
              if (currentRoundCount != 0&&!string.IsNullOrEmpty(sl))
              {
                  tempStudentModels = runningWindowSys.LoadingStudentChipData(sl,projectType,currentRoundCount );
                  if (tempStudentModels != null && tempStudentModels.Count > 0)
                  {
                      IsMakeGroupSucess = true;
                      uiButton8.Enabled = false;
                      uiButton9.Enabled = false;
                      ShowStudentDataInDataGridView(uiDataGridView2,tempStudentModels);
                      if (tempStudentModels .Count > 0)
                      {
                          CreateCurrentStudentData(tempStudentModels);
                      }
                  }
              }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiComboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentRoundCount = uiComboBox6.SelectedIndex + 1;
            SelectStudentDataByIDNumber( );
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiTextBox1_TextChanged(object sender, EventArgs e)
        {
            
            SelectStudentDataByIDNumber();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tempStudentIdNumber"></param>
        /// <param name="i"></param>
        private void SelectStudentDataByIDNumber( )
        {
            if (!string.IsNullOrEmpty(uiComboBox5.Text))
            {
                string idnumber = uiTextBox1.Text.Trim();
                if (!string.IsNullOrEmpty(idnumber))
                {
                    if (IsMakeGroupSucess == false)
                    {
                        StudentDataModel studentDataModel=runningWindowSys.SelectStudentDataByIDNumber(projectID, idnumber, currentRoundCount,projectType);
                        if (studentDataModel != null && tempStudentModels.Count > 0)
                        {
                            var data = tempStudentModels.Find(a => a.studentName == studentDataModel.studentName && a.idNumber.Equals(studentDataModel.idNumber));
                            if (data != null)
                            {
                                UIMessageBox.ShowWarning("已添加");
                                return;
                            }
                        }
                        tempStudentModels.Add(studentDataModel);
                        uiTextBox1.Text = null;
                        if (!studentDataModel.isTest) Speaking($"请{studentDataModel.studentName}做好测试准备");
                        ShowStudentDataInDataGridView(uiDataGridView2,tempStudentModels);
                    }
                    if (uiButton9.Enabled == false)
                    {
                        uiButton9.Enabled = true;
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }

        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void uiButton11_Click(object sender, EventArgs e)
        {
            ShowEquipMentWindow();
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton13_Click(object sender, EventArgs e)
        {
            if (_isCanStart)
            {
                
                if(timer1.Enabled==true){timer1.Enabled =false;timer1.Stop();}

                if (timer3.Enabled == false) { timer3.Enabled = true; timer3.Start(); }
                _isStart = true;
                if(_isCanWrite)_isCanWrite=false;
                
                if(equipmentCodeList.Count>0)equipmentCodeList.Clear();

                if (projectType == 0) //FHL
                {
                    var cons = _FHLControlls.FindAll(A => A.EquipMentCode == null);
                    if (cons.Count > 0)
                    {
                        UIMessageBox.ShowWarning("当前处于匹配设备中，无法开始！！");
                        return;
                    }
                    else
                    {
                        var equipCode= _FHLControlls[0].EquipMentCode;// 只需要第一台设备的设备号
                        SendStartTestingMessage(equipCode);
       
                    }
                }
                else
                {
                    var cons = _SGTZControlls.FindAll(A => A.EquipMentCode == null);
                    if (cons.Count > 0)
                    {
                        UIMessageBox.ShowWarning("当前处于匹配设备中，无法开始！！");
                        return;
                    }
                    else
                    {
                        var equipCode = _SGTZControlls[0].EquipMentCode;// 只需要第一台设备的设备号
                        SendStartTestingMessage(equipCode);
                    }
                }
                if (timer2.Enabled == false) { timer2.Enabled = true; timer2.Start(); }

            }
            else
            {
                UIMessageBox.ShowWarning("当前处于匹配设备中，无法开始！！");
                return;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton14_Click(object sender, EventArgs e)
        {
            if (_isCanWrite)
            {
                if(_isStart)_isStart = false;
                if( runningWindowSys.WriteStudentDataToDataBase(projectID,groupName,currentRoundCount,projectType, currentStudentDataModels))
                {
                    UIMessageBox.ShowSuccess("写入成功");
                    if (isTemp == false) SelectCurrentStudentData();
                    else
                    {
                        tempStudentModels.Clear();
                        currentStudentDataModels.Clear();
                        uiDataGridView2.Rows.Clear();
                        tempStudentModels = runningWindowSys.LoadingStudentChipData(uiComboBox5.Text.Trim(), projectType, roundCount);
                        if(tempStudentModels.Count > 0)
                        {
                            IsMakeGroupSucess= true;
                            uiButton9.Enabled = false;
                            uiButton8.Enabled=false;
                            ShowStudentDataInDataGridView(uiDataGridView2,tempStudentModels);
                            CreateCurrentStudentData(studentDataModels);
                        }
                        else
                        {
                            uiButton9.Enabled = true;
                            uiButton8.Enabled= false;
                            IsMakeGroupSucess = false;
                        }
                    }

                }
                else
                {
                    UIMessageBox.ShowWarning("写入失败");
                    return;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton15_Click(object sender, EventArgs e)
        {
            UIMessageBox.ShowWarning("请前往主页上传");
            return ;
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton16_Click(object sender, EventArgs e)
        {
            runningWindowSys.ExportCurentProjectStudentScore(projectType, projectID,roundCount);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton17_Click(object sender, EventArgs e)
        {
            runningWindowSys.PriteCurrentChooseGroupData(projectType, projectID, roundCount, groupName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string po = uiComboBox1.Text.Trim();
            if (!string.IsNullOrEmpty(po)) currentChoosePort = po;
            string ports = currentChoosePort;
            OpenSerialPortService(ports, nBaudrate);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string pl = uiComboBox2.Text;
            nBaudrate = int.Parse(pl);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        
        private void uiComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            string groups = uiComboBox3.Text;
            if (!string.IsNullOrEmpty(groups))groupName = groups;
            if (currentRoundCount != 0)
            {
                SelectCurrentStudentData();
            }
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiComboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = uiComboBox4.SelectedIndex;
            currentRoundCount = index + 1;
            SelectCurrentStudentData();
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 缺考ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetStudentErrorData("缺考");
        }

        private void 中退ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetStudentErrorData("中退");
        }

        private void 犯规ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetStudentErrorData("犯规");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 弃权ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetStudentErrorData("弃权");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiTabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(uiTabControl2.SelectedIndex==0)
            {
                if (UIMessageBox.ShowAsk("当前是否切换到系统分组"))
                {
                    isTemp= false;
                    scanerHook.Stop();
                }

            }
            else
            {
                if (UIMessageBox.ShowAsk("当前是否将模式切换到临时分组模式"))
                {
                    isTemp = true; 
                    StartHookListen();
                    if (roundCount != null)
                    {
                         
                        if (uiComboBox1.Items.Count > 0)
                            uiComboBox1.Items.Clear();
                        for (int i = 0; i < roundCount; i++)
                        {
                            uiComboBox1.Items.Add((i + 1).ToString());
                        }
                        if (uiComboBox1.Items.Count > 0)
                        {
                            uiComboBox1.SelectedIndex = 0;
                        }
                    }
                    runningWindowSys.LoadingChipInfos(uiComboBox5);
                    SetDataGridViewColumText(uiDataGridView2);
                }
            }
        }

        private void StartHookListen()
        {
            if (isTemp)
            {
                scanerHook.Start();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        private void SetStudentErrorData(string v)
        {
            if(uiDataGridView1.SelectedRows.Count > 0)
            {
                for (int i = 0; i < uiDataGridView1.SelectedRows.Count; i++)
                {
                    string idnumber = uiDataGridView1.SelectedRows[i].Cells[7].Value.ToString();
                    string name = uiDataGridView1.SelectedRows[i].Cells[5].Value.ToString();
                    runningWindowSys.SetStudentErrorData(projectID,projectType , idnumber, name, v, currentRoundCount,groupName);
                }
            }
            SelectCurrentStudentData();
        }

        /// <summary>
        /// 
        /// </summary>
        private void SelectCurrentStudentData()
        {
            if (string.IsNullOrEmpty(groupName) == false)
            {
                List<StudentDataModel> studentDataModels =
                    runningWindowSys.LoadingCurrentStudentData(projectType, projectID, groupName, currentRoundCount);
                if (studentDataModels != null && studentDataModels.Count > 0)
                {
                    this.studentDataModels = studentDataModels;
                    ShowStudentDataInDataGridView(uiDataGridView1,studentDataModels);

                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiDataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex >= 0)
                {
                    //uiDataGridView1.ClearSelection();
                   contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="studentDataGridView"></param>
        private void ShowStudentDataInDataGridView(UIDataGridView studentDataGridView,List<StudentDataModel> studentDataModel)
        {
            studentDataGridView.Rows.Clear();
            if (studentDataModel.Count>0)
            {
                int len = studentDataModel.Count;
                DataGridViewRow[] rows= new DataGridViewRow[len];
                int index = 1;
                foreach (var student in studentDataModel)
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
                        row.Cells.Add(_DataGridService.SetNewDataGridViewCell(student.SgtzScoreData.roundCount, Color.Black, Color.White));
                        row.Cells.Add(_DataGridService.SetNewDataGridViewCell(student.SgtzScoreData.score1, student.SgtzScoreData.score1=="未测试"? Color.Red:Color.SpringGreen, Color.White));
                        row.Cells.Add(_DataGridService.SetNewDataGridViewCell(student.SgtzScoreData.score2, student.SgtzScoreData.score2=="未测试"? Color.Red:Color.SpringGreen, Color.White));
                        row.Cells.Add(_DataGridService.SetNewDataGridViewCell(student.SgtzScoreData.score3, student.SgtzScoreData.score3=="未测试"? Color.Red:Color.SpringGreen, Color.White));

                    }
                    else
                    {
                        row.Cells.Add(_DataGridService.SetNewDataGridViewCell(student.FhlScoreData.roundCount,  Color.Black, Color.White));

                        row.Cells.Add(_DataGridService.SetNewDataGridViewCell(student.FhlScoreData.score,
                            student.FhlScoreData.score == "未测试" ? Color.Red : Color.SpringGreen, Color.White));
                    }
                    rows[index-1]=row;
                    index++;
                }
                
                studentDataGridView.Rows.AddRange(rows);
                
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataGridView"></param>
        private void SetDataGridViewColumText(UIDataGridView dataGridView )
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
            if (projectType==1)//SGTZ
            {
                dataGridView.Columns.Add(_DataGridService.SetDataGridViewTextBoxColumn("轮次", 100));
                dataGridView.Columns.Add(_DataGridService.SetDataGridViewTextBoxColumn("身高成绩", 100));
                dataGridView.Columns.Add(_DataGridService.SetDataGridViewTextBoxColumn("体重成绩", 100));
                dataGridView.Columns.Add(_DataGridService.SetDataGridViewTextBoxColumn("BMI成绩", 100));

            }
            else
            {
                dataGridView.Columns.Add(_DataGridService.SetDataGridViewTextBoxColumn("轮次", 100));
                dataGridView.Columns.Add(_DataGridService.SetDataGridViewTextBoxColumn("成绩", 100));
            }
            
        }
        /// <summary>
        /// 
        /// </summary>
        private void AutoMatchStudent()
        {
            ClearAllMatchControll();
            if(currentStudentDataModels.Count>0)currentStudentDataModels.Clear();
            if (studentDataModels.Count>0)
            {
                CreateCurrentStudentData(studentDataModels);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void ChooseMatchStudent()
        {
            if (uiDataGridView1.SelectedRows.Count>0)
            {
                List<StudentDataModel> studentDataModelss = new List<StudentDataModel>();
                for (int j = 0; j < uiDataGridView1.SelectedRows.Count; j++)
                {
                    string studentName = uiDataGridView1.SelectedRows[j].Cells[5].Value.ToString();
                    string idnumber = uiDataGridView1.SelectedRows[j].Cells[7].Value.ToString();
                    if (studentDataModels.Count > 0)
                    {
                        var stu=   studentDataModels.Find(a => a.studentName == studentName && a.idNumber == idnumber);
                        if (stu != null)
                        {
                            if (stu.isTest) break;
                            else
                            {
                               studentDataModelss.Add(stu);
                            }
                            
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                if (studentDataModelss.Count > 0)
                {
                    CreateCurrentStudentData(studentDataModelss);
                }
            }
            else
            {
                UIMessageBox.ShowWarning("请先选择你需要匹配的学生！！");
                return;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void CreateCurrentStudentData(  List<StudentDataModel> studentDataModelss)
        {
            if (projectType == 1) // SGTZ
            {
                List<StudentDataModel> studentDataModel= studentDataModelss.FindAll(a => a.isTest == false);
                if (studentDataModel.Count>0 && _SGTZControlls.Count > 0)
                {
                    int len = studentDataModel.Count;
                    int lens = _SGTZControlls.Count;
                    if (len >= lens)
                    {
                        for (int i = 0; i < lens; i++)
                        {

                            StudentDataModel stu = studentDataModel[i];
                            stu.equipmentCode = _SGTZControlls[i].EquipMentCode;
                            currentStudentDataModels.Add(stu);
                                 
                        }
                    }
                    else
                    {
                        for (int i = 0; i < lens; i++)
                        {
                            if (i <= len)
                            {
                                StudentDataModel stu = studentDataModel[i];
                                stu.equipmentCode = _SGTZControlls[i].EquipMentCode;
                                currentStudentDataModels.Add(stu);
                            }
                            else
                            {
                                currentStudentDataModels.Add(new StudentDataModel()
                                {
                                    personId = i.ToString(),
                                    schoolName = "",
                                    sex = "",
                                    groupName = "",
                                    idNumber = "",
                                    isTest = false,
                                    studentName = "",
                                    gradeName = "",
                                    className = "",
                                    SgtzScoreData = new SGTZScoreData()
                                    {
                                        roundCount = currentRoundCount,
                                        score1 = "",
                                        score2 = "",
                                        score3 = "",
                                    },
                                    equipmentCode = null

                                }) ;
                            }
                        }
                    }
                }
            }
            else
            {
                var studentDataModel= studentDataModelss.FindAll(a => a.isTest == false);
                if (studentDataModel.Count>0 && _FHLControlls.Count > 0)
                {
                    int len = studentDataModel.Count;
                    int lens = _FHLControlls.Count;
                    if (len >= lens)
                    {
                        for (int i = 0; i < lens; i++)
                        {
                            StudentDataModel stu = studentDataModel[i];
                            stu.equipmentCode = _FHLControlls[i].EquipMentCode;
                            currentStudentDataModels.Add(stu);
                                 
                        }
                    }
                    else
                    {
                        for (int i = 0; i < lens; i++)
                        {
                            if (i <= len)
                            {
                                StudentDataModel stu = studentDataModel[i];
                                stu.equipmentCode = _FHLControlls[i].EquipMentCode;
                                currentStudentDataModels.Add(stu);
                            }
                            else
                            {
                                currentStudentDataModels.Add(new StudentDataModel()
                                {
                                    personId = i.ToString(),
                                    schoolName = "",
                                    sex = "",
                                    groupName = "",
                                    idNumber = "",
                                    isTest = false,
                                    studentName = "",
                                    gradeName = "",
                                    className = "",
                                    FhlScoreData= new FHLScoreData()
                                    {
                                        roundCount = currentRoundCount,
                                        score = ""
                                    },
                                    equipmentCode=null,
                                    
                                });
                            }
                        }
                    }
                }
            }
            AutoMatchControlls();
        }
        /// <summary>
        /// 
        /// </summary>
        private void AutoMatchControlls()
        {
            if (currentStudentDataModels.Count > 0)
            {
                if (projectType == 1&& _SGTZControlls.Count>0) //SGTZ
                {
                    for (int i = 0; i < currentStudentDataModels.Count; i++)
                    {
                        var student = currentStudentDataModels[i];
                        _SGTZControlls[i].p_Name = student.studentName;
                        _SGTZControlls[i].p_IdNumber = student.idNumber;
                        _SGTZControlls[i].p_Score1 = "";
                        _SGTZControlls[i].p_Score2 = "";
                        _SGTZControlls[i].p_Score = "";
                        if(_SGTZControlls[i].p_roundCbx_items.Count == 0)
                        {
                            List<string> items = new List<string>();
                            for(int J = 0; J< roundCount;J++)
                            {
                                items.Add($"第{J + 1}轮");
                            }
                            _SGTZControlls[i].p_roundCbx_items = items; 
                        }
                        _SGTZControlls[i].p_roundCbx_selectIndex = student.SgtzScoreData.roundCount - 1;
                        _SGTZControlls[i].p_stateCbx_selectIndex = 0;
                        
                    }
                }
                else if (projectType == 0&&_FHLControlls.Count>0) // FHL
                {
                    for (int i = 0; i < currentStudentDataModels.Count; i++)
                    {
                        var student = currentStudentDataModels[i];
                        _FHLControlls[i].p_Name = student.studentName;
                        _FHLControlls[i].p_IdNumber = student.idNumber;
                        _FHLControlls[i].p_Score = "";
                        if (_FHLControlls[i].p_roundCbx_items.Count == 0)
                        {
                            List<string> items = new List<string>();
                            for (int J = 0; J < roundCount; J++)
                            {
                                items.Add($"第{J + 1}轮");
                            }
                            _FHLControlls[i].p_roundCbx_items = items;
                        }
                        _FHLControlls[i].p_roundCbx_selectIndex = student.FhlScoreData.roundCount - 1;
                        _FHLControlls[i].p_stateCbx_selectIndex = 0;

                    }
                }
            }
        }
        private bool IsSpeeking = false;
       /// <summary>
       /// 
       /// </summary>
       /// <param name="saying"></param>
        private void Speaking(string saying)
        {
            IsSpeeking = true;
            string say = saying;
            Thread task = new Thread(new ThreadStart(() =>
            {
                SpeechSynthesizer speech = new SpeechSynthesizer();
                speech.Volume = 100; //音量
                System.Globalization.CultureInfo keyboardCulture = System.Windows.Forms.InputLanguage.CurrentInputLanguage.Culture;
                InstalledVoice neededVoice = speech.GetInstalledVoices(keyboardCulture).FirstOrDefault();
                if (neededVoice == null)
                {
                    say = "未知的操作";
                }
                else
                {
                    speech.SelectVoice(neededVoice.VoiceInfo.Name);
                }
                speech.Speak(say);
                IsSpeeking = false;
            }));
            task.Start();
        }






        #endregion

        private void uiDataGridView2_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {

        }
    }
}
