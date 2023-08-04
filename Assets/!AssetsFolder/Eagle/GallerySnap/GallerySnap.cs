using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Novena.DAL.Model.Guide;
using Image = UnityEngine.UI.Image;
using System.Collections;
using Cysharp.Threading.Tasks;
using Doozy.Engine.UI;
using Coffee.UISoftMask;
using Scripts.Utility;
using System;

public class GallerySnap : MonoBehaviour {
	[SerializeField] public GalleryImage[] _objectsInList;
	[SerializeField] protected float _transitionSpeed;
	public float _cycleSpeed;
	[SerializeField] private bool _useDotIndicators;
	[SerializeField] protected bool _useAutoSlideShow;
	[SerializeField] private AutoSlideShow _autoSlideShowGO;
	[SerializeField] private GameObject _dotIndicatorPrefab;
	[SerializeField] private Transform _dotsContainer;
	[SerializeField] private Color _activeDotColor;
	[SerializeField] private Color _inactiveDotColor;
	[SerializeField] private GameObject _gestureListenerVertical;
	[SerializeField] protected GameObject _gestureListenerHorizontal;
	[SerializeField] protected GameObject _nextButton;
	[SerializeField] protected GameObject _previousButton;

	private int _selectedObjectIndex;
	protected List<string> _photoPaths = new List<string>();
	private List<Photo> _photos = new List<Photo>();
	private List<Image> _dotList = new List<Image>();
	private Image _lastDot;
	protected bool _useVerticalGallery;
	public int TextureIndex;
	protected float _distance;
	private float _declaredCycleSpeed;

	protected GalleryImageLoader _galleryImageLoader;

	private void Awake()
	{
		_galleryImageLoader = new GalleryImageLoader();
	}

	private void Start()
	{
		_declaredCycleSpeed = _cycleSpeed;

		_distance = Vector2.Distance(_objectsInList[0].Rect.anchoredPosition, _objectsInList[1].Rect.anchoredPosition);

		GetComponent<AutoSlideShow>().enabled = false;
	}
	private bool isFirst = true;
	public void Setup(List<string> photoPaths, int index = 0)
	{
		if (isFirst)
		{
			var o1 = _objectsInList[0].Rect.gameObject.AddComponent<SoftMask>();
			var o2 = _objectsInList[1].Rect.gameObject.AddComponent<SoftMask>();
			_objectsInList[0].PhotoRawImage.gameObject.AddComponent<SoftMaskable>();
			_objectsInList[1].PhotoRawImage.gameObject.AddComponent<SoftMaskable>();
			o1.showMaskGraphic = false;
			o2.showMaskGraphic = false;
			isFirst = false;
		}
		_photoPaths = new List<string>(photoPaths);
		Init(index).Forget();
	}

	public void Setup(List<Photo> photos, int index = 0)
	{
		_photos = new List<Photo>(photos);
		_photos.ForEach((p) => _photoPaths.Add(p.FullPath));
		Init(index).Forget();
	}

	public virtual async UniTaskVoid Init(int index = 0)
	{
		AutoSlideShow.OnCycleEndEvent += NextStep;
		GenerateDots();
		TextureIndex = index;
		_objectsInList[0].IsSelected = true;

		_objectsInList[0].Rect.anchoredPosition = Vector2.zero;

		if (_useVerticalGallery)
			_objectsInList[1].Rect.anchoredPosition = Vector2.up * _distance;
		else
			_objectsInList[1].Rect.anchoredPosition = Vector2.left * _distance;

		//Let's load first image!
		_objectsInList[0].PhotoRawImage.texture = await AssetsFileLoader.LoadTextureAsync(_photoPaths[index], 5, true);

		CheckForAspectRatios();

		_objectsInList[1].IsSelected = false;

		if (_useAutoSlideShow)
		{
			ResetTimer();
			EnableAutoSlideShow(true);
		}

		if (_photoPaths.Count <= 1)
		{
			_gestureListenerHorizontal.SetActive(false);
			_nextButton.SetActive(false);
			_previousButton.SetActive(false);
		}
		else
		{
			_gestureListenerHorizontal.SetActive(true);
			_nextButton.SetActive(true);
			_previousButton.SetActive(true);
		}
	}

	public void ResetGallery()
	{
		AutoSlideShow.OnCycleEndEvent -= NextStep;
		ClearDots();
		ResetTimer();
		EnableAutoSlideShow(false);
		_photoPaths.Clear();
		_galleryImageLoader.Dispose();
		Resources.UnloadUnusedAssets();
	}

	public void ResetTimer()
	{
		_autoSlideShowGO._timeRemaining = _declaredCycleSpeed;
	}

