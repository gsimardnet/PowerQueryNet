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
using PowerQuery.Samples.Properties;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace PowerQuery.Samples
{
    public partial class FormMain : Form
    {
        private string queriesPath;
        private string credentialsPath;

        PowerQueryCommand powerQueryCommand;

        public FormMain()
        {
            InitializeComponent();

            queriesPath = Path.Combine(Environment.CurrentDirectory, Settings.Default.DefaultQueriesPath);
            credentialsPath = Path.Combine(queriesPath, Settings.Default.DefaultCredentialsFile);

            LoadQueries();
        }

        private void LoadQueries()
        {
            LabelStatus.Text = queriesPath;
            LabelCredential.Text = credentialsPath;
            TextBoxPQ.Text = "";
            GridResult.DataSource = null;
            LoadPowerQueryCommand();
            DisplayQueries();
        }

        private void ButtonOpenFolder_Click(object sender, EventArgs e)
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.EnsurePathExists = true;
                dialog.EnsureFileExists = false;
                dialog.AllowNonFileSystemItems = false;
                dialog.DefaultFileName = "Select Folder";

                dialog.Filters.Add(new CommonFileDialogFilter("Power Query Files", ".pq;.m"));
                dialog.Filters.Add(new CommonFileDialogFilter("Credentials Files", ".xml"));

                dialog.InitialDirectory = queriesPath;

                var commonFileDialogResult = dialog.ShowDialog();

                if (commonFileDialogResult == CommonFileDialogResult.Ok && !string.IsNullOrWhiteSpace(dialog.FileName))
                {
                    queriesPath = Directory.Exists(dialog.FileName) ? dialog.FileName : Path.GetDirectoryName(dialog.FileName);
                    credentialsPath = Path.Combine(queriesPath, Settings.Default.DefaultCredentialsFile);
                    LoadQueries();
                }
            }
        }

        private void LoadPowerQueryCommand()
        {
            if (!File.Exists(credentialsPath))
                credentialsPath = null;
            powerQueryCommand = new PowerQueryCommand
            {
                Credentials = Credentials.LoadFromFile(credentialsPath),
                Queries = Queries.LoadFromFolder(queriesPath),
            };
        }

        private void DisplayQueries()
        {
            var listPQ = new List<string>();

            foreach (Query q in powerQueryCommand.Queries)
            {
                listPQ.Add(q.Name);
            }
            ListBoxPQ.DataSource = listPQ;
        }

        private void ButtonExecute_Click(object sender, EventArgs e)
        {
            if (ListBoxPQ.SelectedItem == null) return;
            var queryName = ListBoxPQ.SelectedItem.ToString();
            var result = powerQueryCommand.Execute(queryName);

            DisplayResult(result);
        }

        private void DisplayResult(PowerQueryResponse result)
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
                GridResult.DataSource = result.DataTable;
            }
        }

        private void ListBoxPQ_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayFormula();
        }

        private void DisplayFormula()
        {
            if (ListBoxPQ.SelectedItem == null) return;
            var queryName = ListBoxPQ.SelectedItem.ToString();
            TextBoxPQ.Text = powerQueryCommand.Queries[queryName].Formula;
        }

        private void FormMain_Activated(object sender, EventArgs e)
        {
            LoadPowerQueryCommand();
            DisplayQueries();
            DisplayFormula();
        }

    }
}
