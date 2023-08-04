using Doozy.Engine.UI;
using Novena.DAL.Model.Guide;
using Novena.Networking;
using Scripts.Utility;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OfferButton : MonoBehaviour {
	[SerializeField] private RawImage _rawImage;
	[SerializeField] private UIButton _button;
	[SerializeField] private TMP_Text _text;

	public static Action<Theme> OnButtonPresed;


	public async void Setup(Theme theme, bool isBig)
	{

		if (_rawImage != null)
			StartCoroutine(RefreshButtonForSoftMask());

		_text.text = theme.Name;
		_button.OnClick.OnTrigger.Event.AddListener(() => {
			OnButtonPresed?.Invoke(theme);

		});
		if (_rawImage != null && theme.ImagePath != null)
		{
			_rawImage.texture = await AssetsFileLoader.LoadTextureAsync(isBig ? theme.GetMediaByName("Gallery").GetPhotos()[0].FullPath : Api.GetFullLocalPath(theme.ImagePath));
			if (_rawImage.GetComponent<AspectRatioFitter>() != null)
				_rawImage.GetComponent<AspectRatioFitter>().aspectRatio = (float)_rawImage.texture.width / _rawImage.texture.height;
		}
		else if (theme.GetMediaByName("Gallery") != null)
			if (theme.GetMediaByName("Gallery").GetPhotos() != null)
			{
				_rawImage.texture = await AssetsFileLoader.LoadTextureAsync(theme.GetMediaByName("Gallery").GetPhotos()[0].FullPath);
				if (_rawImage.GetComponent<AspectRatioFitter>() != null)
					_rawImage.GetComponent<AspectRatioFitter>().aspectRatio = (float)_rawImage.texture.width / _rawImage.texture.height;
			}
	}

	/// <summary>
	/// this is how you make softmaskscript work
	/// </summary>
	private IEnumerator RefreshButtonForSoftMask()
	{
		_rawImage.enabled = false;
		yield return new WaitForEndOfFrame();
		_rawImage.enabled = true;
	}
}
