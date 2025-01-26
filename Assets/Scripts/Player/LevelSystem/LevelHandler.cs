using Assets.Scripts.Enums;
using Assets.Scripts.Player.QuestSystem;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Player.LevelSystem
{
    public class LevelHandler : MonoBehaviour
    {
        public static LevelHandler Instance;

        [SerializeField] private GameObject starPrefab;
        public GameObject LevelPanelPrefab; 
        public Level[] Levels;
        public int SelectedLevel = -1;
        public int currentlyActiveLevel = 0;

        private void Awake()
        {
            if(Instance==null)
            {
                Instance = this;
            } else if (Instance != this)
            {
                Destroy(this);
                return;
            }
            DontDestroyOnLoad(this);

            Levels = new Level[Globals.N_LEVELS];

        }
        private void Start()
        {
            // Init levels
            IQuest[] questsLevelZero = new IQuest[8];
            // Init quests
            questsLevelZero[0] = new MultipleInputQuest(new KeyCode[] { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D }, "Move around by pressing the keys: W, A, S, D");
            questsLevelZero[1] = new PositionQuest(new float3(128, 34, 128), 100, "Find the nest of the colony!");
            questsLevelZero[2] = new SelectionQuest(EntityType.BEE_NEST, "Select the nest you have just found with Left Click to see some information about the colony.");
            questsLevelZero[3] = new SelectionQuest(EntityType.BEE, "Select a bee to see some specific information of that bee.");
            questsLevelZero[4] = new ActionQuest(ActionType.FOLLOW, new DataUnion { EntityType = EntityType.BEE }, "You can follow a bee by pressing 'F'");
            questsLevelZero[5] = new ActionQuest(ActionType.SWITCH_TO_PLAYER_MODE, new DataUnion { Int = 0 }, "Help the colony by foraging. \nConvert to a bee pressing 'B'");
            questsLevelZero[6] = new ActionQuest(ActionType.COLLECT, new DataUnion { Int = 100 }, "Why don't you collect some pollen and nectar for the colony? \nFind a flower and recollect 100 of pollen.");
            questsLevelZero[7] = new ActionQuest(ActionType.REST, new DataUnion { Int = 0 }, "You've collected some pollen and nectar, go to the nest to store it and get some rest!");
            Levels[0] = new Level(0, "Level 0 - Tutorial", "Complete the tutorial.", questsLevelZero, starPrefab, new float3(200, 0, 0));


            // Level one
            IQuest[] questsLevelOne = new IQuest[2];
            questsLevelOne[0] = new ActionQuest(ActionType.COLLECT_NECTAR, new DataUnion { Int = 450 }, "Collect 450 of nectar.", 120.0f);
            Levels[1] = new Level(1, "Level 1 - Collect nectar", "Collect a total of 450 of nectar in less than 2 minutes", questsLevelOne, starPrefab, new float3(200, 0, 0));

            // Level one
            IQuest[] questsInfinite = null;
            Levels[Levels.Length-1] = new Level(2, "Infinite game", "Play with no goals, as long as you want.", questsInfinite, null, new float3(200, 0, 0));

            LoadLevels();

            for (int i = 0; i < Levels.Length - 1; i++)
            {
                Levels[i].RecreateUI(starPrefab, new float3(200, 0, 0));
            }
            Levels[Levels.Length-1].RecreateUI(null, float3.zero);
        }

        public void OnDisplayLevelSelector()
        {
            for (int i = 0; i < Levels.Length - 1; i++)
            {
                Levels[i].RecreateUI(starPrefab, new float3(200, 0, 0));
            }
            Levels[Levels.Length - 1].RecreateUI(null, float3.zero);
        }

        public void SelectLevel(int levelId)
        {
            if (SelectedLevel != -1)
                Levels[SelectedLevel].LevelPanel.Unselect();
            SelectedLevel = levelId;
            SoundManager.Instance.PlaySound(SoundType.BUTTON_CLICK, 1, true);
        }
        public void OnPlay()
        {
            QuestSystem.QuestSystem.Instance.StartQuestline(Levels[SelectedLevel].Quests);
            QuestSystem.QuestSystem.Instance.OnCompleteQuestLine += OnCompleteQuestLine;
            currentlyActiveLevel = SelectedLevel;
            SelectedLevel = -1;
        }
        public void UnsubscribeFromEvents()
        {
            QuestSystem.QuestSystem.Instance.OnCompleteQuestLine -= OnCompleteQuestLine;
        }
        public void OnCompleteQuestLine(float progress)
        {
            // Show screen of level completion
            LevelFinalScreen.Instance.Display(Levels[currentlyActiveLevel], progress);
            SaveLevelsProgress();
            Levels[currentlyActiveLevel].CompleteQuests(progress);
        }

        public void SaveLevelsProgress()
        {
            for(int i = 0; i < Levels.Length-1; i++)
            {
                PlayerPrefs.SetInt("lvl-"+i, Levels[i].CompletedStars);
            }
            PlayerPrefs.Save();
        }

        public void LoadLevels()
        {
            for (int i = 0; i < Levels.Length - 1; i++)
            {
                string lvlString = "lvl-" + i;
                if(PlayerPrefs.HasKey(lvlString))
                {
                    Levels[i].CompletedStars = PlayerPrefs.GetInt(lvlString);
                }
            }
        }
    }
}
