using RenderHeads.Media.AVProVideo;
using UnityEngine;

namespace Novena.Components.AvProVideoPlayer
{
  public class AvProVideoPlayer : MonoBehaviour
  {
    [SerializeField] private MediaPlayer _mediaPlayer;

    public void LoadVideo(string path)
    {
      MediaPath mediaPath = new MediaPath(path, MediaPathType.AbsolutePathOrURL);
      _mediaPlayer.OpenMedia(mediaPath, false);
    }

    public MediaPlayer GetPlayer()
    {
      return _mediaPlayer;
    }

    public void PlayVideo()
    {
      _mediaPlayer.Play();
      
    }

    public void ResetPlayer()
    {
      _mediaPlayer.CloseMedia();
    }
  }
}