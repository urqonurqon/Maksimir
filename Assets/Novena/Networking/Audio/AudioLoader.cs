#nullable enable
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Novena.Networking.Audio
{
  public static class AudioLoader
  {
    /// <summary>
    /// Download audio file and create AudioClip from URI.
    /// Download file completely before returning audio clip.
    /// </summary>
    /// <param name="path">Path to file</param>
    /// <param name="updateProgressAction">Function to be called on every progress update</param>
    /// <param name="cancellationToken">To cancel async download operation</param>
    /// <returns>AudioClip. Null if unable or error in download.</returns>
    public static async UniTask<AudioClip?> GetAudioClip(string path, Action<float> updateProgressAction, CancellationToken cancellationToken)
    {
      //This is part of UniTask to get download progress
      var progress = Progress.Create<float>(x =>
      {
        updateProgressAction(x);
      });
      
      //Start of async request
      var response = await UnityWebRequestMultimedia
        .GetAudioClip(path, AudioLoaderHelper.GetAudioType(path))
        .SendWebRequest().ToUniTask(progress: progress, PlayerLoopTiming.Update, cancellationToken: cancellationToken);

      var downloadHandler = (DownloadHandlerAudioClip)response.downloadHandler;

      //After async callback complete check response status
      if (response.result != UnityWebRequest.Result.Success)
      {
        Log.Log.LogError(response.error);
      }
      else
      {
        if (!response.isDone) return null;
        
        if (response.result == UnityWebRequest.Result.Success)
        {
          return downloadHandler.audioClip;
        }
      }
      return null;
    }

    

    #region Not working

    [Obsolete("Stream does not work in Unity :(")]
    public static async UniTask<bool> Stream(string path, AudioSource source)
    {
      var request = UnityWebRequestMultimedia.GetAudioClip(path, AudioLoaderHelper.GetAudioType(path));
      
      //This does not work
      ((DownloadHandlerAudioClip)request.downloadHandler).streamAudio = true;

      await request.SendWebRequest();
      
      while (request.isDone == false)
      {
        await UniTask.Yield();
        
        Debug.Log("Progres: " + request.downloadProgress + " Is Done: " + request.isDone);
      
        if (request.result == UnityWebRequest.Result.InProgress && request.downloadProgress > 0.5)
        {
          var clip = ((DownloadHandlerAudioClip)request.downloadHandler).audioClip;
          source.clip = clip;
          source.Play();
        }
      }
      return true;
    }

    #endregion
  }
}
