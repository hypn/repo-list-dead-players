using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using Hypn;
using Photon.Pun;

namespace Hypn.Patches
{
	[HarmonyPatch(typeof(RoundDirector), "Update")]
	internal class HypnListDeadPlayers
	{
		private static readonly object _updateLock = new();

		static void Postfix(RoundDirector __instance)
		{
			lock (_updateLock)
			{
				if (Plugin.Instance.screenLabel == null) {
					Plugin.Logger.LogInfo($"{Plugin.ModName}: Creating label...");
					Plugin.Instance.SetupLabel();
				}
			}

			if (GameDirector.instance.PlayerList.Count < 1) {
				Plugin.Instance.screenLabelText.SetText("");
				Plugin.Instance.screenLabel.SetActive(false);
				return;
			}

			int numDeadPlayers = 0;
			foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
			{
				if (playerAvatar.isDisabled)
				{
					numDeadPlayers++;
				}
			}

			if (numDeadPlayers > 0) {
				Plugin.Instance.screenLabelText.SetText($"Dead players: {numDeadPlayers}/{GameDirector.instance.PlayerList.Count}");
				Plugin.Instance.screenLabelText.color = Color.red;
				Plugin.Instance.screenLabel.SetActive(true);
			} else {
				Plugin.Instance.screenLabelText.SetText($"No dead players :)");
				Plugin.Instance.screenLabelText.color = Color.green;
				Plugin.Instance.screenLabel.SetActive(true);
			}
		}
	}
}