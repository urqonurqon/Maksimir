using Cysharp.Threading.Tasks;
using Novena.UiUtility.Base;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.IO;
using Doozy.Engine.UI;
using Utility;
using DG.Tweening;
using System.Linq;
using Doozy.Engine;
using System;
using Newtonsoft.Json;
using Application = UnityEngine.Application;
using UnityEngine.UI;
using System.Collections;
using Novena.DAL;
using Doozy.Engine.Nody;
using System.Text.RegularExpressions;
using System.Net.Mail;
using Scripts.Utility;

public class LostFoundViewController : UiController {
	[SerializeField] private InputFieldCustom _name;
	[SerializeField] private InputFieldCustom _mobile;
	[SerializeField] private InputFieldCustom _email;
	[SerializeField] private InputFieldCustom _content;

	[SerializeField] private TMP_Text _responseText;

	[SerializeField] private UIButton _send;
	[SerializeField] private UIButton _back;

	[SerializeField] private List<CanvasGroup> _lostFound = new List<CanvasGroup>();
	[SerializeField] private CanvasGroup _iLostResponse;
	[SerializeField] private CanvasGroup _lostItem;
	[SerializeField] private CanvasGroup _menu;
	[SerializeField] private GameObject _englishExtras;
	[SerializeField] private GameObject _itemPrefab;
	[SerializeField] private RectTransform _itemsParent;

	[SerializeField] private List<UIToggle> _listToggles = new List<UIToggle>();

	[SerializeField] private TMP_Text _lostItemText;
	[SerializeField] private TMP_FontAsset _bold;
	[SerializeField] private TMP_FontAsset _regular;

	[SerializeField] private UIButton _menu1;
	[SerializeField] private UIButton _menu2;
	[SerializeField] private UIButton _menu3;
	[SerializeField] private TMP_Text _backText;
	//[SerializeField] private RawImage _qr;

	private List<GameObject> _instantiatedItems = new List<GameObject>();
	private List<Item> _items;

	public override void Awake()
	{
		base.Awake();
		//SetServerUrl();
		_send.OnClick.OnTrigger.Event.RemoveAllListeners();
		_send.OnClick.OnTrigger.Event.AddListener(() => {
			if (CheckFormating())
			{

				SendReportData().Forget();

				ShowCanvasGroup.Show(_lostFound[1], false, .5f);
				ShowCanvasGroup.Show(_iLostResponse, true, .5f);
			}
		});
		_listToggles[0].OnValueChanged.RemoveAllListeners();
		_listToggles[0].OnValueChanged.AddListener((isOn) => {
			if (isOn)
			{
				DestroyLostItems();
				GenerateLostItems(_items.Where(item => item.ObjectCategoryId == 2));
			}
			OnLostFoundCategorieClicked(_listToggles[0], isOn);
		});
		_listToggles[1].OnValueChanged.RemoveAllListeners();
		_listToggles[1].OnValueChanged.AddListener((isOn) => {
			if (isOn)
			{
				DestroyLostItems();
				GenerateLostItems(_items.Where(item => item.ObjectCategoryId == 1));
			}
			OnLostFoundCategorieClicked(_listToggles[1], isOn);
		});


		Data.OnTranslatedContentUpdated += () => {
			if (FindObjectOfType<GraphController>().Graph.ActiveNode.Name == "LostFoundView")
			{
				Data.Theme = Data.TranslatedContent.GetThemeByLanguageSwitchCode(Data.Theme.LanguageSwitchCode);
				_backText.text = Data.Theme.Name;
				OnShowViewStart();
				OnHideViewFinished();

				//SetQr().Forget();
			}
		};

	}



