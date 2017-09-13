using System;
using System.ComponentModel;

namespace ExN2 {
    
    // config tree view: ViewModel of ending node = leaf
    public class CfgTreeArchiver_VM : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        private String _LeafName;       // textual name of item

        //.......................................................................................
        public CfgTreeArchiver_VM() {
            _LeafName = "archiver 1";
        }

        //.......................................................................................
        public string LeafName {
            get { return _LeafName; }
            set { _LeafName = value;
                  OnPropertyChanged("LeafName");
            }
        }

        //.......................................................................................
        public string ImageUri {
            get {
                return "pack://application:,,,/resources/" + "archiver.png";
            }
        }

        //.......................................................................................
        protected virtual void OnPropertyChanged(string propertyName) {
            //PropertyChangedEventHandler handler = PropertyChanged;
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    };

}
