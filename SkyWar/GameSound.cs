using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using EleCho.ScratchGame;
using SkyWar.Properties;

namespace SkyWar
{
    internal class GameSound : GameSprite
    {
        static WaveFileReader backgroundMusic = new WaveFileReader(Resources.game_music);

        public GameSound()
        {
            backgroundSoundPlayer = new WaveOut();
            backgroundSoundPlayer.Init(backgroundMusic);
            backgroundSoundPlayer.PlaybackStopped += BackgroundSoundPlayer_PlaybackStopped;
        }

        private void BackgroundSoundPlayer_PlaybackStopped(object? sender, StoppedEventArgs e)
        {
            backgroundMusic.Seek(0, SeekOrigin.Begin);
            backgroundSoundPlayer.Play();
        }

        WaveOut backgroundSoundPlayer;

        public override void Load()
        {
            backgroundSoundPlayer.Play();
        }
    }
}
