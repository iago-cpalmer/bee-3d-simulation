using Assets.Scripts.Enums;
using UnityEngine;

namespace Assets.Scripts.Player.QuestSystem
{
    public class SelectionQuest : IQuest
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
        private EntityType _entityType;

        public SelectionQuest(EntityType entityType, string questDescription)
        {
            _entityType = entityType;
            _questDescription = questDescription;
        }

        public SelectionQuest(EntityType entityType, string questDescription, float maxTime)
        {
            _entityType = entityType;
            _questDescription = questDescription;
            _hasLimitTime = true;
            _remainingTime = maxTime;
        }

        public void ActivateQuest()
        {
            _questState = QuestState.IN_PROGRESS;
            // Subscribe to events
            SelectionHandler.Instance.OnSelectEntity += TryComplete;
        }

        public void Complete()
        {
            _questState = QuestState.COMPLETED;
            SelectionHandler.Instance.OnSelectEntity -= TryComplete;
            QuestSystem.Instance.CompleteQuest(this);
        }

        public void TryComplete(EntityType entityType)
        {
            if(entityType==_entityType)
            {
                Complete();
            }
        }

        public string GetStateInString()
        {
            return "";
        }

        public float GetProgress()
        {
            return _questState == QuestState.COMPLETED ? 1 : 0;
        }

        public void TryComplete()
        {
            
        }

        public void Reset()
        {
            _questState = QuestState.PENDING;
        }
    }
}
