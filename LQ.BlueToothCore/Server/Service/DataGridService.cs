using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LQ.BlueToothCore.Server
{
    /// <summary>
    /// 
    /// </summary>
    public class DataGridService
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="foreColor"></param>
        /// <param name="backColor"></param>
        /// <returns></returns>
        public DataGridViewCell SetNewDataGridViewCell(object value, Color foreColor, Color backColor)
        {
            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = value.ToString();
            cell.Style.ForeColor = foreColor;
            cell.Style.BackColor = backColor;
            return cell;
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public DataGridViewTextBoxColumn SetDataGridViewTextBoxColumn(object value, int width)
        {
            DataGridViewTextBoxColumn dataGridTextBoxColumn = new DataGridViewTextBoxColumn();
            dataGridTextBoxColumn.HeaderText = value.ToString();
            dataGridTextBoxColumn.Width = width;
            dataGridTextBoxColumn.Name = value.ToString();
            return dataGridTextBoxColumn;
        }
    }
}
