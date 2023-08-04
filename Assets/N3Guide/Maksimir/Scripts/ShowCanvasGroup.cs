using System;
using UnityEngine;
using DG.Tweening;
using Novena.Utility;

namespace Utility {
	public class ShowCanvasGroup {
		public static void Show(CanvasGroup cg, bool show, float time = 0.0f, float delay = 0.0f, Ease ease = Ease.OutQuad, Action finish = null)
		{
			UiBlocker.Enable();
			if (finish != null)
			{
				cg.DOFade(show ? 1 : 0, time).SetDelay(delay).SetEase(ease).OnComplete(() => {
					UiBlocker.Disable();
					finish();
				});
			}
			else
			{
				cg.DOFade(show ? 1 : 0, time).SetDelay(delay).SetEase(ease).OnComplete(UiBlocker.Disable);
			}

			cg.interactable = show;
			cg.blocksRaycasts = show;

		}

		public static void Kill(CanvasGroup cg, bool complete = true)
		{
			cg.DOKill(complete);
		}

	}


}


