using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace StageShooter
{
    /// <summary>
    /// This is an example menu for these demo's
    /// </summary>
    [RequireComponent(typeof(MenuBase))]
    public class GameMenu : MonoBehaviour {

        /// <summary>
        /// Title text for the menu
        /// </summary>
        public Text TopText;

        [Tooltip("Restart button on menu")]
        public Transform RestartButton;

        [Tooltip("Resume button on menu")]
        public Transform ResumeButton;

        [Tooltip("Sub menu")]
        public GameObject SubMenu;
        [Tooltip("Restart button on menu")]
        public GameObject MainMenu;

        public LevelController Controller;

        public GameController GameController;
        private MenuBase mMenuBase;

        private bool bInit = false;
        void OnValidate()
        {
            //if (Controller == null)
                //Debug.LogError("LevelController is null");

            if (GameController == null)
                Debug.LogError("Game Controller is null. From " + this.name );

            mMenuBase = gameObject.GetComponent<MenuBase>();
            if (mMenuBase == null)
                Debug.LogError(name + ": MenuBase component is missing");

        }

        void OnEnable()
        {

            mMenuBase = gameObject.GetComponent<MenuBase>();
            if (mMenuBase == null)
            {
                Debug.LogError(name + ": MenuBase component is missing");
                return;
            }

            if (Controller != null)
            {
                Controller.PlayerLoose += new System.EventHandler(Controller_PlayerLoose);
                Controller.PlayerWin += new System.EventHandler(Controller_PlayerWin);
            }

            GameObject go = GameObject.Find("GameController");

            if (go != null)
            {
                GameController = go.GetComponent<GameController>();
                Debug.Log("Got a game controller" + GameController.myID.ToString());
            }

            bInit = true;
        }

        /// <summary>
        /// Triggered on player win event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Controller_PlayerWin(object sender, System.EventArgs e)
        {
            mMenuBase.Show();
            MainMenu.SetActive(false);
            GameController.Pause();
            ResumeButton.gameObject.SetActive(false);
            SubMenu.SetActive(true);
            TopText.text = "Victory";
        }

        /// <summary>
        /// triggered on player lose event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Controller_PlayerLoose(object sender, System.EventArgs e)
        {
            mMenuBase.Show();
            GameController.Pause();
            ResumeButton.gameObject.SetActive(false);
            MainMenu.SetActive(false);
            SubMenu.SetActive(true);
            TopText.text = "Game Over";
        }

        /// <summary>
        /// Restart button handler
        /// </summary>
        public void ButtonRestartLevelPressed()
        {
            if (Controller == null) return;

            GameController.Instance.RestartLevel();
            GameController.Resume();
        }

        /// <summary>
        /// Resume button handler
        /// </summary>
        public void ButtonResumePressed()
        {
            Debug.Log("Resume pressed");
            GameController.Resume();
            mMenuBase.Hide();
        }

        /// <summary>
        /// Back button handler
        /// </summary>
        public void ButtonBackToMenuPressed()
        {
            //MainMenu.SetActive(true);
            //SubMenu.SetActive(false);
        }

        /// <summary>
        /// Button that goes to the classic shooter example
        /// </summary>
        public void ButtonClassicShooterExamplePressed()
        {
            Application.LoadLevel(0);
            GameController.Resume();
        }

        /// <summary>
        /// Button that goes to the tactical shooter example
        /// </summary>
        public void ButtonTacticalShooterPressed()
        {
            Application.LoadLevel(1);
            GameController.Resume();
        }

        /// <summary>
        /// Button that goes to the weapon example
        /// </summary>
        public void ButtonWeaponExamplesPressed()
        {
            Application.LoadLevel(2);
            GameController.Resume();
        }


        void Update()
        {
            if (bInit == false) return;
            
            if (Input.GetKeyUp(KeyCode.Escape) == true)
            {
                if (mMenuBase.MenuOpen == false)
                {
                    GameController.Pause();

                    mMenuBase.Show();
                    MainMenu.SetActive(false);
                    ResumeButton.gameObject.SetActive(true);
                    SubMenu.SetActive(true);
                    
                    if (Controller == null)
                    {
                        RestartButton.gameObject.SetActive(false);
                    }
                }
                else
                {
                    mMenuBase.Hide();
                    GameController.Resume();
                }
            }
        }

    }
}
