using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;
using UnityEngine.Windows;
using JetBrains.Annotations;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(OnScreenKeyboard))]
public class ObjectBuilderEditor : Editor {
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		var s = (OnScreenKeyboard)target;
		if (GUILayout.Button("Check all tmpro"))
		{
			for (int i = 0; i < s.specialKeys.Length; i++)
			{
				Text b;
				for (int c = 0; c < s.specialKeys[i].transform.childCount; c++)
					if (s.specialKeys[i].transform.GetChild(c).TryGetComponent<Text>(out b))
					{
						var t = b.text;
						var f = b.fontSize;
						DestroyImmediate(b);
						var tm = s.specialKeys[i].transform.GetChild(c).gameObject.AddComponent<TextMeshProUGUI>();
						tm.text = t;
						tm.fontSize = f;
					}
			}
		}
	}
}


#endif

[DefaultExecutionOrder(1)]
public class OnScreenKeyboard : MonoBehaviour {

	public InputFieldCustom focus;
	public bool showNumeric = false;
	public Color32 textColor;
	public Color32 mainColor;
	public Color32 specialColor;
	public Color32 backgroundColor;
	public Sprite mainSprite;
	public Sprite specialSprite;
	public GameObject[] panels;
	public GameObject[] keys;
	public GameObject[] specialKeys;

	[HideInInspector]
	public bool isActive = false;
	[HideInInspector]
	public bool capsEnabled = false;

	void Start()
	{
		ShowNumeric(showNumeric);
		SetTextColor(textColor, backgroundColor);
		SetMainColor(mainColor);
		SetSpecialColor(specialColor);
		SetBackgroundColor(backgroundColor);
		//SetMainSprite(mainSprite);
		//SetSpecialSprite(specialSprite);
	}

	public void ShowNumeric(bool b)
	{
		panels[3].SetActive(b);
		showNumeric = b;
	}

	public void SetTextColor(Color32 c, Color32 c2)
	{
		foreach (GameObject go in keys)
		{
			go.transform.Find("Text").GetComponent<TextMeshProUGUI>().color = c;
		}

		foreach (GameObject go in specialKeys)
		{
			if (go.transform.Find("Text") != null)
				go.transform.Find("Text").GetComponent<TextMeshProUGUI>().color = c2;
			if (go.transform.Find("Image") != null)
				go.transform.Find("Image").GetComponent<Image>().color = c2;
		}
	}

	public void SetMainColor(Color32 c)
	{
		foreach (GameObject go in keys)
		{
			go.GetComponent<Image>().color = c;
		}
	}

	public void SetSpecialColor(Color32 c)
	{
		foreach (GameObject go in specialKeys)
		{
			go.GetComponent<Image>().color = c;
		}
	}



	public void SetBackgroundColor(Color32 c)
	{
		gameObject.GetComponent<Image>().color = c;
	}

	public void SetMainSprite(Sprite s)
	{
		foreach (GameObject go in keys)
		{
			go.GetComponent<Image>().sprite = s;
		}
	}

	public void SetSpecialSprite(Sprite s)
	{
		foreach (GameObject go in specialKeys)
		{
			go.GetComponent<Image>().sprite = s;
		}
	}

	public void SetFocus(InputFieldCustom i)
	{
		focus = i;
	}

	public void SetActiveFocus(InputFieldCustom i)
	{
		focus = i;
		//SetActive(true);
		focus.MoveTextEnd(true);
	}

	public void WriteKey(TextMeshProUGUI t)
	{
		StartCoroutine(WriteKeyRoutine(t));
	}

	private IEnumerator WriteKeyRoutine(TextMeshProUGUI t)
	{
		var currentCaretPos = focus.caretPosition;
		EventSystem system = EventSystem.current;
		yield return new WaitForEndOfFrame();
		if (focus)
		{
			focus.text = focus.text.Insert(focus.caretPosition, t.text);
			focus.caretPosition = currentCaretPos + 1;
			//SetCarretAlpha(false);
			//SetCarretAlpha(true);

			system.SetSelectedGameObject(focus.gameObject);
		}
	}
	//private IEnumerator PrevChar()
	//{
	//	EventSystem system = EventSystem.current;
	//	yield return new WaitForEndOfFrame();
	//	if (focus != null)
	//		focus.caretPosition--;
	//	system.SetSelectedGameObject(focus.gameObject);
	//}

