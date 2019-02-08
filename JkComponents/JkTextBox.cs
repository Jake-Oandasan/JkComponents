using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace JkComponents
{
    public class JkTextBox: TextBox
    {
        private bool _Required;
        private String WatermarkText = "Required";
        public bool Required
        {
            get
            {
                return _Required;
            }
            set
            {
                _Required = value;

                if (value)
                    AddWaterMark();
                else
                    RemoveWaterMark();
            }
        }
        private Panel WaterMarkHandler = new Panel();

        public JkTextBox()
        {
            WaterMarkHandler.Paint += WaterMarkHandler_Paint;
            WaterMarkHandler.Click += WaterMarkHandler_Click;

            this.SuspendLayout();
            this.Leave += JkTextBox_Leave;
            this.Enter += JkTextBox_Enter;
            this.ResumeLayout(false);
        }

        private void AddWaterMark()
        {
            if (String.IsNullOrWhiteSpace(this.Text))
            {
                this.Controls.Add(WaterMarkHandler);
            }
        }

        private void RemoveWaterMark()
        {
            this.Controls.Remove(WaterMarkHandler);
        }

        private void WaterMarkHandler_Paint(object sender, PaintEventArgs e)
        {
            using (Graphics g = e.Graphics)
            {
                using (Brush b = new SolidBrush(Color.Gray))
                {
                    g.DrawString(WatermarkText, this.Font, b, new PointF(-1.0F, 1.0F));
                }
            }
        }

        private void WaterMarkHandler_Click(object sender, EventArgs e)
        {
            this.Controls.Clear();
            this.Focus();
        }

        private void JkTextBox_Leave(object sender, EventArgs e)
        {
            if (Required)
                AddWaterMark();
        }

        private void JkTextBox_Enter(object sender, EventArgs e)
        {
            this.Controls.Clear();
            this.Focus();
        }
    }
}
