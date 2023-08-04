using System;
using System.Linq;

namespace Novena.DAL.Model.Guide
{
  [Serializable]
  public class Tag
  {
    public int Id;
    public string Title;
    public int TagCategoryId;

    /// <summary>
    /// Tag category of tag.
    /// </summary>
    public TagCategorie TagCategorie => GetTagCategorie();

    /// <summary>
    /// Search in TagCategorie of current selected translated content.
    /// </summary>
    /// <returns>TagCategorie</returns>
    private TagCategorie GetTagCategorie()
    {
      return Data.TranslatedContent
        .TagCategories
        .FirstOrDefault(tc => tc.Id == Convert.ToInt32(TagCategoryId));
    }
  }
  
  [Serializable]
  public class TagCategorie
  {
    public int Id;
    public string Title;
    public Tag[] Tags;
  }
}