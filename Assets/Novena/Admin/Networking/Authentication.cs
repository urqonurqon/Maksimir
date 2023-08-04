namespace Novena.Admin.Networking
{
  public static class Authentication
  {
    /// <summary>
    /// Creates basic authentication string.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static string GetBasicAuth(string username, string password)
    {
      var auth = username + ":" + password;
      auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
      auth = "Basic " + auth;
      return auth;
    }
  }
}