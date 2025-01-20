using TMPro;
using UnityEngine;
using Zenject;

namespace MyWay
{
    public class WelcomeTextController : MonoBehaviour
    {
        [SerializeField] private string _messageUrl;
        [SerializeField] private TextMeshProUGUI _welcomeTM;
        private DataLoader _loader;


        [Inject]
        private void Construct(DataLoader loader)
        {
            _loader = loader;
        }


        private void Start()
        {
            ResetWelcomeTextAsync();
        }


        private async void ResetWelcomeTextAsync()
        {
            MessageData mes = await _loader.GetInitialRequestSerializableAsync<MessageData>(_messageUrl);
            _welcomeTM.SetText(mes.Message);
        }


        public void ResetWelcomeText()
        {
            ResetWelcomeTextAsync();
        }
    }
}
