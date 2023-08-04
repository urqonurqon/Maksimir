using Doozy.Engine;
using Doozy.Engine.Nody;
using Novena.DAL;
using Novena.DAL.Model.Guide;
using Novena.UiUtility.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utility;
using DG.Tweening;
using TMPro;
using Scripts.Utility;
using Cysharp.Threading.Tasks;
using Doozy.Engine.UI;
using Image = UnityEngine.UI.Image;
using System.Linq;

public class HeritageViewController : UiController {


	[SerializeField] private GameObject _hierarchyPrefab;
	[SerializeField] private GameObject _heritagePrefabNoImg;
	[SerializeField] private GameObject _heritagePrefab;
	[SerializeField] private Transform _hierarchyParent;


	private List<Hierarchy> _instantiatedHierarchies = new List<Hierarchy>();
	private Hierarchy _currentHierarchy = new Hierarchy();
	[SerializeField] private CanvasGroup _heritageDetail;
	[SerializeField] private ScrollRect _scrollRect;
	[SerializeField] private TMP_Text _title;
	[SerializeField] private TMP_Text _text;
	[SerializeField] private RawImage _image;

	[SerializeField] private TMP_Text _backButtonText;

	[SerializeField] private GallerySnapVariation _fullscreenGallery;
	[SerializeField] private UIButton _fullscreenButton;
	[SerializeField] private UIButton _fullscreenCloseButton;
	[SerializeField] private GameObject _bottomImagePrefab;
	[SerializeField] private Transform _bottomContainer;
	private List<GameObject> _listOfInstantiatedObjects = new List<GameObject>();
	private List<string> _listOfPhotos = new List<string>();
	private Theme _theme;
	private List<Theme> _themes = new List<Theme>();
	private Theme[] _subThemes;
	private int _hierarchyIndex = -1;
	private Theme _detailsTheme;

	public override void Awake()
	{
		base.Awake();
		HeritageButton.OnMoreSubThemes += InstantiateAndFillHierarchy;
		HeritageButton.OnThemeClicked += (theme) => {
			SetHeritageDetails(theme).Forget();
		};

		_fullscreenButton.OnClick.OnTrigger.Event.RemoveAllListeners();
		_fullscreenButton.OnClick.OnTrigger.Event.AddListener(() => {
			ShowCanvasGroup.Show(_fullscreenGallery.GetComponent<CanvasGroup>(), true, .5f);
			_fullscreenGallery.Setup(_listOfPhotos);
			StartCoroutine(BlurFade(.5f));
		});

		_fullscreenCloseButton.OnClick.OnTrigger.Event.AddListener(() => {
			if (FindObjectOfType<GraphController>().Graph.ActiveNode.Name == "HeritageView")
			{
				ShowCanvasGroup.Show(_fullscreenGallery.GetComponent<CanvasGroup>(), false, .5f,0,DG.Tweening.Ease.OutQuad,() => _fullscreenGallery.ResetGallery());
		StartCoroutine(BlurFade(.5f));
			}
		});

		Data.OnTranslatedContentUpdated += () => {
			if (FindObjectOfType<GraphController>().Graph.ActiveNode.Name == "HeritageView")
			{
				List<Theme> allThemes = new List<Theme>();
				for (int l = 0; l < Data.TranslatedContent.Themes.ToList().Count; l++)
				{
					if (Data.TranslatedContent.Themes[l].SubThemes != null)
						allThemes.AddRange(Data.TranslatedContent.Themes[l].SubThemes.ToList());
					allThemes.Add(Data.TranslatedContent.Themes[l]);
				}
				if (_heritageDetail.alpha > 0)
				{
					for (int i = 0; i < allThemes.Count; i++)
					{
						if (_detailsTheme.LanguageSwitchCode == allThemes[i].LanguageSwitchCode)
							_detailsTheme = allThemes[i];
					}


					_backButtonText.text = _detailsTheme.Name.Replace("<br>", "");
					_title.text = _detailsTheme.Name.Replace("<br>", "");
					if (_detailsTheme.GetMediaByName("Text") != null)
						_text.text = _detailsTheme.GetMediaByName("Text").Text;
					else
						_text.text = "";


				}



				for (int i = 0; i < allThemes.Count; i++)
				{
					for (int j = 0; j < _themes.Count; j++)
					{
						if (_themes[j].LanguageSwitchCode == allThemes[i].LanguageSwitchCode)
						{
							_themes[j] = allThemes[i];
						}
					}
				}

				for (int i = 0; i < _instantiatedHierarchies.Count; i++)
				{
					_instantiatedHierarchies[i].Name = _themes[i].Name;
				}
				_backButtonText.text = _themes[_themes.Count - 1].Name;

			}
		};
	}


	public override void OnShowViewStart()
	{
		base.OnShowViewStart();
		_theme = Data.Theme;
		var subThemes = _theme.SubThemes;
		_hierarchyIndex = -1;
		_currentHierarchy = null;
		var scrollViews = _text.GetComponentInParent<ScrollRect>();
		scrollViews.normalizedPosition = new Vector2(0, 1);
		ShowCanvasGroup.Show(_heritageDetail, false);
		DestroyHierarchies();


		InstantiateAndFillHierarchy(_theme, subThemes);



	}

