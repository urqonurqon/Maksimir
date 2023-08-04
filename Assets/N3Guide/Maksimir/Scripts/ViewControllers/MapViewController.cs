using Assets.Scripts.Components.PinchScrollRect;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Doozy.Engine.Nody;
using Doozy.Engine.UI;
using Novena.DAL;
using Novena.DAL.Model.Guide;
using Novena.Networking;
using Novena.UiUtility.Base;
using Novena.Utility;
using Scripts.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utility;

public class MapViewController : UiController {
	[SerializeField] private Button _prevButton;
	[SerializeField] private Button _nextButton;


	[SerializeField] private RectTransform _categoriesRect;

	[SerializeField] private UIToggle[] _mapCategories;

	[SerializeField] private TMP_FontAsset _bold;
	[SerializeField] private TMP_FontAsset _regular;

	private List<GameObject> _routePins = new List<GameObject>();
	private List<GameObject> _routes = new List<GameObject>();
	private List<List<GameObject>> _regularPins = new List<List<GameObject>>();

	private List<List<Theme>> _regularThemes = new List<List<Theme>>();
	private List<Theme> _routeThemes = new List<Theme>();

	[SerializeField] private Transform _routePinParent;
	[SerializeField] private Transform _routeParent;
	[SerializeField] private Transform _pinParent;
	[SerializeField] private GameObject _routePinPrefab;
	[SerializeField] private GameObject _routePrefab;
	[SerializeField] private GameObject _pinPrefab;
	[SerializeField] private SideQR _sideQR;
	[SerializeField] private CanvasGroup _loader;
	[SerializeField] private TMP_Text _backText;

	private PinchableScrollRect _pinchScrollRect;
	private List<Categorie> _categories = new List<Categorie>();
	private List<Theme> _pinThemes = new List<Theme>();
	private CanvasGroup _activeTextBox;
	private CanvasGroup _activeRoute;
	private RectTransform _activePin;
	private Coroutine _loadingRoutine;
	public override void Awake()
	{

		base.Awake();
		//InitController.OnGuideLoaded += GetPinsAndRoutes;

		_prevButton.onClick.RemoveAllListeners();
		_prevButton.onClick.AddListener(() => ChangeCategorie(164.56f));
		_nextButton.onClick.RemoveAllListeners();
		_nextButton.onClick.AddListener(() => ChangeCategorie(-164.56f));

		_pinchScrollRect = FindObjectOfType<PinchableScrollRect>();


		_categories.Clear();
		for (int i = 0; i < _mapCategories.Length; i++)
		{

			_categories.Add(new Categorie(_mapCategories[i]));

			int j = i;
			_mapCategories[i].OnValueChanged.RemoveAllListeners();
			_mapCategories[i].OnValueChanged.AddListener((isOn) => {
				HideAllPins();
				if (j == 0 && isOn)
					ShowPins(_categories[j]);
				else if (j != 0)
					OnMapCategorieClick(_categories[j], isOn, (j < 5) ? true : false);
			});
		}



		Data.OnTranslatedContentUpdated += () => {
			if (FindObjectOfType<GraphController>().Graph.ActiveNode.Name == "MapView")
			{
				Data.Theme = Data.TranslatedContent.GetThemeByLanguageSwitchCode(Data.Theme.LanguageSwitchCode);
				List<Theme> subThemes = new List<Theme>();
				for (int l = 0; l < Data.TranslatedContent.Themes.ToList().Count; l++)
				{
					if (Data.TranslatedContent.Themes[l].SubThemes != null)
						subThemes.AddRange(Data.TranslatedContent.Themes[l].SubThemes.ToList());
				}
				for (int p = 0; p < _pinThemes.Count; p++)
				{
					for (int m = 0; m < subThemes.Count; m++)
					{
						if (subThemes[m].LanguageSwitchCode == _pinThemes[p].LanguageSwitchCode)
						{
							_pinThemes[p] = subThemes[m];
						}
					}

				}


				SetText();

				SetQr().Forget();

				var subThemeList = Data.Theme.SubThemes.ToList();

				int g = 0;
				for (int i = 0; i < _routeThemes.Count; i++)
				{
					for (int j = 0; j < subThemeList.Count; j++)
					{
						if (subThemeList[j].LanguageSwitchCode == _routeThemes[i].LanguageSwitchCode)
						{
							_routeThemes[i] = subThemeList[j];
						}
					}
				}
				for (int i = 0; i < _routeThemes.Count; i++)
				{
						_routePins[i].GetComponentInChildren<TMP_Text>().text = _routeThemes[i].Name;

				}


				List<List<Theme>> regularThemesTranslated = new List<List<Theme>>();
				for (int i = 0; i < _regularThemes.Count; i++)
				{
					for (int j = 0; j < _regularThemes[i].Count; j++)
					{
						for (int k = 0; k < subThemeList.Count; k++)
						{
							if (subThemeList[k].LanguageSwitchCode == _regularThemes[i][j].LanguageSwitchCode)
							{
								_regularThemes[i][j] = subThemeList[k];

							}
						}
					}
				}

				for (int i = 0; i < _regularThemes.Count; i++)
				{
					for (int j = 0; j < _regularThemes[i].Count; j++)
					{
						_regularPins[i][j].GetComponentInChildren<TMP_Text>().text = _regularThemes[i][j].Name;
					}
				}


			}
		};
	}


