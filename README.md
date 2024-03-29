# electron_native

利用 [electron-edge-js](https://github.com/agracio/electron-edge-js) 實現 Node.js 與 .NET dll 相互溝通。

## 測試
```
git clone git@github.com:poumason/electron_native.git
cd electron_native_windows
npm install
npm start
```

## 開發
#### TestDll.Net
基於 .NET Framework 4.7 開發，並引入 NAudio 播放歌曲，定時更新歌曲的 duration 與 position 到 Node.js
```
namespace TestDll.Net
{
    public class MyAudioPlayer
    {
        public async Task<object> Play(dynamic param)
        {
            var url = "https://pebblebus.blob.core.windows.net/pebble/01.mp3";

            var reader = new MediaFoundationReader(url);
            woEvent.Init(reader);

            timer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
            {
                if (woEvent.PlaybackState == PlaybackState.Playing)
                {
                    var ms = woEvent.GetPositionTimeSpan().GetSongPositionString();
                    ((Func<object, Task<object>>)param.event_handler)(new TrackTimespan
                    {
                        Position = ms,
                        Duration = reader.TotalTime.GetSongPositionString()
                    }).Start();
                }
                else if (woEvent.PlaybackState == PlaybackState.Stopped)
                {
                    timer.Enabled = false;
                    timer.Stop();
                    ((Func<object, Task<object>>)param.stream_end)(e).Start();
                }
            };
            
            timer.Enabled = true;
            timer.Start();

            woEvent.Play();

            return (Func<object, Task<object>>)(async (dynamic data) =>
            {
                timer.Enabled = false;
                return "OK";
            });
        }
    }
}
```
## main.js
使用 [electron-edge-js](https://github.com/agracio/electron-edge-js) 載入 TestDll.Net.dll，並宣告要使用的 method。
定義 callback 給 .NET 端送資料回到 Node.js。

```
var edge = require('electron-edge-js')
var path = require('path')

var driver = edge.func({
    assemblyFile: path.resolve('./dll/TestDll.Net.dll'),
    typeName: 'TestDll.Net.MyAudioPlayer',
    methodName: 'Play'
})

driver({
    interval: 2000,
    event_handler: function (data, cb) {
      console.log("duration: %s, position: %s", data.Duration, data.Position);
      cb();
    },
    stream_end: function (data, cb) {
      console.log("stream end");
      cb();
    }
}, function (error, unsubscribe) {
    if (error) throw error;
    console.log(unsubscribe);
});
```
