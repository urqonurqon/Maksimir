using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Doozy.Engine;
using Novena.Admin.Repository.Model;
using Novena.Components.Preloader;
using Novena.Components.SnackBar;
using Novena.Helpers;
using Novena.UiUtility.Base;
using UnityEngine;

namespace Novena.Admin.Controller.GuideList
{
  public class GuideListController : UiController
  {
    /// <summary>
    /// Singleton instance.
    /// </summary>
    public static GuideListController Instance { get; private set; }
    public static AvailableGuides AvailableGuides { get; set; }

    [SerializeField] private Transform _itemContainer;
    [SerializeField] private GuideListItem _itemPrefab;

    #region Private fields

    private List<GuideListItem> _listItems = new List<GuideListItem>();
    private List<GuideData> _guideDatas = new List<GuideData>();

    #endregion

    public override void Awake()
    {
      Instance = this;
      base.Awake();
    }

    public override void OnShowViewFinished()
    {
      Preloader.Instance.Show();
      GetGuidesData().Forget();
    }

    public override void OnHideViewFinished()
    {
      _guideDatas.Clear();
      UnityHelper.DestroyObjects(_listItems);
    }

    private async UniTaskVoid GetGuidesData()
    {
      foreach (var g in AvailableGuides.Guides)
      {
        var uGuideData = await GuideData.GetData(g.DownloadCode);
        _guideDatas.Add(uGuideData);
      }
      
      GenerateGuideItems();
      
      Preloader.Instance.Hide();
    }

    private void GenerateGuideItems()
    {
      foreach (var gd in _guideDatas)
      {
        var item = Instantiate(_itemPrefab, _itemContainer);
        item.Setup(gd);
        
        _listItems.Add(item);
      }
    }

    public void OnDownloadButton_Click()
    {
      foreach (var guideListItem in _listItems)
      {
        if (guideListItem.Selected)
        {
          DownloadController.GuideDataList.Add(guideListItem.Data);
        }
      }
      
      if (DownloadController.GuideDataList.Any() == false)
      {
        SnackBar.Instance.Show("Please select guide!");
        return;
      }
      
      GameEventMessage.SendEvent("GoToDownload");
    }
  }
}