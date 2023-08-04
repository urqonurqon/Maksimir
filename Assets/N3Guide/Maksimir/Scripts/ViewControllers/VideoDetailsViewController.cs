using DG.Tweening;
using Doozy.Engine.Nody;
using Doozy.Engine.UI;
using Novena.Components.AvProVideoPlayer;
using Novena.DAL;
using Novena.DAL.Model.Guide;
using Novena.UiUtility.Base;
using RenderHeads.Media.AVProVideo;
using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;
using Image = UnityEngine.UI.Image;

public class VideoDetailsViewController : UiController {
	public static Action OnMediaPlayerInstantiated;
	public static Action<string> OnHideSendTag;

	[SerializeField] private UIButton _fullscreenButton;
	[SerializeField] private UIButton _exitFullscreenButton;

	[SerializeField] private TMP_Text _title;
	[SerializeField] private TMP_Text _content;
	[SerializeField] private TMP_Text _backButtonText;

	[SerializeField] private GameObject _videoPrefab;
	[SerializeField] private RectTransform _videoParent;

	[SerializeField] private CanvasGroup _fullscreenVideo;
	[SerializeField] private Camera _360Camera;
	[SerializeField] private VlcPlayer _vlcPlayer;

	private GameObject _videoObject;
	private AvProVideoPlayer _videoPlayer;
	//private ApplyToMesh _360video;
	private RawImage _display360;
	private Theme _theme;

	public override void Awake()
	{
		base.Awake();
		//_360video = FindObjectOfType<ApplyToMesh>();
		//_360video.gameObject.SetActive(false);

		VideoButton.OnVideoClicked += SetupVideo;

		_fullscreenButton.OnClick.OnTrigger.Event.RemoveAllListeners();
		_fullscreenButton.OnClick.OnTrigger.Event.AddListener(() => {
			ShowCanvasGroup.Show(_fullscreenVideo, true, .5f);
			StartCoroutine(BlurFade(.5f));
			_videoObject.transform.SetParent(_fullscreenVideo.transform);
			_videoObject.transform.SetSiblingIndex(1);
			_videoObject.GetComponent<RectTransform>().DOSizeDelta(new Vector2(1028.746f, 578.67f), .5f);
			_videoObject.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 103.67f), .5f);
		});

		_exitFullscreenButton.OnClick.OnTrigger.Event.RemoveAllListeners();
		_exitFullscreenButton.OnClick.OnTrigger.Event.AddListener(() => {
			ShowCanvasGroup.Show(_fullscreenVideo, false, .5f);
			StartCoroutine(BlurFade(.5f));
			_videoObject.transform.SetParent(_videoParent);
			_videoObject.transform.SetAsFirstSibling();
			_videoObject.GetComponent<RectTransform>().DOSizeDelta(new Vector2(867.5f, 488f), .5f);
			_videoObject.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, .5f);
		});


		Data.OnTranslatedContentUpdated += () => {
			if (FindObjectOfType<GraphController>().Graph.ActiveNode.Name == "VideoDetailsView")
			{
				var videoParentTheme = Data.TranslatedContent.GetThemeByLanguageSwitchCode(3000);
				_theme = videoParentTheme.GetSubThemeByLanguageSwitchCode(_theme.LanguageSwitchCode);
				_backButtonText.text = _theme.Name;
				_title.text = _theme.Name;
				if (_theme.GetMediaByName("Text") != null)
					_content.text = _theme.GetMediaByName("Text").Text;
			}
		};
	}
	private void Start()
	{
		StartCoroutine(InstantiateVideo());

	}
	private IEnumerator InstantiateVideo()
	{
		yield return new WaitForSeconds(2);
		if (_videoObject != null)
			Destroy(_videoObject);
		_videoObject = Instantiate(_videoPrefab, _videoParent);
		_videoPlayer = _videoObject.GetComponentInChildren<AvProVideoPlayer>();
		_videoObject.transform.SetAsFirstSibling();
		OnMediaPlayerInstantiated?.Invoke();
		_display360 = _videoPlayer.GetComponentInChildren<RawImage>();
		//_display360.texture = Resources.Load<RenderTexture>("3DRender");
		//_display360.GetComponent<SimpleRotateSphere>().Camera360 = _360Camera.transform;
		_vlcPlayer = _videoObject.GetComponentInChildren<VlcPlayer>();

	}

	public override void OnShowViewStart()
	{
		base.OnShowViewStart();

		var scrollViews = GetComponentsInChildren<ScrollRect>().ToList();
		scrollViews.ForEach(s => s.normalizedPosition = new Vector2(0, 1));

	}


	private IEnumerator BlurFade(float time)
	{
		Material material = _fullscreenVideo.transform.GetChild(0).GetComponent<Image>().material;
		while (time > 0)
		{
			yield return null;
			time -= Time.deltaTime;
			material.SetFloat("_Alpha", _fullscreenVideo.alpha);
		}
	}

	public void SetupVideo(Theme theme)
	{
		_theme = theme;

		if (theme.GetThemeTagByCategoryName("VIDEO").Title == "Tag4")
		{
			//_360video.gameObject.SetActive(true);
			//_360video.Player = _videoPlayer.GetPlayer();
			_vlcPlayer.gameObject.SetActive(true);
			_vlcPlayer.LoadVideo(theme.GetMediaByName("Video").FullLocalPath);
			//_vlcPlayer.Play();
			_display360.gameObject.SetActive(true);
		}
		else
		{
			_vlcPlayer.UnloadPlayer();
			_vlcPlayer.gameObject.SetActive(false);
			_display360.gameObject.SetActive(false);
			//_360video.gameObject.SetActive(false);
			_videoPlayer.LoadVideo(theme.GetMediaByName("Video").FullLocalPath);
			_videoPlayer.PlayVideo();
		}

		_backButtonText.text = theme.Name;
		_title.text = theme.Name;
		if (theme.GetMediaByName("Text") != null)
			_content.text = theme.GetMediaByName("Text").Text;
	}

	public override void OnHideViewStart()
	{
		base.OnHideViewStart();
		_videoPlayer.ResetPlayer();
		_vlcPlayer.UnloadPlayer();
		ShowCanvasGroup.Show(_fullscreenVideo, false);
		StartCoroutine(BlurFade(0));
		_videoObject.transform.SetParent(_videoParent);
		_videoObject.transform.SetAsFirstSibling();
		_videoObject.GetComponent<RectTransform>().DOSizeDelta(new Vector2(818, 488f), .5f);
		_videoObject.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, .5f);
		if (_theme != null)
			OnHideSendTag?.Invoke(_theme.GetThemeTagByCategoryName("VIDEO").Title);
	}


}
