using System;
using Novena.Settings;
using RenderHeads.Media.AVProVideo;
using UnityEngine;

namespace Novena.Components.Idle {
	public class IdleController : MonoBehaviour {
		/// <summary>
		/// When timer ends and idle is enabled.
		/// </summary>
		public static Action OnIdleEnabled;

		/// <summary>
		/// Singletone instance.
		/// </summary>
		public static IdleController Instance { get; set; }

		[Tooltip("How long until idle invokes")]

		[SerializeField]
		private float _resetTime;

		[Tooltip("Current timer time (for editor only)")]
		[SerializeField] private float _currentTime;

		[Tooltip("Name of node in noody that is designated for idle.")]
		[SerializeField] private string _idleNodeName;

		private MediaPlayer _avPlayer;
		private VlcPlayer _vlcPlayer;


		#region Timer

		private float _timeRemaining;
		private bool _timerIsRunning = true;

		#endregion

		private void Awake()
		{
			Instance = this;
			VideoDetailsViewController.OnMediaPlayerInstantiated += ReferenceVideoPlayers;
			_timeRemaining = _resetTime;
		}

		private void ReferenceVideoPlayers()
		{
			_vlcPlayer = FindObjectOfType<VlcPlayer>();
			_avPlayer = FindObjectOfType<MediaPlayer>();
		}

		//private void OnSettingsUpdate()
		//{
		//  _timeRemaining = _resetTime;
		//}

		/// <summary>
		/// Reset idle timer to defined reset time.
		/// </summary>
		public void ResetIdleTimer()
		{
			ResetTimer();
		}

		/// <summary>
		/// Reset's time.
		/// </summary>
		private void ResetTimer()
		{
			_timeRemaining = _resetTime;
			_timerIsRunning = true;
		}

		private void EnableIdle()
		{
			_avPlayer.Stop();
			_vlcPlayer.UnloadPlayer();
			OnIdleEnabled?.Invoke();
			IdleHelper.GoToIdleNode(_idleNodeName);
		}

		/// <summary>
		/// Detects input and resets timer.
		/// </summary>
		private void CheckInput()
		{
			if (Input.anyKey || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.touches.Length > 0)
			{
				ResetTimer();
			}
		}

		private void Update()
		{
			if (Time.timeSinceLevelLoad < 2.4f) return;
			if (_avPlayer.Control.IsPlaying()) return;
			if (_vlcPlayer.IsPlaying) return;

			CheckInput();
			if (_timerIsRunning)
			{
				if (_timeRemaining > 0)
				{
					_timeRemaining -= Time.deltaTime;
				}
				else
				{
					_timerIsRunning = false;
					_timeRemaining = _resetTime;
					EnableIdle();
				}
			}

			_currentTime = _timeRemaining;
		}
	}
}