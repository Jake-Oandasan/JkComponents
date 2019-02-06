using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace JkComponents
{
    public partial class JkListPropertyEditorForm : Form
    {
        public string Value
        {
            get
            {
                if (comboBoxItem.SelectedItem == null)
                    return Convert.ToString(null);
                else
                    return comboBoxItem.SelectedItem.ToString();
            }
        }

        public JkListPropertyEditorForm(string currentValue)
        {
            InitializeComponent();

            foreach (JkLookUpProvider provider in JkLookUpProviders.List)
            {
                foreach (Control dataset in provider.Controls)
                    comboBoxItem.Items.Add(dataset.Name);
            }

            comboBoxItem.Sorted = true;
            comboBoxItem.SelectedIndex = comboBoxItem.FindStringExact(currentValue);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

    public class JkListPropertyTypeEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService edSvc;
            edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            JkListPropertyEditorForm editorForm;
            editorForm = new JkListPropertyEditorForm((string)value);
            edSvc.ShowDialog(editorForm);

            return editorForm.Value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
