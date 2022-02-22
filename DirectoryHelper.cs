// Author: Anja Haumann 2018 - MIT License
// Explanation and more content at my blog: http://anja-haumann.de/unity-how-to-save-on-sd-card/

using UnityEngine;

public static class DirectoryHelper
{
	public static string GetAndroidExternalFilesDir()
	{
		using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaObject context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				// Get all available external file directories (emulated and sdCards)
				AndroidJavaObject[] externalFilesDirectories =
									context.Call<AndroidJavaObject[]>
									("getExternalFilesDirs", (object)null);

				AndroidJavaObject emulated = null;
				AndroidJavaObject sdCard = null;

				for (int i = 0; i < externalFilesDirectories.Length; i++)
				{
					AndroidJavaObject directory = externalFilesDirectories[i];
					using (AndroidJavaClass environment = new AndroidJavaClass("android.os.Environment"))
					{
						// Check which one is the emulated and which the sdCard.
						bool isRemovable = environment.CallStatic<bool>
										  ("isExternalStorageRemovable", directory);
						bool isEmulated = environment.CallStatic<bool>
										  ("isExternalStorageEmulated", directory);
						if (isEmulated)
							emulated = directory;
						else if (isRemovable && isEmulated == false)
							sdCard = directory;
					}
				}
				// Return the sdCard if available
				if (sdCard != null)
					return sdCard.Call<string>("getAbsolutePath");
				else
					return emulated.Call<string>("getAbsolutePath");
			}
		}
	}
}
