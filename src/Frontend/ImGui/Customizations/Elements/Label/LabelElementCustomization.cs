﻿using ImGuiNET;

namespace YURI_Overlay;

internal sealed class LabelElementCustomization : Customization
{
	public bool Visible = true;
	public string Format = "{0}";
	public LabelElementSettingsCustomization Settings = new();
	public OffsetCustomization Offset = new();
	public ColorCustomization Color = new();
	public LabelElementShadowCustomization Shadow = new();

	public LabelElementCustomization() { }

	public bool RenderImGui(string visibleName, string customizationName = "label", LabelElementCustomization defaultCustomization = null)
	{
		var localization = LocalizationManager.Instance.ActiveLocalization.Data.ImGui;

		var isChanged = false;

		isChanged |= ImGuiHelper.ResetButton(customizationName, defaultCustomization, Reset);

		if(ImGui.TreeNode($"{visibleName}##{customizationName}"))
		{
			isChanged |= ImGuiHelper.ResettableCheckbox($"{localization.Visible}##{customizationName}", ref Visible, defaultCustomization?.Visible);
			isChanged |= ImGuiHelper.ResettableInputText($"{localization.Format}##{customizationName}", ref Format, defaultValue: defaultCustomization?.Format);

			isChanged |= Settings.RenderImGui(customizationName, defaultCustomization?.Settings);
			isChanged |= Offset.RenderImGui(customizationName, defaultCustomization?.Offset);
			isChanged |= Color.RenderImGui(customizationName, defaultCustomization?.Color);
			isChanged |= Shadow.RenderImGui(customizationName, defaultCustomization?.Shadow);

			ImGui.TreePop();
		}

		return isChanged;
	}

	public void Reset(LabelElementCustomization defaultCustomization = null)
	{
		if(defaultCustomization is null) return;

		Visible = defaultCustomization.Visible;
		Format = defaultCustomization.Format;
		Offset.Reset(defaultCustomization.Offset);
		Color.Reset(defaultCustomization.Color);
		Shadow.Reset(defaultCustomization.Shadow);
	}
}
