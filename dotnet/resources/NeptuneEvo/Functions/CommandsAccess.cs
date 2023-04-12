using Database;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using LinqToDB;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using NeptuneEvo.Core;
using NeptuneEvo.Database.Models;
using NeptuneEvo.Fractions;

namespace NeptuneEvo.Functions
{
    public enum AdminLevel {
        None = 0,
        Admin1,
        Admin2,
        Admin3,
        Admin4,
        Admin5,
        Admin6,
        Admin7,
        Main,
        Director,
    }
    public static class AdminCommands
    {
        public const string Id = "id";
        public const string Admins = "admins";
        public const string A = "a";
        public const string Checkkill = "checkkill";
        public const string Sp = "sp";
        public const string Inv = "inv";
        public const string Agm = "agm";
        public const string Tpc = "tpc";
        public const string Kl = "kl";
        public const string Checkdim = "checkdim";
        public const string Setdim = "setdim";
        public const string Fz = "fz";
        public const string UnFz = "unfz";
        public const string Mute = "mute";
        public const string Offmute = "offmute";
        public const string Offunmute = "offunmute"; //+
        public const string Unmute = "unmute";
        public const string Flip = "flip";
        public const string Stats = "stats";
        public const string Offstats = "offstats";
        public const string Mypos = "mypos";
        public const string Nhistory = "nhistory";
        public const string Checkwanted = "checkwanted";
        public const string Hp = "hp"; //+
        public const string Asms = "asms";
        public const string Redname = "redname";
        public const string Slap = "slap";
        public const string TakeMask = "takemask";
        public const string Tpcarnumber = "tpcarnumber";

        public const string Kick = "kick";
        public const string Jail = "jail"; //+
        public const string Unjail = "unjail";
        public const string Offjail = "offjail";
        public const string Offunjail = "offunjail"; //+
        public const string Carnumber = "carnumber"; //+
        public const string Givecarnumber = "givecarnumber"; //+
        public const string Check = "check"; //+
        public const string Spveh = "spveh"; //+
        public const string Spvehid = "spvehid"; //+
        public const string Afuel = "afuel";
        public const string Fixcar = "fixcar";
        public const string Fixcarid = "fixcarid";//=+++++++++++++++++++++++

        public const string Ban = "ban";
        public const string Warn = "warn";
        public const string Checkmoney = "checkmoney";
        public const string Sethp = "sethp"; //+
        public const string Delad = "delad";
        public const string Tp = "tp";
        public const string Spawn = "spawn";
        public const string Metp = "metp"; //+
        public const string Metpcar = "metpcar"; //+
        public const string Setdimcar = "setdimcar"; //+
        public const string Tpcar = "tpcar"; //+
        public const string Gm = "gm";
        public const string Kill = "kill";
        public const string Createmp = "createmp";
        public const string Startmp = "startmp";
        public const string Stopmp = "stopmp";
        public const string Mpveh = "mpveh";
        public const string Mpreward = "mpreward";
        public const string Mpkick = "mpkick";
        public const string Banmp = "banmp";
        public const string Unbanmp = "unbanmp";
        public const string Mphp = "mphp";
        //public const string Mpar = "mpar";
        public const string Mpplayers = "mpplayers";
        public const string Mpo = "mpo"; //+
        public const string Skin = "skin"; //+
        public const string Clear = "clear";
        public const string mpskin = "mpskin";
        public const string mpskins = "mpskins";
        public const string mppos = "mppos";

        public const string Offwarn = "offwarn";
        public const string Hardban = "hardban";
        public const string PritonBan = "pritonban";
        public const string Offhardban = "offhardban";
        public const string Offban = "offban";
        public const string Global = "global"; //+
        public const string Veh = "veh";
        public const string Delveh = "delveh"; //+
        public const string Delvehid = "delvehid"; //+
        public const string Delvehall = "delvehall"; //+
        public const string Delmyveh = "delmyveh"; //+
        
        public const string Sendcreator = "sendcreator";
        public const string Delobj = "delobj"; //+
        public const string Revive = "revive";
        public const string Aclear = "aclear";
        public const string Agl = "agl";
        public const string Unbanip = "unbanip";
        public const string SkipQuest = "skipquest";

        public const string Unwarn = "unwarn";
        public const string Offunwarn = "offunwarn";
        public const string Unban = "unban";
        public const string Unhardban = "unhardban";
        public const string Setleader = "setleader";
        public const string Delleader = "delleader";
        public const string Deljob = "deljob";
        public const string Delfrac = "delfrac";
        public const string Offdelfrac = "offdelfrac";
        public const string Skick = "skick";
        public const string Sban = "sban";
        public const string Setar = "setar"; //+
        public const string Givegun = "givegun"; //+
        public const string Delgun = "delgun"; //+
        public const string Giveammo = "giveammo";
        public const string Setname = "setname"; //+
        public const string Setnameoff = "setnameoff"; //+
        public const string Setbizmafia = "setbizmafia";
        public const string Setcolour = "setcolour";
        public const string Goadditem = "goadditem";
        public const string Nalog = "nalog";
        public const string Acancel = "acancel";

