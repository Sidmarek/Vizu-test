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
    public partial class MainWindow : Window
    {
        CfgTreeLeaf_VM[] mesta;
        CfgTreeLeaf_VM[] mesta2;
        CfgTreeNode_VM[] nodes;

        public MainWindow()
        {
            InitializeComponent();

            mesta = new CfgTreeLeaf_VM[] {
                    new CfgTreeLeaf_VM("Praha", "node.png"),
                    new CfgTreeLeaf_VM("Ostrava", "node2.png"),
                    new CfgTreeLeaf_VM("Brno", "root.png")
               };

            mesta2 = new CfgTreeLeaf_VM[] {
                    new CfgTreeLeaf_VM("Praha2", "node.png"),
                    new CfgTreeLeaf_VM("Ostrav2a", "node2.png"),
                    new CfgTreeLeaf_VM("Brno2", "root.png")
                };

            nodes = new CfgTreeNode_VM[] {
                    new CfgTreeNode_VM("group1", "node.png", mesta),
                    new CfgTreeNode_VM("group2", "node.png", mesta2)
                };


        }

        private void button_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void button_Open_Click(object sender, RoutedEventArgs e) {

            treeView.ItemsSource = nodes;

         }

        private void button_Save_Click(object sender, RoutedEventArgs e) {
            nodes[0].LeafList[1].LeafName = "Zmena";

         }

        private void MenuItem_Help(object sender, RoutedEventArgs e) {
            Dlg_About Dlg = new Dlg_About();
            Dlg.Owner = this;
            Dlg.ShowDialog();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) {
            Dlg_ConvertOldLoader Dlg = new Dlg_ConvertOldLoader();
            Dlg.Owner = this;
            Dlg.ShowDialog();
        }
    }
}
