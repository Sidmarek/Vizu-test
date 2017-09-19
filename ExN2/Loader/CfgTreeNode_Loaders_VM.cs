using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace ExN2 {

    // config tree view: ViewModel of internal node = node
    public class CfgTreeNode_Loaders_VM : CfgTreeNode_VM {

        public List<CfgEventLoader> _LeafList = new List<CfgEventLoader>();

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
            if (Tmp.New(Parent))
            {
                AddLeaf(Tmp);
            }
        }

        public override void Edit(object selectedTreeItem, Window Parent)
        {
            if (selectedTreeItem is CfgEventLoader)
            {
                CfgEventLoader cfgEventLoader = (CfgEventLoader)selectedTreeItem;
                _LeafList.Remove(cfgEventLoader);
                var edited = cfgEventLoader.Edit(cfgEventLoader, Parent);
                _LeafList.Add(edited);
            }
        }

        public override void Delete(object selectedTreeItem, Window Parent)
        {
            if (selectedTreeItem is CfgEventLoader)
            {
                CfgEventLoader cfgEventLoader = (CfgEventLoader)selectedTreeItem;
                _LeafList.Remove(cfgEventLoader);
            }
        }
        
    };
}
