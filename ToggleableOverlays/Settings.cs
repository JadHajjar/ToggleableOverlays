using Colossal;
using Colossal.IO.AssetDatabase;

using Game.Modding;
using Game.Settings;

using System.Collections.Generic;

namespace ToggleableOverlays
{
	[FileLocation(nameof(ToggleableOverlays))]
	[SettingsUIGroupOrder("Settings")]
	[SettingsUIShowGroupName("Settings")]
	public class Setting : ModSetting
	{
		public Setting(IMod mod) : base(mod)
		{

		}

		[SettingsUIHidden]
		public bool DefaultBlock { get; set; }

		[SettingsUISection("Main", "Settings")]
		public bool CloseInfoViewOnAssetChange { get; set; }

		[SettingsUISection("Main", "Settings")]
		public bool AutomaticallyOpenInfoView { get; set; } = true;

		[SettingsUISection("Main", "Settings"), SettingsUIDisableByCondition(typeof(Setting), nameof(CloseInfoViewOnAssetChange))]
		public bool AutomaticallySwitchInfoViewIfOpen { get; set; } = true;

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
				{ m_Setting.GetSettingsLocaleID(), "Toggleable Overlays" },
				{ m_Setting.GetOptionTabLocaleID("Main"), "Main" },

				{ m_Setting.GetOptionGroupLocaleID("Settings"), "Settings" },

				{ m_Setting.GetOptionLabelLocaleID(nameof(Setting.CloseInfoViewOnAssetChange)), "Close info-view overlay when selecting a different asset" },
				{ m_Setting.GetOptionDescLocaleID(nameof(Setting.CloseInfoViewOnAssetChange)), $"When selecting an asset that does not match your active info-view, the info-view will automatically close." },

				{ m_Setting.GetOptionLabelLocaleID(nameof(Setting.AutomaticallyOpenInfoView)), "Automatically select the active asset's info-view" },
				{ m_Setting.GetOptionDescLocaleID(nameof(Setting.AutomaticallyOpenInfoView)), $"While an asset is selected, clicking the info-view button on the top-left of your screen, or using the hotkey, will automatically enable the asset's corresponding inf-view." },

				{ m_Setting.GetOptionLabelLocaleID(nameof(Setting.AutomaticallySwitchInfoViewIfOpen)), "Automatically switch to to an asset's info-view when selected" },
				{ m_Setting.GetOptionDescLocaleID(nameof(Setting.AutomaticallySwitchInfoViewIfOpen)), $"While any info-view is active, selecting an an asset will automatically enable the asset's corresponding inf-view. This has no effect if no info-view is active." },
			};
		}

		public void Unload()
		{

		}
	}
}
