using Doozy.Engine.UI;
using Novena.DAL.Entity;
using TMPro;
using UnityEngine;

namespace Novena.Admin.Controller.GuideList
{
  public class GuideListItem : MonoBehaviour
  {
    public bool Selected { get; private set; }
    public bool IsActive { get; private set; }
    public GuideData Data { get; private set; }
    
    [SerializeField] private TMP_Text _titleTmp;
    [SerializeField] private TMP_Text _totalFileCountTmp;
    [SerializeField] private TMP_Text _totalFileSizeTmp;
    [SerializeField] private TMP_Text _downloadFileCountTmp;
    [SerializeField] private TMP_Text _downloadFileSizeTmp;

    [SerializeField] private UIToggle _activeUIToggle;
    [SerializeField] private UIToggle _selectedUIToggle;
    

    [SerializeField] private TMP_Text _messageTmp;
    
    
    public void Setup(GuideData guideData)
    {
      Data = guideData;
      
      _titleTmp.text = guideData.Guide.Name;

      SetActiveSlideToggle();
      /*_totalFileCountTmp.text = guideData.Files.AllFiles.Count.ToString();
      _totalFileSizeTmp.text = (guideData.Files.TotalSize / 1024) / 1024 + " MB";

      _downloadFileCountTmp.text = guideData.Files.FilesToDownload.Count.ToString();
      _downloadFileSizeTmp.text = (guideData.Files.DownloadSize / 1024) / 1024 + " MB";*/
      SetMessage();
    }

    private void SetActiveSlideToggle()
    {
      _activeUIToggle.IsOn = Data.IsActive == 1;
      
      if (Data.IsNewGuide)
      {
        //Lets disable toggle if guide is new and not yet installed.
        //All so set it active by default.
        _activeUIToggle.IsOn = true;
        _activeUIToggle.gameObject.SetActive(false);
      }
      
      _activeUIToggle.Toggle.onValueChanged.AddListener(OnIsActiveToggleChange);
    }

    private void SetMessage()
    {
      if (Data.IsNewGuide)
      {
        _messageTmp.text = "Guide not installed!";
        return;
      }
      _messageTmp.text = Data.IsDownloadRequired ? "New content available!" : "Content up to date!";
    }

    public void OnButtonClick()
    {
      Selected = !Selected;
      _selectedUIToggle.IsOn = Selected;
    }

    public void OnIsSelectedToggleChange(bool state)
    {
      Selected = state;
    }

    public void OnIsActiveToggleChange(bool state)
    {
      IsActive = state;
      Data.IsActive = IsActive ? 1:0;

      using (GuidesEntity guidesEntity = new GuidesEntity())
      {
        guidesEntity.SetActive(IsActive, Data.Guide.Id);
      }
    }
  }
}