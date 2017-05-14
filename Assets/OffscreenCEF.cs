using Aleab.CefUnity.Structs;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using Xilium.CefGlue;


namespace Aleab.CefUnity
{
    public class OffscreenCEF : MonoBehaviour
    {
        private bool mShouldQuit = false;
        private OffscreenCEFClient cefClient;

        [SerializeField]
        private Size windowSize = new Size(1280, 720);

        public Texture2D BrowserTexture { get; private set; }

        private void Awake()
        {
            this.BrowserTexture = new Texture2D(this.windowSize.Width, this.windowSize.Height, TextureFormat.BGRA32, false);

            CefRuntime.Load(@".\Assets\Plugins\x86_64");

            var cefMainArgs = new CefMainArgs(new string[] { });
            var cefApp = new OffscreenCEFClient.OffscreenCEFApp();

            // This is where the code path divereges for child processes.
            if (CefRuntime.ExecuteProcess(cefMainArgs, cefApp, IntPtr.Zero) != -1)
                Debug.LogError("Could not start the secondary process.");

            var settings = new CefSettings
            {
                MultiThreadedMessageLoop = false,
                SingleProcess = true,
                //ExternalMessagePump = true,
                //BrowserSubprocessPath = @"D:\Alessandro\Documents\Unity\CEFGlueTest\Assets\Plugins\x86_64\Xilium.CefGlue.Client.exe",
                LogSeverity = CefLogSeverity.Verbose,
                LogFile = "cef.log",
                //ResourcesDirPath = @".\Assets\Plugins\x86_64\Resources",
                WindowlessRenderingEnabled = true,
                NoSandbox = true,
            };

            try
            {
                // Start the browser process (a child process).
                CefRuntime.Initialize(cefMainArgs, settings, cefApp, IntPtr.Zero);
            }
            catch (Exception e)
            {
                File.WriteAllText("cefdbg.log", e.StackTrace);
            }

            // Instruct CEF to not render to a window at all.
            CefWindowInfo cefWindowInfo = CefWindowInfo.Create();
            cefWindowInfo.SetAsWindowless(IntPtr.Zero, false);

            // Settings for the browser window itself (e.g. enable JavaScript?).
            CefBrowserSettings cefBrowserSettings = new CefBrowserSettings();

            // Initialize some the custom interactions with the browser process.
            this.cefClient = new OffscreenCEFClient(this.windowSize.Width, this.windowSize.Height);

            // Start up the browser instance.
            CefBrowserHost.CreateBrowser(cefWindowInfo, this.cefClient, cefBrowserSettings, "http://www.google.com/");

            this.StartCoroutine(this.MessagePump());
            DontDestroyOnLoad(this.gameObject);
        }

        private void OnDestroy()
        {
            this.Quit();
        }

        private void OnApplicationQuit()
        {
            this.Quit();
        }

        private void Quit()
        {
            this.mShouldQuit = true;
            this.StopAllCoroutines();
            this.cefClient.Shutdown();
            CefRuntime.Shutdown();
        }

        private IEnumerator MessagePump()
        {
            while (!this.mShouldQuit)
            {
                CefRuntime.DoMessageLoopWork();
                if (!this.mShouldQuit)
                    this.cefClient.UpdateTexture(this.BrowserTexture);
                yield return null;
            }
        }
    }
}