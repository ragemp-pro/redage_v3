using System;
using System.Collections.Generic;
using NeptuneEvo.Character;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Handles;
using NeptuneEvo.Organizations.Models;
using NeptuneEvo.Organizations.Player;
using NeptuneEvo.Table.Models;
using Newtonsoft.Json;

namespace NeptuneEvo.Organizations.Table.Upgrate
{
    public class Repository
    {
        public static List<OrganizationUpdateData> Get(ExtPlayer player, OrganizationData organizationData)
        {
            var updateInfo = new List<OrganizationUpdateData>();
            if (player.IsOrganizationAccess(RankToAccess.OrgUpgrate, false))
            {
                int upgraded = organizationData.OfficeUpgrade;
                if (upgraded == 0) 
                    updateInfo.Add(new OrganizationUpdateData("upgrade", "Улучшение офиса", "organisationsicon-office", false, Main.PricesSettings.FirstOrgPrice));
                else if (upgraded == 1) 
                    updateInfo.Add(new OrganizationUpdateData("upgrade", "Улучшение офиса", "organisationsicon-office", true, Main.PricesSettings.SecondOrgPrice));
                    
                if (!organizationData.Stock)
                    updateInfo.Add(new OrganizationUpdateData("upgrades", "Построить склад", "inv-safe", false, Main.PricesSettings.StockPrice));

                if (organizationData.CrimeOptions)
                {
                    foreach (var p in organizationData.Schemes)
                    {
                        if (Manager.WeaponsOrgPrice.ContainsKey(p.Key) && !p.Value)
                        {
                            var wType = p.Key == "Armor"
                                ? ItemId.BodyArmor
                                : (ItemId) Enum.Parse(typeof(ItemId), p.Key);
                            
                            updateInfo.Add(new OrganizationUpdateData(p.Key, $"Чертёж {p.Key}", Chars.Repository.ItemsInfo[wType].Icon, false, Manager.WeaponsOrgPrice[p.Key]));
                        }
                    }
                }

                if (organizationData.IsLeader(player.GetUUID()))
                {
                    updateInfo.Add(new OrganizationUpdateData("updtype", "Сменить тип организации",
                        "fractionsicon-fractions", true, Main.PricesSettings.UpdateTypeOrganization));
                    
                    updateInfo.Add(new OrganizationUpdateData("newleader", "Сменить лидера",
                        "fractionsicon-members", true, Main.PricesSettings.UpdateOrganizationLeader));
                }
            }

            return updateInfo;
        }

        public static void GetData(ExtPlayer player)
        {
            try
            {
                var organizationData = player.GetOrganizationData();
                if (organizationData == null) 
                    return;
                
                var upgrate = Get(player, organizationData);
                
                Trigger.ClientEvent(player, "client.org.main.setUpgrate", JsonConvert.SerializeObject(upgrate));
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        
        public static void OnBuy(ExtPlayer player, string type)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                if (!player.IsOrganizationAccess(RankToAccess.OrgUpgrate)) return;

                switch (type)
                {
                    case "upgrade":
                        if (Manager.UpgradeOrganization(player))
                            GetData(player);
                        break;
                    case "upgrades":
                        if (Manager.StockBuy(player))
                            GetData(player);
                        break;
                    case "updtype":
                        if (Manager.UpdateCrimeOptions(player))
                            GetData(player);
                        break;
                    default:
                        foreach (var p in Organizations.Manager.WeaponsOrgPrice)
                        {
                            if (p.Key != type)
                                continue;
                            
                            if (Manager.SchemeBuy(player, p.Key, p.Value))
                                GetData(player);
                            break;
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
    }
}