using Aleab.CefUnity.Structs;
using System;
using System.Collections;
using UnityEngine;
using Xilium.CefGlue;

namespace Aleab.CefUnity
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshRenderer))]
    public class OffscreenCEF : MonoBehaviour
    {
        [Space]
        [SerializeField]
        private Size windowSize = new Size(1280, 720);

        [SerializeField]
        private string url = "http://www.google.com";

        [Space]
        [SerializeField]
        private bool hideScrollbars = false;

        private bool shouldQuit = false;
        private OffscreenCEFClient cefClient;

        public Texture2D BrowserTexture { get; private set; }

        private void Awake()
        {
            this.BrowserTexture = new Texture2D(this.windowSize.Width, this.windowSize.Height, TextureFormat.BGRA32, false);
            this.GetComponent<MeshRenderer>().material.mainTexture = this.BrowserTexture;
        }

        private void Start()
        {
            this.StartCef();
            this.StartCoroutine(this.MessagePump());
            DontDestroyOnLoad(this.gameObject.transform.root.gameObject);
        }

        private void OnDestroy()
        {
            this.Quit();
        }

        private void OnApplicationQuit()
        {
            this.Quit();
        }

        private void StartCef()
        {
#if UNITY_EDITOR
            CefRuntime.Load("./Assets/Plugins/x86_64");
#else
            CefRuntime.Load();
#endif

            var cefMainArgs = new CefMainArgs(new string[] { });
            var cefApp = new OffscreenCEFClient.OffscreenCEFApp();

            // This is where the code path diverges for child processes.
            if (CefRuntime.ExecuteProcess(cefMainArgs, cefApp, IntPtr.Zero) != -1)
                Debug.LogError("Could not start the secondary process.");

            var cefSettings = new CefSettings
            {
                //ExternalMessagePump = true,
                MultiThreadedMessageLoop = false,
                SingleProcess = true,
                LogSeverity = CefLogSeverity.Verbose,
                LogFile = "cef.log",
                WindowlessRenderingEnabled = true,
                NoSandbox = true,
            };

            // Start the browser process (a child process).
            CefRuntime.Initialize(cefMainArgs, cefSettings, cefApp, IntPtr.Zero);

            // Instruct CEF to not render to a window.
            CefWindowInfo cefWindowInfo = CefWindowInfo.Create();
            cefWindowInfo.SetAsWindowless(IntPtr.Zero, false);

            // Settings for the browser window itself (e.g. enable JavaScript?).
            CefBrowserSettings cefBrowserSettings = new CefBrowserSettings()
            {
                BackgroundColor = new CefColor(255, 60, 85, 115),
                JavaScript = CefState.Enabled,
                JavaScriptAccessClipboard = CefState.Disabled,
                JavaScriptCloseWindows = CefState.Disabled,
                JavaScriptDomPaste = CefState.Disabled,
                JavaScriptOpenWindows = CefState.Disabled,
                LocalStorage = CefState.Disabled
            };

            // Initialize some of the custom interactions with the browser process.
            this.cefClient = new OffscreenCEFClient(this.windowSize, this.hideScrollbars);

            // Start up the browser instance.
            CefBrowserHost.CreateBrowser(cefWindowInfo, this.cefClient, cefBrowserSettings, string.IsNullOrEmpty(this.url) ? "http://www.google.com" : this.url);
        }

        private void Quit()
        {
            this.shouldQuit = true;
            this.StopAllCoroutines();
            this.cefClient.Shutdown();
            CefRuntime.Shutdown();
        }

        private IEnumerator MessagePump()
        {
            while (!this.shouldQuit)
            {
                CefRuntime.DoMessageLoopWork();
                if (!this.shouldQuit)
                    this.cefClient.UpdateTexture(this.BrowserTexture);
                yield return null;
            }
        }
    }
}