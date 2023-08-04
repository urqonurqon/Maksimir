using Scripts.Utility;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using Novena.UiUtility.Base;
using Cysharp.Threading.Tasks;

public class HeaderController : UiController {

	[SerializeField] private TMP_Text _date;
	[SerializeField] private TMP_Text _time;
	[SerializeField] private TMP_Text _temperature;
	[SerializeField] private TMP_Text _humidity;
	[SerializeField] private RawImage _weatherIcon;


	public async override void Awake()
	{
		await TimeWeather.GetWeather();
		StartCoroutine(RefreshTime(1f));
		StartCoroutine(RefreshWeather(1800f));
	}

	private IEnumerator RefreshTime(float refreshRate)
	{

		var datetime = DateTime.Now;

		_time.text = (datetime.Hour < 10 ? "0" : "") + datetime.Hour + ":" + (datetime.Minute < 10 ? "0" : "") + datetime.Minute;
		_date.text = (datetime.Day < 10 ? "0" : "") + datetime.Day + "/" + (datetime.Month < 10 ? "0" : "") + datetime.Month + "/" + datetime.Year;
		yield return new WaitForSeconds(refreshRate);
		StartCoroutine(RefreshTime(refreshRate));
	}

	public IEnumerator RefreshWeather(float refreshRate)
	{
		try
		{
			_temperature.text = TimeWeather.GetTemperature();
			_humidity.text = TimeWeather.GetHumidity();
			AssetsFileLoader.LoadTexture2D("https:" + TimeWeather.GetIcon(), _weatherIcon);
		}
		catch
		{
			_temperature.text = "- °C";
			_humidity.text = "- %";
		}
		TimeWeather.GetWeather().Forget();
		yield return new WaitForSeconds(refreshRate);
		StartCoroutine(RefreshWeather(refreshRate));
	}



}
