using Cysharp.Threading.Tasks;
using DG.Tweening;
using Doozy.Engine;
using Doozy.Engine.UI;
using Novena.Components.Idle;
using Novena.DAL;
using Novena.UiUtility.Base;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utility;

public class LanguageViewController : UiController {


	[SerializeField] private GameObject _languagePrefab;
	[SerializeField] private TMP_Text _appVersionTmp;
	[SerializeField] private TMP_Text _sloganText;
	[SerializeField] private Transform _buttonContainer;
	[SerializeField] private GameObject _adBanner;
	[SerializeField] private GameObject _nonAdBanner;
	private RectTransform _banner;

	[SerializeField] private UIButton _enterNewsViewButton;

	private readonly List<GameObject> _buttons = new List<GameObject>();


	private List<string> _slogan = new List<string>();
	private IEnumerator _sloganCoroutine;

	private List<News> _news = new List<News>();


	[SerializeField] private TMP_Text _warningOverlayText;
	[SerializeField] private CanvasGroup _warningOverlay;
	[SerializeField] private CanvasGroup _circlePulse;



	public override void Awake()
	{
		base.Awake();
		if (!SceneManager.GetActiveScene().name.Contains("Mali"))
			FooterController.OnContainerInstantiated += (container) => _banner = container;

		_appVersionTmp.text = Application.version;
		_enterNewsViewButton.OnClick.OnTrigger.Event.RemoveAllListeners();
		_enterNewsViewButton.OnClick.OnTrigger.Event.AddListener(() => GameEventMessage.SendEvent("GoToNewsView"));

		IdleController.OnIdleEnabled += async () => {
			Data.Theme = null;
			await NewsData.GetWarningJson();
			if (NewsData.Warnings.Count > 0)
			{
				List<News> homepageWarnings = new List<News>();
				if (AnyNewsIsFrontPage(NewsData.Warnings, out homepageWarnings))
					SetupWarningOverlay(homepageWarnings);
			}
		};

		Data.OnTranslatedContentUpdated += () => {
			if (_sloganCoroutine != null)
			{
				StopCoroutine(_sloganCoroutine);
				_sloganCoroutine = null;
				_sloganCoroutine = SloganWrite();
			}
			_sloganText.text = "";
			_slogan.Clear();
			_slogan.Add(Data.TranslatedContent.GetThemeByName("MISC").GetMediaByName("Text9").Text);
			_slogan.Add(Data.TranslatedContent.GetThemeByName("MISC").GetMediaByName("Text8").Text);
			if (_sloganCoroutine!=null)
			StartCoroutine(_sloganCoroutine);
		};
	}

	public override async void OnShowViewStart()
	{
		base.OnShowViewStart();
		_adBanner.SetActive(true);
		_nonAdBanner.SetActive(false);
		if (!SceneManager.GetActiveScene().name.Contains("Mali"))
			_banner.DOAnchorPosY(321f, .5f);
		await NewsData.GetNews();
		_warningOverlay.transform.GetChild(0).GetComponent<Image>().material.SetFloat("_Alpha", 0);
		ShowCanvasGroup.Show(_warningOverlay, false, 0);

		GenerateLanguageButtons();

		//_slogan.Clear();
		//_slogan.Add(Data.TranslatedContent.GetThemeByName("MISC").GetMediaByName("Text8").Text);
		//_slogan.Add(Data.TranslatedContent.GetThemeByName("MISC").GetMediaByName("Text9").Text);

		//_slogan.Add("Dobrodošli u park Maksimir.");
		//_slogan.Add("Maksimalno na strani prirode.");
		if (_sloganCoroutine != null)
		{
			StopCoroutine(_sloganCoroutine);
			_sloganCoroutine = null;
		}
		_slogan.Clear();
		_slogan.Add(Data.TranslatedContent.GetThemeByName("MISC").GetMediaByName("Text9").Text);
		_slogan.Add(Data.TranslatedContent.GetThemeByName("MISC").GetMediaByName("Text8").Text);
		_sloganText.text = _slogan[_slogan.Count - 1];
		_sloganCoroutine = SloganWrite();
		StartCoroutine(_sloganCoroutine);
		await NewsData.GetWarningJson();
		if (NewsData.Warnings.Count > 0)
		{
			List<News> homepageWarnings = new List<News>();
			if (AnyNewsIsFrontPage(NewsData.Warnings, out homepageWarnings))
			{
				SetupWarningOverlay(homepageWarnings);

			}
		}

	}

