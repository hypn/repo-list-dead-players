using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using Hypn;
using Photon.Pun;

namespace Hypn.Patches
{
	[HarmonyPatch(typeof(PlayerAvatar), "Update")]
	internal class PlayerAvatarPatch
	{
		private static readonly object _updateLock = new();

		static void Postfix(PlayerAvatar __instance)
		{
			Plugin.Instance.SetupLabel();

			string playerName = __instance.playerName.Trim();
			if (__instance.isDisabled && !string.IsNullOrWhiteSpace(playerName) && !Plugin.Instance.deadPlayers.Contains(playerName) && !SemiFunc.MenuLevel())
			{
				Plugin.Instance.deadPlayers.Add(playerName);
			}
			else if (Plugin.Instance.deadPlayers.Contains(playerName))
			{
				Plugin.Instance.deadPlayers.Remove(playerName);
			}

			// set what the UI label text should say:
			int numDeadPlayers = Plugin.Instance.deadPlayers.Count;
			if (numDeadPlayers > 0) {
				string text = $"Dead players: {numDeadPlayers}/{GameDirector.instance.PlayerList.Count}\n" + string.Join("\n", Plugin.Instance.deadPlayers);
				Plugin.Instance.screenLabelText.SetText(text);
				Plugin.Instance.screenLabelText.color = Color.red;
				Plugin.Instance.screenLabel.SetActive(!SemiFunc.MenuLevel());
			} else {
				// Plugin.Instance.screenLabelText.SetText($"No dead players :)");
				// Plugin.Instance.screenLabelText.color = Color.green;
				// Plugin.Instance.screenLabel.SetActive(!SemiFunc.MenuLevel());
				Plugin.Instance.screenLabel.SetActive(false);
			}
		}
	}
}