        public const string Setskin = "setskin";
        public const string Elections = "elections"; //+
        public const string Sc = "sc";
        public const string Sac = "sac";
        public const string Pa = "pa";
        public const string Sa = "sa";
        public const string Szstate = "szstate";
        public const string Ishard = "ishard";
        public const string Promosync = "promosync";
        public const string Bonussync = "bonussync";//++++++++++++++++++++
        public const string Alog = "alog";
        public const string Accept = "accept";


        public const string Restart = "restart"; //+
        public const string Save = "save";
        public const string Deladmin = "deladmin";
        public const string Offdeladmin = "offdeladmin";
        public const string Arank = "arank"; //+
        public const string Offarank = "offarank"; //+
        public const string Givevip = "givevip"; //+
        public const string Giveexp = "giveexp";
        public const string Givelvl = "givelvl";
        public const string Payday = "payday";
        public const string Offgivevip = "offgivevip"; //+
        public const string Additem = "additem"; //+
        public const string Carcoupon = "carcoupon"; //+
        public const string Spvehs = "spvehs"; //+
        public const string SaveServer = "saveserver"; //+
        


        //Хз что

        public const string Att = "att";
        public const string Givereds = "givereds";
        public const string Giveredsall = "giveredsall";
        public const string Givecase = "givecase";
        public const string Setadmin = "setadmin";
        public const string Givemoney = "givemoney";
        public const string Offgivemoney = "offgivemoney";
        public const string Giveclothes = "giveclothes";
        public const string Hidenick = "hidenick";
        public const string Hideme = "hideme";
        public const string Newsimcard = "newsimcard";
        public const string Takeoffbiz = "takeoffbiz";
        public const string Fsetcmd = "fsetcmd";
        public const string Medialist = "medialist";
        public const string Vlist = "vlist";
        public const string Setprod = "setprod";
        public const string Createbusiness = "createbusiness";
        public const string Createunloadpoint = "createunloadpoint";
        public const string Changebiztax = "changebiztax";
        public const string Deletebusiness = "deletebusiness";
        public const string Setvehcord = "setvehcord";


        public const string Setproductbyindex = "setproductbyindex";
        public const string Deleteproducts = "deleteproducts";
        public const string Changebizprice = "changebizprice";
        public const string Changehouseprice = "changehouseprice";
        public const string Changestock = "changestock";
        public const string Vehchange = "vehchange";
        public const string Startmatwars = "startmatwars";
        public const string Stopmatwars = "stopmatwars";
        public const string Housetypeprice = "housetypeprice";
        public const string Delhouseowner = "delhouseowner";
        public const string Stt = "stt";
        public const string Dmgmodif = "dmgmodif";
        public const string Svm = "svm";
        public const string Svn = "svn";
        public const string Svh = "svh";
        public const string Setfractun = "setfractun";
        public const string Setfracveh = "setfracveh";
        public const string Vehs = "vehs";
        public const string Fclear = "fclear";
        public const string Vehcustom = "vehcustom";
        public const string Vehcustompcolor = "vehcustompcolor";
        public const string Vehcustomscolor = "vehcustomscolor";
        public const string Sl = "sl";
        public const string Sw = "sw";
        public const string St = "st";
        public const string loadipl = "loadipl";
        public const string unloadipl = "unloadipl";
        public const string loadprop = "loadprop";
        public const string unloadprop = "unloadprop";
        public const string starteffect = "starteffect";
        public const string stopeffect = "stopeffect";
        public const string offgivereds = "offgivereds";
        public const string ptime = "ptime";
        //public const string muted = "muted";
        public const string createsafe = "createsafe";
        public const string removesafe = "removesafe";
        public const string setvehdirt = "setvehdirt";
        public const string givehc = "givehc";
        public const string givehcrad = "givehcrad";
        public const string tr_ev_start = "tr_ev_start";
        public const string setgarage = "setgarage";
        public const string creategarage = "creategarage";
        public const string removegarage = "removegarage";
        public const string createhouse = "createhouse";
        public const string tphouse = "tphouse";
        public const string tpbiz = "tpbiz";
        public const string setparkplace = "setparkplace";
        public const string removehouse = "removehouse";
        public const string housechange = "housechange";
        public const string setbliporg = "setbliporg";
        public const string delbliporg = "delbliporg";
        public const string setmicrophone = "setmicrophone";
        public const string setfamily = "setfamily";
        public const string Crimeban = "crimeban";
        public const string Offcrimeban = "offcrimeban";
        public const string Uncrimeban = "uncrimeban";
        public const string Offuncrimeban = "offuncrimeban";


