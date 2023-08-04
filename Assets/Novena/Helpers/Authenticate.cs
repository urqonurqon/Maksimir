public static class Authenticate {
	private const string UserName = "service@novena.hr";
	private const string Password = "fdf(zX^Ud-S*,7Sy";

	/// <summary>
	/// Generate authentication string for request header.
	/// </summary>
	/// <returns></returns>
	public static string GetAuthentication()
	{
		string auth = UserName + ":" + Password;
		auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
		auth = "Basic " + auth;
		return auth;
	}
}
