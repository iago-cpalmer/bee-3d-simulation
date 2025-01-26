using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Player.LevelSystem
{
    public class Star : MonoBehaviour
    {
        [SerializeField] private Image starImage;
        [SerializeField] private Sprite fullStar;
        [SerializeField] private Sprite emptyStar;
        public bool isFull;

        private void Start()
        {
            SetStarState(isFull);
        }

        public void SetStarState(bool full)
        {
            isFull = full;
            if(isFull)
            {
                starImage.sprite = fullStar;
            } else
            {
                starImage.sprite = emptyStar;
            }
        }
    }
}
