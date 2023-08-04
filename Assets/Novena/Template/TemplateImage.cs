using UnityEngine;
using UnityEngine.UI;

namespace Novena.Template
{
  [RequireComponent(typeof(RawImage))]
  public class TemplateImage : TemplateItem
  {
    private RawImage _rawImage;
    private void Awake()
    {
      _rawImage = GetComponent<RawImage>();
    }
    
    
    
  }
}