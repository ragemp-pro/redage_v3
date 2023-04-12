using System;
using System.Collections.Generic;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using Newtonsoft.Json;
using NeptuneEvo.Core;
using Redage.SDK;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Database;
using LinqToDB;
using Localization;
using MySqlConnector;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Functions;
using NeptuneEvo.Quests.Models;

namespace NeptuneEvo.Houses
{
    public class HouseFurniture
    {
        public string Name { get; }
        public string Model { get; }
        public int Id { get; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public bool IsSet { get; set; }

        [JsonIgnore]
        public GTANetworkAPI.Object obj { get; private set; }

        public HouseFurniture(int id, string name, string model)
        {
            Name = name;
            Model = model;
            Id = id;
            IsSet = false;
        }

        public GTANetworkAPI.Object Create(uint Dimension)
        {
            try
            {
                obj = NAPI.Object.CreateObject(NAPI.Util.GetHashKey(Model), Position, Rotation, 255, Dimension);
                Selecting.Objects.TryAdd(obj.Id, new Selecting.ObjData
                {
                    Type = (Name.Equals("Оружейный сейф") ? "WeaponSafe" : 
                            Name.Equals("Шкаф с одеждой") ? "ClothesSafe" : 
                            Name.Equals("Взломостойкий сейф") ? "BurglarProofSafe" :
                            Name.Equals("Шкаф с предметами") ? "SubjectSafe" :
                            "InteriorItem"),
                    entity = obj,

                });
                return obj;
            }
            catch (Exception e)
            {
                FurnitureManager.Log.Write($"Create Exception: {e.ToString()}");
                return null;
            }
        }
    }

    public class ShopFurnitureBuy
    {
        public string Prop { get; }
        public string Type { get; }
        public int Price;
        public Dictionary<ItemId, int> Items { get; }

