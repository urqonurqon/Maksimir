using Cysharp.Threading.Tasks;
using DG.Tweening;
using Newtonsoft.Json;
using Novena.UiUtility.Base;
using Scripts.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FooterController : UiController {
	public static Action<RectTransform> OnContainerInstantiated;

	[SerializeField] private GameObject _containerPrefab;
	[SerializeField] private RectTransform _containerParent;

	[SerializeField] private List<Texture2D> _lawTextures;
	private List<Texture2D> _textures = new List<Texture2D>();
	private RawImage _rawImage1;
	private RawImage _rawImage2;
	private GameObject _container;
	public override async void OnShowViewStart()
	{
		base.OnShowViewStart();

		InstantiateContainers();
		await GetBanners();
		AddLawTextures();
		StartCoroutine(ChangePictures());
	}

	private void InstantiateContainers()
	{
		if (_container != null)
			Destroy(_container);
		_container = Instantiate(_containerPrefab, _containerParent);
		_rawImage1 = _container.transform.GetChild(0).GetComponent<RawImage>();
		_rawImage2 = _container.transform.GetChild(1).GetComponent<RawImage>();
		OnContainerInstantiated?.Invoke(_container.GetComponent<RectTransform>());
	}

	private async UniTask GetBanners()
	{
		try
		{
			string bannersJson = (await UnityWebRequest.Get(CMSBaseManager.GetCMSPath() + "gallery/get-gallery.ashx").SendWebRequest()).downloadHandler.text;
			var banners = JsonConvert.DeserializeObject<List<Banner>>(bannersJson);
			for (int i = 0; i < banners.Count; i++)
			{
				_textures.Add(await AssetsFileLoader.LoadTextureAsync(banners[i].ImagePath, 5, true));
			}
		}
		catch (Exception e)
		{
			Debug.Log(e);
		}

	}

	private void AddLawTextures()
	{
		_textures.AddRange(_lawTextures);
	}

	private IEnumerator ChangePictures()
	{
		_rawImage1.texture = _textures[0];
		_rawImage1.SetNativeSize();
		int i = 0;

		while (true)
		{
			i++;
			if (i > _textures.Count - 1) i = 0;

			if (_rawImage1.color.a > 0)
			{
				_rawImage2.texture = _textures[i];
				_rawImage2.SetNativeSize();

				_rawImage1.DOFade(0, 1.5f);
				_rawImage2.DOFade(1, 1.5f);
			}
			else
			{
				_rawImage1.texture = _textures[i];
				_rawImage1.SetNativeSize();

				_rawImage2.DOFade(0, 1.5f);
				_rawImage1.DOFade(1, 1.5f);
			}

			yield return new WaitForSeconds(10);
		}
	}

}

[Serializable]
public class Banner {
	public string ImagePath { get; set; }
}
