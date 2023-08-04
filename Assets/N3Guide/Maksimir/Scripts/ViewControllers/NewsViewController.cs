using Cysharp.Threading.Tasks;
using DanielLochner.Assets.SimpleScrollSnap;
using DG.Tweening;
using Doozy.Engine;
using Doozy.Engine.Nody;
using Doozy.Engine.UI;
using Kiosk;
using Novena.DAL;
using Novena.UiUtility.Base;
using Scripts.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utility;
using Image = UnityEngine.UI.Image;

public class NewsViewController : UiController {

	public static Action<News> OnNewsButtonClick;
	public static Action OnShowView;

	[SerializeField] private DynamicContent _dynamicContentTop;
	[SerializeField] private DynamicContent _dynamicContentBot;
	[SerializeField] private CanvasGroup _body;
	[SerializeField] private CanvasGroup _loader;
	[SerializeField] private GameObject _sideInfo;
	[SerializeField] private Button _sideInfoClose;
	//[SerializeField] private Button _kioskModButton;


	private News _warning;
	private List<News> _actualities;
	private List<News> _happenings;


	private News _currentNews;
	private int _newsCounter;

	private Coroutine _loadingRoutine;

	private bool _isLanguageChangedFromDetails;

	public override void Awake()
	{
		base.Awake();


		//_kioskModButton.onClick.RemoveAllListeners();
		//_kioskModButton.onClick.AddListener(() => {
		//	if (KioskController.Instance.IsOn == true)
		//	{
		//		KioskController.Instance.DisableKioskMode();
		//	}
		//	else
		//	{
		//		KioskController.Instance.EnableKioskMode();
		//	}
		//});

		if (SceneManager.GetActiveScene().name.Contains("Mali"))
		{
			_sideInfo.SetActive(true);
		}
		else
		{
			_sideInfo.SetActive(false);
		}


		_sideInfo.GetComponent<Button>().onClick.RemoveAllListeners();
		_sideInfo.GetComponent<Button>().onClick.AddListener(() => {
			_sideInfo.GetComponentInChildren<ScrollRect>().verticalNormalizedPosition = 1;
			_sideInfo.transform.GetChild(0).GetComponent<Image>().DOFade(0, .5f);
			_sideInfo.GetComponent<RectTransform>().DOSizeDelta(new Vector2(325, 280), .5f).OnComplete(() => _sideInfo.GetComponent<Image>().raycastTarget = true);

			_sideInfo.GetComponent<Image>().raycastTarget = false;
			ShowCanvasGroup.Show(_sideInfoClose.GetComponent<CanvasGroup>(), true, .5f, .3f);
			ShowCanvasGroup.Show(_sideInfo.GetComponentInChildren<TMP_Text>().GetComponent<CanvasGroup>(), true, .5f, .3f);
		});

		_sideInfoClose.onClick.RemoveAllListeners();
		_sideInfoClose.onClick.AddListener(() => {

			_sideInfo.transform.GetChild(0).GetComponent<Image>().DOFade(1, .5f);
			//_sideInfo.GetComponent<RectTransform>().DOSizeDelta(new Vector2(68, 68), .5f).SetDelay(.3f).OnComplete(() => _sideInfo.GetComponent<Image>().raycastTarget = true);
			_sideInfo.GetComponent<RectTransform>().DOSizeDelta(new Vector2(100, 98), .5f).SetDelay(.3f);


			ShowCanvasGroup.Show(_sideInfoClose.GetComponent<CanvasGroup>(), false, .5f);
			ShowCanvasGroup.Show(_sideInfo.GetComponentInChildren<TMP_Text>().GetComponent<CanvasGroup>(), false, .5f);

		});


		_dynamicContentTop.scrollSnap.OnPanelInstantiated = null;
		_dynamicContentTop.scrollSnap.OnPanelInstantiated += async (panel) => {
			await SetupNewsPrefab(panel);
		};
		_dynamicContentBot.scrollSnap.OnPanelInstantiated = null;
		_dynamicContentBot.scrollSnap.OnPanelInstantiated += async (panel) => {
			await SetupNewsPrefab(panel);
		};



		Data.OnTranslatedContentUpdated += () => {
			if ((FindObjectOfType<GraphController>().Graph.ActiveNode.Name == "NewsView") || (FindObjectOfType<GraphController>().Graph.ActiveNode.Name == "DetailsView"))
			{
				_isLanguageChangedFromDetails = true;
				OnHideViewFinished();
				OnShowViewStart();
				OnShowViewFinished();
			}
		};

	}

