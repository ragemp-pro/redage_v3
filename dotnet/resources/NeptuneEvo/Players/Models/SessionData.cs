using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Chars.Models;
using System;
using System.Collections.Generic;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Players.Popup.Models;
using NeptuneEvo.Table.Tasks.Models;

namespace NeptuneEvo.Players.Models
{
    public class SessionData
    {
        public int Value { get; set; } = -1;
        public uint Dimension { get; set; } = 0;
        public string Name { get; set; } = "";
        public bool IsSession { get; set; } = false;
        public bool IsSessionOneTime { get; set; } = false;
        public AuntificationData AuntificationData { get; set; } = new AuntificationData();
        public string RecoveryCode { get; set; } = null;
        public bool IsConnect { get; set; } = true;
        public bool LoggedIn { get; set; } = false;
        public WorkData WorkData { get; set; } = new WorkData();
        public AdminData AdminData { get; set; } = new AdminData();
        public SellItemData SellItemData { get; set; } = new SellItemData();
        public string oldPlayerStats { get; set; } = null;
        public bool LookingStats { get; set; } = false;
        public VoiceData VoiceData { get; set; } = new VoiceData();
        public TimingsData TimingsData { get; set; } = new TimingsData();
        public TicketsData TicketsData { get; set; } = new TicketsData();
        public ResistData ResistData { get; set; } = new ResistData();
        public CuffedData CuffedData { get; set; } = new CuffedData();
        public TimersData TimersData { get; set; } = new TimersData();
        public RequestData RequestData { get; set; } = new RequestData();
        public DeliveryData DeliveryData { get; set; } = new DeliveryData();
        public DeathData DeathData { get; set; } = new DeathData();
        public InviteData InviteData { get; set; } = new InviteData();
        public DiceData DiceData { get; set; } = new DiceData();
        public DSchoolData DSchoolData { get; set; } = new DSchoolData();
        public SelectData SelectData { get; set; } = new SelectData();
        public CreatorData CreatorData { get; set; } = new CreatorData();
        public HotelData HotelData { get; set; } = new HotelData();
        public HouseData HouseData { get; set; } = new HouseData();
        public ATMData ATMData { get; set; } = new ATMData();
        public OrderData OrderData { get; set; } = new OrderData();
        public TaxiData TaxiData { get; set; } = new TaxiData();
        public List<uint> Attachments { get; set; } = new List<uint>();
        public WeaponData LastActive { get; set; } = new WeaponData();
        public AntiKill KillData { get; set; } = new AntiKill();
        public string RealSocialClub { get; set; } = "NONE";
        public string SocialClubName { get; set; } = null;
        public string RealHWID { get; set; } = "NONE";
        public string Address { get; set; } = null;
        public int InsideSafeZone { get; set; } = -1;
        public bool IsSafeZone { get; set; } = false;
        public bool AccBanned { get; set; } = false;
        public bool InCpMode { get; set; } = false;
        public int InArrestArea { get; set; } = -1;
        public bool IsHicjacking { get; set; } = false;
        public int SitPos { get; set; } = -1;
        public int BizID { get; set; } = -1;
        public int TempBizID { get; set; } = -1;
        public int TempSafeID { get; set; } = -1;
        public int CurrentStage { get; set; } = -1;
        public int OnMafiaID { get; set; } = -1;
        public int OnBikerID { get; set; } = -1;
        public int OnFracStock { get; set; } = 0;
        public int GarageID { get; set; } = -1;
        public int HouseID { get; set; } = -1;
        public ExtPlayer Following { get; set; } = null;
        public ExtPlayer Follower { get; set; } = null;
        public bool AntiAnimDown { get; set; } = false;
        public ExtPlayer DicePlayingWith { get; set; } = null;
        public int DiceMoney { get; set; } = 0;
        public int SMSNum { get; set; } = -1;
        public bool ToResetAnimPhone { get; set; } = false;
        public ExtVehicle VehicleMats { get; set; } = null;
        public string WhereLoad { get; set; } = null;
        public bool Phone { get; set; } = false;
        public bool HeadPocket { get; set; } = false;
        public bool HandsUp { get; set; } = false;
        public int SuperSecretData { get; set; } = -1;
        public string CarSellGov { get; set; } = null;
        public string TempCar { get; set; } = null;
        public bool AdminSkin { get; set; } = false;
        public ItemStruct ActiveWeap { get; set; } = new ItemStruct("", -1, null);
        public int LastActiveWeap { get; set; } = 0;
        public ExtVehicle TestDriveVehicle { get; set; } = null;
        public byte CaseWin { get; set; } = 255;
        public byte CaseItemWin { get; set; } = 255;
        public bool LastCoverState { get; set; } = false;
        public ExtVehicle AttachToVehicle { get; set; } = null;
        public List<string> Muted { get; set; } = new List<string>();
        public string IsCasinoGame { get; set; } = null;
        public bool isDeaf { get; set; } = false;
        public bool Punishments { get; set; } = false;
        public DateTime StreetRaceTime { get; set; } = DateTime.Now;
        public int ArmorHealth { get; set; } = -1;
        public TradePropertyData TradePropertyData { get; set; } = null;
        public List<RouletteData> RouletteData { get; set; } = new List<RouletteData>();
        public ChangeAutoNumber ChangeAutoNumber { get; set; } = null;
        public int SappeData { get; set; } = -1;
        public string AnimationUse { get; set; } = null;
        public KilledData KilledData { get; set; } = new KilledData();
        public ItemsTrade ItemsTrade { get; set; } = null;
        public RentData RentData { get; set; } = null;
        public int WalkieTalkieFrequency { get; set; } = -99;
        public bool IsRadioOn { get; set; } = false;
        public bool IsInLabelActionShape { get; set; } = false;
        public bool IsPressedDangerButton { get; set; } = false;
        public int AcceptedDangerCall { get; set; } = -1;
        public AfkData AfkData { get; set; } = new AfkData();
        public string TempNewRefCode { get; set; } = null;
        public bool IsAirDropHacking { get; set; } = false;
        public Vector3 PositionCaptureOrBizwar { get; set; } = null;
        public byte UsedTPOnCaptureOrBizwar { get; set; } = 0;
        public bool IsInAirDropZone { get; set; } = false;
        public int CurrentBankTransferSumAccBankInfo { get; set; } = 0;
        public int CurrentBankTransferBankInfo { get; set; } = 0;
        public int CurrentBankTransferSum { get; set; } = 0;
        public byte IsCalledGovMember { get; set; } = 0;
        public bool WasOnStartCapture { get; set; } = false;
        public int IsInBoomboxShape { get; set; } = -1;
        public string InventoryOtherLocationName { get; set; } = null;
        public InventoryTentData InventoryTentData { get; set; } = null;
        public int TentIndex { get; set; } = -1;
        public ExtPed MyPed { get; set; } = null;
        public ExtPlayer CarryPlayer { get; set; } = null;
        public DateTime CarryPlayerTime { get; set; } = DateTime.Now;
        public WeddingData WeddingData { get; set; } = null;
        public int InAirsoftLobby { get; set; } = -1;
        public int InMafiaLobby { get; set; } = -1;
        public int InTanksLobby { get; set; } = -1;
        public DateTime LastPointTime { get; set; } = DateTime.Now;
        public int ActiveIndex { get; set; } = -1;
        public bool IsOrgTableActive { get; set; } = false;
        public bool IsOrgStockActive { get; set; } = false;


