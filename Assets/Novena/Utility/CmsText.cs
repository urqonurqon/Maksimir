using Novena.DAL;
using Novena.Enumerators;
using TMPro;
using UnityEngine;

namespace Novena.Utility {
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class CmsText : MonoBehaviour {
		[SerializeField] public int _languageSwitchCode;

		[SerializeField] public string _mediaName;

		private TMP_Text _textMeshPro;

		private void Awake()
		{
			_textMeshPro = GetComponent<TMP_Text>();
			Data.OnTranslatedContentUpdated -= OnTranslatedContentUpdated;
			Data.OnTranslatedContentUpdated += OnTranslatedContentUpdated;

			//For CmsText objects that are enabled after OnTranslatedContentUpdate triggered!
			if (Data.TranslatedContent != null)
			{
				SetText();
			}
		}

		/// <summary>
		/// When language is changed lets update cmsText
		/// </summary>
		private void OnTranslatedContentUpdated()
		{
			SetText();
		}

		private void SetText()
		{
			_textMeshPro.text = "";

			var theme = Data.TranslatedContent.GetThemeByLanguageSwitchCode(_languageSwitchCode);

			if (theme == null)
			{
				foreach (var themes in Data.TranslatedContent.Themes)
				{
					theme = themes.GetSubThemeByLanguageSwitchCode(_languageSwitchCode);
				}

				if (theme == null) return;
			}
			var media = _mediaName == "" ? theme.GetMediaByType(MediaType.Text) : theme.GetMediaByName(_mediaName);

			if (media != null)
			{
				_textMeshPro.text = media.Text;
			}
		}
	}
}