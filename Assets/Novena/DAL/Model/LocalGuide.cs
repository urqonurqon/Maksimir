namespace Novena.DAL.Model
{
  /*Model class for GuidesEntity*/
  public class LocalGuide
  {
    public int Id { get; set; }
    public int GuideId { get; set; }

    public int TemplateId { get; set; }
    public string Json { get; set; }
    public bool Active { get; set; }
  }

  public class LocalTemplate
  {
    public int Id { get; set; }

    public int TemplateId { get; set; }

    public string Json { get; set; }
  }
}