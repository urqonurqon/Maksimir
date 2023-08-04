using System;
using UnityEngine;

namespace Kiosk
{
  
  public class KioskController : MonoBehaviour
  {
    public static KioskController Instance;

    [Header("Settings")] 
    [SerializeField] private bool enableKioskPlugin;

    private void Awake()
    {
      DontDestroyOnLoad(this);
      if (Instance != null && Instance!=this)
      {
        Destroy(this.gameObject);
      }
      else
      {
        Instance = this;
      }
      if (!enableKioskPlugin) return;
      
      if (Application.platform == RuntimePlatform.Android)
      {
        KioskPlugin.Initialize();
			}
    }

    private void Start()
    {
      KioskPlugin.Start();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
      Debug.Log("UNITY: OnApplicationFocus = " + hasFocus);
      KioskPlugin.OnApplicationFocus(hasFocus);
    }

    public void EnableKioskMode()
		{
      KioskPlugin.Start();
		}

    public void DisableKioskMode()
    {
      KioskPlugin.Stop();

		}



  }
}