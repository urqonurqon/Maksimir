using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Novena.Admin.FileSave;
using Novena.DAL.Entity;
using Novena.DAL.Model.Guide;

namespace Novena.Admin.Controller.GuideList
{
  public class GuideData
  {
    public Guide Guide { get; set; }
    public DAL.Model.Template Template { get; set; }
    public string GuideJson { get; set; }
    public Files Files { get; set; }
    public int IsActive { get; set; }
    public bool IsDownloadRequired { get; set; }

    public bool IsNewGuide { get; set; }
    
    
    /// <summary>
    /// Collect's data about requested guide.
    /// Checks for files and json of Guide and Template.
    /// </summary>
    /// <param name="downloadCode"></param>
    /// <returns></returns>
    public static async UniTask<GuideData> GetData(string downloadCode)
    {
      GuideData guideData = new GuideData();

      var newGuideJson = await Repository.Repository.GetGuideJson(downloadCode);
      var templateJson = await Repository.Repository.GetTemplateJson(downloadCode);

      var guide = JsonConvert.DeserializeObject<Guide>(newGuideJson.Content);
      var template = JsonConvert.DeserializeObject<DAL.Model.Template>(templateJson.Content);
        
      Files files = new Files(guide, template);
      var f = await files.GetFilesAsync();
      
      bool active = true;
      bool downloadRequired = false;
      //Is guide new on device
      bool isNew = true;

      //Lets get guide from DB.
      using (GuidesEntity guidesEntity = new GuidesEntity())
      {
        //Lets get guide from DB
        var localGuide = guidesEntity.GetGuideById(guide.Id);
        
        if (localGuide != null)
        {
          isNew = false;
          
          //We have guide on device
          //Let's compare it's json with existing json
          downloadRequired = !String.Equals(newGuideJson.Content, localGuide.Json);
          active = localGuide.Active;
          
          //Template that all ready exist on device
          var localTemplate = guidesEntity.GetTemplateById(localGuide.TemplateId);

          //This stupid Serialization is just to compare two json strings
          //One that Pero sends doesn't put some properties and string is all ways different.
          string currentTemplateJson = JsonConvert.SerializeObject(template);

          //If downloadRequired flag is true dont override it.
          if (localTemplate != null && downloadRequired == false)
          {
            downloadRequired = !String.Equals(currentTemplateJson, localTemplate.Json);
          }
        }
      }

      if (files.FilesToDownload.Any())
      {
        downloadRequired = true;
      }
      
      guideData.Files = files;
      guideData.Guide = guide;
      guideData.GuideJson = newGuideJson.Content;
      guideData.Template = template;
      guideData.IsActive = active ? 1:0;
      guideData.IsDownloadRequired = downloadRequired;
      guideData.IsNewGuide = isNew;

      return guideData;
    }
  }
}