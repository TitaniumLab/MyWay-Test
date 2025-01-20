using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MyWay
{
    public class RefreshButtonController : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onButtonClick;
        [SerializeField] private float _cooldown;
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnButtonClick);
        }


        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnButtonClick);
        }


        private async void OnButtonClick()
        {
            _button.interactable = false;
            _onButtonClick.Invoke();
            await Awaitable.WaitForSecondsAsync(_cooldown);
            _button.interactable = true;
        }
    }
}
