﻿using System.Text.Json.Serialization;

using ImGuiNET;

namespace YURI_Overlay;

internal sealed class BarElementSettingsCustomization : Customization
{
	[JsonIgnore]
	private int _fillDirectionIndex = (int) FillDirections.LeftToRight;
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public FillDirections FillDirection { get => (FillDirections) _fillDirectionIndex; set => _fillDirectionIndex = (int) value; }

	public bool Inverted = false;

	public bool RenderImGui(string parentName = "", BarElementSettingsCustomization defaultCustomization = null)
	{
		var localization = LocalizationManager.Instance.ActiveLocalization.Data.ImGui;
		var localizationHelper = LocalizationHelper.Instance;

		var isChanged = false;
		var customizationName = $"{parentName}-settings";

		isChanged |= ImGuiHelper.ResetButton(customizationName, defaultCustomization, Reset);

		if(ImGui.TreeNode($"{localization.Settings}##{customizationName}"))
		{
			isChanged |= ImGuiHelper.ResettableCombo($"{localization.FillDirection}##{customizationName}", ref _fillDirectionIndex, localizationHelper.FillDirections, defaultCustomization?._fillDirectionIndex);
			isChanged |= ImGuiHelper.ResettableCheckbox($"{localization.Inverted}##{customizationName}", ref Inverted, defaultCustomization?.Inverted);

			ImGui.TreePop();
		}

		return isChanged;
	}

	public void Reset(BarElementSettingsCustomization defaultCustomization = null)
	{
		if(defaultCustomization is null) return;

		FillDirection = defaultCustomization.FillDirection;
		Inverted = defaultCustomization.Inverted;
	}
}
