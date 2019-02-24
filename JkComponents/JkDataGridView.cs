﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Data;
using System.IO;

namespace JkComponents
{
    public class JkDataGridView: DataGridView
    {
        private Panel GridParent = new Panel();
        private FlowLayoutPanel GridFooter = new FlowLayoutPanel();
        public enum ColumnType { CheckBoxColumn, ComboBoxColumn, TextBoxColumn }

        [Browsable(false)]
        public JkDetailDataSet DataSet { get; set; }

        public JkDataGridView()
        {
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
            this.DataMemberChanged += new System.EventHandler(this.JkDataGridView_DataMemberChanged);
            this.DataSourceChanged += new System.EventHandler(this.JkDataGridView_DataSourceChanged);
            this.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.JkDataGridView_CellFormatting);
            this.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.JkDataGridView_CellValueChanged);
            this.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.JkDataGridView_ColumnWidthChanged);
            this.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.JkDataGridView_DataError);
            this.DefaultValuesNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.JkDataGridView_DefaultValuesNeeded);
            this.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.JkDataGridView_EditingControlShowing);
            this.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.JkDataGridView_RowsAdded);
            this.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.JkDataGridView_RowsRemoved);
            this.Scroll += new System.Windows.Forms.ScrollEventHandler(this.JkDataGridView_Scroll);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.JkDataGridView_MouseClick);
            this.ParentChanged += new System.EventHandler(this.JkDataGridView_ParentChanged);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        private void JkDataGridView_ParentChanged(object sender, EventArgs e)
        {
            //change the parent of grid on runtime
            if (!DesignMode && this.Parent != null && this.Parent != GridParent)
            {
                GridParent.Name = this.Name + "Parent";
                GridFooter.Name = this.Name + "Footer";

                GridFooter.BackColor = Color.Ivory;
                GridFooter.BorderStyle = BorderStyle.Fixed3D;
                GridFooter.WrapContents = false;
                //remove auto scrolling feature of the footer
                GridFooter.AutoScroll = false;
                GridFooter.Dock = DockStyle.Bottom;

                this.Parent.Controls.Add(GridParent);
                this.Parent.Controls.Remove(this);

                this.Size = new Size(GridParent.Size.Width, GridParent.Size.Height - 35);
                GridFooter.Size = new Size(this.Size.Width, 35);

                GridParent.Controls.Add(this);
                GridParent.Controls.Add(GridFooter);
                GridParent.Dock = DockStyle.Fill;

                this.Dock = DockStyle.Fill;
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
            JkDetailColumn ic = null;
            int EstimatedWidth = 0, offset = 35, gridWidth = 0;
            
            if (VisibleColumnCount() != 0)
                EstimatedWidth = Convert.ToInt32((this.Width) / VisibleColumnCount()) - 18;

            GridFooter.Padding = new Padding(3, 3, 3, 3);

            if (DataSet.Columns.Count > 0)
            {
                Label lblOffset = new Label();

                lblOffset.Name = "lblFooterOffset";
                lblOffset.Width = offset;
                lblOffset.Margin = new Padding(0, 0, 0, 0);

                if (GridFooter.Controls.Find(lblOffset.Name, false).Length == 0)
                    GridFooter.Controls.Add(lblOffset);

                for (int i = 0; i <= DataSet.Columns.Count - 1; i++)
                {
                    ic = DataSet.Columns[i];

                    foreach (DataGridViewColumn column in this.Columns)
                    {
                        if (column.DataPropertyName == ic.Name)
                            gridWidth = column.Width;
                    }

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
                            lblFooter.Width = gridWidth;

                        if (ic.FooterType != JkDetailColumn.ColumnFooterTypes.ftNone)
                        {
                            lblFooter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                            lblFooter.Margin = new Padding(2, 0, 0, 0);
                        }
                        else
                        {
                            lblFooter.Margin = new Padding(0, 0, 0, 0);
                        }

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
                if (ic.Visible
                    && ic.FooterType != JkDetailColumn.ColumnFooterTypes.ftNone
                    && DataSet.DataTable.Columns.Contains(ic.Name))
                {
                    if (ic.FooterType == JkDetailColumn.ColumnFooterTypes.ftAvg)
                    {
                        double total = 0;

                        if (this.Rows.Count > 0 && this.NewRowIndex != 0)
                        {
                            foreach (DataRow row in DataSet.DataTable.Rows)
                                if (row.RowState != DataRowState.Deleted && row[ic.Name] != null)
                                    total += Double.Parse(row[ic.Name].ToString());

                            total = total / DataSet.DataTable.Rows.Count;
                        }
                        value = total.ToString("N2");
                    }
                    else if (ic.FooterType == JkDetailColumn.ColumnFooterTypes.ftCount)
                        value = DataSet.DataTable.Rows.Count.ToString();
                    else if (ic.FooterType == JkDetailColumn.ColumnFooterTypes.ftMax)
                    {
                        double max = 0;

                        if (this.Rows.Count > 0 && this.NewRowIndex != 0)
                        {
                            foreach (DataRow row in DataSet.DataTable.Rows)
                                if (row.RowState != DataRowState.Deleted && row[ic.Name] != null && Double.Parse(row[ic.Name].ToString()) > max)
                                    max = Double.Parse(row[ic.Name].ToString());
                        }

                        if (ic.DataType == SqlDbType.BigInt || ic.DataType == SqlDbType.Int)
                            value = max.ToString();
                        else
                            value = max.ToString("N2");
                    }
                    else if (ic.FooterType == JkDetailColumn.ColumnFooterTypes.ftMin)
                    {
                        double min = 2147483647;

                        if (this.Rows.Count > 0 && this.NewRowIndex != 0)
                        {
                            foreach (DataRow row in DataSet.DataTable.Rows)
                                if (row.RowState != DataRowState.Deleted && row[ic.Name] != null && Double.Parse(row[ic.Name].ToString()) < min)
                                    min = Double.Parse(row[ic.Name].ToString());
                        }
                        else
                            min = 0;

                        if (ic.DataType == SqlDbType.BigInt || ic.DataType == SqlDbType.Int)
                            value = min.ToString();
                        else
                            value = min.ToString("N2");
                    }
                    else if (ic.FooterType == JkDetailColumn.ColumnFooterTypes.ftSum)
                    {
                        double total = 0;

                        if (this.Rows.Count > 0 && this.NewRowIndex != 0)
                        {
                            foreach (DataRow row in DataSet.DataTable.Rows)
                                if (row.RowState != DataRowState.Deleted && row[ic.Name] != null)
                                    total += Double.Parse(row[ic.Name].ToString());
                        }

                        value = total.ToString("N2");
                    }

                    foreach (Control c in GridFooter.Controls)
                    {
                        if (c.Name == "lblFooter" + ic.Caption.Trim())
                            c.Text = AssignFooterValue(ic.FooterType, value);
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

        private void JkDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            //to remove error generated by .Net when assigning a value at a ComboBox
        }

        private void JkDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (this.AllowUserToAddRows && e.Control != null && e.Control is ComboBox)
            {
                (e.Control as ComboBox).IntegralHeight = false;
                (e.Control as ComboBox).MaxDropDownItems = 10;
            }
        }

        private void JkDataGridView_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            foreach (Control c in GridFooter.Controls)
            {
                if (c.Name == "lblFooter" + e.Column.HeaderText.Trim())
                {
                    c.Width = e.Column.Width;
                }
            }
        }

        private void JkDataGridView_Scroll(object sender, ScrollEventArgs e)
        {
            HScrollBar hScrollBar = this.Controls.OfType<HScrollBar>().First();

            //GridFooter will scroll depending on scroll of the grid
            if (hScrollBar.Visible && e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                GridFooter.HorizontalScroll.Maximum = hScrollBar.Maximum;
                GridFooter.HorizontalScroll.Minimum = hScrollBar.Minimum;
                GridFooter.HorizontalScroll.LargeChange = hScrollBar.LargeChange;
                GridFooter.HorizontalScroll.SmallChange = hScrollBar.SmallChange;
                GridFooter.HorizontalScroll.Value = e.NewValue;
                GridFooter.Update();
                this.Update();
            }
        }

        private void JkDataGridView_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            foreach (JkDetailColumn column in DataSet.Columns)
            {
                if (!String.IsNullOrWhiteSpace(column.DefaultValue))
                {
                    foreach(DataGridViewColumn gridColumn in this.Columns)
                    {
                        if (!String.IsNullOrWhiteSpace(gridColumn.DataPropertyName) &&
                            gridColumn.DataPropertyName == column.Name)
                            e.Row.Cells[gridColumn.Index].Value = column.DefaultValue;
                    }
                }
            }
        }

        private void JkDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            ComputeFooterValues();
        }

        private void JkDataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            ComputeFooterValues();
        }

        private void JkDataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            ComputeFooterValues();
        }

        private void JkDataGridView_DataSourceChanged(object sender, EventArgs e)
        {
            this.AutoGenerateColumns = false;
        }

        private void JkDataGridView_DataMemberChanged(object sender, EventArgs e)
        {
            this.AutoGenerateColumns = false;
        }

        public static void ApplyColumnStyle(JkDetailColumn detailColumn, DataGridViewColumn gridColumn)
        {
            if (detailColumn.DataType == SqlDbType.DateTime) //checkbox
                gridColumn.DefaultCellStyle.Format = "MM'/'dd'/'yyyy";
            else if (!String.IsNullOrWhiteSpace(detailColumn.ControlName)) //combobox
            {
                (gridColumn as DataGridViewComboBoxColumn).DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                (gridColumn as DataGridViewComboBoxColumn).DropDownWidth = gridColumn.Width + 50;
            }
            else if (detailColumn.DataType == SqlDbType.Money //numeric textbox
                || detailColumn.DataType == SqlDbType.Float
                || detailColumn.DataType == SqlDbType.Decimal)
            {
                gridColumn.DefaultCellStyle.Format = "N2";
                gridColumn.ValueType = Type.GetType("System.Decimal");
                gridColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        public void CreateGrid()
        {
            for (int i = 0; i <= this.DataSet.Columns.Count - 1; i++)
            {
                JkDetailColumn column = this.DataSet.Columns[i];

                if (column.DataType == SqlDbType.Bit)
                    CreateColumn(ColumnType.CheckBoxColumn, column, this);
                else if (!String.IsNullOrWhiteSpace(column.ControlName))
                    CreateColumn(ColumnType.ComboBoxColumn, column, this);
                else
                    CreateColumn(ColumnType.TextBoxColumn, column, this);
            }

            foreach (DataGridViewColumn column in this.Columns)
                if (this.DataSet.GridColumn.Find(c => c.Name == column.Name) == null)
                    this.DataSet.GridColumn.Add(column);
        }

        public void CreateColumn(ColumnType type, JkDetailColumn column, DataGridView grid)
        {
            if (type == ColumnType.CheckBoxColumn)
            {
                DataGridViewCheckBoxColumn GridColCheck = new DataGridViewCheckBoxColumn();

                GridColCheck.Name = this.Name + "Column" + column.Name.Trim();
                GridColCheck.HeaderText = column.Caption;
                GridColCheck.Width = column.Width;
                GridColCheck.Visible = column.Visible;
                GridColCheck.DataPropertyName = column.Name;
                GridColCheck.ReadOnly = column.ReadOnly;

                ApplyColumnStyle(column, GridColCheck);
                grid.Columns.Add(GridColCheck);
            }
            else if (type == ColumnType.ComboBoxColumn)
            {
                DataGridViewComboBoxColumn GridColCombo = new DataGridViewComboBoxColumn();

                GridColCombo.Name = this.Name + "Column" + column.Name.Trim();
                GridColCombo.HeaderText = column.Caption;
                GridColCombo.Width = column.Width;
                GridColCombo.Visible = column.Visible;
                GridColCombo.DataPropertyName = column.Name;
                GridColCombo.ReadOnly = column.ReadOnly;

                ApplyColumnStyle(column, GridColCombo);
                grid.Columns.Add(GridColCombo);
            }
            else if (type == ColumnType.TextBoxColumn)
            {
                DataGridViewTextBoxColumn GridColText = new DataGridViewTextBoxColumn();

                GridColText.Name = this.Name + "Column" + column.Name.Trim();
                GridColText.HeaderText = column.Caption;
                GridColText.Width = column.Width;
                GridColText.Visible = column.Visible;
                GridColText.DataPropertyName = column.Name;
                GridColText.ReadOnly = column.ReadOnly;

                ApplyColumnStyle(column, GridColText);
                grid.Columns.Add(GridColText);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            //to fix unwanted creation of columns in designer.cs, which was generated by .net
            base.AutoGenerateColumns = false;
        }

        public void CreateColumn(ColumnType type, JkDetailColumn column, JkGridColumnSerializer.JkGridColumnProperties properties, DataGridView grid)
        {
            if (type == ColumnType.CheckBoxColumn)
            {
                DataGridViewCheckBoxColumn GridColCheck = new DataGridViewCheckBoxColumn();

                GridColCheck.Name = properties.Name;
                GridColCheck.HeaderText = properties.HeaderText;
                GridColCheck.Width = properties.Width;
                GridColCheck.Visible = properties.Visible;
                GridColCheck.DataPropertyName = properties.DataPropertyName;
                GridColCheck.ReadOnly = properties.ReadOnly;

                ApplyColumnStyle(column, GridColCheck);
                grid.Columns.Add(GridColCheck);
            }
            else if (type == ColumnType.ComboBoxColumn)
            {
                DataGridViewComboBoxColumn GridColCombo = new DataGridViewComboBoxColumn();

                GridColCombo.Name = properties.Name;
                GridColCombo.HeaderText = properties.HeaderText;
                GridColCombo.Width = properties.Width;
                GridColCombo.Visible = properties.Visible;
                GridColCombo.DataPropertyName = properties.DataPropertyName;
                GridColCombo.ReadOnly = properties.ReadOnly;

                ApplyColumnStyle(column, GridColCombo);
                grid.Columns.Add(GridColCombo);
            }
            else if (type == ColumnType.TextBoxColumn)
            {
                DataGridViewTextBoxColumn GridColText = new DataGridViewTextBoxColumn();

                GridColText.Name = properties.Name;
                GridColText.HeaderText = properties.HeaderText;
                GridColText.Width = properties.Width;
                GridColText.Visible = properties.Visible;
                GridColText.DataPropertyName = properties.DataPropertyName;
                GridColText.ReadOnly = properties.ReadOnly;

                ApplyColumnStyle(column, GridColText);
                grid.Columns.Add(GridColText);
            }
        }

        public void UpdateGridSize(bool AutoSize, bool ResizeColumns = false)
        {
            if (AutoSize)
                this.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            else
            {
                this.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                if (ResizeColumns)
                    foreach (DataGridViewColumn dg in this.Columns)
                    {
                        dg.Width = (this.DataSet.Columns.First(col => col.Name == dg.DataPropertyName) as JkDetailColumn).Width;
                    }
            }
            this.Update();
        }

        private void JkDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(this.Columns[e.ColumnIndex].DataPropertyName))
                if (this.DataSet.Columns.Find(c => c.Name == this.Columns[e.ColumnIndex].DataPropertyName).ReadOnly)
                    e.CellStyle.BackColor = Color.LightGray;
        }

        private void JkDataGridView_MouseClick(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Right
            //    && dataGridView.HitTest(e.X, e.Y).Type == DataGridViewHitTestType.Cell
            //    && dataGridView.HitTest(e.X, e.Y).RowIndex != dataGridView.NewRowIndex)
            //{
            //    int RowIndex = dataGridView.HitTest(e.X, e.Y).RowIndex,
            //        ColumnIndex = dataGridView.HitTest(e.X, e.Y).ColumnIndex;
            //    ContextMenu menu = new ContextMenu();

            //    MenuItem ClearMenu = new MenuItem();
            //    MenuItem CopyMenu = new MenuItem();
            //    MenuItem DeleteMenu = new MenuItem();
            //    MenuItem PasteMenu = new MenuItem();

            //    //set text
            //    ClearMenu.Text = "Clear Cell";
            //    CopyMenu.Text = "Copy";
            //    DeleteMenu.Text = "Delete Row";
            //    PasteMenu.Text = "Paste";

            //    //set event
            //    ClearMenu.Click += delegate(object s, EventArgs ea)
            //    {
            //        if (dstDetail.DataTable.Columns[dataGridView.Columns[ColumnIndex].DataPropertyName].AllowDBNull)
            //            dstDetail.DataTable.Rows[RowIndex][dataGridView.Columns[ColumnIndex].DataPropertyName] = DBNull.Value;
            //        else
            //            dstDetail.DataTable.Rows[RowIndex][dataGridView.Columns[ColumnIndex].DataPropertyName] = 0;
            //        splitContainerMasterDetail.Panel1.Focus();
            //        dataGridView.Focus();
            //    };
            //    CopyMenu.Click += delegate(object s, EventArgs ea)
            //    {
            //        Clipboard.SetText(dataGridView.Rows[RowIndex].Cells[ColumnIndex].Value.ToString(), TextDataFormat.Text);
            //    };
            //    DeleteMenu.Click += delegate(object s, EventArgs ea)
            //    {
            //        if (dataGridView.SelectedRows.Count > 0)
            //        {
            //            foreach (DataGridViewRow row in dataGridView.SelectedRows)
            //            {
            //                if (!row.IsNewRow)
            //                    dataGridView.Rows.RemoveAt(row.Index);
            //            }
            //        }
            //        else
            //            dataGridView.Rows.RemoveAt(RowIndex);

            //        splitContainerMasterDetail.Panel1.Focus();
            //        dataGridView.Focus();
            //    };
            //    PasteMenu.Click += delegate(object s, EventArgs ea)
            //    {
            //        dstDetail.DataTable.Rows[RowIndex][dataGridView.Columns[ColumnIndex].DataPropertyName] = Clipboard.GetText();
            //        splitContainerMasterDetail.Panel1.Focus();
            //        dataGridView.Focus();
            //    };

            //    //set if enabled
            //    ClearMenu.Enabled = dataGridView.Rows[RowIndex].Cells[ColumnIndex].Value != null
            //        && dataGridView.AllowUserToDeleteRows
            //        && !dataGridView.Rows[RowIndex].Cells[ColumnIndex].ReadOnly;
            //    CopyMenu.Enabled = !String.IsNullOrEmpty(dataGridView.Rows[RowIndex].Cells[ColumnIndex].Value.ToString());
            //    DeleteMenu.Enabled = dataGridView.AllowUserToDeleteRows;
            //    PasteMenu.Enabled = Clipboard.ContainsText()
            //        && dataGridView.AllowUserToAddRows
            //        && !dataGridView.Rows[RowIndex].Cells[ColumnIndex].ReadOnly;

            //    //add on ContextMenu
            //    menu.MenuItems.Add(ClearMenu);
            //    menu.MenuItems.Add(CopyMenu);
            //    menu.MenuItems.Add(DeleteMenu);
            //    menu.MenuItems.Add(PasteMenu);

            //    menu.Show(dataGridView, new Point(e.X, e.Y));
            //}
        }
    }
}