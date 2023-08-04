using Doozy.Engine.UI;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Novena.DAL;
using System.Linq;
using UnityEngine.UI;
using Novena.Utility;

public class LanguageButtonsController : MonoBehaviour {
	[SerializeField] private RectTransform _buttonsBackground;

	[SerializeField] private UIToggle _toggle;

	[SerializeField] private GameObject _languagePrefab;


	private List<GameObject> _buttons = new List<GameObject>();

	private void Awake()
	{
		_toggle.IsOn = false;
		_toggle.OnValueChanged.RemoveAllListeners();
		_toggle.OnValueChanged.AddListener((isOn) => {
			Resources.UnloadUnusedAssets();
			if (isOn)
			{
				GenerateLanguageButtons();
				UiBlocker.Enable();
				_buttonsBackground.DOAnchorPos(new Vector2(0, 64.2f), 0.5f).From(new Vector2(-_buttons.Count * 100, 64.2f)).OnComplete(UiBlocker.Disable);
			}
			else
			{
				UiBlocker.Enable();
				_buttonsBackground.DOAnchorPos(new Vector2(-_buttons.Count * 100, 64.2f), 0.5f).OnComplete(() => {
					DestroyGameObjects();
					UiBlocker.Disable();
				});
			}
		});
	}

	private void GenerateLanguageButtons()
	{
		if (Data.Guide.TranslatedContents.Any() == false) return;

		foreach (var tc in Data.Guide.TranslatedContents)
		{
			GameObject obj = Instantiate(_languagePrefab, _buttonsBackground);
			obj.SetActive(true);

			if (Data.TranslatedContent == tc)
			{
				obj.GetComponent<UIToggle>().IsOn = true;
				obj.GetComponent<RawImage>().DOFade(1, 0);
			}
			else
				obj.GetComponent<UIToggle>().IsOn = false;
			obj.GetComponent<LanguageButton>().Setup(tc);
			obj.GetComponent<Toggle>().onValueChanged.AddListener((isOn) => {
				if (isOn)
				{
					_toggle.IsOn = false;
				}
			});
			_buttons.Add(obj);
		}
	}
	private void DestroyGameObjects()
	{
		for (int i = 0; i < _buttons.Count; i++)
		{
			Destroy(_buttons[i].gameObject);
		}
		_buttons.Clear();
	}
}
