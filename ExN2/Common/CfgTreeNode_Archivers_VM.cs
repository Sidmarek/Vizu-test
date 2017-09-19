using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace ExN2 {

    // config tree view: ViewModel of internal node = node
    public class CfgTreeNode_Archivers_VM : CfgTreeNode_VM {
        //public event PropertyChangedEventHandler PropertyChanged;

        private String _NodeName;      // textual name of item
        private String _ImageFile;     // bitmap file name without path

        List<CfgTreeArchiver_VM> _LeafList = new List<CfgTreeArchiver_VM>();

        public CfgTreeNode_Archivers_VM() : base("archivers", "archiver.png") {
        }

        //.......................................................................................
        //public CfgTreeNode_Archivers_VM(String NodeName, String ImageFile) {
          //  _NodeName = NodeName;
            //_ImageFile = ImageFile;
        //}

        //.......................................................................................
        public void AddLeaf(CfgTreeArchiver_VM leaf) {
            _LeafList.Add(leaf);
        }
        
        //.......................................................................................
        public List<CfgTreeArchiver_VM> LeafList {
            get { return _LeafList; }
        }
        public override void New(Window Parent)
        {
           
        }
        public override void Edit(object selectedTreeItem, Window Parent)
        {
            
        }
        public override void Delete(object selectedTreeItem, Window Parent)
        {
            
        }
        //.......................................................................................
        //protected virtual void OnPropertyChanged(string propertyName) {
        //if (this.PropertyChanged != null)
        //    this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //}

    };
}
