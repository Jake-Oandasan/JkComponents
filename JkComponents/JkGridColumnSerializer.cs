using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace JkComponents
{
    public static class JkGridColumnSerializer
    {
        public static void SerializeGrid(JkDataGridView grid, String filename)
        {
            using (TextWriter writer = new StreamWriter(filename))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<JkGridColumnProperties>));
                List<JkGridColumnProperties> propCollection = new List<JkGridColumnProperties>();

                foreach(DataGridViewColumn column in grid.Columns)
                {
                    JkGridColumnProperties prop = new JkGridColumnProperties();

                    if (column is DataGridViewCheckBoxColumn)
                        prop.ColumnType = JkDataGridView.ColumnType.CheckBoxColumn;
                    else if (column is DataGridViewComboBoxColumn)
                        prop.ColumnType = JkDataGridView.ColumnType.ComboBoxColumn;
                    else
                        prop.ColumnType = JkDataGridView.ColumnType.TextBoxColumn;

                    prop.Name = column.Name;
                    prop.HeaderText = column.HeaderText;
                    prop.Width = column.Width;
                    prop.Visible = column.Visible;
                    prop.DataPropertyName = column.DataPropertyName;
                    prop.ReadOnly = column.ReadOnly;

                    propCollection.Add(prop);
                }

                try
                {
                    serializer.Serialize(writer, propCollection);
                    writer.Close();
                }
                catch(Exception ex)
                {
                    throw new Exception("JkGridColumnSerializer Error: " + ex.Message);
                }
            }
        }

        public static void DeserializeGrid(JkDataGridView grid, String filename)
        {
            if (!File.Exists(filename))
                throw new Exception("Filename: " + filename + " doesn't exists");

            using (TextReader reader = new StreamReader(filename))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<JkGridColumnProperties>));
                List<JkGridColumnProperties> propCollection;

                try
                {
                    propCollection = serializer.Deserialize(reader) as List<JkGridColumnProperties>;
                    reader.Close();
                }
                catch (Exception ex)
                {
                    throw new Exception("JkGridColumnSerializer Error: " + ex.Message);
                }

                foreach (JkGridColumnProperties prop in propCollection)
                    grid.CreateColumn(prop.ColumnType, grid.DataSet.Columns.Find(c => c.Name == prop.DataPropertyName), prop, grid);
            }
        }

        public class JkGridColumnProperties
        {
            [XmlAttribute]
            public JkDataGridView.ColumnType ColumnType { get; set;}

            [XmlAttribute]
            public String Name { get; set; }

            [XmlAttribute]
            public String HeaderText { get; set; }

            [XmlAttribute]
            public int Width { get; set; }

            [XmlAttribute]
            public bool Visible { get; set; }

            [XmlAttribute]
            public String DataPropertyName { get; set; }

            [XmlAttribute]
            public bool ReadOnly { get; set; }
        }
    }
}
