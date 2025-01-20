using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace MyWay
{
    public class SpriteSwaper : MonoBehaviour
    {
        [SerializeField] private string _assetBundleUrl;
        [SerializeField] private string _assetName = "Button";
        [SerializeField] private Image _image;

        private DataLoader _loader;

        [Inject]
        private void Construct(DataLoader loader)
        {
            _loader = loader;
        }

        private async void Start()
        {
            var bundle = await _loader.GetInitialRequestAssetBundleAsync(_assetBundleUrl);
            var prefab = bundle.LoadAsset<GameObject>(_assetName);
            _image.sprite = prefab.GetComponent<Image>().sprite;
            bundle.Unload(false);
        }


        private async void ResetImagesAsync()
        {
            var bundle = await _loader.GetRequestAssetBundleAsync(_assetBundleUrl);
            var prefab = bundle.LoadAsset<GameObject>(_assetName);
            _image.sprite = prefab.GetComponent<Image>().sprite;
            bundle.Unload(false);
        }

        public void ResetImages()
        {
            ResetImagesAsync();
        }
    }
}