        //То что доступно по нику
        public const string Banlogin = "banlogin";
        public const string Getlogin = "getlogin";
        public const string GetRb = "getrb";
        public const string GetVip = "getvip";
        public const string Unbanlogin = "unbanlogin";
        public const string Connecttype = "connecttype";
        public const string Fixmerger = "fixmerger";
        public const string MoneyMultiplier = "MoneyMultiplier";
        public const string Expmultiplier = "expmultiplier";
        public const string Ecosync = "ecosync";
        public const string Bansync = "bansync";
        public const string Updateadminaccess = "updateadminaccess";
        public const string Defaultadminaccess = "defaultadminaccess";

        public const string Tsc = "tsc";

        public const string Enablefunc = "enablefunc";
        public const string Refresh = "refresh";
        public const string Drone = "drone";
        public const string RefresEverydayAward = "refreseverydayaward";
        public const string Refreshsystemstate = "refreshsystemstate";

        public const string UpdateCDN = "updatecdn";
        
        public const string DelObjects = "delobjects";
        
        public const string ReloadResources = "reloadresources";


        public const string StartCam = "startcam";
    }
    
    public static class VipCommands
    {
        public const string Cam = "cam"; 
        public const string Camtime = "camtime"; //+
    }

    class CommandsAccess : Script
    {
        private static readonly nLog Log = new nLog("Functions.CommandsAccess");

        public static string AdminPrefix = "!{#DF5353}Администратор ";
        public static string AdminPrefixChat = "~r~Администратор ";

