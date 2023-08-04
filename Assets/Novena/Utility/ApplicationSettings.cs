using UnityEngine;

namespace Novena.Utility
{
  public class ApplicationSettings : MonoBehaviour
  {
    [SerializeField] private bool _multitouch = false;
    [SerializeField] private bool _forcePortait = true;
    [SerializeField] private int _fpsLimit;


		[Header("Screen")]
		[SerializeField] private int _width;
		[SerializeField] private int _height;
		[SerializeField] private bool _isFullscreen;


		private void Start()
    {
      if (_forcePortait)
      {
        Screen.orientation = ScreenOrientation.Portrait;
      }

      SetFpsLimit();
      SetMultitouch();
      SetScreen();
    }

		private void SetScreen()
		{
			Screen.SetResolution(_width, _height, _isFullscreen);
		}

		private void SetMultitouch()
    {
      Input.multiTouchEnabled = _multitouch;
    }

    private void SetFpsLimit()
    {
      QualitySettings.vSyncCount = 0;
      Application.targetFrameRate = _fpsLimit;
    }
  }
}