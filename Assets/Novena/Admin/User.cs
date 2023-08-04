using Novena.Admin.Networking;
using UnityEngine;

namespace Novena.Admin
{
  public class User
  {
    public string Username { get; private set; }
    public string Password { get; private set; }
    public string BasicAuth { get; set; }
    public bool IsRemembered { get; set; }

    private const string USERNAME_KEY = "USER_UserName";
    private const string PASSWWORD_KEY = "USER_Password";
    private const string REMEMBER_KEY = "USER_Remember";

    public User(string username, string password, bool remember)
    {
      PlayerPrefs.SetString(USERNAME_KEY, username);
      PlayerPrefs.SetString(PASSWWORD_KEY, password);
      PlayerPrefs.SetInt(REMEMBER_KEY, remember ? 1:0);

      BasicAuth = Authentication.GetBasicAuth(username, password);
      Username = username;
      Password = password;
      IsRemembered = remember;
    }
    
    /// <summary>
    /// Returns user if was saved and set to remember.
    /// </summary>
    /// <returns>User</returns>
    public static User GetUser()
    {
      string username = PlayerPrefs.GetString(USERNAME_KEY);
      string password = PlayerPrefs.GetString(PASSWWORD_KEY);
      int remember = PlayerPrefs.GetInt(REMEMBER_KEY);

      if (string.IsNullOrEmpty(username) == false)
      {
        if (remember == 1)
        {
          return new User(username, password, true);
        }
      }

      return null;
    }

    /// <summary>
    /// Remove user from preferences!.
    /// </summary>
    public static void DeleteUser()
    {
      PlayerPrefs.DeleteKey(USERNAME_KEY);
      PlayerPrefs.DeleteKey(PASSWWORD_KEY);
      PlayerPrefs.DeleteKey(REMEMBER_KEY);
    }
  }
}