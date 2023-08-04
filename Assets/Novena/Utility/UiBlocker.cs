using UnityEngine;

namespace Novena.Utility {
	[DefaultExecutionOrder(-20000)]
	public class UiBlocker : MonoBehaviour {
		/// <summary>
		/// Singleton
		/// </summary>
		private static UiBlocker _instance { get; set; }

		private void Awake()
		{
			if (_instance == null)
				_instance = this;
			Disable();
		}

		/// <summary>
		/// Enable UiBlocker
		/// </summary>
		public static void Enable()
		{
			_instance.gameObject.SetActive(true);
		}

		/// <summary>
		/// Disable UiBlocker
		/// </summary>
		public static void Disable()
		{
			_instance.gameObject.SetActive(false);
		}

	}
}