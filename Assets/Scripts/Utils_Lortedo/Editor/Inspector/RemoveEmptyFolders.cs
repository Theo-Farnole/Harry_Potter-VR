using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System;

// code from
// https://gist.github.com/mob-sakai/b98a62c1f2c94c1fef9021101635a5cd

/// <summary>
/// Remove empty folders automatically.
/// </summary>
public class RemoveEmptyFolders : UnityEditor.AssetModificationProcessor
{
	public const string kMenuText = "Assets/Remove Empty Folders";
	static readonly StringBuilder s_Log = new StringBuilder();
	static readonly List<DirectoryInfo> s_Results = new List<DirectoryInfo>();

	/// <summary>
	/// Raises the will save assets event.
	/// </summary>
	static string[] OnWillSaveAssets(string[] paths)
	{
		// If menu is unchecked, do nothing.
		if (!EditorPrefs.GetBool(kMenuText, true))
			return paths;
	
		// Get empty directories in Assets directory
		s_Results.Clear();
		var assetsDir = Application.dataPath + Path.DirectorySeparatorChar;
		GetEmptyDirectories(new DirectoryInfo(assetsDir), s_Results);

		// When empty directories has detected, remove the directory.
		if (0 < s_Results.Count)
		{
			s_Log.Length = 0;
			s_Log.AppendFormat("Remove {0} empty directories as following:\n", s_Results.Count);
			foreach (var d in s_Results)
			{
				s_Log.AppendFormat("- {0}\n", d.FullName.Replace(assetsDir, ""));
				FileUtil.DeleteFileOrDirectory(d.FullName);
			}

			// UNITY BUG: Debug.Log can not set about more than 15000 characters.
			s_Log.Length = Mathf.Min(s_Log.Length, 15000);
			Debug.Log(s_Log.ToString());
			s_Log.Length = 0;

			AssetDatabase.Refresh();
		}
		return paths;
	}

	/// <summary>
	/// Get empty directories.
	/// </summary>
	static bool GetEmptyDirectories(DirectoryInfo dir, List<DirectoryInfo> results)
	{
		bool isEmpty = true;
		try
		{
			isEmpty = dir.GetDirectories().Count(x => !GetEmptyDirectories(x, results)) == 0	// Are sub directories empty?
					&& dir.GetFiles("*.*").All(x => x.Extension == ".meta");	// No file exist?
		}
		catch
		{
		}

		// Store empty directory to results.
		if (isEmpty)
			results.Add(dir);
		return isEmpty;
	}
}