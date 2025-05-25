using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using Hypn.Patches;
using BepInEx.Configuration;
using TMPro;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Threading;

namespace Hypn;

[BepInPlugin("Hypn.ListDeadPlayers", "ListDeadPlayers", "1.1.0")]
public class Plugin : BaseUnityPlugin
{
	public const string ModGUID = "Hypn.ListDeadPlayers";
	public const string ModName = "ListDeadPlayers";
	public const string ModVersion = "1.1.0";
	private readonly Harmony harmony = new Harmony(ModGUID);

	internal static Plugin Instance { get; private set; } = null!;
	public static new ManualLogSource Logger { get; private set; } = null!;

	public GameObject? screenLabel;
	public TextMeshProUGUI? screenLabelText;
	public Thread? monitorThread;
	public List<string> deadPlayers = new List<string>();
	private static readonly object _updateLock = new();

	private void Awake()
	{
		Instance = this;

		// Prevent the plugin from being deleted
		this.gameObject.transform.parent = null;
		this.gameObject.hideFlags = HideFlags.HideAndDontSave;

		Logger = BepInEx.Logging.Logger.CreateLogSource(ModGUID);
		Logger.LogInfo($"Plugin {ModName} v{ModVersion} is loaded!");

		harmony.PatchAll();
	}

	internal void Unpatch()
	{
		harmony?.UnpatchSelf();
	}

	public void SetupLabel()
	{
		lock (_updateLock)
		{
			if (screenLabel == null) {
				Logger.LogInfo($"{Plugin.ModName}: Creating label...");

				GameObject hud = GameObject.Find("Game Hud");
				GameObject haul = GameObject.Find("Tax Haul");

				if (hud == null || haul == null)
				{
					Logger.LogInfo($"{Plugin.ModName}: Error getting HUD or Haul - not setting up label :(");
					return;
				}
				
				TMP_FontAsset font = haul.GetComponent<TMP_Text>().font;
				screenLabel = new GameObject();
				screenLabel.SetActive(false);
				screenLabel.name = "ListDeadPlayers";
				screenLabel.AddComponent<TextMeshProUGUI>();

				screenLabelText = screenLabel.GetComponent<TextMeshProUGUI>();
				screenLabelText.font = font;
				screenLabelText.color = Color.red;
				screenLabelText.fontSize = 24f;
				screenLabelText.enableWordWrapping = true;
				screenLabelText.overflowMode = TextOverflowModes.Overflow;
				screenLabelText.alignment = TextAlignmentOptions.BottomRight;
				screenLabelText.horizontalAlignment = HorizontalAlignmentOptions.Right;
				screenLabelText.verticalAlignment = VerticalAlignmentOptions.Bottom;
				screenLabelText.SetText("");
				screenLabel.transform.SetParent(hud.transform, false);

				RectTransform component = screenLabel.GetComponent<RectTransform>();

				component.anchorMax = new Vector2(1f, 0f);
				component.anchorMin = new Vector2(1f, 0f);
				component.anchoredPosition = new Vector2(-10f, 10f);
				component.pivot = new Vector2(1f, 0f);
				component.sizeDelta = new Vector2(300f, 400f);
			}
		}
	}
}
