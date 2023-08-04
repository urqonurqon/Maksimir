using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;

public class LanguageViewWarningHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler {

	[SerializeField] private ScrollRect _scrollRect;
	[SerializeField] private CanvasGroup _circlePulse;


	private bool _isDragging;
	private LanguageViewController _languageViewController;

	private void Awake()
	{
		_languageViewController = FindObjectOfType<LanguageViewController>();
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		_isDragging = true;
		ShowCanvasGroup.Show(_circlePulse, false, .5f);
		ExecuteEvents.Execute(_scrollRect.gameObject, eventData, ExecuteEvents.beginDragHandler);
	}

	public void OnDrag(PointerEventData eventData)
	{
		ExecuteEvents.Execute(_scrollRect.gameObject, eventData, ExecuteEvents.dragHandler);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		ExecuteEvents.Execute(_scrollRect.gameObject, eventData, ExecuteEvents.endDragHandler);
		_isDragging = false;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (_isDragging)
			ExecuteEvents.Execute(_scrollRect.gameObject, eventData, ExecuteEvents.endDragHandler);
		else
			_languageViewController.HideWarningOverlay();

	}
}
