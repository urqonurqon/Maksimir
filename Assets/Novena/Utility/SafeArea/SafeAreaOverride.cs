using UnityEngine;
using UnityEngine.UI;

namespace Novena.Utility.SafeArea
{
  /// <summary>
  /// Component that overrides safe area top and bottom.
  /// </summary>
  public class SafeAreaOverride : MonoBehaviour
  {
    [Tooltip("Extends Rect Transform to the top of Screen")] 
    [SerializeField] private bool _overrideTop;

    [Tooltip("Extends Rect Transform to the bottom of Screen")] 
    [SerializeField] private bool _overrideBottom;

    private float _diffTop = 0;
    private float _diffBottom = 0;
    private CanvasScaler _canvasScaler;

    private void Awake()
    {
      _canvasScaler = GetComponentInParent<CanvasScaler>();
      _diffBottom = Screen.safeArea.yMin;
      _diffTop = Screen.height - Screen.safeArea.yMax;
      SetOverride();
    }

    /// <summary>
    /// Set's offset Min and Max of rect transform.
    /// </summary>
    private void SetOverride()
    {
      var rectTransform = this.transform.GetComponent<RectTransform>();
      var referenceResolution = _canvasScaler.referenceResolution;

      if (_overrideBottom)
      {
        var bottomRatio = _diffBottom / Screen.currentResolution.width;
        var bottomUnits = bottomRatio * referenceResolution.x;
        
        var offsetMin = rectTransform.offsetMin;
        rectTransform.offsetMin = new Vector2(offsetMin.x , offsetMin.y - bottomUnits);
      }

      if (_overrideTop)
      {
        var topRatio = _diffTop / Screen.currentResolution.width;
        var topUnits = topRatio * referenceResolution.x;
        
        var offsetMax = rectTransform.offsetMax;
        rectTransform.offsetMax = new Vector2(offsetMax.x , offsetMax.y + topUnits);
      }
    }
  }
}