using Cysharp.Threading.Tasks;
using Doozy.Engine.Nody;
using Novena.DAL;
using Novena.DAL.Model.Guide;
using Scripts.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using DG.Tweening;
using Novena.Networking;

public class HeritageButton : MonoBehaviour {
	public static Action<Theme, Theme[]> OnMoreSubThemes;
	public static Action<Theme> OnThemeClicked;

	private Button _button;
	private TMP_Text _text;
	private RawImage _rawImage;
	private Theme _theme;
	private CanvasGroup _moreTheme;

	private void Awake()
	{
		_button = GetComponentInChildren<Button>();
		_text = GetComponentInChildren<TMP_Text>();
		_rawImage = GetComponentInChildren<RawImage>();
		_moreTheme = GetComponentInChildren<CanvasGroup>();

		Data.OnTranslatedContentUpdated += () => {
			if (FindObjectOfType<GraphController>().Graph.ActiveNode.Name == "HeritageView")
			{
				List<Theme> subThemes = new List<Theme>();
				for (int l = 0; l < Data.TranslatedContent.Themes.ToList().Count; l++)
				{
					if (Data.TranslatedContent.Themes[l].SubThemes != null)
						subThemes.AddRange(Data.TranslatedContent.Themes[l].SubThemes.ToList());
				}

				for (int i = 0; i < subThemes.Count; i++)
				{

					if (subThemes[i].LanguageSwitchCode == _theme.LanguageSwitchCode)
					{
						_theme = subThemes[i];

					}
				}
				_text.text = _theme.Name;
				_button.onClick.RemoveAllListeners();
				_button.onClick.AddListener(() => {
					if (_theme.Label == "Sub")
					{
						var subThemes = Data.TranslatedContent.Themes.First(x => x.Name == _theme.Name).SubThemes;
						OnMoreSubThemes?.Invoke(_theme, subThemes);
					}
					else
					{
						OnThemeClicked.Invoke(_theme);
					}
				});
			}
		};
	}

	public async UniTaskVoid Setup(Theme theme)
	{
		_theme = theme;
		_text.text = _theme.Name;

		if (_rawImage != null && theme.ImagePath != null)
		{
			_rawImage.texture = await AssetsFileLoader.LoadTextureAsync(Api.GetFullLocalPath(theme.ImagePath));
			//_rawImage.GetComponent<AspectRatioFitter>().aspectRatio = (float)_rawImage.texture.width / _rawImage.texture.height;
		}
		else if (_theme.GetMediaByName("Gallery") != null)
			if (_theme.GetMediaByName("Gallery").GetPhotos() != null)
			{
				_rawImage.texture = await AssetsFileLoader.LoadTextureAsync(_theme.GetMediaByName("Gallery").GetPhotos()[0].FullPath);
				//_rawImage.GetComponent<AspectRatioFitter>().aspectRatio = (float)_rawImage.texture.width / _rawImage.texture.height;
			}
		if (_theme.Label == "Sub")
			_moreTheme.DOFade(1, .5f);
		else
			_moreTheme.DOFade(0, .5f);

		_button.onClick.AddListener(() => {
			if (_theme.Label == "Sub")
			{
				var subThemes = Data.TranslatedContent.Themes.First(x => x.Name == _theme.Name).SubThemes;
				OnMoreSubThemes?.Invoke(_theme, subThemes);
			}
			else
			{
				OnThemeClicked.Invoke(_theme);
			}
		});


	}
}
