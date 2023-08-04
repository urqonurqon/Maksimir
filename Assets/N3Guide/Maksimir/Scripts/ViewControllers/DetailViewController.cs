using Cysharp.Threading.Tasks;
using Doozy.Engine;
using Doozy.Engine.Nody;
using Doozy.Engine.UI;
using Novena.DAL;
using Novena.UiUtility.Base;
using Scripts.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;
using Image = UnityEngine.UI.Image;

public class DetailViewController : UiController {

	[SerializeField] private RectTransform _topImageParent;
	[SerializeField] private GameObject _topImagePrefab;
	[SerializeField] private RawImage _qr;
	[SerializeField] private GameObject _whenWherePrefab;
	[SerializeField] private TMP_Text _title;
	[SerializeField] private TMP_Text _content;
	[SerializeField] private TMP_Text _dates;

	[SerializeField] private GameObject _bottomImagePrefab;
	[SerializeField] private Transform _bottomContainer;

	private List<ScrollRect> _scrollViews;

	private List<GameObject> _listOfInstantiatedObjects = new List<GameObject>();
	private List<string> _listOfPhotos = new List<string>();

	[SerializeField] private GallerySnap _fullscreenGallery;
	[SerializeField] private UIButton _fullscreenButton;
	[SerializeField] private UIButton _fullscreenCloseButton;

	[SerializeField] private CanvasGroup _body;
	[SerializeField] private CanvasGroup _loader;


	private Coroutine _loadingRoutine;

	public override void Awake()
	{
		base.Awake();
		NewsViewController.OnNewsButtonClick += async (n) => await Setup(n);

		_fullscreenButton.OnClick.OnTrigger.Event.RemoveAllListeners();
		_fullscreenButton.OnClick.OnTrigger.Event.AddListener(() => {
			ShowCanvasGroup.Show(_fullscreenGallery.GetComponent<CanvasGroup>(), true, .5f);
			_fullscreenGallery.Setup(_listOfPhotos);
			StartCoroutine(BlurFade(.5f));
		});

		_fullscreenCloseButton.OnClick.OnTrigger.Event.AddListener(() => {
			if (FindObjectOfType<GraphController>().Graph.ActiveNode.Name == "DetailsView")
			{
				ShowCanvasGroup.Show(_fullscreenGallery.GetComponent<CanvasGroup>(), false, .5f,0,DG.Tweening.Ease.OutQuad,()=> _fullscreenGallery.ResetGallery());
				StartCoroutine(BlurFade(.5f));
			}
		});


		Data.OnTranslatedContentUpdated += () => {
			if (FindObjectOfType<GraphController>().Graph.ActiveNode.Name == "DetailsView")
			{
				GameEventMessage.SendEvent("Back");
			}
		};
	}

	private IEnumerator LoaderTurning()
	{
		while (true)
		{
			_loader.transform.Rotate(Vector3.back);
			yield return null;
		}
	}

	public override void OnShowViewStart()
	{
		base.OnShowViewStart();

		_loadingRoutine = StartCoroutine(LoaderTurning());
	}


