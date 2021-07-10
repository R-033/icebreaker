using UnityEngine;

namespace Hypercooled.Managed
{
	class Configuration
	{
#if !UNITY_WEBGL
		public static string LastSelectedProjectPath
		{
			get
			{
				return PlayerPrefs.GetString("ProjectPath");
			}
			set
			{
				PlayerPrefs.SetString("ProjectPath", value);
			}
		}
#endif
	}
}
