using Novena.Admin.Utility;
using UnityEngine;

namespace Novena.Components.AdminButton
{
  public class AdminButton : MonoBehaviour
  {
    public void OnButton_LongClick()
    {
      AdminUtility.LoadAdminScene();
    }
  }
}