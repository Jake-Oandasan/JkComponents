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
using System.Xml.Serialization;

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
                    GridView.UpdateGridSize(value, true);

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
                        GridView.CreateGrid();
                        GridView.UpdateGridSize(GridAutoSize);
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

        private JkDataGridView _GridView;
        [Category("(Custom)")]
        public JkDataGridView GridView
        {
            get { return _GridView; }
            set
            {
                _GridView = value;

                if (value == null)
                    _GridView.DataSet = null;
                else
                    _GridView.DataSet = this;
            }
        }

        [Category("(Custom)")]
        public String ConnectionString { get; set; }

        [Browsable(false)]
        public DataTable DataTable = new DataTable();

        private bool _LinkToMaster = true;
        [Category("(Custom)")]
        public bool LinkToMaster { get { return _LinkToMaster; } set { _LinkToMaster = value; } }

        public JkDetailDataSet()
        {
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

        public void AddTemporaryColumns()
        {
            foreach (JkDetailColumn column in this.Columns)
            {
                if (column.Temporary && !this.DataTable.Columns.Contains(column.Name))
                {
                    DataColumn tableColumn = new DataColumn();

                    tableColumn.ColumnName = column.Name;
                    tableColumn.DataType = JKTypeConverter.ToNetType(JKTypeConverter.ToDbType(column.DataType));
                    tableColumn.DefaultValue = column.DefaultValue;
                    tableColumn.AllowDBNull = !column.Required;
                    tableColumn.Caption = column.Caption;

                    this.DataTable.Columns.Add(tableColumn);
                }
            }
        }

        public void RemoveTemporaryColumns()
        {
            foreach (JkDetailColumn column in this.Columns)
            {
                if (column.Temporary && this.DataTable.Columns.Contains(column.Name))
                    this.DataTable.Columns.Remove(column.Name);
            }
        }
    }
}
