using UnityEngine;

namespace Novena.Networking
{
  public static class UrlPlatformHelper 
  {
    /// <summary>
    /// Get platform specific file path.
    /// </summary>
    /// <param name="path">Path to file on disk</param>
    /// <returns>
    /// Mobile = file://path; Editor = file:///path. Null if path is empty
    /// </returns>
    public static string GetPlatformFilePath(string path)
    {
      string output = string.Empty;

      if (string.IsNullOrWhiteSpace(path) == false)
      {
        switch (Application.platform)
        {
          case RuntimePlatform.WindowsEditor:
            output = "file:///" + path;
            break;
          case RuntimePlatform.OSXEditor:
            output = "file:///" + path;
            break;
          case RuntimePlatform.Android:
            output = "file://" + path;
            break;
          case RuntimePlatform.IPhonePlayer:
            output = "file://" + path;
            break;
        }
      }

      return output;
    }
  }
}
