using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Design;
using System.Data;
using System.Data.SqlClient;

namespace JkComponents
{
    [ToolboxItem(true)]
    public class JkDataSet: Control
    {
        [Category("(Custom)")]
        [Editor(typeof(JkTextPropertyTypeEditor), typeof(UITypeEditor))]
        public String CommandText { get; set; }

        private List<JkDataSetParameter> _Parameters = new List<JkDataSetParameter>();
        [Category("(Custom)")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<JkDataSetParameter> Parameters
        {
            get { return _Parameters; }
            set { _Parameters = value;  }
        }

        private List<JkDataSetColumn> _Columns = new List<JkDataSetColumn>();
        [Category("(Custom)")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<JkDataSetColumn> Columns
        {
            get { return _Columns; }
            set { _Columns = value; }
        }

        private bool _ZLoadColumns;
        [Category("(Custom)")]
        public bool ZLoadColumns
        { 
            get { return _ZLoadColumns; }
            set
            {
                _ZLoadColumns = value;

                if (String.IsNullOrWhiteSpace(CommandText) || !value)
                    Columns.Clear();
                else if(!String.IsNullOrWhiteSpace(CommandText) && value)
                    LoadColumns();
            }
        }

        [Browsable(false)]
        public bool Active = false;

        [Category("(Custom)")]
        public JkConnection Connection { get; set; }

        public delegate void BeforeOpenHandler();
        public event BeforeOpenHandler BeforeOpen;
        protected virtual void OnBeforeOpen()
        {
            if (BeforeOpen != null)
                BeforeOpen();
        }

        private DataTable DataTable = new DataTable();

        public JkDataSet()
        {
            this.BackColor = Color.Khaki;
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

        private void LoadColumns()
        {
            SqlDataAdapter DataAdapter = new SqlDataAdapter(CommandText, Connection.ConnectionString);

            try
            {
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
                    foreach (DataColumn Column in DataTable.Columns)
                    {
                        JkDataSetColumn DatasetColumn = new JkDataSetColumn();

                        DatasetColumn.Name = Column.ColumnName;
                        if (Columns.Find(col => col.Name == Column.ColumnName) == null)
                            Columns.Add(DatasetColumn);
                    }
                }
                finally
                {
                    DataAdapter.SelectCommand.Connection.Close();
                    DataAdapter.Dispose();
                    DataTable.Clear();
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        public void Open()
        {
            OnBeforeOpen();
            SqlDataAdapter DataAdapter = new SqlDataAdapter(CommandText, Connection.ConnectionString);

            try
            {
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
                }
                finally
                {
                    DataAdapter.SelectCommand.Connection.Close();
                    DataAdapter.Dispose();
                    Active = true;
                }
            }
            catch(Exception ex)
            {
                new Exception("Object Name: " + this.Name + "\rError: " + ex.Message);
            }
        }

        public void Close()
        {
            Active = false;
            DataTable.Clear();
        }

        public JkDataSetParameter ParamByName(String ParameterName)
        {
            JkDataSetParameter param = Parameters.Find(p => p.Name == ParameterName);

            if (param == null)
                throw new Exception("Object Name: " + this.Name + "\rError: No parameter found named: " + ParameterName);
            else
                return param;
        }

        public int RecordCount()
        {
            return DataTable.Rows.Count;
        }

        public Object Lookup(String KeyField, Object KeyValue, String ResultField)
        {
            if (!Active)
                Open();

            return DataTable.Select(KeyField + "=" + KeyValue)[0][ResultField];
        }

        public Object Lookup(String[] KeyField, Object[] KeyValue, String ResultField)
        {
            String filtering = null;

            if (!Active)
                Open();

            foreach (String field in KeyField)
            {
                if (!String.IsNullOrWhiteSpace(filtering))
                    filtering = filtering + " AND ";

                foreach (String value in KeyValue)
                {
                    filtering = field + "=" + value;
                }
            }

            return DataTable.Select(filtering)[0][ResultField];
        }
    }
}
