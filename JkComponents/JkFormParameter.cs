using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace JkComponents
{
    [Serializable]
    public class JkFormParameter
    {
        public String Name { get; set; }
        public String Value { get; set; }
        public int Index { get; set; }
        public SqlDbType Type { get; set; }
        public String ControlName { get; set; } //for assigning of lookup values
        public bool Visible { get; set; }
        public String Caption { get; set; }
        public int Width { get; set; }
    }
}
