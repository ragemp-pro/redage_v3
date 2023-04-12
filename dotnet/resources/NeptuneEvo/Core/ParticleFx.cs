using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Core
{
    class ParticleFxColor
    {
        public int r { get; set; } = 0;
        public int g { get; set; } = 0;
        public int b { get; set; } = 0;
    }
    class ParticleFxData
    {
        public float scale { get; set; } = 1f;
        public ParticleFxColor rgb { get; set; } = new ParticleFxColor();
        public float xOffset { get; set; } = 0f;
        public float yOffset { get; set; } = 0f;
        public float zOffset { get; set; } = 0f;
        public float xRot { get; set; } = 0f;
        public float yRot { get; set; } = 0f;
        public float zRot { get; set; } = 0f;

        public ParticleFxData(float scale = 1f, float xOffset = 0f, float yOffset = 0f, float zOffset = 0f, float xRot = 0f, float yRot = 0f, float zRot = 0f)
        {
            this.scale = scale;
            this.xOffset = xOffset;
            this.yOffset = yOffset;
            this.zOffset = zOffset;
            this.xRot = xRot;
            this.yRot = yRot;
            this.zRot = zRot;
        }
    }
    class ParticleFx : Script
    {        
        public static void PlayFXonPos(Vector3 pos, float range, float xPos, float yPos, float zPos, string fxLib, string fxName, int dellTime = 1000, ParticleFxData data = null)
        {
            Trigger.ClientEventInRange(pos, range, "client.playFXonPos", xPos, yPos, zPos, fxLib, fxName, dellTime, data);
        }
        public static void PlayFXonPosOnce(Vector3 pos, float range, float xPos, float yPos, float zPos, string fxLib, string fxName, int dellTime = 1000, ParticleFxData data = null)
        {
            Trigger.ClientEventInRange(pos, range, "client.playFXonPosOnce", xPos, yPos, zPos, fxLib, fxName, dellTime, data);
        }
        public static void PlayFXonEntity(Vector3 pos, float range, Entity entity, string fxLib, string fxName, int dellTime = 1000, ParticleFxData data = null)
        {
            Trigger.ClientEventInRange(pos, range, "client.playFXonEntity", entity, fxLib, fxName, dellTime, data);
        }
        public static void PlayFXonEntityOnce(Vector3 pos, float range, Entity entity, string fxLib, string fxName, ParticleFxData data = null)
        {
            Trigger.ClientEventInRange(pos, range, "client.playFXonEntityOnce", entity, fxLib, fxName, data);
        }
        public static void PlayFXonEntityBone(Vector3 pos, float range, Entity entity, int boneName, string fxLib, string fxName, int dellTime = 1000, ParticleFxData data = null)
        {
            Trigger.ClientEventInRange(pos, range, "client.playFXonEntityBone", entity, boneName, fxLib, fxName, dellTime, data);
        }
    }
}