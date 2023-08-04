using Cysharp.Threading.Tasks;
using Doozy.Engine.UI;
using Novena.DAL.Model.Guide;
using Novena.DAL;
using Novena.UiUtility.Base;
using Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utility;
using TMPro;
using Image = UnityEngine.UI.Image;
using Doozy.Engine.Nody;

public class InfoSmallViewController : UiController {

	[SerializeField] private TMP_Text _backButtonText;
	[SerializeField] private TMP_Text _title;
	[SerializeField] private TMP_Text _text;
	[SerializeField] private RawImage _image;
	[SerializeField] private Transform _bottomContainer;
	[SerializeField] private GameObject _bottomImagePrefab;

	[SerializeField] private GallerySnapVariation _fullscreenGallery;
	[SerializeField] private UIButton _fullscreenButton;
	[SerializeField] private UIButton _fullscreenCloseButton;


	private List<string> _listOfPhotos = new List<string>();
	private List<GameObject> _listOfInstantiatedObjects = new List<GameObject>();


	public override void Awake()
	{
		base.Awake();
		_fullscreenButton.OnClick.OnTrigger.Event.RemoveAllListeners();
		_fullscreenButton.OnClick.OnTrigger.Event.AddListener(() => {
			ShowCanvasGroup.Show(_fullscreenGallery.GetComponent<CanvasGroup>(), true, .5f);
			_fullscreenGallery.Setup(_listOfPhotos);
			StartCoroutine(BlurFade(.5f));
		});

		_fullscreenCloseButton.OnClick.OnTrigger.Event.AddListener(() => {
			if (FindObjectOfType<GraphController>().Graph.ActiveNode.Name == "InfoSmallView")
			{
				ShowCanvasGroup.Show(_fullscreenGallery.GetComponent<CanvasGroup>(), false, .5f, 0, DG.Tweening.Ease.OutQuad, () => _fullscreenGallery.ResetGallery());
				StartCoroutine(BlurFade(.5f));
			}
		});


		Data.OnTranslatedContentUpdated += () => {
			if (FindObjectOfType<GraphController>().Graph.ActiveNode.Name == "InfoSmallView")
			{
				Data.Theme = Data.TranslatedContent.GetThemeByLanguageSwitchCode(5062).GetSubThemeByLanguageSwitchCode(50621);
				_title.text = Data.Theme.GetMediaByName("TitletSmall").Text;
				_text.text = Data.Theme.GetMediaByName("TextSmall").Text;
				_backButtonText.text = Data.Theme.GetMediaByName("TitletSmall").Text;
			}
		};
	}


	public override void OnShowViewStart()
	{
		base.OnShowViewStart();
		SetThemeDetails(Data.Theme).Forget();
	}


	private async UniTask SetThemeDetails(Theme theme)
	{


		bool isGallery = false;
		bool isBigPicture = false;
		_backButtonText.text = theme.GetMediaByName("TitletSmall").Text;
		_image.transform.parent.gameObject.SetActive(false);
		ShowCanvasGroup.Show(_image.transform.parent.GetComponent<CanvasGroup>(), false);
		_title.text = theme.GetMediaByName("TitletSmall").Text;
		_text.text = theme.GetMediaByName("TextSmall").Text;

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

		//LayoutRebuilder.ForceRebuildLayoutImmediate(_details.GetComponent<RectTransform>());
		//StartCoroutine(FixVerticalGroupSpacing());
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

	public override void OnHideViewStart()
	{
		base.OnHideViewStart();
		_fullscreenGallery.ResetGallery();
		ShowCanvasGroup.Show(_fullscreenGallery.GetComponent<CanvasGroup>(), false);
		StartCoroutine(BlurFade(0));
	}
}