        private static string DefaultAdminAccess = JsonConvert.SerializeObject(new Dictionary<string, sbyte>()
        {
            { AdminCommands.Id, 1 },
            { AdminCommands.Admins, 1 },
            { AdminCommands.A, 1 },
            { AdminCommands.Sp, 1 },
            { AdminCommands.Inv, 1 },
            { AdminCommands.Agm, 1 },
            { AdminCommands.Tpc, 1 },
            { AdminCommands.Kl, 1 },
            { AdminCommands.Checkdim, 1 },
            { AdminCommands.Setdim, 1 },
            { AdminCommands.Fz, 1 },
            { AdminCommands.UnFz, 1 },
            { AdminCommands.Mute, 1 },
            { AdminCommands.Offmute, 1 },
            { AdminCommands.Offunmute, 1 },
            { AdminCommands.Unmute, 1 },
            { AdminCommands.Flip, 1 },
            { AdminCommands.Stats, 1 },
            { AdminCommands.Offstats, 1 },
            { AdminCommands.Mypos, 1 },
            { AdminCommands.Nhistory, 1 },
            { AdminCommands.Checkwanted, 1 },
            { AdminCommands.Hp, 1 },
            { AdminCommands.Asms, 1 },
            { AdminCommands.Redname, 1 },
            { AdminCommands.Slap, 1 },
            { AdminCommands.ptime, 1},
            { AdminCommands.TakeMask, 1},
            { AdminCommands.Tpcarnumber, 1 },

            { AdminCommands.Spawn, 2 },
            { AdminCommands.Checkkill, 2 },
            { AdminCommands.Kick, 2 },
            { AdminCommands.Jail, 2 },
            { AdminCommands.Unjail, 2 },
            { AdminCommands.Offjail, 2 },
            { AdminCommands.Offunjail, 2 },
            { AdminCommands.Carnumber, 2 },
            { AdminCommands.Check, 2 },
            { AdminCommands.Fixcar, 2 },
            { AdminCommands.Fixcarid, 2 },
            { AdminCommands.Spveh, 2 },
            { AdminCommands.Spvehid, 2 },
            { AdminCommands.Tp, 2 },

            { AdminCommands.Ban, 3 },
            { AdminCommands.Warn, 3 },
            { AdminCommands.Checkmoney, 3 },
            { AdminCommands.Sethp, 3 },
            { AdminCommands.Delad, 3 },
            { AdminCommands.Metp, 3 },
            { AdminCommands.Metpcar, 3 },
            { AdminCommands.Setdimcar, 1 },
            { AdminCommands.Tpcar, 3 },
            { AdminCommands.Gm, 3 },
            { AdminCommands.Kill, 3 },
            { AdminCommands.Createmp, 3 },
            { AdminCommands.Startmp, 3 },
            { AdminCommands.Stopmp, 3 },
            { AdminCommands.Mpveh, 3 },
            { AdminCommands.Mpkick, 3 },
            { AdminCommands.Banmp, 3 },
            { AdminCommands.Unbanmp, 3 },
            { AdminCommands.Mphp, 3 },
            //{ AdminCommands.Mpar, 3 },
            { AdminCommands.Mpplayers, 3 },
            { AdminCommands.Mpo, 3 },
            { AdminCommands.tphouse, 3 },
            { AdminCommands.tpbiz, 3 },
            { AdminCommands.Clear, 3 },
            { AdminCommands.mpskin, 3 }, 
            { AdminCommands.mpskins, 3 },
            { AdminCommands.mppos, 3 },

            { AdminCommands.Offwarn, 4 },
            { AdminCommands.Hardban, 4 },
            { AdminCommands.Offhardban, 4 },
            { AdminCommands.Offban, 4 },
            { AdminCommands.Global, 4 },
            { AdminCommands.Veh, 4 },
            { AdminCommands.Delveh, 4 },
            { AdminCommands.Delvehid, 4 },
            { AdminCommands.Delvehall, 4 },
            { AdminCommands.Delmyveh, 4 },
            { AdminCommands.Sendcreator, 4 },
            { AdminCommands.Revive, 4 },
            { AdminCommands.Delobj, 4 },

            { AdminCommands.Unwarn, 5 },
            { AdminCommands.Offunwarn, 5 },
            { AdminCommands.Setleader, 5 },
            { AdminCommands.Delleader, 5 },
            { AdminCommands.Deljob, 5 },
            { AdminCommands.Delfrac, 5 },
            { AdminCommands.Offdelfrac, 5 },
            { AdminCommands.Sban, 5 },
            { AdminCommands.Setar, 5 },
            { AdminCommands.Givegun, 5 },
            { AdminCommands.Delgun, 5 },
            { AdminCommands.Giveammo, 5 },
            { AdminCommands.Setname, 5 },
            { AdminCommands.Setnameoff, 5 },
            { AdminCommands.Afuel, 5 },
            { AdminCommands.Unban, 5 },
            { AdminCommands.Accept, 5 },
            { AdminCommands.Getlogin, 5 },
            { AdminCommands.Agl, 6 },
            { AdminCommands.Unbanip, 5 },
            { AdminCommands.SkipQuest, 5 },
            { AdminCommands.Stopmatwars, 5 },

            { AdminCommands.Hideme, 6 },
            { AdminCommands.Unhardban, 6 },
            { AdminCommands.Skick, 6 },
            { AdminCommands.Setbizmafia, 6 },
            { AdminCommands.Setcolour, 6 },
            { AdminCommands.Setskin, 6 },
            { AdminCommands.Elections, 6 },
            { AdminCommands.Sc, 6 },
            { AdminCommands.Sac, 6 },
            { AdminCommands.Pa, 6 },
            { AdminCommands.Sa, 6 },
            { AdminCommands.Szstate, 6 },
            { AdminCommands.Ishard, 6 },
            { AdminCommands.Promosync, 6 },
            { AdminCommands.Bonussync, 6 },
            { AdminCommands.Alog, 6 },
            { AdminCommands.Medialist, 6 },
            { AdminCommands.Startmatwars, 6 },
            { AdminCommands.Skin, 6 },
            { AdminCommands.Setvehcord, 6 },
            { AdminCommands.DelObjects, 6 },
            { AdminCommands.Tsc, 6 },
            { AdminCommands.Goadditem, 6 },
            { AdminCommands.Nalog, 6 },
            { AdminCommands.Acancel, 6 },
            
            { AdminCommands.Restart, 7 },
            { AdminCommands.Save, 7 },
            { AdminCommands.Banlogin, 7 },
            { AdminCommands.Setadmin, 7 },
            { AdminCommands.Deladmin, 7 },
            { AdminCommands.Offdeladmin, 7 },
            { AdminCommands.Arank, 7 },
            { AdminCommands.Offarank, 7 },
            { AdminCommands.Givevip, 7 },
            { AdminCommands.Giveexp, 7 },
            { AdminCommands.Givelvl, 7 },
            { AdminCommands.GetRb, 6 },
            { AdminCommands.GetVip, 1 },
            { AdminCommands.Payday, 7 },
            { AdminCommands.Offgivevip, 7 },
            { AdminCommands.Stt, 7 },
            { AdminCommands.Svm, 7 },
            { AdminCommands.Svn, 7 },
            { AdminCommands.Svh, 7 },
            { AdminCommands.Setfractun, 7 },
            { AdminCommands.Setfracveh, 7 },
            { AdminCommands.Vehs, 7 },
            { AdminCommands.Fclear, 7 },
            { AdminCommands.Aclear, 7 },
            { AdminCommands.Enablefunc, 7 },
            { AdminCommands.setfamily, 7 },
            { AdminCommands.Crimeban, 6 },
            { AdminCommands.Offcrimeban, 6 },
            { AdminCommands.Uncrimeban, 6 },
            { AdminCommands.Offuncrimeban, 6 },

            

            { AdminCommands.Givecarnumber, 8 },
            { AdminCommands.Additem, 8 },
            { AdminCommands.Carcoupon, 8 },
            { AdminCommands.Givereds, 8 },
            { AdminCommands.Giveredsall, 9 },
            { AdminCommands.Offgivemoney, 8 },
            { AdminCommands.Givemoney, 8 },
            { AdminCommands.Giveclothes, 8 },
            { AdminCommands.Takeoffbiz, 8 },
            { AdminCommands.Fsetcmd, 8 },
            { AdminCommands.Vlist, 8 },
            { AdminCommands.Setprod, 8 },
            { AdminCommands.Changestock, 8 },
            { AdminCommands.Vehchange, 8 },
            { AdminCommands.offgivereds, 8 },
            { AdminCommands.creategarage, 8 },
            { AdminCommands.setbliporg, 8 },
            { AdminCommands.delbliporg, 8 },
            { AdminCommands.setmicrophone, 8 },
            { AdminCommands.Spvehs, 7 },

            { AdminCommands.Att, 9 },
            { AdminCommands.Givecase, 9 },
            { AdminCommands.Hidenick, 9 },
            { AdminCommands.Newsimcard, 9 },
            { AdminCommands.Createbusiness, 9 },
            { AdminCommands.Createunloadpoint, 9 },
            { AdminCommands.Changebiztax, 9 },
            { AdminCommands.Deletebusiness, 9 },
            { AdminCommands.Setproductbyindex, 9 },
            { AdminCommands.Deleteproducts, 9 },
            { AdminCommands.Changebizprice, 9 },
            { AdminCommands.Changehouseprice, 9 },
            { AdminCommands.Housetypeprice, 9 },
            { AdminCommands.Delhouseowner, 9 },
            { AdminCommands.Dmgmodif, 9 },
            { AdminCommands.Vehcustom, 9 },
            { AdminCommands.Vehcustompcolor, 9 },
            { AdminCommands.Vehcustomscolor, 9 },
            { AdminCommands.Sl, 9 },
            { AdminCommands.Sw, 9 },
            { AdminCommands.St, 9 },
            { AdminCommands.loadipl, 9 },
            { AdminCommands.unloadipl, 9 },
            { AdminCommands.loadprop, 9 },
            { AdminCommands.unloadprop, 9 },
            { AdminCommands.starteffect, 9 },
            { AdminCommands.stopeffect, 9 },
            //{ AdminCommands.muted, 9 },
            { AdminCommands.createsafe, 9 },
            { AdminCommands.removesafe, 9 },
            { AdminCommands.setvehdirt, 9 },
            { AdminCommands.givehc, 6 },
            { AdminCommands.givehcrad, 6 },
            { AdminCommands.tr_ev_start, 9 },
            { AdminCommands.removegarage, 9 },
            { AdminCommands.createhouse, 9 },
            { AdminCommands.setparkplace, 9 },
            { AdminCommands.setgarage, 9 },
            { AdminCommands.removehouse, 9 },
            { AdminCommands.housechange, 9 },



        });
        private static Dictionary<string, sbyte> AdminAccess = new Dictionary<string, sbyte>();

