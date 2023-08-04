using DG.Tweening;
using Doozy.Engine.Nody;
using Doozy.Engine.UI;
using Novena.DAL;
using Novena.UiUtility.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VideoViewController : UiController {

	[SerializeField] private UIToggle[] _videoCategories;

	[SerializeField] private TMP_FontAsset _bold;
	[SerializeField] private TMP_FontAsset _regular;

	[SerializeField] private GameObject _videoPrefab;
	[SerializeField] private Transform _videosContainer;
	[SerializeField] private TMP_Text _backButtonText;
	[SerializeField] private TMP_Text _comingSoon;

	private List<GameObject> _instantiatedList = new List<GameObject>();



	public override void Awake()
	{
		base.Awake();

		VideoDetailsViewController.OnHideSendTag += OnDetailsHide;

		for (int i = 0; i < _videoCategories.Length; i++)
		{
			int j = i;
			_videoCategories[i].OnValueChanged.RemoveAllListeners();
			_videoCategories[i].OnValueChanged.AddListener((isOn) => {
				DestroyAllVideos();
				OnVideoCategorieClick(_videoCategories[j], isOn);
			});
		}


		Data.OnTranslatedContentUpdated += () => {
			if (FindObjectOfType<GraphController>().Graph.ActiveNode.Name == "VideoView")
			{
				Data.Theme = Data.TranslatedContent.GetThemeByLanguageSwitchCode(Data.Theme.LanguageSwitchCode);
				_backButtonText.text = Data.Theme.Name;
				string tagIsOn = "";
				for (int j = 0; j < _videoCategories.Length; j++)
				{
					_videoCategories[j].GetComponentInChildren<TMP_Text>().text = Data.Theme.GetMediaByName("Text" + (j + 1).ToString()).Text;

					if (_videoCategories[j].IsOn)
						tagIsOn = _videoCategories[j].name;
				}
				var instantiatedVideoThemes = Data.Theme.GetSubThemesByTag(tagIsOn);

				for (int i = 0; i < _instantiatedList.Count; i++)
				{
					var prefabTitle = _instantiatedList[i].GetComponent<VideoButton>().GetTitle();
					var prefabText = _instantiatedList[i].GetComponent<VideoButton>().GetText();

					prefabTitle.text = instantiatedVideoThemes[i].Name;
					//if (instantiatedVideoThemes[i].GetMediaByName("Text") != null)
					//	prefabText.text = instantiatedVideoThemes[i].GetMediaByName("Text").Text;
				}
			}
		};
	}

	private void OnDetailsHide(string tagTitle)
	{
		for (int i = 0; i < _videoCategories.Length; i++)
		{
			if (_videoCategories[i].name == tagTitle)
			{
				_videoCategories[i].IsOn = false;
				_videoCategories[i].IsOn = true;
			}
		}
	}

	private void OnVideoCategorieClick(UIToggle uIToggle, bool isOn)
	{
		TMP_Text toggleText = uIToggle.GetComponentInChildren<TMP_Text>();
		if (isOn)
		{
			ShowVideos(uIToggle);

			toggleText.DOColor(new Color(0.6f, 0.7686275f, 0.3960784f), 0.5f);
			StartCoroutine(GradualFontChange(toggleText, 28));
			toggleText.font = _bold;
		}
		else
		{
			toggleText.DOColor(Color.white, 0.5f);
			StartCoroutine(GradualFontChange(toggleText, 24));
			toggleText.font = _regular;
		}
	}

	public override void OnShowViewStart()
	{
		base.OnShowViewStart();
		//ShowVideosTest();
		var scrollViews = GetComponentsInChildren<ScrollRect>().ToList();
		scrollViews.ForEach(s => s.normalizedPosition = new Vector2(0, 1));
		_backButtonText.text = Data.Theme.Name;
		for (int j = 0; j < _videoCategories.Length; j++)
		{
			_videoCategories[j].GetComponentInChildren<TMP_Text>().text = Data.Theme.GetMediaByName("Text" + (j + 1).ToString()).Text;

		}
		if (FindObjectOfType<GraphController>().Graph.PreviousActiveNode.Name != "VideoDetailsView")
		{
			_videoCategories[0].IsOn = false;
			_videoCategories[0].IsOn = true;

		}
	}

	//private void ShowVideosTest()
	//{
	//	var videoThemes = Data.Theme.SubThemes;

	//	for (int i = 0; i < videoThemes.Length; i++)
	//	{

	//		var videoButton = Instantiate(_videoPrefab, _videosContainer);
	//		videoButton.GetComponent<VideoButton>().Setup(videoThemes[i]).Forget();
	//		_instantiatedList.Add(videoButton);

	//	}
	//}

	private void ShowVideos(UIToggle uIToggle)
	{
		DestroyAllVideos();
		if (Data.Theme == null) return;
		var videoThemes = Data.Theme.SubThemes;

		for (int i = 0; i < videoThemes.Length; i++)
		{
			if (videoThemes[i].GetThemeTagByCategoryName("VIDEO") != null)
				if (videoThemes[i].GetThemeTagByCategoryName("VIDEO").Title == uIToggle.name)
				{
					var videoButton = Instantiate(_videoPrefab, _videosContainer);
					videoButton.GetComponent<VideoButton>().Setup(videoThemes[i]).Forget();
					_instantiatedList.Add(videoButton);
				}
		}
		if (_instantiatedList.Count > 0)
		{
			_comingSoon.transform.parent.gameObject.SetActive(false);
		}
		else
		{
			_comingSoon.transform.parent.gameObject.SetActive(true);
		}
	}
	private void DestroyAllVideos()
	{
		for (int i = 0; i < _instantiatedList.Count; i++)
		{
			Destroy(_instantiatedList[i]);
		}
		_instantiatedList.Clear();
		Resources.UnloadUnusedAssets();
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

	public override void OnHideViewFinished()
	{
		base.OnHideViewFinished();
		DestroyAllVideos();
	}
}
