using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Components.PinchScrollRect {
	public class PinchableScrollRect : ScrollRect {

		public static Action<Vector3> OnZoom;

		[SerializeField] public float _minZoom = 1f;
		[SerializeField] public float _maxZoom = 11f;
		[SerializeField] public float _zoomLerpSpeed = 10f;
		public float _currentZoom = 1;
		public bool _isPinching = false;
		private float _startPinchDist;
		private float _startPinchZoom;
		public Vector2 _startPinchCenterPosition;
		public Vector2 _startPinchScreenPosition;
		private float _mouseWheelSensitivity = 1;
		private bool blockPan = false;
		private new Camera camera;

		public bool activate = true;

		public TMP_Text debug;

		public int touches = 0;

		protected override void Awake()
		{
			Input.multiTouchEnabled = true;
			_minZoom = 1f;
			_maxZoom = 11f;
			camera = FindObjectOfType<AudioListener>().GetComponent<Camera>();
		}

		public void restart()
		{
			//debug.text = "restart<br>";

			string[] endings = new string[] { "exe", "x86", "x86_64", "app" };
			string executablePath = Application.dataPath + "/..";
			foreach (string file in System.IO.Directory.GetFiles(executablePath))
			{
				foreach (string ending in endings)
				{
					//debug.text += file.ToLower() + " : " + file.ToLower().EndsWith("." + ending) + "<br>";
					if (file.ToLower().EndsWith("." + ending))
					{
						//debug.text += (file) + "<br>";
						System.Diagnostics.Process.Start(file);
						Application.Quit();
						return;
					}
				}
			}
		}

		private void Update()
		{
			if (Input.touches.Length > 10)
			{
				//restart();
			}

			if (Input.GetMouseButton(0))
			{
				//restart();
			}


			if (Input.touches.Length == 2)
			{
				//debug.text = "Input.touches.Length >= 2: " + Input.touches.Length + "<br>";
				//debug.text += "_isPinching: " + _isPinching + "<br>";

				if (!_isPinching)
				{
					_isPinching = true;
					OnPinchStart();
				}
				OnPinch();
			}
			else
			{
				_isPinching = false;
				if (Input.touches.Length == 0)
				{
					blockPan = false;
				}
			}
			//pc input

			float scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");
			if (Mathf.Abs(scrollWheelInput) > float.Epsilon)
			{
				//debug.text = "_currentZoom: " + _currentZoom;
				_currentZoom *= 1 + scrollWheelInput * _mouseWheelSensitivity;
				_currentZoom = Mathf.Clamp(_currentZoom, _minZoom, _maxZoom);
				_startPinchScreenPosition = (Vector2)Input.mousePosition;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(content, _startPinchScreenPosition, camera, out _startPinchCenterPosition);
				Vector2 pivotPosition = new Vector3(content.pivot.x * content.rect.size.x, content.pivot.y * content.rect.size.y);
				Vector2 posFromBottomLeft = pivotPosition + _startPinchCenterPosition;
				SetPivot(content, new Vector2(posFromBottomLeft.x / content.rect.width, posFromBottomLeft.y / content.rect.height));
			}

			//pc input end

			if (Mathf.Abs(content.localScale.x - _currentZoom) > 0.001f)
			{
				content.localScale = Vector3.Lerp(content.localScale, Vector3.one * _currentZoom, _zoomLerpSpeed * Time.deltaTime);
				OnZoom?.Invoke(content.localScale);
			}
		}


		protected override void SetContentAnchoredPosition(Vector2 position)
		{
			if (_isPinching || blockPan) return;
			base.SetContentAnchoredPosition(position);
		}

		private void OnPinchStart()
		{
			//debug.text += "OnPinchStart: " + Input.touches[0].position + ":" + Input.touches[1].position + "<br>";

			Vector2 pos1 = Input.touches[0].position;
			Vector2 pos2 = Input.touches[1].position;

			_startPinchDist = Distance(pos1, pos2) * content.localScale.x;
			_startPinchZoom = _currentZoom;
			_startPinchScreenPosition = (pos1 + pos2) / 2;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(content, _startPinchScreenPosition, camera, out _startPinchCenterPosition);

			Vector2 pivotPosition = new Vector3(content.pivot.x * content.rect.size.x, content.pivot.y * content.rect.size.y);
			Vector2 posFromBottomLeft = pivotPosition + _startPinchCenterPosition;

			SetPivot(content, new Vector2(posFromBottomLeft.x / content.rect.width, posFromBottomLeft.y / content.rect.height));
			blockPan = true;
		}

		public void SetItemPivot(float x, float y)
		{
			_startPinchScreenPosition = new Vector2(x, y);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(content, _startPinchScreenPosition, camera, out _startPinchCenterPosition);
			Vector2 pivotPosition = new Vector3(content.pivot.x * content.rect.size.x, content.pivot.y * content.rect.size.y);
			Vector2 posFromBottomLeft = pivotPosition + _startPinchCenterPosition;
			SetPivot(content, new Vector2(posFromBottomLeft.x / content.rect.width, posFromBottomLeft.y / content.rect.height));
		}

		private void OnPinch()
		{
			float currentPinchDist = Distance(Input.touches[0].position, Input.touches[1].position) * content.localScale.x;
			_currentZoom = (currentPinchDist / _startPinchDist) * _startPinchZoom;
			_currentZoom = Mathf.Clamp(_currentZoom, _minZoom, _maxZoom);
			//debug.text += "OnPinch _currentZoom: " + _currentZoom + "<br>";
		}

		private float Distance(Vector2 pos1, Vector2 pos2)
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(content, pos1, camera, out pos1);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(content, pos2, camera, out pos2);
			return Vector2.Distance(pos1, pos2);
		}

		private static void SetPivot(RectTransform rectTransform, Vector2 pivot)
		{
			if (rectTransform == null) return;

			Vector2 size = rectTransform.rect.size;
			Vector2 deltaPivot = rectTransform.pivot - pivot;
			Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y) * rectTransform.localScale.x;
			rectTransform.pivot = pivot;
			rectTransform.localPosition -= deltaPosition;
		}

		public void ChangeZoom(float zoomFactor)
		{
			Vector2 pos1 = new Vector2(0f, 0f);
			Vector2 pos2 = new Vector2(1080f, 1920f);

			_startPinchDist = Distance(pos1, pos2) * content.localScale.x;
			_startPinchZoom = _currentZoom;
			_startPinchScreenPosition = (pos1 + pos2) / 2;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(content, _startPinchScreenPosition, camera, out _startPinchCenterPosition);

			Vector2 pivotPosition = new Vector3(content.pivot.x * content.rect.size.x, content.pivot.y * content.rect.size.y);
			Vector2 posFromBottomLeft = pivotPosition + _startPinchCenterPosition;

			SetPivot(content, new Vector2(posFromBottomLeft.x / content.rect.width, posFromBottomLeft.y / content.rect.height));

			_currentZoom += zoomFactor;

			if (_currentZoom > _maxZoom)
			{
				_currentZoom = _maxZoom;
			}

			if (_currentZoom < _minZoom)
			{
				_currentZoom = _minZoom;
			}
		}

		public void ResetPosition()
		{
			_currentZoom = 1f;
			content.offsetMin = new Vector2(0, 0);
			content.offsetMax = new Vector2(0, 0);
			content.localScale = new Vector3(1, 1, 1);
		}
	}
}