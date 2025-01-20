using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Zenject;

namespace MyWay
{
    public class CountController : MonoBehaviour
    {
        [SerializeField] private string _settingsUrl;
        [SerializeField] private List<TextMeshProUGUI> _textMeshs;
        [SerializeField] private string _jsonLocalFileName = "Settings.json";
        private DataLoader _loader;
        private ICounter _counter;


        [Inject]
        private void Construct(DataLoader loader)
        {
            _loader = loader;
        }


        private async void Awake()
        {
            var path = Path.Combine(Application.streamingAssetsPath, _jsonLocalFileName);
            if (File.Exists(path))
            {
                var set = GetSettingsFromFile(path);
                _counter = new Counter(Int32.Parse(set.StartingNumber));
            }
            else
            {
                await ResetCounterAsync();
            }
            SetCounterText();
        }


        private void OnApplicationQuit()
        {
            var path = Path.Combine(Application.streamingAssetsPath, _jsonLocalFileName);
            Settings set = File.Exists(path) ? GetSettingsFromFile(path) : new Settings();
            set.StartingNumber = _counter.CurrentValue.ToString();
            var json = JsonUtility.ToJson(set);
            File.WriteAllText(path, json);
        }


        private Settings GetSettingsFromFile(string path)
        {
            var json = File.ReadAllText(path);
            Settings set = JsonUtility.FromJson<Settings>(json);
            return set;
        }


        public async Task ResetCounterAsync()
        {
            Settings set = await _loader.GetInitialRequestSerializableAsync<Settings>(_settingsUrl);
            _counter = new Counter(Int32.Parse(set.StartingNumber));
        }


        public async void ResetCounterAndTextAsync()
        {
            Settings set = await _loader.GetRequestSerializableAsync<Settings>(_settingsUrl);
            _counter = new Counter(Int32.Parse(set.StartingNumber));
            SetCounterText();
        }


        public void SetCounterText()
        {
            if (_textMeshs.Count != 0)
            {
                _textMeshs.ForEach(t => t.SetText(_counter.CurrentValue.ToString()));
            }
        }


        public void AddToCounter(int add = 1)
        {
            _counter.Increment(add);
        }
    }
}
