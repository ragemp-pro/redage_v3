using Database;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Chars;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using NeptuneEvo.Character;

namespace NeptuneEvo.Functions
{
    class FunctionsAccess : Script
    {
        private static readonly nLog Log = new nLog("Functions.CommandsAccess");

        private static Dictionary<string, bool> SystemState = new Dictionary<string, bool>()
        {
            { "DeleteCharacter", true },
            { "metro", false },
            { "PayDayBonus", false },
            //{"ClothesShop", false }
        };
        
        public void UpdateSystemState()
        {
            using (var db = new ConfigBD("ConfigDB"))
            {
                Dictionary<string, bool> _SystemState = new Dictionary<string, bool>();
                List<Systemstates> SystemStateList = db.Systemstate.ToList();
                foreach (Systemstates _systemState in SystemStateList)
                {
                    _SystemState.Add(_systemState.Name, _systemState.Toggle);
                }
                SystemState = _SystemState;
            }
        }
        [Command(AdminCommands.Refreshsystemstate)]
        public void CMD_Refreshclothes(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                else if (!CommandsAccess.CanUseCmd(player, AdminCommands.Refreshsystemstate)) return;
                UpdateSystemState();
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы обновили .", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_Refreshclothes Exception: {e.ToString()}");
            }
        }

        [Command(AdminCommands.Enablefunc)]
        public void CMD_Enablefunc(ExtPlayer player, string name, bool toogled)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                else if (!CommandsAccess.CanUseCmd(player, AdminCommands.Enablefunc)) return;
                if (!SystemState.ContainsKey(name)) SystemState.Add(name, toogled);
                else SystemState[name] = toogled;
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы изменили доступ к функции {name} на {toogled}", 10000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_Enablefunc Exception: {e.ToString()}");
            }
        }

        
        // Если система отключена, то у неё должно стоять false
        // Если система включена, то true
        public static bool IsWorking(string name)
        {
            try
            {
                if (!SystemState.ContainsKey(name)) return true; // Если в списке переключенных систем не находится нужная нам, значит та, что мы запрашиваем - точно работает, возвращаем true
                return SystemState[name]; // Если она есть в списке, возвращаем значение из списка.
            }
            catch (Exception e)
            {
                Log.Write($"IsWorking Exception: {e.ToString()}");
                return false; // Если вдруг среди ясного дня прогремит гром, то на всякий случай возвращаем, что система отключена
            }
        }
    }
}
