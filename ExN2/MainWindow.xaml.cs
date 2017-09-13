using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace ExN2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        CfgTreeNode_VM[] nodes;

        public MainWindow() {
            InitializeComponent();

            nodes = new CfgTreeNode_VM[] {
                    new CfgTreeNode_Loaders_VM()
                    /*new CfgTreeNode_Archivers_VM("archivers", "archiver.png")*/
                };

            CfgTreeNode_Loaders_VM node0 = nodes[0] as CfgTreeNode_Loaders_VM;
            CfgEventLoader loader = new CfgEventLoader();
            node0.AddLeaf(loader);

            /*CfgTreeNode_Archivers_VM node1 = nodes[1] as CfgTreeNode_Archivers_VM;
            CfgTreeArchiver_VM archiver = new CfgTreeArchiver_VM();*/
            //node1.AddLeaf(archiver);
            //node1.AddLeaf(archiver);


            treeView.ItemsSource = nodes;
            //treeView.
        }

        private void button_Exit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void button_Open_Click(object sender, RoutedEventArgs e) {
            //            treeView.ItemsSource = nodes;
        }

        private void button_Save_Click(object sender, RoutedEventArgs e) {
            //nodes[0].LeafList[1].LeafName = "Zmena";
        }

 
        //...............................................................................
        private void treeView_BtnNew(object sender, RoutedEventArgs e) {
            //Object ob = treeView.SelectedItem;
            //if (Common.IsItLoader(ob)) {
                nodes[0].New(this);
            //}
        }

        //...............................................................................
        private void treeView_BtnDel(object sender, RoutedEventArgs e) {
        }

        //...............................................................................
        private void treeView_BtnEdit(object sender, RoutedEventArgs e) {
        }

        //...............................................................................
        private void MenuItem_Help(object sender, RoutedEventArgs e) {
            Dlg_About Dlg = new Dlg_About();
            Dlg.Owner = this;
            Dlg.ShowDialog();
        }

        //...............................................................................
        private void MenuItem_LoadOldIni(object sender, RoutedEventArgs e) {
            Dlg_ConvertOldLoader Dlg = new Dlg_ConvertOldLoader();
            Dlg.Owner = this;
            Dlg.ShowDialog();
        }

        //...............................................................................
        private void SomeCommand(object sender, RoutedEventArgs e) {
            System.Windows.MessageBox.Show("Esc");

        }
    }
}
