using Assets.Scripts.Player;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Entities;
using Assets.Scripts.Systems;
using Assets.Scripts.Player.QuestSystem;
using Assets.Scripts.Sound;
using Assets.Scripts.Player.LevelSystem;
using System.Collections;

namespace Assets.Scripts.UI
{
    public class MenuHandler : MonoBehaviour
    {
        public static MenuHandler Instance { get { return _instance; } }
        private static MenuHandler _instance;
        [SerializeField] private string GameScene;
        [SerializeField] private string MainMenuScene;

        [SerializeField] private GameObject mainMenu;
        [SerializeField] private GameObject levelMenu;
        [SerializeField] private GameObject settingsMenu;
        [SerializeField] private GameObject helpMenu;

        [SerializeField] private GameObject ingameUI;

        [SerializeField] private Slider sensitivitySlider;
        [SerializeField] private Slider volumeSlider;
        private TMP_Text sensitivityText;
        private TMP_Text volumeText;

        [SerializeField] private GameObject loadingText;


        // Simulation control
        [SerializeField] private GameObject simulationControl;
        [SerializeField] private Image accelerateIcon;
        [SerializeField] private Image slowDownIcon;
        [SerializeField] private GameObject pauseIcon;
        [SerializeField] private GameObject resumeIcon;
        [SerializeField] private TMP_Text speedText;

        // Player modes
        [SerializeField] private GameObject spectatorButton;
        [SerializeField] private GameObject beeButton;
        private bool _spectatorMode = true;

        // Follow selected entity
        [SerializeField] private GameObject followButton;
        [SerializeField] private GameObject unfollowButton;

        public bool Paused { get { return _paused; } }
        private bool _paused;
        public bool Loading { get { return _loading; } }
        private bool _loading;

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            sensitivityText = sensitivitySlider.GetComponentInChildren<TMP_Text>();
            volumeText = volumeSlider.GetComponentInChildren<TMP_Text>();
            Globals.SIMULATION_PAUSED = false;
            if(levelMenu!=null)
                levelMenu.SetActive(false);
        }

