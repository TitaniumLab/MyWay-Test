using DG.Tweening;
using UnityEngine;
using Zenject;

namespace MyWay
{
    public class LoadingScreen : MonoBehaviour
    {
        private DataLoader _loader;
        [SerializeField] private RectTransform _animIcon;
        [SerializeField] private float _iconRotFreq = 1.0f;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _fadeDelay = 0.5f;
        [SerializeField] private float _fadeDuration = 0.5f;
        private Tween _tween;


        [Inject]
        private void Construct(DataLoader loader)
        {
            _loader = loader;
        }


        private void OnEnable()
        {
            _tween = _animIcon.DORotate(new Vector3(0, 0, 360), 1 / _iconRotFreq, RotateMode.WorldAxisAdd).SetEase(Ease.Flash).SetLoops(-1);
            _tween.Play();
            _loader.OnInitialRequestsCompletion += FadeOutFoadingScreen;
        }

        private void OnDisable()
        {
            _tween.Pause();
            _loader.OnInitialRequestsCompletion -= FadeOutFoadingScreen;
        }


        public async void FadeOutFoadingScreen()
        {
            await Awaitable.WaitForSecondsAsync(_fadeDelay);
            await _canvasGroup.DOFade(0, _fadeDuration).AsyncWaitForCompletion();
            _canvasGroup.gameObject.SetActive(false);
        }
    }
}
