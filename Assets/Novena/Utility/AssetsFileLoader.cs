using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Scripts.Utility {
	public static class AssetsFileLoader {

		public static List<string> ListOfLoadingPhotos = new List<string>();

		public static IEnumerator LoadSprite(string path, Image image)
		{
			UnityWebRequest webRequest;
			string filepath = "";
			Texture2D texture;

			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				filepath = "file:///" + path;
			}
			if (Application.platform == RuntimePlatform.Android)
			{
				filepath = "file://" + path;
			}
			if (Application.platform == RuntimePlatform.WindowsPlayer)
			{
				filepath = "file:///" + path;
			}
			Debug.Log("<color=yellow>ImageLoader LoadSprite Image path:</color> " + filepath);

			using (webRequest = UnityWebRequestTexture.GetTexture(filepath))
			{
				yield return webRequest.SendWebRequest();

				if (webRequest.result == UnityWebRequest.Result.ConnectionError)
				{
					Debug.Log(webRequest.error);
				}
				else
				{
					Debug.Log("Loaded");
					texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
					texture.filterMode = FilterMode.Bilinear;
					texture.Compress(false);
					yield return new WaitForEndOfFrame();
					float aspectRatio = (float)texture.width / (float)texture.height;
					yield return new WaitForEndOfFrame();
					try
					{
						image.gameObject.GetComponent<AspectRatioFitter>().aspectRatio = aspectRatio;
					}
					catch
					{
						Debug.Log("<color=yellow>No Aspect Ratio Component</color>");
					}

					image.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0f, 0f));

					yield return new WaitForEndOfFrame();
					webRequest.Dispose();
				}
			}
		}


		public static async UniTask<Texture2D> LoadTextureAsync(string texturePath, int timeout = 0, bool isWritingOrReadingCache = false)
		{
			if (!Directory.Exists(Application.persistentDataPath + "/CMSPhotos"))
			{
				Directory.CreateDirectory(Application.persistentDataPath + "/CMSPhotos");
			}


			if (!ListOfLoadingPhotos.Contains(texturePath))
			{
				var uwt = UnityWebRequestTexture.GetTexture(texturePath);
				uwt.timeout = timeout;
				UnityWebRequest response = null;

				try
				{
					response = await uwt.SendWebRequest();
				}
				catch (Exception e)
				{
					Debug.Log(e);
				}


				if (response != null && response.isDone)
				{
					if (response.result == UnityWebRequest.Result.Success)
					{
						Texture2D tex = ((DownloadHandlerTexture)response.downloadHandler).texture;
						if (isWritingOrReadingCache)
							File.WriteAllBytes(Application.persistentDataPath + "/CMSPhotos" + texturePath.Remove(0, texturePath.LastIndexOf("/")), tex.EncodeToJPG());
						return tex;
					}
				}

				uwt.Dispose();
				if (isWritingOrReadingCache)
				{
					uwt = UnityWebRequestTexture.GetTexture(Application.persistentDataPath + "/CMSPhotos" + texturePath.Remove(0, texturePath.LastIndexOf("/")));
					uwt.timeout = 0;
					response = null;

					try
					{
						response = await uwt.SendWebRequest();
					}
					catch (Exception e)
					{
						Debug.Log(e);
					}

					if (response != null && response.isDone)
					{
						if (response.result == UnityWebRequest.Result.Success)
						{
							ListOfLoadingPhotos.Add(texturePath);
							LoadTextureAsync(texturePath).Forget();

							Texture2D tex = ((DownloadHandlerTexture)response.downloadHandler).texture;
							return tex;
						}
					}
					else
					{
						ListOfLoadingPhotos.Add(texturePath);
						LoadTextureAsync(texturePath).Forget();
						return null;
					}
					return null;
				}
				return null;
			}
			else
			{
				var uwt = UnityWebRequestTexture.GetTexture(texturePath);
				uwt.timeout = 0;
				UnityWebRequest response = null;

				try
				{
					response = await uwt.SendWebRequest();
				}
				catch (Exception e)
				{
					Debug.Log(e);
					ListOfLoadingPhotos.Remove(texturePath);
					return null;
				}
				ListOfLoadingPhotos.Remove(texturePath);
				var tex = ((DownloadHandlerTexture)response.downloadHandler).texture;
				if (isWritingOrReadingCache)
					File.WriteAllBytes(Application.persistentDataPath + "/CMSPhotos" + texturePath.Remove(0, texturePath.LastIndexOf("/")), tex.EncodeToJPG());
				return tex;
			}

		}




		public static IEnumerator LoadAudio(string path, Action<AudioClip> onAudioLoaded)
		{
			UnityWebRequest webRequest;
			string filepath = "file:///" + path;

			using (webRequest = UnityWebRequestMultimedia.GetAudioClip(filepath, AudioType.OGGVORBIS))
			{
				yield return webRequest.SendWebRequest();

				if (webRequest.result == UnityWebRequest.Result.ConnectionError)
				{
					Debug.Log(webRequest.error);
				}
				else
				{
					AudioClip audioclip = DownloadHandlerAudioClip.GetContent(webRequest);
					onAudioLoaded(audioclip);
				}
			}
		}

		public static void LoadTexture2D(string texturePath, RawImage rawImage, FilterMode filterMode = FilterMode.Bilinear)
		{
			Uri fileUri = new Uri(texturePath);
			WebClient client = new WebClient();
			byte[] raw = client.DownloadData(fileUri);
			Texture2D tx = new Texture2D(1, 1);
			tx.filterMode = filterMode;
			tx.LoadImage(raw);
			float aspectRatio = (float)tx.width / (float)tx.height;
			if (rawImage.gameObject.GetComponent<AspectRatioFitter>() != null)
			{
				rawImage.gameObject.GetComponent<AspectRatioFitter>().aspectRatio = aspectRatio;
			}
			rawImage.texture = tx;
		}

		

		private static void SetAspectRatio(RawImage rawImage, float width, float height)
		{
			float aspectRatio = (float)width / (float)height;
			try
			{
				rawImage.gameObject.GetComponent<AspectRatioFitter>().aspectRatio = aspectRatio;
			}
			catch
			{
				Debug.Log("LoadTexture2D: <color=yellow>No Aspect Ratio Component</color>");
			}
		}
	}
}