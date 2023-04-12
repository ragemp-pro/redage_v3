using GTANetworkAPI;
using NeptuneEvo.Handles;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NeptuneEvo.Character.BindConfig
{
    class Events : Script
    {
        private static readonly nLog Log = new nLog("Core.Character.BindConfig.Events");

        [RemoteEvent("bindConfigSave")]
        public void Save(ExtPlayer player, byte key, byte value)
        {
            Repository.Update(player, key, value);
        }
    }
}
