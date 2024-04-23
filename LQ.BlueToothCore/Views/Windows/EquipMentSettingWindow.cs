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

namespace LQ.BlueToothCore.Views.Windows
{
    public partial class EquipMentSettingWindow : UIForm
    {
        public EquipMentSettingWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 
        /// </summary>
        private string ports = "CH340";
        /// <summary>
        /// 
        /// </summary>
        public string Ports
        {
            get=>ports; 
        }
        /// <summary>
        /// 
        /// </summary>
        private int equipMentCount=0;
        /// <summary>
        /// 
        /// </summary>
        public int EquipMentCount
        {
            get => equipMentCount;  
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EquipMentSettingWindow_Load(object sender, EventArgs e)
        {
            uiComboBox1.SelectedIndex = 0;
            uiComboBox2.Items.Clear();
            for(int i=0; i<100;i++)
            {
                uiComboBox2.Items.Add((i+1).ToString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ports=uiComboBox1.Text.Trim();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string cou = uiComboBox2.Text.Trim();
            if (!string.IsNullOrEmpty(cou))
            {
                int count = int.Parse(cou);
                equipMentCount=count;
            }
            else
            {
                return;
            }

        }

        private void uiButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