	private bool AnyNewsIsFrontPage(List<News> warnings, out List<News> homepageWarnings)
	{
		bool isFront = false;
		homepageWarnings = new List<News>();
		for (int i = 0; i < warnings.Count; i++)
		{
			if (warnings[i].IsFrontPage)
			{
				homepageWarnings.Add(warnings[i]);
				isFront = true;
			}
		}
		return isFront;
	}

	private IEnumerator SloganWrite()
	{
		while (true)
		{
			for (int i = 0; i < _slogan.Count; i++)
			{
				yield return new WaitForSeconds(5);
				_sloganText.text = "";
				for (int j = 0; j < _slogan[i].Length; j++)
				{
					_sloganText.text += _slogan[i][j];

					yield return new WaitForSeconds(0.05f);
				}
			}


		}
	}

	private void SetupWarningOverlay(List<News> warnings)
	{
		ShowCanvasGroup.Show(_circlePulse, true, .5f);
		ShowCanvasGroup.Show(_warningOverlay, true, 0.5f);
		StartCoroutine(WarningOverlayBlurFade(.5f));

		string warningTitleStyle = "<font=DalaFloda/DalaFloda-Black SDF><size=100>";
		string warningTitleStyleEnd = "</font></size><br><br>";
		_warningOverlayText.text = "";
		for (int i = 0; i < warnings.Count; i++)
		{
			warnings[i].Title = warnings[i].Title.ReplaceHTMLTags();
			warnings[i].IntroText = warnings[i].IntroText.ReplaceHTMLTags();
			_warningOverlayText.text += warningTitleStyle + warnings[i].Title + warningTitleStyleEnd + warnings[i].IntroText + "<br><br><br>";
		}
	}

	public void HideWarningOverlay()
	{
		float time = 0.5f;
		ShowCanvasGroup.Show(_warningOverlay, false, time);
		StartCoroutine(WarningOverlayBlurFade(time));
	}

	private IEnumerator WarningOverlayBlurFade(float time)
	{
		Material material = _warningOverlay.transform.GetChild(0).GetComponent<Image>().material;
		while (time > 0)
		{
			yield return null;
			time -= Time.deltaTime;
			material.SetFloat("_Alpha", _warningOverlay.alpha);
		}
	}

	private void GenerateLanguageButtons()
	{
		if (Data.Guide.TranslatedContents.Any() == false) return;
		Data.TranslatedContent = Data.Guide.TranslatedContents[0];

		foreach (var tc in Data.Guide.TranslatedContents)
		{
			GameObject obj = Instantiate(_languagePrefab, _buttonContainer);
			obj.SetActive(true);
			obj.GetComponent<LanguageButton>().Setup(tc);
			if (Data.TranslatedContent == tc)
			{
				obj.GetComponent<UIToggle>().IsOn = true;
				obj.GetComponent<RawImage>().DOFade(1, 0);
			}
			else
				obj.GetComponent<UIToggle>().IsOn = false;
			_buttons.Add(obj);
		}
	}

	public override void OnHideViewFinished()
	{
		base.OnHideViewFinished();
		_adBanner.SetActive(false);
		_nonAdBanner.SetActive(true);
		if (!SceneManager.GetActiveScene().name.Contains("Mali"))
			_banner.DOAnchorPosY(198, .3f);
		DestroyGameObjects();
		StopCoroutine(_sloganCoroutine);
		NewsData.Warnings.Clear();
		Resources.UnloadUnusedAssets();
	}

	private void DestroyGameObjects()
	{
		for (int i = 0; i < _buttons.Count; i++)
		{
			Destroy(_buttons[i].gameObject);
		}
		_buttons.Clear();
	}
}