	public override async void OnShowViewStart()
	{
		base.OnShowViewStart();
		OnHideViewFinished();
		UiBlocker.Enable();

		ShowCanvasGroup.Show(_loader, true);
		_loadingRoutine = StartCoroutine(LoaderTurning());
		SetText();
		_sideQR.Reset();
		await SetQr();
		await GetPinsAndRoutes();
		ShowCanvasGroup.Show(_loader, false, .5f, 0, Ease.OutQuad, () => {

			StopCoroutine(_loadingRoutine);
			UiBlocker.Disable();
		});
		PinchableScrollRect.OnZoom -= ScalePins;
		PinchableScrollRect.OnZoom += ScalePins;
		StartCoroutine(FrameMap(new Vector2(0.1908828f, 0.2750408f), 2.318162f, new Vector2(-184.831f, 57.65663f), .5f, .2f));

		_mapCategories[0].IsOn = true;

		if (_activeTextBox != null)
			ShowCanvasGroup.Show(_activeTextBox, false, .5f);
		if (_activeRoute != null)
			ShowCanvasGroup.Show(_activeRoute, false, .5f);
	}

	private async UniTask SetQr()
	{
		if (Data.Theme.GetMediaByName("QR") != null)
		{
			if (Data.Theme.GetMediaByName("QR").ContentPath != "")
			{
				var tex = await AssetsFileLoader.LoadTextureAsync(Api.GetFullLocalPath(Data.Theme.GetMediaByName("QR").ContentPath));
				_sideQR.QRImage.GetComponent<RawImage>().texture = tex;
			}
		}
	}
	private IEnumerator LoaderTurning()
	{
		while (true)
		{
			_loader.transform.Rotate(Vector3.back);
			yield return null;
		}
	}
	private void SetText()
	{
		for (int i = 5; i < _mapCategories.Length; i++)
		{
			_mapCategories[i].GetComponentInChildren<TMP_Text>().text = Data.Theme.GetMediaByName("Tag" + (i - 3).ToString()).Text;
		}
		for (int i = 1; i < 5; i++)
		{
			_mapCategories[i].GetComponentInChildren<TMP_Text>().text = Data.Theme.GetMediaByName("Tag1" + Encoding.ASCII.GetString(new byte[] { (byte)(i + 96) })).Text;
		}


		_backText.text = Data.Theme.Name;


	}

