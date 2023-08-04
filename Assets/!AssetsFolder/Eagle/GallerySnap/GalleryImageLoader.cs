using Cysharp.Threading.Tasks;
using Novena.Networking.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GalleryImageLoader : IDisposable {
	private GalleryCache _galleryCache = new GalleryCache();

	/// <summary>
	/// List of images currently in download.
	/// </summary>
	private List<string> _inDownload = new List<string>();

	public void Dispose()
	{
		_galleryCache.CleanCache();
	}

	/// <summary>
	/// Load image
	/// Checks if image already cached
	/// </summary>
	/// <param name="rawImage"></param>
	/// <param name="path"></param>
	/// <returns></returns>
	public async UniTaskVoid LoadImage(RawImage rawImage, string path)
	{
		var loadedTexture = _galleryCache.Load(path);
		if (loadedTexture != null)
		{
			rawImage.texture = loadedTexture;
			ImageLoader.SetAspectRatio(rawImage.texture, rawImage);
		}
		else
		{

			Debug.Log("Image not in cache! Downloading.");

			if (_inDownload.Any(i => i == path)) return;

			_inDownload.Add(path);

			try
			{
				var downloadedTexture = await ImageLoader.GetTextureExplicit(path);

				_inDownload.Remove(downloadedTexture.name);

				if (downloadedTexture != null)
				{
					_galleryCache.Save(path, downloadedTexture);
					rawImage.texture = downloadedTexture;
					ImageLoader.SetAspectRatio(rawImage.texture, rawImage);
				}
			}
			catch (ImageLoaderException e)
			{
				_inDownload.Remove(e.PhotoPath);
			}
		}
	}

	public async UniTask LoadImageVariation(RawImage rawImage, string path)
	{
		var loadedTexture = _galleryCache.Load(path);
		if (loadedTexture != null)
		{
			rawImage.texture = loadedTexture;
			ImageLoader.SetAspectRatio(rawImage.texture, rawImage);
		}
		else
		{

			Debug.Log("Image not in cache! Downloading.");

			if (_inDownload.Any(i => i == path)) return;

			_inDownload.Add(path);

			try
			{
				var downloadedTexture = await ImageLoader.GetTextureExplicit(path);

				_inDownload.Remove(downloadedTexture.name);

				if (downloadedTexture != null)
				{
					_galleryCache.Save(path, downloadedTexture);
					rawImage.texture = downloadedTexture;
					ImageLoader.SetAspectRatio(rawImage.texture, rawImage);
				}
			}
			catch (ImageLoaderException e)
			{
				_inDownload.Remove(e.PhotoPath);
			}
		}
	}
}
