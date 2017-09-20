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
        public bool Run { get; set; }
        public string LoaderName {  get; set; }
        public string DbConnStr { get; set; }
        public string TableName { get; set; }
        public string SysTableName { get; set; }
        public string UDPSocketLocal { get; set; }
        public string UDPSocketRemote { get; set; }
        public int ReceiveTimeoutMs { get; set; }
        public bool IntelOrder { get; set; }
        public bool LastPtrIsFreePtr { get; set; }
        public tN4T_version N4T_Version { get; set; }
        public int EventBodyLenBytes { get; set; }
        public int TypeFieldByteOffs { get; set; }
        public int AdjustTimePeriod_Sec { get; set; }
        public int AdjustTimeOffset_Sec { get; set; }
        public List<cfgEvent> EventsList { get; set; }

        public Dlg_LoaderProps() {
            InitializeComponent();
            this.DataContext = this;    // data binding
        }

        private void button_Cancel_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
            Close();
        }

        private void button_OK_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
            Close();
        }

        private void Button_AddEvent_Click(object sender, RoutedEventArgs e)
        {
            Wnds.Dlg_AddEvent Dlg2 = new Wnds.Dlg_AddEvent();
            bool done = (bool)Dlg2.ShowDialog();
            if (done == true)
            {
                if (EventsList == null)
                    EventsList = new List<cfgEvent>();
                EventsList.Add(new cfgEvent() { EventTypes = Dlg2.EventTypes,eventLineList = Dlg2.eventLineList});
                EventsListView.ItemsSource = EventsList;
            }
        }

        private void Button_DeleteEvent_Click(object sender, RoutedEventArgs e)
        {
            cfgEvent itemForEdit = (cfgEvent)EventsListView.SelectedItem;
            
            Wnds.Dlg_AddEvent Dlg3 = new Wnds.Dlg_AddEvent();
            bool done = (bool)Dlg3.ShowDialog();
            Dlg3.EventTypes = itemForEdit.EventTypes;
            Dlg3.eventLineList = itemForEdit.eventLineList;
            if (done == true)
            {
                if (EventsList == null)
                    EventsList = new List<cfgEvent>();
                EventsList.Remove(itemForEdit);
                EventsList.Add(new cfgEvent() { EventTypes = Dlg3.EventTypes, eventLineList = Dlg3.eventLineList });
                EventsListView.ItemsSource = EventsList;
            }
        }

        private void Button_EditEvent_Click(object sender, RoutedEventArgs e)
        {
            if (EventsList == null)
                EventsList = new List<cfgEvent>();
            cfgEvent itemForDelete = (cfgEvent)EventsListView.SelectedItem;
            EventsList.Remove(itemForDelete);
            EventsListView.ItemsSource = EventsList;
        }
    }
}
