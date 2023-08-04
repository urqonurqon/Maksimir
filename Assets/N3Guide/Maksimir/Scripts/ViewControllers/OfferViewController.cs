using Cysharp.Threading.Tasks;
using DG.Tweening;
using Doozy.Engine;
using Doozy.Engine.Nody;
using Doozy.Engine.UI;
using Novena.DAL;
using Novena.DAL.Model.Guide;
using Novena.Networking;
using Novena.Networking.Image;
using Novena.UiUtility.Base;
using Scripts.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;
using Image = UnityEngine.UI.Image;

[Flags]
public enum OfferTags {
	EdukativniProgrami = 1,
	KulturnaBastina = 2,
	PrirodnaBastina = 4,
	SportIRekreacija = 8,
	UgostiteljskaPonuda = 16,
	ZOO = 32,
}

public class OfferViewController : UiController, IPointerClickHandler {

	[SerializeField] private CanvasGroup _question;
	[SerializeField] private CanvasGroup _answer;
	[SerializeField] private CanvasGroup _all;
	[SerializeField] private CanvasGroup _nonDetails;
	[SerializeField] private CanvasGroup _details;
	[SerializeField] private CanvasGroup _header;
	[SerializeField] private CanvasGroup _headerAll;
	[SerializeField] private CanvasGroup _yourOfferReadyTxt;

	[SerializeField] private Slider _slider;
	[SerializeField] private UIToggle[] _toggle;
	[SerializeField] private UIToggle[] _reasonToggles;

	[SerializeField] private List<UIToggle> _offerCategories = new List<UIToggle>();


	[SerializeField] private UIButton _showOfferButton;
	[SerializeField] private UIButton _colorAllOffers;
	[SerializeField] private Button _prevButton;
	[SerializeField] private Button _nextButton;


	[SerializeField] private GameObject _offerPrefabBig;
	[SerializeField] private GameObject _offerPrefabBigNoImg;
	[SerializeField] private GameObject _offerPrefabSmall;
	[SerializeField] private GameObject _offerPrefabSmallNoImg;

	[SerializeField] private Transform _oneTwo;
	[SerializeField] private Transform _threeTop;
	[SerializeField] private Transform _threeBottom;
	[SerializeField] private Transform _four;
	[SerializeField] private GameObject _englishEducation;


	[SerializeField] private TMP_FontAsset _boldFont;
	[SerializeField] private TMP_FontAsset _regularFont;
	[SerializeField] private TMP_Text _backButtonText;
	[SerializeField] private TMP_Text _title;
	[SerializeField] private TMP_Text _text;

	//[SerializeField] private RawImage _image;

	[SerializeField] private List<TMP_Text> _offers = new List<TMP_Text>();
	[SerializeField] private List<TMP_Text> _questions = new List<TMP_Text>();
	[SerializeField] private TMP_Text _questionText;
	[SerializeField] private TMP_Text _allText;
	[SerializeField] private TMP_Text _showAllText;
	[SerializeField] private TMP_Text _showOfferText;
	[SerializeField] private TMP_Text _didntFind;
	[SerializeField] private RectTransform _categoriesRect;
	[SerializeField] private RawImage _image;

	[SerializeField] private GallerySnapVariation _fullscreenGallery;
	[SerializeField] private UIButton _fullscreenButton;
	[SerializeField] private UIButton _fullscreenCloseButton;

	private List<string> _listOfPhotos = new List<string>();
	[SerializeField] private List<UIButton> _categories = new List<UIButton>();
	private List<GameObject> _instantiatedThemes = new List<GameObject>();

	private OfferTags _offerTags;

	[Header("Button culling:")]
	[SerializeField] private ScrollRect _offersRect;
	[SerializeField] private List<CanvasGroup> _buttonsCanvasGroup;
	[SerializeField] private float _screenTopEdgePosition;
	[SerializeField] private int _skipIndex;
	private int _lastChangedButtonIndex = 0;
	[SerializeField] private GameObject _bottomImagePrefab;
	[SerializeField] private Transform _bottomContainer;
	[SerializeField] private RawImage _qr;

	//private bool _isLanguageChange;
	private Theme _detailsTheme;

	private List<GameObject> _listOfInstantiatedObjects = new List<GameObject>();
	private List<Theme> _filteredThemes = new List<Theme>();


