using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PowerQueryNet.Client
{
    /// <summary>
    /// Interface for PowerQueryService
    /// </summary>
    [ServiceContract(Namespace = "")]
    public interface IPowerQueryService
    {
        /// <summary>
        /// Execute the specified query.
        /// </summary>
        /// <param name="powerQueryCommand">Inputs for the method</param>
        /// <returns></returns>
        [OperationContract]
        PowerQueryResponse Execute(PowerQueryCommand powerQueryCommand);

        /// <summary>
        /// Get the mashup (queries) from an Excel or Power BI file.
        /// </summary>
        /// <param name="fileName">Full path of the file</param>
        /// <returns></returns>
        [OperationContract]
        string MashupFromFile(string fileName);

        //[OperationContract]
        //void Stop();
    }
}
