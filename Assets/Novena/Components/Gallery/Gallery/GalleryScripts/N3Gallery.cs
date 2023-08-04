using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Doozy.Engine.UI;
using Novena.DAL.Model.Guide;
using Novena.Networking.Image;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cache = Novena.Utility.Cache.Cache;
using Debug = UnityEngine.Debug;

namespace Novena.Components.Gallery.Gallery.GalleryScripts {
	// TODO - napraviti refrekturaciju koda
	// TODO - napraviti da se pocetna slika odma ucita a ostale fade-aju
	// TODO - napraviti da se one page ljepse postavlja!!

	// light version of Gallery from base project
	public class N3Gallery : MonoBehaviour {

		#region Public fields for Custom Editor
		// adjustable settings
		public float AutomaticImageChangeSpeed = 0.0f;
		// override gallery name loading Media
		public bool OverrideGalleryNameMediaText;
		public string _OverrideGalleryNameMediaText;
		// SlideShow Options
		public bool UseDefault;
		public bool UseOnePageSlideShow;
		public bool UseTwoPagesSlideShow;
		// Gallery presets names
		public string LeftShowPresetName;
		public string LeftHidePresetName;
		public string RightShowPresetName;
		public string RightHidePresetName;
		// gallery Text section
		public TMP_Text PagerText;
		// Gallery Images
		public RawImage ImageOne;
		public RawImage ImageTwo;
		// UI views
		public UIView ImageOneUIView;
		public UIView ImageTwoUIView;
		// REFERENCES
		public GameObject PagerContainer;
		public Texture2D ImagePlaceholderTexture;
		#endregion

		#region Private gallery fields
		private const string UNCATEGORIZED = "Uncategorized";
		private const string DEFAULT = "Default";
		private const string GALLERY = "Gallery";

		private Theme _theme;
		private List<Photo> _photoList;
		private int _currentPageIndex;
		private bool _imageOneIsActive;
		private bool _lastClicked;
		private CanvasGroup _thisCanvasGroup;
		
		private IEnumerator LoadAutomaticNewImageCoroutine;
		#endregion

		// Gallery starting point
		// Called out of Gallery
		public void Setup(Theme theme)
		{
			_theme = theme;

			ResetGallery();
			SetupGallery();

			_thisCanvasGroup.DOFade(1.0f, 0.0f);

			// custom testing
			//test();
		}

		// testiranje- poziva se u Setup() metodi
		private void test()
		{
			//UIView test = UIView.ShowView(GALLERY, LeftShowPresetName,true);
			List<UIView> test = UIView.GetViews(GALLERY,"ImageOne");
			test[0].ShowBehavior.Animation.Move.Duration = 1.0f;
		}

		// reset to start settings
		public async void ResetGallery()
		{
			if (LoadAutomaticNewImageCoroutine != null)
			{
				StopCoroutine(LoadAutomaticNewImageCoroutine);
			}
			
			_photoList = new List<Photo>();

			_imageOneIsActive = true;
			_lastClicked = true;

			_thisCanvasGroup = GetComponent<CanvasGroup>();

			_currentPageIndex = 1;

			//LoadPlaceholderImage();
			
			await Cache.ClearCacheAsync("Gallery");
		}

		private void LoadPlaceholderImage()
		{
			SetTexture(ImageOne, ImagePlaceholderTexture);
			SetTexture(ImageTwo, ImagePlaceholderTexture);
		}

		private void SetTexture(RawImage rawImage, Texture2D texture2D)
		{
			rawImage.texture = texture2D;
			float aspect = (float)texture2D.width / (float)texture2D.height;

			try
			{
				var aspecRatioComponent = rawImage.GetComponent<AspectRatioFitter>();

				aspecRatioComponent.aspectRatio = aspect;
			}
			catch (Exception e)
			{
				Debug.Log("Missing aspect ratio component! " + e);
			}
		}

		// called once to Load/Initialized gallery settings, must call before "SetupGallery"
		private void SetupGalleryEditorSettings()
		{
			if (UseOnePageSlideShow || UseTwoPagesSlideShow)
			{
				ImageOneUIView.InstantHide();
				ImageTwoUIView.InstantHide();
			} 
			else if (UseOnePageSlideShow == false || UseTwoPagesSlideShow == false)
			{
				ImageTwoUIView.InstantHide();
			}
			else
			{
				ImageOneUIView.ShowBehavior.LoadPreset(UNCATEGORIZED, DEFAULT);
				ImageOneUIView.HideBehavior.LoadPreset(UNCATEGORIZED, DEFAULT);

				ImageTwoUIView.ShowBehavior.LoadPreset(UNCATEGORIZED, DEFAULT);
				ImageTwoUIView.HideBehavior.LoadPreset(UNCATEGORIZED, DEFAULT);
			}
		}

