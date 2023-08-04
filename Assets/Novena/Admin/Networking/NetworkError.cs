using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

namespace Novena.Admin.Networking
{
  /// <summary>
  /// Networking errors helper class.
  /// </summary>
  public class NetworkError
  {
    public string Code { get; }
    public string Message { get; }

    public CustomError CustomError { get; }

    public UnityWebRequestException Exception { get; set; }
    
    public NetworkError(UnityWebRequestException webRequestException)
    {
      Exception = webRequestException;
      
      try
      {
        CustomError = JsonConvert.DeserializeObject<CustomError>(webRequestException.Text);
        Code = CustomError.Errors[0].Code;
        Message = CustomError.Errors[0].Message;
      }
      catch (Exception e)
      {
        Code = "";
        Message = webRequestException.Message;
      }
    }
  }

  [Serializable]
  public class CustomError
  {
    public List<Error> Errors { get; set; }
  }

  [Serializable]
  public class Error
  {
    public string Code { get; set; }
    public string Message { get; set; }
  }
}