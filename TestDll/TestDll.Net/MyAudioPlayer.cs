using NAudio.Utils;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestDll.Net
{
    public class MyAudioPlayer
    {
        private WaveOutEvent woEvent;

        private System.Timers.Timer timer;

        public MyAudioPlayer()
        {
            woEvent = new WaveOutEvent();
            timer = new System.Timers.Timer(500);
        }

        /// <summary>
        /// param.interval = period interval time
        /// param.eventHandler = node.js event handler
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<object> Play(dynamic param)
        {
            var url = "https://pebblebus.blob.core.windows.net/pebble/love.mp3";
            url = "https://pebblebus.blob.core.windows.net/pebble/01.mp3";

            timer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
            {
                if (woEvent.PlaybackState == PlaybackState.Playing)
                {
                    var ms = woEvent.GetPositionTimeSpan().GetSongPositionString();
                    ((Func<object, Task<object>>)param.event_handler)(ms).Start();
                }
                else if (woEvent.PlaybackState == PlaybackState.Stopped)
                {
                    timer.Enabled = false;
                    timer.Stop();
                    ((Func<object, Task<object>>)param.stream_end)(e).Start();
                }
            };

            //woEvent.PlaybackStopped += (object sender, StoppedEventArgs e)=>
            //{
            //    timer.Enabled = false;
            //    timer.Stop();
            //    ((Func<object, Task<object>>)param.stream_end)(e).Start();
            //};

            woEvent.Init(new MediaFoundationReader(url));

            timer.Enabled = true;
            timer.Start();

            woEvent.Play();

            // Return a function that can be used by Node.js to 
            // unsubscribe from the event source.
            return (Func<object, Task<object>>)(async (dynamic data) => {
                timer.Enabled = false;
                return "OK";
            });
        }

        public async Task Stop()
        {
            //player.Pause();
        }
    }

    public static class TimeSpanExtensions
    {
        public static string GetSongPositionString(this TimeSpan timeSpan)
        {
            var minutes = (int)timeSpan.TotalMinutes;
            var seconds = timeSpan.Seconds;

            if (minutes >= 100)
            {
                return $"{minutes}:{seconds.ToString("00")}";
            }
            else
            {
                return $"{minutes.ToString("00")}:{seconds.ToString("00")}";
            }
        }

    }
    }
