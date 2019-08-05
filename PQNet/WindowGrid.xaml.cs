using PowerQueryNet.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PowerQueryNet.PQNet
{
    /// <summary>
    /// Interaction logic for WindowGrid.xaml
    /// </summary>
    public partial class WindowGrid : Window
    {
        public WindowGrid(DataTable dataTable)
        {
            InitializeComponent();

            Title += " - " + dataTable.TableName;

            MyDataGrid.DataContext = dataTable.DefaultView;
        }
    }
}
