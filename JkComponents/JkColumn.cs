using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JkComponents
{
    [Serializable]
    public class JkColumn
    {
        public enum ColumnDataTypes { dtString, dtInteger, dtBoolean, dtDouble, dtDateTime };
        public String Name { get; set; }
        public String Caption { get; set; }
        public ColumnDataTypes DataType { get; set; }
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
