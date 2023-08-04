using UnityEngine;

namespace Novena.Plugin.NovenaUtilityPlugin
{
  public class PathProvider
  {
    public string PersistentDataPath { get; private set; }
    
    private AndroidJavaObject PathProviderJavaObject { get; }

    public PathProvider(AndroidJavaObject androidJavaObject)
    {
      PathProviderJavaObject = androidJavaObject;
      SetPersistentDataPath();
    }

    private void SetPersistentDataPath()
    {
      if (Application.platform == RuntimePlatform.Android && Application.platform != RuntimePlatform.WindowsEditor)
      {
        PersistentDataPath = PathProviderJavaObject.Call<string>("GetExternalFilesDir");  
      }
      else
      {
        PersistentDataPath = Application.persistentDataPath;
      }
    }
  }
}