	private OfferTags _tempOfferTags;
	private int _tempSliderValue;
	public override void Awake()
	{
		base.Awake();

		_offersRect.onValueChanged.RemoveAllListeners();
		_offersRect.onValueChanged.AddListener(OnSrollRectPositionChange);

		_fullscreenButton.OnClick.OnTrigger.Event.RemoveAllListeners();
		_fullscreenButton.OnClick.OnTrigger.Event.AddListener(() => {
			ShowCanvasGroup.Show(_fullscreenGallery.GetComponent<CanvasGroup>(), true, .5f);
			_fullscreenGallery.Setup(_listOfPhotos);
			StartCoroutine(BlurFade(.5f));
		});

		_fullscreenCloseButton.OnClick.OnTrigger.Event.AddListener(() => {
			if (FindObjectOfType<GraphController>().Graph.ActiveNode.Name == "OfferView")
			{
				ShowCanvasGroup.Show(_fullscreenGallery.GetComponent<CanvasGroup>(), false, .5f, 0, DG.Tweening.Ease.OutQuad, () => _fullscreenGallery.ResetGallery());
				StartCoroutine(BlurFade(.5f));
			}
		});

		_prevButton.onClick.RemoveAllListeners();
		_nextButton.onClick.RemoveAllListeners();
		_prevButton.onClick.AddListener(() => ChangeCategorie(240));
		_nextButton.onClick.AddListener(() => ChangeCategorie(-240));
		ShowCanvasGroup.Show(_question, true);

		_showOfferButton.OnClick.OnTrigger.Event.RemoveAllListeners();
		_showOfferButton.OnClick.OnTrigger.Event.AddListener(() => ShowOffer(_offerTags, (int)_slider.value));

		for (int i = 0; i < _toggle.Length; i++)
		{
			int k = i;
			_toggle[i].OnValueChanged.RemoveAllListeners();
			_toggle[i].OnValueChanged.AddListener((isOn) => {
				if (k == 0)
				{
					ShowCanvasGroup.Show(_question, isOn, .5f);
					ShowCanvasGroup.Show(_answer, !isOn, .5f);
					ShowCanvasGroup.Show(_all, !isOn, .5f);
					ShowCanvasGroup.Show(_yourOfferReadyTxt, true, .5f);
					_yourOfferReadyTxt.gameObject.SetActive(true);
					_yourOfferReadyTxt.transform.parent.GetComponent<VerticalLayoutGroup>().padding.top = 0;
				}
				if (k == 1)
				{
					_englishEducation.SetActive(false);
					ShowCanvasGroup.Show(_all, isOn, .5f);
					ShowCanvasGroup.Show(_question, !isOn, .5f);
					ShowCanvasGroup.Show(_answer, !isOn, .5f);
					ShowCanvasGroup.Show(_yourOfferReadyTxt, true, .5f);
					_yourOfferReadyTxt.gameObject.SetActive(true);
					_yourOfferReadyTxt.transform.parent.GetComponent<VerticalLayoutGroup>().padding.top = 0;
				}
				_toggle[k].GetComponentInChildren<TMP_Text>().font = isOn ? _boldFont : _regularFont;
				_toggle[k].GetComponentInChildren<TMP_Text>().DOColor(isOn ? new Color(0.6f, 0.7686275f, 0.3960784f, 1) : Color.white, 0.5f);
				StartCoroutine(GradualFontChange(_toggle[k].GetComponentInChildren<TMP_Text>(), isOn ? 28 : 24));
			});
		}
		OfferButton.OnButtonPresed += async (theme) => await SetThemeDetails(theme);

		int j = 0;
		foreach (OfferTags offer in Enum.GetValues(typeof(OfferTags)))
		{
			var offerBtn = _categories[j];
			int br = j;
			offerBtn.OnClick.OnTrigger.Event.RemoveAllListeners();
			offerBtn.OnClick.OnTrigger.Event.AddListener(() => {
				ChangeCategorie(br * -220);
				_offerCategories[br].IsOn = false;
				_offerCategories[br].IsOn = true;
				ShowCanvasGroup.Show(_header, false, .5f);
				ShowCanvasGroup.Show(_all, false, .5f);
				ShowCanvasGroup.Show(_headerAll, true, .5f);
			});
			_offerCategories[j].OnValueChanged.RemoveAllListeners();
			_offerCategories[j].OnValueChanged.AddListener((isOn) => {
				OnCategorieClick(_offerCategories[br].GetComponentInChildren<TMP_Text>(), isOn);
				if (isOn)
				{
					ShowCanvasGroup.Show(_yourOfferReadyTxt, false, .5f);
					_yourOfferReadyTxt.gameObject.SetActive(false);
					_yourOfferReadyTxt.transform.parent.GetComponent<VerticalLayoutGroup>().padding.top = 65;

					ShowOffer(offer, 2);


				}
			});
			j++;



			for (int i = 0; i < _reasonToggles.Length; i++)
			{
				int l = i;
				_reasonToggles[i].OnValueChanged.RemoveAllListeners();
				_reasonToggles[i].OnValueChanged.AddListener((isOn) => ColorToggles(isOn, _reasonToggles[l], Mathf.Pow(2, l)));
			}
			_colorAllOffers.OnClick.OnTrigger.Event.RemoveAllListeners();
			_colorAllOffers.OnClick.OnTrigger.Event.AddListener(() => {
				for (int i = 0; i < _reasonToggles.Length; i++)
				{
					_reasonToggles[i].IsOn = true;
				}
			});


		}


		Data.OnTranslatedContentUpdated += () => {
			if (FindObjectOfType<GraphController>().Graph.ActiveNode.Name == "OfferView")
			{
				Data.Theme = Data.TranslatedContent.GetThemeByLanguageSwitchCode(Data.Theme.LanguageSwitchCode);
				_backButtonText.text = Data.Theme.Name;
				SetQr().Forget();
				List<Theme> subThemes = new List<Theme>();
				for (int l = 0; l < Data.TranslatedContent.Themes.ToList().Count; l++)
				{
					if (Data.TranslatedContent.Themes[l].SubThemes != null)
						subThemes.AddRange(Data.TranslatedContent.Themes[l].SubThemes.ToList());
				}
				if (_headerAll.alpha > 0)
				{
					for (int j = 0; j < _offerCategories.Count; j++)
					{
						if (_offerCategories[j].IsOn)
						{
							//_isLanguageChange = true;
							_offerCategories[j].Toggle.group.allowSwitchOff = true;
							_offerCategories[j].IsOn = false;
							_offerCategories[j].IsOn = true;
							_offerCategories[j].Toggle.group.allowSwitchOff = false;
						}
					}
				}
				else if (_answer.alpha > 0)
				{
					for (int p = 0; p < _filteredThemes.Count; p++)
					{
						for (int m = 0; m < subThemes.Count; m++)
						{
							if (subThemes[m].LanguageSwitchCode == _filteredThemes[p].LanguageSwitchCode)
							{
								_filteredThemes[p] = subThemes[m];
							}
						}

					}

					ShowOffer(_tempOfferTags, _tempSliderValue);
				}

				if (_details.alpha > 0)
				{
					for (int m = 0; m < subThemes.Count; m++)
					{

						if (subThemes[m].LanguageSwitchCode == _detailsTheme.LanguageSwitchCode)
							_detailsTheme = subThemes[m];
					}
					if (_detailsTheme.LanguageSwitchCode > 2100 && _detailsTheme.LanguageSwitchCode < 2200)
						Back();
					else
					{

						_backButtonText.text = _detailsTheme.Name.Replace("<br>", "");
						_title.text = _detailsTheme.Name.Replace("<br>", "");
						if (_detailsTheme.GetMediaByName("Text") != null)
							_text.text = _detailsTheme.GetMediaByName("Text").Text;
					}
				}


				int i = 0;
				foreach (OfferTags offer in Enum.GetValues(typeof(OfferTags)))
				{
					_categories[i].GetComponentInChildren<TMP_Text>().text = Data.TranslatedContent.GetThemeByName("MISC").GetMediaByName(offer.ToString()).Text;
					_offerCategories[i].GetComponentInChildren<TMP_Text>().text = Data.TranslatedContent.GetThemeByName("MISC").GetMediaByName(offer.ToString()).Text;
					_reasonToggles[i].transform.GetChild(2).GetComponent<TMP_Text>().text = Data.TranslatedContent.GetThemeByName("MISC").GetMediaByName(offer.ToString()).Text.Replace("<br>", " ");
					_reasonToggles[i].transform.GetChild(2).GetComponent<TMP_Text>().text = _reasonToggles[i].transform.GetChild(2).GetComponent<TMP_Text>().text.Replace("- ", "");
					i++;
				}
			}
		};
	}



