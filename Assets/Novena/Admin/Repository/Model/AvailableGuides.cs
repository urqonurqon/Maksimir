using System;
using System.Collections.Generic;

namespace Novena.Admin.Repository.Model
{
  /// <summary>
  /// Model class for available guides API.
  /// </summary>
  [Serializable]
  public class AvailableGuides
  {
    public List<Guide> Guides { get; set; }
  }
  
  [Serializable]
  public class Guide
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string GuideZipFilename { get; set; }
    public string TemplateZipFilename { get; set; }
    public string ApkUrl { get; set; }
    public int ApkAndroidVersionCode { get; set; }
    public string DownloadCode { get; set; }
  }
}