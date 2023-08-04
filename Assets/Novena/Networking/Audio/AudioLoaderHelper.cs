using System.IO;
using UnityEngine;

namespace Novena.Networking.Audio
{
  public static class AudioLoaderHelper
  {
    /// <summary>
    /// Get audio type by file extension.
    /// </summary>
    /// <param name="filePath">Path to file</param>
    /// <returns>If not detected AudioType.UNKNOWN</returns>
    public static AudioType GetAudioType(string filePath)
    {
      AudioType output = AudioType.UNKNOWN;

      if (string.IsNullOrEmpty(filePath)) return output;
      
      var extension = Path.GetExtension(filePath);

      if (string.IsNullOrWhiteSpace(extension) == false)
      {
        switch (extension)
        {
          case ".mp3":
            output = AudioType.MPEG;
            break;
          case ".ogg":
            output = AudioType.OGGVORBIS;
            break;
          case ".wav":
            output = AudioType.WAV;
            break;
        }
      }
      
      return output;
    }
  }
}