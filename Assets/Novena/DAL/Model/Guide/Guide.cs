using Novena.Admin.FileSave.Attribute;

namespace Novena.DAL.Model.Guide
{
  [System.Serializable]
  public class Guide
  {
    public int Id;
    public string Name;
    public TranslatedContent[] TranslatedContents { get; set; }
    public Map[] Maps { get; set; }
  }
  
  
  [System.Serializable]
  public struct Map
  {
    public int Id;
    public string Name;
    [Admin.FileSave.Attribute.File(FileAttributeType.Path)]
    public string ImagePath { get; set; }
    [Admin.FileSave.Attribute.File(FileAttributeType.Timestamp)]
    public string ImageTimestamp { get; set; }
    [Admin.FileSave.Attribute.File(FileAttributeType.Size)]
    public ulong ImageSize { get; set; }
  }
}