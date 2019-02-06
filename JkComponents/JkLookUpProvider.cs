using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace JkComponents
{
    public class JkLookUpProvider: UserControl
    {
        public JkLookUpProvider()
        {
            this.BackColor = Color.Silver;
            JkLookUpProviders.Add(this);
        }
    }

    public static class JkLookUpProviders
    {
        public static List<JkLookUpProvider> List = new List<JkLookUpProvider>();

        public static void Add(JkLookUpProvider item)
        {
            if (List.Find(l => l.Name == item.Name) == null)
                List.Add(item);
        }

        public static JkLookUpProvider FindByName(String name)
        {
            return List.Find(l => l.Name == name);
        }
    }
}
