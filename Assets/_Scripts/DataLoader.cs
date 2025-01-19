using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace MyWay
{
    public class DataLoader : MonoBehaviour
    {
        [SerializeField] private string _settingsUrl;
        [SerializeField] private string _messageUrl;

        private async void Awake()
        {
            var set = await GetRequest<Settings>(_settingsUrl);

            Debug.Log(set.StartingNumber);
        }


        private async Task<TSerializableResultType> GetRequest<TSerializableResultType>(string url)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {

                await webRequest.SendWebRequest();

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(url + ": Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(url + ": HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log(url + ":\nReceived: " + webRequest.downloadHandler.text);
                        var data = JsonUtility.FromJson<TSerializableResultType>(webRequest.downloadHandler.text);
                        return data;
                }
                return default;
            }
        }
    }
}
