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
        [Category("(Custom)")]
        [Editor(typeof(JkListPropertyTypeEditor), typeof(UITypeEditor))]
        public String DataSet { get; set; }

        [Category("(Custom)")]
        public string Key { get; set; }

        [Category("(Custom)")]
        public string DisplayText { get; set; }

        [Browsable(false)]
        public int SelectedKey
        {
            get
            {
                if (this.SelectedItem == null)
                    return Convert.ToInt32(null);

                return (this.SelectedItem as JkLookupItem).Key;
            }
            set
            {
                foreach(Object item in this.Items)
                {
                    if ((item as JkLookupItem).Key == value)
                    {
                        this.SelectedItem = item;
                        this.Text = (item as JkLookupItem).DisplayText;
                    }
                }
            }
        }

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
        }

        public void LoadData()
        {
            if (String.IsNullOrWhiteSpace(DataSet))
                return;

            JkDataSet DS = null;

            foreach (Control provider in JkLookUpProviders.List)
            {
                foreach (Control dataset in provider.Controls)
                {
                    if (dataset.Name == DataSet)
                    {
                        DS = (dataset as JkDataSet);
                        break;
                    }
                }
            }

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

                    foreach (DataRow row in DS.DataTable.Rows)
                    {
                        this.Items.Add(new JkLookupItem(Convert.ToInt32(row[Key]), row[DisplayText].ToString()));
                    }
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

        private class JkLookupItem
        {
            private int _Key;
            private string _DisplayText;

            public JkLookupItem(int Key, string DisplayText)
            {
                _Key = Key;
                _DisplayText = DisplayText;
            }

            public int Key
            {
                get { return _Key; }
                set { _Key = value; }
            }

            public string DisplayText
            {
                get { return _DisplayText; }
                set { _DisplayText = value; }
            }

            public override string ToString()
            {
                return DisplayText;
            }
        }
    }
}
