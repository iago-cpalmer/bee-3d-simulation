using Assets.Scripts.Player.QuestSystem;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Player.LevelSystem
{
    public class Level
    {
        public int LevelId;
        public IQuest[] Quests;
        public string Name;
        public string Description;
        public int CompletedStars;
        public Star[] Stars;
        public LevelPanelHandler LevelPanel;

        public Level(int levelId, string name, string description, IQuest[] quests, GameObject starPrefab, float3 initialPositionStars)
        {
            LevelId = levelId;
            Name = name;
            Description = description;
            Quests = quests;
        }

        public void RecreateUI(GameObject starPrefab, float3 initialPositionStars)
        {
            LevelPanel = LevelMenuHandler.Instance.CreateLevelPanel(this);
            if (starPrefab == null)
                return;
            Stars = new Star[5];
            for (int i = 0; i < 5; i++)
            {
                Stars[i] = GameObject.Instantiate(starPrefab, LevelPanel.gameObject.transform).GetComponent<Star>();
                Stars[i].gameObject.transform.localPosition = initialPositionStars + new float3(50 * i, 0, 0);
                if (i<CompletedStars)
                {
                    Stars[i].SetStarState(true);
                }
                
            }
        }

        public void CompleteQuests(float progress)
        {
            CompletedStars = math.max((int)((progress) * 5), CompletedStars);
        }

    }
}
