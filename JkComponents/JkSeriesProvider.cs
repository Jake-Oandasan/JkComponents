using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;

namespace JkComponents
{
    public class JkSeriesProvider: Control
    {
        [Category("(Custom)")]
        public String CompanyId { get; set; }

        [Category("(Custom)")]
        public String Code { get; set; }

        [Browsable(false)]
        public String Value { get; set; }

        [Category("(Custom)")]
        public String ConnectionString { get; set; }

        [Category("(Custom)")]
        public String TransactionColumn { get; set; }

        [Browsable(false)]
        private String SelectCommandText = "SELECT NextSeries AS TransactionNo FROM tblSystemSeries WHERE CompanyId = @CompanyId AND Code = @Code";

        [Browsable(false)]
        private String UpdateCommandText = "UPDATE tblSystemSeries SET NextNumber = NextNumber + 1 WHERE CompanyId = @CompanyId AND Code = @Code";

        public JkSeriesProvider()
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

        public void GetSeries()
        {
            if (String.IsNullOrWhiteSpace(CompanyId))
                throw new Exception("Object name: " + this.Name + "\rError: No CompanyId column provided.");

            if (String.IsNullOrWhiteSpace(Code))
                throw new Exception("Object name: " + this.Name + "\rError: No Code column provided.");

            if (String.IsNullOrWhiteSpace(ConnectionString))
                throw new Exception("Object name: " + this.Name + "\rError: No ConnectionString provided.");

            SqlDataAdapter DataAdapter = new SqlDataAdapter(SelectCommandText, ConnectionString);
            DataTable DataTable = new DataTable();

            try
            {
                DataAdapter.SelectCommand.Parameters.AddWithValue("@CompanyId", Convert.ToInt32(CompanyId));
                DataAdapter.SelectCommand.Parameters.AddWithValue("@Code", Code);
                DataTable.Clear();
                DataAdapter.Fill(DataTable);
                Value = DataTable.Rows[0]["TransactionNo"].ToString();
            }
            finally
            {
                DataAdapter.SelectCommand.Connection.Close();
                DataAdapter.Dispose();
                DataTable.Clear();
            }
        }

        public void UpdateSeries()
        {
            if (String.IsNullOrWhiteSpace(CompanyId as String))
                throw new Exception("Object name: " + this.Name + "\rError: No CompanyId column provided.");

            if (String.IsNullOrWhiteSpace(Code))
                throw new Exception("Object name: " + this.Name + "\rError: No Code column provided.");

            if (String.IsNullOrWhiteSpace(ConnectionString))
                throw new Exception("Object name: " + this.Name + "\rError: No ConnectionString provided.");

            SqlCommand Command = new SqlCommand();
            SqlConnection Connection = new SqlConnection(ConnectionString);

            try
            {
                try
                {
                    Command.Connection = Connection;
                    Command.CommandText = UpdateCommandText;
                    Command.CommandType = CommandType.Text;
                    Command.Parameters.AddWithValue("@CompanyId", Convert.ToInt32(CompanyId));
                    Command.Parameters.AddWithValue("@Code", Code);
                    Command.Connection.Open();
                    Command.ExecuteNonQuery();
                    Command.Connection.Close();
                }
                finally
                {
                    Command.Dispose();
                    Connection.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Object name: " + this.Name + "\rError: " + ex.Message);
            }
        }
    }
}
