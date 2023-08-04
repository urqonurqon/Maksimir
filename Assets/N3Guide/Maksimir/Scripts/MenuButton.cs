using Cysharp.Threading.Tasks;
using DG.Tweening;
using Doozy.Engine.Nody;
using Doozy.Engine.UI;
using Novena.DAL;
using Novena.DAL.Model.Guide;
using Novena.Networking;
using Scripts.Utility;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class MenuButton : MonoBehaviour {

	public static Action OnMenuButtonClicked;

	[Header("Components")]
	[SerializeField] protected TMP_Text _title;
	[SerializeField] protected Image _border;


	[SerializeField] protected RawImage _rawImage;


	[HideInInspector] public UIToggle ColorToggle;

	private void Awake()
	{
		NewsViewController.OnShowView += () => { if (ColorToggle != null) ColorToggle.IsOn = false; };
		ColorToggle = GetComponent<UIToggle>();
		ColorToggle.OnClick.OnToggleOff.Event.AddListener(() => _border.DOFade(0, 0f));
		ColorToggle.OnValueChanged.AddListener((isOn) => _border.DOFade(isOn ? .9f : 0, 0f));
	}
#nullable enable
	public virtual async void SetButton(Theme theme)
	{
		Tag? themeTag = theme.GetThemeTagByCategoryName("VIEW");

		var graph = FindObjectOfType<GraphController>().Graph;
		ColorToggle.IsOn = false;

		if (theme.GetMediaByName("TitletSmall") != null)
			_title.text = theme.GetMediaByName("TitletSmall")?.Text;
		else
			_title.text = theme.Name;
		if (theme.ImagePath is not null && !gameObject.name.Contains("White"))
		{
			var tex = await AssetsFileLoader.LoadTextureAsync(Api.GetFullLocalPath(theme.ImagePath));
			_rawImage.texture = tex;
			if (_rawImage.GetComponent<AspectRatioFitter>() != null)
				_rawImage.GetComponent<AspectRatioFitter>().aspectRatio = (float)tex.width / tex.height;
			if (!gameObject.name.Contains("White"))
				_rawImage.SetNativeSize();
		}

		var btn = GetComponentInChildren<UIButton>();
		btn.OnClick.OnTrigger.Event.AddListener(() => {

			ColorToggle.IsOn = true;
			Data.Theme = theme;
			OnMenuButtonClicked?.Invoke();

			if (themeTag != null)
			{
				graph.SetActiveNodeByName(themeTag.Title);
			}
			else if (theme.Label != "Green")
			{
				graph.SetActiveNodeByName("InfoSmallView");
			}





		});

	}
#nullable disable
}
