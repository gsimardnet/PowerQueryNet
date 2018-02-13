using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PowerQueryNet.Client
{
    [ServiceContract(Namespace = "")]
    public interface IPowerQueryService
    {
        [OperationContract]
        ExecuteResponse Execute(string queryName, Queries queries, Credentials credentials);

        [OperationContract]        
        string ExecuteToSQL(string connectionString, string queryName, Queries queries, Credentials credentials);

        [OperationContract]
        string MashupFromFile(string fileName);

        [OperationContract]
        void Stop();
    }
}