	private void InstantiateAndFillHierarchy(Theme theme, Theme[] subThemes)
	{
		_hierarchyIndex++;
		GetComponent<CanvasGroup>().blocksRaycasts = false;
		_currentHierarchy = new Hierarchy();
		var hierarchy = Instantiate(_hierarchyPrefab, _hierarchyParent);
		_currentHierarchy.CanvasGroup = hierarchy.GetComponent<CanvasGroup>();
		_currentHierarchy.Name = theme.Name;
		_backButtonText.text = _currentHierarchy.Name;
		_themes.Add(theme);
		ShowCanvasGroup.Show(_currentHierarchy.CanvasGroup, true, .5f, 0, Ease.OutQuad, () => GetComponent<CanvasGroup>().blocksRaycasts = true);

		_instantiatedHierarchies.Add(_currentHierarchy);
		if (_hierarchyIndex > 0)
			ShowCanvasGroup.Show(_instantiatedHierarchies[_hierarchyIndex - 1].CanvasGroup, false, .5f);

		_scrollRect.content = _currentHierarchy.CanvasGroup.GetComponent<RectTransform>();

		for (int i = 0; i < subThemes.Length; i++)
		{
			GameObject heritage;
			if (subThemes[i].GetMediaByName("Gallery") != null)
				if (subThemes[i].GetMediaByName("Gallery").GetPhotos() != null)
					heritage = Instantiate(_heritagePrefab, _currentHierarchy.CanvasGroup.transform);
				else
					heritage = Instantiate(_heritagePrefabNoImg, _currentHierarchy.CanvasGroup.transform);
			else
				heritage = Instantiate(_heritagePrefabNoImg, _currentHierarchy.CanvasGroup.transform);
			heritage.GetComponent<HeritageButton>().Setup(subThemes[i]).Forget();
		}

	}

	private async UniTaskVoid SetHeritageDetails(Theme theme)
	{
		_detailsTheme = theme;
		bool isGallery = false;
		bool isBigPicture = false;
		_listOfPhotos.Clear();
		ShowCanvasGroup.Show(_image.transform.parent.GetComponent<CanvasGroup>(), false);
		ShowCanvasGroup.Show(_currentHierarchy.CanvasGroup, false, .5f);
		_backButtonText.text = theme.Name.Replace("<br>", "");
		_image.transform.parent.gameObject.SetActive(false);
		_title.text = theme.Name.Replace("<br>", "");
		if (theme.GetMediaByName("Text") != null)
			_text.text = theme.GetMediaByName("Text").Text;
		else
			_text.text = "";

		ShowCanvasGroup.Show(_heritageDetail, true, .5f);
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

				}
				else
				{
					_bottomContainer.parent.parent.gameObject.SetActive(false);
				}


			}
		}
		StartCoroutine(SetScrollSize(isGallery, isBigPicture));
		LayoutRebuilder.ForceRebuildLayoutImmediate(_title.GetComponent<RectTransform>());

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


	private void CheckForAspectRatios()
	{
		if (_image.GetComponent<AspectRatioFitter>().aspectRatio >= 1)
			_image.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
		else
			_image.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.FitInParent;
	}


	public void GoBackOneHierarchyLevel()
	{
		GetComponent<CanvasGroup>().blocksRaycasts = false;
		if (_heritageDetail.alpha == 0)
		{
			_instantiatedHierarchies.Remove(_currentHierarchy);

			var tmp = _currentHierarchy;  //jer se currenthierarhija promjeni dole u funkciji prije nego se lambda zove
			ShowCanvasGroup.Show(tmp.CanvasGroup, false, .5f, 0, Ease.OutQuad, () => {
				Destroy(tmp.CanvasGroup.gameObject);
			});
			_hierarchyIndex--;
			_themes.RemoveAt(_themes.Count - 1);
		}

		ShowCanvasGroup.Show(_heritageDetail, false, .5f, 0, Ease.OutQuad, () => {
			GetComponent<CanvasGroup>().blocksRaycasts = true;
			for (int i = 0; i < _listOfInstantiatedObjects.Count; i++)
			{
				Destroy(_listOfInstantiatedObjects[i]);
			}
			_listOfInstantiatedObjects.Clear();
		});



		if (_hierarchyIndex == -1)
		{
			_themes.Clear();
			GameEventMessage.SendEvent("Back");
			return;
		}
		_currentHierarchy = _instantiatedHierarchies[_hierarchyIndex];
		_scrollRect.content = _currentHierarchy.CanvasGroup.GetComponent<RectTransform>();
		_backButtonText.text = _currentHierarchy.Name;

		ShowCanvasGroup.Show(_currentHierarchy.CanvasGroup, true, .5f);
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

	public override void OnHideViewFinished()
	{
		base.OnHideViewFinished();
		ShowCanvasGroup.Show(_heritageDetail, false, .5f);
		DestroyHierarchies();

		for (int i = 0; i < _listOfInstantiatedObjects.Count; i++)
		{
			Destroy(_listOfInstantiatedObjects[i]);
		}
		_listOfInstantiatedObjects.Clear();
	}

	private void DestroyHierarchies()
	{
		for (int i = 0; i < _instantiatedHierarchies.Count; i++)
		{
			Destroy(_instantiatedHierarchies[i].CanvasGroup.gameObject);
		}
		_instantiatedHierarchies.Clear();
	}
	public override void OnHideViewStart()
	{
		base.OnHideViewStart();

		ShowCanvasGroup.Show(_fullscreenGallery.GetComponent<CanvasGroup>(), false);
		StartCoroutine(BlurFade(0));
	}
}


public class Hierarchy {
	public string Name { get; set; }
	public CanvasGroup CanvasGroup { get; set; }

}