using Coffee.UISoftMask;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Scripts.Utility;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GallerySnapVariation : GallerySnap {



	public override void MoveRight(bool isRight)
	{
		_gestureListenerHorizontal.SetActive(false);
		_nextButton.GetComponent<Button>().interactable = false;
		_previousButton.GetComponent<Button>().interactable = false;
		var routine = StartCoroutine(OnOffRawImage());
		for (int i = 0; i < _objectsInList.Length; i++)
		{
			_objectsInList[i].Rect.DOAnchorPosX(_objectsInList[i].Rect.anchoredPosition.x + (isRight ? _distance : _distance * -1), _transitionSpeed).OnComplete(() => {
				_gestureListenerHorizontal.SetActive(true);
				_nextButton.GetComponent<Button>().interactable = true;
				_previousButton.GetComponent<Button>().interactable = true;
				StopCoroutine(routine);
			});
		}

	}
	public new void Setup(List<string> photoPaths, int index)
	{

		_photoPaths = new List<string>(photoPaths);
		Init(index).Forget();
	}

	public override async UniTaskVoid Init(int index = 0)
	{
		CheckIfOneImage();

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
		_objectsInList[0].PhotoRawImage.texture = await AssetsFileLoader.LoadTextureAsync(_photoPaths[index],5,true);
		//_objectsInList[0].PhotoRawImage.GetComponent<AspectRatioFitter>().aspectRatio = _objectsInList[0].PhotoRawImage.texture.width / _objectsInList[0].PhotoRawImage.texture.height;

		_objectsInList[1].IsSelected = false;

		if (_useAutoSlideShow)
		{
			ResetTimer();
			EnableAutoSlideShow(true);
		}

		OnOffRawImageOnInit();
		CheckForAspectRatios();

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

	private void CheckIfOneImage()
	{
		if (_photoPaths.Count < 2)
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

	private void OnOffRawImageOnInit()
	{
		_objectsInList[0].PhotoRawImage.enabled = false;
		_objectsInList[1].PhotoRawImage.enabled = false;
		_objectsInList[0].PhotoRawImage.enabled = true;
		_objectsInList[1].PhotoRawImage.enabled = true;
	}



}
