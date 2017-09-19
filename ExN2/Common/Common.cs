using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows;

namespace ExN2 {
    //-------------------------------------------------------------------
    // veci spolecne pro cely program
    //-------------------------------------------------------------------
    public class Common {
        public static string cfgFilesPath   = @"c:\Akce\C#\Vizu\ExN2\config\";
        public static string bmpFilesPath   = @"c:\Akce\C#\Vizu\ExN2\bmp\";
        public static string version        = "1.00";

        //...............................................................................
        public static void SysErr(string Msg) {
            // DODELAT
            System.Windows.MessageBox.Show(Msg, "Internal error");
        }

        //...............................................................................
        public static Bitmap CreateBitmapFromResource(String Name) {
            Assembly _assembly;
            Stream _imageStream;
            Bitmap Bmp;
            try {
                _assembly = Assembly.GetExecutingAssembly();
                _imageStream = _assembly.GetManifestResourceStream("Resources." + Name);
            }
            catch {
                Common.SysErr("CreateBitmapFromResource / Resource access error: " + Name);
                return null;
            };

            try {
                Bmp = new Bitmap(_imageStream);
            }
            catch {
                Common.SysErr("CreateBitmapFromResource / Resource access error: " + Name);
                return null;
            }

            return Bmp;
        }

      /*  //...............................................................................
        public static bool IsItLoader(Object ob) {
            if (ob == null)
                return false;
            return (ob.GetType() == typeof(CfgTreeNode_Loaders_VM)) ||
                   (ob.GetType() == typeof(CfgTreeLoader_VM));
        }

        //...............................................................................
        public static bool IsItArchiver(Object ob) {
            if (ob == null)
                return false;
            return (ob.GetType() == typeof(CfgTreeNode_Archivers_VM)) ||
                   (ob.GetType() == typeof(CfgTreeArchiver_VM));
        }*/

    }

    //-----------------------------------------------------------------------------------
    // bazova trida pro udrzovani konfigurace jednoho procesu = list konfiguracniho stromu
    //-----------------------------------------------------------------------------------
    public class ProcCfgBase : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _sLeafName;       // textual name of item
        private string _sIconFile;       // resource

        //.......................................................................................
        public ProcCfgBase(string qsLeafName, string qsIconFile) {
            _sLeafName = qsLeafName;
            _sIconFile = qsIconFile;
        }

        //.......................................................................................
        public string LeafName{
            get { return _sLeafName; }
            set {
                _sLeafName = value;
                OnPropertyChanged("LeafName");
            }
        }

        //.......................................................................................
        public string ImageUri {
            get {
                return "pack://application:,,,/resources/" + _sIconFile;
            }
        }

        //.......................................................................................
        protected virtual void OnPropertyChanged(string propertyName) {
            //PropertyChangedEventHandler handler = PropertyChanged;
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    //-------------------------------------------------------------------
    // bazova trida pro uzel konf. stromu (napr.vsechny loadery)
    //-------------------------------------------------------------------
    public abstract class CfgTreeNode_VM : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        private String _NodeName;      // textual name of item
        private String _ImageFile;     // bitmap file name without path

        //.......................................................................................
        public CfgTreeNode_VM(string qsNodeName, string qsIconFile) {
            _NodeName = qsNodeName;
            _ImageFile = qsIconFile;
        }

        //.......................................................................................
        public string NodeName {
            get { return _NodeName; }
            set {
                _NodeName = value;
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
        protected virtual void OnPropertyChanged(string propertyName) {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        //.......................................................................................
        //public abstract void New(Window Parent);
        public abstract void New(Window Parent);
        public abstract void Edit(object selectedTreeItem, Window Parent);
        public abstract void Delete (object selectedTreeItem, Window Parent);
    };
}
