const { app, BrowserWindow } = require('electron')

var edge = require('electron-edge-js')
var path = require('path')

function createWindow() {
  // Create the browser window.
  const win = new BrowserWindow({
    width: 800,
    height: 600,
    webPreferences: {
      nodeIntegration: true
    }
  })

  // and load the index.html of the app.
  win.loadFile('index.html')

  // Open the DevTools.
  win.webContents.openDevTools()

  var driver = edge.func({
    assemblyFile: path.resolve('./dll/TestDll.Net.dll'),
    typeName: 'TestDll.Net.MyAudioPlayer',
    methodName: 'Play'
  })

  // driver(null, function (err, result) {
  //   if (err) {
  //     throw err;
  //   } else {
  //     console.log(result);
  //   }
  // });
  
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
    console.log('Subscribed to .NET events. Unsubscribing in 7 seconds...');
    // setTimeout(function () {
    //   unsubscribe(null, function (error) {
    //     if (error) throw error;
    //     console.log('Unsubscribed from .NET events.');
    //     console.log('Waiting 5 seconds before exit to show that no more events are generated...')
    //     setTimeout(function () { }, 5000);
    //   });
    // }, 7000);
    console.log(unsubscribe);
  });

}

// This method will be called when Electron has finished
// initialization and is ready to create browser windows.
// Some APIs can only be used after this event occurs.
app.whenReady().then(createWindow)

// Quit when all windows are closed.
app.on('window-all-closed', () => {
  // On macOS it is common for applications and their menu bar
  // to stay active until the user quits explicitly with Cmd + Q
  if (process.platform !== 'darwin') {
    app.quit()
  }
})

app.on('activate', () => {
  // On macOS it's common to re-create a window in the app when the
  // dock icon is clicked and there are no other windows open.
  if (BrowserWindow.getAllWindows().length === 0) {
    createWindow()
  }
})

// In this file you can include the rest of your app's specific main process
// code. You can also put them in separate files and require them here.