        private static string DefaultVipAccess = JsonConvert.SerializeObject(new Dictionary<string, sbyte>()
        {
            { VipCommands.Cam, 5 },
            { VipCommands.Camtime, 5 },
        });

        private static Dictionary<string, sbyte> VipAccess = new Dictionary<string, sbyte>();

        private static string[] LoginAccess = new string[]
        {
            AdminCommands.Banlogin,
            AdminCommands.Getlogin,
            AdminCommands.Unbanlogin,
            AdminCommands.Connecttype,
            AdminCommands.Fixmerger,
            AdminCommands.MoneyMultiplier,
            AdminCommands.Expmultiplier,
            AdminCommands.Ecosync,
            AdminCommands.Bansync,
            AdminCommands.Updateadminaccess,
            AdminCommands.Defaultadminaccess,
            AdminCommands.Redname,
            AdminCommands.Drone,
            AdminCommands.RefresEverydayAward,
            AdminCommands.Refreshsystemstate,
            AdminCommands.UpdateCDN,
            AdminCommands.ReloadResources,
            AdminCommands.SaveServer,
            AdminCommands.Refresh,
            AdminCommands.Givelvl,
            AdminCommands.Givereds,
            AdminCommands.Givemoney,
            AdminCommands.Additem,
            AdminCommands.Givegun,
            AdminCommands.Giveclothes,
            AdminCommands.Restart,
            AdminCommands.Stt
        };
        
        public static string[] LoginsDirector = new string[3] { "source1488", "sokolyansky", "qwelpy" };

