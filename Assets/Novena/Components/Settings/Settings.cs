using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Novena.Settings
{
  public static class Settings
  {
    /// <summary>
    /// When settings are updated.
    /// </summary>
    public static Action OnSettingsUpdate;
    
    private static SettingsManager _settingsManager;
    static Settings()
    {
      if (_settingsManager != null) return;
      _settingsManager = Object.FindObjectOfType<SettingsManager>();
    }
    
    /// <summary>
    /// Search's for property by name and returns it's value.
    /// </summary>
    /// <remarks>
    /// If type conversion is unsuccessful or field is not found it will return default value. 
    /// </remarks>
    /// <param name="name">Name of property.</param>
    /// <typeparam name="T">Int, float, string...</typeparam>
    /// <returns>Value converted to Type(T).</returns>
    /// <example>
    /// <code>
    /// var value = Settings.GetValue&lt;float&gt;("IdleTimer");
    /// </code>
    /// </example>
    public static T GetValue<T>(string name)
    {
      try
      {
        var settingItem = GetSettingItem(name);
        
        if (settingItem != null)
        {
          return settingItem.Value.ChangeType<T>();
        }
      }
      catch (Exception e)
      {
        Debug.LogException(e);
      }
      
      return default;
    }
    
    /// <summary>
    /// Search's for property by name and returns it's value.
    /// </summary>
    /// <remarks>
    /// If type conversion is unsuccessful or field is not found it will return default value. 
    /// </remarks>
    /// <param name="name">Name of property.</param>
    /// <typeparam name="T">Int, float, string...</typeparam>
    /// <returns>Value converted to Type(T).</returns>
    /// <example>
    /// <code>
    /// var value = value.GetSettingsValue&lt;float&gt;("IdleTimer");
    /// </code>
    /// </example>
    public static T GetSettingsValue<T>(this object obj, string name)
    {
      try
      {
        var settingItem = GetSettingItem(name);
        
        if (settingItem != null)
        {
          var value = settingItem.Value.ChangeType<T>();
          return value;
        }
      }
      catch (Exception e)
      {
        Debug.LogException(e);
      }

      return default;
    }

    /// <summary>
    /// Find setting item by name!
    /// </summary>
    /// <param name="name"></param>
    /// <returns>SettingItem. NULL if nothing found!</returns>
    private static SettingItem GetSettingItem(string name)
    {
      SettingItem output = null;

      output = _settingsManager.SettingItems.FirstOrDefault(s => s.Name == name);

      return output;
    }
    
    private static T ChangeType<T>(this object obj)
    {
      return (T)Convert.ChangeType(obj, typeof(T));
    }
  }
}