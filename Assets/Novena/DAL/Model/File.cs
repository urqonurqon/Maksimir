namespace Novena.DAL.Model
{
  public class File
  {
    public int Id { get; set; }
    public int GuideId { get; set; }
    public string FilePath { get; set; }
    public string LocalPath { get; set; }
    public string TimeStamp { get; set; }
  }
}