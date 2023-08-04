using System;
using Novena.Admin.FileSave.Attribute;

namespace Novena.DAL.Model
{
  [Serializable]
  public class Template
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public TemplateItemValues[] TemplateItemValues { get; set; }
  }

  [Serializable]
  public class TemplateItemValues
  {
    public int ItemId { get; set; }
    public int TypeId { get; set; }
    [Admin.FileSave.Attribute.File(FileAttributeType.Path)]
    public string ItemValue { get; set; }
    [Admin.FileSave.Attribute.File(FileAttributeType.Timestamp)]
    public string ImageTimestamp { get; set; }
    [Admin.FileSave.Attribute.File(FileAttributeType.Size)]
    public ulong ImageSize { get; set; }
  }
}