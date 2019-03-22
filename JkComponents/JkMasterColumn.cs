using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel;

namespace JkComponents
{
    [Serializable]
    public class JkMasterColumn
    {
        public String Name { get; set; }
        public SqlDbType DataType { get; set; }
        public String DefaultValue { get; set; }
        public String ControlName { get; set; }
        public bool Required { get; set; }
        [Browsable(false)]
        public Object Value { get; set; }
        public String LabelName { get; set; }
    }
}