	public void EnableAutoSlideShow(bool enable)
	{
		GetComponent<AutoSlideShow>().enabled = enable;
	}

	public void SetGalleryVertical()
	{
		_useVerticalGallery = true;
		_gestureListenerHorizontal.gameObject.SetActive(false);
		_gestureListenerVertical.gameObject.SetActive(true);
		var pos = _objectsInList[0].Rect.anchoredPosition;
		pos.y -= _objectsInList[0].Rect.rect.height;
		_objectsInList[1].Rect.anchoredPosition = pos;
	}

	public void SetGalleryHorizontal()
	{
		_useVerticalGallery = false;
		_gestureListenerVertical.gameObject.SetActive(false);
		_gestureListenerHorizontal.gameObject.SetActive(true);
		var pos = _objectsInList[0].Rect.anchoredPosition;
		pos.x -= _objectsInList[0].Rect.rect.width;
		_objectsInList[1].Rect.anchoredPosition = pos;
	}

	private void SwitchPoint(int index)
	{
		if (!_useDotIndicators) return;
		//Refactor later!!
		try
		{
			_dotList[index].GetComponent<Image>().DOColor(_activeDotColor, 0);
			_lastDot.DOColor(_inactiveDotColor, 0);
			_lastDot = _dotList[index];
		}
		catch (System.Exception e)
		{
			Debug.LogException(e);
			throw;
		}
	}

	protected void GenerateDots()
	{
		if (!_useDotIndicators) return;
		_dotList = new List<Image>();
		for (int i = 0; i < _photoPaths.Count; i++)
		{
			var dot = Instantiate(_dotIndicatorPrefab, _dotsContainer);
			_dotList.Add(dot.GetComponent<Image>());
		}
		SwitchPoint(0);
	}

	private void ClearDots()
	{
		if (_dotList != null)
			_dotList.Clear();
		foreach (Transform child in _dotsContainer.transform)
		{
			Destroy(child.gameObject);
		}
	}

	public void NextStep()
	{
		_selectedObjectIndex = Mathf.Clamp(_selectedObjectIndex - 1, 0, _objectsInList.Length - 1);
		ResetTimer();
		ChangePosition(true).Forget();
		SwitchPoint(TextureIndex);
	}

	public void PrevStep()
	{
		_selectedObjectIndex = Mathf.Clamp(_selectedObjectIndex + 1, 0, _objectsInList.Length - 1);
		ResetTimer();
		ChangePosition(false).Forget();
		SwitchPoint(TextureIndex);
	}


	/// <summary>
	/// Slide images up or down
	/// </summary>
	/// <param name="isUp"></param>
	/// <returns></returns>
	private async UniTaskVoid ChangePosition(bool isUp)
	{
		if (isUp)
		{
			if (_objectsInList[0].IsSelected)
			{
				_objectsInList[1].PhotoRawImage.texture = await AssetsFileLoader.LoadTextureAsync(_photoPaths[SetIndex(isUp)], 5, true);
				//await _galleryImageLoader.LoadImageVariation(_objectsInList[1].PhotoRawImage, _photoPaths[SetIndex(isUp)]);
				_objectsInList[1].IsSelected = true;
				_objectsInList[0].IsSelected = false;

				if (_useVerticalGallery)
				{
					_objectsInList[1].Rect.anchoredPosition = Vector2.up * -_distance;
					MoveUp(true);
				}
				else
				{
					_objectsInList[1].Rect.anchoredPosition = Vector2.left * _distance;
					MoveRight(true);
				}
			}
			else
			{

				_objectsInList[0].PhotoRawImage.texture = await AssetsFileLoader.LoadTextureAsync(_photoPaths[SetIndex(isUp)], 5, true);
				//await _galleryImageLoader.LoadImageVariation(_objectsInList[0].PhotoRawImage, _photoPaths[SetIndex(isUp)]);
				_objectsInList[0].IsSelected = true;
				_objectsInList[1].IsSelected = false;

				if (_useVerticalGallery)
				{
					_objectsInList[0].Rect.anchoredPosition = Vector2.up * -_distance;
					MoveUp(true);
				}
				else
				{
					_objectsInList[0].Rect.anchoredPosition = Vector2.left * _distance;
					MoveRight(true);
				}

			}
		}
		else
		{
			if (_objectsInList[0].IsSelected)
			{
				_objectsInList[1].PhotoRawImage.texture = await AssetsFileLoader.LoadTextureAsync(_photoPaths[SetIndex(isUp)], 5, true);
				//await _galleryImageLoader.LoadImageVariation(_objectsInList[1].PhotoRawImage, _photoPaths[SetIndex(isUp)]);
				_objectsInList[1].IsSelected = true;
				_objectsInList[0].IsSelected = false;

				if (_useVerticalGallery)
				{
					_objectsInList[1].Rect.anchoredPosition = Vector2.up * _distance;
					MoveUp(false);
				}
				else
				{
					_objectsInList[1].Rect.anchoredPosition = Vector2.right * _distance;
					MoveRight(false);
				}
			}
			else
			{
				_objectsInList[0].PhotoRawImage.texture = await AssetsFileLoader.LoadTextureAsync(_photoPaths[SetIndex(isUp)], 5, true);
				//await _galleryImageLoader.LoadImageVariation(_objectsInList[0].PhotoRawImage, _photoPaths[SetIndex(isUp)]);
				_objectsInList[0].IsSelected = true;
				_objectsInList[1].IsSelected = false;

				if (_useVerticalGallery)
				{
					_objectsInList[0].Rect.anchoredPosition = Vector2.up * _distance;
					MoveUp(false);
				}
				else
				{
					_objectsInList[0].Rect.anchoredPosition = Vector2.right * _distance;
					MoveRight(false);
				}
			}

		}
		CheckForAspectRatios();
	}

