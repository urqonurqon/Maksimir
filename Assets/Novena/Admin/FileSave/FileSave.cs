namespace Novena.Admin.FileSave
{
  public class FileSave
  {
    /// <summary>
    /// Save file permanently in device.
    /// </summary>
    /// <param name="rawBytes"></param>
    /// <param name="filePath"></param>
    public static void Save(byte[] rawBytes, string filePath)
    {
      FileSaveHelper.FileExist(filePath);
    }
  }
}
