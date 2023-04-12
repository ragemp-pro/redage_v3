namespace NeptuneEvo.Chars.Models
{
    /// <summary>
    /// Данные пользователя
    /// </summary>
    public class FireworkData
    {
        public string ParticleName { get; set; }
        public string AnimName { get; set; }
        public int EffectTime { get; set; }

        public FireworkData(string particleName, string animName, int effectTime)
        {
            ParticleName = particleName;
            AnimName = animName;
            EffectTime = effectTime;
        }
    }
}