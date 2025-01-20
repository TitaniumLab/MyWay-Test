using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace MyWay
{
    public class DataLoader : MonoBehaviour
    {
        private List<UnityWebRequest> _initialRequests = new List<UnityWebRequest>();
        public event Action OnInitialRequestsCompletion;


        private async void Start()
        {
            await Awaitable.NextFrameAsync();
            await StartInitialRequestsAsync();
        }


        public async Task StartInitialRequestsAsync()
        {
            var reqNum = _initialRequests.Count;
            _initialRequests.ForEach(req =>
            {
                req.SendWebRequest();
            });

            while (_initialRequests.Count != 0)
            {
                await Task.Yield();
            }

            Debug.Log($"{reqNum} initial requests done successful");
            OnInitialRequestsCompletion?.Invoke();
        }


        public async Task<TSerializableResultType> GetInitialRequestSerializableAsync<TSerializableResultType>(string url)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                _initialRequests.Add(webRequest);

                while (!webRequest.isDone)
                {
                    await Task.Yield();
                }

                if (CheckRequestSuccessful(webRequest))
                {
                    var data = JsonUtility.FromJson<TSerializableResultType>(webRequest.downloadHandler.text);
                    _initialRequests.Remove(webRequest);
                    return data;
                }
                return default;
            }
        }


        public async Task<AssetBundle> GetInitialRequestAssetBundleAsync(string url)
        {
            using (UnityWebRequest webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url))
            {
                _initialRequests.Add(webRequest);

                while (!webRequest.isDone)
                {
                    await Task.Yield();
                }

                _initialRequests.Remove(webRequest);
                if (CheckRequestSuccessful(webRequest))
                {
                    return ((DownloadHandlerAssetBundle)webRequest.downloadHandler).assetBundle;
                }
                return default;
            }
        }


        public async Task<TSerializableResultType> GetRequestSerializableAsync<TSerializableResultType>(string url)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                await webRequest.SendWebRequest();

                if (CheckRequestSuccessful(webRequest))
                {
                    var data = JsonUtility.FromJson<TSerializableResultType>(webRequest.downloadHandler.text);
                    return data;
                }
                return default;
            }
        }


        public async Task<AssetBundle> GetRequestAssetBundleAsync(string url)
        {
            using (UnityWebRequest webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url))
            {
                await webRequest.SendWebRequest();

                if (CheckRequestSuccessful(webRequest))
                {
                    return ((DownloadHandlerAssetBundle)webRequest.downloadHandler).assetBundle;
                }
                return default;
            }
        }


        private bool CheckRequestSuccessful(UnityWebRequest request)
        {
            switch (request.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(request.url + ": Error: " + request.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(request.url + ": HTTP Error: " + request.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(request.url + ":\nRequest processed successfully");
                    return true;
            }
            return false;
        }
    }
}
