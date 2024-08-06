using System;
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
        [SerializeField] private string configPath = "Resource/Config/PatchEntryConfig.asset";
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

            // Load PatchEntryConfig
            PatchEntryConfig config = await Addressables.LoadAssetAsync<PatchEntryConfig>(configPath);

            /*Load MetaData*/
            List<TextAsset> aotMetaAssets = new List<TextAsset>();
            foreach (var reference in config.hybridClrAotMetaReferences)
            {
                TextAsset asset = await reference.LoadAssetAsync<TextAsset>();
                aotMetaAssets.Add(asset);
            }

            foreach (var metaAsset in aotMetaAssets)
            {
                LoadImageErrorCode errorCode =
                    RuntimeApi.LoadMetadataForAOTAssembly(metaAsset.bytes, HomologousImageMode.Consistent);
                Debug.Log($"Load AOT META [{metaAsset.name}]: {errorCode}");
            }

            foreach (var metaAsset in aotMetaAssets)
            {
                Addressables.Release(metaAsset);
            }

            /*Load DLL*/

            List<TextAsset> dllAssets = new List<TextAsset>();

            foreach (var reference in config.hybridClrDllReferences)
            {
                TextAsset asset = await reference.LoadAssetAsync<TextAsset>();
                dllAssets.Add(asset);
            }

            foreach (var dllAsset in dllAssets)
            {
#if !UNITY_EDITOR
                Assembly.Load(dllAsset.bytes);
#endif
                Debug.Log($"Load DLL: {dllAsset.name}");
            }

            foreach (var dllAsset in dllAssets)
            {
                Addressables.Release(dllAsset);
            }

            Addressables.Release(config);

            /*Enter Game*/
            gameEntryReference.LoadSceneAsync();
        }

        private async void StartUpdate()
        {
            retryButton.gameObject.SetActive(false);
            progressText.text = "";
            contentText.text = "";
            versionText.text = $"{Application.version}";
            enterGameButton.gameObject.SetActive(false);
            await AddressableUpdate();
            enterGameButton.gameObject.SetActive(true);
        }

        // private bool hasLeftResource = false;

        private async UniTask AddressableUpdate()
        {
            List<object> keys = new List<object>();
            await Addressables.InitializeAsync(true).Task;
            /*更新CATALOG*/
            var checkHandle = Addressables.CheckForCatalogUpdates(false);
            var catalogList = await checkHandle;

            // if (checkHandle.Status != AsyncOperationStatus.Succeeded)
            // {
            //     retryButton.gameObject.SetActive(true);
            //     Debug.Log("CheckForCatalogUpdates failed");
            //     return;
            // }


            if (catalogList.Count <= 0)
            {
                Debug.Log("没有找到Catalog更新");

                // if (hasLeftResource)
                // {
                //     await DownloadLeftResource();
                // }
                //
                // hasLeftResource = false;
                
                Addressables.Release(checkHandle);
                return;
            }

            foreach (var catalog in catalogList)
            {
                Debug.Log($"需要更新的CataLog: {catalog}");
            }

            try
            {
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
                    await UniTask.DelayFrame(1);
                }

                contentText.text = "下载完成"; //"Download complete";
                progressText.text = "100%";

                Addressables.Release(checkHandle);
                Addressables.Release(downloadHandle);
            }
            catch (Exception e)
            {
                retryButton.gameObject.SetActive(true);
                Debug.LogError(e);
            }
        }
    }
}