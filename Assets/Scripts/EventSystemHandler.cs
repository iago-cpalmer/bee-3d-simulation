using UnityEngine;

namespace Assets.Scripts
{
    class EventSystemHandler : MonoBehaviour
    {

        public static EventSystemHandler Instance { get { return _instance; } }
        private static EventSystemHandler _instance;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(this);
                return;
            }
            DontDestroyOnLoad(this);
        }
    }
}