	private async UniTask Setup(News news)
	{
		_scrollViews = GetComponentsInChildren<ScrollRect>().ToList();
		_scrollViews.ForEach(s => s.normalizedPosition = new Vector2(0, 1));
		ShowCanvasGroup.Show(_body, false);
		ShowCanvasGroup.Show(_loader, true);
		_listOfPhotos.Add(news.ImagePath);

		_topImageParent.gameObject.SetActive(false);
		_title.text = news.Title.ReplaceHTMLTags();
		_content.text = news.Content.ReplaceHTMLTags();

		bool isWhenWhereActive = false;
		if (news.Dates != "")
		{
			if (news.Dates.Contains("\r\n\r\n"))
			{
				_dates.transform.parent.parent.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(_dates.transform.parent.parent.parent.GetComponent<RectTransform>().sizeDelta.x, 310);
			}
			else
			{
				_dates.transform.parent.parent.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(_dates.transform.parent.parent.parent.GetComponent<RectTransform>().sizeDelta.x, 200);

			}
			_dates.transform.parent.parent.parent.gameObject.SetActive(true);
			_dates.text = news.Dates.ReplaceHTMLTags();
			_title.rectTransform.sizeDelta = new Vector2(523.04f, _title.rectTransform.sizeDelta.y);
			_content.rectTransform.sizeDelta = new Vector2(523.04f, _title.rectTransform.sizeDelta.y);
		}
		else
		{
			_dates.transform.parent.parent.parent.gameObject.SetActive(false);
			_title.rectTransform.sizeDelta = new Vector2(829.67f, _title.rectTransform.sizeDelta.y);
			_content.rectTransform.sizeDelta = new Vector2(829.67f, _title.rectTransform.sizeDelta.y);
		}

		if (news.ProgramSchedule != "")
		{
			var whenWhere = Instantiate(_whenWherePrefab, _bottomContainer);
			_listOfInstantiatedObjects.Add(whenWhere);
			whenWhere.GetComponentInChildren<TMP_Text>().text = news.ProgramSchedule.ReplaceHTMLTags();
			isWhenWhereActive = true;
		}

		if (news.ImagePath != "http://maksimir.s13.novenaweb.info")
		{
			var bigPhoto = Instantiate(_topImagePrefab, _topImageParent);
			bigPhoto.transform.SetAsFirstSibling();
			_listOfInstantiatedObjects.Add(bigPhoto);
			var tex = await AssetsFileLoader.LoadTextureAsync(news.ImagePath, 5, true);
			if (tex != null)
			{
				bigPhoto.GetComponentInChildren<RawImage>().texture = tex;
				bigPhoto.GetComponentInChildren<RawImage>().GetComponent<AspectRatioFitter>().aspectRatio = (float)tex.width / tex.height;
				_topImageParent.gameObject.SetActive(true);
			}
			else
				_topImageParent.gameObject.SetActive(false);

		}

		//this is sent from CMS if there is no qr code, basically null check
		if (news.QrCode == "http://maksimir.s13.novenaweb.info")
		{
			_qr.transform.parent.gameObject.SetActive(false);
		}
		else
		{
			var tex = await AssetsFileLoader.LoadTextureAsync(news.QrCode, 5, true);
			if (tex != null)
			{
				_qr.texture = tex;
				_qr.transform.parent.gameObject.SetActive(true);
			}
			else
			{
				_qr.transform.parent.gameObject.SetActive(false);

			}
		}

		if (news.Photos != null)
		{
			_listOfPhotos.AddRange(news.Photos);
			var gallery = news.Photos;
			bool isAnyInstantiated = false;
			for (int i = 0; i < gallery.Count; i++)
			{
				var tex = await AssetsFileLoader.LoadTextureAsync(gallery[i], 5, true);
				if (tex != null)
				{
					isAnyInstantiated = true;
					var photo = Instantiate(_bottomImagePrefab, _bottomContainer);
					_listOfInstantiatedObjects.Add(photo);
					photo.GetComponentInChildren<RawImage>().texture = tex;
					photo.GetComponentInChildren<RawImage>().GetComponent<AspectRatioFitter>().aspectRatio = (float)tex.width / tex.height;
					int j = i;
					photo.GetComponent<UIButton>().OnClick.OnTrigger.Event.AddListener(() => {
						ShowCanvasGroup.Show(_fullscreenGallery.GetComponent<CanvasGroup>(), true, .5f);
						_fullscreenGallery.Setup(_listOfPhotos, j + 1);
						StartCoroutine(BlurFade(.5f));
					});
				}
			}
			if (isAnyInstantiated || isWhenWhereActive)
			{
				_bottomContainer.parent.parent.gameObject.SetActive(true);
				if (gallery.Count > (isWhenWhereActive ? 2 : 3))
				{
					//_bottomContainer.GetComponent<ContentSizeFitter>().enabled = true;
					_bottomContainer.parent.parent.GetChild(0).gameObject.SetActive(true);
					_bottomContainer.parent.parent.GetChild(1).gameObject.SetActive(true);
				}
				else
				{
					//_bottomContainer.GetComponent<ContentSizeFitter>().enabled = false;
					_bottomContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(_bottomContainer.parent.GetComponent<RectTransform>().rect.width, _bottomContainer.GetComponent<RectTransform>().sizeDelta.y);

					_bottomContainer.parent.parent.GetChild(0).gameObject.SetActive(false);
					_bottomContainer.parent.parent.GetChild(1).gameObject.SetActive(false);
				}
			}
			else
			{
				_bottomContainer.parent.parent.gameObject.SetActive(false);
			}
		}
		else
		{
			if (isWhenWhereActive)
				_bottomContainer.parent.parent.gameObject.SetActive(true);
			else
				_bottomContainer.parent.parent.gameObject.SetActive(false);

		}

		StartCoroutine(SetScrollSize(_bottomContainer.parent.parent.gameObject.activeSelf, _topImageParent.gameObject.activeSelf));
		StartCoroutine(RebuildLayoutEndOfFrame());
		ShowCanvasGroup.Show(_body, true, .5f);
		ShowCanvasGroup.Show(_loader, false, .5f);
		if (_loadingRoutine != null)
			StopCoroutine(_loadingRoutine);

	}

