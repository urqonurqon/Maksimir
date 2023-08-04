using System;
using System.Linq;
using DG.Tweening;
using Doozy.Engine;
using Kiosk;
using Newtonsoft.Json;
using Novena.Admin.Utility;
using Novena.DAL;
using Novena.DAL.Entity;
using Novena.DAL.Model.Guide;
using Novena.UiUtility.Base;
using Novena.Utility;

namespace Novena.Controller {

	/// <summary>
	/// Main entry of application!
	/// </summary>
	public class InitController : UiController {
		public static Action OnGuideLoaded;
		public override void OnShowViewFinished()
		{
			GetGuidesData();
			UiBlocker.Disable();
		}

		/// <summary>
		/// Get guide json.
		/// </summary>
		private void GetGuidesData()
		{
			using GuidesEntity guidesEntity = new GuidesEntity();

			var guides = guidesEntity.GetAll().Where(g => g.Active).ToList();

			if (guides.Any() == false)
			{
				//We dont have any active guide!
				AdminUtility.LoadAdminScene();

				UiBlocker.Disable();
				return;
			}

			//If there is more than one active guide!
			if (guides.Count > 1)
			{
				GameEventMessage.SendEvent("OnGuidesLoaded");
				return;
			}

			//If its only one active guide!
			DOVirtual.DelayedCall(1f, () => {
				Data.Guide = JsonConvert.DeserializeObject<Guide>(guides[0].Json);
				GameEventMessage.SendEvent("OnGuideLoaded");
				KioskController.Instance.EnableKioskMode();
				OnGuideLoaded?.Invoke();
			});
		}
	}
}