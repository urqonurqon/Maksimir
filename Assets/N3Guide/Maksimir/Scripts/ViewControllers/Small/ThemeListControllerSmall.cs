using Novena.DAL.Model.Guide;
using Novena.DAL;
using Novena.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ThemeListControllerSmall : ThemeListController {

	[SerializeField] private Transform _mainMenuGreenContainer;
	public override void GenerateMainMenu()
	{
		UnityHelper.DestroyObjects(_menuButtonList);

		var themeList = Data.TranslatedContent.GetThemesExcludeByTag("SYSTEM");
		themeList = themeList.Where(t => !t.ContainsTag("TagNoSmall")).ToList();

		themeList.Add(Data.TranslatedContent.GetThemeByLanguageSwitchCode(5062).GetSubThemeByLanguageSwitchCode(50621));


		for (int i = 0; i < themeList.Count; i++)
		{
			GameObject go = null;
			MenuButton mb = null;
			Theme theme = themeList[i];

			if (theme.Label != "Green")
				go = Instantiate(_mainMenuBtnPrefabWhite, _mainMenuContainer);
			else
			{
				if (!theme.ContainsTag("Extend"))
					go = Instantiate(_mainMenuBtnPrefabGreen, _mainMenuGreenContainer);
				else
					go = Instantiate(_mainMenuBtnPrefabGreenExtend, _mainMenuGreenContainer);
			}



			mb = go.GetComponent<MenuButton>();
			if (mb.ColorToggle != null)
				mb.ColorToggle.Toggle.group = _colorToggleGroup;

			mb.SetButton(theme);


			go.SetActive(true);

			_menuButtonList.Add(go);

		}
	}
}
