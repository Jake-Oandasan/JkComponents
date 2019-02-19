using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Design;
using System.Data;
using System.Data.SqlClient;

namespace JkComponents
{
    public class JkDetailDataSet: UserControl
    {
        [Editor(typeof(JkTextPropertyTypeEditor), typeof(UITypeEditor))]
        [Category("(Custom)")]
        public String CommandText { get; set; }

        private List<JkFormParameter> _Parameters = new List<JkFormParameter>();
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("(Custom)")]
        public List<JkFormParameter> Parameters { get { return _Parameters; } set { _Parameters = value; } }

        private List<JkDetailColumn> _Columns = new List<JkDetailColumn>();
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("(Custom)")]
        public List<JkDetailColumn> Columns { get { return _Columns; } set { _Columns = value; } }

        private bool _ZLoadColumns;
        [Category("(Custom)")]
        public bool ZLoadColumns
        {
            get { return _ZLoadColumns; }
            set
            {
                if (value)
                {
                    try
                    {
                        CreateColumns();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Object name: " + this.Name + "\rUnable to load columns, " + ex.Message);
                    }
                }
                else
                {
                    if (_Columns.Count > 0)
                    {
                        if (MessageBox.Show("Columns are already loaded, do you want to remove it?", "Development", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            _Columns.Clear();
                        else
                            return;
                    }
                }
                _ZLoadColumns = value;
            }
        }

        private bool _GridAutoSize = false;
        [Category("(Custom)")]
        public bool GridAutoSize
        {
            get { return _GridAutoSize; }
            set
            {
                if (GridView != null)
                    UpdateGridSize(value, true);

                _GridAutoSize = value;
            }
        }

        private List<DataGridViewColumn> _GridColumn = new List<DataGridViewColumn>();
        [Category("(Custom)")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<DataGridViewColumn> GridColumn { get { return _GridColumn; } set { _GridColumn = value; } }

        private bool _ZLoadGrid;
        [Category("(Custom)")]
        public bool ZLoadGrid
        {
            get { return _ZLoadGrid; }
            set
            {
                if (GridView != null)
                {
                    if (value)
                    {
                        CreateGrid();
                        GridView.CreateFooter();
                    }
                    else
                    {
                        GridView.Columns.Clear();
                    }
                }
                _ZLoadGrid = value;
            }
        }

        [Category("(Custom)")]
        public JkDataGridView GridView { get; set; }

        [Category("(Custom)")]
        public String ConnectionString { get; set; }

        [Browsable(false)]
        public DataTable DataTable = new DataTable();

        public JkDetailDataSet()
        {
            InitializeComponent();
            this.BackColor = Color.Tan;
            this.Visible = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            this.Size = new Size(Convert.ToInt32(e.Graphics.MeasureString(this.Name, new Font("Tahoma", 8, FontStyle.Regular)).Width) + 2, 20);

            using (SolidBrush myBrush = new SolidBrush(Color.Black))
            {
                e.Graphics.DrawString(this.Name, new Font("Tahoma", 8, FontStyle.Regular), myBrush, new PointF(0, (this.Height / 2) - 8F));
            }

            using (Pen myPen = new Pen(Color.Black))
            {
                e.Graphics.DrawRectangle(myPen, 0, 0, this.Size.Width - 1, this.Size.Height - 1);
            }
        }

        private void CreateColumns()
        {
            SqlDataAdapter DataAdapter = new SqlDataAdapter(CommandText, ConnectionString);

            try
            {
                if (Parameters.Count > 0)
                {
                    for (int i = 0; i <= Parameters.Count - 1; i++)
                    {
                        if (String.IsNullOrWhiteSpace(Parameters[i].Value))
                        {
                            DataAdapter.SelectCommand.Parameters.AddWithValue("@" + Parameters[i].Name, 0);
                        }
                    }
                }
                DataAdapter.Fill(DataTable);

                foreach (DataColumn dc in DataTable.Columns)
                {
                    JkDetailColumn column = new JkDetailColumn();

                    column.Caption = dc.ColumnName;
                    column.DataType = ConvertTypeToSqlType(dc.DataType);
                    column.Name = dc.ColumnName;
                    column.Required = !dc.AllowDBNull;
                    if (column.Name.Contains("Id"))
                        column.Visible = false;
                    column.Width = 100;

                    if (Columns.Find(col => col.Name == column.Name) == null)
                        _Columns.Add(column);
                }
            }
            finally
            {
                DataAdapter.Dispose();
            }
        }

        private SqlDbType ConvertTypeToSqlType(Type type)
        {
            SqlParameter param;
            TypeConverter converter;

            param = new SqlParameter();
            converter = TypeDescriptor.GetConverter(param.DbType);

            if (converter.CanConvertFrom(type))
                param.DbType = (DbType)converter.ConvertFrom(type.Name);
            else
            {
                try
                {
                    param.DbType = (DbType)converter.ConvertFrom(type.Name);
                }
                catch { }
            }

            return param.SqlDbType;
        }

        private void CreateGrid()
        {
            JkDetailColumn col;

            for (int i = 0; i <= Columns.Count - 1; i++)
            {
                col = Columns[i];

                DataGridViewTextBoxColumn GridColText = new DataGridViewTextBoxColumn();
                DataGridViewCheckBoxColumn GridColCheck = new DataGridViewCheckBoxColumn();
                DataGridViewComboBoxColumn GridColCombo = new DataGridViewComboBoxColumn();
                DataGridViewTextBoxColumn GridColDate = new DataGridViewTextBoxColumn();

                if (col.DataType == SqlDbType.Bit)
                {
                    GridColCheck.Name = "dataGridViewColumn" + col.Name.Trim();
                    GridColCheck.HeaderText = col.Caption;
                    GridColCheck.Width = col.Width;
                    GridColCheck.Visible = col.Visible;
                    GridColCheck.DataPropertyName = col.Name;

                    GridView.Columns.AddRange(new DataGridViewColumn[] { GridColCheck });
                    if (_GridColumn.Find(gridCol => gridCol.Name == GridColCheck.Name) == null)
                        _GridColumn.Add(GridColCheck);
                }
                else if (col.DataType == SqlDbType.DateTime)
                {
                    GridColDate.Name = "dataGridViewColumn" + col.Name.Trim();
                    GridColDate.HeaderText = col.Caption;
                    GridColDate.Width = col.Width;
                    GridColDate.Visible = col.Visible;
                    GridColDate.DataPropertyName = col.Name;
                    GridColDate.DefaultCellStyle.Format = "MM'/'dd'/'yyyy";

                    GridView.Columns.AddRange(new DataGridViewColumn[] { GridColDate });
                    if (_GridColumn.Find(gridCol => gridCol.Name == GridColDate.Name) == null)
                        _GridColumn.Add(GridColDate);
                }
                else if (!String.IsNullOrWhiteSpace(col.ControlName))
                {
                    GridColCombo.Name = "dataGridViewColumn" + col.Name.Trim();
                    GridColCombo.HeaderText = col.Caption;
                    GridColCombo.Width = col.Width;
                    GridColCombo.Visible = col.Visible;
                    GridColCombo.DataPropertyName = col.Name;
                    GridColCombo.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
                    GridColCombo.DropDownWidth = 200;

                    GridView.Columns.AddRange(new DataGridViewColumn[] { GridColCombo });
                    if (_GridColumn.Find(gridCol => gridCol.Name == GridColCombo.Name) == null)
                        _GridColumn.Add(GridColCombo);
                }
                else
                {
                    GridColText.Name = "dataGridViewColumn" + col.Name.Trim();
                    GridColText.HeaderText = col.Caption;
                    GridColText.Width = col.Width;
                    GridColText.Visible = col.Visible;
                    GridColText.DataPropertyName = col.Name;

                    if (col.DataType == SqlDbType.Money || col.DataType == SqlDbType.Float || col.DataType == SqlDbType.Decimal)
                    {
                        GridColText.DefaultCellStyle.Format = "N2";
                        GridColText.ValueType = Type.GetType("System.Decimal");
                        GridColText.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }

                    GridView.Columns.AddRange(new DataGridViewColumn[] { GridColText });
                    if (_GridColumn.Find(gridCol => gridCol.Name == GridColText.Name) == null)
                        _GridColumn.Add(GridColText);
                }
            }
            UpdateGridSize(GridAutoSize);
        }

        private void UpdateGridSize(bool AutoSize, bool ResizeColumns = false)
        {
            if (AutoSize)
                GridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            else
            {
                GridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                if (ResizeColumns)
                    foreach (DataGridViewColumn dg in GridView.Columns)
                    {
                        dg.Width = (Columns.First(col => col.Name == dg.DataPropertyName) as JkDetailColumn).Width;
                    }
            }
            GridView.Update();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // JkDetailDataSet
            // 
            this.Name = "JkDetailDataSet";
            this.ResumeLayout(false);

        }
    }
}
