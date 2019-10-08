using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

using UnityObject = UnityEngine.Object;


namespace RedHoney.Utilities
{
    using Debug = Log.ContextDebug<AssetBundlesManager>;

    ///////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// This utility class helps with async operations on AssetBundles downloading
    /// and unpucking
    /// </summary>
    class AssetBundlesManager
    {
        private readonly MonoBehaviour monoBehaviour;

        ///////////////////////////////////////////////////////////////////////////
        /// <summary>It needs a MonoBehaviour to spawn coroutines</summary>
        public AssetBundlesManager(MonoBehaviour monoBehaviour)
        {
            this.monoBehaviour = monoBehaviour;
        }

        ///////////////////////////////////////////////////////////////////////////
        /// <summary>Asynchronously downloads an AssetBundle from the web</summary>
        public void GetFromWeb(string url, Action<AssetBundle> successCallback, Action<string> errorCallback = null)
        {
            IEnumerator routine()
            {
                UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url);
                www.certificateHandler = new BypassCertificate();
                yield return www.SendWebRequest();

                if (!www.isNetworkError && !www.isHttpError)
                {
                    AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
                    Debug.Log($"Loaded {url}. Content:\n{string.Join(", ", bundle.GetAllAssetNames())}");
                    successCallback?.Invoke(bundle);
                }
                else
                {
                    Debug.LogError($"Cannot load {url}: {www.error}");
                    errorCallback?.Invoke(www.error);
                }
            }
            monoBehaviour.StartCoroutine(routine());
        }

        ///////////////////////////////////////////////////////////////////////////
        /// <summary>Asynchronously loads an AssetBundle from a file</summary>
        public void GetFromFile(string filePath, Action<AssetBundle> successCallback, Action<string> errorCallback = null)
        {
            IEnumerator routine()
            {
                AssetBundleCreateRequest loadRequest = AssetBundle.LoadFromFileAsync(filePath);
                yield return loadRequest;

                AssetBundle bundle = loadRequest.assetBundle;
                if (bundle != null)
                {
                    Debug.Log($"Loaded {filePath}. Content:\n{string.Join(", ", bundle.GetAllAssetNames())}");
                    successCallback?.Invoke(bundle);
                }
                else
                {
                    Debug.LogError($"Cannot load {filePath}");
                    errorCallback?.Invoke($"Failed to load AssetBundle {filePath}!");
                }
            }
            monoBehaviour.StartCoroutine(routine());
        }

        ///////////////////////////////////////////////////////////////////////////
        /// <summary>Asynchronously loads an AssetBundle from the streaming assets</summary>
        public void GetFromStreamingAssets(string filePath, Action<AssetBundle> successCallback, Action<string> errorCallback = null)
        {
            GetFromFile(Path.Combine(Application.streamingAssetsPath, filePath), successCallback, errorCallback);
        }

        ///////////////////////////////////////////////////////////////////////////
        /// <summary>Asynchronously loads an Object packed in an AssetBundle</summary>
        public void LoadAsset(AssetBundle assetBundle, string name, Action<UnityObject> successCallback, Action<string> errorCallback = null)
        {
            IEnumerator routine()
            {
                AssetBundleRequest assetLoadRequest = assetBundle.LoadAssetAsync<UnityObject>(name);
                yield return assetLoadRequest;

                if (assetLoadRequest.asset != null)
                {
                    Debug.Log($"Loaded asset {name}");
                    successCallback?.Invoke(assetLoadRequest.asset);
                }
                else
                {
                    Debug.LogError($"Cannot load asset {name}");
                    errorCallback?.Invoke($"Failed to open AssetBundle {assetBundle.name}!");
                }
            }
            monoBehaviour.StartCoroutine(routine());
        }

        ///////////////////////////////////////////////////////////////////////////
        private class BypassCertificate : CertificateHandler
        {
            protected override bool ValidateCertificate(byte[] certificateData) => true;
        }
    }
}