using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ExN2 {
    class Common {
        public static string cfgFilesPath = @"c:\Akce\C#\Vizu\ExN2\config\";
        public static string bmpFilesPath = @"c:\Akce\C#\Vizu\ExN2\bmp\";

        public static string version = "1.00";

        public static void SysErr(string Msg) {
            // DODELAT
            MessageBox.Show(Msg, "Internal error");
        }

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
    }
}
