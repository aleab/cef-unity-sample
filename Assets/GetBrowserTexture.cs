using UnityEngine;


namespace Aleab.CefUnity
{
    public class GetBrowserTexture : MonoBehaviour
    {
        public OffscreenCEF BrowserTextureSrc;

        private Material mMtl;

        private void Start()
        {
            this.mMtl = this.GetComponent<MeshRenderer>().material;
            this.mMtl.SetTexture("_MainTex", this.BrowserTextureSrc.BrowserTexture);
        }
    }
}