using System.Collections.Generic;
using UnityEngine;

namespace Novena.Settings
{
  public class SettingsManager : MonoBehaviour
  {
    public List<SettingItem> SettingItems { get; private set; }
    
    private void Awake()
    {
      SettingItems = new List<SettingItem>();
      LoadSettings();
    }

    private void Start()
    {
      //This only for initialization and event subscribers.
      Settings.OnSettingsUpdate?.Invoke();
    }

    /// <summary>
    /// Add new item into settings.
    /// </summary>
    /// <param name="settingItem"></param>
    public void AddNewItem(SettingItem settingItem)
    {
      if (string.IsNullOrEmpty(settingItem.Name))
      {
        Debug.LogError($"Unable to add setting item. Name cannot be empty!");
        return;
      }
      
      for (int i = 0; i < SettingItems.Count; i++)
      {
        var item = SettingItems[i];

        if (item.Name.ToLower() == settingItem.Name.ToLower())
        {
          Debug.LogError($"Unable to add setting item. Field with same name {{{settingItem.Name}}} exist");
          return;
        }
      }

      SettingItems.Add(settingItem);
    }

    /// <summary>
    /// Delete item from settings.
    /// </summary>
    /// <param name="fieldName"></param>
    public void DeleteItem(string fieldName)
    {
      for (int i = 0; i < SettingItems.Count; i++)
      {
        var item = SettingItems[i];

        if (item.Name.ToLower() == fieldName.ToLower())
        {
          if (SettingItems.Remove(item) == false)
          {
            Debug.LogError($"Unable to DELETE setting item {fieldName}.");
          }
        }
      }
    }

    public void SaveSettings()
    {
      SettingsUtility.Save(SettingItems);
    }

    public void LoadSettings()
    {
      SettingItems = SettingsUtility.Load();
    }
  }
}