	private void LateUpdate()
	{
		if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject == focus)
		{
			focus.GetType().GetField("m_AllowInput", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(focus, true);
			focus.GetType().InvokeMember("SetCaretVisible", BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance, null, focus, null);
		}
	}
	public void SetCarretVisible()
	{
		focus.Select();
		InputFieldCustom inputField = focus;
		//inputField.caretPosition = focus.text.Length; // desired cursor position

		inputField.GetType().GetField("m_AllowInput", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(inputField, true);
		inputField.GetType().InvokeMember("SetCaretVisible", BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance, null, inputField, null);

	}

	public void SetCarretAlpha(bool visible)
	{
		focus.caretColor = new Color(focus.caretColor.r, focus.caretColor.g, focus.caretColor.b, visible ? 1 : 0);
		//focus.GetType().GetField("m_AllowInput", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(focus, visible);
	}

	public void WriteSpecialKey(int n)
	{
		switch (n)
		{
			case 0:
				StartCoroutine(Backspace());
				//if (!focus) { return; }
				//Debug.Log(focus.selectionFocusPosition);
				//if (focus.text.Length > 0)
				//{
				//	var t = "";
				//	var t2 = "";

				//	if (focus.text.Length == focus.selectionStringFocusPosition - 1 || focus.selectionStringFocusPosition == 0)
				//		t = focus.text.Substring(0, focus.text.Length - 1);
				//	else if (focus.selectionStringFocusPosition - 2 < 0 || focus.selectionStringFocusPosition - 1 < 0)
				//	{
				//		break;
				//	}
				//	else
				//	{
				//		t = focus.text.Substring(0, focus.selectionStringFocusPosition - 2);
				//		t2 = focus.text.Substring(focus.selectionStringFocusPosition - 1);
				//	}
				//	focus.text = t + t2;
				//}
				break;
			case 1:
				EventSystem system;
				system = EventSystem.current;
				if (!focus) { return; }
				focus.OnSubmit(new PointerEventData(system));
				break;
			case 2:
				SwitchCaps();
				break;
			case 3:
				//SetActive(false);
				break;
			case 4:
				SetKeyboardType(1);
				break;
			case 5:
				SetKeyboardType(2);
				break;
			case 6:
				FocusPrevious();
				break;
			case 7:
				FocusNext();
				break;
			case 8:
				SetKeyboardType(0);
				break;
			case 9:
				StartCoroutine(PrevChar());
				break;
			case 10:
				StartCoroutine(NextChar());
				break;
			case 11:
				if (!focus) { return; }
				//focus.text += "\n";
				//focus.caretPosition++;
				StartCoroutine(WriteNewLineRoutine());
				break;
		}
	}

	private IEnumerator WriteNewLineRoutine()
	{
		if (focus.lineType == InputFieldCustom.LineType.SingleLine) yield break;
		var currentCaretPos = focus.caretPosition;
		EventSystem system = EventSystem.current;
		yield return new WaitForEndOfFrame();
		if (focus)
		{
			focus.text = focus.text.Insert(focus.caretPosition, "\n");
			focus.caretPosition = currentCaretPos + 1;
			//SetCarretAlpha(false);
			//SetCarretAlpha(true);

			system.SetSelectedGameObject(focus.gameObject);
		}
	}


	private IEnumerator Backspace()
	{
		var currentCaretPos = focus.caretPosition;
		EventSystem system = EventSystem.current;
		yield return new WaitForEndOfFrame();
		if (!focus) { yield break; }
		if (focus.text.Length == 0) { yield break; }


		if (focus.caretPosition != 0)
		{
			focus.text = focus.text.Remove(focus.caretPosition - 1, 1);
			focus.caretPosition = currentCaretPos - 1;
		}
		//SetCarretVisible();
		system.SetSelectedGameObject(focus.gameObject);
	}


	private IEnumerator NextChar()
	{
		EventSystem system = EventSystem.current;
		yield return new WaitForEndOfFrame();
		if (focus != null)
			focus.caretPosition++;
		system.SetSelectedGameObject(focus.gameObject);
	}

	private IEnumerator PrevChar()
	{
		EventSystem system = EventSystem.current;
		yield return new WaitForEndOfFrame();
		if (focus != null)
			focus.caretPosition--;
		system.SetSelectedGameObject(focus.gameObject);
	}

	public void SetActive(bool b)
	{
		if (b)
		{
			if (!isActive)
			{
				gameObject.GetComponent<Animator>().Rebind();
				gameObject.GetComponent<Animator>().enabled = true;
			}
		}
		else
		{
			if (isActive)
			{
				gameObject.GetComponent<Animator>().SetBool("Hide", true);
			}
		}

		isActive = b;
	}

	public void SetCaps(bool b)
	{
		if (b)
		{
			foreach (GameObject go in keys)
			{
				var t = go.transform.Find("Text").GetComponent<TextMeshProUGUI>();
				t.text = t.text.ToUpper();
			}
		}
		else
		{
			foreach (GameObject go in keys)
			{
				var t = go.transform.Find("Text").GetComponent<TextMeshProUGUI>();
				t.text = t.text.ToLower();
			}
		}
		capsEnabled = b;
	}

	public void SwitchCaps()
	{
		SetCaps(!capsEnabled);
	}

	public void FocusPrevious()
	{
		EventSystem system;
		system = EventSystem.current;

		if (!focus) { return; }

		Selectable current = focus.GetComponent<Selectable>();
		Selectable next = current.FindSelectableOnLeft();
		if (!next)
		{
			next = current.FindSelectableOnUp();
		}
		if (!next)
		{
			return;
		}
		InputFieldCustom inputfield = next.GetComponent<InputFieldCustom>();
		if (inputfield != null)
		{
			inputfield.OnPointerClick(new PointerEventData(system));
			focus = inputfield;
		}
		system.SetSelectedGameObject(next.gameObject);
	}

	public void FocusNext()
	{
		EventSystem system;
		system = EventSystem.current;

		if (!focus) { return; }

		Selectable current = focus.GetComponent<Selectable>();
		Selectable next = current.FindSelectableOnRight();
		if (!next)
		{
			next = current.FindSelectableOnDown();
		}
		if (!next)
		{
			return;
		}
		InputFieldCustom inputfield = next.GetComponent<InputFieldCustom>();
		if (inputfield != null)
		{
			inputfield.OnPointerClick(new PointerEventData(system));
			focus = inputfield;
		}
		system.SetSelectedGameObject(next.gameObject);
	}

	public void SetKeyboardType(int n)
	{
		switch (n)
		{
			case 0:
				panels[0].SetActive(true);
				panels[1].SetActive(false);
				panels[2].SetActive(false);
				break;
			case 1:
				panels[0].SetActive(false);
				panels[1].SetActive(true);
				panels[2].SetActive(false);
				break;
			case 2:
				panels[0].SetActive(false);
				panels[1].SetActive(false);
				panels[2].SetActive(true);
				break;
		}
	}

}