        [ServerEvent(Event.ResourceStart)]
        public void Event_ResourceStart()
        {
            LoadCommands();
        }
        public async void LoadCommands()
        {
            await using var db = new ServerBD("MainDB");//При старте игры

            var account = db.Adminaccess
                .ToList();

            AdminAccess = JsonConvert.DeserializeObject<Dictionary<string, sbyte>>(DefaultAdminAccess);
            VipAccess = JsonConvert.DeserializeObject<Dictionary<string, sbyte>>(DefaultVipAccess);

            if (account.Count == 0)
            {
                foreach (KeyValuePair<string, sbyte> access in AdminAccess)
                {
                    db.Insert(new Adminaccesses
                    {
                        Command = access.Key,
                        Isadmin = true,
                        Minrank = access.Value,
                    });
                }
                foreach (KeyValuePair<string, sbyte> access in VipAccess)
                {
                    db.Insert(new Adminaccesses
                    {
                        Command = access.Key,
                        Isadmin = false,
                        Minrank = access.Value,
                    });
                }
            }
            else
            {
                foreach (Adminaccesses command in account)
                {
                    if (Convert.ToBoolean(command.Isadmin) && AdminAccess.ContainsKey(command.Command))
                    {
                        AdminAccess[command.Command] = Convert.ToSByte(command.Minrank);
                    }
                    else if (!Convert.ToBoolean(command.Isadmin) && VipAccess.ContainsKey(command.Command))
                    {
                        VipAccess[command.Command] = Convert.ToSByte(command.Minrank);
                    }
                }
            }

            CMD_ReloadResources(null);
        }
        [Command(AdminCommands.Updateadminaccess)]
        public void CMD_updateadminaccess(ExtPlayer player)
        {
            if (!player.IsCharacterData()) return;
            else if (!CanUseCmd(player, AdminCommands.Updateadminaccess)) return;
            LoadCommands();
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно обновили доступы к командам!", 10000);
        }
        [Command(AdminCommands.Defaultadminaccess)]
        public void CMD_defaultadminaccess(ExtPlayer player)
        {
            if (!player.IsCharacterData()) return;
            else if (!CanUseCmd(player, AdminCommands.Defaultadminaccess)) return;
            AdminAccess = JsonConvert.DeserializeObject<Dictionary<string, sbyte>>(DefaultAdminAccess);
            VipAccess = JsonConvert.DeserializeObject<Dictionary<string, sbyte>>(DefaultVipAccess);
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно сделали стандартные доступы к командам!", 10000);
        }
        [Command(AdminCommands.ReloadResources)]
        public void CMD_ReloadResources(ExtPlayer player)
        {
            //if (!player.IsCharacterData()) return;
            //if (!CanUseCmd(player, AdminCommands.ReloadResources)) return; 
            //NAPI.Player.ReloadResources();
            
            
            
            //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно обновили ресурсы 1!", 10000);

            
            //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно обновили ресурсы! 2", 10000);
        }