	public override void OnShowViewStart()
	{
		base.OnShowViewStart();
		ResetValues();
		_backButtonText.text = Data.Theme.Name;
		int i = 0;
		foreach (OfferTags offer in Enum.GetValues(typeof(OfferTags)))
		{
			_categories[i].GetComponentInChildren<TMP_Text>().text = Data.TranslatedContent.GetThemeByName("MISC").GetMediaByName(offer.ToString()).Text;
			_offerCategories[i].GetComponentInChildren<TMP_Text>().text = Data.TranslatedContent.GetThemeByName("MISC").GetMediaByName(offer.ToString()).Text;
			_reasonToggles[i].transform.GetChild(2).GetComponent<TMP_Text>().text = Data.TranslatedContent.GetThemeByName("MISC").GetMediaByName(offer.ToString()).Text.Replace("<br>", " ");
			_reasonToggles[i].transform.GetChild(2).GetComponent<TMP_Text>().text = _reasonToggles[i].transform.GetChild(2).GetComponent<TMP_Text>().text.Replace("- ", "");
			i++;
		}

		SetQr().Forget();
	}

	private async UniTaskVoid SetQr()
	{
		if (Data.Theme.GetMediaByName("QR") != null)
		{
			if (Data.Theme.GetMediaByName("QR").ContentPath != "")
			{
				var tex = await AssetsFileLoader.LoadTextureAsync(Api.GetFullLocalPath(Data.Theme.GetMediaByName("QR").ContentPath));
				_qr.texture = tex;
			}
		}
	}

