using Cysharp.Threading.Tasks;
using Doozy.Engine;
using Doozy.Engine.Nody;
using Doozy.Engine.UI;
using Novena.DAL;
using Novena.DAL.Model.Guide;
using Novena.Networking;
using Scripts.Utility;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VideoButton : MonoBehaviour {

	public static Action<Theme> OnVideoClicked;
	[SerializeField] private TMP_Text _title;
	[SerializeField] private TMP_Text _text;
	[SerializeField] private RawImage _rawImage;

	private UIButton _button;
	private Theme _theme;
	private void Awake()
	{
		_button = GetComponent<UIButton>();

		Data.OnTranslatedContentUpdated += () => {
			if (FindObjectOfType<GraphController>().Graph.ActiveNode.Name == "VideoView")
			{
				var videoParentTheme = Data.TranslatedContent.GetThemeByLanguageSwitchCode(3000);
				_theme = videoParentTheme.GetSubThemeByLanguageSwitchCode(_theme.LanguageSwitchCode);
				_title.text = _theme.Name;

				//if (_theme.GetMediaByName("Text") != null)
				//	_title.text = _theme.GetMediaByName("Text").Text;
				_button.OnClick.OnTrigger.Event.RemoveAllListeners();
				_button.OnClick.OnTrigger.Event.AddListener(() => {
					OnVideoClicked?.Invoke(_theme);
					GameEventMessage.SendEvent("GoToVideoDetails");
				});
			}
		};
	}
	public async UniTaskVoid Setup(Theme theme)
	{
		_theme = theme;
		_title.text = theme.Name;
		//if (theme.GetMediaByName("Text") != null)
		//	_title.text = theme.GetMediaByName("Text").Text;
		Texture2D tex = null;
		if (theme.ImagePath != null)
			tex = await AssetsFileLoader.LoadTextureAsync(Api.GetFullLocalPath(theme.ImagePath));
		_rawImage.texture = tex;

		_button.OnClick.OnTrigger.Event.AddListener(() => {
			OnVideoClicked?.Invoke(theme);
			GameEventMessage.SendEvent("GoToVideoDetails");
		});
	}

	public TMP_Text GetTitle()
	{
		return _title;
	}
	public TMP_Text GetText()
	{
		return _text;
	}
}
