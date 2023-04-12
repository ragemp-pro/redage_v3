using System;
using GTANetworkAPI;
using NeptuneEvo.Utils.Redis.Models;
using Newtonsoft.Json;
using Redage.SDK;
using StackExchange.Redis;

namespace NeptuneEvo.Utils.Redis
{
    public class Repository
    {
        private static nLog Log = new nLog("Utils.Redis");

        private static ConnectionMultiplexer RedisInstance;

        private static string ConfirmEmail = $"{Main.ServerNumber}_confirmEmail";
        private static string Global = $"{Main.ServerNumber}_global";
        
        public static void Init()
        {
            
            try
            {
                var configurationOptions = new ConfigurationOptions
                {
                    EndPoints = { "127.0.0.1:6379" },
                    Password = ""
                };
                RedisInstance = ConnectionMultiplexer.Connect(configurationOptions);

                var sub = RedisInstance.GetSubscriber();

                sub.Subscribe(ConfirmEmail, ConfirmEmailHandler);
                sub.Subscribe(Global, GlobalHandler);
            
                Log.Write($"Start: All ok. For server: {Main.ServerNumber}", nLog.Type.Info);
            }
            catch (Exception e)
            {
                Log.Write($"StartWork Exception: {e.ToString()}");
            }
            
        }

        private static void ConfirmEmailHandler(RedisChannel channel, RedisValue message)
        {
            try
            {
                var confirmEmailVerification = JsonConvert.DeserializeObject<ConfirmEmailVerification>(message);
            
                Accounts.Email.Registration.Repository.VerificationConfirm(confirmEmailVerification.Hash, confirmEmailVerification.Ga);
                Accounts.Email.Confirmation.Repository.VerificationConfirm(confirmEmailVerification.Hash, confirmEmailVerification.Ga);
            }
            catch (Exception e)
            {
                Log.Write($"ConfirmEmailHandler Exception: {e.ToString()}");
            }
        }
        private static void GlobalHandler(RedisChannel channel, RedisValue message)
        {
            NAPI.Chat.SendChatMessageToAll(message);
        }
    }
}