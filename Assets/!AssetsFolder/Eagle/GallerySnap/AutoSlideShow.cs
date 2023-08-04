using System;
using UnityEngine;

public class AutoSlideShow : MonoBehaviour
{
    public static Action OnCycleEndEvent;
    private float _cycleTime;
    private float _currentTime;

    public float _timeRemaining;
    private GallerySnap _gallery;

    private void Awake()
    {
        _gallery = GetComponent<GallerySnap>();
        _cycleTime = _gallery._cycleSpeed;
    }

    private void Update()
    {
        if (_timeRemaining > 0)
        {
            _timeRemaining -= Time.deltaTime;
        }
        else
        {
            _timeRemaining = _cycleTime;
            OnCycleEndEvent?.Invoke();

        }

        _currentTime = _timeRemaining;
    }
}
