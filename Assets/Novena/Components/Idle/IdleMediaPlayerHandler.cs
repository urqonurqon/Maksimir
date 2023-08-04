using System.Collections.Generic;
using System.Linq;
using RenderHeads.Media.AVProVideo;
using UnityEngine;

namespace Novena.Components.Idle {
	/*
   * This component checks is AvPro MediaPlayer playing.
   * If it is than reset's idle timer in another words
   * doesn't go to idle if any player is playing.
   *
   * ######
   * If MediaPlayer object is disabled at awake it will not be checked by this component!!!
   * ######
   */
	[RequireComponent(typeof(IdleController))]
	public class IdleMediaPlayerHandler : MonoBehaviour {
		/// <summary>
		/// All media players in scene,
		/// </summary>
		private List<MediaPlayer> _mediaPlayers = new List<MediaPlayer>();

		/// <summary>
		/// Active players that are in play mode.
		/// </summary>
		private List<MediaPlayer> _activePlayers = new List<MediaPlayer>();

		private void Awake()
		{
			GetMediaPlayers();
			SubscribeToEvents();
			VideoDetailsViewController.OnMediaPlayerInstantiated += GetMediaPlayers;
			VideoDetailsViewController.OnMediaPlayerInstantiated += SubscribeToEvents;
		}

		/// <summary>
		/// Get all media players in scene
		/// </summary>
		private void GetMediaPlayers()
		{
			_mediaPlayers = FindObjectsOfType<MediaPlayer>().ToList();
		}

		/// <summary>
		/// Subscribe and listen media player events
		/// </summary>
		private void SubscribeToEvents()
		{
			for (int i = 0; i < _mediaPlayers.Count; i++)
			{
				var mediaPlayer = _mediaPlayers[i];

				mediaPlayer.Events.AddListener((player, eventType, code) => {
					switch (eventType)
					{
						case MediaPlayerEvent.EventType.Started:
							_activePlayers.Add(player);
							break;
						case MediaPlayerEvent.EventType.FinishedPlaying:
							_activePlayers.Remove(player);
							break;
						case MediaPlayerEvent.EventType.Closing:
							_activePlayers.Remove(player);
							break;
					}
				});
			}
		}

		/// <summary>
		/// Iterate through each active player and if it is playing reset idle timer.
		/// </summary>
		private void CheckIsPlaying()
		{
			for (int i = 0; i < _activePlayers.Count; i++)
			{
				var player = _activePlayers[i];

				if (player.Control.IsPlaying())
				{
					IdleController.Instance.ResetIdleTimer();
				}
			}
		}

		private void Update()
		{
			//If we have active players lets check are they are playing (not paused).
			if (_activePlayers.Any())
			{
				CheckIsPlaying();
			}
		}
	}
}