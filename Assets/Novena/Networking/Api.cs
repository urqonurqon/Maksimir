using System;
using Novena.DAL;
using UnityEngine;

namespace Novena.Networking
{
  /// <summary>
  /// Collection of API endpoints.
  /// </summary>
  public static class Api
  {
    /// <summary>
    /// 
    /// </summary>
    public const string GET_AVAILABLE_GUIDES = "http://n3guide.novena.agency/sys/api/user/availableGuides.aspx";
    
    public const string DOWNLOAD_GUIDE_ZIP_WITH_CODE = 
      "http://n3guide.novena.agency/sys/api/guide/download.aspx?downloadCode=";
    
    public const string DOWNLOAD_TEMPLATE_JSON_WITH_CODE =
      "http://n3guide.novena.agency/sys/api/template/json.ashx?downloadCode=";
    
    /// <summary>
    /// http://n3guide.novena.agency/sys/api/guide/json.ashx?downloadCode=
    /// </summary>
    public const string DOWNLOAD_GUIDE_JSON = 
      "http://n3guide.novena.agency/sys/api/guide/json.ashx?downloadCode=";
    
    /// <summary>
    /// http://n3guide.novena.agency/sys/api/template/json.ashx?downloadCode=
    /// </summary>
    public const string DOWNLOAD_TEMPLATE_JSON = 
      "http://n3guide.novena.agency/sys/api/template/json.ashx?downloadCode=";
    
    public const string DOWNLOAD_FILES = 
      "http://n3guide.novena.agency";

    
    /// <summary>
    /// Combines persistent data path with file path.
    /// </summary>
    /// <param name="filePath">Part of file path</param>
    /// <returns>Full path to file</returns>
    public static string GetFullPath(string filePath)
    {
      return Application.persistentDataPath + filePath;
    }

		/// <summary>
		/// Combines persistent data path with file path and adds files
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static string GetFullLocalPath(string filePath)
    {
      filePath = filePath.Replace("~", "");
			string path = GetFullPath(filePath);
      return UrlPlatformHelper.GetPlatformFilePath(path);
    }
    
    /// <summary>
    /// Combines persistent data path with guide download code.
    /// </summary>
    /// <returns>Part of path to guide directory</returns>
    public static string GetGuidePath()
    {
      return Application.persistentDataPath + "/" + DataAccess.Instance.downloadCode;
    }
  }
}