		// Setup buttons, Load images and caption from Theme...
		private void SetupGallery()
		{
			SetupGalleryEditorSettings();

			SetupPhotos();

			//If there is NO images in theme 
			if (_photoList == null) return;
			
			LoadNewImage();
		}

		private void SetupPhotos()
		{
			
			//ToDo Handle _photoList null
			if (OverrideGalleryNameMediaText)
			{
				//_photoList = MediaHelper.Get.GetMediaPhotos(_theme, _OverrideGalleryNameMediaText);
				_photoList = _theme.GetMediaByName(_OverrideGalleryNameMediaText)?.GetPhotos();
			} else
			{
				_photoList = _theme.GetMediaByName(GALLERY)?.GetPhotos();
				//_photoList = MediaHelper.Get.GetMediaPhotos(_theme, GALLERY);
			}

			//_photoList = _photoList.OrderBy(p => p.Rank).ToList();
		}

		private void LoadNewImage(bool pageIndexIncrement = false)
		{
			SetPager();

			if (UseOnePageSlideShow == true && (UseOnePageSlideShow && UseTwoPagesSlideShow) == false)
			{
				ImageOneUIView.Hide();

				if (pageIndexIncrement)
				{
					ImageOneUIView.ShowBehavior.LoadPreset(GALLERY, RightShowPresetName);
					ImageOneUIView.HideBehavior.LoadPreset(GALLERY, LeftHidePresetName);
				}
				else
				{
					ImageOneUIView.ShowBehavior.LoadPreset(GALLERY, LeftShowPresetName);
					ImageOneUIView.HideBehavior.LoadPreset(GALLERY, RightHidePresetName);
				}

				//FadingContentCG.DOFade(0.0f, fadingSpeedSwitcher).OnComplete(
				//	() => {
						LoadContent();
						if (UseOnePageSlideShow)
						{
							ImageOneUIView.Show();
						}
					//FadingContentCG.DOFade(1.0f, fadingSpeedSwitcher);
				//});
			} 
			else if (UseTwoPagesSlideShow)
			{
				// ako je prijasnji klik jednak sadasnjem (next-next) tada stavlja
				// boolean flag na last clicked tako da ne switch-a preset animacije
				if (_lastClicked != pageIndexIncrement)
				{
					_lastClicked = pageIndexIncrement;
				} else
				{
					if (_lastClicked)
					{
						_lastClicked = false;
						_imageOneIsActive = true;
					} else
					{
						_lastClicked = true;
						_imageOneIsActive = false;
					}
				}
				// ulazi ako se ide udesno
				if (pageIndexIncrement)
				{
					// ulazi ako je first page aktivan
					if (_imageOneIsActive)
					{
						ImageOneUIView.ShowBehavior.LoadPreset(GALLERY, RightShowPresetName);
						ImageTwoUIView.HideBehavior.LoadPreset(GALLERY, LeftHidePresetName);
					}
					// ulazi ako je second screen aktivan
					else
					{
						ImageTwoUIView.ShowBehavior.LoadPreset(GALLERY, RightShowPresetName);
						ImageOneUIView.HideBehavior.LoadPreset(GALLERY, LeftHidePresetName);
					}

					//FadingContentCG.DOFade(0.0f, fadingSpeedSwitcher).OnComplete(
					//	() => {
							LoadDualContent(_imageOneIsActive);
							//FadingContentCG.DOFade(1.0f, fadingSpeedSwitcher);

							if (_imageOneIsActive)
							{
								_imageOneIsActive = false;
								ImageOneUIView.Show();
								ImageTwoUIView.Hide();
							} else
							{
								_imageOneIsActive = true;
								ImageOneUIView.Hide();
								ImageTwoUIView.Show();
							}
					//});
				}
				// ulazi ako se ide ulijevo
				else
				{
					// ulazi ako je first page aktivan
					if (_imageOneIsActive)
					{
						ImageOneUIView.ShowBehavior.LoadPreset(GALLERY, LeftShowPresetName);
						ImageTwoUIView.HideBehavior.LoadPreset(GALLERY, RightHidePresetName);
					}
					// ulazi ako first page nije aktivan
					else
					{
						ImageTwoUIView.ShowBehavior.LoadPreset(GALLERY, LeftShowPresetName);
						ImageOneUIView.HideBehavior.LoadPreset(GALLERY, RightHidePresetName);

					}

					//FadingContentCG.DOFade(0.0f, fadingSpeedSwitcher).OnComplete(
					//	() => {
							LoadDualContent( _imageOneIsActive);
							//FadingContentCG.DOFade(1.0f, fadingSpeedSwitcher);
							if (_imageOneIsActive)
							{
								_imageOneIsActive = false;
								ImageOneUIView.Show();
								ImageTwoUIView.Hide();
							} else
							{
								_imageOneIsActive = true;
								ImageOneUIView.Hide();
								ImageTwoUIView.Show();
							}
					//});
				}
			} 
			else
			{
				//FadingContentCG.DOFade(0.0f, fadingSpeedSwitcher).OnComplete(
				//	() => {
						LoadContent();
					//FadingContentCG.DOFade(1.0f, fadingSpeedSwitcher);
				//});
			}
			// metoda ucitava content u jednu sliku!!!!
			async void LoadContent()
			{
				int loadOnCurrentIndex = _currentPageIndex - 1;

				//Debug.Log(loadOnCurrentIndex);

				await ImageLoader.LoadImageAsync(_photoList[loadOnCurrentIndex].Path.TrimStart('~'),
					_photoList[loadOnCurrentIndex].Timestamp, ImageOne);

				//await ImageLoader.LoadImageWithCacheAsync(_photoList[loadOnCurrentIndex], ImageOne);

				//AssetsFileLoader.LoadTexture2D(_photoList[loadOnCurrentIndex].Path, ImageOne);

				LoadAutomaticNewImageCoroutine = LoadAutomaticNewImage(true);
				StartCoroutine(LoadAutomaticNewImageCoroutine);
			}

			// metoda ucitava dual content u obje slike , u slucaju ako se koristi 2 site slideshow
			void LoadDualContent(bool imageOneIsActive) // ako se ide lijevo tada se treba puniti iz liste -1....???
			{
				int loadOnCurrentIndex = _currentPageIndex - 1;

				if (imageOneIsActive)
				{
					//AssetsFileLoader.LoadTexture2D(_photoList[loadOnCurrentIndex].Path, ImageOne);
					//ImageLoader.LoadImageWithCacheAsync(_photoList[loadOnCurrentIndex], ImageOne).Forget();
					ImageLoader.LoadImageAsync(_photoList[loadOnCurrentIndex].Path.TrimStart('~'),
						_photoList[loadOnCurrentIndex].Timestamp, ImageOne).Forget();
				} else
				{
					//AssetsFileLoader.LoadTexture2D(_photoList[loadOnCurrentIndex].Path, ImageTwo);
					//ImageLoader.LoadImageWithCacheAsync(_photoList[loadOnCurrentIndex], ImageOne).Forget();
					ImageLoader.LoadImageAsync(_photoList[loadOnCurrentIndex].Path.TrimStart('~'),
						_photoList[loadOnCurrentIndex].Timestamp, ImageOne).Forget();
				}

				LoadAutomaticNewImageCoroutine = LoadAutomaticNewImage(true);
				StartCoroutine(LoadAutomaticNewImageCoroutine);
			}

			Resources.UnloadUnusedAssets();
		}

