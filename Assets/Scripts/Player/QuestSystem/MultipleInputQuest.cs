using Assets.Scripts.Enums;
using UnityEngine;

namespace Assets.Scripts.Player.QuestSystem
{
    public class MultipleInputQuest : IQuest
    {
        // State
        QuestState IQuest.QuestState { get; }
        private QuestState _questState;

        public string QuestDescription { get { return _questDescription; } }
        private string _questDescription;
        public bool HasLimitTime { get { return _hasLimitTime; } }
        private bool _hasLimitTime;
        public float RemainingTime { get { return _remainingTime; } set { _remainingTime = value; } }
        private float _remainingTime;
        // Specific of quest
        private KeyCode[] _keys;

        public MultipleInputQuest(KeyCode[] keys, string questDescription)
        {
            _keys = keys;
            _questDescription = questDescription;
        }
        public MultipleInputQuest(KeyCode[] keys, string questDescription, float maxTime)
        {
            _keys = keys;
            _questDescription = questDescription;
            _hasLimitTime = true;
            _remainingTime = maxTime;
        }

        
        public void ActivateQuest()
        {
            _questState = QuestState.IN_PROGRESS;
            // Subscribe to events
            SpectatorController.Instance.OnPressKey += TryComplete;
        }

        public void TryComplete()
        {
            for (int i = 0; i < _keys.Length; i++)
            {
                if (Input.GetKeyDown(_keys[i])) {
                    // key pressed
                    Complete();
                    return;
                }
            }
        }

        public void Complete()
        {
            _questState = QuestState.COMPLETED;
            SpectatorController.Instance.OnPressKey -= TryComplete;
            QuestSystem.Instance.CompleteQuest(this);
        }

        public string GetStateInString()
        {
            return "";
        }

        public float GetProgress()
        {
            return _questState == QuestState.COMPLETED ? 1 : 0;
        }

        public void Reset()
        {
            _questState = QuestState.PENDING;
        }
    }
}
