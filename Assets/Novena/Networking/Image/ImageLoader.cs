#nullable enable
using System;
using System.IO;
using Cysharp.Threading.Tasks;
using DG.Tweening.Plugins.Core.PathCore;
using Novena.DAL;
using Novena.DAL.Entity;
using Novena.DAL.Model.Guide;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Cache = Novena.Utility.Cache.Cache;
using Path = System.IO.Path;

namespace Novena.Networking.Image {
	/// <summary>
	/// Image utility for downloading and loading of images
	/// </summary>
	public static class ImageLoader {
		/// <summary>
		/// Test get texture
		/// </summary>
		/// <param name="path"></param>
		/// <param name="timeStamp"></param>
		/// <returns></returns>
		private static async UniTask<Texture2D?> GetTextureTest(string path, string timeStamp)
		{
			bool fileExist = false;

			string orgPath = path;

			//Just to remove / from string for path combine
			var pathForCombine = path.Substring(1, path.Length - 1);

			//File path on disk
			string filePath = Path.Combine(Application.persistentDataPath, DataAccess.Instance.downloadCode, pathForCombine);

			//Check Db for file
			DAL.Model.File? fileFromDb;
			using (FilesEntity filesEntity = new FilesEntity())
			{
				fileFromDb = filesEntity.GetByFilePath(orgPath);
			}

			//If file exist on disk dont write it to disk again and dont store it to db again
			if (fileFromDb != null)
			{
				fileExist = true;
				//Log.Log.LogMessage("File Exist: " + fileFromDb.FilePath);
				path = UrlPlatformHelper.GetPlatformFilePath(fileFromDb.LocalPath);
			}
			else
			{
				path = Api.DOWNLOAD_FILES + path;
				//Log.Log.LogMessage("File DONT Exist: " + path);
			}

			//DownloadFile and save it to Db
			var uwr = new UnityWebRequest(path);

			uwr.downloadHandler = new DownloadHandlerBuffer();

			var response = await uwr.SendWebRequest();

			if (response.result != UnityWebRequest.Result.Success)
			{
				Log.Log.LogError("ImageLoader GetTexture: " + response.error);
			}

			if (response.isDone)
			{
				if (response.result == UnityWebRequest.Result.Success)
				{
					if (fileExist == false)
					{
						if (Directory.Exists(Path.GetDirectoryName(filePath)) == false)
						{
							Directory.CreateDirectory(Path.GetDirectoryName(filePath));
						}

						//Save bytes to file on disk
						File.WriteAllBytes(filePath, response.downloadHandler.data);

						//Save to db
						DAL.Model.File file = new DAL.Model.File();
						file.FilePath = orgPath;
						file.GuideId = 1;
						file.LocalPath = filePath;
						file.TimeStamp = timeStamp;

						using (FilesEntity filesEntity = new FilesEntity())
						{
							filesEntity.Insert(file);
						}
					}

					//Create texture and return it
					Texture2D tex = new Texture2D(1, 1);
					tex.LoadImage(response.downloadHandler.data, true);
					return tex;
				}
			}

			return null;
		}


		/// <summary>
		/// Download image from URI.
		/// </summary>
		/// <param name="path"></param>
		/// <returns>Texture2D if successful NULL if error in download</returns>
		private static async UniTask<Texture2D?> GetTexture(string path)
		{
			var response = await UnityWebRequestTexture.GetTexture(path, true).SendWebRequest();

			if (response.result != UnityWebRequest.Result.Success)
			{
				Log.Log.LogError("ImageLoader GetTexture: " + response.error);
			}

			if (response.isDone)
			{
				if (response.result == UnityWebRequest.Result.Success)
				{
					return DownloadHandlerTexture.GetContent(response);
				}
			}

			return null;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static async UniTask<Texture2D> GetTextureExplicit(string path)
		{
			try
			{
				var response = await UnityWebRequestTexture.GetTexture(path, true).SendWebRequest();
				if (response.result != UnityWebRequest.Result.Success)
				{
					Debug.LogError("ImageLoader GetTexture: " + response.error);
				}
				if (response.result == UnityWebRequest.Result.Success)
				{
					var output = DownloadHandlerTexture.GetContent(response);
					output.name = path;
					return output;
				}
				else
				{
					var e = new ImageLoaderException("Download error!", path);
					Debug.LogException(e);
					throw e;
				}
			}
			catch (UnityWebRequestException e)
			{
				var ex = new ImageLoaderException(e.Message, path);
				Debug.LogException(ex);
				throw ex;
			}
		}
		/// <summary>
		/// Download image and set it to rawImage with calculated aspect ratio if aspect ratio fitter exist.
		/// </summary>
		/// <param name="path">File path or Url</param>
		/// <param name="rawImage">RawImage component to apply texture</param>
		public static async UniTask<bool> LoadImageAsync(string path, string timeStamp, RawImage rawImage)
		{
			bool output = false;

			//var texture = await GetTexture(path);

			var texture = await GetTextureTest(path, timeStamp);

			if (texture != null)
			{
				try
				{
					texture.name = path;
					rawImage.texture = texture;
					SetAspectRatio(texture, rawImage);
					output = true;
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
			}

			return output;
		}

		/// <summary>
		/// Download image and cache it. Set it to rawImage with calculated aspect ratio if aspect ratio fitter exist.
		/// If image is cached it will not download it again.
		/// </summary>
		/// <param name="photo">Media photo</param>
		/// <param name="rawImage">RawImage component</param>
		public static async UniTask LoadImageWithCacheAsync(Photo photo, RawImage rawImage)
		{
			Texture2D? texture;

			if (Cache.Contains("Gallery", photo.Id))
			{
				texture = (Cache.Get("Gallery", photo.Id).CachedObject as Texture2D)!;
			}
			else
			{
				texture = (await GetTexture(photo.FullPath))!;
				Cache.Store("Gallery", photo.Id, texture);
			}

			rawImage.texture = texture;

			SetAspectRatio(texture, rawImage);
		}


		/// <summary>
		/// Sets aspect ratio on AspectRatioFitter 
		/// </summary>
		/// <param name="texture"></param>
		/// <param name="rawImage"></param>
		/// <exception cref="Exception">If no AspectRatioFitter component is attached.</exception>
		public static void SetAspectRatio(Texture texture, RawImage rawImage)
		{
			float aspectRatio = (float)texture.width / (float)texture.height;

			try
			{
				var aspectRatioFitter = rawImage.GetComponent<AspectRatioFitter>();
				aspectRatioFitter.aspectRatio = aspectRatio;
			}
			catch (Exception e)
			{
				Log.Log.LogMessage(e.Message);
			}
		}
	}
}