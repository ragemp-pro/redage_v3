using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Accounts;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NeptuneEvo.Character.Config
{
    class Events : Script
    {
        private static readonly nLog Log = new nLog("Core.Character.Config");

        [RemoteEvent("chatConfigSave")]
        public void Save(ExtPlayer player, string chatData)
        {
            try
            {
                Repository.Update(player, chatData);
            }
            catch (Exception e)
            {
                Log.Write($"bindConfigSave Exception: {e.ToString()}");
            }
        }
    }
}
