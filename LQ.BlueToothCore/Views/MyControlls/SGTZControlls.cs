using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LQ.BlueToothCore.Views.MyControlls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SGTZControlls : UserControl
    {
        public SGTZControlls()
        {
            InitializeComponent();
        }
        private byte[] equipMentCode;
        /// <summary>
        /// 设备号
        /// </summary>
        public byte[] EquipMentCode
        {
            get => equipMentCode;
            set => equipMentCode = value;
        }

        [Description("标题"), Category("自定义属性")]
        public string p_title
        {
            get
            {
                return this.mProjectName.Text;
            }
            set
            {
                this.mProjectName.Text = value;
            }
        }
        [Description("标题颜色"), Category("自定义属性")]
        public Color p_title_Color
        {
            get
            {
                return this.mProjectName.BackColor;
            }
            set
            {
                this.mProjectName.BackColor = value;
            }
        }
        [Description("姓名"), Category("自定义属性")]
        public string p_Name
        {
            get
            {
                return this.uiLabel1.Text;
            }
            set
            {
                this.uiLabel1.Text = value;
            }
        }
        [Description("考号"), Category("自定义属性")]
        public string p_IdNumber
        {
            get
            {
                return this.uiLabel2.Text;
            }
            set
            {
                this.uiLabel2.Text = value;
            }
        }
        [Description("身高成绩"), Category("自定义属性")]
        public string p_Score
        {
            get
            {
                return this.uiLabel3.Text;
            }
            set
            {
                this.uiLabel3.Text = value;
            }
        }
        [Description("体重成绩"), Category("自定义属性")]
        public string p_Score1
        {
            get
            {
                return this.uiLabel4.Text;
            }
            set
            {
                this.uiLabel4.Text = value;
            }
        }
        [Description("BMI成绩"), Category("自定义属性")]
        public string p_Score2
        {
            get
            {
                return this.uiLabel5.Text;
            }
            set
            {
                this.uiLabel4.Text = value;
            }
        }
        [Description("设备状态"), Category("自定义属性")]
        public string p_toolState
        {
            get
            {
                return this.toolStripStatusLabel1.Text;
            }
            set
            {
                this.toolStripStatusLabel1.Text = value;
            }
        }
        [Description("设备状态颜色"), Category("自定义属性")]
        public Color p_toolState_color
        {
            get
            {
                return this.toolStripStatusLabel1.ForeColor;
            }
            set
            {
                this.toolStripStatusLabel1.ForeColor = value;
            }
        }
        [Description("轮次"), Category("自定义属性")]
        public int p_roundCbx_selectIndex
        {
            get
            {
                return this.uiComboBox1.SelectedIndex;
            }
            set
            {
                this.uiComboBox1.SelectedIndex = value;
            }
        }

        [Description("轮次items"), Category("自定义属性")]
        public List<string> p_roundCbx_items
        {
            get
            {
                List<string> items = new List<string>();
                foreach (var item in uiComboBox1.Items)
                {
                    items.Add(item.ToString());
                }
                return items;
            }
            set
            {
                uiComboBox1.Items.Clear();
                foreach (var item in value)
                {
                    uiComboBox1.Items.Add(item);
                }
            }
        }

        [Description("状态"), Category("自定义属性")]
        public int p_stateCbx_selectIndex
        {
            get
            {
                return this.uiComboBox2.SelectedIndex;
            }
            set
            {
                uiComboBox2.SelectedIndex = value;
            }
        }
    }
}
