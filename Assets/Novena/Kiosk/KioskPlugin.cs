using System;
using UnityEngine;

namespace Kiosk
{
  public static class KioskPlugin
  {
    private const string PluginClassName = "hr.novena.kiosk.Kiosk";
    private static AndroidJavaObject _pluginClass;

    public static void Initialize()
    {
      AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
      AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
      _pluginClass = new AndroidJavaObject(PluginClassName);

      if (_pluginClass != null)
      {
        _pluginClass.Call("Initialize", activity);
      }
      else
      {
        Debug.LogError("No plugin class: " + PluginClassName + " or plugin is not included in Plugins folder!");
      }
    }

    public static void Start()
    {
			if (_pluginClass != null)
			{
      _pluginClass.Call("EnableKioskMode");
			}
    }
    
    public static void Stop()
    {
      if (_pluginClass != null)
      { 
        _pluginClass.Call("DisableKioskMode");      
      }
    }

    public static void OnApplicationFocus(bool hasFocus)
    {
      if (_pluginClass != null)
      {
        _pluginClass.Call("OnApplicationFocus", hasFocus);
      }
    }
  }
}