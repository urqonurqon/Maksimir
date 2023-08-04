using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;

public static class TimeWeather {


	private static Weather _weather = new Weather();


	public static async UniTask GetWeather()
	{
		try
		{
			var uwr = UnityWebRequest.Get("http://api.weatherapi.com/v1/current.json?key=b529449316484115b4e71929222704&q=Zagreb&aqi=no");
			uwr.timeout = 5;
			string weatherJson = (await uwr.SendWebRequest()).downloadHandler.text;
			_weather = JsonConvert.DeserializeObject<Weather>(weatherJson);
		}
		catch (Exception e)
		{
			Debug.Log(e);
			_weather = null;
		}
	}


	//public static string GetTime()
	//{
	//	string output = null;
	//	output = _weather.location.localtime;
	//	output = output.Split(" ")[1] + " h";
	//	return output;
	//}
	//public static string GetDate()
	//{
	//	string output = null;
	//	output = _weather.location.localtime;
	//	output = output.Split(" ")[0];
	//	output = output.Replace("-", ".");
	//	output = output.Split(".")[2] + "." + output.Split(".")[1] + "." + output.Split(".")[0] + ".";
	//	return output;
	//}
	public static string GetTemperature()
	{
		if (_weather == null) return null;
		string output = null;
		if (_weather.current != null)
			output = Mathf.RoundToInt(_weather.current.temp_c) + "°C";
		return output;
	}
	public static string GetHumidity()
	{
		if (_weather == null) return null;
		string output = null;
		if (_weather.current != null)
			output = _weather.current.humidity + "%";
		return output;
	}
	public static string GetIcon()
	{
		if (_weather == null) return null;
		string output = null;
		if (_weather.current != null)
			output = _weather.current.condition.icon;
		return output;
	}

}
