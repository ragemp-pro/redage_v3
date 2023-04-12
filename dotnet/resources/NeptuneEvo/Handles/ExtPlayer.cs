using System.Collections.Concurrent;
using System.Collections.Generic;
using GTANetworkAPI;
using NeptuneEvo.Accounts.Models;
using NeptuneEvo.BattlePass.Models;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Core;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Functions;
using NeptuneEvo.GUI;
using NeptuneEvo.Organizations.Models;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players.Phone.Models;
using NeptuneEvo.Quests.Models;
using NeptuneEvo.Table.Tasks.Models;

namespace NeptuneEvo.Handles
{
    public class ExtPlayer : Player
    {       
        public ExtPlayer(NetHandle handle) : base(handle)
        {
        }
        
        //
        
        private string CharacterName;

        public void SetName(string name)
        {
            this.CharacterName = name;
            
            if (SessionData != null)
                SessionData.Name = name;
        }

        public string GetName() => this.CharacterName;
        
        
        //
        
        private int UUID;

        public void SetUUID(int uuid)
        {
            this.UUID = uuid;
        }

        public int GetUUID() => this.UUID;
        
        //
        
        
        public SessionData SessionData;
        public void SetSessionData(SessionData sessionData)
        {
            SessionData = sessionData;
        }
        
        public bool IsRestartSaveCharacterData = false;
        public CharacterData CharacterData;
        public void SetCharacterData(CharacterData characterData)
        {
            CharacterData = characterData;
        }
        
        public bool IsRestartSaveAccountData = false;
        public AccountData AccountData;
        public void SetAccountData(AccountData accountData)
        {
            AccountData = accountData;
        }
        
        public List<ExtColShapeData> ColShapesData;
        public void AddColShapeData(ExtColShapeData сolShapeData)
        {
            if (ColShapesData == null)
                ColShapesData = new List<ExtColShapeData>();
            
            if (InteractionCollection.isFunction(сolShapeData.ColShapeId.ToString()))
                ColShapesData.Add(сolShapeData);
            else
                ColShapesData.Insert(0,сolShapeData);

        }
        public void DeleteColShapeData(ExtColShapeData сolShapeData)
        {
            if (ColShapesData != null && ColShapesData.Contains(сolShapeData))
                ColShapesData.Remove(сolShapeData);
        }
        //
        public PlayerCustomization Customization;
        public void SetCustomization(PlayerCustomization сustomization)
        {
            Customization = сustomization;
        }
        
        //Квесты
        public PlayerQuestModel Quest;
        public void SelectQuest(PlayerQuestModel quest)
        {
            Quest = quest;
        }
        public void ClearQuest()
        {
            Quest = null;
        }

        //
        public ConcurrentDictionary<int, InventoryItemData> Accessories = null;
        public void SetAccessories(int slotId, InventoryItemData item)
        {
            if (Accessories == null)
                Accessories = new ConcurrentDictionary<int, InventoryItemData>();

            Accessories[slotId] = item;
        }
        public void DeleteAccessories(int slotId)
        {
            if (Accessories != null && Accessories.ContainsKey(slotId))
                Accessories.TryRemove(slotId, out _);
        }
        public bool IsAccessories(int slotId)
        {
            if (Accessories != null)
                return Accessories.ContainsKey(slotId);

            return false;
        }
        public void ClearAccessories()
        {
            Accessories = null;
        }

        //
        
        public PedHash Skin = PedHash.Skidrow01AMM;
        
        public PedHash GetSkin()
        {
            return Skin;
        }

        //
        public BattlePassData BattlePassData;
        public void SetBattlePassData(BattlePassData battlePassData)
        {
            BattlePassData = battlePassData;
        }
        
        //
        public MissionData MissionData;
        public void SetMissionTask(MissionData missionData)
        {
            MissionData = missionData;
            
            if (missionData.Tasks.Count == 0)
                BattlePass.Repository.UpdateMission(this);
        }
        
        //
        
        public PhoneData PhoneData;
        public void SetPhoneData(PhoneData phoneData)
        {
            PhoneData = phoneData;
        }
        
        //
        
        public KeyClampData KeyClampData;
        public void SetKeyClampData(KeyClampData keyClampData = null)
        {
            KeyClampData = keyClampData;
        }
        
        //

        public OrganizationMemberData OrganizationData;
        public void SetOrganizationData(OrganizationMemberData organizationData = null)
        {
            OrganizationData = organizationData;
        }
        
        //

        public FractionMemberData FractionData;
        public void SetFractionData(FractionMemberData fractionData = null)
        {
            FractionData = fractionData;
        }
        
        //
        public TableTaskPlayerData[] OrganizationTasksData = null;
        
        //
        public TableTaskPlayerData[] FractionTasksData = null;
        
        

    }
}