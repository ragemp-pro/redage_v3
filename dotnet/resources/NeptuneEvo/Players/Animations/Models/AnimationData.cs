using System;
using NeptuneEvo.Core;

namespace NeptuneEvo.Players.Animations.Models
{
    public class AnimationData
    {
        public string AnimDict;
        public string AnimName;
        public int Flag;
        public string StopAnimDict = String.Empty;
        public string StopAnimName = String.Empty;
        public int StopFlag;
        public uint Attachment = 0;
        public string Sound;
        public SoundData SoundData = null;
        public float SoundRange = 10f;
    }
}