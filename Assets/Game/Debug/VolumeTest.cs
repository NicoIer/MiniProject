using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityToolkit;
using Random = UnityEngine.Random;

namespace Game
{
    public class VolumeTest : MonoBehaviour
    {
        [SerializeField] private Button addVolumeToProfileButton;
        [SerializeField] private Button logAllVolumesButton;
       [SerializeField] private Button addVolumeToStack;
        [SerializeField] private Button overrideVolumeButton;
        [SerializeField] private Button logAllVolumesButton2;

        [SerializeField] private Volume globalVolume;

        private void Awake()
        {
            addVolumeToProfileButton.onClick.AddListener(AddVolumeToProfile);
            logAllVolumesButton.onClick.AddListener(LogAllVolumes);
            addVolumeToStack.onClick.AddListener(AddVolumeToStack);
            overrideVolumeButton.onClick.AddListener(OverrideVolume);
            logAllVolumesButton2.onClick.AddListener(LogAllVolumes2);
        }

        private void OverrideVolume()
        {
            if (!globalVolume.profile.TryGet(out TestVolume testVolume))
            {
                Debug.LogError("Volume not found");
                return;
            }

            testVolume.parameter.value = Random.Range(int.MinValue, int.MaxValue);
            testVolume.parameter.overrideState = true;
            testVolume.active = true;
        }

        private void LogAllVolumes()
        {
            foreach (var type in VolumeManager.instance.baseComponentTypeArray)
            {
                Debug.Log($"Volume type: {type}");
            }
        }

        private void LogAllVolumes2()
        {
            foreach (var type in CoreUtils.GetAllAssemblyTypes().Where(t => t.IsSubclassOf(typeof(VolumeComponent))))
            {
                Debug.Log($"Volume type: {type}");
            }
        }

        private void AddVolumeToStack()
        {
            VolumeManager.instance.AddDefault<TestVolume>();
        }

        private void AddVolumeToProfile()
        {
            Debug.Log("Add Volume");
            globalVolume.profile.Add<TestVolume>();
            Debug.Log("Volume added");
        }
    }
}