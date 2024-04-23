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
    public partial class FHLControlls : UserControl
    {
        public FHLControlls()
        {
            InitializeComponent();
        }
        private byte[] equipMentCode;
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
                return this.mStudentName.Text;
            }
            set
            {
                this.mStudentName.Text = value;
            }
        }
        [Description("考号"), Category("自定义属性")]
        public string p_IdNumber
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
        [Description("分数"), Category("自定义属性")]
        public string p_Score
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
