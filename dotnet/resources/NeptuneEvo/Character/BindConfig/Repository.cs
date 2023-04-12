using Database;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using LinqToDB;
using NeptuneEvo.Character.BindConfig.Models;
using NeptuneEvo.Character.Config.Models;
using NeptuneEvo.Chars;
using NeptuneEvo.Core;
using NeptuneEvo.Players;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Localization;

namespace NeptuneEvo.Character.BindConfig
{
    class Repository
    {
        private static readonly nLog Log = new nLog("Core.Character.BindConfig.Repository");
        public static async Task<ConfigData> Load(ServerBD db, int uuid)
        {
            try
            {
                var bindConfigBD = await db.Bindcfg
                    .Where(bc => bc.Uuid == uuid)
                    .FirstOrDefaultAsync();

                if (bindConfigBD != null)
                {
                    var configData = new ConfigData();
                    try
                    {
                        configData.BindConfig = JsonConvert.DeserializeObject<Dictionary<object, int>>(bindConfigBD.BindSetting);
                    }
                    catch
                    {
                        configData.BindConfig = new Dictionary<object, int>();
                    }
                    configData.AnimFavorites = bindConfigBD.AnimFavorites;
                    configData.AnimBind = bindConfigBD.AnimBind;
                    try
                    {
                        configData.AdminOption = JsonConvert.DeserializeObject<AdminOption>(bindConfigBD.AdminOption);
                    }
                    catch
                    {
                        configData.AdminOption = new AdminOption();
                    }
                    return configData;
                }
                else
                {
                    db.Insert(new Bindcfgs
                    {
                        Uuid = uuid,
                        BindSetting = JsonConvert.SerializeObject(new Dictionary<object, int>()),
                        ChatSetting = "{}",
                        AnimFavorites = "[]",
                        AnimBind = "[0,0,0,0,0,0,0,0,0,0]",
                        AdminOption = JsonConvert.SerializeObject(new AdminOption()),
                    });
                }
            }
            catch (Exception e)
            {
                Log.Write($"Load Exception: {e.ToString()}");
            }
            return new ConfigData();
        }

        public static void Init(ExtPlayer player, ConfigData configData)
        {
            try
            {
                if (configData == null) return;
                Trigger.ClientEvent(player, "loadBindConfig", JsonConvert.SerializeObject(configData.BindConfig));
                Trigger.ClientEvent(player, "client.animationStore.animFavorites", configData.AnimFavorites);
                Trigger.ClientEvent(player, "client.animationStore.animBind", configData.AnimBind);
            }
            catch (Exception e)
            {
                Log.Write($"StartWork Exception: {e.ToString()}");
            }
        }
        public static void Update(ExtPlayer player, byte key, byte value)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) 
                    return;

                characterData.ConfigData.BindConfig[key] = value;
            }
            catch (Exception e)
            {
                Log.Write($"Update Exception: {e.ToString()}");
            }
        }
        public static async Task Save(ServerBD db, ExtPlayer player, int uuid)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null)
                    return;

                var configData = characterData.ConfigData;

                await db.Bindcfg
                     .Where(bc => bc.Uuid == uuid)
                     .Set(bc => bc.BindSetting, JsonConvert.SerializeObject (configData.BindConfig))
                     .Set(bc => bc.AnimFavorites, configData.AnimFavorites)
                     .Set(bc => bc.AnimBind, configData.AnimBind)
                     .Set(bc => bc.AdminOption, JsonConvert.SerializeObject(configData.AdminOption))
                     .UpdateAsync();
            }
            catch (Exception e)
            {
                Log.Write($"Save Exception: {e.ToString()}");
            }
        }
        public static void InitAdmin(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) 
                    return;

                var characterData = player.GetCharacterData();
                if (characterData == null)
                    return;
                else if (characterData.AdminLVL <= 0) 
                    return;

                var adminConfig = characterData.ConfigData.AdminOption;

                //player.Eval("mp.game.ui.setMinimapComponent(15, true, -1);");
                sessionData.AdminData.MuteCount = 0;
                sessionData.AdminData.KickCount = 0;
                sessionData.AdminData.JailCount = 0;
                sessionData.AdminData.WarnCount = 0;
                sessionData.AdminData.BansCount = 0;
                sessionData.AdminData.AclearCount = 0;
                sessionData.AdminData.LastPunishMinute = DateTime.Now.Minute;

                player.SetSharedData("ALVL", characterData.AdminLVL);

                if (adminConfig.RedName) 
                    player.SetSharedData("REDNAME", true);

                if (adminConfig.HideNick) 
                    player.SetSharedData("HideNick", true);

                if (adminConfig.ESP > 0) 
                    Trigger.ClientEvent(player, "setEspState", adminConfig.ESP);

                if (adminConfig.AGM) 
                    player.SetSharedData("AGM", true);

                adminConfig.Invisible = false;

                //if (adminConfig.Invisible)
                //   player.SetSharedData("INVISIBLE", true);

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.UniqAdminSettings), 3000);

                if (!Main.AllAdminsOnline.Contains (player)) 
                    Main.AllAdminsOnline.Add(player); 

                ReportSys.onAdminLoad(player);
            }
            catch (Exception e)
            {
                Log.Write($"CheckMyOptions Exception: {e.ToString()}");
            }
        }
        public static void DeleteAdmin(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null)
                    return;
                else if (characterData.AdminLVL <= 0)
                    return;

                characterData.AdminLVL = 0;

                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        await db.Characters
                            .Where(c => c.Uuid == characterData.UUID)
                            .Set(c => c.Adminlvl, 0)
                            .UpdateAsync();
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });

                player.ResetSharedData("ALVL");
                player.ResetSharedData("REDNAME");
                player.ResetSharedData("HideNick");
                Trigger.ClientEvent(player, "setEspState", 0);
                player.ResetSharedData("INVISIBLE");
                player.ResetSharedData("AGM");

                if (Main.AllAdminsOnline.Contains(player))
                    Main.AllAdminsOnline.Remove(player);

                ReportSys.onAdminUnLoad(player);

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.UniqAdminSettings), 3000);
            }
            catch (Exception e)
            {
                Log.Write($"DeleteAdmin Exception: {e.ToString()}");
            }
        }
    }
}
