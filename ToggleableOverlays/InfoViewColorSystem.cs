using Colossal.Serialization.Entities;

using Game;
using Game.Prefabs;
using Game.Rendering;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;

using static Game.Rendering.LightingSystem;

namespace ToggleableOverlays
{
	internal partial class InfoViewColorSystem : GameSystemBase
	{
		private LightingSystem lightingSystem;
		private PrefabSystem prefabSystem;
		private InfoviewInitializeSystem infoViewInitializeSystem;
		private State lastLightingState = State.Day;
		private ColorMode lastColorMode = ColorMode.Default;
		private readonly Dictionary<ColorInfomodeBasePrefab, Color> cachedColorInfoModes = new();
		private readonly Dictionary<GradientInfomodeBasePrefab, (Color low, Color medium, Color high)> cachedGradientInfoModes = new();

		protected override void OnCreate()
		{
			base.OnCreate();

			lightingSystem = World.GetOrCreateSystemManaged<LightingSystem>();
			prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
			infoViewInitializeSystem = World.GetOrCreateSystemManaged<InfoviewInitializeSystem>();
		}

		public override int GetUpdateInterval(SystemUpdatePhase phase)
		{
			return 1024;
		}

		protected override void OnUpdate()
		{
			if (lastLightingState != lightingSystem.state || lastColorMode != Mod.Settings.ColorblindMode)
			{
				lastLightingState = lightingSystem.state;
				lastColorMode = Mod.Settings.ColorblindMode;

				ChangeOverlayColors();
			}
		}

		protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
		{
			base.OnGameLoadingComplete(purpose, mode);

			ChangeOverlayColors();
		}

		private void ChangeOverlayColors()
		{
			ChangeOverlayColors(lastLightingState switch
			{
				State.Day => Mod.Settings.UseDaytimeForDarkMode ? new(0.5f, 0.5f, 0.5f) : new(0.2f, 0.2f, 0.2f),
				State.Night => new(0.7f, 0.7f, 0.7f),
				_ => new(0.3f, 0.3f, 0.3f),
			});
		}

		private void ChangeOverlayColors(Color baseColor)
		{
			var viewsToDarken = new[]
			{
				"LandValue.LandValueInfomode",
				"TelecomService.TelecomCoverageInfomode",
				"Residential.GroundPollutionInfomode",
				"Commercial.CustomersInfomode",
				"Population.PopulationInfomode",
				"DisasterControl.Damage"
			};

			foreach (var infoView in infoViewInitializeSystem.infoviews)
			{
				infoView.m_DefaultColor = baseColor;

				foreach (var infoMode in infoView.m_Infomodes)
				{
					ChangeColorsForColorblind(infoMode.m_Mode);

					if (viewsToDarken.Contains($"{infoView.name}.{infoMode.m_Mode.name}") && infoMode.m_Mode is GradientInfomodeBasePrefab gradientInfoModeBasePrefab)
					{
						gradientInfoModeBasePrefab.m_Low = baseColor;
					}

					if ($"{infoView.name}.{infoMode.m_Mode.name}" == "DisasterControl.Destroyed")
					{
						(infoMode.m_Mode as ColorInfomodeBasePrefab).m_Color = new Color(1f, 0.8f, 0.8f);
					}
				}
			}

			var prefabs = typeof(PrefabSystem)
				.GetField("m_Prefabs", BindingFlags.NonPublic | BindingFlags.Instance)
				.GetValue(prefabSystem) as List<PrefabBase>;

			foreach (var item in prefabs)
			{
				if (item.name is "FertilityInfomode" or "OreInfomode" or "OilInfomode" or "ForestInfomode")
				{
					ChangeColorsForColorblind(item);

					(item as GradientInfomodeBasePrefab).m_Low = baseColor;
				}
			}
		}

		private void ChangeColorsForColorblind(PrefabBase item)
		{
			if (Mod.Settings.ColorblindMode == ColorMode.Default)
			{
				return;
			}

			if (item is GradientInfomodeBasePrefab gradientInfomode)
			{
				if (!cachedGradientInfoModes.ContainsKey(gradientInfomode))
				{
					cachedGradientInfoModes[gradientInfomode] = (gradientInfomode.m_Low, gradientInfomode.m_Medium, gradientInfomode.m_High);
				}

				gradientInfomode.m_Low = ColorblindUtil.ConvertColor(cachedGradientInfoModes[gradientInfomode].low, Mod.Settings.ColorblindMode);
				gradientInfomode.m_Medium = ColorblindUtil.ConvertColor(cachedGradientInfoModes[gradientInfomode].medium, Mod.Settings.ColorblindMode);
				gradientInfomode.m_High = ColorblindUtil.ConvertColor(cachedGradientInfoModes[gradientInfomode].high, Mod.Settings.ColorblindMode);
			}
			else if (item is ColorInfomodeBasePrefab colorInfomode)
			{
				if (!cachedColorInfoModes.ContainsKey(colorInfomode))
				{
					cachedColorInfoModes[colorInfomode] = colorInfomode.m_Color;
				}

				colorInfomode.m_Color = ColorblindUtil.ConvertColor(cachedColorInfoModes[colorInfomode], Mod.Settings.ColorblindMode);
			}
		}
	}
}
