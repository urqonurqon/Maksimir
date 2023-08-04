using UnityEngine.Networking;

namespace Novena.Admin.Networking
{
  /// <summary>
  /// Network request result.
  /// </summary>
  public class Result
  {
    public string Content { get; set; }
    public NetworkError NetworkError { get; set; }

    public UnityWebRequest WebRequest { get; set; }
  }
}