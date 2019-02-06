using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace JkComponents
{
    public partial class JkTextPropertyEditorForm : Form
    {
        public String Value { get { return txtInput.Text; } }

        public JkTextPropertyEditorForm(String currentValue)
        {
            InitializeComponent();
            txtInput.Text = currentValue;
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

    public class JkTextPropertyTypeEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService edSvc;
            edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            JkTextPropertyEditorForm editorForm;
            editorForm = new JkTextPropertyEditorForm((string)value);
            edSvc.ShowDialog(editorForm);

            return editorForm.Value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
