using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Data;

namespace JkComponents
{
    public class JkDataGridView: DataGridView
    {
        private Panel GridParent = new Panel();
        private FlowLayoutPanel GridFooter = new FlowLayoutPanel();

        [Browsable(false)]
        public JkDetailDataSet DataSet { get; set; }

        public JkDataGridView()
        {
            this.AutoGenerateColumns = false;
            InitializeComponent();
            ApplyStyleOnGrid(this);
        }

        private void InitializeComponent()
        {
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // JkDataGridView
            // 
            this.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.JkDataGridView_CellEndEdit);
            this.ParentChanged += new System.EventHandler(this.JkDataGridView_ParentChanged);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        private void JkDataGridView_ParentChanged(object sender, EventArgs e)
        {
            //change the parent of grid on runtime
            if (!DesignMode && this.Parent != null && this.Parent != GridParent)
            {
                GridFooter.BackColor = Color.Silver;
                GridFooter.BorderStyle = BorderStyle.Fixed3D;
                GridFooter.WrapContents = false;

                this.Parent.Controls.Add(GridParent);
                this.Parent.Controls.Remove(this);

                this.Dock = DockStyle.Fill;
                GridFooter.Dock = DockStyle.Bottom;

                this.Size = new Size(GridParent.Size.Width, GridParent.Size.Height - 35);
                GridFooter.Size = new Size(GridParent.Size.Width, 35);

                GridParent.Controls.Add(this);
                GridParent.Controls.Add(GridFooter);
                GridParent.Dock = DockStyle.Fill;
            }
        }

        public static void ApplyStyleOnGrid(DataGridView gridView)
        {
            gridView.GridColor = Color.Peru;
            gridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(180, 255, 200);
            gridView.DefaultCellStyle.BackColor = Color.WhiteSmoke;
            gridView.BorderStyle = BorderStyle.Fixed3D;
        }

        public void CreateFooter()
        {
            JkDetailColumn ic;
            int EstimatedWidth = 0, offset = 35;

            if (VisibleColumnCount() != 0)
                EstimatedWidth = Convert.ToInt32((this.Width) / VisibleColumnCount()) - 18;

            GridFooter.Padding = new Padding(offset, 3, 3, 3);

            if (DataSet.Columns.Count > 0)
            {
                for (int i = 0; i <= DataSet.Columns.Count - 1; i++)
                {
                    ic = DataSet.Columns[i];
                    if (ic.Visible)
                    {
                        Label lblFooter = new Label();
                        lblFooter.Name = "lblFooter" + ic.Caption.Trim();
                        lblFooter.TextAlign = ContentAlignment.MiddleCenter;
                        lblFooter.Text = AssignFooterValue(ic.FooterType, "0");
                        lblFooter.Font = new Font(this.Font.Name, this.Font.Size, FontStyle.Bold);

                        if (DataSet.GridAutoSize)
                            lblFooter.Width = EstimatedWidth;
                        else
                            lblFooter.Width = ic.Width;

                        if (i == 0)
                            lblFooter.Width -= offset;

                        if (ic.FooterType != JkDetailColumn.ColumnFooterTypes.ftNone)
                            lblFooter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

                        if (GridFooter.Controls.Find(lblFooter.Name, false).Length == 0)
                            GridFooter.Controls.Add(lblFooter);
                    }
                }
            }
        }

        public void ComputeFooterValues()
        {
            String value = "";

            foreach (JkDetailColumn ic in DataSet.Columns)
            {
                if (ic.Visible && ic.FooterType != JkDetailColumn.ColumnFooterTypes.ftNone)
                {
                    if (ic.FooterType == JkDetailColumn.ColumnFooterTypes.ftCount)
                        value = DataSet.DataTable.Rows.Count.ToString();
                    //todo: other footer types

                    if (ic.FooterType == JkDetailColumn.ColumnFooterTypes.ftSum)
                    {
                        double total = 0;

                        foreach (DataRow row in DataSet.DataTable.Rows)
                        {
                            total += Convert.ToDouble(row[ic.Name]);
                        }

                        value = total.ToString("N2");
                    }

                    foreach (Control c in GridFooter.Controls)
                    {
                        if (c.Name == "lblFooter" + ic.Caption.Trim())
                        {
                            c.Text = AssignFooterValue(ic.FooterType, value);
                        }
                    }
                }
            }
        }

        private int VisibleColumnCount()
        {
            int count = 0;

            for (int i = 0; i <= this.Columns.Count - 1; i++)
            {
                if (this.Columns[i].Visible)
                    count += 1;
            }

            return count;
        }

        private String AssignFooterValue(JkDetailColumn.ColumnFooterTypes FooterType, String value)
        {
            String text = "";

            if (FooterType == JkDetailColumn.ColumnFooterTypes.ftNone)
                text = "";
            else if (FooterType == JkDetailColumn.ColumnFooterTypes.ftAvg)
                text = "Avg:   " + value;
            else if (FooterType == JkDetailColumn.ColumnFooterTypes.ftCount)
                text = "Count:   " + value;
            else if (FooterType == JkDetailColumn.ColumnFooterTypes.ftMax)
                text = "Max:   " + value;
            else if (FooterType == JkDetailColumn.ColumnFooterTypes.ftMin)
                text = "Min:   " + value;
            else if (FooterType == JkDetailColumn.ColumnFooterTypes.ftSum)
                text = "Sum:   " + value;

            return text;
        }

        private void JkDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            ComputeFooterValues();
        }
    }
}