	private async UniTask GetPinsAndRoutes()
	{
		var subThemes = Data.TranslatedContent.GetThemeByTag("MapView").SubThemes;

		List<Theme> routes = new List<Theme>();
		List<Theme> tag1 = new List<Theme>();
		List<Theme> tag2 = new List<Theme>();
		List<Theme> tag3 = new List<Theme>();
		List<Theme> tag4 = new List<Theme>();
		List<Theme> tag5 = new List<Theme>();
		List<Theme> tag6 = new List<Theme>();
		List<Theme> tag1a = new List<Theme>();
		List<Theme> tag1b = new List<Theme>();
		List<Theme> tag1c = new List<Theme>();
		List<Theme> tag1d = new List<Theme>();
		_regularThemes.Clear();
		_routeThemes.Clear();
		_pinThemes.Clear();
		for (int i = 0; i < subThemes.Length; i++)
		{
			switch (GetThemeTagByCategoryName(subThemes[i].Tags, "MapCat").Title)
			{
				case "Ruta":
					if (!SceneManager.GetActiveScene().name.Contains("Mali"))
						routes.Add(subThemes[i]);
					break;
				case "SmallRuta":
					if (SceneManager.GetActiveScene().name.Contains("Mali"))
						routes.Add(subThemes[i]);
					break;

				case "Tag1":
					tag1.Add(subThemes[i]);
					break;
				case "Tag2":
					tag2.Add(subThemes[i]);
					break;
				case "Tag3":
					tag3.Add(subThemes[i]);
					break;
				case "Tag4":
					tag4.Add(subThemes[i]);
					break;
				case "Tag5":
					tag5.Add(subThemes[i]);
					break;
				case "Tag6":
					tag6.Add(subThemes[i]);
					break;
			}
		}

		for (int i = 0; i < tag1.Count; i++)
		{
			switch (GetThemeTagByCategoryName(tag1[i].Tags, "Info").Title)
			{
				case "Tag1a":
					tag1a.Add(tag1[i]);
					break;
				case "Tag1b":
					tag1b.Add(tag1[i]);
					break;
				case "Tag1c":
					tag1c.Add(tag1[i]);
					break;
				case "Tag1d":
					tag1d.Add(tag1[i]);
					break;
			}
		}






		_regularThemes.Add(tag1a);
		_regularThemes.Add(tag1b);
		_regularThemes.Add(tag1c);
		_regularThemes.Add(tag1d);

		_regularThemes.Add(tag2);
		_regularThemes.Add(tag3);
		_regularThemes.Add(tag4);
		_regularThemes.Add(tag5);
		_regularThemes.Add(tag6);

		_routeThemes.AddRange(routes);

		_regularThemes.ForEach(r => _pinThemes.AddRange(r));
		_pinThemes.AddRange(routes);
		List<CanvasGroup> tmpCGRoutes = new List<CanvasGroup>();

		for (int i = 0; i < routes.Count; i++)
		{
			var routePin = Instantiate(_routePinPrefab, _routePinParent);

			tmpCGRoutes.Add(routePin.GetComponent<CanvasGroup>());




			var routePinRT = routePin.GetComponent<RectTransform>();
			var route = Instantiate(_routePrefab, _routeParent).GetComponent<RawImage>();
			float routePosX = 0f;
			float routePosY = 0f;
			try
			{
				float.TryParse(routes[i].GetMediaByName("Position").Text.Split(",")[0], out routePosX);
				float.TryParse(routes[i].GetMediaByName("Position").Text.Split(",")[1], out routePosY);
			}
			catch (Exception e)
			{
				print(e);
			}
			_routePins.Add(routePin);
			_routes.Add(route.gameObject);
			route.rectTransform.anchoredPosition = new Vector2(routePosX / 10.29f - 0.8f, routePosY / 10.29f - 0.8f);
			//print(i + "=" + route.rectTransform.sizeDelta);
			route.rectTransform.sizeDelta /= 5.14f/* new Vector2(route.rectTransform.sizeDelta.x / (5.14268f), route.rectTransform.sizeDelta.y / (5.14268f))*/;
			//print(i + "=" + route.rectTransform.sizeDelta);
			routePinRT.anchoredPosition = new Vector2(routes[i].PositionX /*+ routePinRT.sizeDelta.x / 2 - 10*/, routes[i].PositionY /*+ 4*/);
			routePin.GetComponentInChildren<TMP_Text>().text = routes[i].Name;
			int j = i;
			//var zoom = 3.0592315E-10f * Mathf.Pow(route.GetComponent<RectTransform>().sizeDelta.x * route.GetComponent<RectTransform>().sizeDelta.y, 2)
			//	- 0.0000659999f * (route.GetComponent<RectTransform>().sizeDelta.x * route.GetComponent<RectTransform>().sizeDelta.y)
			//	+ 4.32512f;
			//var zoom = 14.3953f - 0.931481f * Mathf.Log(4.36428f * Mathf.Max(route.rectTransform.sizeDelta.x * route.rectTransform.sizeDelta.y) + 1);

			await LoadImg(routes[i], route);
			//var zoom = 4.5f - 0.00853022f * Mathf.Max(route.rectTransform.sizeDelta.x, route.rectTransform.sizeDelta.y);
			float zoom = (float)-0.005325f * Mathf.Max(route.rectTransform.sizeDelta.x, route.rectTransform.sizeDelta.y) + 3.875f;
			routePin.GetComponent<Button>().onClick.AddListener(() => {
				routePin.transform.SetAsLastSibling();
				routePin.transform.parent.SetAsLastSibling();

				if (_activeTextBox != null)
					ShowCanvasGroup.Show(_activeTextBox, false, .5f);
				if (_activeRoute != null)
					ShowCanvasGroup.Show(_activeRoute, false, .5f);
				if (_activePin != null)
					_activePin.DOSizeDelta(routePinRT.sizeDelta, .5f);

				if (_activePin != routePinRT)
					routePinRT.DOSizeDelta(routePinRT.sizeDelta * 1.5f, .5f);
				ShowCanvasGroup.Show(routePin.transform.GetChild(0).GetComponent<CanvasGroup>(), true, .5f);
				ShowCanvasGroup.Show(route.GetComponent<CanvasGroup>(), true, .5f);


				_activePin = routePinRT;
				_activeTextBox = routePin.transform.GetChild(0).GetComponent<CanvasGroup>();
				_activeRoute = route.GetComponent<CanvasGroup>();

				var routeCenter = route.GetComponent<RectTransform>().anchoredPosition + route.GetComponent<RectTransform>().sizeDelta / 2 + new Vector2(10, 0);
				StartCoroutine(FrameMap(routeCenter / _pinParent.GetComponent<RectTransform>().sizeDelta, zoom, Vector2.zero, .5f));
			});

			routePin.transform.GetChild(0).GetComponentInChildren<Button>().onClick.AddListener(() => {
				HideRoutePin(routePin, routePinRT, route);
			});
		}

		_categories[0].CanvasGroups = tmpCGRoutes;

		for (int i = 0; i < _regularThemes.Count; i++)
		{
			List<GameObject> tmp = new List<GameObject>();
			List<CanvasGroup> tmpCG = new List<CanvasGroup>();
			for (int h = 0; h < _regularThemes[i].Count; h++)
			{
				var pin = Instantiate(_pinPrefab, _pinParent);
				var pinRT = pin.GetComponent<RectTransform>();
				tmp.Add(pin);
				tmpCG.Add(pin.GetComponent<CanvasGroup>());
				pinRT.anchoredPosition = new Vector2(_regularThemes[i][h].PositionX - (pinRT.sizeDelta.x / 2), _regularThemes[i][h].PositionY);
				pin.GetComponentInChildren<TMP_Text>().text = _regularThemes[i][h].Name;
				pin.GetComponent<Button>().onClick.AddListener(() => {
					if (_activeTextBox != null)
						ShowCanvasGroup.Show(_activeTextBox, false, .5f);
					if (_activePin != null)
						_activePin.DOSizeDelta(pinRT.sizeDelta, .5f);

					if (_activePin != pinRT)
						pinRT.DOSizeDelta(pinRT.sizeDelta * 1.5f, .5f);
					ShowCanvasGroup.Show(pin.transform.GetChild(0).GetComponent<CanvasGroup>(), true, .5f);

					_activePin = pinRT;
					_activeTextBox = pin.transform.GetChild(0).GetComponent<CanvasGroup>();


					pin.transform.SetAsLastSibling();
					pin.transform.parent.SetAsLastSibling();


					var pinCenter = pinRT.anchoredPosition + pinRT.sizeDelta / 2;
					StartCoroutine(FrameMap(pinCenter / _pinParent.GetComponent<RectTransform>().sizeDelta, 7, Vector2.zero, .5f));
				});
				pin.transform.GetChild(0).GetComponentInChildren<Button>().onClick.AddListener(() => {
					HideBubble(pin, pinRT);
				});

			}
			_categories[i + 1].CanvasGroups = tmpCG;
			_regularPins.Add(tmp);
		}






	}

