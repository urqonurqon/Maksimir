using System;
using Cysharp.Threading.Tasks;
using Doozy.Engine;
using Doozy.Engine.UI;
using Kiosk;
using Newtonsoft.Json;
using Novena.Admin.Controller.GuideList;
using Novena.Admin.Repository.Model;
using Novena.Components.Preloader;
using Novena.Components.SnackBar;
using Novena.Plugin.NovenaUtilityPlugin;
using Novena.UiUtility.Base;
using TMPro;
using UnityEngine;

namespace Novena.Admin.Controller
{
  public class UserLoginController : UiController
  {
    [Header("Components DownloadCode")]
    [SerializeField] private InputFieldCustom _downloadCodeInput;

    [Header("Components Login")]
    [SerializeField] private InputFieldCustom _userNameInput;
    [SerializeField] private InputFieldCustom _passwordInput;
    [SerializeField] private UIToggle _rememberToggle;
    
    public override void OnShowViewStart()
    {
			KioskController.Instance.DisableKioskMode();
      Init();

			
    }

    private void Init()
    {
      var user = User.GetUser();
      
      _downloadCodeInput.text = "";
      
      _userNameInput.text = "";
      _passwordInput.text = "";
      _rememberToggle.IsOn = false;

      if (user != null)
      {
        _userNameInput.text = user.Username;
        _passwordInput.text = user.Password;
        _rememberToggle.IsOn = user.IsRemembered;
      }
    }

    private bool ValidateLoginForm()
    {
      if (string.IsNullOrEmpty(_userNameInput.text.Trim()) && 
          string.IsNullOrEmpty(_passwordInput.text.Trim()))
      {
        SnackBar.Instance.Show("All fields required!");
        return false;
      }
      return true;
    }
    
    public async void OnDownloadBtn_Click()
    {
      Preloader.Instance.Show();

      if (string.IsNullOrEmpty(_downloadCodeInput.text.Trim()))
      {
        Preloader.Instance.Hide();
        SnackBar.Instance.Show("Please enter code!");
        return;
      }
      
      var guideData = await GuideData.GetData(_downloadCodeInput.text);

      if (guideData.Guide != null)
      {
        guideData.IsActive = 1;
        DownloadController.GuideDataList.Add(guideData);
        GameEventMessage.SendEvent("GoToDownload");
      }
      
      Preloader.Instance.Hide();
    }
    
    public void OnLoginBtn_Click()
    {
      if (ValidateLoginForm() == false) return;
      
      GetGuides().Forget();
    }

    private async UniTaskVoid GetGuides()
    {
      Preloader.Instance.Show();
      
      User user = new User(_userNameInput.text, _passwordInput.text, _rememberToggle.IsOn);
      
      var result = await Repository.Repository.GetAvailableGuidesJson(user);

      if (result.NetworkError is not null)
      {
        Preloader.Instance.Hide();
        SnackBar.Instance.Show(result.NetworkError.Message);
        Debug.LogError(result.NetworkError.Message);
        return;
      }

      try
      {
        var guides = JsonConvert.DeserializeObject<AvailableGuides>(result.Content);
        GuideListController.AvailableGuides = guides;
        GameEventMessage.SendEvent("GoToGuideList");
      }
      catch (Exception e)
      {
        Debug.LogException(e);
      }
      
      Debug.Log(result);
      Preloader.Instance.Hide();
    }
  }
}