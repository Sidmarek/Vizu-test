using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser;
using IniParser.Model;
using System.Net;

namespace ExN2 {

    [Flags]
    enum Priznaky : Byte {
        Prvni = 1,
        Druha = 2,
        Treti = 4
    }

    enum tEventItemType:int {
        itInt8,
        itInt16,
        itInt32,
        itVarChar,
        itIntConst,
        N_tEventItemType    // pocet hodnot typu
    };

    enum tN4T_version:int {
        n4t_undef,  // nedovolena hodnota
        n4t_ver001, // 0.01 - to jeste nemelo hlavicku, prvni byl rovnou iCommand
        n4t_ver100, // 1.00 - tohle posilalo v tele zpravy jako prvni polozku i cislo evt-bufferu
        n4t_ver200  // 2.00 - tohle uz neposila cislo evt-bufferu
    };

    class EventDef {
        static public int[] ItemTypeLen = new int[] { 1, 2, 4, 10, 0 };
    };

    //======= popis jedne polozky v sablone udalosti =======
    class cCfgEventItem {

        public String sName;      // item name = field name in SQL-table
        public tEventItemType Type;       // type definuje offset v datech a zaroven typ pole v SQL-tabulce
        public bool bStore;     // false = neukladat do DB, polozka pouze posouva offset v udalosti
        public int iLenBytes;  // delka dat v bajtech
        public int iConstValue;// pro pseudopole - konstantni hodnota
        public double rCoef;      // nasobitel hodnoty pred ulozenim do DB

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
    class cCfgEventTemplate {
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

    //-------------------------------------------------------------------
    //  parametry jednoho loaderu, krome sablon udalosti - ty jsou v poli
    //-------------------------------------------------------------------
    class CfgEventLoader {
        // pripojeni k databazi PostgreSQL
        public string DB_ConnectString;
        public string DB_TableName;
        public string DB_SysTableName;

        // parametry komunikace s PLC
        public IPEndPoint SocketLocal;
        public IPEndPoint SocketRemote;

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

        //...........................................................................
        // z INI souboru nacte konfiguraci jednoho loaderu do dodane struktury pCfg.
        // Jednotlivy loader je v INI identifikovan cislem (zero based).
        public bool LoadFromOldIni(String FullName) {

            Priznaky PP = 0;
            PP = Priznaky.Prvni;
            PP |= Priznaky.Druha;

            if (PP.HasFlag(Priznaky.Treti)) {

            }

            String S = PP.ToString();

            Byte B = (Byte)PP;



            FileIniDataParser Parser = new FileIniDataParser();
            IniData ini = Parser.ReadFile(FullName);

            sTaskName = ini["Main"]["TaskName"];

            DB_ConnectString = ini["Main"]["DB_ConnectString"];// retezec je navic v uvozovkach
            DB_TableName = ini["Main"]["DB_TableName"];
            DB_SysTableName = ini["Main"]["DB_SysTableName"];

            string[] Sock = ini["Main"]["UDPSocketLocal"].Split(':');
            if (Sock[0] == "") 
                Sock[0] = "127.0.0.1";
            SocketLocal = new IPEndPoint(System.Net.IPAddress.Parse(Sock[0]), Int32.Parse(Sock[1]));

            Sock = ini["Main"]["UDPSocketRemote"].Split(':');
            if (Sock[0] == "")
                Sock[0] = "127.0.0.1";
            SocketRemote = new IPEndPoint(System.Net.IPAddress.Parse(Sock[0]), Int32.Parse(Sock[1]));

            iRcvTimeoutMs = Int32.Parse(ini["Main"]["ReceiveTimeoutMs"]);

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
            return true;
        }
    }


        class Loader {
            //tCfgEventLoader Cfg;


            // class proc_Loader {
        }

}


