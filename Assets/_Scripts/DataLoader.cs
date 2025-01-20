using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace MyWay
{
    public class DataLoader : MonoBehaviour
    {
        //[SerializeField] private string _settingsUrl; // https://drive.google.com/uc?export=download&id=10PxQzHTwlNV293_gb19ROboctSFAuiJo
        //[SerializeField] private string _messageUrl; // https://drive.google.com/uc?export=download&id=1q5QrVd4V8BsbJtHfWitrXOEejSM-xm3n
        private List<UnityWebRequest> _initialRequests = new List<UnityWebRequest>();
        public event Action OnInitialRequestsCompletion;


        //private async void Awake()
        //{
        //    //var setHandler = GetInitialRequestSerializable<Settings>(_settingsUrl);
        //    //var mesHandler = GetInitialRequestSerializable<MessageData>(_messageUrl);
        //    //List<Task> tasks = new List<Task>();
        //    //tasks.Add(setHandler);
        //    //tasks.Add(mesHandler);
        //    //await Task.WhenAll(tasks);
        //}


        private async void Start()
        {
            await StartInitialRequests();
        }


        public async Task StartInitialRequests()
        {
            _initialRequests.ForEach(req =>
            {
                req.SendWebRequest();
            });

            while (_initialRequests.Count != 0)
            {
                await Task.Yield();
            }
            OnInitialRequestsCompletion?.Invoke();
        }


        public async Task<TSerializableResultType> GetInitialRequestSerializable<TSerializableResultType>(string url)
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
                }
                return default;
            }
        }


        public async Task<AssetBundle> GetInitialRequestAssetBundle(string url)
        {
            using (UnityWebRequest webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url))
            {
                _initialRequests.Add(webRequest);

                while (!webRequest.isDone)
                {
                    await Task.Yield();
                }
                return ((DownloadHandlerAssetBundle)webRequest.downloadHandler).assetBundle;
            }
        }


        public async Task<TSerializableResultType> GetRequestSerializable<TSerializableResultType>(string url)
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


        public async Task<AssetBundle> GetRequestAssetBundle(string url)
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