        private void Update()
        {
            if (ingameUI == null)
                return;
            if (Input.GetKeyDown(KeyCode.Escape)&& !_paused)
            {
                OnPause();
            }
            if (_paused ||accelerateIcon==null || !_spectatorMode)
            {
                return;
            }
            if(Input.GetKeyDown(KeyCode.RightArrow) && !Globals.SIMULATION_PAUSED)
            {
                // ACCELERATE SIMULATION
                if(Globals.SIMULATION_SPEED_SELECTED >= Globals.SIMULATION_SPEEDS.Length-1)
                {
                    accelerateIcon.color = Color.gray;
                } else
                {
                    accelerateIcon.color = Color.white;
                    slowDownIcon.color = Color.white;
                    Globals.SIMULATION_SPEED_SELECTED++;
                    speedText.text = "x" + Globals.SIMULATION_SPEED;
                    if (Globals.SIMULATION_SPEED_SELECTED >= Globals.SIMULATION_SPEEDS.Length - 1){
                        accelerateIcon.color = Color.gray;
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow) && !Globals.SIMULATION_PAUSED)
            {
                // SLOW DOWN SIMULATION
                if (Globals.SIMULATION_SPEED_SELECTED <= 0)
                {
                    slowDownIcon.color = Color.gray;
                }
                else
                {
                    slowDownIcon.color = Color.white;
                    accelerateIcon.color = Color.white;
                    Globals.SIMULATION_SPEED_SELECTED--;
                    speedText.text = "x" + Globals.SIMULATION_SPEED;
                    if (Globals.SIMULATION_SPEED_SELECTED <= 0)
                    {
                        slowDownIcon.color = Color.gray;
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                // Pause or Resume SIMULATION
                Globals.SIMULATION_PAUSED = !Globals.SIMULATION_PAUSED;
                if(Globals.SIMULATION_PAUSED)
                {
                    pauseIcon.SetActive(false);
                    resumeIcon.SetActive(true);
                } else
                {
                    pauseIcon.SetActive(true);
                    resumeIcon.SetActive(false);
                }
            }
        }
        
        public void SwitchMode(bool isSpectatorMode)
        {
            if(isSpectatorMode)
            {
                beeButton.SetActive(true);
                spectatorButton.SetActive(false);
            } else
            {
                beeButton.SetActive(false);
                spectatorButton.SetActive(true);
            }
        }

        public void SelectEntity(bool selected, bool followMode)
        {
            if(selected)
            {
                if(followMode)
                {
                    followButton.SetActive(false);
                    unfollowButton.SetActive(true);
                }
                else
                {
                    followButton.SetActive(true);
                    unfollowButton.SetActive(false);
                }
                
            } else
            {
                followButton.SetActive(false);
                unfollowButton.SetActive(false);
            }
        }

        public void OnNewGame()
        {
            // Open level selector menu
            SoundManager.Instance.PlaySound(SoundType.BUTTON_CLICK, 1, true);
            mainMenu.SetActive(false);
            if(LevelMenuHandler.Instance.LevelsCreated==0)
            {
                levelMenu = LevelMenuHandler.Instance.gameObject;
                LevelHandler.Instance.OnDisplayLevelSelector();
            }
            levelMenu.SetActive(true);
            
        }

        public void OnPlay()
        {
            if (LevelHandler.Instance.SelectedLevel == -1)
            {
                return;
            }
            SceneManager.LoadScene("SampleScene");
            //SceneManager.LoadScene("EntitySubscene", LoadSceneMode.Additive);
            Unity.Entities.World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<ColonySpawnerSystem>().Enabled = true;
            Unity.Entities.World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<SelectionSystem>().Enabled = true;
            Unity.Entities.World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<HandleScans>().FirstUpdate = true;
            Unity.Entities.World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<HandleScans>().Enabled = true;
            Unity.Entities.World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<BeeSoundSystem>().Enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            SoundManager.Instance.PlaySound(SoundType.BUTTON_CLICK, 1, true);
            SoundManager.Instance.PauseAllSounds(false);
        }

        public void OnSettings()
        {
            mainMenu.SetActive(false);
            settingsMenu.SetActive(true);
            sensitivitySlider.value = SettingsHandler.Instance.Sensitivity;
            volumeSlider.value = SettingsHandler.Instance.Volume;
            SoundManager.Instance.PlaySound(SoundType.BUTTON_CLICK, 1, true);
        }

        public void OnHelp()
        {
            mainMenu.SetActive(false);
            helpMenu.SetActive(true);
            //sensitivitySlider.value = SettingsHandler.Instance.Sensitivity;
            SoundManager.Instance.PlaySound(SoundType.BUTTON_CLICK, 1, true);
        }

        public void OnQuit()
        {
            LevelHandler.Instance.SaveLevelsProgress();
            Application.Quit();
        }

        public void OnBackToMainMenu()
        {
            mainMenu.SetActive(true);
            settingsMenu.SetActive(false);
            helpMenu.SetActive(false);
            SoundManager.Instance.PlaySound(SoundType.BUTTON_CLICK, 1, true);
        }

        public void OnLeaveGame()
        {
            Cursor.lockState = CursorLockMode.None;
            LevelHandler.Instance.UnsubscribeFromEvents();
            Unity.Entities.World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<SelectionSystem>().Enabled = false;
            Unity.Entities.World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<HandleScans>().Enabled = false;
            //Unity.Entities.World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<BeeSoundSystem>().Enabled = false;
            Unity.Entities.World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<HandlePolenRequestsSystem>().ClearRequests();
            QuestSystem.Instance.StopAllCoroutines();
            QuestSystem.ResetSingleton();
            //Unity.Entities.World.DisposeAllWorlds();
            SceneManager.LoadScene("MainMenuSecond");
            SoundManager.Instance.PlaySound(SoundType.BUTTON_CLICK, 1, true);
            //SoundManager.Instance.PauseAllSounds(true);
            SoundManager.Instance.RemoveSounds();
        }

        public void OnPause()
        {
            _paused = true;
            Cursor.lockState = CursorLockMode.None;
            mainMenu.SetActive(true);
            //Time.timeScale = 0;
            ingameUI.SetActive(false);
            SoundManager.Instance.PauseAllSounds(true);
            SoundManager.Instance.PlaySound(SoundType.BUTTON_CLICK, 1, true);
            SoundManager.Instance.PauseAmbient(true);
            Globals.SIMULATION_PAUSED = true;
        }

        public void OnLoadScreen()
        {
            _loading = true;
            Cursor.lockState = CursorLockMode.None;
            ingameUI.SetActive(false);
            loadingText.SetActive(true);
        }
        public void OnFinishLoading()
        {
            Cursor.lockState = CursorLockMode.Locked;
            _loading = false;
            ingameUI.SetActive(true);
            loadingText.SetActive(false);
            SoundManager.Instance.PauseAmbient(false);
            LevelHandler.Instance.OnPlay();
        }
        public void OnUnpause()
        {
            Cursor.lockState = CursorLockMode.Locked;
            mainMenu.SetActive(false);
            settingsMenu.SetActive(false);
            //Time.timeScale = 1;
            _paused = false;
            ingameUI.SetActive(true);
            SoundManager.Instance.PauseAllSounds(false);
            SoundManager.Instance.PlaySound(SoundType.BUTTON_CLICK, 1, true);
            SoundManager.Instance.PauseAmbient(false);
            Globals.SIMULATION_PAUSED = false;
        }

     

        public void OnChangeSlider(Slider slider)
        {
            if(slider.gameObject.name=="SensitivitySlider")
            {
                SettingsHandler.Instance.Sensitivity = slider.value;
                sensitivityText.text = slider.value + "";
            } else
            {
                SettingsHandler.Instance.Volume = slider.value;
                volumeText.text = slider.value + "";
                AudioListener.volume = slider.value / 100;
            }
            
        }

        public void OnChangeInvertMouseX(Toggle change)
        {
            if(change.gameObject.name.Equals("InvertMouseX"))
            {
                SettingsHandler.Instance.InvertMouseX = change.isOn;
            } else if (change.gameObject.name.Equals("InvertMouseY"))
            {
                SettingsHandler.Instance.InvertMouseY = change.isOn;
            } else
            {
                Debug.Log(change.gameObject.name);
            }
        }
    }
}
