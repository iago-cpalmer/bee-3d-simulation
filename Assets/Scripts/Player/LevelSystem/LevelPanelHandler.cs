using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Player.LevelSystem
{
    public class LevelPanelHandler : MonoBehaviour
    {
        public int LevelId;
        [SerializeField] private TMP_Text lvlName;
        [SerializeField] private TMP_Text description;
        [SerializeField] private Image panelImage;
        [SerializeField] private Color selectedColor;
        [SerializeField] private Color unselectedColor;
        public void OnSelect()
        {
            LevelHandler.Instance.SelectLevel(LevelId);
            panelImage.color = selectedColor;
        }

        public void Unselect()
        {
            panelImage.color = unselectedColor;
        }

        public void SetInfo(Level level)
        {
            LevelId = level.LevelId;
            lvlName.text = level.Name;
            description.text = level.Description;
        }
    }
}