	private void ChangeCategorie(float amount)
	{
		_categoriesRect.DOAnchorPosX(_categoriesRect.anchoredPosition.x + amount, 0.5f);
	}

	private async UniTask SetThemeDetails(Theme theme)
	{
		_detailsTheme = theme;
		bool isEduTheme = false;
		Theme eduTheme = null;
		if (Data.TranslatedContent.GetThemeByTag("EdukativniProgrami") != null)
		{
			eduTheme = Data.TranslatedContent.GetThemeByTag("EdukativniProgrami");
			isEduTheme = eduTheme.SubThemes.Any((t) => t.Name == theme.Name);
		}
		bool isGallery = false;
		bool isBigPicture = false;
		ShowCanvasGroup.Show(_nonDetails, false, .5f);
		ShowCanvasGroup.Show(_details, true, .5f);
		_listOfPhotos.Clear();
		_backButtonText.text = theme.Name.Replace("<br>", "");
		_image.transform.parent.gameObject.SetActive(false);
		ShowCanvasGroup.Show(_image.transform.parent.GetComponent<CanvasGroup>(), false);
		if (isEduTheme)
			_title.text = "";
		else
			_title.text = theme.Name.Replace("<br>", "");
		if (theme.GetMediaByName("Text") != null)
			_text.text = theme.GetMediaByName("Text").Text;
		else
			_text.text = "";

		if (theme.GetMediaByName("Gallery") != null)
		{
			if (theme.GetMediaByName("Gallery").GetPhotos() != null)
			{
				var gallery = theme.GetMediaByName("Gallery").GetPhotos();
				gallery.ForEach((p) => _listOfPhotos.Add(p.FullPath));

				var bigTex = await AssetsFileLoader.LoadTextureAsync(gallery[0].FullPath);
				ShowCanvasGroup.Show(_image.transform.parent.GetComponent<CanvasGroup>(), true, .5f);
				_image.texture = bigTex;
				_image.GetComponent<AspectRatioFitter>().aspectRatio = (float)bigTex.width / bigTex.height;
				_image.transform.parent.gameObject.SetActive(true);
				isBigPicture = true;

				if (gallery.Count > 1)
				{
					_bottomContainer.parent.parent.gameObject.SetActive(true);
					for (int i = 1; i < gallery.Count; i++)
					{
						var tex = await AssetsFileLoader.LoadTextureAsync(gallery[i].FullPath);
						if (tex != null)
						{
							var photo = Instantiate(_bottomImagePrefab, _bottomContainer);
							_listOfInstantiatedObjects.Add(photo);
							photo.GetComponentInChildren<RawImage>().texture = tex;
							photo.GetComponentInChildren<RawImage>().GetComponent<AspectRatioFitter>().aspectRatio = (float)tex.width / tex.height;
							int j = i;
							photo.GetComponent<UIButton>().OnClick.OnTrigger.Event.AddListener(() => {
								ShowCanvasGroup.Show(_fullscreenGallery.GetComponent<CanvasGroup>(), true, .5f);
								_fullscreenGallery.Setup(_listOfPhotos, j);
								StartCoroutine(BlurFade(.5f));
							});
						}
					}
					isGallery = true;
					if (gallery.Count > 3)
					{
						_bottomContainer.GetComponent<ContentSizeFitter>().enabled = true;
						_bottomContainer.parent.parent.GetChild(0).gameObject.SetActive(true);
						_bottomContainer.parent.parent.GetChild(1).gameObject.SetActive(true);
					}
					else
					{
						_bottomContainer.GetComponent<ContentSizeFitter>().enabled = false;
						_bottomContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(_bottomContainer.parent.GetComponent<RectTransform>().rect.width, _bottomContainer.GetComponent<RectTransform>().sizeDelta.y);

						_bottomContainer.parent.parent.GetChild(0).gameObject.SetActive(false);
						_bottomContainer.parent.parent.GetChild(1).gameObject.SetActive(false);
					}
				}
				else
				{
					_bottomContainer.parent.parent.gameObject.SetActive(false);
				}

				//if (theme.GetMediaByName("Gallery").GetPhotos().Count == 1)
				//{
				//	var tex = await AssetsFileLoader.LoadTextureAsync(theme.GetMediaByName("Gallery").GetPhotos()[0].FullPath);
				//	CheckForAspectRatios();
				//	_image.texture = tex;
				//	_image.transform.parent.gameObject.SetActive(true);
				//	ShowCanvasGroup.Show(_image.transform.parent.GetComponent<CanvasGroup>(), true, .5f);
				//	_gallery.gameObject.SetActive(false);
				//}
				//else
				//{
				//_gallery.gameObject.SetActive(true);
				//_gallery.ResetGallery();
				//_gallery.Setup(_listOfPhotos);
				//}
			}
		}
		StartCoroutine(SetScrollSize(isGallery, isBigPicture));

		LayoutRebuilder.ForceRebuildLayoutImmediate(_details.GetComponent<RectTransform>());
		//StartCoroutine(FixVerticalGroupSpacing());
	}


