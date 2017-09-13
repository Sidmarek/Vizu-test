﻿using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ExN2 {

    // config tree view: ViewModel of internal node = node
    public class CfgTreeNode_Archivers_VM : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        private String _NodeName;      // textual name of item
        private String _ImageFile;     // bitmap file name without path

        List<CfgTreeArchiver_VM> _LeafList = new List<CfgTreeArchiver_VM>();

        //.......................................................................................
        public CfgTreeNode_Archivers_VM(String NodeName, String ImageFile) {
            _NodeName = NodeName;
            _ImageFile = ImageFile;
        }

        //.......................................................................................
        public void AddLeaf(CfgTreeArchiver_VM leaf) {
            _LeafList.Add(leaf);
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
        public List<CfgTreeArchiver_VM> LeafList {
            get { return _LeafList; }
        }

        //.......................................................................................
        protected virtual void OnPropertyChanged(string propertyName) {
            if (this.PropertyChanged != null)
              this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    };
}