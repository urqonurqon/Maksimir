using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;


[DefaultExecutionOrder(-1000)]
public static class NewsData {


	private static List<News> _newsData;

	public static List<News> Happenings { get; set; }
	public static List<News> Actualities { get; set; }
	public static List<News> Warnings { get; set; }



	public static async UniTask GetNews()
	{
		Happenings = new List<News>();
		Actualities = new List<News>();
		Warnings = new List<News>();
		await GetNewsJson();
		FilterNews();
	}


	private static async UniTask GetNewsJson()
	{

		try
		{
			var www = UnityWebRequest.Get(CMSBaseManager.GetCMSPath() + "get-news.ashx");
			www.timeout = 5;
			string newsJson = (await www.SendWebRequest()).downloadHandler.text;
			_newsData = JsonConvert.DeserializeObject<List<News>>(newsJson);
			if (!Directory.Exists(Application.persistentDataPath + "/CMSJsons"))
			{
				Directory.CreateDirectory(Application.persistentDataPath + "/CMSJsons");
			}
			File.WriteAllText(Application.persistentDataPath + "/CMSJsons" + "/CachedNews.txt", newsJson);
		}
		catch (Exception e)
		{
			Debug.Log(e);
			_newsData = JsonConvert.DeserializeObject<List<News>>(File.ReadAllText(Application.persistentDataPath + "/CMSJsons" + "/CachedNews.txt"));
		}

	}


	public static async UniTask GetWarningJson()
	{
		List<News> warningsUnfiltered = null;
		try
		{
			var www = UnityWebRequest.Get(CMSBaseManager.GetCMSPath() + "get-news.ashx?catid=3");
			www.timeout = 5;
			string newsJson = (await www.SendWebRequest()).downloadHandler.text;
			warningsUnfiltered = JsonConvert.DeserializeObject<List<News>>(newsJson);
			if (!Directory.Exists(Application.persistentDataPath + "/CMSJsons"))
			{
				Directory.CreateDirectory(Application.persistentDataPath + "/CMSJsons");
			}
			File.WriteAllText(Application.persistentDataPath + "/CMSJsons" + "/CachedWarning.txt", newsJson);
		}
		catch (Exception e)
		{
			Debug.Log(e);
			warningsUnfiltered = JsonConvert.DeserializeObject<List<News>>(File.ReadAllText(Application.persistentDataPath + "/CMSJsons" + "/CachedWarning.txt"));
		}

		if (Warnings != null)
			Warnings.Clear();
		else
			Warnings = new List<News>();
		for (int i = 0; i < warningsUnfiltered.Count; i++)
		{
			if (Warnings.Any(w => w.LanguageCode == warningsUnfiltered[i].LanguageCode)) continue;
			Warnings.Add(warningsUnfiltered[i]);
		}
	}



	private static void FilterNews()
	{
		for (int i = 0; i < _newsData.Count; i++)
		{
			switch (_newsData[i].CategoryId)
			{
				case 2:
					Actualities.Add(_newsData[i]);
					break;
				case 3:
					Happenings.Add(_newsData[i]);
					break;

				case 10:
					Actualities.Add(_newsData[i]);
					break;
				case 9:
					Happenings.Add(_newsData[i]);
					break;

				case 12:
					Actualities.Add(_newsData[i]);
					break;
				case 13:
					Happenings.Add(_newsData[i]);
					break;

				case 15:
					Actualities.Add(_newsData[i]);
					break;
				case 16:
					Happenings.Add(_newsData[i]);
					break;
				default:
					Debug.Log("no id");
					break;
			}
		}
	}

	//private static News CheckWarning(News lastWarning, News currentWarning)
	//{
	//	if (lastWarning == null)
	//	{
	//		lastWarning = currentWarning;
	//		Warnings.Add(lastWarning);
	//	}
	//	if (currentWarning.LanguageCode != lastWarning.LanguageCode) Warnings.Add(lastWarning);
	//	return currentWarning;
	//}


}




[Serializable]
public class News {
	public int Id { get; set; }
	public int CategoryId { get; set; }
	public string LanguageCode { get; set; }
	public string Title { get; set; }
	public string ImagePath { get; set; }
	public string Content { get; set; }
	public DateTime Date { get; set; }
	public string IntroText { get; set; }
	public string QrCode { get; set; }
	public string ProgramSchedule { get; set; }
	public string Dates { get; set; }
	public List<string> Photos { get; set; }
	public bool IsFrontPage { get; set; }

}