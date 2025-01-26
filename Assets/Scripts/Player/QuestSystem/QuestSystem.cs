using Assets.Scripts.Enums;
using Assets.Scripts.Sound;
using System;
using System.Collections;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Player.QuestSystem
{
    class QuestSystem : MonoBehaviour
    {
        // Singleton pattern
        public static QuestSystem Instance { get { return _instance; } }
        private static QuestSystem _instance;

        // References
        [SerializeField] private GameObject questUI;
        [SerializeField] private Image check;
        [SerializeField] private Image checkBox;
        [SerializeField] private TMP_Text textInfoQuest;
        [SerializeField] private TMP_Text textInfoQuestStriked;
        [SerializeField] private TMP_Text textQuestState;

        public event Action<float> OnCompleteQuestLine;

        // Attributes
        private int _currentActiveQuest = -1;
        private IQuest[] _quests;

        private void Awake()
        {
            if(_instance==null)
            {
                _instance = this;
            } else
            {
                Destroy(this);
                return;
            }
        }

        private void Update()
        {
            if (_quests == null)
                return;
            if (_currentActiveQuest>= 0 && _quests[_currentActiveQuest].HasLimitTime)
            {
                _quests[_currentActiveQuest].RemainingTime -= Time.deltaTime;
                ShowInformationOfActiveQuest();
                if (_quests[_currentActiveQuest].RemainingTime <= 0)
                {
                    // Quest has finished. 
                    _quests[_currentActiveQuest].TryComplete();
                    OnCompleteQuestLine?.Invoke(_quests[_currentActiveQuest].GetProgress());
                    _quests[_currentActiveQuest].Reset();
                }
            }
        }


        public void StartQuestline(IQuest[] quests)
        {
            _quests = quests;
            if (_quests == null)
            {
                questUI.SetActive(false);
                return;
            }
                
            for(int i = 0; i < _quests.Length; i++)
            {
                _quests[i]?.Reset();
            }
            // Start first quest
            StartCoroutine(NextQuest(1));
        }

        public static void ResetSingleton()
        {
            Destroy(_instance);
            _instance = null;
        }

        public void CompleteQuest(IQuest quest)
        {
            if (_quests == null)
                return;
            SoundManager.Instance.PlaySound(SoundType.COMPLETE_QUEST, 1, false);
            StartCoroutine(NextQuest(1));
        }
        private bool ActivateNextQuest()
        {
            if (_quests == null)
                return false;

            if (_currentActiveQuest < _quests.Length - 1)
            {
                _quests[++_currentActiveQuest].ActivateQuest();
                textInfoQuest.text = _quests[_currentActiveQuest].QuestDescription;
                textInfoQuestStriked.text = textInfoQuest.text;
                ShowInformationOfActiveQuest();
                return true;
            } else
            {
                return false;
            }
                
        }

        public void ShowInformationOfActiveQuest()
        {
            if (_quests == null)
                return;
            textQuestState.text = _quests[_currentActiveQuest].GetStateInString();
        }

        public IEnumerator NextQuest(float duration)
        {
            float elapsed = 0;
            // Strike
            if (_currentActiveQuest != -1) {
                textInfoQuestStriked.color = new Color(textInfoQuest.color.r, textInfoQuest.color.g, textInfoQuest.color.b, 1);
                check.color = new Color(textInfoQuest.color.r, textInfoQuest.color.g, textInfoQuest.color.b, 1);
                elapsed = 0;
                while (elapsed <= duration)
                {
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                elapsed = 0;
                // Fade out
                while (elapsed <= duration)
                {
                    textInfoQuest.color = new Color(textInfoQuest.color.r, textInfoQuest.color.g, textInfoQuest.color.b, math.lerp(1, 0, elapsed / duration));
                    textInfoQuestStriked.color = new Color(textInfoQuest.color.r, textInfoQuest.color.g, textInfoQuest.color.b, math.lerp(1, 0, elapsed / duration));
                    textQuestState.color = new Color(textInfoQuest.color.r, textInfoQuest.color.g, textInfoQuest.color.b, math.lerp(1, 0, elapsed / duration));
                    check.color = new Color(textInfoQuest.color.r, textInfoQuest.color.g, textInfoQuest.color.b, math.lerp(1, 0, elapsed / duration));
                    checkBox.color = new Color(textInfoQuest.color.r, textInfoQuest.color.g, textInfoQuest.color.b, math.lerp(1, 0, elapsed / duration));
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                elapsed = 0;
            }

            textInfoQuestStriked.color = new Color(textInfoQuest.color.r, textInfoQuest.color.g, textInfoQuest.color.b, 0);
            check.color = new Color(textInfoQuest.color.r, textInfoQuest.color.g, textInfoQuest.color.b, 0);

            if (!ActivateNextQuest())
            {
                // Quest line finished. Disable UI
                questUI.SetActive(false);
                // End of level UI
                OnCompleteQuestLine?.Invoke(1);
            } else
            {
                // Fade in
                while (elapsed <= duration)
                {
                    textInfoQuest.color = new Color(textInfoQuest.color.r, textInfoQuest.color.g, textInfoQuest.color.b, math.lerp(0, 1, elapsed / duration));
                    textQuestState.color = new Color(textInfoQuest.color.r, textInfoQuest.color.g, textInfoQuest.color.b, math.lerp(0, 1, elapsed / duration));
                    checkBox.color = new Color(textInfoQuest.color.r, textInfoQuest.color.g, textInfoQuest.color.b, math.lerp(0, 1, elapsed / duration));
                    elapsed += Time.deltaTime;
                    yield return null;
                }
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
