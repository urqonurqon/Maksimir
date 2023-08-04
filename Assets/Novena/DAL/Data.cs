using System;
using Novena.DAL.Model.Guide;

namespace Novena.DAL
{
    /// <summary>
    /// Utility class for storing objects in use (Guide, Theme...)
    /// </summary>
    public static class Data
    {
        #region Events

        /// <summary>
        /// Invoked when translated content is changed!
        /// </summary>
        public static Action OnTranslatedContentUpdated;

        #endregion

        /// <summary>
        /// Current guide type. Long (true) or short.
        /// </summary>
        public static bool GuideType { get; set; }

        public static Guide Guide { get; set; }


        /// <summary>
        /// Current translated content (language).
        /// <para>
        /// Use this to store and get current selected language.
        /// </para>
        /// <example>
        /// When user click language button store that in here.
        /// </example>
        /// </summary>
        public static TranslatedContent TranslatedContent
        {
            get { return s_translatedContent; }
            set
            {
                /*if (s_translatedContent != null && value.Id != s_translatedContent.Id)
                {
                  s_translatedContent = value;
                  OnTranslatedContentUpdated?.Invoke();
                }*/
                s_translatedContent = value;
                OnTranslatedContentUpdated?.Invoke();
            }
        }

        /// <summary>
        /// Current theme.
        /// <para>
        /// Use this to store and get current theme.
        /// </para>
        /// <example>
        /// When user click theme in list of themes store that in here.
        /// </example>
        /// </summary>
        public static Theme Theme
        {
            get
            {
                return _currentTheme;
            }
            set
            {
                _lastTheme = _currentTheme;
                _currentTheme = value;
            }
        }

        public static Theme PreviousTheme
        {
            get { return _lastTheme; }
        }

        /// <summary>
        /// Current subtheme.
        /// <para>
        /// Use this to store and get current theme.
        /// </para>
        /// <example>
        /// When user click theme in list of themes store that in here.
        /// </example>
        /// </summary>
        public static SubTheme SubTheme
        { get; set; }


        //This is custom for VRSAR app
        /// <summary>
        /// Current detected AR media
        /// </summary>
        public static Media ArMedia { get; set; }


        #region Private fields

        private static TranslatedContent s_translatedContent;

        private static Theme _lastTheme;

        private static Theme _currentTheme;

        #endregion
    }
}