	private void HideRoutePin(GameObject routePin, RectTransform routePinRT, RawImage route)
	{
		ShowCanvasGroup.Show(routePin.transform.GetChild(0).GetComponent<CanvasGroup>(), false, .5f);
		ShowCanvasGroup.Show(route.GetComponent<CanvasGroup>(), false, .5f);
		_activeTextBox = null;
		_activeRoute = null;
		if (_activePin != null)
			_activePin.DOSizeDelta(_activePin.sizeDelta / 1.5f, .5f);
		_activePin = null;
	}

	private void HideBubble(GameObject pin, RectTransform pinRT)
	{
		ShowCanvasGroup.Show(pin.transform.GetChild(0).GetComponent<CanvasGroup>(), false, .5f);
		_activeTextBox = null;
		if (_activePin != null)
			_activePin.DOSizeDelta(_activePin.sizeDelta / 1.5f, .5f);
		_activePin = null;
	}

	private async UniTask LoadImg(Theme route, RawImage rawImage)
	{
		if (route.ImagePath != null)
		{
			var tex = await AssetsFileLoader.LoadTextureAsync(Api.GetFullLocalPath(route.ImagePath));
			rawImage.texture = tex;
		}

		rawImage.SetNativeSize();
		rawImage.rectTransform.sizeDelta = new Vector2(rawImage.rectTransform.sizeDelta.x / 10.29f, rawImage.rectTransform.sizeDelta.y / 10.29f);
	}