	private IEnumerator SetScrollSize(bool isGallery, bool isBigPicture)
	{
		yield return new WaitForEndOfFrame();
		if (!isGallery)
			_text.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(
				_text.transform.parent.GetComponent<RectTransform>().sizeDelta.x, _text.rectTransform.sizeDelta.y + _title.rectTransform.sizeDelta.y + 80f);
		else
			_text.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(
				_text.transform.parent.GetComponent<RectTransform>().sizeDelta.x, _text.rectTransform.sizeDelta.y + _title.rectTransform.sizeDelta.y + 80f +
				 _bottomContainer.parent.parent.GetComponent<RectTransform>().sizeDelta.y);

		if (isBigPicture)
			//{
			//	_content.transform.parent.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(_content.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x,
			//		_content.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.y < 660.57f ? 660.57f : _content.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.y);
			_text.transform.parent.parent.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(_text.transform.parent.parent.parent.GetComponent<RectTransform>().sizeDelta.x, 684.02f);
		//}
		else
			//{
			//	_content.transform.parent.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(_content.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x,
			//		_content.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.y < 1181.2f ? 1181.2f : _content.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.y);
			_text.transform.parent.parent.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(_text.transform.parent.parent.parent.GetComponent<RectTransform>().sizeDelta.x, 1102.05f);
		//}
	}


