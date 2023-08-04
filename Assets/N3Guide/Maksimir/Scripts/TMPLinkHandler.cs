using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TMPLinkHandler : MonoBehaviour, IPointerClickHandler {

	[SerializeField] private TMP_Text _linkText;
	[SerializeField] private Camera _camera;

	private TMP_LinkInfo _linkInfo;

	public static Action OnAllOfferClicked;

	private void Awake()
	{
		_camera = Camera.main;
		_linkText = GetComponent<TMP_Text>();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		int linkIndex = TMP_TextUtilities.FindIntersectingLink(_linkText, Input.mousePosition, _camera);
		if (linkIndex != -1)
			_linkInfo = _linkText.textInfo.linkInfo[linkIndex];

		LinkCases(_linkInfo.GetLinkID());
	}


	/// <summary>
	/// Atach this script to a TMP_Text that has a /<link/> tag and add a case here to execute commands when a link with a specific ID is clicked.
	/// </summary>
	/// <param name="linkID"></param>
	private void LinkCases(string linkID)
	{
		switch (linkID)
		{
			case "AllOffer":
				OnAllOfferClicked?.Invoke();
				break;


			default:
				break;
		}

	}
}