	private void DestroyPins()
	{
		for (int i = 0; i < _routePins.Count; i++)
		{
			Destroy(_routePins[i]);
			Destroy(_routes[i]);
		}
		_routePins.Clear();
		_routes.Clear();


		for (int i = 0; i < _regularPins.Count; i++)
		{
			for (int j = 0; j < _regularPins[i].Count; j++)
			{
				Destroy(_regularPins[i][j]);
			}
			_regularPins[i].Clear();
		}
		_regularPins.Clear();

	}

	private IEnumerator FrameMap(Vector2 pivot, float zoomTo, Vector2 anchorPos, float pivotAndZoomTime = 0, float delay = 0)
	{
		var zoomStartValue = _pinchScrollRect._currentZoom;
		var pinchContent = _pinchScrollRect.content;
		pinchContent.DOPivot(pivot, pivotAndZoomTime);
		DOVirtual.Float(zoomStartValue, zoomTo, pivotAndZoomTime, (v) => _pinchScrollRect._currentZoom = v);
		yield return new WaitForEndOfFrame();
		pinchContent.DOAnchorPos(anchorPos, 0.33f).SetDelay(delay);
	}

	private void ScalePins(Vector3 scale)
	{
		for (int i = 0; i < _routePins.Count; i++)
		{
			_routePins[i].transform.localScale = new Vector3(1.5f / Mathf.Sqrt(_pinchScrollRect.content.localScale.x), 1.5f / Mathf.Sqrt(_pinchScrollRect.content.localScale.y), 1);
		}

		for (int i = 0; i < _regularPins.Count; i++)
			for (int j = 0; j < _regularPins[i].Count; j++)
				_regularPins[i][j].transform.localScale = new Vector3(1.5f / Mathf.Sqrt(_pinchScrollRect.content.localScale.x), 1.5f / Mathf.Sqrt(_pinchScrollRect.content.localScale.y), 1);
	}

