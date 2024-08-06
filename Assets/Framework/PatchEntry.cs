using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using HybridCLR;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using UnityToolkit;

namespace Framework
{
    public class PatchEntry : MonoSingleton<PatchEntry>, IOnlyPlayingModelSingleton
    {
        [SerializeField] private AssetLabelReference hybridClrDllLabelReference;
        [SerializeField] private AssetLabelReference hybridClrAotMetaLabelReference;
        [SerializeField] private AssetReference gameEntryReference;

        [SerializeField] private Button retryButton;
        [SerializeField] private Button enterGameButton;

        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private TextMeshProUGUI contentText;
        [SerializeField] private TextMeshProUGUI versionText;

        protected override void OnInit()
        {
            retryButton.onClick.AddListener(StartUpdate);
            enterGameButton.onClick.AddListener(StartEnterGame);
            StartUpdate();
        }

        private async void StartEnterGame()
        {
            enterGameButton.gameObject.SetActive(false);
            /*Load MetaData*/
            var aotMetaAssets = await Addressables.LoadAssetsAsync<TextAsset>(hybridClrAotMetaLabelReference, null);
            foreach (var metaAsset in aotMetaAssets)
            {
                LoadImageErrorCode errorCode =
                    RuntimeApi.LoadMetadataForAOTAssembly(metaAsset.bytes, HomologousImageMode.Consistent);
                Debug.Log($"Load AOT META [{metaAsset.name}]: {errorCode}");
            }

            Addressables.Release(aotMetaAssets);
            /*Load DLL*/

            var dllAssets = await Addressables.LoadAssetsAsync<TextAsset>(hybridClrDllLabelReference, null);
            foreach (var dllAsset in dllAssets)
            {
#if !UNITY_EDITOR
                Assembly.Load(dllAsset.bytes);
#endif
                Debug.Log($"Load DLL: {dllAsset.name}");
            }

            Addressables.Release(dllAssets);
            /*Enter Game*/ 
            gameEntryReference.LoadSceneAsync();
        }

        private async void StartUpdate()
        {
            enterGameButton.gameObject.SetActive(false);
            retryButton.gameObject.SetActive(false);
            progressText.text = "";
            contentText.text = "";
            versionText.text = $"{Application.version}";
            await AddressableUpdate();
        }


        private async UniTask AddressableUpdate()
        {
            enterGameButton.gameObject.SetActive(false);
            List<object> keys = new List<object>();
            await Addressables.InitializeAsync(true).Task;
            /*更新CATALOG*/
            var checkHandle = Addressables.CheckForCatalogUpdates();
            var catalogList = await checkHandle.Task;

            if (checkHandle.Status != AsyncOperationStatus.Succeeded)
            {
                retryButton.gameObject.SetActive(true);
                Debug.Log("CheckForCatalogUpdates failed");
                return;
            }

            if (catalogList.Count <= 0)
            {
                Debug.Log("No updates found");
                return;
            }

            foreach (var catalog in catalogList)
            {
                Debug.Log($"catalog: {catalog}");
            }

            contentText.text = "发现更新"; //"Found updates";

            Debug.Log("Start update catalog");
            var catalogResourceLocators = await Addressables.UpdateCatalogs(catalogList, true).Task;


            foreach (var catalogResource in catalogResourceLocators)
            {
                Debug.Log($"catalogResource.LocatorId: {catalogResource.LocatorId}");
                keys.AddRange(catalogResource.Keys);
            }

            Debug.Log("Update catalog done");
            /*根据CATALOG预先下载好所有资源*/
            var downloadSize = await Addressables.GetDownloadSizeAsync((IEnumerable)keys).Task;
            var downloadSizeMb = downloadSize / 1024 / 1024; // mb
            Debug.Log($"downloadSize: {downloadSizeMb}MB");
            contentText.text = $"下载大小: {downloadSizeMb}MB"; //"Download size: {downloadSizeMb}MB";

            if (downloadSize <= 0)
            {
                Debug.Log("No updates found");
                return;
            }

            var downloadHandle =
                Addressables.DownloadDependenciesAsync((IEnumerable)keys, Addressables.MergeMode.Union);

            var status = downloadHandle.GetDownloadStatus();
            while (!downloadHandle.IsDone &&
                   downloadHandle.Status != AsyncOperationStatus.Succeeded &&
                   downloadHandle.Status != AsyncOperationStatus.Failed)
            {
                contentText.text =
                    $"下载中{status.DownloadedBytes}/{status.TotalBytes}";
                progressText.text = $"{downloadHandle.PercentComplete * 100}%";
            }

            Addressables.Release(downloadHandle);

            enterGameButton.gameObject.SetActive(true);
        }
    }
}