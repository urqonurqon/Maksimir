#nullable enable
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Novena.Networking.AssetBundle
{
  public static class AssetBundleLoader
  {
    /// <summary>
    /// Load asset from file on disk.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="loadProgressAction"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async UniTask<UnityEngine.AssetBundle?> LoadFromFile(string path, Action<float> loadProgressAction,
      CancellationToken cancellationToken)
    {
      //This is part of UniTask to get download progress
      var progress = Progress.Create<float>(x =>
      {
        loadProgressAction(x);
      });

      try
      {
        return await UnityEngine.AssetBundle.LoadFromFileAsync(path)
          .ToUniTask(progress, PlayerLoopTiming.Update, cancellationToken);
      }
      catch (Exception e)
      {
        Log.Log.LogError("AssetBundle LoadFromFile: " + e.Message);
      }

      return null;
    }
  }
}