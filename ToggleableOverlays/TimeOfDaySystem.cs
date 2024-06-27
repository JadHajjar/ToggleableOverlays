using Colossal.Serialization.Entities;

using Game;
using Game.Prefabs;
using Game.Rendering;
using Game.Simulation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using static Game.Rendering.LightingSystem;

namespace ToggleableOverlays
{
	internal partial class TimeOfDaySystem : GameSystemBase
	{
		private LightingSystem lightingSystem;
		private PrefabSystem prefabSystem;
		private InfoviewInitializeSystem infoViewInitializeSystem;
		private State lastLightingState = State.Day;

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
			if (lastLightingState != lightingSystem.state)
			{
				lastLightingState = lightingSystem.state;

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
				State.Day => new(0.2f, 0.2f, 0.2f),
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
					if (viewsToDarken.Contains($"{infoView.name}.{infoMode.m_Mode.name}") && infoMode.m_Mode is GradientInfomodeBasePrefab gradientInfoModeBasePrefab)
					{
						gradientInfoModeBasePrefab.m_Low = baseColor;
					}

					if ($"{infoView.name}.{infoMode.m_Mode.name}" == "DisasterControl.Destroyed")
					{
						(infoMode.m_Mode as ColorInfomodeBasePrefab).m_Color = new UnityEngine.Color(1f, 0.8f, 0.8f);
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
					(item as GradientInfomodeBasePrefab).m_Low = baseColor;
				}
			}
		}
	}
}
