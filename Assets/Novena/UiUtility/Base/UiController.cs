using Doozy.Engine.UI;
using Novena.Utility;
using UnityEngine;

namespace Novena.UiUtility.Base {
	/// <summary>
	/// Base class for doozy UiView event implementation.
	/// This class is standard for every controller that handles Ui.
	/// <example>
	/// Every script inheriting this must be on object with UiView component.
	/// </example>
	/// </summary>
	[RequireComponent(typeof(UIView))]
	public class UiController : MonoBehaviour {
		/// <summary>
		/// UiView component.
		/// </summary>
		[HideInInspector]
		public UIView UiView;

		public virtual void Awake()
		{
			UiView = gameObject.GetComponent<UIView>();

			UiView.ShowBehavior.OnStart.Event.RemoveListener(OnShowViewStart);
			UiView.ShowBehavior.OnStart.Event.AddListener(OnShowViewStart);

			UiView.ShowBehavior.OnFinished.Event.RemoveListener(OnShowViewFinished);
			UiView.ShowBehavior.OnFinished.Event.AddListener(OnShowViewFinished);

			UiView.HideBehavior.OnStart.Event.RemoveListener(OnHideViewStart);
			UiView.HideBehavior.OnStart.Event.AddListener(OnHideViewStart);

			UiView.HideBehavior.OnFinished.Event.RemoveListener(OnHideViewFinished);
			UiView.HideBehavior.OnFinished.Event.AddListener(OnHideViewFinished);

			UiView.OnVisibilityChanged.RemoveListener(OnVisibilityChanged);
			UiView.OnVisibilityChanged.AddListener(OnVisibilityChanged);

			UiView.OnInverseVisibilityChanged.RemoveListener(OnInverseVisibilityChanged);
			UiView.OnInverseVisibilityChanged.AddListener(OnInverseVisibilityChanged);
		}

		/// <summary>
		/// ProgressEvent executed when the view is animating (showing or hiding) and the progress has been updated.
		/// </summary>
		/// <param name="arg0">Passes the InverseVisibility (float between 1 – NotVisible and 0 – Visible). InverseVisibility = 1 – Visibility</param>
		public virtual void OnInverseVisibilityChanged(float arg0)
		{

		}

		/// <summary>
		/// ProgressEvent executed when the view is animating (showing or hiding) and the progress has been updated.
		/// </summary>
		/// <param name="arg0">Passes the Visibility (float between 0 – NotVisible and 1 – Visible)</param>
		public virtual void OnVisibilityChanged(float arg0)
		{

		}

		/// <summary>
		/// Doozy event when View Show animation start.
		/// </summary>
		public virtual void OnShowViewStart()
		{

			UiBlocker.Enable();
			Resources.UnloadUnusedAssets();
		}

		/// <summary>
		/// Doozy event when View Show animation finish.
		/// </summary>
		public virtual void OnShowViewFinished()
		{
		}

		/// <summary>
		/// Doozy event when View Hide animation start.
		/// </summary>
		public virtual void OnHideViewStart()
		{

		}

		/// <summary>
		/// Doozy event when View Hide animation finish.
		/// </summary>
		public virtual void OnHideViewFinished()
		{

			UiBlocker.Disable();
		}
	}
}