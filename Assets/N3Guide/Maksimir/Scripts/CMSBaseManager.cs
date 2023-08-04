public static class CMSBaseManager {

	private const string _cmsPath = "http://maksimir.s13.novenaweb.info/sys/api/";
	private const string _secureCmsPath = "https://maksimir.s13.novenaweb.info/sys/api/";

	public static string GetCMSPath()
	{
		return _cmsPath;
	}

	public static string GetSecureCMSPath()
	{
		return _cmsPath;
	}
}
