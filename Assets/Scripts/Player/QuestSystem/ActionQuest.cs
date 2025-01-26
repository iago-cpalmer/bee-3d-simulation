using Assets.Scripts.Enums;
using Assets.Scripts.Systems;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Player.QuestSystem
{
    public class ActionQuest : IQuest
    {
        // State
        QuestState IQuest.QuestState { get; }
        private QuestState _questState;

        public string QuestDescription { get { return _questDescription; } }

        public bool HasLimitTime { get { return _hasLimitTime; } }
        private bool _hasLimitTime;
        public float RemainingTime { get { return _remainingTime; } set { _remainingTime = value; } }
        private float _remainingTime;
        private float timeToComplete;

        private string _questDescription;
       
        
        // Specific of quest
        private ActionType _actionType;
        private DataUnion _specificData;
        private int _currentPollenAmount;

        public ActionQuest(ActionType actionType, DataUnion specificData, string questDescription)
        {
            _actionType = actionType;
            _specificData = specificData;
            _questDescription = questDescription;
        }

        public ActionQuest(ActionType actionType, DataUnion specificData, string questDescription, float maxTime)
        {
            _actionType = actionType;
            _specificData = specificData;
            _questDescription = questDescription;
            _hasLimitTime = true;
            _remainingTime = maxTime;
            timeToComplete = maxTime;
        }
       

        public void ActivateQuest()
        {
            _questState = QuestState.IN_PROGRESS;
            // Subscribe to events
            SpectatorController.Instance.OnDoAction += TryComplete;
        }

        public void Complete()
        {
            _questState = QuestState.COMPLETED;
            SpectatorController.Instance.OnDoAction -= TryComplete;
            QuestSystem.Instance.CompleteQuest(this);
        }

        public void TryComplete(ActionType action, DataUnion data)
        {
            if(_actionType == action)
            {
                switch(action)
                {
                    case ActionType.FOLLOW:
                        if (data.EntityType == _specificData.EntityType)
                        {
                            Complete();
                            return;
                        }
                        break;
                    case ActionType.COLLECT:
                    case ActionType.COLLECT_NECTAR:
                        _currentPollenAmount = data.Int;
                        if (data.Int>=_specificData.Int)
                        {
                            Complete();
                        }
                        QuestSystem.Instance.ShowInformationOfActiveQuest();
                        return;
                    default:
                        Complete();
                        return;
                }
            }
        }
        public void TryComplete()
        {
            switch (_actionType)
            {
                case ActionType.COLLECT:
                    if (_currentPollenAmount >= _specificData.Int)
                    {
                        Complete();
                    }
                    QuestSystem.Instance.ShowInformationOfActiveQuest();
                    return;
            }
        }

        public string GetStateInString()
        {
            if(_actionType==ActionType.COLLECT)
            {
                if(_hasLimitTime)
                {
                    return "Time left: " + (int)(_remainingTime / 60) + "m. " + (int)(_remainingTime % 60) + "s. \n\nPollen: " + _currentPollenAmount + "/" + _specificData.Int;
                } else
                {
                    return "Pollen: " + _currentPollenAmount + "/" + _specificData.Int;
                }
                
            }else if(_actionType==ActionType.COLLECT_NECTAR)
            {
                if (_hasLimitTime)
                {
                    return "Time left: " + (int)(_remainingTime / 60) + "m. " + (int)(_remainingTime % 60) + "s. \n\nNectar: " + _currentPollenAmount + "/" + _specificData.Int;
                }
                else
                {
                    return "Nectar: " + _currentPollenAmount + "/" + _specificData.Int;
                }
            } 
            else
            {
                return "";
            }
        }

        public float GetProgress()
        {
            if(_actionType == ActionType.COLLECT || _actionType == ActionType.COLLECT_NECTAR)
            {
                return (float)(_currentPollenAmount) / (float)(_specificData.Int);
            }
            return _questState==QuestState.COMPLETED ? 1 : 0;
        }

        public void Reset()
        {
            _questState = QuestState.PENDING;
            _currentPollenAmount = 0;
            _remainingTime = timeToComplete;
        }
    }
}
