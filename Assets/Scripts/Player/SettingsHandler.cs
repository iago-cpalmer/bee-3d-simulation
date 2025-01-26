using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Player
{
    class SettingsHandler : MonoBehaviour
    {
        public static SettingsHandler Instance { get { return _instance; } }
        private static SettingsHandler _instance;

        public float Sensitivity;
        public float Volume;

        public bool InvertMouseX;
        public bool InvertMouseY;

        private void Awake()
        {
            if(_instance==null)
            {
                _instance = this;
            } else
            {
                Destroy(_instance);
                _instance = this;
            }
            DontDestroyOnLoad(this);
        }
    }
}
