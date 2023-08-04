using Cysharp.Threading.Tasks;
using Doozy.Engine.Nody;
using Novena.DAL;
using Novena.DAL.Model.Guide;
using Novena.Networking;
using Novena.UiUtility.Base;
using Scripts.Utility;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeepsakeViewController : UiController {

	[SerializeField] private TMP_Text _backText;
	[SerializeField] private SideQR _sideQR;

	public override void Awake()
	{
		base.Awake();
		Data.OnTranslatedContentUpdated += () => {
			if (FindObjectOfType<GraphController>().Graph.ActiveNode.Name == "KeepsakeView")
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
		_backText.text = Data.Theme.Name;
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


	public override void OnHideViewFinished()
	{
		base.OnHideViewFinished();
		_sideQR.Reset();
	}
}
