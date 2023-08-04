using DG.Tweening;
using Novena.UiUtility.Base;
using UnityEngine;

namespace Novena.Components.Preloader
{
  public class Preloader : UiController
  {
    /// <summary>
    /// Singleton
    /// </summary>
    public static Preloader Instance { get; private set; }
    
    [SerializeField] private RectTransform _circleProgress;

    #region PrivateFields

    private Sequence _circleSequence;

    #endregion

    public override void Awake()
    {
      Instance = this;
      SetCircleAnimation();
      base.Awake();
    }

    private void SetCircleAnimation()
    {
      _circleSequence = DOTween.Sequence();
      _circleSequence.SetAutoKill(false);
      _circleSequence.Append(DOVirtual.Float(0, -360f, 1f, value =>
      {
        _circleProgress.rotation = Quaternion.Euler(0, 0, value);
      })).SetLoops(-1);
      _circleSequence.Pause();
    }

    public void Show()
    {
      UiView.Show();
      _circleSequence.PlayForward();
    }

    public void Hide()
    {
      UiView.Hide();
      _circleSequence.Pause();
    }
  }
}