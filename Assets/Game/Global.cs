using UnityEngine;
using UnityToolkit;

namespace Game
{
    public class Global : MonoSingleton<Global>
    {
        private SystemLocator _locator;

        public static string version
        {
            get
            {
                return Application.version;
            }
        }
        protected override void OnInit()
        {
            base.OnInit();
        }

        protected override void OnDispose()
        {
            base.OnDispose();
        }
    }
}