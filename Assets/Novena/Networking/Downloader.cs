using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Novena.DAL;
using UnityEngine;
using UnityEngine.Networking;

namespace Novena.Networking
{
  public static class Downloader
  {
    private const string TAG = "DOWNLOADER";

    /// <summary>
    /// Download Guide json from designated API.
    /// </summary>
    /// <returns>json string if successful else empty string!</returns>
    public static async UniTask<string> GetGuideJson(string code)
    {
      string uri = Api.DOWNLOAD_GUIDE_JSON + code;

      try
      {
        var res = await UnityWebRequest.Get(uri).SendWebRequest();

        if (res.result == UnityWebRequest.Result.Success)
        {
          if (res.downloadHandler.text.StartsWith("ERROR"))
          {
            Log.Log.LogError(TAG + " " + res.downloadHandler.text);
            return "";
          }

          return res.downloadHandler.text;
        }

        Log.Log.LogMessage(TAG + " " + res.result);
      }
      catch (Exception e)
      {
        Log.Log.LogError(TAG + " " + e.Message);
      }

      return "";
    }
    
    /// <summary>
    /// Download file from url.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="downloadProgressAction"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async UniTask<bool> Download(string path, Action<float> downloadProgressAction, CancellationToken cancellationToken)
    {
      string url = Api.DOWNLOAD_FILES + path;
      
      Debug.Log(url);
      
      string saveFilePath = Application.persistentDataPath + path;

      var request = new UnityWebRequest(url);
      var downloadHandler = new DownloadHandlerFile(saveFilePath);

      request.downloadHandler = downloadHandler;
      
      //This is part of UniTask to get download progress
      var progress = Progress.Create<float>(x =>
      {
        downloadProgressAction(x);
      });
      
      try
      {
        var response = await request.SendWebRequest().ToUniTask(progress, PlayerLoopTiming.Update, cancellationToken);

        if (response.result == UnityWebRequest.Result.Success)
        {
          return true;
        }
      }
      catch (Exception e)
      {
        Log.Log.LogError("Network file download: " + e.Message);
      }
      
      return false;
    }
  }
}