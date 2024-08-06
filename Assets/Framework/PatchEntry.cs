using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using UnityToolkit;

namespace Framework
{
    public class PatchEntry : MonoSingleton<PatchEntry>, IOnlyPlayingModelSingleton
    {
        [SerializeField] private AssetReference hybridCLRDllFolder;
        [SerializeField] private AssetReference hybridCLRAotFolder;

        [SerializeField] private Button retryButton;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private TextMeshProUGUI contentText;
        [SerializeField] private TextMeshProUGUI versionText;

        protected override async void OnInit()
        {
            progressText.text = "";
            contentText.text = "";
            versionText.text = $"{Application.version}";
            await AddressableUpdate();
        }


        private async UniTask AddressableUpdate()
        {
            await Addressables.InitializeAsync().Task;
            var catalogList = await Addressables.CheckForCatalogUpdates().Task;
        }
    }
}