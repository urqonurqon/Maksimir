using System;
using System.Linq;
using Novena.Settings.Enums;
using UnityEditor;
using UnityEngine;

namespace Novena.Settings.Editor
{
  /*
   * SettingsManager script has to be ADDED in Project Settings => Script Execution Order and set to -1
   */
  [CustomEditor(typeof(SettingsManager)), CanEditMultipleObjects]
  public class SettingsManagerEditor : UnityEditor.Editor
  {
    public FieldType FieldType;
    public string FieldName;
    public string FieldValue;
    private string FieldDescription;
    
    private SettingsManager _settingsManager;

    private void Awake()
    {
      if (_settingsManager == null)
        _settingsManager = (SettingsManager)target;
      
      _settingsManager.LoadSettings();
    }

    public override void OnInspectorGUI()
    {
      DrawInfo();
      DrawPropertyCreator();
      DrawListOfProperties();
    }

    private void DrawInfo()
    {
      EditorGUILayout.Space(10);

      EditorGUILayout.LabelField(@"INFO: This script has to be ADDED in Project Settings => Script Execution Order and set to -1");
      
      EditorGUILayout.Space(10);
    }

    private void DrawListOfProperties()
    {
      EditorGUILayout.Space(20);

      GUIStyle style = new GUIStyle();
      style.alignment = TextAnchor.MiddleCenter;
      style.normal.textColor = Color.white;

      EditorGUILayout.LabelField("PROPERTIES", style);

      EditorGUILayout.Space(10);

      try
      {
        foreach (var setting in _settingsManager.SettingItems)
        {
          EditorGUILayout.BeginHorizontal();
        
          setting.Name = EditorGUILayout.TextField(setting.Name, GUILayout.Width(200));

          Enum.TryParse(setting.Type, out FieldType type);
          var fieldType = type;

          setting.Type = EditorGUILayout.EnumPopup(fieldType, GUILayout.Width(100)).ToString();
        
          setting.Value = EditorGUILayout.TextField(setting.Value);
        
          if (GUILayout.Button("REMOVE"))
            RemoveItem(setting.Name);
        
          EditorGUILayout.EndHorizontal();

          setting.Description = EditorGUILayout.TextArea(setting.Description);
          
          EditorGUILayout.Space();
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
      
      EditorGUILayout.Space(10);

      if (_settingsManager.SettingItems.Any())
      {
        if (GUILayout.Button("SAVE"))
          SaveChanges();
      }
    }

    private void DrawPropertyCreator()
    {
      GUIStyle style = new GUIStyle();
      style.alignment = TextAnchor.MiddleCenter;
      style.normal.textColor = Color.white;

      EditorGUILayout.LabelField("PROPERTY CREATOR", style);

      EditorGUILayout.Separator();

      FieldName = EditorGUILayout.TextField("Name:", FieldName);

      EditorGUILayout.Space();

      FieldType = (FieldType)EditorGUILayout.EnumPopup("Type:", FieldType);

      EditorGUILayout.Space();

      FieldValue = EditorGUILayout.TextField("Value", FieldValue);
      
      EditorGUILayout.Space();
      
      FieldDescription = EditorGUILayout.TextField("Description", FieldDescription);

      EditorGUILayout.Space(5);

      if (GUILayout.Button("ADD PROPERTY"))
        AddSettingItem();
    }

    void AddSettingItem()
    {
      SettingItem settingItem = new SettingItem();

      settingItem.Name = FieldName;
      settingItem.Type = FieldType.ToString();
      settingItem.Value = FieldValue;
      settingItem.Description = FieldDescription;

      _settingsManager.AddNewItem(settingItem);
    }

    void RemoveItem(string fieldName)
    {
      _settingsManager.DeleteItem(fieldName);
      SaveChanges();
    }

    void SaveChanges()
    {
      _settingsManager.SaveSettings();
    }
  }
}