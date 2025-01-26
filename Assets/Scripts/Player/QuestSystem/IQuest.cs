using Assets.Scripts.Enums;

namespace Assets.Scripts.Player.QuestSystem
{
    public interface IQuest
    {
        protected QuestState QuestState { get; }
        public string QuestDescription { get; }
        public bool HasLimitTime { get; }
        public float RemainingTime { get; set; }
        public void ActivateQuest();
        public void Complete();
        public void TryComplete();

        public string GetStateInString();
        public float GetProgress();

        public void Reset();

    }
}