	public override void OnShowViewStart()
	{
		base.OnShowViewStart();

		if (Data.TranslatedContent.LanguageEnglishName == "Croatian")
		{
			_englishExtras.SetActive(false);

			GetLostItems().Forget();

			_menu1.OnClick.OnTrigger.Event.RemoveAllListeners();
			_menu2.OnClick.OnTrigger.Event.RemoveAllListeners();
			_menu3.OnClick.OnTrigger.Event.RemoveAllListeners();

			_menu1.OnClick.OnTrigger.Event.AddListener(() => ShowMenu(0));
			_menu2.OnClick.OnTrigger.Event.AddListener(() => ShowMenu(1));
			_menu3.OnClick.OnTrigger.Event.AddListener(() => ShowMenu(2));
			ShowCanvasGroup.Show(_menu, true);
		}
		else
		{
			ShowCanvasGroup.Show(_menu, false);
			_englishExtras.SetActive(true);
			ShowMenu(0);


		}


		_backText.text = Data.Theme.Name;

		_lostFound.ForEach((cg) => ShowCanvasGroup.Show(cg, false));
		ShowCanvasGroup.Show(_iLostResponse, false);
		ShowCanvasGroup.Show(_lostFound[2], false);
		_send.Interactable = false;
		_name.text = "";
		_mobile.text = "";
		_email.text = "";
		_content.text = "";

		//SetQr().Forget();
	}

	//private async UniTaskVoid SetQr()
	//{
	//	if (Data.Theme.GetMediaByName("QR") != null)
	//	{
	//		if (Data.Theme.GetMediaByName("QR").ContentPath != "")
	//		{
	//			var tex = await AssetsFileLoader.LoadTextureAsync(Data.Theme.GetMediaByName("QR").ContentPath);
	//			_qr.texture = tex;
	//		}
	//	}
	//}


