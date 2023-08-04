using Kiosk;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Novena.Admin.Utility
{
  public static class AdminUtility
  {
    public static void LoadGuideScene()
    {
      SceneManager.LoadScene(0);
    }

    public static void LoadAdminScene()
    {
      SceneManager.LoadScene(1);
    }
  }
}