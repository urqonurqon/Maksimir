using Novena.DAL;
using Novena.DAL.Model.Guide;
using Novena.Helpers;
using Novena.UiUtility.Base;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Doozy.Engine.UI;
using Image = UnityEngine.UI.Image;
using UnityEngine.SceneManagement;
using Novena.Utility;

public class ThemeListController : UiController {




	[Header("MainMenu components")]
	[SerializeField] protected Transform _mainMenuContainer;
	[SerializeField] protected GameObject _mainMenuBtnPrefabWhite;
	[SerializeField] protected GameObject _mainMenuBtnPrefabGreen;
	[SerializeField] protected GameObject _mainMenuBtnPrefabGreenExtend;

	[Header("Header components")]
	[SerializeField] protected UIToggle _themeListToggle;
	[SerializeField] protected UIToggle _languageListToggle;

	[SerializeField] protected Image _themeListToggleIcon;
	[SerializeField] protected Image _themeListToggleIconClose;
	protected Image _bannerBackground;
	[SerializeField] protected Image _languageListToggleIcon;
	[SerializeField] protected Image _languageListSlideInBackground;
	[SerializeField] protected ToggleGroup _colorToggleGroup;




	#region protected fields

	protected List<GameObject> _menuButtonList;
	protected RectTransform _rt;

	#endregion

	public override void Awake()
	{
		base.Awake();
		_menuButtonList = new List<GameObject>();
		_rt = GetComponent<RectTransform>();
		MenuButton.OnMenuButtonClicked += MenuButtonClicked;
		if (!SceneManager.GetActiveScene().name.Contains("Mali"))
			FooterController.OnContainerInstantiated += (container) => _bannerBackground = container.GetComponent<Image>();

		_themeListToggle.OnValueChanged.RemoveAllListeners();
		_themeListToggle.OnValueChanged.AddListener((isOn) => {
			if (isOn)
			{
				if (!SceneManager.GetActiveScene().name.Contains("Mali"))
					_bannerBackground.DOColor(new Color(0.1411765f, 0.227451f, 0.1921569f, 1), .8f);
				ChangeColors(new Color(0.1411765f, 0.227451f, 0.1921569f, 1), new Color(0.6f, 0.7686275f, 0.3960785f, 1));
				_themeListToggleIcon.DOFade(0, 0.8f);
				_themeListToggleIconClose.DOFade(1, 0.8f);
				_rt.DOAnchorPosX(0, .5f);
			}
			else
			{
				if (!SceneManager.GetActiveScene().name.Contains("Mali"))
					_bannerBackground.DOColor(Color.white, .8f);
				ChangeColors(new Color(0.6f, 0.7686275f, 0.3960785f, 1), new Color(0.1411765f, 0.227451f, 0.1921569f, 1));
				_themeListToggleIcon.DOFade(1, 0.8f);
				_themeListToggleIconClose.DOFade(0, 0.8f);
			}
		});
		_themeListToggle.OnClick.OnToggleOff.Event.RemoveAllListeners();
		_themeListToggle.OnClick.OnToggleOff.Event.AddListener(() => {
			if (!SceneManager.GetActiveScene().name.Contains("Mali"))
				_bannerBackground.DOColor(Color.white, .8f);
			_rt.DOAnchorPosX(-1080, .5f);
			ChangeColors(new Color(0.6f, 0.7686275f, 0.3960785f, 1), new Color(0.1411765f, 0.227451f, 0.1921569f, 1));
			_themeListToggleIcon.DOFade(1, 0.8f);
			_themeListToggleIconClose.DOFade(0, 0.8f);
		});

		MenuButton.OnMenuButtonClicked += () => _rt.DOAnchorPosX(-1080, .5f).OnComplete(UiBlocker.Disable);


		Data.OnTranslatedContentUpdated += () => {
			for (int i = 0; i < _menuButtonList.Count; i++)
			{
				Destroy(_menuButtonList[i]);
			}
			_menuButtonList.Clear();
			GenerateMainMenu();
		};
	}

	protected void MenuButtonClicked()
	{
		_themeListToggle.IsOn = false;
	}

	protected void ChangeColors(Color iconColor, Color backgroundColor)
	{
		_languageListToggle.GetComponent<Image>().DOColor(backgroundColor, 0.8f);
		_themeListToggle.GetComponent<Image>().DOColor(backgroundColor, 0.8f);

		_languageListToggleIcon.DOColor(iconColor, 0.8f);
		_themeListToggleIcon.DOColor(iconColor, 0.8f);
		_themeListToggleIconClose.DOColor(iconColor, 0.8f);

		//_languageListSlideInBackground.DOColor(iconColor,.8f);
	}

	public override void OnShowViewStart()
	{
		base.OnShowViewStart();
		foreach (var toggle in _colorToggleGroup.ActiveToggles())
		{
			toggle.isOn = false;
		}
		GenerateMainMenu();
		_rt.anchoredPosition = new Vector2(-1080f, 0);
	}




	public virtual void GenerateMainMenu()
	{
		UnityHelper.DestroyObjects(_menuButtonList);

		var themeList = Data.TranslatedContent.GetThemesExcludeByTag("SYSTEM");

		

		for (int i = 0; i < themeList.Count; i++)
		{
			GameObject go = null;
			MenuButton mb = null;
			Theme theme = themeList[i];

			if (theme.Label != "Green")
				go = Instantiate(_mainMenuBtnPrefabWhite, _mainMenuContainer);
			else
			{
				if (!theme.ContainsTag("Extend"))
					go = Instantiate(_mainMenuBtnPrefabGreen, _mainMenuContainer);
				else
					go = Instantiate(_mainMenuBtnPrefabGreenExtend, _mainMenuContainer);
			}



			mb = go.GetComponent<MenuButton>();
			if (mb.ColorToggle != null)
				mb.ColorToggle.Toggle.group = _colorToggleGroup;

			mb.SetButton(theme);


			go.SetActive(true);

			_menuButtonList.Add(go);

		}
	}
	public override void OnHideViewFinished()
	{
		base.OnHideViewFinished();
		_themeListToggle.IsOn = false;

		for (int i = 0; i < _menuButtonList.Count; i++)
		{
			Destroy(_menuButtonList[i]);
		}
		_menuButtonList.Clear();
	}
}

