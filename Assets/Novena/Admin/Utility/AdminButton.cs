using Novena.Admin.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdminButton : MonoBehaviour
{
  public void OnLong_ButtonClick()
  {
    AdminUtility.LoadAdminScene();
  }
}