	private IEnumerator RebuildLayoutEndOfFrame()
	{
		yield return new WaitForEndOfFrame();
		LayoutRebuilder.MarkLayoutForRebuild(_title.rectTransform);
		LayoutRebuilder.MarkLayoutForRebuild(_content.rectTransform);
		LayoutRebuilder.MarkLayoutForRebuild(_scrollViews[0].GetComponent<RectTransform>());

	}

	private IEnumerator SetScrollSize(bool isGallery, bool isBigPicture)
	{
		yield return new WaitForEndOfFrame();
		if (!isGallery)
			_content.transform.parent.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(
				_content.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x, _content.rectTransform.sizeDelta.y + _title.rectTransform.sizeDelta.y + 40f);
		else
			_content.transform.parent.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(
				_content.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x, _content.rectTransform.sizeDelta.y + _title.rectTransform.sizeDelta.y + 40f +
				 _bottomContainer.parent.parent.GetComponent<RectTransform>().sizeDelta.y);

		if (isBigPicture)
			//{
			//	_content.transform.parent.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(_content.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x,
			//		_content.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.y < 660.57f ? 660.57f : _content.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.y);
			_content.transform.parent.parent.parent.GetComponent<RectTransform>().offsetMin = new Vector2(_content.transform.parent.parent.parent.GetComponent<RectTransform>().offsetMin.x, 0);
		//}
		else
			//{
			//	_content.transform.parent.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(_content.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x,
			//		_content.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.y < 1181.2f ? 1181.2f : _content.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.y);
			_content.transform.parent.parent.parent.GetComponent<RectTransform>().offsetMin = new Vector2(_content.transform.parent.parent.parent.GetComponent<RectTransform>().offsetMin.x, -520.86f);
		//}
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

		for (int i = 0; i < _listOfInstantiatedObjects.Count; i++)
		{
			Destroy(_listOfInstantiatedObjects[i]);
		}
		_listOfInstantiatedObjects.Clear();
		_listOfPhotos.Clear();
		_fullscreenGallery.ResetGallery();

	}
	public override void OnHideViewStart()
	{
		base.OnHideViewStart();

		ShowCanvasGroup.Show(_fullscreenGallery.GetComponent<CanvasGroup>(), false);
		StartCoroutine(BlurFade(0));
	}
}
