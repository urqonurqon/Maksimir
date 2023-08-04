using System;

namespace Novena.Admin.FileSave.Attribute
{
  [AttributeUsage(AttributeTargets.All)]
  public class File : System.Attribute
  {
    private FileAttributeType _type;

    public File(FileAttributeType type)
    {
      _type = type;
    }

    public FileAttributeType Type => _type;
  }
  
  public enum FileAttributeType
  {
    Path,
    Size,
    Timestamp
  }
}