using RenderHeads.Media.AVProVideo;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Novena.Components.AvProVideoPlayer.Controls
{
  public class TimelineSliderControl : MonoBehaviour
  {
    [SerializeField] private TMP_Text _timeTextTMP;
    
    private VideoPlayerControls _videoPlayerControls;
    private Slider _sliderTime = null;
    private MediaPlayer _mediaPlayer = null;

    private bool _wasPlayingBeforeTimelineDrag;
    //private bool _isHoveringOverTimeline;
    private TimeRange _timeRange;

    private void Awake()
    {
      _sliderTime = GetComponent<Slider>();
      
      _videoPlayerControls = GetComponentInParent<VideoPlayerControls>();
      _videoPlayerControls.OnVideoReadyToPlay += OnVideoReadyToPlay;
      
      _mediaPlayer = _videoPlayerControls.MediaPlayer;

      CreateTimelineDragEvents();
    }

    private void OnVideoReadyToPlay()
    {
      _timeRange = GetTimelineRange();
    }

    private void CreateTimelineDragEvents()
    {
      EventTrigger trigger = GetComponent<EventTrigger>();
      if (trigger != null)
      {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((data) => { OnTimeSliderBeginDrag(); });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Drag;
        entry.callback.AddListener((data) => { OnTimeSliderDrag(); });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;
        entry.callback.AddListener((data) => { OnTimeSliderEndDrag(); });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { OnTimelineBeginHover((PointerEventData)data); });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerExit;
        entry.callback.AddListener((data) => { OnTimelineEndHover((PointerEventData)data); });
        trigger.triggers.Add(entry);
      }
    }
    
    private void OnTimeSliderBeginDrag()
    {
      if (_mediaPlayer && _mediaPlayer.Control != null)
      {
        _wasPlayingBeforeTimelineDrag = _mediaPlayer.Control.IsPlaying();
        if (_wasPlayingBeforeTimelineDrag)
        {
          _mediaPlayer.Pause();
        }

        OnTimeSliderDrag();
      }
    }
    
    private void OnTimeSliderDrag()
    {
      if (_mediaPlayer && _mediaPlayer.Control != null)
      {
        TimeRange timelineRange = GetTimelineRange();
        double time = timelineRange.startTime + (_sliderTime.value * timelineRange.duration);
        _mediaPlayer.Control.Seek(time);
        //_isHoveringOverTimeline = true;
      }
    }
    
    private void OnTimeSliderEndDrag()
    {
      if (_mediaPlayer && _mediaPlayer.Control != null)
      {
        if (_wasPlayingBeforeTimelineDrag)
        {
          _mediaPlayer.Play();
          _wasPlayingBeforeTimelineDrag = false;
        }
      }
    }
    
    private void OnTimelineBeginHover(PointerEventData eventData)
    {
      if (eventData.pointerCurrentRaycast.gameObject != null)
      {
        //_isHoveringOverTimeline = true;
        //Set scale of slider if necessary
        _sliderTime.transform.localScale = new Vector3(1f, 1f, 1f);
      }
    }
    
    private void OnTimelineEndHover(PointerEventData eventData)
    {
      //_isHoveringOverTimeline = false;
      //Set scale of slider if necessary
      _sliderTime.transform.localScale = new Vector3(1f, 1f, 1f);
    }
    
    private TimeRange GetTimelineRange()
    {
      if (_mediaPlayer.Info != null)
      {
        return Helper.GetTimelineRange(_mediaPlayer.Info.GetDuration(), _mediaPlayer.Control.GetSeekableTimes());
      }

      return new TimeRange();
    }

    private void UpdateTimeText()
    {
      if (_timeTextTMP)
      {
        string t1 = Helper.GetTimeString((_mediaPlayer.Control.GetCurrentTime() - _timeRange.startTime), false);
        string d1 = Helper.GetTimeString(_timeRange.duration, false);
        _timeTextTMP.text = string.Format("{0} / {1}", t1, d1);
      }
    }

    private void Update()
    {
      // Update time slider position
      if (_sliderTime && _videoPlayerControls.MediaPlayer.Info.HasVideo())
      {
        double t = 0.0;

        //_timeRange.duration Mono => Development BUILD on ANDROID doesnt work!!
        if (_timeRange.duration > 0.0)
        {
          t = (_mediaPlayer.Control.GetCurrentTime() - _timeRange.startTime) / _timeRange.duration;
        }

        _sliderTime.value = Mathf.Clamp01((float)t);

        UpdateTimeText();
      }
    }
  }
}