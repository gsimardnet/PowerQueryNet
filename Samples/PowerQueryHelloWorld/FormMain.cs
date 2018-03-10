using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PowerQueryNet.Client;
using System.IO;

namespace PowerQuery.Samples
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();            
        }

        private void BtnHelloWorldString_Click(object sender, EventArgs e)
        {
            var q = new Query { Formula = "let hw = \"Hello World\" in hw" };
            var pq = new PowerQueryCommand();
            var result = pq.Execute(q);
            DisplayResult(result);
        }

        private void DisplayResult(ExecuteResponse result)
        {
            if (result == null)
            {
                MessageBox.Show("Result is null.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (result.ExceptionMessage != null)
            {
                MessageBox.Show(result.ExceptionMessage, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                dgvResult.DataSource = result.DataTable;
            }
        }
    }
}
