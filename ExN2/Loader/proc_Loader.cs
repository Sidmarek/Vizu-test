using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser;
using IniParser.Model;
using System.Net;
using System.Xml;
using System.ComponentModel;
using System.Windows;
using System.Xml.Serialization;
using System.IO;

namespace ExN2 {

    [Flags]
    public enum Priznaky : Byte {
        Prvni = 1,
        Druha = 2,
        Treti = 4
    }

    public enum tEventItemType :int {
        itInt8,
        itInt16,
        itInt32,
        itVarChar,
        itIntConst,
        N_tEventItemType    // pocet hodnot typu
    };

    public enum tN4T_version:int {
        n4t_undef = -1,  // nedovolena hodnota
        n4t_ver001, // 0.01 - to jeste nemelo hlavicku, prvni byl rovnou iCommand
        n4t_ver100, // 1.00 - tohle posilalo v tele zpravy jako prvni polozku i cislo evt-bufferu
        n4t_ver200  // 2.00 - tohle uz neposila cislo evt-bufferu
    };

    
    public class EventDef {
        static public int[] ItemTypeLen = new int[] { 1, 2, 4, 10, 0 }; 
    };

    //======= popis jedne polozky v sablone udalosti =======
    public class cCfgEventItem {

        public List<int> EventTypesList;
        public String sName;      // item name = field name in SQL-table
        public tEventItemType Type;       // type definuje offset v datech a zaroven typ pole v SQL-tabulce
        public bool bStore;     // false = neukladat do DB, polozka pouze posouva offset v udalosti
        public int iLenBytes;  // delka dat v bajtech
        public int iConstValue;// pro pseudopole - konstantni hodnota
        public double rCoef;      // nasobitel hodnoty pred ulozenim do DB
        public cCfgEventItem() {}
        public cCfgEventItem(String qsName, tEventItemType qType, bool qbNoStore, int qiConstValue, double qrCoef) {

            // pokud jmeno zacina "dummy", nebude se ukladat do DB
            bStore = !qbNoStore;
            sName = qsName;
            Type = qType;
            iConstValue = qiConstValue;
            rCoef = qrCoef;

            iLenBytes = EventDef.ItemTypeLen[(int)Type];
            if (Type == tEventItemType.itVarChar) {
                // pro string se delka predava jako parametr Value, byla v INI uvedena v modifikatoru "len="
                // deklarovana delka je cela bajtova delka vcetne hlavicky (2 B)
                iLenBytes = qiConstValue;
            }
        }
    }

    // ======= sablona cele udalosti, je to array polozek =======
    public class cCfgEventTemplate {
        public int iTaskIdx;       // pro chybove hlasky
        List<cCfgEventItem> Items;          // polozky

        public cCfgEventTemplate(int qiTaskIdx) {

            iTaskIdx = qiTaskIdx;
            Items = new List<cCfgEventItem>();
        }

        public void Add(String sName, tEventItemType aType, bool bNoStore, int iConstValue, double rCoef) {       // prida na konec
            Items.Add(new cCfgEventItem(sName, aType, bNoStore, iConstValue, rCoef));
        }
    }

    public class CfgTreeLoader_VM1 : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        private String _LeafName;       // textual name of item

        //.......................................................................................
        public CfgTreeLoader_VM1() {
            _LeafName = "Loader1";
        }

        //.......................................................................................
        public string LeafName {
            get { return _LeafName; }
            set {
                _LeafName = value;
                OnPropertyChanged("LeafName");
            }
        }

        //.......................................................................................
        public string ImageUri {
            get {
                return "pack://application:,,,/resources/" + "ico_loader.png";
            }
        }

