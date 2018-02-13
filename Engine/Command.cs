using Microsoft.Data.Mashup;
using Microsoft.Mashup.Engine.Interface;
using Microsoft.Mashup.Tools;
using PowerQueryNet.Client;
using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace PowerQueryNet.Engine
{
    public class Command : IDisposable
    {
        public CredentialStore CredentialStore { get; set; }

        private QueryExecutor queryExecutor = new QueryExecutor();

        public Command(CommandCredentials commandCredentials)
        {
            CredentialStore = commandCredentials.CredentialStore;
        }        

        public DataTable Execute(string queryName, string mashup)
        {
            DataTable dataTable = null;
            MashupConnectionStringBuilder mcsb = GetMashupConnectionStringBuilder(mashup);
            
            mcsb.Location = queryName;

            using (Task<QueryExecutionResults> task = queryExecutor.CreateExecution(mcsb, CredentialStore, true))
            {
                if (task == null)
                    throw new Exception("QueryExecutionResults is null.");

                task.RunSynchronously();
                if (task.Result.Error != null)
                    throw task.Result.Error;

                dataTable = task.Result.Table;
                dataTable.TableName = mcsb.Location;
            }

            return dataTable;
        }

        private static MashupConnectionStringBuilder GetMashupConnectionStringBuilder(string mashup)
        {
            IError error;
            MashupConnectionStringBuilder mcsb = Utilities.CreateMashupConnectionInfo(mashup, out error);
            if (mcsb == null)
            {
                if (error != null)
                    throw new Exception(error.Message);
                else
                    throw new Exception("CreateMashupConnectionInfo has failed.");
            }
            return mcsb;
        }

        public static string MashupFromFile(string fileName)
        {
            string fileExtension = Path.GetExtension(fileName);
            switch (fileExtension)
            {
                case ".xlsx":
                    return Utilities.MashupFromXlsx(fileName);
                case ".pbix":
                case ".pbit":
                    return Utilities.MashupFromPbix(fileName);
                default:
                    return null;
            }
        }

        public static Queries MashupToQueries(string fileName)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            queryExecutor.Cancel();
            queryExecutor = null;
            MashupConnection.TryCleanup();
        }
    }
}
