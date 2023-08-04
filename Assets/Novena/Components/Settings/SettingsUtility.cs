using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace Novena.Settings
{
  public static class SettingsUtility
  {
    private static string FilePath => GetFilePath();
    
    /// <summary>
    /// Save settings to file.
    /// </summary>
    /// <param name="items"></param>
    public static void Save(List<SettingItem> items)
    {
      var json = JsonConvert.SerializeObject(items);
      SaveToFile(json);
      Settings.OnSettingsUpdate?.Invoke();
    }
    
    /// <summary>
    /// Load settings from file.
    /// </summary>
    /// <returns>List of setting items</returns>
    public static List<SettingItem> Load()
    {
      List<SettingItem> output = new List<SettingItem>();

      //File doesn't exist. Return empty list.
      if (File.Exists(FilePath) == false) return output;

      var json = File.ReadAllText(FilePath);

      if (string.IsNullOrEmpty(json) == false)
      {
        output = JsonConvert.DeserializeObject<List<SettingItem>>(json);
      }

      Settings.OnSettingsUpdate?.Invoke();
      
      return output;
    }

    private static void SaveToFile(string json)
    {
      File.WriteAllText(FilePath, json);
    }

    /// <summary>
    /// Get file path of settings.json.
    /// Creates directory if doesn't exist.
    /// </summary>
    /// <returns>File path</returns>
    private static string GetFilePath()
    {
      StringBuilder strBuilder = new StringBuilder();
      strBuilder.Append(Application.streamingAssetsPath);
      strBuilder.Append("/Settings");
      
      if (Directory.Exists(strBuilder.ToString()) == false)
      {
        Directory.CreateDirectory(strBuilder.ToString());
      }
      
      strBuilder.Append("/settings.json");

      return strBuilder.ToString();
    }
  }
}