using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace JkComponents
{
    [Serializable]
    public class JkColumn
    {
        public String Name { get; set; }
        public String Caption { get; set; }
        public SqlDbType DataType { get; set; }
        public int Width { get; set; }
        public enum ColumnFooterTypes { ftNone, ftCount, ftSum, ftAvg, ftMin, ftMax };
        public ColumnFooterTypes FooterType { get; set; }
        private bool _Visible = true;
        public bool Visible
        {
            get { return _Visible; }
            set { _Visible = value; }
        }

        //todo: add lookup functionality
    }
}
