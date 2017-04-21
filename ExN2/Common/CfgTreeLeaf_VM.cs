using System;
using System.ComponentModel;

namespace ExN2 {
    
    // config tree view: ViewModel of ending node = leaf
    public class CfgTreeLeaf_VM : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        private String _LeafName;       // textual name of item
        private String _ImageFile;      // bitmap file name without path

        //.......................................................................................
        public CfgTreeLeaf_VM(String LeafName, String ImageFile) {
            _LeafName = LeafName;
            _ImageFile = ImageFile;
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
                return "pack://application:,,,/resources/" + _ImageFile;
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
