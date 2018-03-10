using Microsoft.Data.Mashup;
using Microsoft.Mashup.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerQueryNet.Engine
{
    public class CommandCredentials
    {
        internal CredentialStore CredentialStore { get; set; }

        public CommandCredentials()
        {
            CredentialStore = new CredentialStore();
        }

        public void SetCredentialFile(string fileName)
        {
            DataSource dataSource = new DataSource("File", fileName);
            DataSourceSetting dataSourceSetting = new DataSourceSetting("Windows");

            CredentialStore.SetCredential(dataSource, dataSourceSetting, null);

            return;
        }

        public void SetCredentialWeb(string url)
        {
            DataSource dataSource = new DataSource("Web", url);
            DataSourceSetting dataSourceSetting = new DataSourceSetting("Anonymous");

            CredentialStore.SetCredential(dataSource, dataSourceSetting, null);
            
            return;
        }

        public void SetCredentialSQL(string path)
        {
            DataSource dataSource = new DataSource("SQL", path);
            DataSourceSetting dataSourceSetting = new DataSourceSetting("Windows");
            
            CredentialStore.SetCredential(dataSource, dataSourceSetting, null);

            return;
        }

        public bool LoadCredentials(string fileName)
        {
            try
            {
                using (FileStream fileStream = File.OpenRead(fileName))
                    CredentialStore.Load((Stream)fileStream);
                return true;
            }
            catch (Exception ex)
            {
                if (!Microsoft.Mashup.Common.SafeExceptions.IsSafeException(ex))
                {
                    throw;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
