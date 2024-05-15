using Colossal.IO.AssetDatabase;
using Colossal.Logging;

using Game;
using Game.Modding;
using Game.SceneFlow;

using HarmonyLib;

namespace ToggleableOverlays
{
	public class Mod : IMod
	{
		private Harmony _harmony;

		public static ILog Log { get; } = LogManager.GetLogger(nameof(ToggleableOverlays)).SetShowsErrorsInUI(false);
		public static Setting Settings { get; private set; }

		public void OnLoad(UpdateSystem updateSystem)
		{
			Log.Info(nameof(OnLoad));

			Settings = new Setting(this);
			Settings.RegisterInOptionsUI();
			GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(Settings));
			AssetDatabase.global.LoadSettings(nameof(ToggleableOverlays), Settings, new Setting(this) { DefaultBlock = true });

			_harmony = new Harmony($"{nameof(ToggleableOverlays)}.{nameof(Mod)}");
			_harmony.PatchAll(typeof(Mod).Assembly);

			updateSystem.UpdateAt<ToggleableOverlaysUISystem>(SystemUpdatePhase.UIUpdate);
		}

		public void OnDispose()
		{
			Log.Info(nameof(OnDispose));
			_harmony?.UnpatchAll($"{nameof(ToggleableOverlays)}.{nameof(Mod)}");
		}
	}
}
