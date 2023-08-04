using Doozy.Engine.UI;
using Novena.UiUtility.Base;
using TMPro;
using UnityEngine;
using Utility;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using System;
using System.Net.Mail;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using DG.Tweening;
using Novena.DAL;
using Doozy.Engine.Nody;
using Novena.DAL.Model.Guide;
using System.Collections.Generic;
using System.Linq;
using Image = UnityEngine.UI.Image;
using static UnityEngine.UI.Extensions.Gradient2;
using Scripts.Utility;
using Novena.Networking;

public class ReportViewController : UiController {

	[SerializeField] private InputFieldCustom _name;
	[SerializeField] private InputFieldCustom _email;
	[SerializeField] private InputFieldCustom _content;
	[SerializeField] private SideQR _sideQR;


	[SerializeField] private UIButton _send;

	[SerializeField] private TMP_Text _backText;
	[SerializeField] private CanvasGroup _input;
	[SerializeField] private CanvasGroup _response;

	[SerializeField] private TMP_Text _responseText;



	public override void Awake()
	{
		base.Awake();
		_send.OnClick.OnTrigger.Event.RemoveAllListeners();
		_send.OnClick.OnTrigger.Event.AddListener(() => {
			if (CheckFormating())
			{
				SendReportData().Forget();

				ShowCanvasGroup.Show(_input, false, .5f);
				ShowCanvasGroup.Show(_response, true, .5f);
			}
		});

		Data.OnTranslatedContentUpdated += () => {
			if (FindObjectOfType<GraphController>().Graph.ActiveNode.Name == "ReportView")
			{
				Data.Theme = Data.TranslatedContent.GetThemeByLanguageSwitchCode(Data.Theme.LanguageSwitchCode);
				_backText.text = Data.Theme.Name;

				SetQr().Forget();
			}
		};

	}



	public override void OnShowViewStart()
	{
		base.OnShowViewStart();
		ShowCanvasGroup.Show(_input, true);
		ShowCanvasGroup.Show(_response, false);
		_backText.text = Data.Theme.Name;
		_name.text = "";
		_email.text = "";
		_content.text = "";
		_send.Interactable = false;

		SetQr().Forget();
	}

	private async UniTaskVoid SetQr()
	{
		if (Data.Theme.GetMediaByName("QR") != null)
		{
			if (Data.Theme.GetMediaByName("QR").ContentPath != "")
			{
				var tex = await AssetsFileLoader.LoadTextureAsync(Api.GetFullLocalPath(Data.Theme.GetMediaByName("QR").ContentPath));
				_sideQR.QRImage.GetComponent<RawImage>().texture = tex;
			}
		}
	}
	private bool CheckFormating()
	{
		bool isNameCorrect = Regex.IsMatch(_name.text, @"^[a-zA-Z]+$");
		bool isEmailCorrect = Regex.IsMatch(_email.text, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");

		if (!isNameCorrect)
		{
			_name.transform.GetChild(1).GetComponent<Image>().DOKill();
			_name.transform.GetChild(1).GetComponent<Image>().DOFade(1, 0);
			_name.transform.GetChild(1).GetComponent<Image>().DOFade(0, 1f);
		}
		if (!isEmailCorrect)
		{
			_email.transform.GetChild(1).GetComponent<Image>().DOKill();
			_email.transform.GetChild(1).GetComponent<Image>().DOFade(1, 0);
			_email.transform.GetChild(1).GetComponent<Image>().DOFade(0, 1f).SetDelay(1);
		}
		return (isNameCorrect && isEmailCorrect);
	}



	private async UniTaskVoid SendReportData()
	{
		var form = new WWWForm();
		form.AddField("name", _name.text);
		form.AddField("email", _email.text);
		form.AddField("message", _content.text);

		using (UnityWebRequest www = UnityWebRequest.Post(CMSBaseManager.GetCMSPath() + "report/post-reports.ashx", form))
		{
			try
			{
				await www.SendWebRequest();
			}
			catch (Exception)
			{
				_responseText.text = "Nema interneta.";
				Debug.LogError(www.result);
			}
		}
	}

	public void OnInputChanged()
	{
		if (_name.text == "" || _email.text == "" || _content.text == "")
		{
			_send.Interactable = false;
		}
		else
		{
			_send.Interactable = true;
		}
	}
	public override void OnHideViewFinished()
	{
		base.OnHideViewFinished();
		_sideQR.Reset();
	}

}
