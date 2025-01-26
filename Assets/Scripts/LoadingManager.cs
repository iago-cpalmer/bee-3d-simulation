using Assets.Scripts.UI;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    class LoadingManager : MonoBehaviour
    {
        [SerializeField] private ProgressBarHandler progressBar;
        [SerializeField] private TMP_Text curiosityText;
        [SerializeField] private string[] curiosities;
        private float timer;

        private void Start()
        {
            MenuHandler.Instance.OnLoadScreen();
            Globals.SIMULATION_SPEED_SELECTED = Globals.SIMULATION_SPEEDS.Length - 1;
            Time.timeScale = 1;
            curiosityText.text = curiosities[(int)Time.unscaledTime % curiosities.Length];
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if(timer<=20)
            {
                progressBar.UpdateState(timer / 20);
            } else
            {
                Globals.SIMULATION_SPEED_SELECTED = 3;
                progressBar.gameObject.SetActive(false);
                gameObject.SetActive(false);
                MenuHandler.Instance.OnFinishLoading();
            }
        }
    }
}