		private void SetPager()
		{
			PagerText.text = _currentPageIndex + "/" + _photoList.Count;
		}

		//private void SetImageIndexThenLoadImage(bool pageIndexIncrement)
		//{
		//	int maxPageIndex = _photoList.Count;

		//	if (pageIndexIncrement == true)
		//	{
		//		_currentPageIndex += 1;
		//		if (_currentPageIndex > maxPageIndex)
		//		{
		//			_currentPageIndex = 1;
		//		}
		//	} else
		//	{
		//		_currentPageIndex -= 1;
		//		if (_currentPageIndex < 1)
		//		{
		//			_currentPageIndex = maxPageIndex;
		//		}
		//	}

		//	LoadNewImage(pageIndexIncrement);

		//	StartCoroutine(LoadAutomaticNewImage(pageIndexIncrement));
		//}

		private IEnumerator LoadAutomaticNewImage(bool pageIndexIncrement)
		{
			yield return new WaitForSeconds(AutomaticImageChangeSpeed);

			int maxPageIndex = _photoList.Count;

			if (pageIndexIncrement == true)
			{
				_currentPageIndex += 1;
				if (_currentPageIndex > maxPageIndex)
				{
					_currentPageIndex = 1;
				}
			} else
			{
				_currentPageIndex -= 1;
				if (_currentPageIndex < 1)
				{
					_currentPageIndex = maxPageIndex;
				}
			}

			LoadNewImage(pageIndexIncrement);
		}
	}// end of Gallery class
}// end of namespace