	public void OnSrollRectPositionChange(Vector2 f)
	{
		if (_buttonsCanvasGroup.Count < 10)
			return;
		int maxIteration = 10;
		while (maxIteration > 0)
		{
			int i = _lastChangedButtonIndex;
			if (_offersRect.velocity.y >= 0)
			{
				if (_buttonsCanvasGroup[i] != null)
					if (_offersRect.content.anchoredPosition.y - Mathf.Abs(_buttonsCanvasGroup[i].GetComponent<RectTransform>().anchoredPosition.y) > _screenTopEdgePosition)
					{
						_buttonsCanvasGroup[i].alpha = 0;
						_buttonsCanvasGroup[i + (_buttonsCanvasGroup.Count % 2 == 0 ? 1 : -1)].alpha = 0;
						int y = Mathf.Min(_buttonsCanvasGroup.Count - 2, i + _skipIndex);
						_buttonsCanvasGroup[y].alpha = 1;
						_buttonsCanvasGroup[y + (_buttonsCanvasGroup.Count % 2 == 0 ? 1 : -1)].alpha = 1;
						_lastChangedButtonIndex = Mathf.Min(_buttonsCanvasGroup.Count - 2, i + 2);
						//if (_buttonsCanvasGroup.Count % 2 == 1 && _lastChangedButtonIndex == _buttonsCanvasGroup.Count - 2) _buttonsCanvasGroup[_buttonsCanvasGroup.Count - 1].alpha = 1;
					}
					else
						break;
			}
			else
			{
				if (_buttonsCanvasGroup[i] != null)
					if (_offersRect.content.anchoredPosition.y - Mathf.Abs(_buttonsCanvasGroup[i].GetComponent<RectTransform>().anchoredPosition.y) < _screenTopEdgePosition)
					{
						_buttonsCanvasGroup[i].alpha = 1;
						_buttonsCanvasGroup[i + (_buttonsCanvasGroup.Count % 2 == 0 ? 1 : -1)].alpha = 1;
						int y = Mathf.Min(_buttonsCanvasGroup.Count - 2, i + _skipIndex);
						if (y - _lastChangedButtonIndex < _skipIndex)
						{
							_lastChangedButtonIndex = Mathf.Max(i - 2, _buttonsCanvasGroup.Count % 2 == 0 ? 0 : 1);
							break;
						}
						_lastChangedButtonIndex = Mathf.Max(i - 2, _buttonsCanvasGroup.Count % 2 == 0 ? 0 : 1);
						_buttonsCanvasGroup[y].alpha = 0;
						_buttonsCanvasGroup[y + (_buttonsCanvasGroup.Count % 2 == 0 ? 1 : -1)].alpha = 0;
						//if (_buttonsCanvasGroup.Count % 2 == 1 && _lastChangedButtonIndex == i - 2) _buttonsCanvasGroup[_buttonsCanvasGroup.Count - 1].alpha = 0;
					}
					else
						break;
			}
			maxIteration--;
		}
	}

	//private void CheckForAspectRatios()
	//{
	//	if (_image.GetComponent<AspectRatioFitter>().aspectRatio >= 1)
	//		_image.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
	//	else
	//		_image.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.FitInParent;
	//}


	//private IEnumerator FixVerticalGroupSpacing()
	//{
	//	_details.GetComponent<VerticalLayoutGroup>().enabled = false;
	//	yield return new WaitForEndOfFrame();
	//	_details.GetComponent<VerticalLayoutGroup>().enabled = true;
	//}

	private void ColorToggles(bool isOn, UIToggle toggle, float offer)
	{

		toggle.transform.GetChild(0).GetComponent<Image>().DOColor(isOn ? new Color(0.6f, 0.7686275f, 0.3960784f, 1) : Color.white, 0.5f);
		toggle.transform.GetChild(1).GetComponent<TMP_Text>().DOColor(isOn ? Color.white : new Color(0.1411765f, 0.227451f, 0.1921569f), 0.5f);
		toggle.transform.GetChild(2).GetComponent<TMP_Text>().font = isOn ? _boldFont : _regularFont;

		if (isOn)
		{
			_offerTags |= (OfferTags)offer;
		}
		else
		{
			_offerTags &= ~(OfferTags)offer;
		}
	}



	private void OnCategorieClick(TMP_Text toggleText, bool isOn)
	{
		if (isOn)
		{
			toggleText.DOColor(new Color(0.6f, 0.7686275f, 0.3960784f), 0.5f);
			StartCoroutine(GradualFontChange(toggleText, 28));
			toggleText.font = _boldFont;
		}
		else
		{
			toggleText.DOColor(Color.white, 0.5f);
			StartCoroutine(GradualFontChange(toggleText, 24));
			toggleText.font = _regularFont;
		}
	}

	private IEnumerator GradualFontChange(TMP_Text text, float size)
	{
		float t = 0;
		while (t <= 0.5f)
		{
			text.fontSize = Mathf.Lerp(text.fontSize, size, t);
			t += Time.fixedDeltaTime;

			yield return null;
		}
	}
	private void ResetValues()
	{
		_englishEducation.SetActive(false);

		ShowCanvasGroup.Show(_question, true);
		ShowCanvasGroup.Show(_nonDetails, true);
		ShowCanvasGroup.Show(_answer, false);
		ShowCanvasGroup.Show(_details, false);
		ShowCanvasGroup.Show(_all, false);
		ShowCanvasGroup.Show(_headerAll, false);
		ShowCanvasGroup.Show(_header, true);
		_toggle[0].IsOn = true;

		var scrollViews = GetComponentsInChildren<ScrollRect>().ToList();
		scrollViews.ForEach(s => s.normalizedPosition = new Vector2(0, 1));

		for (int i = 0; i < _reasonToggles.Length; i++)
		{
			_reasonToggles[i].IsOn = false;
		}
		for (int i = 0; i < _buttonsCanvasGroup.Count; i++)
		{
			Destroy(_buttonsCanvasGroup[i]);
		}
		_buttonsCanvasGroup.Clear();
		for (int i = 0; i < _listOfInstantiatedObjects.Count; i++)
		{
			Destroy(_listOfInstantiatedObjects[i]);
		}
		_listOfInstantiatedObjects.Clear();
		_slider.value = 0;
	}