        public static bool CanUseCmd(ExtPlayer player, string cmd, string args = "")
        {
            Accounts.Models.AccountData accountData = player.GetAccountData();
            if (accountData == null) return false;

            CharacterData characterData = player.GetCharacterData();
            if (characterData == null) return false;

            if (AdminAccess.ContainsKey(cmd) && characterData.AdminLVL >= AdminAccess[cmd]) return true;
            if (VipAccess.ContainsKey(cmd) && accountData.VipLvl >= VipAccess[cmd]) return true;
            if (LoginAccess.Contains(cmd) && (LoginsDirector.Contains(accountData.Login) || Main.ServerNumber == 0)) return true;

            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно прав", 3000);
            return false;
        }
        [Command("tvv")]
        public void tests(ExtPlayer player, int id)
        {
            if (!player.IsCharacterData()) return;
            Trigger.ClientEvent(player, "startTestV", id);
        }
        [Command("cams")]
        public void cam(ExtPlayer player)
        {
            if (Main.ServerNumber != 0)
                return;
            var characterData = player.GetCharacterData();
            if (characterData == null)
                return;
            Trigger.ClientEvent(player, "screen", "char", characterData.Gender);
        }
        [Command("tests")]
        public void testss(ExtPlayer player)
        {
            if (Main.ServerNumber != 0)
                return;
            
            
            Trigger.ClientEvent(player, "StartDangerButtonSound_client", "sounds/panic.mp3");
        }
        [Command("camtype")]
        public void camtype(ExtPlayer player, string name)
        {
            if (Main.ServerNumber != 0)
                return;
            Task.Run(() =>
            {
                var characterData = player.GetCharacterData();
                if (characterData == null)
                    return;
                bool gender = characterData.Gender;
                
                var dictionary = (ClothesComponent)Enum.Parse(typeof(ClothesComponent), name);
                
                
                ConcurrentDictionary<int, ClothesData> shoesData = null; 
                
                if (dictionary == ClothesComponent.Bugs)
                {
                    shoesData = ClothesComponents.ClothesBugsData;
                }
                else
                {
                    if (dictionary == ClothesComponent.Undershort)shoesData = ClothesComponents.ClothesComponentData[gender][ClothesComponent.Tops];
                    else shoesData = ClothesComponents.ClothesComponentData[gender][dictionary];
                }
                
                var prop = new Dictionary<ClothesComponent, int>()
                {
                    {ClothesComponent.Hat, 0 },
                    {ClothesComponent.Watches, 6 },
                    {ClothesComponent.Glasses, 1 },
                    {ClothesComponent.Ears, 2 },
                    {ClothesComponent.Bracelets, 7 },
                };

                var variation = new Dictionary<ClothesComponent, int>()
                {
                    {ClothesComponent.Masks, 1 },
                    {ClothesComponent.Tops, 11 },
                    {ClothesComponent.Undershort, 11 },
                    {ClothesComponent.Legs, 4 },
                    {ClothesComponent.Shoes, 6 },
                    {ClothesComponent.Accessories, 7 },
                    {ClothesComponent.Bugs, 5 },
                    {ClothesComponent.Torsos, 3 },
                };
                if (variation.ContainsKey(dictionary))
                {
                    Trigger.ClientEvent(player, "startVariationCam", variation[dictionary]);
                    Trigger.ClientEvent(player, "startVariation", -1, variation[dictionary], shoesData[1].Variation, 0);
                }
                if (prop.ContainsKey(dictionary))
                {
                    Trigger.ClientEvent(player, "startPropCam", prop[dictionary]);
                    Trigger.ClientEvent(player, "startProp", -1, prop[dictionary], shoesData[1].Variation, 0);
                }
                
                Thread.Sleep(10000);
                
                foreach (var clData in shoesData)
                {
                    if (dictionary == ClothesComponent.Tops && clData.Value.Type == -1) continue;
                    if (dictionary == ClothesComponent.Undershort && clData.Value.Type != -1) continue;
                    if (clData.Value.Gender != -1 && clData.Value.Gender != Convert.ToInt32(gender)) continue;
                    if (clData.Value.Textures.Count == 0) continue;
                    Thread.Sleep(250);
                    foreach (var text in clData.Value.Textures)
                    {
                        if (variation.ContainsKey(dictionary))
                        {
                            Trigger.ClientEvent(player, "startVariation", clData.Key, variation[dictionary], clData.Value.Variation, text);
                            Thread.Sleep(250); 
                        }
                        if (prop.ContainsKey(dictionary))
                        {
                            Trigger.ClientEvent(player, "startProp", clData.Key, prop[dictionary], clData.Value.Variation, text);
                            Thread.Sleep(250);
                        }
                    }
                }
            });
        }
        [Command("camtypev")]
        public void camtypev(ExtPlayer player, int Variation, int Texture)
        {
            
            if (Main.ServerNumber != 0)
                return;
            Task.Run(() =>
            {
                Trigger.ClientEvent(player, "startVariationCam", 3);
                Trigger.ClientEvent(player, "startVariation", Variation, 3, Variation, Texture);
            });
        }
        [Command("camtypes")]
        public void camtypes(ExtPlayer player, int slot, int Variation, int Texture)
        {
            
            if (Main.ServerNumber != 0)
                return;
            Task.Run(() =>
            {
                Trigger.ClientEvent(player, "startVariationCam", slot);
                Trigger.ClientEvent(player, "startVariation", Variation, slot, Variation, Texture);
            });
        }
        [Command("camtypeid")]
        public void camtypeid(ExtPlayer player, string name, int startid, int count)
        {
            if (Main.ServerNumber != 0)
                return;
            Task.Run(() =>
            {
                var characterData = player.GetCharacterData();
                if (characterData == null)
                    return;
                bool gender = characterData.Gender;
                
                var dictionary = (ClothesComponent)Enum.Parse(typeof(ClothesComponent), name);
                
                ConcurrentDictionary<int, ClothesData> shoesData = null; 
                
                if (dictionary == ClothesComponent.Bugs)
                {
                    shoesData = ClothesComponents.ClothesBugsData;
                }
                else
                {
                    if (dictionary == ClothesComponent.Undershort)
                        dictionary = ClothesComponent.Tops;
                    
                    if (!ClothesComponents.ClothesComponentData[gender].ContainsKey(dictionary))
                        return;
                    
                    shoesData = ClothesComponents.ClothesComponentData[gender][dictionary];
                }
                
                var prop = new Dictionary<ClothesComponent, int>()
                {
                    {ClothesComponent.Hat, 0 },
                    {ClothesComponent.Watches, 6 },
                    {ClothesComponent.Glasses, 1 },
                    {ClothesComponent.Ears, 2 },
                    {ClothesComponent.Bracelets, 7 },
                };

                var variation = new Dictionary<ClothesComponent, int>()
                {
                    {ClothesComponent.Masks, 1 },
                    {ClothesComponent.Tops, 11 },
                    {ClothesComponent.Undershort, 11 },
                    {ClothesComponent.Legs, 4 },
                    {ClothesComponent.Shoes, 6 },
                    {ClothesComponent.Accessories, 7 },
                    {ClothesComponent.Bugs, 5 },
                    {ClothesComponent.Torsos, 3 },
                };
                if (variation.ContainsKey(dictionary))
                {
                    Trigger.ClientEvent(player, "startVariationCam", variation[dictionary]);
                    Trigger.ClientEvent(player, "startVariation", -1, variation[dictionary], shoesData[startid].Variation, 0);
                }
                if (prop.ContainsKey(dictionary))
                {
                    Trigger.ClientEvent(player, "startPropCam", prop[dictionary]);
                    Trigger.ClientEvent(player, "startProp", -1, prop[dictionary], shoesData[startid].Variation, 0);
                }
                
                Thread.Sleep(10000);


                for (int i = startid; i < startid + count; i++)
                {
                    if (shoesData.ContainsKey(i))
                    {
                        var clData = shoesData[i];        
                        if (dictionary == ClothesComponent.Tops && clData.Type == -1) continue;
                        if (dictionary == ClothesComponent.Undershort && clData.Type != -1) continue;
                        if (clData.Gender != -1 && clData.Gender != Convert.ToInt32(gender)) continue;
                        if (clData.Textures.Count == 0) continue;
                        foreach (var text in clData.Textures)
                        {
                            if (variation.ContainsKey(dictionary))
                            {
                                Trigger.ClientEvent(player, "startVariation", i, variation[dictionary], clData.Variation, text);
                                Thread.Sleep(250);
                            }
                            if (prop.ContainsKey(dictionary))
                            {
                                Trigger.ClientEvent(player, "startProp", i, prop[dictionary], clData.Variation, text);
                                Thread.Sleep(250);
                            }
                        }
                        Thread.Sleep(250);
                    }
                }
            });
        }
        [Command("camv")]
        public void camv(ExtPlayer player)
        {
            if (Main.ServerNumber != 0)
                return;
            var characterData = player.GetCharacterData();
            if (characterData == null)
                return;
            Trigger.ClientEvent(player, "screen", "veh");
        }
        [Command("camveh")]
        public void camveh(ExtPlayer player)
        {
            if (Main.ServerNumber != 0)
                return;
            Task.Run(() =>
            {
                var vehs = new List<string>()
                {
                    "fordraptor",
                    "206gti",
                    "snowbike",
                    "brz13",
                    "audirs7",

                };
                foreach (var car in vehs)
                {
                    Trigger.ClientEvent(player, "startVeh", car);
                    Thread.Sleep(750);
                }
            });
        }
        [Command("pcam")]
        public void pcam(ExtPlayer player)
        {
            if (Main.ServerNumber != 0)
                return;
            Trigger.ClientEvent(player, "__client_smartphone_camera", false, true);
        }
        [Command("testsp")]
        public void testsp(ExtPlayer player, int prop, int max)
        {
            if (Main.ServerNumber != 0)
                return;
            if (!player.IsCharacterData()) return;
            Trigger.ClientEvent(player, "startProp", prop, max);
        }
        [Command("camo")]
        public void camo(ExtPlayer player)
        {
            if (Main.ServerNumber != 0)
                return;
            var characterData = player.GetCharacterData();
            if (characterData == null)
                return;
            Trigger.ClientEvent(player, "screen", "obj");
        }
        [Command("camobj")]
        public void camobj(ExtPlayer player)
        {
            if (Main.ServerNumber != 0)
                return;
            Task.Run(() =>
            {
                var objList = new List<uint>()
                {
                    289,
                    290, 
                    291,
                    292,
                    293,
                    294,
                    295,
                    296,
                    297,
                    298,
                    299,
                    300,
                    301,
                    302,
                    303,
                    304,
                    305,
                    306,
                    307,
                    308,
                    309,
                    310,
                    311,
                    312,
                    313,
                    314,
                    315,
                    316,
                    317,
                    318,
                    319,
                    320,
                    321,
                    322,
                    323,
                    324,
                    325,
                    326,
                    327,
                    328,
                    329,
                    330,
                    331,
                    332,
                    333,
                    334
                };
                int index = 0;
                foreach (var car in objList)
                {
                    Trigger.ClientEvent(player, "startObj",index, car);
                    index++;
                    Thread.Sleep(250);
                }
                /*foreach (var car in Houses.FurnitureManager.NameModels)
                {
                    Trigger.ClientEvent(player, "startObj",car.Value.Prop, NAPI.Util.GetHashKey(car.Value.Prop));
                    Thread.Sleep(250);
                }*/
            });
        }
        [Command("objecteditor")]
        public void objecteditor(ExtPlayer player, string model)
        {
            
            if (Main.ServerNumber != 0)
                return;
            Trigger.ClientEvent(player, "objecteditor", model);
        }
    }
}
