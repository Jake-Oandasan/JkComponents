using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing;

namespace JkComponents
{
    public class JkLookUpComboBox: ComboBox
    {
        private String _DataSet;
        [Category("(Custom)")]
        [Editor(typeof(JkListPropertyTypeEditor), typeof(UITypeEditor))]
        public String DataSet
        {
            get { return _DataSet; }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                    foreach (Control provider in JkLookUpProviderList.List)
                    {
                        foreach (Control dataset in provider.Controls)
                        {
                            if (dataset.Name == value)
                            {
                                DS = (dataset as JkDataSet);
                                break;
                            }
                        }
                    }

                _DataSet = value;
            }
        }

        private JkDataSet DS;

        [Category("(Custom)")]
        public string Key { get; set; }

        [Category("(Custom)")]
        public string DisplayText { get; set; }


        private String WatermarkText = "Required";
        private bool _Required;

        [Category("(Custom)")]
        public bool Required
        {
            get { return _Required; }
            set
            {
                _Required = value;

                if (value)
                    AddWaterMark();
                else
                    RemoveWaterMark();
            }
        }

        public JkLookUpComboBox()
        {
            InitializeComponent();
            this.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            this.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        public void LoadData()
        {
            if (String.IsNullOrWhiteSpace(DataSet))
                return;

            if (DS == null)
                throw new Exception(String.Format("Object name: {0} \rNo dataset found named: {1}", this.Name, DataSet));

            if (DS.Connection != null && !String.IsNullOrWhiteSpace(DS.CommandText) && this.Items.Count == 0)
            {
                if (!DS.Active)
                    DS.Open();

                if (!String.IsNullOrWhiteSpace(Key) && !String.IsNullOrWhiteSpace(DisplayText))
                {
                    if (!DS.DataTable.Columns.Contains(Key))
                        throw new Exception(String.Format("Object name: {0} \rNo Key column found named: {1}", this.Name, Key));
                    if (!DS.DataTable.Columns.Contains(DisplayText))
                        throw new Exception(String.Format("Object name: {0} \rNo DisplayText column found named: {1}", this.Name, DisplayText));

                    this.DataSource = DS.DataTable;
                    this.DisplayMember = this.DisplayText;
                    this.ValueMember = this.Key;
                    JkLookUpComboBoxList.Add(this);
                }
            }
        }

        private void AddWaterMark()
        {
            if (this.SelectedIndex == -1)
            {
                this.ForeColor = Color.Gray;
                this.Text = WatermarkText;
            }
        }

        private void RemoveWaterMark()
        {
            if (this.SelectedIndex != -1 || !Required)
            {
                if (this.Text == WatermarkText)
                    this.Text = String.Empty;

                this.ForeColor = Color.Black;
                this.Update();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // JkLookUpComboBox
            // 
            this.DropDown += new System.EventHandler(this.JkLookUpComboBox_DropDown);
            this.EnabledChanged += new System.EventHandler(this.JkLookUpComboBox_EnabledChanged);
            this.Enter += new System.EventHandler(this.JkLookUpComboBox_Enter);
            this.Leave += new System.EventHandler(this.JkLookUpComboBox_Leave);
            this.ResumeLayout(false);

        }

        private void JkLookUpComboBox_DropDown(object sender, EventArgs e)
        {
            if (this.ForeColor == Color.Gray && this.Text == WatermarkText)
            {
                this.ForeColor = Color.Black;
                this.Text = String.Empty;
            }
        }

        public void FilterDataSource()
        {
            if (DS.Filtered
                && DS.Filter != null
                && !String.IsNullOrWhiteSpace(DS.Filter))
            {
                Object value = this.SelectedValue;
                (this.DataSource as DataTable).DefaultView.RowFilter = DS.Filter;

                if (value != null && value != DBNull.Value)
                    this.SelectedValue = value;
            }
            else
                RemoveFilterOnDataSource();
        }

        public void RemoveFilterOnDataSource()
        {
            (this.DataSource as DataTable).DefaultView.RowFilter = String.Empty;
        }

        private void JkLookUpComboBox_Enter(object sender, EventArgs e)
        {
            if (this.ForeColor == Color.Gray && this.Text == WatermarkText)
            {
                this.ForeColor = Color.Black;
                this.Text = String.Empty;
            }
        }

        private void JkLookUpComboBox_Leave(object sender, EventArgs e)
        {
            if (this.Required)
                AddWaterMark();
            else
                RemoveWaterMark();
        }

        private void JkLookUpComboBox_EnabledChanged(object sender, EventArgs e)
        {
            if (this.Enabled)
                FilterDataSource();
            else
                RemoveFilterOnDataSource();
        }
    }

    public static class JkLookUpComboBoxList
    {
        public static List<JkLookUpComboBox> List = new List<JkLookUpComboBox>();

        public static void Add(JkLookUpComboBox item)
        {
            if (List.Find(l => l.Name == item.Name) == null)
                List.Add(item);
        }

        public static JkLookUpComboBox FindByName(String name)
        {
            return List.Find(l => l.Name == name);
        }
    }
}
