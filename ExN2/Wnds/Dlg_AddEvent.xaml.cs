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

namespace ExN2.Wnds
{
    /// <summary>
    /// Interaction logic for Dlg_AddEvent.xaml
    /// </summary>
    public partial class Dlg_AddEvent : Window
    {
        public List<int> EventTypes { get; set; }
        public List<cCfgEventItem> eventLineList { get; set; }

        public String sName { get; set; }      // item name = field name in SQL-table
        public tEventItemType Type { get; set; }       // type definuje offset v datech a zaroven typ pole v SQL-tabulce
        public bool bStore { get; set; }     // false = neukladat do DB, polozka pouze posouva offset v udalosti
        public int iLenBytes { get; set; }  // delka dat v bajtech
        public int iConstValue { get; set; }// pro pseudopole - konstantni hodnota
        public double rCoef { get; set; }      // nasobitel hodnoty pred ulozenim do DB

        public Dlg_AddEvent()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            if (eventLineList == null)
                eventLineList = new List<cCfgEventItem>();
            cCfgEventItem eventItem = new cCfgEventItem();
            eventItem.sName = sName;
            eventItem.Type = Type;
            eventItem.bStore = bStore;
            eventItem.iLenBytes = iLenBytes;
            eventItem.iConstValue = iConstValue;
            eventItem.rCoef = rCoef;
            eventLineList.Add(eventItem);
            //eventLineListView.
        }
        private void Button_Edit_Click(object sender, RoutedEventArgs e)
        {
            if (eventLineList == null)
                eventLineList = new List<cCfgEventItem>();
            cCfgEventItem itemForEdit = (cCfgEventItem)eventLineListView.SelectedItem;
            sName = itemForEdit.sName;
            Type = itemForEdit.Type;
            bStore = itemForEdit.bStore;
            iLenBytes = itemForEdit.iLenBytes;
            iConstValue = itemForEdit.iConstValue;
            rCoef = itemForEdit.rCoef;
        }
        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (eventLineList == null)
                eventLineList = new List<cCfgEventItem>();
            cCfgEventItem itemForDelete = (cCfgEventItem)eventLineListView.SelectedItem;
            eventLineList.Remove(itemForDelete);
        }
        private void button_OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (eventLineList == null)
                eventLineList = new List<cCfgEventItem>();
            cCfgEventItem itemForDelete = (cCfgEventItem)eventLineListView.SelectedItem;
            eventLineList.Remove(itemForDelete);

            cCfgEventItem newItem =new cCfgEventItem();
            newItem.sName = sName;
            newItem.Type = Type;
            newItem.iLenBytes = iLenBytes;
            newItem.iConstValue = iConstValue;
            newItem.rCoef = rCoef;
            eventLineList.Add(newItem);
        }
    }
}
