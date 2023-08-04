using Kiosk;
using UnityEngine;
using UnityEngine.UI;

public class ToggleHelper : MonoBehaviour
{
  Toggle m_Toggle;

	void Start()
  {
    m_Toggle = GetComponent<Toggle>();

    m_Toggle.onValueChanged.AddListener(delegate { ToggleValueChanged(m_Toggle); });

    Debug.Log(m_Toggle.isOn);
  }

  void ToggleValueChanged(Toggle change)
  {
		if (m_Toggle.isOn)
		{
      Debug.Log("Enabling kiosk mode!");
      KioskController.Instance.EnableKioskMode();
    }
    else
		{
      Debug.Log("Disabling kiosk mode!");
      KioskController.Instance.DisableKioskMode();
    }

    Debug.Log(m_Toggle.isOn);
  }
}
