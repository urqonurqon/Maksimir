using UnityEngine;

namespace Novena.Plugin.NovenaUtilityPlugin
{
  public class DeviceAdmin
  {
    private AndroidJavaObject DeviceAdminJavaObject { get; }
    public DeviceAdmin(AndroidJavaObject androidJavaObject)
    {
      DeviceAdminJavaObject = androidJavaObject;
    }
    
    /// <summary>
    /// Enable kiosk mode and put device in lock state.
    /// </summary>
    /// <remarks>
    /// This will hide android navigation buttons and info from status bar. Disables android home button.
    /// </remarks>
    public void EnableKioskMode()
    {
#if !UNITY_EDITOR
            DeviceAdminJavaObject.Call("StartLockTask");
#endif
    }

    /// <summary>
    /// Disable kiosk mode and put device out of lock state.
    /// </summary>
    public void DisableKioskMode()
    {
#if !UNITY_EDITOR
        DeviceAdminJavaObject.Call("StopLockTask");
#endif
        }
    }
}