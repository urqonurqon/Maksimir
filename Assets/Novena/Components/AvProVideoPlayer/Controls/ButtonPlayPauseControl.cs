using System;
using UnityEngine;
using UnityEngine.UI;

namespace Novena.Components.AvProVideoPlayer.Controls {
	public class ButtonPlayPauseControl : MonoBehaviour {
		public Action OnClick;

		[Header("Components")]
		[SerializeField] private Image _playIcon = null;
		[SerializeField] private Image _pauseIcon = null;

		private VideoPlayerControls _videoPlayerControls = null;
		private Button _button = null;

		private void Awake()
		{
			_button = GetComponent<Button>();
			_button.onClick.AddListener(OnButtonClick);

			_videoPlayerControls = GetComponentInParent<VideoPlayerControls>();

			_videoPlayerControls.OnVideoStarted += OnVideoStarted;
			_videoPlayerControls.OnVideoPaused += OnVideoPaused;
			_videoPlayerControls.OnVideoUnPaused += OnVideoUnPaused;

			TogglePlayPauseButtonIcon(false);
		}

		private void OnVideoPaused()
		{
			TogglePlayPauseButtonIcon(false);
		}

		private void OnVideoUnPaused()
		{
			TogglePlayPauseButtonIcon(true);
		}

		private void OnVideoStarted()
		{
			TogglePlayPauseButtonIcon(true);
		}

		private void OnButtonClick()
		{
			OnClick?.Invoke();
		}

		private void TogglePlayPauseButtonIcon(bool isPlay)
		{
			_playIcon.gameObject.SetActive(!isPlay);
			_pauseIcon.gameObject.SetActive(isPlay);
		}
	}
}