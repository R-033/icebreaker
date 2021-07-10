using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hypercooled.Managed.Menus
{
    public class ProjectCreationMenu : MonoBehaviour
    {
        public MainMenu mainMenu;
        public InputField ProjectPath;
        public Dropdown GameType;
        public InputField GlobalBPath;
        public InputField TrackPath;
        public Button CreateProjectButton;
        public UIScript mainScreen;

        void OnEnable()
        {
            this.ValidateInput();
        }

        public void ValidateInput()
        {
            this.CreateProjectButton.interactable =
                !string.IsNullOrWhiteSpace(this.ProjectPath.text) &&
                !string.IsNullOrWhiteSpace(this.GlobalBPath.text) &&
                !string.IsNullOrWhiteSpace(this.TrackPath.text) &&
                System.IO.File.Exists(this.GlobalBPath.text) &&
                System.IO.File.Exists(this.TrackPath.text);
        }

        public void ToggleMenu(bool enable)
		{
			this.gameObject.SetActive(enable);
		}

        public void /*ReturnToMonke()*/ReturnToMainMenu()
        {
            this.ToggleMenu(false);
        }

        public void OpenProjectPathPicker()
        {
            this.ProjectPath.text = this.mainMenu.MainScript.SelectAndVerifyUnpackDirectory(this.ProjectPath.text);
        }

        public void OpenGlobalBPicker()
        {
            this.GlobalBPath.text = this.mainMenu.MainScript.SelectAndVerifyGlobalFile(this.GlobalBPath.text);
            
        }

        public void OpenTrackPathPicker()
        {
            this.TrackPath.text = this.mainMenu.MainScript.SelectAndVerifyLocationFile(this.TrackPath.text);
        }

        public void CreateAndOpenProject()
        {
            this.mainMenu.ToggleMenu(false);
            this.ToggleMenu(false);
            Game game;
            switch (this.GameType.value)
            {
                case 0:
                    game = Game.Underground1;
                    break;
                case 1:
                    game = Game.Underground2;
                    break;
                case 2:
                    game = Game.MostWanted;
                    break;
                default:
                    game = Game.Carbon;
                    break;
            }
            this.mainMenu.MainScript.StartCoroutine(this.mainMenu.MainScript.UnpackProject(game, this.GlobalBPath.text, this.TrackPath.text, this.ProjectPath.text));
            this.mainScreen.ToggleMenu(true);
        }
    }
}
