using Assets.Scripts.Enums;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Player.QuestSystem
{
    public class PositionQuest : IQuest
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
        private float3 _position;
        private float _distancesq;
        private float3 _currentPosition;

        public PositionQuest(float3 position, float distancesq, string questDescription)
        {
            _position = position;
            _distancesq = distancesq;
            _questDescription = questDescription;
        }public PositionQuest(float3 position, float distancesq, string questDescription, float maxTime)
        {
            _position = position;
            _distancesq = distancesq;
            _questDescription = questDescription;
            _hasLimitTime = true;
            _remainingTime = maxTime;
        }

        public void ActivateQuest()
        {
            _questState = QuestState.IN_PROGRESS;
            // Subscribe to events
            SpectatorController.Instance.OnMove += TryComplete;
        }

        public void Complete()
        {
            _questState = QuestState.COMPLETED;
            SpectatorController.Instance.OnMove -= TryComplete;
            QuestSystem.Instance.CompleteQuest(this);
        }

        public void TryComplete(float3 pos)
        {
            if(math.distancesq(pos, _position)<=_distancesq)
            {
                Complete();
            } else
            {
                _currentPosition = pos;
                QuestSystem.Instance.ShowInformationOfActiveQuest();
            }
        }

        public string GetStateInString()
        {
            return (int)math.distance(_position, _currentPosition) + " meters away";
        }

        public float GetProgress()
        {
            return _questState == QuestState.COMPLETED ? 1 : 0;
        }

        public void TryComplete()
        {
            TryComplete(_currentPosition);
        }

        public void Reset()
        {
            _questState = QuestState.PENDING;
        }
    }
}
