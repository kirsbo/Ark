using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;

namespace Ark
{
    public static class SoundEffects
    {
        public enum EffectEnum
        {
            Archive,
            Click,
            Open,
            Open2,
            Thump
        }
        public static void Play(EffectEnum effect)
        {
            if (Properties.Settings.Default.PlaySounds == false) { return; }

            SoundPlayer soundPlayer = new SoundPlayer();
            switch (effect)
            {
                case EffectEnum.Archive:
                    soundPlayer.Stream = Properties.Resources.SoundArchive;
                    break;
                case EffectEnum.Click:
                    soundPlayer.Stream = Properties.Resources.SoundClick;
                    break;
                case EffectEnum.Open:
                    soundPlayer.Stream = Properties.Resources.SoundOpen;
                    break;
                case EffectEnum.Open2:
                    soundPlayer.Stream = Properties.Resources.SoundOpen2;
                    break;
                case EffectEnum.Thump:
                    soundPlayer.Stream = Properties.Resources.SoundThump;
                    break;
                default:
                    break;
            }
            
            soundPlayer.Play();
        }
    }
}
