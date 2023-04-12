using Database;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using LinqToDB;
using NeptuneEvo.Accounts;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Core;
using NeptuneEvo.Functions;
using NeptuneEvo.GUI;
using NeptuneEvo.MoneySystem;
using NeptuneEvo.PedSystem.Pet.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Quests;
using NeptuneEvo.Quests.Models;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Localization;
using NeptuneEvo.Players.Phone.Messages.Models;

namespace NeptuneEvo.PedSystem.Pet
{
    class Repository
    {
        private static readonly nLog Log = new nLog("PedSystem.Pet");
        public static ConcurrentDictionary<ExtPed, PetData> PetsData = new ConcurrentDictionary<ExtPed, PetData>();

        public static void LoadInGame()
        {
            Timers.Start("PetHealth", 1000 * 60 * 8, () => PetHealth(), true);
            
            PedSystem.Repository.CreateQuest("a_m_y_soucent_03", new Vector3(268.109, -641.3529, 42.01984), 60.41674f, title: "~y~NPC~w~ Виталий\nПродавец питомцев", colShapeEnums: ColShapeEnums.PetShop);
            Main.CreateBlip(new Main.BlipData(141, LangFunc.GetText(LangType.Ru, DataName.PetSeller), new Vector3(268.109, -641.3529, 42.01984), 9, true));

            /*await using var db = new ServerBD("MainDB");//При старте сервера

            var inGamePets = await db.Pet
                .Where(p => p.InGame == true)
                .ToListAsync();

            foreach (var pet in inGamePets)
            {
                var position = JsonConvert.DeserializeObject<Vector3>(pet.Position);
                //var rotation = JsonConvert.DeserializeObject<Vector3>(pet.Rotation);
                var ped = (ExtPed) NAPI.Ped.CreatePed((uint)pet.Model, position, pet.Heading, dynamic: true, invincible: false, controlLocked: false, dimension: (uint)pet.Dimension);

                ped.SetSharedData("isPet", true);

                if (pet.Name.Length > 0)
                    ped.SetSharedData("petName", pet.Name);

                PetsData.TryAdd(ped, new PetData
                {
                    AutoId = pet.AutoId,
                    Name = pet.Name,
                    OwnerUUID = pet.OwnerUUID,
                    Model = (uint)pet.Model,
                    Health = pet.Health,
                    Death = pet.Death,
                    InGame = true
                });
                //ped.Controller = player;
                //ped.
                //ped.SetSharedData("FollowPet", player.Value + 1);
            }*/
        }

        private static void PetHealth()
        {
            foreach (var ped in PetsData.Keys.ToList())
            {
                Damage(ped, -1);
            }
        }
        [Interaction(ColShapeEnums.PetShop)]
        public static void PetShop(ExtPlayer player, int index)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null)
                return;

            var characterData = player.GetCharacterData();
            if (characterData == null)
                return;