        public ExtPed SelectPed { get; set; } = null;
        public string DelObjects { get; set; } 
        public int SelectUUID { get; set; } = 0;

        public Dictionary<int, Dictionary<int, ComponentVariation>> Clothes { get; set; } =
            new Dictionary<int, Dictionary<int, ComponentVariation>>();

        public Dictionary<int, Dictionary<int, ComponentVariation>> Accessory { get; set; } =
            new Dictionary<int, Dictionary<int, ComponentVariation>>();
        
        public TempEditableClothingData TempClothingData { get; set; } = new TempEditableClothingData();
        
        public int LastCashOperationSum { get; set; } = 0;
        public int LastBankOperationSum { get; set; } = 0;
        public int LastSellOperationSum { get; set; } = 0;
        public ExtVehicle TicketVehicle { get; set; } = null;
        public int TicketCost { get; set; } = 0;
        public DateTime TicketAntiFlood { get; set; } = DateTime.Now;
        public bool IsTicketRender { get; set; } = false;
        public bool IsInitMission { get; set; } = false;
        public PopupData PopupData { get; set; } = new PopupData();
        public bool HitPoint { get; set; } = false; 
        public bool IsRadioInterceptor { get; set; } = false; 
        
        public WarData WarData { get; set; } = new WarData();
        public TableTaskData TableTaskData { get; set; } = new TableTaskData();
        public bool IsWarMarker = false;
        
        public TableTaskPlayerData[] TasksData = null;
    }
}
