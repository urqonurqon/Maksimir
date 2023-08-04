using UnityEngine;

namespace Novena.Utility
{
  public static class PlayerPrefHelper
  {
    #region Save

    public static void SaveValue(string value, PlayerPrefKey key) 
    {
      PlayerPrefs.SetString(key.ToString(), value);
    }
    
    public static void SaveValue(int value, PlayerPrefKey key) 
    {
      PlayerPrefs.SetInt(key.ToString(), value);
    }

    #endregion
    
    

    #region Get

    public static string GetStringValue(PlayerPrefKey key)
    {
      return PlayerPrefs.GetString(key.ToString());
    }
    
    public static int GetIntValue(PlayerPrefKey key)
    {
      return PlayerPrefs.GetInt(key.ToString(), 0);
    }

    #endregion

  }

  public enum PlayerPrefKey
  {
    IsOnBoardSeen = 1
  }
}