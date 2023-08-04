using Doozy.Engine.UI;
using Novena.DAL;
using Novena.DAL.Model.Guide;
using Novena.Networking.Image;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using Scripts.Utility;
using Novena.Networking;

public class LanguageButton : MonoBehaviour {
	public static Action OnLanguageButtonClicked;

	[SerializeField] private RawImage _thumbnailRawImage;

	private TranslatedContent _translatedContent;

	public void Setup(TranslatedContent translatedContent)
	{
		_translatedContent = translatedContent;

		SetThumbnail();

		var uiToggle = GetComponent<UIToggle>();
		var rawImage = GetComponent<RawImage>();

		uiToggle.Toggle.group = GetComponentInParent<ToggleGroup>();

		//Don't forget to unsub all events
		uiToggle.OnValueChanged.RemoveAllListeners();

		uiToggle.OnValueChanged.AddListener((isOn) => {
			if (isOn)
			{
				rawImage.DOFade(1, 0.5f);
				if (Data.TranslatedContent != translatedContent)
					Data.TranslatedContent = translatedContent;

			}
			else
			{
				rawImage.DOFade(0.35f, 0.5f);

			}

		});
		//if (transform.parent.tag == "Side")
		//	uiToggle.OnClick.OnToggleOn.Event.AddListener(() => OnLanguageButtonClicked?.Invoke());
	}

	private async void SetThumbnail()
	{
		var thumbPath = Api.GetFullLocalPath(_translatedContent.LanguageThumbnailPath);

		if (thumbPath != null)
		{
			var tex = await AssetsFileLoader.LoadTextureAsync(thumbPath);
			_thumbnailRawImage.texture = tex;
			//_thumbnailRawImage.SetNativeSize();
		}
	}
}
