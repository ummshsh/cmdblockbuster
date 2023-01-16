using cmdblockbuster.Sound;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace BlockBuster
{
    internal class WpfSoundPlayer : ISoundPlayer
    {
        private readonly Dictionary<TetrisSound, MediaPlayer> loadedSounds = new Dictionary<TetrisSound, MediaPlayer>();

        public WpfSoundPlayer()
        {
            loadedSounds = new Dictionary<TetrisSound, MediaPlayer>();

            var names = Enum.GetNames(typeof(TetrisSound));
            foreach (var name in names)
            {
                TetrisSound key = Enum.Parse<TetrisSound>(name, true);
                var player = new MediaPlayer();
                player.Open(new Uri(Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + GetSoundPath(key)));
                loadedSounds.Add(key, player);
            }
        }

        public void PlaySound(object sender, TetrisSound sound)
        {
            DispatcherExtensions.BeginInvoke(Application.Current.Dispatcher, () =>
            {
                if (loadedSounds.TryGetValue(sound, out MediaPlayer player))
                {
                    player.Volume= 0.5;
                    player.Position = TimeSpan.Zero;
                    player.Play();
                }
            });
        }

        private string GetSoundPath(TetrisSound tetrisSound)
        {
            return tetrisSound switch
            {
                TetrisSound.Rotation => "./Sounds/Rotation.wav",
                TetrisSound.Movement => "./Sounds/Movement.wav",
                TetrisSound.LandingOnSurface => "./Sounds/Landing.wav",
                TetrisSound.TouchingWall => "./Sounds/TouchingWall.wav",
                TetrisSound.Locking => "./Sounds/Locking.wav",
                TetrisSound.LineClear => "./Sounds/LineClear.wav",
                TetrisSound.GameOver => "./Sounds/GameOver.wav",
                TetrisSound.Hold => "./Sounds/Hold.wav",
                _ => throw new FileNotFoundException("Sound not found")
            };
        }
    }
}
