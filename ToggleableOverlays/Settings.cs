using Colossal;
using Colossal.IO.AssetDatabase;

using Game.Input;
using Game.Modding;
using Game.Settings;

using System.Collections.Generic;

namespace ToggleableOverlays
{
	[FileLocation(nameof(ToggleableOverlays))]
	[SettingsUIGroupOrder("Settings", "Color", "KeyBindings")]
	[SettingsUIShowGroupName("Settings", "Color", "KeyBindings")]
	public class Setting : ModSetting
	{
		private bool _closeInfoViewOnAssetChange;
		private bool _automaticallyOpenInfoView = true;
		private bool _automaticallySwitchInfoViewIfOpen = true;

		public Setting(IMod mod) : base(mod)
		{

		}

		[SettingsUISection("Main", "Settings")]
		public bool OpenInfoViewWhenSelectingAsset { get; set; }

		[SettingsUISection("Main", "Settings"), SettingsUIDisableByCondition(typeof(Setting), nameof(OpenInfoViewWhenSelectingAsset))]
		public bool CloseInfoViewOnAssetChange { get => _closeInfoViewOnAssetChange && !OpenInfoViewWhenSelectingAsset; set => _closeInfoViewOnAssetChange = value; }

		[SettingsUISection("Main", "Settings"), SettingsUIDisableByCondition(typeof(Setting), nameof(OpenInfoViewWhenSelectingAsset))]
		public bool AutomaticallyOpenInfoView { get => _automaticallyOpenInfoView && !OpenInfoViewWhenSelectingAsset; set => _automaticallyOpenInfoView = value; }

		[SettingsUISection("Main", "Settings"), SettingsUIDisableByCondition(typeof(Setting), nameof(DisableAutomaticallySwitchInfoViewIfOpen))]
		public bool AutomaticallySwitchInfoViewIfOpen { get => _automaticallySwitchInfoViewIfOpen && !DisableAutomaticallySwitchInfoViewIfOpen; set => _automaticallySwitchInfoViewIfOpen = value; }

		[SettingsUISection("Main", "Color")]
		public bool UseDaytimeForDarkMode { get; set; }

		[SettingsUISection("Main", "Color")]
		public ColorMode ColorblindMode { get; set; } = ColorMode.Default;

		[SettingsUIKeyboardBinding(BindingKeyboard.T, nameof(ToggleShaderKeyBinding), ctrl: true)]
		[SettingsUISection("Main", "KeyBindings")]
		public ProxyBinding ToggleShaderKeyBinding { get; set; }

		public bool DisableAutomaticallySwitchInfoViewIfOpen => OpenInfoViewWhenSelectingAsset || CloseInfoViewOnAssetChange;

		public override void SetDefaults()
		{

		}
	}

	public class LocaleEN : IDictionarySource
	{
		private readonly Setting m_Setting;
		public LocaleEN(Setting setting)
		{
			m_Setting = setting;
		}

		public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
		{
			return new Dictionary<string, string>
			{
				{ m_Setting.GetBindingMapLocaleID(), "Toggleable Overlays" },
				{ m_Setting.GetSettingsLocaleID(), "Toggleable Overlays" },
				{ m_Setting.GetOptionTabLocaleID("Main"), "Main" },

				{ m_Setting.GetOptionGroupLocaleID("Settings"), "Settings" },
				{ m_Setting.GetOptionGroupLocaleID("Color"), "Colors" },
				{ m_Setting.GetOptionGroupLocaleID("KeyBindings"), "KeyBindings" },

				{ m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenInfoViewWhenSelectingAsset)), "Open the infoview of an asset when selecting it" },
				{ m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenInfoViewWhenSelectingAsset)), $"This is the default vanilla behavior, whenever you select an asset, its infoview will automatically be opened." },

				{ m_Setting.GetOptionLabelLocaleID(nameof(Setting.CloseInfoViewOnAssetChange)), "Close infoview overlay when selecting a different asset" },
				{ m_Setting.GetOptionDescLocaleID(nameof(Setting.CloseInfoViewOnAssetChange)), $"When selecting an asset that does not match your active infoview, the infoview will automatically close." },

				{ m_Setting.GetOptionLabelLocaleID(nameof(Setting.AutomaticallyOpenInfoView)), "Automatically select the active asset's infoview" },
				{ m_Setting.GetOptionDescLocaleID(nameof(Setting.AutomaticallyOpenInfoView)), $"While an asset is selected, clicking the infoview button on the top-left of your screen, or using the hotkey, will automatically enable the asset's corresponding inf-view." },

				{ m_Setting.GetOptionLabelLocaleID(nameof(Setting.AutomaticallySwitchInfoViewIfOpen)), "Automatically switch to to an asset's infoview when selected" },
				{ m_Setting.GetOptionDescLocaleID(nameof(Setting.AutomaticallySwitchInfoViewIfOpen)), $"While any infoview is active, selecting an an asset will automatically enable the asset's corresponding inf-view. This has no effect if no infoview is active." },

				{ m_Setting.GetOptionLabelLocaleID(nameof(Setting.UseDaytimeForDarkMode)), "Use in-game time for for light or dark mode colors" },
				{ m_Setting.GetOptionDescLocaleID(nameof(Setting.UseDaytimeForDarkMode)), $"Makes the base color of info-views a lighter grey when it's daytime in your city." },

				{ m_Setting.GetOptionLabelLocaleID(nameof(Setting.ColorblindMode)), "Color-blind Mode" },
				{ m_Setting.GetOptionDescLocaleID(nameof(Setting.ColorblindMode)), $"Automatically change infoviews' colors to match the selected color-blind spectrum." },

				{ m_Setting.GetBindingKeyLocaleID(nameof(Setting.ToggleShaderKeyBinding)), "Toggle Infoview Filter" },
				{ m_Setting.GetOptionLabelLocaleID(nameof(Setting.ToggleShaderKeyBinding)), "Toggle Infoview Filter" },
				{ m_Setting.GetOptionDescLocaleID(nameof(Setting.ToggleShaderKeyBinding)), "Determines the hot-key to quickly toggle the infoview filter." },
				
				{ m_Setting.GetEnumValueLocaleID(ColorMode.Default), "Default" },
				{ m_Setting.GetEnumValueLocaleID(ColorMode.Tritanopia), "Tritanopia" },
				{ m_Setting.GetEnumValueLocaleID(ColorMode.Protanopia), "Protanopia" },
				{ m_Setting.GetEnumValueLocaleID(ColorMode.Deuteranopia), "Deuteranopia" },
			};
		}

		public void Unload()
		{

		}
	}
}