	private bool CheckFormating()
	{
		bool isNameCorrect = Regex.IsMatch(_name.text, @"^[a-zA-Z ]+$");
		bool isEmailCorrect = Regex.IsMatch(_email.text, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
		bool isMobileCorrect = Regex.IsMatch(_mobile.text, @"^\+?[0-9]+$");

		if (!isNameCorrect)
		{
			_name.transform.GetChild(1).GetComponent<Image>().DOKill();
			_name.transform.GetChild(1).GetComponent<Image>().DOFade(1, 0);
			_name.transform.GetChild(1).GetComponent<Image>().DOFade(0, 1f).SetDelay(1);
		}
		if (!isEmailCorrect)
		{
			_email.transform.GetChild(1).GetComponent<Image>().DOKill();
			_email.transform.GetChild(1).GetComponent<Image>().DOFade(1, 0);
			_email.transform.GetChild(1).GetComponent<Image>().DOFade(0, 1f).SetDelay(1);
		}
		if (!isMobileCorrect)
		{
			_mobile.transform.GetChild(1).GetComponent<Image>().DOKill();
			_mobile.transform.GetChild(1).GetComponent<Image>().DOFade(1, 0);
			_mobile.transform.GetChild(1).GetComponent<Image>().DOFade(0, 1f).SetDelay(1);
		}
		return (isNameCorrect && isEmailCorrect && isMobileCorrect);
	}



	private async UniTaskVoid SendReportData()
	{
		var form = new WWWForm();
		form.AddField("name", _name.text);
		form.AddField("phone", _mobile.text);
		form.AddField("email", _email.text);
		form.AddField("description", _content.text);

		using (UnityWebRequest uwr = UnityWebRequest.Post(CMSBaseManager.GetCMSPath() + "lost_found/post-lost_found.ashx", form))
		{
			try
			{
				await uwr.SendWebRequest();
			}
			catch (Exception)
			{
				_responseText.text = "Nema interneta.";
				Debug.LogError(uwr.result);
			}
		}
	}

	public void OnInputChanged()
	{
		if (_name.text == "" || _email.text == "" || _mobile.text == "" || _content.text == "")
		{
			_send.Interactable = false;
		}
		else
		{
			_send.Interactable = true;
		}
	}

	private async UniTaskVoid GetLostItems()
	{
		try
		{
			string lostItems = (await UnityWebRequest.Get(CMSBaseManager.GetCMSPath() + "object_lost_found/get-Object_lost_found.ashx").SendWebRequest()).downloadHandler.text;
			_items = JsonConvert.DeserializeObject<List<Item>>(lostItems);
			if (!Directory.Exists(Application.persistentDataPath + "/CMSJsons"))
			{
				Directory.CreateDirectory(Application.persistentDataPath + "/CMSJsons");
			}
			File.WriteAllText(Application.persistentDataPath + "/CMSJsons" + "/CachedLostItems.txt", lostItems);
		}
		catch (Exception e)
		{
			Debug.Log(e);
			_items = JsonConvert.DeserializeObject<List<Item>>(File.ReadAllText(Application.persistentDataPath + "/CMSJsons" + "/CachedLostItems.txt"));
		}
	}

	private void GenerateLostItems(IEnumerable<Item> itemsEnum)
	{
		List<Item> items = itemsEnum.ToList();
		for (int i = 0; i < items.Count; i++)
		{
			var item = Instantiate(_itemPrefab, _itemsParent);
			_instantiatedItems.Add(item);
			item.GetComponentInChildren<TMP_Text>().text = items[i].Title;
			int j = i;
			item.GetComponent<UIButton>().OnClick.OnTrigger.Event.AddListener(() => {
				_lostItemText.text = items[j].Title;
				ShowCanvasGroup.Show(_lostItem, true, .5f);
				ShowCanvasGroup.Show(_lostFound[2], false, .5f);
			});
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(_itemsParent);
	}

	private void DestroyLostItems()
	{
		for (int i = 0; i < _instantiatedItems.Count; i++)
		{
			Destroy(_instantiatedItems[i]);
		}
		_instantiatedItems.Clear();
	}


	public void ShowMenu(int index)
	{
		ShowCanvasGroup.Show(_menu, false, .5f);
		ShowCanvasGroup.Show(_lostFound[index], true, .5f);
		if (index == 1)
		{
			_send.Interactable = false;
			_name.text = "";
			_mobile.text = "";
			_email.text = "";
			_content.text = "";
		}

		if (index == 2)
		{
			_listToggles[0].IsOn = true;
			DestroyLostItems();
			try
			{
				GenerateLostItems(_items.Where(item => item.ObjectCategoryId == 2));
			}
			catch (Exception e)
			{
				print(e);
			}
		}
	}

	public void Back()
	{
		if (Data.TranslatedContent.LanguageEnglishName == "Croatian")
		{
			if (_lostItem.alpha > 0)
			{
				ShowCanvasGroup.Show(_lostItem, false, .5f);
				ShowCanvasGroup.Show(_lostFound[2], true, .5f);
			}
			else
			{
				if (_lostFound.Any(cg => cg.alpha > 0) || _iLostResponse.alpha > 0)
				{
					foreach (var cg in _lostFound)
					{
						ShowCanvasGroup.Show(cg, false, .5f);
					}
					ShowCanvasGroup.Show(_iLostResponse, false, .5f);
					ShowCanvasGroup.Show(_menu, true, .5f);
				}
				else
					GameEventMessage.SendEvent("Back");
			}
		}
		else
		{
			GameEventMessage.SendEvent("Back");

		}
	}
	private void OnLostFoundCategorieClicked(UIToggle toggle, bool isOn)
	{
		TMP_Text toggleText = toggle.GetComponentInChildren<TMP_Text>();
		if (isOn)
		{
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
	//private void SetServerUrl()
	//{
	//	string filePath = Application.streamingAssetsPath + "/serverPath.txt";

	//	if (File.Exists(filePath))
	//	{
	//		_serverURL = File.ReadAllText(filePath);
	//	}
	//	else
	//	{
	//		Debug.Log("No Server url file in StreamingAssets");
	//	}
	//}
}


[Serializable]
public class Item {
	public int Id { get; set; }
	public string Title { get; set; }
	public int ObjectCategoryId { get; set; }
}
