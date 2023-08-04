using Cysharp.Threading.Tasks;
using Novena.Admin.Networking;
using Novena.Networking;
using UnityEngine.Networking;
using NetworkError = Novena.Admin.Networking.NetworkError;

namespace Novena.Admin.Repository
{
  public static class Repository
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public static async UniTask<Result> GetAvailableGuidesJson(User user)
    {
      Result result = new Result();
      
      var request = UnityWebRequest.Get(Api.GET_AVAILABLE_GUIDES);
      
      request.SetRequestHeader("AUTHORIZATION", user.BasicAuth);
      
      try
      {
        var response = await request.SendWebRequest();

        if (response.result == UnityWebRequest.Result.Success)
        {
          result.Content = response.downloadHandler.text;
          return result;
        }
      }
      catch (UnityWebRequestException e)
      {
        result.NetworkError = new NetworkError(e);
        return result;
      }
      
      return null;
    }

    /// <summary>
    /// Send request to Download guide json with download code API.
    /// </summary>
    /// <param name="downloadCode"></param>
    /// <returns>Guide json string</returns>
    public static async UniTask<Result> GetGuideJson(string downloadCode)
    {
      Result result = new Result();
      
      var request = UnityWebRequest.Get(Api.DOWNLOAD_GUIDE_JSON + downloadCode);
      
      try
      {
        var response = await request.SendWebRequest();

        if (response.result == UnityWebRequest.Result.Success)
        {
          result.Content = response.downloadHandler.text;
          return result;
        }
      }
      catch (UnityWebRequestException e)
      {
        result.NetworkError = new NetworkError(e);
        return result;
      }
      
      return null;
    }

    /// <summary>
    /// Send request to Download template json with download code API.
    /// </summary>
    /// <param name="downloadCode"></param>
    /// <returns></returns>
    public static async UniTask<Result> GetTemplateJson(string downloadCode)
    {
      Result result = new Result();
      
      var request = UnityWebRequest.Get(Api.DOWNLOAD_TEMPLATE_JSON + downloadCode);
      
      try
      {
        var response = await request.SendWebRequest();

        if (response.result == UnityWebRequest.Result.Success)
        {
          result.WebRequest = response;
          result.Content = response.downloadHandler.text;
          return result;
        }
      }
      catch (UnityWebRequestException e)
      {
        result.NetworkError = new NetworkError(e);
        return result;
      }
      
      return null;
    }
  }
}