	private IEnumerable<OfferTags> GetFilteredOfferTags(OfferTags offerTags)
	{
		foreach (OfferTags value in Enum.GetValues(offerTags.GetType()))
		{
			if (offerTags.HasFlag(value))
				yield return value;
		}
	}

	private void ShowOffer(OfferTags offerTags, int timeInPark)
	{
		DestroyOffers();
		bool eduExists = false;

		_tempOfferTags = offerTags;
		_tempSliderValue = timeInPark;
		_filteredThemes = FilterOffers(offerTags, timeInPark, out eduExists);
		GameObject offerButton = null;

		switch (_filteredThemes.Count)
		{
			case >= 4:
				_buttonsCanvasGroup.Clear();
				_englishEducation.SetActive(false);
				for (int i = 0; i < _filteredThemes.Count; i++)
				{
					bool themeHasImage = _filteredThemes[i].ImagePath != null;
					if (_filteredThemes[i].GetMediaByName("Gallery") != null)
						if (_filteredThemes[i].GetMediaByName("Gallery").GetPhotos() != null)
							themeHasImage = true;
					offerButton = Instantiate(themeHasImage ? _offerPrefabSmall : _offerPrefabSmallNoImg, _four);
					offerButton.GetComponent<OfferButton>().Setup(_filteredThemes[i], false);
					_instantiatedThemes.Add(offerButton);
					_buttonsCanvasGroup.Add(offerButton.GetComponent<CanvasGroup>());
				}
				_offersRect.verticalNormalizedPosition = 1;
				_lastChangedButtonIndex = _buttonsCanvasGroup.Count % 2 == 0 ? 0 : 1;
				break;
			case 3:
				_buttonsCanvasGroup.Clear();
				_englishEducation.SetActive(false);
				for (int i = 0; i < _filteredThemes.Count; i++)
				{
					bool themeHasImage = _filteredThemes[i].ImagePath != null;
					if (_filteredThemes[i].GetMediaByName("Gallery") != null)
						if (_filteredThemes[i].GetMediaByName("Gallery").GetPhotos() != null)
							themeHasImage = true;
					if (i == 0)
					{
						offerButton = Instantiate(themeHasImage ? _offerPrefabBig : _offerPrefabBigNoImg, _threeTop);
						offerButton.GetComponent<OfferButton>().Setup(_filteredThemes[i], true);
					}
					else
					{
						offerButton = Instantiate(themeHasImage ? _offerPrefabSmall : _offerPrefabSmallNoImg, _threeBottom);
						offerButton.GetComponent<OfferButton>().Setup(_filteredThemes[i], false);
					}
					_instantiatedThemes.Add(offerButton);
				}

				break;

			case <= 0:
				if (!eduExists && (offerTags == OfferTags.EdukativniProgrami || _offerTags == OfferTags.EdukativniProgrami))
				{
					_englishEducation.SetActive(true);
					_buttonsCanvasGroup.Clear();
				}
				else
					_englishEducation.SetActive(false);
				break;

			default:
				_englishEducation.SetActive(false);
				_buttonsCanvasGroup.Clear();
				for (int i = 0; i < _filteredThemes.Count; i++)
				{
					bool themeHasImage = _filteredThemes[i].ImagePath != null;
					if (_filteredThemes[i].GetMediaByName("Gallery") != null)
						if (_filteredThemes[i].GetMediaByName("Gallery").GetPhotos() != null)
							themeHasImage = true;
					offerButton = Instantiate(themeHasImage ? _offerPrefabBig : _offerPrefabBigNoImg, _oneTwo);
					offerButton.GetComponent<OfferButton>().Setup(_filteredThemes[i], true);
					_instantiatedThemes.Add(offerButton);
				}
				break;
		}

		ShowCanvasGroup.Show(_question, false, .5f);
		ShowCanvasGroup.Show(_answer, true, .5f);

	}

