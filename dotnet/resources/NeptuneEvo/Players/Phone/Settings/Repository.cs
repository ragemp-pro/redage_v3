using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using LinqToDB;
using NeptuneEvo.Character;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Core;
using NeptuneEvo.Handles;
using NeptuneEvo.Players.Phone.Messages.Models;
using NeptuneEvo.Players.Phone.Settings.Models;
using Redage.SDK;

namespace NeptuneEvo.Players.Phone.Settings
{
    public class Repository
    {
        public static int OnAddSim(ExtPlayer player, int newSim)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return -1;
            
            var sim = characterData.Sim;
            characterData.Sim = newSim;
            Trigger.ClientEvent(player, "client.charStore.Sim", characterData.Sim);
            Main.SimCards[characterData.Sim] = characterData.UUID;
                        
            Trigger.SetTask(async () =>
            {
                try
                {
                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    await db.Characters
                        .Where(c => c.Uuid == characterData.UUID)
                        .Set(c => c.Sim, characterData.Sim)
                        .UpdateAsync();
                
                    var phoneData = player.getPhoneData();
                    if (phoneData != null)
                    {
                        var messagesList = await Messages.Repository.Load(db, characterData.UUID, characterData.Sim);
                    
                        Trigger.ClientEvent(player, "client.phone.initMessages", Phone.Repository.MessagesJson(messagesList));
                    }
                }
                catch (Exception e)
                {
                    Debugs.Repository.Exception(e);
                }
            });
            
            if (sim != -1)
            {
                if (Main.SimCards.ContainsKey(sim))
                    Main.SimCards.TryRemove(sim, out _);
             
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Сим-карта была заменена.", 3000);
                
                return sim;   
            }
            return -1;
        }
        
        public static void OnRemoveSim(ExtPlayer player)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;

            var sim = characterData.Sim;
            