	private void OnMapCategorieClick(Categorie categorie, bool isOn, bool isSide)
	{
		TMP_Text toggleText = categorie.Toggle.GetComponentInChildren<TMP_Text>();
		if (isOn)
		{
			ShowPins(categorie);

			toggleText.DOColor(new Color(0.6f, 0.7686275f, 0.3960784f), 0.5f);
			if (!isSide)
				StartCoroutine(GradualFontChange(toggleText, 28));
			toggleText.font = _bold;
		}
		else
		{
			toggleText.DOColor(Color.white, 0.5f);
			if (!isSide)
				StartCoroutine(GradualFontChange(toggleText, 24));
			toggleText.font = _regular;
		}
	}


	private IEnumerator GradualFontChange(TMP_Text text, float size)
	{
		float t = 0;
		while (t <= 0.5f)
		{
			text.fontSize = Mathf.Lerp(text.fontSize, size, t);
			t += Time.deltaTime;

			yield return null;
		}
	}



	private void ShowPins(Categorie categorie)
	{
		for (int i = 0; i < categorie.CanvasGroups.Count; i++)
		{
			ShowCanvasGroup.Show(categorie.CanvasGroups[i], true, .5f);
		}
	}

	private void HideAllPins()
	{
		for (int i = 0; i < _routes.Count; i++)
		{
			ShowCanvasGroup.Show(_routes[i].GetComponent<CanvasGroup>(), false, .5f);
			ShowCanvasGroup.Show(_routePins[i].GetComponent<CanvasGroup>(), false, .5f);
			HideRoutePin(_routePins[i], _routePins[i].GetComponent<RectTransform>(), _routes[i].GetComponent<RawImage>());
		}

		for (int i = 0; i < _regularPins.Count; i++)
		{
			for (int j = 0; j < _regularPins[i].Count; j++)
			{
				ShowCanvasGroup.Show(_regularPins[i][j].GetComponent<CanvasGroup>(), false, .5f);
				HideBubble(_regularPins[i][j], _regularPins[i][j].GetComponent<RectTransform>());
			}
		}
	}

	private void ChangeCategorie(float amount)
	{
		_categoriesRect.DOAnchorPosX(_categoriesRect.anchoredPosition.x + amount, 0.5f);
	}



	public override void OnHideViewFinished()
	{
		base.OnHideViewFinished();
		//_sideQR.Reset();
		DestroyPins();
		_categories.ForEach((c) => c.Toggle.IsOn = false);
		//_categories.Clear();
		_pinchScrollRect._currentZoom = 1;
		_pinchScrollRect.content.pivot = new Vector2(0.5f, 0.5f);
	}



#nullable enable
	private Tag? GetThemeTagByCategoryName(Tag[] tags, string categoryName)
	{
		TagCategorie? tagCategorie = Data.TranslatedContent.GetTagCategoryByName(categoryName);

		if (tagCategorie == null) return null;

		Tag? tag = tags.FirstOrDefault(tag => tag.TagCategoryId == tagCategorie.Id);

		return tag;
	}
}
#nullable disable

public class Categorie {

	public UIToggle Toggle { get; set; }
	public List<CanvasGroup> CanvasGroups { get; set; }


	public Categorie(UIToggle toggle)
	{
		Toggle = toggle;
	}

}
