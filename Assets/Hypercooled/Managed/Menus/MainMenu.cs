using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hypercooled.Managed.Menus
{
	public class MainMenu : MonoBehaviour
	{
		public Main MainScript;

		public ProjectCreationMenu projectCreationMenu;

		public UIScript mainScreen;

		public Text Version;

		public class ProjectMeta
		{
			public Hypercooled.Shared.Core.ProjectHeader header;
			public string projectFilePath;
		}

		void Start()
		{
			Version.text = "Version " + Application.version;
		}

		public void CreateNewProject()
		{
			this.projectCreationMenu.ToggleMenu(true);
		}

		public void LoadExistingProject()
		{
			string path = this.MainScript.SelectExistingProject();
			if (path == string.Empty)
				return;
			Configuration.LastSelectedProjectPath = path;
			ProjectMeta metadata = this.GetProjectHeader(Configuration.LastSelectedProjectPath);
			this.MainScript.StartCoroutine(MainScript.LoadTrackProject(metadata.header.GameType, metadata.projectFilePath));
			this.ToggleMenu(false);
			this.mainScreen.ToggleMenu(true);
		}

		public void ShowRecentsMenu()
		{
			// todo for now
			ProjectMeta metadata = this.GetProjectHeader(Configuration.LastSelectedProjectPath);
			this.MainScript.StartCoroutine(MainScript.LoadTrackProject(metadata.header.GameType, metadata.projectFilePath));
			this.ToggleMenu(false);
			this.mainScreen.ToggleMenu(true);
		}

		public void ShowAbout()
		{
			// uhh
		}

		public void ToggleMenu(bool enable)
		{
			this.gameObject.SetActive(enable);
		}

		public ProjectMeta GetProjectHeader(string path)
		{
			ProjectMeta result = new ProjectMeta();
			result.header = Newtonsoft.Json.JsonConvert.DeserializeObject<Hypercooled.Shared.Core.ProjectHeader>(System.IO.File.ReadAllText(path));
			result.projectFilePath = path;
			return result;
		}
	}
}
