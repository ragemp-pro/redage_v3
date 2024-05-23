using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Character.BindConfig.Models;
using NeptuneEvo.Character.Config.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using NeptuneEvo.Quests;

namespace NeptuneEvo.Character.Models
{
    public class CharacterData
    {
        /// <summary>
        /// 
        /// </summary>
        public int UUID { get; set; } = -1;
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public Vector3 SpawnPos { get; set; } = new Vector3(0, 0, 0);
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; } = DateTime.Now;
        /// <summary>
        /// 
        /// </summary>
        public string FirstName { get; set; } = null;
        /// <summary>
        /// 
        /// </summary>
        public string LastName { get; set; } = null;
        /// <summary>
        /// 
        /// </summary>
        public bool Gender { get; set; } = true;
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public int Health { get; set; } = 100;
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public int Armor { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        public int LVL { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        public int EXP { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        public long Money { get; set; } = 2500;
        /// <summary>
        /// 
        /// </summary>
        public int Bank { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        public int BankMoney { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        public int WorkID { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public ushort DrugsAddiction { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public int ArrestTime { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public sbyte ArrestType { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public int DemorganTime { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public WantedLevel WantedLVL { get; set; } = null;
        /// <summary>
        /// 
        /// </summary>
        public List<int> BizIDs { get; set; } = new List<int>();
        /// <summary>
        /// 
        /// </summary>
        public int AdminLVL { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        public List<bool> Licenses { get; set; } = new List<bool>();
        /// <summary>
        /// 
        /// </summary>
        public DateTime Unwarn { get; set; } = DateTime.Now;
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public int Unmute { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        public int Warns { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public string OnDutyName { get; set; } = String.Empty;
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public int LastHourMin { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        public int HotelID { get; set; } = -1;
        /// <summary>
        /// 
        /// </summary>
        public int HotelLeft { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        public int Sim { get; set; } = -1;
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public string PetName { get; set; } = "null";
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public Dictionary<int, string> Contacts { get; set; } = new Dictionary<int, string>();
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public List<bool> Achievements { get; set; } = new List<bool>();
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public bool VoiceMuted { get; set; } = false;
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public int InsideHouseID { get; set; } = -1;
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public bool InCasino { get; set; } = false;
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public int InsideGarageID { get; set; } = -1;
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public int SelectGarageSlotId { get; set; } = -1;
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public Vector3 ExteriorPos { get; set; } = new Vector3();
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public int InsideHotelID { get; set; } = -1;
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public int TuningShop { get; set; } = -1;
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public bool IsAlive { get; set; } = false;
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public bool IsSpawned { get; set; } = false;
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public int InsideOrganizationID { get; set; } = -1;
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public Vector3 CurPosition { get; set; } = null;
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public DemorganInfo DemorganInfo { get; set; } = new DemorganInfo();
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public WarnInfo WarnInfo { get; set; } = new WarnInfo();
        /// <summary>
        /// 
        /// </summary>
        public TimeInfo Time { get; set; } = new TimeInfo();
        /// <summary>
        /// 
        /// </summary>
        public ulong Deaths { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        public ulong Kills { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        public ulong EarnedMoney { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        public ulong EatTimes { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        public ulong Revived { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        public ulong Handshaked { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        public string houseId { get; set; } = "-";
        /// <summary>
        /// 
        /// </summary>
        public string houseType { get; set; } = "-";
        /// <summary>
        /// 
        /// </summary>
        public string houseCash { get; set; } = "-";
        /// <summary>
        /// 
        /// </summary>
        public string houseCopiesHour { get; set; } = "-";
        /// <summary>
        /// 
        /// </summary>
        public string housePaid { get; set; } = "-";
        /// <summary>
        /// 
        /// </summary>
        public int maxcars { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        public string BizId { get; set; } = "-";
        /// <summary>
        /// 
        /// </summary>
        public string BizCash { get; set; } = "-";
        /// <summary>
        /// 
        /// </summary>
        public string BizCopiesHour { get; set; } = "-";
        /// <summary>
        /// 
        /// </summary>
        public string BizPaid { get; set; } = "-";
        /// <summary>
        /// 
        /// </summary>
        public string RankData { get; set; } = "";
        /// <summary>
        /// 
        /// </summary>
        public string OrgRankData { get; set; } = "";
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public Dictionary<int, int> JobSkills { get; set; } = new Dictionary<int, int>();
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public string RefCode { get; set; } = null;
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public bool IsInPostalStock { get; set; } = false;
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, bool> Friends { get; set; } = new Dictionary<string, bool>();
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public List<QuestData> QuestsData { get; set; } = new List<QuestData>();
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public ConfigData ConfigData { get; set; } = new ConfigData();
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public ChatData ChatData { get; set; } = new ChatData();
        [JsonIgnore]
        public int WeddingUUID { get; set; } = 0;
        public string WeddingName { get; set; } = "";
        public bool IsBannedMP { get; set; } = false;
        public string BanMPReason { get; set; } = "";
        public bool IsBannedCrime { get; set; } = false;
        public string BanCrimeReason { get; set; } = "";

        [JsonIgnore]
        public Dictionary<int, ComponentVariation> Clothes { get; set; } = new Dictionary<int, ComponentVariation>();
        [JsonIgnore]
        public Dictionary<int, ComponentVariation> UsedClothes { get; set; } = new Dictionary<int, ComponentVariation>();

        [JsonIgnore]
        public Dictionary<int, ComponentVariation> Accessory { get; set; } = new Dictionary<int, ComponentVariation>();
        [JsonIgnore]
        public string SelectedQuest { get; set; } = Zdobich.QuestName;
        [JsonIgnore]
        public bool IsForbesShow { get; set; }
        public bool IsLucky { get; set; }
    }
    
}
