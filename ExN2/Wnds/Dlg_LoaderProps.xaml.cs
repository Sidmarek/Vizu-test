using System;
using System.Collections.Generic;
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

namespace ExN2 {

    /// <summary>
    /// Interaction logic for LoaderProps.xaml
    /// </summary>
    public partial class Dlg_LoaderProps : Window {
        public string LoaderName {  get; set; }
        public string DbConnStr { get; set; }
        public string TablName { get; set; }


        public Dlg_LoaderProps() {
            InitializeComponent();
            this.DataContext = this;    // data binding
            /*LoaderName = "ahoj2";
            DbConnStr = "conn";
            TablName = "tabl";*/
        }

        private void button_Cancel_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
            Close();
        }

        private void button_OK_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
            Close();
        }
    }
}
