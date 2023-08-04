using UnityEngine;
using System;

public class SimpleRotateSphere : MonoBehaviour {
	private const int RMB_ID = 0;
	public Transform Camera360;
	private Vector2? _rmbPrevPos;
	private float _x;
	private float _y;

	[Range(1, 100)]
	[SerializeField]
	private float _rotationSpeed = 4;

	private bool _isAbove;

	//void Awake()
	//{
	//_cachedTransform = this.GetComponent<Camera>().transform;
	//}

	public void AboveObject(bool isAbove)
	{
		_isAbove = isAbove;
	}

	private void Update()
	{
		if (_isAbove)
			TrackRotation();
	}

	private void TrackRotation()
	{
		if (_rmbPrevPos.HasValue)
		{
			if (Input.GetMouseButton(RMB_ID))
			{
				if (((int)_rmbPrevPos.Value.x != (int)Input.mousePosition.x)
						|| ((int)_rmbPrevPos.Value.y != (int)Input.mousePosition.y))
				{
					_x += (_rmbPrevPos.Value.y - Input.mousePosition.y) * Time.deltaTime * _rotationSpeed;
					_y -= (_rmbPrevPos.Value.x - Input.mousePosition.x) * Time.deltaTime * _rotationSpeed;
					Camera360.rotation = Quaternion.Euler(_x, _y, 0);
					_rmbPrevPos = Input.mousePosition;
				}
			}
			else
			{
				_rmbPrevPos = null;
			}
		}
		else if (Input.GetMouseButtonDown(RMB_ID))
		{
			_rmbPrevPos = Input.mousePosition;
		}
	}

	private void ResetPosition(float rotation)
	{
		Debug.Log("Reset");
		_x = 0;
		_y = rotation;
		_rmbPrevPos = null;
	}
}
