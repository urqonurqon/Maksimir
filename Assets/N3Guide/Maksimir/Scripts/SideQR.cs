using DG.Tweening;
using Doozy.Engine.UI;
using TMPro;
using UnityEngine;
using Utility;
using UnityEngine.UI;

public class SideQR : MonoBehaviour {

	[SerializeField] private UIButton _qROpen;
	[SerializeField] private UIButton _qRClose;

	public RectTransform QRImage;

	[SerializeField] private TMP_Text _qRText;



	private void Awake()
	{
		_qROpen.OnClick.OnTrigger.Event.RemoveAllListeners();
		_qROpen.OnClick.OnTrigger.Event.AddListener(() => {

			QRImage.DOAnchorPos(new Vector2(-27.5f, 50), .5f);
			QRImage.DOSizeDelta(new Vector2(124, 124), .5f);
			_qROpen.RectTransform.DOSizeDelta(new Vector2(325, 280), .5f);

			_qROpen.GetComponent<Image>().raycastTarget = false;
			ShowCanvasGroup.Show(_qRClose.GetComponent<CanvasGroup>(), true, .5f, .3f);
			ShowCanvasGroup.Show(_qRText.GetComponent<CanvasGroup>(), true, .5f, .3f);
		});

		_qRClose.OnClick.OnTrigger.Event.RemoveAllListeners();
		_qRClose.OnClick.OnTrigger.Event.AddListener(() => {

			QRImage.DOAnchorPos(Vector2.zero, .5f).SetDelay(.3f);
			QRImage.DOSizeDelta(new Vector2(48, 48), .5f).SetDelay(.3f).OnComplete(() => _qROpen.GetComponent<Image>().raycastTarget = true);
			_qROpen.RectTransform.DOSizeDelta(new Vector2(100, 98), .5f).SetDelay(.3f);


			ShowCanvasGroup.Show(_qRClose.GetComponent<CanvasGroup>(), false, .5f);
			ShowCanvasGroup.Show(_qRText.GetComponent<CanvasGroup>(), false, .5f);

		});
	}


	public void Reset()
	{
		QRImage.anchoredPosition = Vector2.zero;
		QRImage.sizeDelta = new Vector2(48, 48);
		_qROpen.GetComponent<Image>().raycastTarget = true;
		_qROpen.RectTransform.anchoredPosition = new Vector2(0, -573.4f);
		_qROpen.RectTransform.DOSizeDelta(new Vector2(100, 98), 0f);
		ShowCanvasGroup.Show(_qRText.GetComponent<CanvasGroup>(), false);
		ShowCanvasGroup.Show(_qRClose.GetComponent<CanvasGroup>(), false, .5f);
	}
}
