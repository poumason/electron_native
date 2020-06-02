using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace TestDll.UWP
{
    public class MyAudioPlayer
    {
        private MediaPlayer player;

        public MyAudioPlayer()
        {
            player = new MediaPlayer();
            player.AutoPlay = true;
        }

        public async Task<object> Play(object param)
        {
            player.Source = MediaSource.CreateFromUri(new Uri("https://pebblebus.blob.core.windows.net/pebble/love.mp3"));
            player.Play();
            return "UWP";
        }

        public async Task Stop()
        {
            player.Pause();
        }
    }
}