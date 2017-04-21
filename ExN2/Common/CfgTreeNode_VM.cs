using System;
using System.ComponentModel;

namespace ExN2 {

    // config tree view: ViewModel of internal node = node
    public class CfgTreeNode_VM : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        private String _NodeName;      // textual name of item
        private String _ImageFile;     // bitmap file name without path

        CfgTreeLeaf_VM[] _LeafList;

        //.......................................................................................
        public CfgTreeNode_VM(String NodeName, String ImageFile, CfgTreeLeaf_VM[] LeafList) {
            _NodeName = NodeName;
            _ImageFile = ImageFile;
            _LeafList = LeafList;
        }

        //.......................................................................................
        public string NodeName {
            get { return _NodeName; }
            set { _NodeName = value;
                  OnPropertyChanged("NodeName");
            }
        }

        //.......................................................................................
        public string ImageUri {
            get {
                return "pack://application:,,,/Resources/" + _ImageFile;    // attribut "BuildAction" of the resource file must be "Resource"
            }
        }

        //.......................................................................................
        public CfgTreeLeaf_VM[] LeafList {
            get { return _LeafList; }
        }

        //.......................................................................................
        protected virtual void OnPropertyChanged(string propertyName) {
            if (this.PropertyChanged != null)
              this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    };
}
