using Hypercooled.Utils;
using UnityEditor;

namespace Hypercooled.Editor
{
	public class ExportInfo
	{
		private static void Run()
		{
			foreach (var selected in Selection.gameObjects)
			{
				if (selected == null)
				{
					// TODO: handle errors
					continue;
				}

				//ObjectExport.ExportObj(selected, true);
			}
		}

		[MenuItem("GameObject/Hypercooled/Export Model", false, -20)]
		private static void ExportModel()
		{
			Run();
		}
	}
}
