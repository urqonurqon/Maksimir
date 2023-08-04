using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Novena.Components.SnackBar
{
  public class SnackBar : MonoBehaviour
  {
    public static SnackBar Instance { get; private set; }

    [SerializeField] private TMP_Text _messageText;

    private CanvasGroup _canvasGroup;
    
    private bool _show;
    private void Awake()
    {
      Instance = this;
      _messageText.text = "";
      _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
      Hide();
    }

    public void Show(string message)
    {
      _messageText.text = message;
      
      Show(true);
      
      DOVirtual.DelayedCall(3f, Hide);
    }

    private void Show(bool show)
    {
      _canvasGroup.alpha = show ? 1 : 0;
      _canvasGroup.interactable = _show;
      _canvasGroup.blocksRaycasts = _show;
    }

    public void Hide()
    {
      Show(false);
    }
  }
}