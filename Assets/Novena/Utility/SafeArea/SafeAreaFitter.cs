using UnityEngine;

namespace Novena.Utility.SafeArea
{
  public class SafeAreaFitter : MonoBehaviour
  {
    #region Private

    private RectTransform _rectTransform;
    private Rect _safeAreaRect;

    #endregion
    
    private void Awake()
    {
      _rectTransform = GetComponent<RectTransform>();
      _safeAreaRect = Screen.safeArea;
      
      CalculateAndSetSafeArea();
    }

    private void CalculateAndSetSafeArea()
    {
      var anchorMin = _safeAreaRect.position;
      var anchorMax = anchorMin + _safeAreaRect.size;

      anchorMin.x /= Screen.width;
      anchorMin.y /= Screen.height;
      anchorMax.x /= Screen.width;
      anchorMax.y /= Screen.height;

      _rectTransform.anchorMin = anchorMin;
      _rectTransform.anchorMax = anchorMax;
    }
  }
}