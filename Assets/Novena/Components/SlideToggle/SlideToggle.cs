using Doozy.Engine.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Novena.Components.SlideToggle
{
  [RequireComponent(typeof(UIToggle))]
  public class SlideToggle : MonoBehaviour
  {
    [Header("Components")]
    [SerializeField] private Image _background;
    [SerializeField] private GameObject _handle;

    [Header("Colors")]
    [SerializeField] private Color _enabledColor;
    [SerializeField] private Color _disabledColor;
    [SerializeField] private Color _inactiveColor;
    

    #region Private

    private UIToggle _uiToggle;

    #endregion
    
    
    private void Awake()
    {
      _uiToggle = GetComponent<UIToggle>();
    }

    private void OnEnable()
    {
      _uiToggle.OnValueChanged.AddListener((state) => OnStateChanged(state));
    }

    private void OnDisable()
    {
      _uiToggle.OnValueChanged.RemoveAllListeners();
    }

    private void OnStateChanged(bool state)
    {
      SetToggle(state);
    }

    private void SetToggle(bool isOn)
    {
      var rt = _handle.GetComponent<RectTransform>();
      
      if (isOn)
      {
        _background.color = _enabledColor;
        rt.anchorMin = new Vector2(1,0.5f);
        rt.anchorMax = new Vector2(1,0.5f);
        rt.pivot = new Vector2(1, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        return;
      }
      
      _background.color = _disabledColor;
      rt.anchorMin = new Vector2(0,0.5f);
      rt.anchorMax = new Vector2(0,0.5f);
      rt.pivot = new Vector2(0, 0.5f);
      rt.anchoredPosition = Vector2.zero;
    }
  }
}