	private int SetIndex(bool isUp)
	{
		if (isUp)
			if (TextureIndex + 1 == _photoPaths.Count)
				TextureIndex = 0;
			else
				TextureIndex++;
		else
				if (TextureIndex - 1 == -1)
			TextureIndex = _photoPaths.Count - 1;
		else
			TextureIndex--;
		return TextureIndex;
	}

	private void MoveUp(bool isUp)
	{
		_gestureListenerVertical.SetActive(false);
		_nextButton.GetComponent<UIButton>().Interactable = false;
		_previousButton.GetComponent<UIButton>().Interactable = false;

		for (int i = 0; i < _objectsInList.Length; i++)
		{
			_objectsInList[i].Rect.DOAnchorPosY(_objectsInList[i].Rect.anchoredPosition.y + (isUp ? _distance : _distance * -1), _transitionSpeed).OnComplete(() => {
				_gestureListenerVertical.SetActive(true);
				_nextButton.GetComponent<UIButton>().Interactable = true;
				_previousButton.GetComponent<UIButton>().Interactable = true;
			});
		}
	}

	public virtual void MoveRight(bool isRight)
	{
		_gestureListenerHorizontal.SetActive(false);
		_nextButton.GetComponent<Button>().interactable = false;
		_previousButton.GetComponent<Button>().interactable = false;

		for (int i = 0; i < _objectsInList.Length; i++)
		{
			_objectsInList[i].Rect.DOAnchorPosX(_objectsInList[i].Rect.anchoredPosition.x + (isRight ? _distance : _distance * -1), _transitionSpeed).OnComplete(() => {
				_gestureListenerHorizontal.SetActive(true);
				_nextButton.GetComponent<Button>().interactable = true;
				_previousButton.GetComponent<Button>().interactable = true;
			});
		}
	}

	protected IEnumerator OnOffRawImage()
	{
		while (true)
		{
			_objectsInList[0].PhotoRawImage.enabled = false;
			_objectsInList[1].PhotoRawImage.enabled = false;
			_objectsInList[0].PhotoRawImage.enabled = true;
			_objectsInList[1].PhotoRawImage.enabled = true;
			yield return null;
		}
	}
	protected void CheckForAspectRatios()
	{
		if (_objectsInList[0].PhotoRawImage.texture != null)
			_objectsInList[0].PhotoRawImage.GetComponent<AspectRatioFitter>().aspectRatio = _objectsInList[0].PhotoRawImage.texture.width / (float)_objectsInList[0].PhotoRawImage.texture.height;
		if (_objectsInList[1].PhotoRawImage.texture != null)
			_objectsInList[1].PhotoRawImage.GetComponent<AspectRatioFitter>().aspectRatio = _objectsInList[1].PhotoRawImage.texture.width / (float)_objectsInList[1].PhotoRawImage.texture.height;

		if (_objectsInList[0].PhotoRawImage.GetComponent<AspectRatioFitter>().aspectRatio >= 1)
			_objectsInList[0].PhotoRawImage.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
		else
			_objectsInList[0].PhotoRawImage.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.FitInParent;

		if (_objectsInList[1].PhotoRawImage.GetComponent<AspectRatioFitter>().aspectRatio >= 1)
			_objectsInList[1].PhotoRawImage.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
		else
			_objectsInList[1].PhotoRawImage.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.FitInParent;
	}

	[System.Serializable]
	public class GalleryImage {
		public bool IsSelected;
		public RectTransform Rect;
		public RawImage PhotoRawImage;
	}
}