        //.......................................................................................
        protected virtual void OnPropertyChanged(string propertyName) {
            //PropertyChangedEventHandler handler = PropertyChanged;
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Only data of all loaders configs 
    /// </summary>
    public class CfgLoaderConfig {
        public List<CfgEventLoader> CfgEventLoaderList;
    }
    //-------------------------------------------------------------------
    //  parametry jednoho loaderu, krome sablon udalosti - ty jsou v poli
    //-------------------------------------------------------------------
    public class CfgEventLoader : ProcCfgBase {
        public event PropertyChangedEventHandler PropertyChanged;
        private String _LeafName;       // textual name of item
        //enable 
        public bool bRun;

        // pripojeni k databazi PostgreSQL
        public string DB_ConnectString;
        public string DB_TableName;
        public string DB_SysTableName;

        // parametry komunikace s PLC
        public string SocketLocal;
        public string SocketRemote;

        // pro kontrolu definujem delku datoveho tela udalosti
        public int iEventBodyLenBytes;

        public int iRcvTimeoutMs;
        public bool bIntelOrder;
        public bool bIntelOrderStr;         // intel order definujeme zvlast pro stringy
        public tN4T_version N4T_version;
        public int iTypeFieldByteOffs;     // offset v bajtech, pole pro typ udalosti (pro vyber sablony)
        public int iRecnoFieldByteOffs;    // offset v bajtech, pole pro cislo udalosti (pro kontrolu preteceni)
        public string sTaskName;

        // false: normalne ukazuje LastPtr v PLC na posledni platny zaznam
        // true:  ve starsich programech ukazuje LastPtr v PLC na prvni volny zaznam
        bool bLastPtrIsFreePtr;

        // perioda a offset pro serizovani casu PLC
        int iAdjustTimePeriod_Sec;
        int iAdjustTimeOffset_Sec;

        public List<cCfgEventItem> EventItemList;

        //...........................................................................
        public CfgEventLoader() : base("Loader", "ico_loader.png") {
        }


        //...........................................................................
        public void SaveToXml(String FullName, CfgLoaderConfig CLC) {
            /*
            XmlDocument Doc = new XmlDocument();
            XmlDeclaration deklarace = Doc.CreateXmlDeclaration("1.0", "utf-8", null);
            Doc.AppendChild(deklarace);
            XmlElement koren = Doc.CreateElement("loaders");

            XmlElement elm = Doc.CreateElement("taskProps");
            elm.SetAttribute("run", "Yes");
            elm.SetAttribute("taskName", "Dubravica");
            koren.AppendChild(elm);

            Doc.AppendChild(koren);
            Doc.Save(FullName);
            */
            
            XmlSerializer xsSubmit = new XmlSerializer(typeof(CfgLoaderConfig));            
            string xml = "";

            using (var sww = new StreamWriter(FullName))
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer,CLC);
                }
            }
        }

        //...........................................................................
        // z INI souboru nacte konfiguraci jednoho loaderu do dodane struktury pCfg.
        // Jednotlivy loader je v INI identifikovan cislem (zero based).
        public CfgLoaderConfig LoadFromOldIni(String FullName) {
            #region test
            /*
            Priznaky PP = 0;
            PP = Priznaky.Prvni;
            PP |= Priznaky.Druha;

            if (PP.HasFlag(Priznaky.Treti)) {

            }

            String S = PP.ToString();

            Byte B = (Byte)PP;
            */
            #endregion
            List<string> lines =  System.IO.File.ReadAllLines(FullName).Select(p => p.Trim()).ToList();
            CfgLoaderConfig CLC = new CfgLoaderConfig();
            CLC.CfgEventLoaderList = new List<CfgEventLoader>();
            bool bAfterEventStart = false;
            cCfgEventItem eventItemInstance = null;
            N4T_version = tN4T_version.n4t_undef;
            string[] partsVal = null;
            string[] partsLen = null;
            string[] address = null;
            int[] timeAdjust = null;

            foreach (string line in lines)
            {
                if (line != "") {
                    if (line.Contains("["))
                    {
                        CfgEventLoader cfgEventLoader = new CfgEventLoader();

                        cfgEventLoader.bRun = bRun;
                        bRun = false;
                        cfgEventLoader.sTaskName = sTaskName;
                        sTaskName = null;
                        cfgEventLoader.DB_ConnectString = DB_ConnectString;
                        DB_ConnectString = null;
                        cfgEventLoader.DB_TableName = DB_TableName;
                        DB_TableName = null;
                        cfgEventLoader.DB_SysTableName = DB_SysTableName;
                        DB_SysTableName = null;
                        cfgEventLoader.SocketLocal = SocketLocal;
                        SocketLocal = null;
                        cfgEventLoader.SocketRemote = SocketRemote;
                        SocketRemote = null;
                        cfgEventLoader.iRcvTimeoutMs = iRcvTimeoutMs;
                        iRcvTimeoutMs = 0;
                        cfgEventLoader.bIntelOrder = bIntelOrder;
                        bIntelOrder = false;
                        cfgEventLoader.N4T_version = N4T_version;
                        N4T_version = tN4T_version.n4t_undef;
                        cfgEventLoader.bLastPtrIsFreePtr = bLastPtrIsFreePtr;
                        bLastPtrIsFreePtr = false;
                        cfgEventLoader.iEventBodyLenBytes = iEventBodyLenBytes;
                        iEventBodyLenBytes = 0;
                        cfgEventLoader.iTypeFieldByteOffs = iTypeFieldByteOffs;
                        iTypeFieldByteOffs = 0;
                        cfgEventLoader.EventItemList = EventItemList;
                        EventItemList = null;
                        cfgEventLoader.iAdjustTimePeriod_Sec = iAdjustTimePeriod_Sec;
                        iAdjustTimePeriod_Sec = 0;
                        cfgEventLoader.iAdjustTimeOffset_Sec = iAdjustTimeOffset_Sec;
                        iAdjustTimeOffset_Sec = 0;


                        string name = line.Replace("[", "");
                        name = name.Replace("]", "");
                        if (name != "Common")
                        {
                            _LeafName = name;
                            if(_LeafName != null)
                                CLC.CfgEventLoaderList.Add(cfgEventLoader);
                            cfgEventLoader._LeafName = _LeafName;                            
                        }
                    }
                    else if (line.Contains("=") && bAfterEventStart == false)
                    {
                        string[] splitedLine = line.Split('=');
                        string keyword = splitedLine[0];

                        if(keyword.Contains("Run"))
                            bRun = splitedLine[1].Equals("Yes");
                            
                        if(keyword.Contains("TaskName"))
                            sTaskName = splitedLine[1];
                            
                        if(keyword.Contains("ConnectString"))
                            DB_ConnectString = line.Substring(line.IndexOf("\"")+1, (line.Length - line.IndexOf("\"")-2));
                            
                        if(keyword.Contains("DB_TableName"))
                            DB_TableName = splitedLine[1];
                            
                        if(keyword.Contains("SysTableName"))
                            DB_SysTableName = splitedLine[1];

                        if (keyword.Contains("SocketLocal"))
                        {
                            address = splitedLine[1].Split(':');
                            if (address[0] == "")
                                address[0] = "127.0.0.1";
                            SocketLocal = address[0] + address[1];
                            //SocketLocal = new IPEndPoint(IPAddress.Parse(address[0]), int.Parse(address[1]));
                        }
                        if (keyword.Contains("SocketRemote"))
                        {
                            address = splitedLine[1].Split(':');
                            if (address[0] == "")
                                address[0] = "127.0.0.1";
                            SocketRemote = address[0] + address[1];
                            //SocketRemote = new IPEndPoint(IPAddress.Parse(address[0]), int.Parse(address[1]));
                        }
                        if(keyword.Contains("ReceiveTimeoutMs"))
                            iRcvTimeoutMs = int.Parse(splitedLine[1]);
                            
                        if(keyword.Contains("IntelOrder"))
                            bIntelOrder = splitedLine[1].Equals("1");
                            
                        if(keyword.Contains("N4T_version"))
                            N4T_version = (tN4T_version) (int)double.Parse(splitedLine[1]);
                            
                        if(keyword.Contains("LastPtrIsFreePtr"))
                            bLastPtrIsFreePtr = splitedLine[1].Equals("1");
                            
                        if(keyword.Contains("EventBodyLenBytes"))                                
                            iEventBodyLenBytes = int.Parse(splitedLine[1]);
                            
                        if(keyword.Contains("TypeFieldByteOffs"))
                            iTypeFieldByteOffs = int.Parse(splitedLine[1]);
                            
                        if(keyword.Contains("RecnoFieldByteOffs"))
                            iRecnoFieldByteOffs = int.Parse(splitedLine[1]);

                        if (keyword.Contains("Event_begin"))
                        {
                            if (EventItemList == null)
                                EventItemList = new List<cCfgEventItem>();
                            eventItemInstance = new cCfgEventItem();
                            if (eventItemInstance.EventTypesList == null)
                                eventItemInstance.EventTypesList = new List<int>();
                            eventItemInstance.EventTypesList = splitedLine[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p.Trim())).ToList(); // that wil trim the every number and give it into the list (EventTypesList)
                            bAfterEventStart = true;
                        }
                        if (keyword.Contains("AdjustPlcTime_Sec"))
                        {
                            timeAdjust = splitedLine[1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p.Trim())).ToArray();
                            iAdjustTimePeriod_Sec = timeAdjust[0];
                            iAdjustTimeOffset_Sec = timeAdjust[1];
                        }
                    }
                        else
                    {
                        if (!line.Contains("Event_end"))
                        {
                            List<string> splittedEventLine = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToList();
                            if (eventItemInstance == null)
                                eventItemInstance = new cCfgEventItem();
                            foreach (string eventAttribute in splittedEventLine)
                            {
                                //coef i dont know how it should be look like
                                if(eventAttribute.Contains("int8"))
                                    eventItemInstance.Type = tEventItemType.itInt8;
                                    eventItemInstance.iLenBytes = EventDef.ItemTypeLen[(int)eventItemInstance.Type];

                                if(eventAttribute.Contains("int16"))
                                    eventItemInstance.Type = tEventItemType.itInt8;
                                    eventItemInstance.iLenBytes = EventDef.ItemTypeLen[(int)eventItemInstance.Type];
                                    
                                if(eventAttribute.Contains("int32"))
                                    eventItemInstance.Type = tEventItemType.itInt32;
                                    eventItemInstance.iLenBytes = EventDef.ItemTypeLen[(int)eventItemInstance.Type];
                                    
                                if(eventAttribute.Contains("s7str"))
                                    eventItemInstance.Type = tEventItemType.itVarChar;
                                    eventItemInstance.iLenBytes = EventDef.ItemTypeLen[(int)eventItemInstance.Type];
                                    
                                if(eventAttribute.Contains("const"))
                                    eventItemInstance.Type = tEventItemType.itIntConst;
                                    eventItemInstance.iLenBytes = EventDef.ItemTypeLen[(int)eventItemInstance.Type];
                                    
                                if(eventAttribute.Contains("noStore"))
                                    eventItemInstance.bStore = false;

                                if (eventAttribute.Contains("len="))
                                {
                                    partsLen = eventAttribute.Split('=');
                                    eventItemInstance.iConstValue = int.Parse(partsLen[1]);
                                }
                                if (eventAttribute.Contains("value="))
                                {
                                    partsVal = eventAttribute.Split('=');
                                    eventItemInstance.iConstValue = int.Parse(partsVal[1]);
                                }
                                if(eventAttribute.Contains("int8") || eventAttribute.Contains("int16") || eventAttribute.Contains("int32") || eventAttribute.Contains("s7str") || eventAttribute.Contains("const") || eventAttribute.Contains("noStore") || eventAttribute.Contains("value="))
                                    eventItemInstance.sName = eventAttribute;
                                    
                            }
                            EventItemList.Add(eventItemInstance);
                        }
                        else
                        {
                            bAfterEventStart = false;
                        }                       
                    }
                }
            }
            CfgEventLoader lastEventLoader = new CfgEventLoader();

            lastEventLoader.bRun = bRun;
            lastEventLoader.sTaskName = sTaskName;
            lastEventLoader.DB_ConnectString = DB_ConnectString;
            lastEventLoader.DB_TableName = DB_TableName;
            lastEventLoader.DB_SysTableName = DB_SysTableName;
            lastEventLoader.SocketLocal = SocketLocal;
            lastEventLoader.SocketRemote = SocketRemote;
            lastEventLoader.iRcvTimeoutMs = iRcvTimeoutMs;
            lastEventLoader.bIntelOrder = bIntelOrder;
            lastEventLoader.N4T_version = N4T_version;
            lastEventLoader.bLastPtrIsFreePtr = bLastPtrIsFreePtr;
            lastEventLoader.iEventBodyLenBytes = iEventBodyLenBytes;
            lastEventLoader.iTypeFieldByteOffs = iTypeFieldByteOffs;
            lastEventLoader.EventItemList = EventItemList;
            lastEventLoader.iAdjustTimePeriod_Sec = iAdjustTimePeriod_Sec;
            lastEventLoader.iAdjustTimeOffset_Sec = iAdjustTimeOffset_Sec;
            
            CLC.CfgEventLoaderList.Add(lastEventLoader);
            #region old
            //IniData ini = Parser.ReadFile(FullName);
            /*
            sTaskName = ini["Main"]["TaskName"];

            DB_ConnectString = ini["Main"]["DB_ConnectString"];// retezec je navic v uvozovkach
            DB_TableName = ini["Main"]["DB_TableName"];
            DB_SysTableName = ini["Main"]["DB_SysTableName"];

            string[] Sock = ini["Main"]["UDPSocketLocal"].Split(':');
            if (Sock[0] == "") 
                Sock[0] = "127.0.0.1";
            SocketLocal = new IPEndPoint(IPAddress.Parse(Sock[0]), Int32.Parse(Sock[1]));

            Sock = ini["Main"]["UDPSocketRemote"].Split(':');
            if (Sock[0] == "")
                Sock[0] = "127.0.0.1";
            SocketRemote = new IPEndPoint(IPAddress.Parse(Sock[0]), Int32.Parse(Sock[1]));

            iRcvTimeoutMs = Int32.Parse(ini["Main"]["ReceiveTimeoutMs"]);
            */
            /*
                else if (stricmp(sKW, "IntelOrder") == 0) {
                    Struc.bIntelOrder = GetAndDelFirstInt(sLine) != 0;
                    Struc.bIntelOrderStr = Struc.bIntelOrder;  // default hodnota pro string je identicka s hlavni volbou, dale ale muze byt prebita
                }
                else if (stricmp(sKW, "IntelOrderStr") == 0)
                    Struc.bIntelOrderStr = GetAndDelFirstInt(sLine) != 0;

                else if (stricmp(sKW, "N4T_version") == 0) {
                    Struc.N4T_version = n4t_undef;
                    iTmp = GetAndDelFirstInt(sLine, '.');
                    if (iTmp == 0) {                        // hlavni verze je 0
                        iTmp = GetAndDelFirstInt(sLine);    // docteme podverzi
                        if (iTmp == 1)
                            Struc.N4T_version = n4t_ver001;
                    }
                    else if (iTmp == 1)
                        Struc.N4T_version = n4t_ver100;
                    else if (iTmp == 2)
                        Struc.N4T_version = n4t_ver200;
                    if (Struc.N4T_version == n4t_undef) {
                        err(iTaskIndex, "N4T-protocol version not defined");
                        return false;
                    }
                }

                else if (stricmp(sKW, "EventBodyLenBytes") == 0)
                    Struc.iEventBodyLenBytes = GetAndDelFirstInt(sLine);

                else if (stricmp(sKW, "TypeFieldByteOffs") == 0)
                    Struc.iTypeFieldByteOffs = GetAndDelFirstInt(sLine);

                else if (stricmp(sKW, "RecnoFieldByteOffs") == 0)
                    Struc.iRecnoFieldByteOffs = GetAndDelFirstInt(sLine);

                else if (stricmp(sKW, "LastPtrIsFreePtr") == 0)
                    Struc.bLastPtrIsFreePtr = GetAndDelFirstInt(sLine) != 0;

                else if (stricmp(sKW, "Event_begin") == 0) {
                    // nejdriv ulozit cisla udalosti do mapy, cisla jsou na zbytku radku oddelena carkami
                    int iTargetIdx = Templates.GetSize();   // na tento index bude pozdeji ulozena sablona udalosti
                    while (!Str_Empty(sLine)) {
                        int iEventNo = GetAndDelFirstInt(sLine, ',');
                        TemplateMap[iEventNo] = iTargetIdx;
                    }
                    // vytvorit novou sablonu a ulozit do pole sablon ukazatel na ni
                    cCfgEventTemplate* tmp = new cCfgEventTemplate(iTaskIndex);
                    Templates.Add(tmp);

                    // docist definici a doplnit do prave vytorene sablony
                    DoctiTemplate(&pLinePtr, tmp);  // predavame ukazatel na cteci ukazatel, aby byl po navratu spravne posunuty
                }

                else if (stricmp(sKW, "AdjustPlcTime_Sec") == 0) {
                    Struc.iAdjustTimePeriod_Sec = GetAndDelFirstInt(sLine);
                    Struc.iAdjustTimeOffset_Sec = GetAndDelFirstInt(sLine);
                }
            }*/
            #endregion           
            return CLC;
        }

        //...........................................................................
        public bool Edit(Window Parent) {   // vraci true, pokud bylo stisknuto OK
            Dlg_LoaderProps Dlg = new Dlg_LoaderProps();
            Dlg.Owner = Parent;
            return (bool)Dlg.ShowDialog();
        }

    }


    class Loader {
            //tCfgEventLoader Cfg;


            // class proc_Loader {
        }

}