            if (sessionData.CuffedData.Cuffed)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsCuffed), 3000);
                return;
            }
            else if (sessionData.DeathData.InDeath)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsDying), 3000);
                return;
            }
            else if (Main.IHaveDemorgan(player, true)) return;


            player.SelectQuest(new PlayerQuestModel("npc_petshop", 0, 0, false, DateTime.Now));
            Trigger.ClientEvent(player, "client.quest.open", index, "npc_petshop", 0, 0, 0);
        }
        public static void OpenPetShop(ExtPlayer player)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null)
                return;

            Trigger.ClientEvent(player, "client.petshop.open", JsonConvert.SerializeObject(PetsShop.Values));
            BattlePass.Repository.UpdateReward(player, 152);
        }
        public static async Task<List<Pets>> LoadPlayerPet(ServerBD db, int uuid)
        {
            try
            {
                if (!FunctionsAccess.IsWorking("pet")) return new List<Pets>();
                var pets = await db.Pet
                    .Where(p => p.InGame == false && p.OwnerUUID == uuid)
                    .OrderByDescending(p => p.AutoId)
                    .ToListAsync();

                return pets;
            }
            catch (Exception e)
            {
                Log.Write($"LoadPlayerPet Exception: {e.ToString()}");
            }
            return new List<Pets>();
        }

        public static void InitPlayerPet(ExtPlayer player, List<Pets> pets)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null)
                    return;

                var createPets = PetsData
                    .Where(p => p.Value.OwnerUUID == characterData.UUID)
                    .Select(p => p.Key)
                    .ToList();

                foreach (var ped in createPets)
                {
                    ped.Controller = player;

                    string petName = ped.GetSharedData<string>("petName");
                    ped.SetSharedData("petName", $"{petName} ({player.Value})");
                    Trigger.ClientEvent(player, "client.initPet", ped, PetsData[ped].Health, petName);
                }

                foreach (var pet in pets)
                {
                    if (pet.Health > 0)
                    {
                        var position = JsonConvert.DeserializeObject<Vector3>(pet.Position);
                        //var rotation = JsonConvert.DeserializeObject<Vector3>(pet.Rotation);
                        var petData = new PetData
                        {
                            AutoId = pet.AutoId,
                            Name = pet.Name,
                            OwnerUUID = pet.OwnerUUID,
                            Model = (uint)pet.Model,
                            Health = pet.Health,
                            Death = pet.Death,
                            InGame = false,
                            Position = position
                        };
                        
                        CreatePed(player, petData);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"LoadPlayerPet Exception: {e.ToString()}");
            }
        }

        private static List<uint> IsAttack = new List<uint>()
        {
            (uint)PedHash.Cat, (uint)PedHash.Rottweiler, (uint)PedHash.Husky, (uint)PedHash.Retriever, (uint)PedHash.Shepherd, (uint)PedHash.MountainLion, (uint)PedHash.Coyote, 3877461608
        };

        public static void AttackPlayerToPlayer(ExtPlayer player, ExtPlayer target)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null)
                return;

            if (!target.IsCharacterData())
                return;
            
            if (player == target)
                return;

            var createPets = PetsData
                .Where(p => p.Value.OwnerUUID == characterData.UUID)
                .Select(p => p.Key)
                .FirstOrDefault();

            if (createPets != null && createPets.Exists && IsAttack.Contains(createPets.Model))
            {
                Trigger.ClientEvent(player, "client.pet.attack", target.Value);
            }
        }

        public static void AttackPlayerToPet(ExtPed ped, ExtPlayer target)
        {
            if (!PetsData.ContainsKey(ped))
                return;

            else if (!target.IsCharacterData())
                return;

            var petData = PetsData[ped];

            var player = Main.GetPlayerByUUID(petData.OwnerUUID);

            if (player != null && player != target && IsAttack.Contains(ped.Model))
            {
                Trigger.ClientEvent(player, "client.pet.attack", target.Value);
            }
        }

        public static void AttacPetToPet(ExtPed ped, ExtPed target)
        {
            if (!PetsData.ContainsKey(ped))
                return;

            if (!PetsData.ContainsKey(target))
                return;

            var petData = PetsData[ped];

            var player = Main.GetPlayerByUUID(petData.OwnerUUID);
            if (player != null && ped != target && IsAttack.Contains(ped.Model))
            {
                Trigger.ClientEvent(player, "client.pet.attackPet", target.Value);
            }
        }
        public static void UnLoad(ExtPlayer player)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null)
                return;

            var createPets = PetsData
                .Where(p => p.Value.OwnerUUID == characterData.UUID)
                .Select(p => p.Key)
                .ToList();

            if (createPets.Count > 0)
            {
                var ListSavePet = new List<PetData>();

                foreach (var ped in createPets)
                {
                    if (!PetsData.ContainsKey(ped))
                        continue;

                    var petData = PetsData[ped];

                    if (petData.InGame)
                    {
                        ped.Controller = null;
                    }
                    else
                    {
                        petData.Position = ped.Position;
                        petData.Rotation = ped.Rotation;
                        petData.Heading = ped.Heading;
                        petData.Dimension = ped.Dimension;

                        ListSavePet.Add(petData);

                        ped.Delete();
                        PetsData.TryRemove(ped, out _);
                    }
                }
                Save(ListSavePet);
            }
        }
        private static void Save(List<PetData> ListSavePet)
        {
            Trigger.SetTask(async () =>
            {
                try
                {
                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    foreach (var petData in ListSavePet)
                    {
                        if (petData == null)
                            return;

                        await db.Pet
                            .Where(p => p.AutoId == petData.AutoId)
                            .Set(p => p.Name, petData.Name)
                            .Set(p => p.Health, petData.Health)
                            .Set(p => p.Death, petData.Death)
                            .Set(p => p.Position, JsonConvert.SerializeObject(petData.Position))
                            .Set(p => p.Heading, petData.Heading)
                            .Set(p => p.Dimension, Convert.ToInt32(petData.Dimension))
                            .UpdateAsync();
                    }
                }
                catch (Exception e)
                {
                    Log.Write($"Save Trigger.SetTask Exception: {e.ToString()}");
                }
            });
        }
        public static void Damage(ExtPed ped, int amount)
        {
            if (!PetsData.ContainsKey(ped))
                return;


            var petData = PetsData[ped];

            var player = Main.GetPlayerByUUID(petData.OwnerUUID);

            var sessionData = player.GetSessionData();
            if (0 > amount && sessionData != null && sessionData.IsSafeZone)
                return;

            petData.Health += amount;

            if (petData.Health > 100)
                petData.Health = 100;
            
            if (petData.Health <= 0)
            {
                petData.Health = 0;
                petData.Death = DateTime.Now;

                var ListSavePet = new List<PetData>();

                ListSavePet.Add(petData);

                Save(ListSavePet);

                ped.Delete();
                PetsData.TryRemove(ped, out _);

                if (player == null)
                    return;

                //Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PetDying), 10000);
                Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Ems,LangFunc.GetText(LangType.Ru, DataName.PetDying), DateTime.Now);
            }
            else if (player != null)
                Trigger.ClientEvent(player, "client.pet.health", petData.Health);
        }

        public static ExtPed GetPetToSpawn(int uuid, int autoId = -1)
        {

            var pet = PetsData
                .Where(p => p.Value.OwnerUUID == uuid && (autoId == -1 || autoId == p.Value.AutoId))
                .Select(p => p.Key)
                .FirstOrDefault();

            return pet;
        }

        public static Dictionary<int, PetShop> PetsShop = new Dictionary<int, PetShop>() 
        { 
            {0, new PetShop(200000, false, (uint)PedHash.Cat) }, 
            {1, new PetShop(42000, true, (uint)PedHash.Rottweiler) }, 
            {2, new PetShop(50000, true, (uint)PedHash.Husky) }, 
            {3, new PetShop(500000, false, (uint)PedHash.Poodle) },// 
            {4, new PetShop(420000, false, (uint)PedHash.Pug) }, 
            {5, new PetShop(800000, false, (uint)PedHash.Retriever) }, 
            {6, new PetShop(1500000, false, (uint)PedHash.Shepherd) }, 
            {7, new PetShop(15000, true, (uint)PedHash.Westy) }, 
            {8, new PetShop(600000, false, (uint)PedHash.Pig) }, 
            {9, new PetShop(1000000, false, (uint)PedHash.Boar) }, 
            {10, new PetShop(3000000, false, (uint)PedHash.MountainLion) }, 
            {11, new PetShop(100000, true, 3877461608) }, 
            {12, new PetShop(750000, false, (uint)PedHash.Coyote) }, 
 
        };

        private static ExtPed CreatePed(ExtPlayer player, PetData petData)
        {
            var ped = (ExtPed) NAPI.Ped.CreatePed(petData.Model, petData.Position, petData.Heading, dynamic: true, invincible: true, controlLocked: false, dimension: (uint)0);

            ped.SetSharedData("isPet", true);
            ped.SetSharedData("petName", $"{petData.Name} ({player.Value})");

            ped.Controller = player;
            Trigger.ClientEvent(player, "client.initPet", ped, 100, petData.Name);
            
            PetsData.TryAdd(ped, petData);
            
            return ped;
        }

        public static void Create(ExtPlayer player, int index)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null)
                return;

            var accountData = player.GetAccountData();
            if (accountData == null)
                return;

            Trigger.SetTask(async () =>
            {
                try
                {
                    var pet = PetsShop[index];

                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    var isPet = await db.Pet
                        .AnyAsync(p => p.OwnerUUID == characterData.UUID);

                    if (isPet)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.AlreadyHavePet), 5000);
                        return;
                    }
                    else if (!pet.isDonate && UpdateData.CanIChange(player, pet.Price, true) != 255) return;
                    else if (pet.isDonate && accountData.RedBucks < pet.Price)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NetRB), 3000);
                        return;
                    }

                    var autoId = await db.InsertWithInt32IdentityAsync(new Pets
                    {
                        Name = "",
                        OwnerUUID = characterData.UUID,
                        Model = pet.Ped,
                        Death = DateTime.MinValue,
                        Position = JsonConvert.SerializeObject(new Vector3()),
                        Rotation = JsonConvert.SerializeObject(new Vector3()),
                        Dimension = 0
                    });

                    NAPI.Task.Run(() =>
                    {
                        try
                        {
                            if (!pet.isDonate)
                            {
                                GameLog.Money($"player({characterData.UUID})", $"buyPet", pet.Price, $"buyPet({pet.Ped.ToString()})");
                                Wallet.Change(player, -pet.Price);
                            }
                            else
                            {
                                UpdateData.RedBucks(player, -pet.Price, msg: LangFunc.GetText(LangType.Ru, DataName.BoughtPet));
                            }

                            var petData = new PetData
                            {
                                AutoId = autoId,
                                Name = "",
                                OwnerUUID = characterData.UUID,
                                Model = pet.Ped,
                                Health = 100,
                                Death = DateTime.MinValue,
                                InGame = false,
                                Position = player.Position,
                                Rotation = player.Rotation,
                            };

                            CreatePed(player, petData);
                        }
                        catch (Exception e)
                        {
                            Log.Write($"Create NAPI.Trigger.SetTask Exception: {e.ToString()}");
                        }
                    });

                }
                catch (Exception e)
                {
                    Log.Write($"Create Trigger.SetTask Exception: {e.ToString()}");
                }
            });  
        }

        public static void RespawnPet(ExtPlayer player)
        {
            if (!FunctionsAccess.IsWorking("pet"))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                return;
            }
            var characterData = player.GetCharacterData();
            if (characterData == null)
                return;

            var createPets = PetsData
                .Count(p => p.Value.OwnerUUID == characterData.UUID);

            if (createPets > 0)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PetIsOk), 5000);
                return;
            }

            Trigger.SetTask(async () =>
            {
                try
                {
                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    var pet = await db.Pet
                        .Where(p => p.OwnerUUID == characterData.UUID && p.Health == 0)
                        .FirstOrDefaultAsync();

                    if (pet == null)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouHaveNoPet), 5000);
                        return;
                    }
                    //int price = Convert.ToInt32(TimeSpan.FromTicks(DateTime.Now.Ticks - pet.Death.Ticks).TotalSeconds) * PriceToSecond;
                    int price = 777;
                    if (UpdateData.CanIChange(player, price, true) != 255) return;

                    NAPI.Task.Run(() =>
                    {
                        Wallet.Change(player, -price);
                        //Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouPetIsOk, price), 10000);
                        Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Ems, LangFunc.GetText(LangType.Ru, DataName.YouPetIsOk, price), DateTime.Now); 

                        var position = new Vector3(300.2901, -578.11456, 43.26088);
                        //var rotation = JsonConvert.DeserializeObject<Vector3>(pet.Rotation);

                        var petData = new PetData
                        {
                            AutoId = pet.AutoId,
                            Name = pet.Name,
                            OwnerUUID = pet.OwnerUUID,
                            Model = (uint) pet.Model,
                            Health = 100,
                            Death = DateTime.MinValue,
                            InGame = false,
                            Position = position
                        };
                        
                        CreatePed(player, petData);
                    });
                }
                catch (Exception e)
                {
                    Log.Write($"RespawnPet Trigger.SetTask Exception: {e.ToString()}");
                }

            });
        }

        public static void AddBall(ExtPlayer player)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null)
                return;
            Commands.RPChat("do", player, $"Питомец принёс мячик.");
            if (Chars.Repository.isFreeSlots(player, ItemId.Ball, 1, send: false, Location: "fastSlots") != 0)
            {
                Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Ball, 1);
                return;
            }
            Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "fastSlots", ItemId.Ball, 1);
        }
        public static void OnToPos(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null)
                    return;

                var ped = GetPetToSpawn(characterData.UUID);

                if (ped == null)
                    return;
                if (!PetsData.ContainsKey(ped))
                    return;

                var petData = PetsData[ped];

                if (petData.OwnerUUID != characterData.UUID)
                    return;

                ped.Position = player.IsInVehicle ? player.Position.Around(4f) : player.Position;
            }
            catch (Exception e)
            {
                Log.Write($"OnToPos Exception: {e.ToString()}");
            }
        }
        public static void OnSniff(ExtPlayer player, ExtPlayer target)
        {
            if (!player.IsCharacterData())
                return;
            var targetСharacterData = target.GetCharacterData();
            if (targetСharacterData == null)
                return;

            string locationName = $"char_{targetСharacterData.UUID}";

            if (Chars.Repository.ItemsData.ContainsKey(locationName))
            {
                foreach (string Location in Chars.Repository.ItemsData[locationName].Keys)
                {
                    //if (Location == "fastSlots") continue;
                    foreach (var itemData in Chars.Repository.ItemsData[locationName][Location])
                    {
                        if (Location == "inventory" || Location == "fastSlots")
                        {
                            var item = itemData.Value;
                            if (Fractions.Police.IllegalsItems.ContainsKey(item.ItemId))
                            {
                                Commands.RPChat("do", player, LangFunc.GetText(LangType.Ru, DataName.PetSniffed));
                                return;
                            }
                        }
                    }
                }
            }
            Commands.RPChat("do", player, LangFunc.GetText(LangType.Ru, DataName.PetSniffs, target.Name));
        }
        public static void OnUpdateDim(ExtPlayer player, uint dimension)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null)
                    return;

                var ped = GetPetToSpawn(characterData.UUID);

                if (ped == null)
                    return;
                if (!PetsData.ContainsKey(ped))
                    return;

                var petData = PetsData[ped];

                if (petData.OwnerUUID != characterData.UUID)
                    return;

                ped.Dimension = dimension;
                ped.Position = player.Position;
                ped.Controller = player;
                Trigger.ClientEvent(player, "client.pet.unFreeze");
            }
            catch (Exception e)
            {
                Log.Write($"OnUpdateDim Exception: {e.ToString()}");
            }
        }
        /*public static void OpenPetMenu(Player player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null)
                    return;

                Menu menu = new Menu("pets", false, false);
                menu.Callback = callback_pets;

                Menu.Item menuItem = new Menu.Item("header", Menu.MenuItem.Header);
                menuItem.Text = "Мои машины";
                menu.Add(menuItem);

                int index = -1;
                foreach (var ped in characterData.Peds)
                {
                    index++;

                    if (!PetsData.ContainsKey(ped))
                        continue;

                    var petData = PetsData[ped];
                    menuItem = new Menu.Item(index.ToString(), Menu.MenuItem.Button);
                    menuItem.Text = petData.Name;
                    menu.Add(menuItem);
                }

                menuItem = new Menu.Item("back", Menu.MenuItem.Button);
                menuItem.Text = LangFunc.GetText(LangType.Ru, DataName.back);
                menu.Add(menuItem);

                menuItem = new Menu.Item("close", Menu.MenuItem.Button);
                menuItem.Text = LangFunc.GetText(LangType.Ru, DataName.Close);
                menu.Add(menuItem);

                menu.Open(player);
            }
            catch (Exception e)
            {
                Log.Write($"OpenMyCarsMenu Exception: {e.ToString()}");
            }
        }*/
        public static void OpenPetMenu(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null)
                    return;

                var characterData = player.GetCharacterData();
                if (characterData == null)
                    return;

                /*MenuManager.Close(player);
                if (item.ID == "close") return;
                else if (item.ID == "back")
                {
                    Main.OpenPlayerMenu(player);
                    return;
                }

                var ped = characterData.Peds[Convert.ToInt32(item.ID)];*/
                var ped = GetPetToSpawn(characterData.UUID);

                if (ped == null)
                    return;
                else if (!PetsData.ContainsKey(ped))
                    return;

                var petData = PetsData[ped];

                if (petData.OwnerUUID != characterData.UUID)
                    return;

                sessionData.SelectPed = ped;

                Trigger.ClientEvent(player, "openInput", LangFunc.GetText(LangType.Ru, DataName.Name), LangFunc.GetText(LangType.Ru, DataName.InputPetName), 36, "make_ped_name");

            }
            catch (Exception e)
            {
                Log.Write($"OpenPetMenu Exception: {e.ToString()}");
            }
        }
        public static void UpdateName(ExtPlayer player, ExtPed ped, string name)
        {
            try
            {
                if (ped == null)
                    return;
                else if (!PetsData.ContainsKey(ped))
                    return;

                var characterData = player.GetCharacterData();

                if (characterData == null)
                    return;

                var petData = PetsData[ped];

                if (petData.OwnerUUID != characterData.UUID)
                    return;

                petData.Name = name;

                ped.SetSharedData("petName", $"{name} ({player.Value})");
                Trigger.ClientEvent(player, "client.pet.nameChange", name);
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PetNameChanged, name), 5000);
            }
            catch (Exception e)
            {
                Log.Write($"UpdateName Exception: {e.ToString()}");
            }
        }
        public static void Sell(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null)
                    return;

                var characterData = player.GetCharacterData();
                if (characterData == null)
                    return;

                var ped = GetPetToSpawn(characterData.UUID);

                if (ped == null)
                    return;
                if (!PetsData.ContainsKey(ped))
                    return;

                var petData = PetsData[ped];

                if (petData.OwnerUUID != characterData.UUID)
                    return;

                var pet = PetsShop.Values
                    .FirstOrDefault(p => p.Ped == petData.Model);

                if (pet == null)
                    return;

                sessionData.SelectPed = ped;

                if (pet.isDonate)
                    Trigger.ClientEvent(player, "openDialog", "sell_pet", LangFunc.GetText(LangType.Ru, DataName.PetSellRb, Wallet.Format(pet.Price / 2)));
                else
                    Trigger.ClientEvent(player, "openDialog", "sell_pet", LangFunc.GetText(LangType.Ru, DataName.PetSellMoney, Wallet.Format(pet.Price / 2)));

            }
            catch (Exception e)
            {
                Log.Write($"callback_cars Exception: {e.ToString()}");
            }
        }
        public static void ConfirmSell(ExtPlayer player, ExtPed ped)
        {
            try
            {
                if (ped == null)
                    return;
                if (!PetsData.ContainsKey(ped))
                    return;

                var characterData = player.GetCharacterData();
                if (characterData == null)
                    return;

                var accountData = player.GetAccountData();
                if (accountData == null)
                    return;

                var petData = PetsData[ped];

                if (petData.OwnerUUID != characterData.UUID)
                    return;

                var pet = PetsShop
                    .Where(p => p.Value.Ped == petData.Model)
                    .Select(p => p.Value)
                    .FirstOrDefault();

                if (pet == null)
                    return;

                if (!pet.isDonate)
                {
                    GameLog.Money($"player({characterData.UUID})", $"sellPet", Convert.ToInt32(pet.Price / 2), $"sellPet({pet.Ped.ToString()})");
                    Wallet.Change(player, Convert.ToInt32(pet.Price / 2));
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PetGivenRb, Convert.ToInt32(pet.Price / 2)), 5000);
                }
                else
                {
                    UpdateData.RedBucks(player, Convert.ToInt32(pet.Price / 2), msg: LangFunc.GetText(LangType.Ru, DataName.PetSell));
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PetGivenMoney, Convert.ToInt32(pet.Price / 2)), 5000);
                }


                ped.Delete();
                PetsData.TryRemove(ped, out _);

                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        db.Pet
                            .Where(p => p.AutoId == petData.AutoId)
                            .Delete();
                    }
                    catch (Exception e)
                    {
                        Log.Write($"ConfirmSell Task Exception: {e.ToString()}");
                    }
                });

            }
            catch (Exception e)
            {
                Log.Write($"ConfirmSell Exception: {e.ToString()}");
            }

        }

        private static IReadOnlyDictionary<ItemId, int> PetFood = new Dictionary<ItemId, int>()
        {
            { ItemId.Crisps, 30 },
            { ItemId.Pizza, 70 },
            { ItemId.Burger, 50 },
            { ItemId.HotDog, 40 },
            { ItemId.Sandwich, 45 },
        };

        public static void SetEat(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null)
                    return;

                var characterData = player.GetCharacterData();
                if (characterData == null)
                    return;

                var ped = GetPetToSpawn(characterData.UUID);

                if (ped == null)
                    return;
                else if (!PetsData.ContainsKey(ped))
                    return;

                var petData = PetsData[ped];

                if (petData.OwnerUUID != characterData.UUID)
                    return;

                else if (ped.Position.DistanceTo(player.Position) > 3f)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PetTooFar), 5000);
                    return;
                }
                else if (petData.Health >= 100)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PetNoHungry), 5000);
                    return;
                }

                ItemStruct ItemStruct = null;

                foreach(var ItemId in PetFood.Keys)
                {
                    ItemStruct = Chars.Repository.isItem(player, "inventory", ItemId);
                    
                    if (ItemStruct != null)
                        break;
                }

                if (ItemStruct == null || ItemStruct.Item == null || ItemStruct.Index == -1 || !PetFood.ContainsKey(ItemStruct.Item.ItemId))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouHaveNoFood), 5000);
                    return;
                }
                else if (DateTime.Now < sessionData.TimingsData.NextEat)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantDoThisNow), 3000);
                    return;
                }
                var eatData = PetFood[ItemStruct.Item.ItemId];
                sessionData.TimingsData.NextEat = DateTime.Now.AddSeconds(10);

                Chars.Repository.RemoveIndex(player, ItemStruct.Location, ItemStruct.Index);

                sessionData.ActiveWeap = new ItemStruct("", -1, null);


                Trigger.PlayAnimation(player, "timetable@gardener@lawnmow@", "base", 39);
                // Trigger.ClientEventInRange(player.Position, 250f, "PlayAnimToKey", player, false, "eatpet");

                if (ItemStruct.Item.ItemId == ItemId.Burger) Attachments.AddAttachment(player, Attachments.AttachmentsName.Burger);
                else if (ItemStruct.Item.ItemId == ItemId.HotDog) Attachments.AddAttachment(player, Attachments.AttachmentsName.HotDog);
                else if (ItemStruct.Item.ItemId == ItemId.Pizza) Attachments.AddAttachment(player, Attachments.AttachmentsName.Pizza);
                else if (ItemStruct.Item.ItemId == ItemId.Sandwich) Attachments.AddAttachment(player, Attachments.AttachmentsName.Sandwich);
                else if (ItemStruct.Item.ItemId == ItemId.Crisps) Attachments.AddAttachment(player, Attachments.AttachmentsName.Crisps);

                Timers.StartOnce(5000, () =>
                {
                    try
                    {
                        if (!player.IsCharacterData()) return;

                        if (ItemStruct.Item.ItemId == ItemId.Burger) Attachments.RemoveAttachment(player, Attachments.AttachmentsName.Burger);
                        else if (ItemStruct.Item.ItemId == ItemId.HotDog) Attachments.RemoveAttachment(player, Attachments.AttachmentsName.HotDog);
                        else if (ItemStruct.Item.ItemId == ItemId.Pizza) Attachments.RemoveAttachment(player, Attachments.AttachmentsName.Pizza);
                        else if (ItemStruct.Item.ItemId == ItemId.Sandwich) Attachments.RemoveAttachment(player, Attachments.AttachmentsName.Sandwich);
                        else if (ItemStruct.Item.ItemId == ItemId.Crisps) Attachments.RemoveAttachment(player, Attachments.AttachmentsName.Crisps);
                        Trigger.StopAnimation(player);
                        player.Position.Z += 1.12f;
                        Damage(ped, eatData);
                        Commands.RPChat("sme", player, $"покормил" + (characterData.Gender ? "" : "а") + LangFunc.GetText(LangType.Ru, DataName.Pets));
                    }
                    catch (Exception e)
                    {
                        Log.Write($"UseSmoke Task#3 Exception: {e.ToString()}");
                    }
                }, true);
            }
            catch (Exception e)
            {
                Log.Write($"callback_cars Exception: {e.ToString()}");
            }
        }
    }
}