        public ShopFurnitureBuy(string prop, string type, int price, Dictionary<ItemId, int> items)
        {
            Prop = prop;
            Type = type;
            Price = price;
            Items = items;
        }
    }
    class FurnitureManager : Script
    {
        public static readonly nLog Log = new nLog("Houses.HouseFurniture");
        public static Dictionary<int, Dictionary<int, HouseFurniture>> HouseFurnitures = new Dictionary<int, Dictionary<int, HouseFurniture>>();
        public static string QuestName = "npc_furniture";
        public static Vector3 FurnitureBuyPos = new Vector3(-591.12317, -285.2158, 35.45478);
        public static void Init()
        {
            try
            {
                using MySqlCommand cmd = new MySqlCommand
                {
                    CommandText = "SELECT * FROM `furniture`"
                };

                using DataTable result = MySQL.QueryRead(cmd);
                if (result == null || result.Rows.Count == 0)
                {
                    Log.Write("DB return null result.", nLog.Type.Warn);
                    return;
                }
                int id = 0;
                string furniture;
                foreach (DataRow Row in result.Rows)
                {
                    try
                    {
                        id = Convert.ToInt32(Row["uuid"].ToString());
                        furniture = Row["furniture"].ToString();
                        Dictionary<int, HouseFurniture> furnitures;
                        if (string.IsNullOrEmpty(furniture)) furnitures = new Dictionary<int, HouseFurniture>();
                        else furnitures = JsonConvert.DeserializeObject<Dictionary<int, HouseFurniture>>(furniture);
                        HouseFurnitures[id] = furnitures;
                    }
                    catch (Exception e)
                    {
                        Log.Write($"FurnitureManager Foreach Exception: {e.ToString()}");
                    }
                }
                Log.Write($"Loaded {HouseFurnitures.Count} players furnitures.", nLog.Type.Success);
                
                Main.CreateBlip(new Main.BlipData(566, "Мебельный магазин",FurnitureBuyPos, 30, true));
                PedSystem.Repository.CreateQuest("s_m_y_airworker", FurnitureBuyPos, -64.57715f, questName: QuestName, title: "~y~NPC~w~ Иван\nПродавец мебели", colShapeEnums: ColShapeEnums.FurnitureBuy);
            }
            catch (Exception e)
            {
                Log.Write($"FurnitureManager Exception: {e.ToString()}");
            }
        }
        [Interaction(ColShapeEnums.FurnitureBuy)]
        private static void Open(ExtPlayer player, int index)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;
            if (!player.IsCharacterData()) return;
            if (sessionData.CuffedData.Cuffed)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsCuffed), 3000);
                return;
            }
            if (sessionData.DeathData.InDeath)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsDying), 3000);
                return;
            }
            if (Main.IHaveDemorgan(player, true)) return;

            player.SelectQuest(new PlayerQuestModel(QuestName, 0, 0, false, DateTime.Now));
            Trigger.ClientEvent(player, "client.quest.open", index, QuestName, 0, 0, 0);
        }
        public static Dictionary<string, ShopFurnitureBuy> NameModels = new Dictionary<string, ShopFurnitureBuy>()
		{
			{ "Оружейный сейф", new ShopFurnitureBuy("prop_ld_int_safe_01", "Хранилища", 0, new Dictionary<ItemId, int>()
				{
					{ ItemId.Iron, 200 },
					{ ItemId.Ruby, 5 },
					{ ItemId.Gold, 5 },
				}
			) },
			{ "Шкаф с одеждой", new ShopFurnitureBuy("bkr_prop_biker_garage_locker_01", "Хранилища", 1, new Dictionary<ItemId, int>()
			{
				{ ItemId.WoodOak, 44 },
				{ ItemId.WoodMaple, 20 },
				{ ItemId.WoodPine, 10 },
			}) },
			{ "Шкаф с предметами", new ShopFurnitureBuy("hei_heist_bed_chestdrawer_04", "Хранилища", 2, new Dictionary<ItemId, int>()
			{
				{ ItemId.WoodOak, 44 },
				{ ItemId.WoodMaple, 20 },
				{ ItemId.WoodPine, 10 },
			}) },
			{ "Взломостойкий сейф", new ShopFurnitureBuy("p_secret_weapon_02", "Хранилища", 3, new Dictionary<ItemId, int>()
			{
				{ ItemId.Iron, 1000 },
				{ ItemId.Gold, 200 },
				{ ItemId.Ruby, 70 },
			}) },

			{ "Пинг-понг", new ShopFurnitureBuy("ch_prop_vault_painting_01a", "Картины", 5, new Dictionary<ItemId, int>()
			{
				{ ItemId.WoodOak, 80 },
			}) },
			{ "Стримснайперы", new ShopFurnitureBuy("ch_prop_vault_painting_01b", "Картины", 6, new Dictionary<ItemId, int>()
			{
				{ ItemId.WoodMaple, 40 },
			}) },
			{ "Завод", new ShopFurnitureBuy("ch_prop_vault_painting_01f", "Картины", 7, new Dictionary<ItemId, int>()
			{
				{ ItemId.WoodPine, 25 },
			}) },
			{ "Переговоры", new ShopFurnitureBuy("ch_prop_vault_painting_01h", "Картины", 8, new Dictionary<ItemId, int>()
			{
				{ ItemId.WoodOak, 80 },
			}) },
			{ "Девчонки", new ShopFurnitureBuy("ch_prop_vault_painting_01j", "Картины", 9, new Dictionary<ItemId, int>()
			{
				{ ItemId.WoodMaple, 40 },
			}) },

			{ "DAB", new ShopFurnitureBuy("vw_prop_casino_art_statue_01a", "Статуи", 10, new Dictionary<ItemId, int>()
			{
				{ ItemId.WoodPine, 250 },
			}) },
			{ "Twerk", new ShopFurnitureBuy("vw_prop_casino_art_statue_02a", "Статуи", 11, new Dictionary<ItemId, int>()
			{
				{ ItemId.Ruby, 150 },
			}) },
			{ "Монахиня", new ShopFurnitureBuy("vw_prop_casino_art_statue_04a", "Статуи", 12, new Dictionary<ItemId, int>()
			{
				{ ItemId.Gold, 1000 },
			}) },

			{ "Paul Ridor", new ShopFurnitureBuy("hei_prop_drug_statue_01", "Фигурки", 13, new Dictionary<ItemId, int>()
			{
				{ ItemId.Ruby, 7 },
			}) },
			{ "Оскар", new ShopFurnitureBuy("ex_prop_exec_award_gold", "Фигурки", 14, new Dictionary<ItemId, int>()
			{
				{ ItemId.Emerald, 13 },
			}) },
			{ "Monkey King", new ShopFurnitureBuy("vw_prop_vw_pogo_gold_01a", "Фигурки", 15, new Dictionary<ItemId, int>()
			{
				{ ItemId.Iron, 100 },
			}) },
			{ "Авария", new ShopFurnitureBuy("xs_prop_trophy_goldbag_01a", "Фигурки", 16, new Dictionary<ItemId, int>()
			{
				{ ItemId.Ruby, 7 },
			}) },
			{ "Кубок FIFA", new ShopFurnitureBuy("sum_prop_ac_wifaaward_01a", "Фигурки", 17, new Dictionary<ItemId, int>()
			{
				{ ItemId.Emerald, 13 },
			}) },
			{ "Шампанское", new ShopFurnitureBuy("xs_prop_trophy_champ_01a", "Фигурки", 18, new Dictionary<ItemId, int>()
			{
				{ ItemId.Iron, 100 },
			}) },

			{ "Пальма", new ShopFurnitureBuy("prop_fbibombplant", "Растения", 19, new Dictionary<ItemId, int>()
			{
				{ ItemId.WoodMaple, 27 },
			}) },
			{ "Маленькое дерево", new ShopFurnitureBuy("prop_plant_int_01a", "Растения", 20, new Dictionary<ItemId, int>()
			{
				{ ItemId.WoodPine, 20 },
			}) },
			{ "Круглое дерево", new ShopFurnitureBuy("prop_plant_int_02b", "Растения", 21, new Dictionary<ItemId, int>()
			{
				{ ItemId.WoodPine, 20 },
			}) },
			{ "Папоротник", new ShopFurnitureBuy("prop_plant_int_03b", "Растения", 22, new Dictionary<ItemId, int>()
			{
				{ ItemId.WoodOak, 60 },
			}) },
			{ "Денежное дерево", new ShopFurnitureBuy("prop_plant_int_04b", "Растения", 23, new Dictionary<ItemId, int>()
			{
				{ ItemId.WoodMaple, 40 },
			}) },
			{ "Кактус", new ShopFurnitureBuy("vw_prop_casino_art_plant_12a", "Растения", 24, new Dictionary<ItemId, int>()
			{
				{ ItemId.WoodMaple, 27 },
			}) },

			{ "Ёлка", new ShopFurnitureBuy("prop_xmas_tree_int", "Ёлки", 25, new Dictionary<ItemId, int>()
			{
				{ ItemId.WoodPine, 60 },
			}) },
			{ "Бриллиантовая ёлка", new ShopFurnitureBuy("ch_prop_ch_diamond_xmastree", "Ёлки", 26, new Dictionary<ItemId, int>()
			{
				{ ItemId.Ruby, 150 },
			}) },

			{ "Счётная машинка", new ShopFurnitureBuy("bkr_prop_money_counter", "Драгоценности", 27, new Dictionary<ItemId, int>()
			{
				{ ItemId.Iron, 200 },
			}) },
			{ "Горка денег", new ShopFurnitureBuy("bkr_prop_moneypack_03a", "Драгоценности", 28, new Dictionary<ItemId, int>()
			{
				{ ItemId.Gold, 1000 },
			}) },
			{ "Гора денег", new ShopFurnitureBuy("ba_prop_battle_moneypack_02a", "Драгоценности", 29, new Dictionary<ItemId, int>()
			{
				{ ItemId.Gold, 3000 },
			}) },
			{ "Ящик денег", new ShopFurnitureBuy("ex_prop_crate_money_bc", "Драгоценности", 30, new Dictionary<ItemId, int>()
			{
				{ ItemId.Gold, 5000 },
			}) },
			{ "Ящик с золотом", new ShopFurnitureBuy("prop_ld_gold_chest", "Драгоценности", 31, new Dictionary<ItemId, int>()
			{
				{ ItemId.Gold, 300 },
			}) },
			{ "Тележка с золотом", new ShopFurnitureBuy("p_large_gold_s", "Драгоценности", 32, new Dictionary<ItemId, int>()
			{
				{ ItemId.Gold, 10000 },
			}) },
			{ "Кейс с деньгами", new ShopFurnitureBuy("prop_cash_case_02", "Драгоценности", 33, new Dictionary<ItemId, int>()
			{
				{ ItemId.Gold, 1700 },
			}) },

			{ "Ящик пива", new ShopFurnitureBuy("hei_heist_cs_beer_box", "Алкоголь", 34, new Dictionary<ItemId, int>()
			{
				{ ItemId.Iron, 50 },
			}) },
			{ "Романтический набор", new ShopFurnitureBuy("ba_prop_club_champset", "Алкоголь", 35, new Dictionary<ItemId, int>()
			{
				{ ItemId.WoodOak, 110 },
			}) },

			{ "Фигурная", new ShopFurnitureBuy("vw_prop_casino_art_vase_08a", "Вазы", 36, new Dictionary<ItemId, int>()
			{
				{ ItemId.Iron, 100 },
			}) },
			{ "Кувшин", new ShopFurnitureBuy("vw_prop_casino_art_vase_08a", "Вазы", 37, new Dictionary<ItemId, int>()
			{
				{ ItemId.Emerald, 13 },
			}) },
			{ "Дутая", new ShopFurnitureBuy("vw_prop_casino_art_vase_05a", "Вазы", 38, new Dictionary<ItemId, int>()
			{
				{ ItemId.Ruby, 7 },
			}) },
			{ "Симметричная", new ShopFurnitureBuy("apa_mp_h_acc_vase_06", "Вазы", 39, new Dictionary<ItemId, int>()
			{
				{ ItemId.WoodOak, 40 },
			}) },
			{ "Зеркальная", new ShopFurnitureBuy("apa_mp_h_acc_vase_05", "Вазы", 40, new Dictionary<ItemId, int>()
			{
				{ ItemId.Iron, 100 },
			}) },
			/*{ "цветок", new ShopFurnitureBuy("apa_mp_h_acc_plant_tall_01", "Вазы", 41, new Dictionary<ItemId, int>()
			{
				{ ItemId.Iron, 100 },
			}) },
			{ "цветок", new ShopFurnitureBuy("prop_fbibombplant", "Вазы", 40, new Dictionary<ItemId, int>()
			{
				{ ItemId.Iron, 100 },
			}) },
			{ "цветок", new ShopFurnitureBuy("prop_fbibombplant", "Вазы", 40, new Dictionary<ItemId, int>()
			{
				{ ItemId.Iron, 100 },
			}) },
			{ "свечи", new ShopFurnitureBuy("apa_mp_h_acc_candles_02", "Вазы", 40, new Dictionary<ItemId, int>()
			{
				{ ItemId.Iron, 100 },
			}) },
			{ "маска", new ShopFurnitureBuy("apa_mp_h_acc_dec_head_01", "Вазы", 40, new Dictionary<ItemId, int>()
			{
				{ ItemId.Iron, 100 },
			}) },
			{ "пива", new ShopFurnitureBuy("beerrow_local", "Вазы", 40, new Dictionary<ItemId, int>()
			{
				{ ItemId.Iron, 100 },
			}) },
			{ "дом кинотеатр", new ShopFurnitureBuy("hei_heist_str_avunitl_03", "Вазы", 40, new Dictionary<ItemId, int>()
			{
				{ ItemId.Iron, 100 },
			}) },
			{ "дом кинотеатр", new ShopFurnitureBuy("apa_mp_h_str_avunitl_01_b", "Вазы", 40, new Dictionary<ItemId, int>()
			{
				{ ItemId.Iron, 100 },
			}) },
			{ "статуя голова", new ShopFurnitureBuy("hei_prop_hei_bust_01", "Вазы", 40, new Dictionary<ItemId, int>()
			{
				{ ItemId.Iron, 100 },
			}) },
			{ "статуя оружие", new ShopFurnitureBuy("ch_prop_ch_trophy_gunner_01a", "Вазы", 40, new Dictionary<ItemId, int>()
			{
				{ ItemId.Iron, 100 },
			}) },
			{ "склад для оружия", new ShopFurnitureBuy("bkr_prop_gunlocker_01a", "Вазы", 40, new Dictionary<ItemId, int>()
			{
				{ ItemId.Iron, 100 },
			}) },*/
		};
        public static async Task Save(ServerBD db, int houseId)
        {
            try
            {
	            if (HouseFurnitures.ContainsKey(houseId))
	            {
		            await db.Furniture
			            .Where(f => f.Uuid == houseId)
			            .Set(f => f.Furniture, JsonConvert.SerializeObject(HouseFurnitures[houseId]))
			            .UpdateAsync();
	            }
            }
            catch (Exception e)
            {
                Log.Write($"Save Exception: {e.ToString()}");
            }
        }
        public static void Create(int id)
        {
            try
            {
                if (!HouseFurnitures.ContainsKey(id))
                {
                    using MySqlCommand cmd = new MySqlCommand
                    {
                        CommandText = "INSERT INTO `furniture`(`uuid`,`furniture`,`access`) VALUES (@val0,@val1,@val3)"
                    };
                    cmd.Parameters.AddWithValue("@val0", id);
                    cmd.Parameters.AddWithValue("@val1", JsonConvert.SerializeObject(new Dictionary<int, HouseFurniture>()));
                    cmd.Parameters.AddWithValue("@val3", JsonConvert.SerializeObject(new List<string>()));
                    MySQL.Query(cmd);
                }
            }
            catch (Exception e)
            {
                Log.Write($"Create Exception: {e.ToString()}");
            }
        }

        public static void NewFurniture(int id, string name)
        {
            try
            {
                if (!HouseFurnitures.ContainsKey(id)) 
                    Create(id);
                
                var houseFurniture = HouseFurnitures[id];
                
                int i = 0;
                while (houseFurniture.ContainsKey(i)) 
                    i++;
                
                var furn = new HouseFurniture(i, name, NameModels[name].Prop);
                houseFurniture.Add(i, furn);
                
                if (NameModels[name].Type.Equals("Хранилища")) 
                    Chars.Repository.RemoveAll($"furniture_{id}_{i}"); //оставалось видимо в хранилище, тестануть так
            }
            catch (Exception e)
            {
                Log.Write($"newFurniture Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("acceptEdit")]
        public void ClientEvent_acceptEdit(ExtPlayer player, float X, float Y, float Z, float XX, float YY, float ZZ)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (!player.IsCharacterData()) return;
                if (!sessionData.HouseData.Editing) return;
                sessionData.HouseData.Editing = false;
                var house = HouseManager.GetHouse(player, true);
                if (house == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoHome), 3000);
                    return;
                }
                Vector3 pos = new Vector3(X, Y, Z);
                if (player.Position.DistanceTo(pos) >= 6f)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MebelDomTooFar), 5000);
                    return;
                }
                if (!HouseFurnitures.ContainsKey(house.ID))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MebelError), 5000);
                    return;
                }

                var furnitures = HouseFurnitures[house.ID];
                foreach (HouseFurniture p in furnitures.Values)
                {
                    if (p != null && p.IsSet && p.Position != null && p.Position.DistanceTo(pos) <= 0.5f)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MebelTooNear), 3000);
                        return;
                    }
                }
                int id = sessionData.HouseData.EditID;
                furnitures[id].IsSet = true;
                Vector3 rot = new Vector3(XX, YY, ZZ);
                furnitures[id].Position = pos;
                furnitures[id].Rotation = rot;
                house.DestroyFurnitures();
                house.CreateAllFurnitures();
                house.IsFurnitureSave = true;
            }
            catch (Exception e)
            {
                Log.Write($"ClientEvent_acceptEdit Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("cancelEdit")]
        public void ClientEvent_cancelEdit(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                sessionData.HouseData.Editing = false;
            }
            catch (Exception e)
            {
                Log.Write($"ClientEvent_cancelEdit Exception: {e.ToString()}");
            }
        }
    }
}
