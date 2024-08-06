using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityToolkit;

namespace Game
{
    public class GameHomeMgr : MonoSingleton<GameHomeMgr>
    {
        private void Start()
        {
            VolumeManager.instance.stack.GetComponent<ColorAdjustments>();
        }
    }
}