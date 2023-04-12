using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.Character.Config.Models
{
    public class ChatData
    {
        public bool Timestamp { get; set; } = false; // 0
        public bool ChatShadow { get; set; } = true; // 0
        public byte Fontsize { get; set; } = 16; // 16
        public byte ChatOpacity { get; set; } = 100; // 0
        public byte Chatalpha { get; set; } = 1; // 0
        public byte Pagesize { get; set; } = 10; // 10
        public byte Widthsize { get; set; } = 100; // 50
        public byte Transition { get; set; } = 0; // 0
        public byte WalkStyle { get; set; } = 0; // 0
        public byte FacialEmotion { get; set; } = 0; // 0
        public bool Deaf { get; set; } = false; // 0
        public bool TagsHead { get; set; } = true; // 0
        public bool HudToggled { get; set; } = true; // 0
        public bool HudStats { get; set; } = true; // 0
        public bool HudSpeed { get; set; } = true; // 0
        public bool HudOnline { get; set; } = true; // 0
        public bool HudLocation { get; set; } = true; // 0
        public bool HudKey { get; set; } = true; // 0
        public bool HudMap { get; set; } = true; // 0
        public bool HudCompass { get; set; } = true; //
        public byte VolumeInterface { get; set; } = 100; // 100
        public byte VolumeQuest { get; set; } = 50; // 100
        public byte VolumeAmbient { get; set; } = 50; // 100
        public byte VolumePhoneRadio { get; set; } = 50; // 100
        public byte VolumeVoice { get; set; } = 100; // 100
        public byte VolumeRadio { get; set; } = 70; // 100
        public byte DistancePlayer { get; set; } = 50; // 100
        public byte DistanceVehicle { get; set; } = 50; // 100
        public bool cPToggled { get; set; } = false; // 0
        public byte cPWidth { get; set; } = 2; // 100
        public byte cPGap { get; set; } = 2; // 100
        public bool cPDot { get; set; } = true; // 100
        public byte cPThickness { get; set; } = 0; // 100
        public byte cPColorR { get; set; } = 255; // 255
        public byte cPColorG { get; set; } = 255; // 255
        public byte cPColorB { get; set; } = 255; // 255
        public byte cPOpacity { get; set; } = 9; // 100
        public bool cPCheck { get; set; } = true; // 100
        public bool FirstMute { get; set; } = false; // 0
        public bool APunishments { get; set; } = false; // 0
        public bool CircleVehicle { get; set; } = true; // 0
        public byte cEfValue { get; set; } = 0; // 0
        public byte notifCount { get; set; } = 2; // 0
        public float RadioVolume { get; set; } = 15; // 0player.GetSessionData();
        public bool hitPoint { get; set; } = false; // 0
    }
}
