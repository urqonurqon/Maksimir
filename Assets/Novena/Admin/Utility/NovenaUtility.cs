using System;
using UnityEngine;

namespace Novena.Plugin.NovenaUtilityPlugin
{
  public static class NovenaUtility
  {
    /// <summary>
    /// Wrapper class that initialize communication with native code.
    /// </summary>
    private static NovenaUtilityPlugin _novenaUtilityPlugin = new ();

    public static DeviceAdmin DeviceAdmin = new DeviceAdmin(_novenaUtilityPlugin.DeviceAdminJavaObject);
    public static PathProvider PathProvider = new PathProvider(_novenaUtilityPlugin.PathProviderJavaObject);

    /// <summary>
    /// Restarts application.
    /// </summary>
    public static void RestartApplication()
    {
      _novenaUtilityPlugin.UtilityJavaObject.CallStatic("RestartApplication");
    }
  }

  /// <summary>
  /// Class that initialize plugin.
  ///
  /// Provides classes inside plugin to communicate with
  ///
  /// <c>DeviceAdmin, PathProvider ...</c>
  /// </summary>
  class NovenaUtilityPlugin
  {
    public AndroidJavaObject UtilityJavaObject { get; private set; }
    public AndroidJavaObject DeviceAdminJavaObject { get; private set; }
    public AndroidJavaObject PathProviderJavaObject { get; private set; }
    
    #region Private properties

    private const string UtilityClassName = "hr.novena.novena_utility_plugin.Utility";
    
    #endregion
    
    public NovenaUtilityPlugin()
    {
      Initialize();
    }
    
    private void Initialize()
    {
      try
      {
        AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
        UtilityJavaObject = new AndroidJavaObject(UtilityClassName);

        if (UtilityJavaObject != null)
        {
          UtilityJavaObject.CallStatic("Initialize", activity);
          DeviceAdminJavaObject = UtilityJavaObject.GetStatic<AndroidJavaObject>("deviceAdmin");
          PathProviderJavaObject = UtilityJavaObject.GetStatic<AndroidJavaObject>("pathProvider");
        }
        else
        {
          Debug.LogError("No plugin class: " + UtilityClassName + " or plugin is not included in Plugins folder!");
        }
      }
      catch (Exception e)
      {
        Debug.LogWarning("PLUGIN NOT IMPLEMENTED! " + e.Message);
      }
    }
  }
}