using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Player.LevelSystem
{
    public class LevelMenuHandler : MonoBehaviour
    {
        public static LevelMenuHandler Instance;
        private LevelPanelHandler[] _levelPanels;
        public int LevelsCreated = 0;
        private void Awake()
        {
            if(Instance!=null)
            {
                Destroy(Instance);
            }
            Instance = this;
            _levelPanels = new LevelPanelHandler[Globals.N_LEVELS];
        }

        private void Start()
        {
            
        }

        public LevelPanelHandler CreateLevelPanel(Level level)
        {
            LevelsCreated++;
            if (LevelsCreated>_levelPanels.Length)
            {
                throw new System.Exception("Created more levels than expected! Expected: " + _levelPanels.Length + ", Created: " + LevelsCreated);
            }
             float3 position = new float3(0, (_levelPanels.Length-LevelsCreated) * 220 - 100, 0);
            _levelPanels[LevelsCreated-1] = Instantiate(LevelHandler.Instance.LevelPanelPrefab, position, Quaternion.identity, transform).GetComponent<LevelPanelHandler>();
            _levelPanels[LevelsCreated - 1].transform.localPosition = position;
            _levelPanels[LevelsCreated - 1].SetInfo(level);
            return _levelPanels[LevelsCreated - 1];
        }
    }
}
