using Doozy.Engine;
using Kiosk;
using Novena.Admin.Utility;
using Novena.UiUtility.Base;
using TMPro;
using UnityEngine;

namespace Novena.Admin.Controller
{
    public class LoginController : UiController
  {
    [SerializeField] private  InputFieldCustom _inputField;
    [SerializeField] private TMP_Text _validatorTmp;
    
    public override void OnShowViewStart()
    {
      _validatorTmp.text = "";
      _inputField.text = "";
		}
    
    public void OnLoginBtnClick()
    {
      if (Validate())
      {
        if (IsPasswordCorrect())
        {
          GameEventMessage.SendEvent("GoToUserLogin");
          return;
        }
        _validatorTmp.text = "Wrong password";
        return;
      }
      _validatorTmp.text = "Please enter password!";
    }

    /// <summary>
    /// Check password.
    /// </summary>
    /// <returns>True if password is correct</returns>
    private bool IsPasswordCorrect()
    {
      return _inputField.text.ToLower() == "novena";
    }

    /// <summary>
    /// Validate input field.
    /// </summary>
    /// <returns>True if field is not white space or null</returns>
    private bool Validate()
    {
      return !string.IsNullOrWhiteSpace(_inputField.text);
    }

    public void OnBackButton_Click()
    {
      AdminUtility.LoadGuideScene();
    }
  }
}