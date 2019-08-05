using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerQueryNet.Client
{
    /// <summary>
    /// Enumerates the different actions available for output to SQL Table.
    /// </summary>
    public enum SqlTableAction
    {
        /// <summary>
        /// Create a new table and insert the rows
        /// </summary>
        Create = 1,

        /// <summary>
        /// Remove all existing rows from the table (Delete) and insert the new rows
        /// </summary>
        DeleteAndInsert = 2,

        /// <summary>
        /// Drop the existing table, create a new table and insert the rows
        /// </summary>
        DropAndCreate = 3,

        /// <summary>
        /// Insert the new rows to the existing table
        /// </summary>
        Insert = 4,

        /// <summary>
        /// Remove all existing rows from the table (Truncate) and insert the new rows
        /// </summary>
        TruncateAndInsert = 5
    }
}