	private void DestroyOffers()
	{
		for (int i = 0; i < _instantiatedThemes.Count; i++)
		{
			Destroy(_instantiatedThemes[i]);
		}
		_instantiatedThemes.Clear();
	}

	private List<Theme> FilterOffers(OfferTags offerTags, int timeInPark, out bool eduExists)
	{
		eduExists = false;
		List<Theme> filteredThemes = new List<Theme>();
		List<SubTheme> filteredSubThemes = new List<SubTheme>();

		foreach (var filteredOfferTags in GetFilteredOfferTags(offerTags))
		{
			if ((filteredOfferTags == OfferTags.EdukativniProgrami && Data.TranslatedContent.GetThemesByTag(filteredOfferTags.ToString()).Count > 0))
				eduExists = true;
			filteredThemes.AddRange(Data.TranslatedContent.GetThemesByTag(filteredOfferTags.ToString()));
		}



		for (int i = 0; i < filteredThemes.Count; i++)
		{

			if (timeInPark == 0)
			{
				filteredSubThemes.AddRange(filteredThemes[i].GetSubThemesByTag("Tag0"));
			}
			else if (timeInPark == 1)
			{
				filteredSubThemes.AddRange(filteredThemes[i].GetSubThemesByTag("Tag0"));
				filteredSubThemes.AddRange(filteredThemes[i].GetSubThemesByTag("Tag30"));
			}
			else if (timeInPark == 2)
			{
				filteredSubThemes.AddRange(filteredThemes[i].GetSubThemesByTag("Tag0"));
				filteredSubThemes.AddRange(filteredThemes[i].GetSubThemesByTag("Tag30"));
				filteredSubThemes.AddRange(filteredThemes[i].GetSubThemesByTag("Tag60"));
			}



		}

		filteredThemes.Clear();

		for (int i = 0; i < filteredSubThemes.Count; i++)
		{
			filteredThemes.Add(filteredSubThemes[i]);
		}


		return filteredThemes;
	}
	private IEnumerator BlurFade(float time)
	{
		Material material = _fullscreenGallery.transform.GetChild(0).GetComponent<Image>().material;
		Material roundedMat = _fullscreenGallery._objectsInList[0].PhotoRawImage.material;
		while (time > 0)
		{
			yield return null;
			time -= Time.deltaTime;
			material.SetFloat("_Alpha", _fullscreenGallery.GetComponent<CanvasGroup>().alpha);
			roundedMat.SetFloat("_Alpha", _fullscreenGallery.GetComponent<CanvasGroup>().alpha);
		}
	}
	public void Back()
	{
		if (_details.alpha > 0)
		{
			ShowCanvasGroup.Show(_details, false, .5f);
			ShowCanvasGroup.Show(_nonDetails, true, .5f);
			_backButtonText.text = Data.Theme.Name;
			for (int i = 0; i < _listOfInstantiatedObjects.Count; i++)
			{
				Destroy(_listOfInstantiatedObjects[i]);
			}
			_listOfInstantiatedObjects.Clear();
		}
		else
		{
			if (_headerAll.alpha > 0)
			{
				ShowCanvasGroup.Show(_headerAll, false, .5f);
				ShowCanvasGroup.Show(_header, true, .5f);
				_englishEducation.SetActive(false);
				ShowCanvasGroup.Show(_answer, false, .5f);
				ShowCanvasGroup.Show(_all, true, .5f);
			}
			else
				GameEventMessage.SendEvent("Back");
		}
	}
	public void OnPointerClick(PointerEventData eventData)
	{

		int linkIndex = TMP_TextUtilities.FindIntersectingLink(_didntFind, Input.mousePosition, Camera.main);
		if (linkIndex != -1)
		{ // was a link clicked?
			TMP_LinkInfo linkInfo = _didntFind.textInfo.linkInfo[linkIndex];

			// open the link id as a url, which is the metadata we added in the text field
			if (linkInfo.GetLinkID() == "AllOffer")
			{
				_toggle[1].IsOn = true;
			}
		}
	}
	public override void OnHideViewStart()
	{
		base.OnHideViewStart();

		ShowCanvasGroup.Show(_fullscreenGallery.GetComponent<CanvasGroup>(), false);
		StartCoroutine(BlurFade(0));
	}

}
