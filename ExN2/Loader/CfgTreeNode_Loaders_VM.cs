using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace ExN2 {

    // config tree view: ViewModel of internal node = node
    public class CfgTreeNode_Loaders_VM : CfgTreeNode_VM {

        List<CfgEventLoader> _LeafList = new List<CfgEventLoader>();

        //.......................................................................................
        public CfgTreeNode_Loaders_VM() : base("loaders", "ico_loader.png") {
        }

        //.......................................................................................
        public void AddLeaf(CfgEventLoader leaf) {
            _LeafList.Add(leaf);
            OnPropertyChanged("LeafList");
        }

        //.......................................................................................
        public List<CfgEventLoader> LeafList {
            get { return _LeafList; }
        }

        //.......................................................................................
        public override void New(Window Parent) {
            CfgEventLoader Tmp = new CfgEventLoader();
            if (Tmp.Edit(Parent)) {
                AddLeaf(Tmp);
            }
        }


    };
}