            if (sim == -1)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Сим карта не вставлена", 3000);
                return;
            }
            if (Chars.Repository.isFreeSlots(player, ItemId.SimCard, 1) != 0) 
                return; 
            
            if (Main.SimCards.ContainsKey(sim))
                Main.SimCards.TryRemove(sim, out _);
            
            characterData.Sim = -1;
            Trigger.ClientEvent(player, "client.charStore.Sim", characterData.Sim);
            
            Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.SimCard, 1, sim.ToString());

            Trigger.SetTask(async () =>
            {
                try
                {
                    await using var db = new ServerBD("MainDB");//В отдельном потоке

                    await db.Characters
                        .Where(c => c.Uuid == characterData.UUID)
                        .Set(c => c.Sim, characterData.Sim)
                        .UpdateAsync();
                }
                catch (Exception e)
                {
                    Debugs.Repository.Exception(e);
                }
            });
            
        }

        public static bool IsAir(SettingsData settings)
        {
            if (settings == null) 
                return true;

            return settings.IsAir;
        }
        
        public static void OnUpdateAvatar(ExtPlayer player, string avatar)
        {
            var phoneData = player.getPhoneData();
            if (phoneData == null || phoneData.Settings == null) 
                return;

            phoneData.Settings.Avatar = avatar;
        }
        
        public static void OnUpdateWallpaper(ExtPlayer player, string wallpaper)
        {
            var phoneData = player.getPhoneData();
            if (phoneData == null || phoneData.Settings == null) 
                return;

            phoneData.Settings.Wallpaper = wallpaper;
        }
        
        public static void OnUpdateAir(ExtPlayer player)
        {
            var phoneData = player.getPhoneData();
            if (phoneData == null || phoneData.Settings == null) 
                return;

            if (phoneData.Settings.IsAirAntiFlood > DateTime.Now)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Авиа режим можно менять раз в 5 минут", 3000);
                return;
            }
            phoneData.Settings.IsAirAntiFlood = DateTime.Now.AddMinutes(5);
            
            phoneData.Settings.IsAir = !phoneData.Settings.IsAir;
            
            if (phoneData.Settings.IsAir)
            {
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        var characterData = player.GetCharacterData();
                        if (characterData == null) 
                            return;
                    
                        await using var db = new ServerBD("MainDB");//В отдельном потоке
                    
                        var messagesList = await Messages.Repository.Load(db, characterData.UUID, characterData.Sim);

                        Trigger.ClientEvent(player, "client.phone.initMessages",
                            Phone.Repository.MessagesJson(messagesList));
                    }
                    catch (Exception e)
                    {
                        Debugs.Repository.Exception(e);
                    }
                });
            }
            else
            {
                var messagesList = new List<PhoneMessageListData>();
                
                Trigger.ClientEvent(player, "client.phone.initMessages",
                    Phone.Repository.MessagesJson(messagesList));
            }
        }
        public static void OnUpdateForbesVisible(ExtPlayer player)
        {
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
            var phoneData = player.getPhoneData();
            if (phoneData == null || phoneData.Settings == null) 
                return;

            phoneData.Settings.ForbesVisible = !phoneData.Settings.ForbesVisible;
            characterData.IsForbesShow = phoneData.Settings.ForbesVisible;
            Trigger.ClientEvent(player, "phone.notify", (int) DefaultNumber.RedAge, $"Статус приватности Forbes изменится со следующим обновлением ТОПа.", 5);
        }
        public static void OnUpdateBellId(ExtPlayer player, int bellId)
        {
            var phoneData = player.getPhoneData();
            if (phoneData == null || phoneData.Settings == null) 
                return;

            phoneData.Settings.BellId = bellId;
        }
        
        public static void OnUpdateSmsId(ExtPlayer player, int smsId)
        {
            var phoneData = player.getPhoneData();
            if (phoneData == null || phoneData.Settings == null) 
                return;

            phoneData.Settings.SmsId = smsId;
        }

        private static List<string> SoundsCall = new List<string>
        {
            "cloud/sound/iphone/calls/jingle.ogg",
            "cloud/sound/iphone/calls/kaby.ogg",
           "cloud/sound/iphone/calls/lastc.ogg",
            "cloud/sound/iphone/calls/mczali.ogg",
            "cloud/sound/iphone/calls/ngbaby.ogg",
            "cloud/sound/iphone/calls/call1.ogg", 
            "cloud/sound/iphone/calls/call2.ogg", 
            "cloud/sound/iphone/calls/beacon.ogg", 
            "cloud/sound/iphone/calls/chimes.ogg", 
            "cloud/sound/iphone/calls/circuit.ogg", 
            "cloud/sound/iphone/calls/constellation.ogg", 
            "cloud/sound/iphone/calls/cosmic.ogg", 
            "cloud/sound/iphone/calls/crystals.ogg", 
            "cloud/sound/iphone/calls/night-owl.ogg", 
            "cloud/sound/iphone/calls/playtime.ogg", 
            "cloud/sound/iphone/calls/presto.ogg", 
            "cloud/sound/iphone/calls/radar.ogg", 
            "cloud/sound/iphone/calls/radiate.ogg", 
            "cloud/sound/iphone/calls/ripples.ogg", 
            "cloud/sound/iphone/calls/sencha.ogg", 
            "cloud/sound/iphone/calls/signal.ogg", 
            "cloud/sound/iphone/calls/silk.ogg", 
            "cloud/sound/iphone/calls/slow-rise.ogg", 
            "cloud/sound/iphone/calls/stargaze.ogg",
            "cloud/sound/iphone/calls/summit.ogg", 
            "cloud/sound/iphone/calls/twinkle.ogg", 
            "cloud/sound/iphone/calls/uplift.ogg", 
            "cloud/sound/iphone/calls/waves.ogg", 
            "cloud/sound/iphone/calls/lida.ogg", 
            "cloud/sound/iphone/calls/snova.ogg",
            "cloud/sound/iphone/calls/bella.ogg", 
            "cloud/sound/iphone/calls/million.ogg", 
            "cloud/sound/iphone/calls/astral.ogg", 
            "cloud/sound/iphone/calls/auf.ogg", 
            "cloud/sound/iphone/calls/pes.ogg", 
            "cloud/sound/iphone/calls/bratva.ogg", 
            "cloud/sound/iphone/calls/chelovek.ogg", 
            "cloud/sound/iphone/calls/astronaut.ogg", 
            "cloud/sound/iphone/calls/gorish.ogg",
            "cloud/sound/iphone/calls/dragons.ogg", 
            "cloud/sound/iphone/calls/suicid.ogg", 
            "cloud/sound/iphone/calls/toxic.ogg", 
            "cloud/sound/iphone/calls/money.ogg",
            "cloud/sound/iphone/calls/everyday.ogg", 
            "cloud/sound/iphone/calls/federico.ogg",
            "cloud/sound/iphone/calls/solnce.ogg", 
            "cloud/sound/iphone/calls/allineed.ogg", 
            "cloud/sound/iphone/calls/mama.ogg", 
            "cloud/sound/iphone/calls/rampampam.ogg", 
            "cloud/sound/iphone/calls/squidgame.ogg", 
            "cloud/sound/iphone/calls/chick.ogg", 
            "cloud/sound/iphone/calls/loshad.ogg", 
            "cloud/sound/iphone/calls/bashmak.ogg", 
            "cloud/sound/iphone/calls/mamaz.ogg", 
            "cloud/sound/iphone/calls/dobrota.ogg", 
            "cloud/sound/iphone/calls/sponge.ogg", 
            "cloud/sound/iphone/calls/xanax.ogg",
            "cloud/sound/iphone/calls/ukraine.ogg",
            "cloud/sound/iphone/calls/antihype.ogg",
            "cloud/sound/iphone/calls/dedinsid.ogg", 
            "cloud/sound/iphone/calls/kurtec.ogg", 
            "cloud/sound/iphone/calls/minecraft.ogg", 
            "cloud/sound/iphone/calls/fortnite.ogg",
            "cloud/sound/iphone/calls/bones.ogg",
            "cloud/sound/iphone/calls/lipsislow.ogg",
            "cloud/sound/iphone/calls/sponge2.ogg",
            "cloud/sound/iphone/calls/internal.ogg",
            "cloud/sound/iphone/calls/pacani.ogg",
            "cloud/sound/iphone/calls/dora.ogg",
            "cloud/sound/iphone/calls/barbiesize.ogg",
            "cloud/sound/iphone/calls/acdc.ogg",
            "cloud/sound/iphone/calls/stiker.ogg",
            "cloud/sound/iphone/calls/bmth.ogg",
            "cloud/sound/iphone/calls/lada.ogg",
            "cloud/sound/iphone/calls/zakat.ogg",
            "cloud/sound/iphone/calls/drug.ogg",
            "cloud/sound/iphone/calls/yaneznayu.ogg",
            "cloud/sound/iphone/calls/batarejka.ogg",
            "cloud/sound/iphone/calls/nalune.ogg",
            "cloud/sound/iphone/calls/run.ogg",
            "cloud/sound/iphone/calls/queen.ogg",
            "cloud/sound/iphone/calls/yunglean.ogg",
            "cloud/sound/iphone/calls/aomine.ogg",
            "cloud/sound/iphone/calls/kish.ogg",
            "cloud/sound/iphone/calls/shipu4ka.ogg",
            "cloud/sound/iphone/calls/myatoy.ogg",
            "cloud/sound/iphone/calls/romashki.ogg",
            "cloud/sound/iphone/calls/leto.ogg",
            "cloud/sound/iphone/calls/spal.ogg",
            "cloud/sound/iphone/calls/malchik.ogg",
            "cloud/sound/iphone/calls/3x3.ogg",
            "cloud/sound/iphone/calls/milliontape.ogg",
            "cloud/sound/iphone/calls/gorodpod.ogg",
            "cloud/sound/iphone/calls/zvezda.ogg",
            "cloud/sound/iphone/calls/show.ogg",
            "cloud/sound/iphone/calls/daleko.ogg",
            "cloud/sound/iphone/calls/horosho.ogg",
            "cloud/sound/iphone/calls/chuvst.ogg",
            "cloud/sound/iphone/calls/industr.ogg",
            "cloud/sound/iphone/calls/abcdefu.ogg",
            "cloud/sound/iphone/calls/reshu.ogg",
            "cloud/sound/iphone/calls/arizona.ogg",
            "cloud/sound/iphone/calls/nihao.ogg",
            "cloud/sound/iphone/calls/err.ogg",
            "cloud/sound/iphone/calls/99mar.ogg",
            "cloud/sound/iphone/calls/99pr.ogg",
            "cloud/sound/iphone/calls/babyw.ogg"
        };

        public static void PlayCall(ExtPlayer player)
        {
            var phoneData = player.getPhoneData();
            if (phoneData == null || phoneData.Settings == null) 
                return;
            
            Sounds.PlayPlayer3d(player, SoundsCall[phoneData.Settings.BellId], new SoundData
            {
                volume = 0.15
            }); 
        }
        public static void StopCall(ExtPlayer player)
        {
            if (!player.IsCharacterData()) 
                return;
            
            Sounds.Stop3d(player); 
        }
        private static List<string> SoundsNotify = new List<string>
        {
            "cloud/sound/iphone/notify/aurora.ogg", 
            "cloud/sound/iphone/notify/chord.ogg", 
            "cloud/sound/iphone/notify/bamboo.ogg", 
            "cloud/sound/iphone/notify/circles.ogg", 
            "cloud/sound/iphone/notify/complete.ogg",
            "cloud/sound/iphone/notify/hello.ogg", 
            "cloud/sound/iphone/notify/input.ogg", 
            "cloud/sound/iphone/notify/keys.ogg", 
            "cloud/sound/iphone/notify/note.ogg", 
            "cloud/sound/iphone/notify/popcorn.ogg", 
            "cloud/sound/iphone/notify/synth.ogg", 
            "cloud/sound/iphone/notify/t1.ogg", 
            "cloud/sound/iphone/notify/t2.ogg"
        };

        public static void PlayNotify(ExtPlayer player)
        {
            var phoneData = player.getPhoneData();
            if (phoneData == null || phoneData.Settings == null) 
                return;
            
            Sounds.PlayPlayer3d(player, SoundsNotify[phoneData.Settings.SmsId], new SoundData
            {
                volume = 0.25
            }); 
        }
    }
}