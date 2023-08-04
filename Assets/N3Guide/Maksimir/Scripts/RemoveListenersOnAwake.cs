using DG.Tweening;
using Doozy.Engine.UI;
using Novena.Components.Idle;
using Novena.Controller;
using Novena.DAL;
using UnityEngine;

public class RemoveListenersOnAwake : MonoBehaviour
{

	[SerializeField] private UIButton _closeFSGallery;
	
	private void Awake()
	{
		DOTween.SetTweensCapacity(500, 50);
		Data.OnTranslatedContentUpdated = null;
		InitController.OnGuideLoaded = null;
		FooterController.OnContainerInstantiated = null;
		IdleController.OnIdleEnabled = null;
		NewsViewController.OnNewsButtonClick=null;
		OfferButton.OnButtonPresed = null;
		HeritageButton.OnMoreSubThemes = null;
		HeritageButton.OnThemeClicked = null;
		VideoDetailsViewController.OnHideSendTag = null;
		VideoDetailsViewController.OnMediaPlayerInstantiated = null;
		VlcPlayer.OnVideoOpened = null;
		VideoButton.OnVideoClicked = null;
		MenuButton.OnMenuButtonClicked = null;
		_closeFSGallery.OnClick.OnTrigger.Event.RemoveAllListeners();
	}
}
