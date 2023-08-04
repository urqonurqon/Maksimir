using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(GallerySnap), true)]
public class GalleryCustomEditor : Editor {
	//private bool isActive;

	//private static GUIStyle _toggleButtonStyleNormal = null;
	//private static GUIStyle _toggleButtonStyleToggled = null;

	public override void OnInspectorGUI()
	{
		GallerySnap gallery = (GallerySnap)target;

		EditorGUILayout.LabelField("Choose gallery type");

		EditorGUILayout.BeginHorizontal();

		if (GUILayout.Button("Horizontal gallery"))
		{
			gallery.SetGalleryHorizontal();
		}
		if (GUILayout.Button("Vertical gallery"))
		{
			gallery.SetGalleryVertical();
		}

		EditorGUILayout.EndHorizontal();
		DrawDefaultInspector();
	}
}
#endif