	public void ResetSideMenu()
	{
		_sideInfo.transform.GetChild(0).GetComponent<Image>().DOFade(1, 0);
		_sideInfo.GetComponent<Image>().raycastTarget = true;
		_sideInfo.GetComponent<RectTransform>().DOSizeDelta(new Vector2(100, 98), 0);
		_sideInfo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -573.4f);
		ShowCanvasGroup.Show(_sideInfo.GetComponentInChildren<TMP_Text>().GetComponent<CanvasGroup>(), false);
		ShowCanvasGroup.Show(_sideInfoClose.GetComponent<CanvasGroup>(), false);
	}

	public async override void OnShowViewStart()
	{

		base.OnShowViewStart();
		ResetSideMenu();


		if ((FindObjectOfType<GraphController>().Graph.PreviousActiveNode.Name != "DetailsView" || _isLanguageChangedFromDetails) && AssetsFileLoader.ListOfLoadingPhotos.Count == 0)
		{
			_isLanguageChangedFromDetails = false;
			ShowCanvasGroup.Show(_body, false);
			ShowCanvasGroup.Show(_loader, true);

			int topChildCount = _dynamicContentTop.scrollSnap.Content.childCount;
			int botChildCount = _dynamicContentBot.scrollSnap.Content.childCount;
			for (int i = 0; i < topChildCount; i++)
			{
				_dynamicContentTop.RemoveFromBack();
			}
			for (int i = 0; i < botChildCount; i++)
			{
				_dynamicContentBot.RemoveFromBack();
			}


			_newsCounter = 0;
			_loader.transform.rotation = Quaternion.identity;
			_loadingRoutine = StartCoroutine(LoaderTurning());
			await NewsData.GetWarningJson();
			GetChosenLanguageNews();
			GenerateButtons();
			if (_warning != null)
				SetupWarningButton(_warning);
		}

		OnShowView?.Invoke();

	}

	private IEnumerator LoaderTurning()
	{
		while (true)
		{
			_loader.transform.Rotate(Vector3.back);
			yield return null;
		}
	}

	private void GetChosenLanguageNews()
	{
		_warning = null;
		_actualities = NewsData.Actualities.Where((a) => a.LanguageCode == Data.TranslatedContent.ContentTitle).ToList();
		_happenings = NewsData.Happenings.Where((h) => h.LanguageCode == Data.TranslatedContent.ContentTitle).ToList();
		var warnings = NewsData.Warnings.Where((w) => w.LanguageCode == Data.TranslatedContent.ContentTitle).ToList();
		if (warnings.Count > 0)
			_warning = warnings[0];
	}



	private void InstantiateAndAddToScrollSnap(bool toBack, DynamicContent dynamicContent)
	{
		if (toBack)
			dynamicContent.AddToBack();
		else
			dynamicContent.AddToFront();
	}

	private void GenerateButtons()
	{
		for (int i = 0; i < _actualities.Count; i++)
		{
			_currentNews = _actualities[i];
			InstantiateAndAddToScrollSnap(true, _dynamicContentTop);
		}
		for (int i = 0; i < _happenings.Count; i++)
		{
			_currentNews = _happenings[i];
			InstantiateAndAddToScrollSnap(true, _dynamicContentBot);
		}

	}


	private void SetupWarningButton(News warning)
	{
		_currentNews = warning;
		InstantiateAndAddToScrollSnap(false, _dynamicContentTop);
	}

	private async UniTask SetupNewsPrefab(GameObject panel)
	{
		var currentNews = _currentNews;
		panel.GetComponentsInChildren<TMP_Text>()[0].text = currentNews.Title.ReplaceHTMLTags();
		//panel.GetComponentsInChildren<TMP_Text>()[1].text = currentNews.IntroText.ReplaceHTMLTags();
		panel.GetComponentsInChildren<TMP_Text>()[1].text =
			currentNews.Date.Day + "."
			+ currentNews.Date.Month + "."
			+ currentNews.Date.Year + "." + "\n"
			+ (currentNews.Date.Hour / 10 < 1 ? "0" : "")
			+ currentNews.Date.Hour
			+ ":" + (currentNews.Date.Minute / 10 < 1 ? "0" : "")
			+ currentNews.Date.Minute;


		panel.GetComponent<UIButton>().OnClick.OnTrigger.Event.AddListener(() => {
			GameEventMessage.SendEvent("GoToDetailsView");
			OnNewsButtonClick?.Invoke(currentNews);
		});

		panel.GetComponentInChildren<RawImage>().DOFade(0, 0);
		Texture2D tex = null;
		if (currentNews.ImagePath != "http://maksimir.s13.novenaweb.info")
			tex = await AssetsFileLoader.LoadTextureAsync(currentNews.ImagePath, 5, true);
		if (tex != null)
		{
			panel.GetComponentInChildren<RawImage>().texture = tex;
			panel.GetComponentInChildren<RawImage>().GetComponent<AspectRatioFitter>().aspectRatio = (float)tex.width / tex.height;
		}
		panel.GetComponentInChildren<RawImage>().DOFade(1, 0.5f);


		//if (!_listOfLoadingPhotos.Contains(currentNews.ImagePath))
		//{
		//	if (!await AssetsFileLoader.LoadTextureAsync(currentNews.ImagePath, panel.GetComponentInChildren<RawImage>(), 5))
		//	{
		//		await AssetsFileLoader.LoadTextureAsyncFromCache(currentNews.ImagePath, panel.GetComponentInChildren<RawImage>());

		//		panel.GetComponentInChildren<RawImage>().DOFade(1, .5f);

		if (currentNews.CategoryId == 4)
		{
			panel.transform.GetChild(1).gameObject.SetActive(true);
		}

		_newsCounter++;
		if (_newsCounter == _happenings.Count + _actualities.Count + (_warning != null ? 1 : 0))
		{
			ShowCanvasGroup.Show(_body, true, .5f);
			ShowCanvasGroup.Show(_loader, false, .5f);
			StopCoroutine(_loadingRoutine);
		}


		//		_listOfLoadingPhotos.Add(currentNews.ImagePath);
		//		await AssetsFileLoader.LoadTextureAsyncIntoCache(currentNews.ImagePath);
		//		_listOfLoadingPhotos.Remove(currentNews.ImagePath);
		//	}
		//}
		//else
		//{
		//	await AssetsFileLoader.LoadTextureAsyncFromCache(currentNews.ImagePath, panel.GetComponentInChildren<RawImage>());

		//	panel.GetComponentInChildren<RawImage>().DOFade(1, .5f);

		//	if (currentNews.CategoryId == 4)
		//	{
		//		panel.transform.GetChild(1).gameObject.SetActive(true);
		//	}

		//	_newsCounter++;
		//	if (_newsCounter == _happenings.Count + _actualities.Count + (_warning != null ? 1 : 0))
		//	{
		//		ShowCanvasGroup.Show(_body, true, .5f);
		//		ShowCanvasGroup.Show(_loader, false, .5f);
		//		StopCoroutine(_loadingRoutine);
		//	}

		//}





	}




	//public override void OnHideViewFinished()
	//{
	//	base.OnHideViewFinished();

	//	if (FindObjectOfType<GraphController>().Graph.ActiveNode.Name != "DetailsView")
	//	{
	//		int topChildCount = _dynamicContentTop.scrollSnap.Content.childCount;
	//		int botChildCount = _dynamicContentBot.scrollSnap.Content.childCount;
	//		for (int i = 0; i < topChildCount; i++)
	//		{
	//			_dynamicContentTop.RemoveFromBack();
	//		}
	//		for (int i = 0; i < botChildCount; i++)
	//		{
	//			_dynamicContentBot.RemoveFromBack();
	//		}
	//	}
	//}
}
