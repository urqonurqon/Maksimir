using DG.Tweening;
using Doozy.Engine.Nody;
using Doozy.Engine.UI;
using Novena.DAL.Model.Guide;
using Novena.Networking;
using Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

public class MenuButtonExtend : MenuButton {

	private Animator _animator;
	[SerializeField] private RuntimeAnimatorController _nonSocialController;
	[SerializeField] private RuntimeAnimatorController _socialController;

	[SerializeField] private List<RawImage> _imageList = new List<RawImage>();
	[SerializeField] private List<RawImage> _qrList = new List<RawImage>();
	[SerializeField] private TMP_Text _extendedText;
	[SerializeField] private CanvasGroup _wifiCG;

	[SerializeField] private List<GameObject> _wifiStates = new List<GameObject>();


	private void Awake()
	{
		_animator = GetComponent<Animator>();
	}
	public override void SetButton(Theme theme)
	{
		_title.text = theme.Name;

		if (theme.GetMediaByName("Text1") != null)
			_extendedText.text = theme.GetMediaByName("Text1").Text;

		if (theme.ImagePath is not null)
		{
			if (theme.Name.Contains("wi-fi",System.StringComparison.OrdinalIgnoreCase))
			{
				ShowCanvasGroup.Show(_wifiCG, true);
				ShowCanvasGroup.Show(_rawImage.GetComponent<CanvasGroup>(), false);
				StartCoroutine(WifiAnimation());
			}
			else
			{
				ShowCanvasGroup.Show(_wifiCG, false);
				ShowCanvasGroup.Show(_rawImage.GetComponent<CanvasGroup>(), true);
				AssetsFileLoader.LoadTexture2D(Api.GetFullLocalPath(theme.ImagePath), _rawImage);
			//_rawImage.SetNativeSize();
			}
			_animator.runtimeAnimatorController = _nonSocialController;
			_imageList.ForEach((img) => img.DOFade(0, 0));


		}
		else
		{
			ShowCanvasGroup.Show(_wifiCG, false);
			ShowCanvasGroup.Show(_rawImage.GetComponent<CanvasGroup>(), true);
			_animator.runtimeAnimatorController = _socialController;
			_rawImage.DOFade(0, 0);
			_imageList.ForEach((img) => img.DOFade(1, 0));
			var qrs = theme.GetMediaByName("QR").GetPhotos();
			var icons = theme.GetMediaByName("Icons").GetPhotos();
			for (int i = 0; i < icons.Count; i++)
			{
				AssetsFileLoader.LoadTexture2D(icons[i].FullPath, _imageList[i]);
				AssetsFileLoader.LoadTexture2D(qrs[i].FullPath, _qrList[i]);
			}
		}



		UIButton btn = gameObject.GetComponent<UIButton>();
		btn.OnClick.OnTrigger.Event.AddListener(() => {
			_animator.SetTrigger("Start");
			_animator.SetBool("ButtonClicked", !_animator.GetBool("ButtonClicked"));
		});

	}


	private IEnumerator WifiAnimation()
	{
		while (FindObjectOfType<GraphController>().Graph.ActiveNode.Name != "LanguageView")
		{
			for (int i = 0; i < _wifiStates.Count; i++)
			{
				_wifiStates.ForEach((s) => s.SetActive(false));
				_wifiStates[i].SetActive(true);
				yield return new WaitForSeconds(.75f);
			}
			yield return null;
		}
	}
}

