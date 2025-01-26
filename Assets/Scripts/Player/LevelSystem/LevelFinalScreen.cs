using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using System.Collections;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Player.LevelSystem
{
    public class LevelFinalScreen : MonoBehaviour
    {
        public static LevelFinalScreen Instance;

        [SerializeField] private Image bgImage;
        [SerializeField] private TMP_Text lvlName;
        [SerializeField] private TMP_Text lvlDesc;
        [SerializeField] private Star[] stars;

        private void Awake()
        {
            Instance = this;
            
        }
        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void Display(Level level, float progress)
        {
            lvlName.text = level.Name;
            lvlDesc.text = level.Description;
            gameObject.SetActive(true);
            StartCoroutine(PlayAnimation(progress));
        }

        public IEnumerator PlayAnimation(float progress)
        {
            SoundManager.Instance.RemoveSounds();
            float duration = 1;
            float elapsed = 0;
            int nStars = (int)(progress * 5);
            float pitch = math.lerp(0.75f, 1.25f, progress);
            SoundManager.Instance.PlaySound(SoundType.COMPLETE_LEVEL, 1, pitch, pitch);
            bgImage.color = new Color(bgImage.color.r, bgImage.color.g, bgImage.color.b, 0);
            lvlName.color = new Color(lvlName.color.r, lvlName.color.g, lvlName.color.b, 0);
            lvlDesc.color = new Color(lvlDesc.color.r, lvlDesc.color.g, lvlDesc.color.b, 0);
            while (elapsed<=duration)
            {
                bgImage.color = new Color(bgImage.color.r, bgImage.color.g, bgImage.color.b, elapsed / duration);
                lvlName.color = new Color(lvlName.color.r, lvlName.color.g, lvlName.color.b, elapsed / duration);
                lvlDesc.color = new Color(lvlDesc.color.r, lvlDesc.color.g, lvlDesc.color.b, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            bgImage.color = new Color(bgImage.color.r, bgImage.color.g, bgImage.color.b,1);
            lvlName.color = new Color(lvlName.color.r, lvlName.color.g, lvlName.color.b,1);
            lvlDesc.color = new Color(lvlDesc.color.r, lvlDesc.color.g, lvlDesc.color.b,1);
            elapsed = 0;
            while (elapsed <= duration)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            elapsed = 0;
            duration = 2;
            int lastIndex = -1;
            while (elapsed <= duration)
            {
                int i = (int)((elapsed / duration) * 5);
                if(i<nStars && lastIndex!=i)
                {
                    stars[i].SetStarState(true);
                    lastIndex = i;
                }
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            elapsed = 0;
            
            while (elapsed <= duration)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
            MenuHandler.Instance.OnLeaveGame();
        }

    }
}
