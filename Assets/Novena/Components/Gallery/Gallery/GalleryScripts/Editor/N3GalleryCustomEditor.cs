using Doozy.Engine.UI;
using Novena.Components.Gallery.Gallery.GalleryScripts;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script for "Gallery" Custom editor
/// Should be placed in folder named "Editor"
/// </summary>
[CustomEditor(typeof(N3Gallery))]
public class N3GalleryCustomEditor : Editor {

	private bool imageTransitionOptions;

	private int	 tab;

	override public void OnInspectorGUI()
	{
		var gallery = target as N3Gallery;

		tab = GUILayout.Toolbar(tab, new string[] { "Settings", "Components" });
		switch (tab)
		{
			case 0:
				gallery.AutomaticImageChangeSpeed = EditorGUILayout.FloatField(new GUIContent("AutomaticImageChangeSpeed", "Time representing automatic image changing speed"), gallery.AutomaticImageChangeSpeed);
				if (gallery.AutomaticImageChangeSpeed < 0.0f)
				{
					gallery.AutomaticImageChangeSpeed = 0.0f;
				}
				// change Image Settings
				#region SlideShow Settings - Section
				EditorGUILayout.Space(15);
				imageTransitionOptions = EditorGUILayout.BeginFoldoutHeaderGroup(imageTransitionOptions, "Image Transition");
				if (imageTransitionOptions)
					if (Selection.activeTransform)
					{
						gallery.UseDefault = GUILayout.Toggle(gallery.UseDefault, "Use default image transition");
						EditorGUILayout.Space(3);
						bool defaultSettings = true;
						gallery.UseOnePageSlideShow = GUILayout.Toggle(gallery.UseOnePageSlideShow, "Use one page slideShow");
						if (gallery.UseOnePageSlideShow)
						{
							defaultSettings = false;
							if (gallery.UseTwoPagesSlideShow == true)
							{
								gallery.UseTwoPagesSlideShow = false;
								gallery.UseDefault = false;
							}
						}
						gallery.UseTwoPagesSlideShow = GUILayout.Toggle(gallery.UseTwoPagesSlideShow, "Use two pages slideShow");
						if (gallery.UseTwoPagesSlideShow)
						{
							defaultSettings = false;
							if (gallery.UseOnePageSlideShow == true)
							{
								gallery.UseOnePageSlideShow = false;
								gallery.UseDefault = false;
							}
						}

						if (defaultSettings)
						{
							gallery.UseDefault = true;
						} 
						else
						{
							gallery.UseDefault = false;
						}

						// Preset Names - section
						if (gallery.UseOnePageSlideShow || gallery.UseTwoPagesSlideShow)
						{
							EditorGUILayout.Space(2);
							GuiLine(1);
							EditorGUILayout.Space(2);
							gallery.LeftShowPresetName = EditorGUILayout.TextField("LeftShow Preset Name", gallery.LeftShowPresetName);
							gallery.LeftHidePresetName = EditorGUILayout.TextField("LeftHide Preset Name", gallery.LeftHidePresetName);
							EditorGUILayout.Space(2);
							gallery.RightShowPresetName = EditorGUILayout.TextField("RightShow Preset Name", gallery.RightShowPresetName);
							gallery.RightHidePresetName = EditorGUILayout.TextField("RightHide Preset Name", gallery.RightHidePresetName);
						}
					}
				if (!Selection.activeTransform)
				{
					imageTransitionOptions = false;
				}
				EditorGUILayout.Space(10);
				EditorGUILayout.EndFoldoutHeaderGroup();
				#endregion

				// advanced settings (options)
				#region Advanced Settings - Section
				EditorGUILayout.Space(3);
				gallery.OverrideGalleryNameMediaText = GUILayout.Toggle(gallery.OverrideGalleryNameMediaText, "Override Gallery Name Media Text");

				if (gallery.OverrideGalleryNameMediaText)
				{
					gallery._OverrideGalleryNameMediaText = EditorGUILayout.TextField("Media text", gallery._OverrideGalleryNameMediaText);
				}
				EditorGUILayout.Space(3);
				#endregion
				break;

			case 1:
				#region Gallery Components - Section
				// TMP Texts
				EditorGUILayout.Space(5);
				EditorGUILayout.LabelField("TMP Texts :", EditorStyles.boldLabel);
				gallery.PagerText = (TMP_Text)EditorGUILayout.ObjectField("Pager gallery Text", gallery.PagerText, typeof(TMP_Text), true);
				// Raw Images
				EditorGUILayout.Space(5);
				EditorGUILayout.LabelField("Raw Images :", EditorStyles.boldLabel);
				gallery.ImageOne = (RawImage)EditorGUILayout.ObjectField("Image One RI", gallery.ImageOne, typeof(RawImage), true);
				gallery.ImageTwo = (RawImage)EditorGUILayout.ObjectField("Image Two RI", gallery.ImageTwo, typeof(RawImage), true);
				// UIViews
				EditorGUILayout.Space(5);
				EditorGUILayout.LabelField("UI Views :", EditorStyles.boldLabel);
				gallery.ImageOneUIView = (UIView)EditorGUILayout.ObjectField("Image One UIView", gallery.ImageOneUIView, typeof(UIView), true);
				gallery.ImageTwoUIView = (UIView)EditorGUILayout.ObjectField("Image Tvo UIView", gallery.ImageTwoUIView, typeof(UIView), true);
				// Scroll rects
				EditorGUILayout.Space(3);
				GuiLine(2);
				EditorGUILayout.LabelField("REFERENCES: ", EditorStyles.boldLabel);
				gallery.PagerContainer = (GameObject)EditorGUILayout.ObjectField("PagerContainer", gallery.PagerContainer, typeof(GameObject), true);
				gallery.ImagePlaceholderTexture = (Texture2D)EditorGUILayout.ObjectField("Placeholder texture", gallery.ImagePlaceholderTexture, typeof(Texture2D), true);
				EditorGUILayout.Space(15);
				#endregion
				break;
		}
		
		// method to draw horizontal line in editor depends on size parameter
		void GuiLine(int i_height = 1)
		{
			Rect rect = EditorGUILayout.GetControlRect(false, i_height);
			rect.height = i_height;
			EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
		}

	}
}// End of Custom Gallery Editor Class

