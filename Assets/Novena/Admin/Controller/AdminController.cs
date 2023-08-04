using Doozy.Engine.UI;
using Novena.DAL.Entity;
using Novena.UiUtility.Base;
using UnityEngine;

namespace Novena.Admin
{
    public class AdminController : UiController
  {
    [Header("Components")]
    [SerializeField] private UIView _loginUiView;
    [SerializeField] private UIView _preloadUiView;
    
    
    public override void OnShowViewStart()
    {
      //First lets show login view!
      _loginUiView.Show();
      _preloadUiView.Hide();
    }

    private void OnDownloadBtnClick()
    {
      //GetGuide().Forget();
    }

    /*/// <summary>
    /// Start the process of guide and guide files download.
    /// </summary>
    private async UniTaskVoid GetGuide()
    {
      //First lets display preload view
      _preloadUiView.Show();
      
      //Delete current Guide JSON!
      DeleteGuideFromDb();
      
      try
      {
        //Then lets get new json
        string json = await Downloader.GetGuideJson();

        //TODO lets take care if there is problem with downloading json
      
        if (string.IsNullOrEmpty(json) == false)
        {
          var guide = JsonUtility.FromJson<Guide>(json);
          
          //Set downloaded guide as current guide.
          Data.Guide = guide;

          //Save new json to DB
          using (GuidesEntity guidesEntity = new GuidesEntity())
          {
            guidesEntity.InsertGuideJson(json, guide.Id);
          }
          
          //Lets collect files to download
          await Files.GetFilesAsync();

          if (Files.FilesToDownload.Any())
          {
            GameEventMessage.SendEvent("GoToDownload");
          }
          else
          {
            //If there is no files to download go to language view
            GameEventMessage.SendEvent("GoToLanguage");
          }
        }
      }
      catch (Exception e)
      {
        Log.Log.LogError(e.Message);
      }
      
      _preloadUiView.Hide();
    }*/
    
    /// <summary>
    /// Deletes Guide JSON from Db if exist.
    /// </summary>
    private void DeleteGuideFromDb()
    {
      using (GuidesEntity guidesEntity = new GuidesEntity())
      {
        guidesEntity.DeleteAll();
      }
    }
  }
}