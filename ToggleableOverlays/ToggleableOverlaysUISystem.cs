using Colossal.Serialization.Entities;
using Colossal.UI.Binding;

using Game;
using Game.Prefabs;
using Game.Tools;
using Game.UI;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ToggleableOverlays
{
	internal partial class ToggleableOverlaysUISystem : UISystemBase
	{
		private ToolSystem _toolSystem;
		private PrefabSystem _prefabSystem;
		private InfoviewInitializeSystem _infoViewInitializeSystem;

		protected override void OnCreate()
		{
			base.OnCreate();

			_toolSystem = World.GetOrCreateSystemManaged<ToolSystem>();
			_prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
			_infoViewInitializeSystem = World.GetOrCreateSystemManaged<InfoviewInitializeSystem>();

			AddBinding(new TriggerBinding("ToggleableOverlays", "InfoViewOpened", OpenInfoView));
		}

		protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
		{
			base.OnGameLoadingComplete(purpose, mode);

			var darkGrey = new UnityEngine.Color(0.2f, 0.2f, 0.2f);
			var viewsToDarken = new[]
			{
				"LandValue.LandValueInfomode",
				"TelecomService.TelecomCoverageInfomode",
				"Residential.GroundPollutionInfomode",
				"Commercial.CustomersInfomode",
				"Population.PopulationInfomode",
				"DisasterControl.Damage"
			};

			foreach (var infoView in _infoViewInitializeSystem.infoviews)
			{
				infoView.m_DefaultColor = darkGrey;

				foreach (var infoMode in infoView.m_Infomodes)
				{
					if (viewsToDarken.Contains($"{infoView.name}.{infoMode.m_Mode.name}") && infoMode.m_Mode is GradientInfomodeBasePrefab gradientInfoModeBasePrefab)
					{
						gradientInfoModeBasePrefab.m_Low = darkGrey;
					}

					if ($"{infoView.name}.{infoMode.m_Mode.name}" == "DisasterControl.Destroyed")
					{
						(infoMode.m_Mode as ColorInfomodeBasePrefab).m_Color = new UnityEngine.Color(1f, 0.8f, 0.8f);
					}
				}
			}

			var prefabs = typeof(PrefabSystem)
				.GetField("m_Prefabs", BindingFlags.NonPublic | BindingFlags.Instance)
				.GetValue(_prefabSystem) as List<PrefabBase>;

			foreach (var item in prefabs)
			{
				if (item.name is "FertilityInfomode" or "OreInfomode" or "OilInfomode" or "ForestInfomode")
				{
					(item as GradientInfomodeBasePrefab).m_Low = darkGrey;
				}
			}
		}

		private void OpenInfoView()
		{
			if (Mod.Settings.AutomaticallyOpenInfoView && _toolSystem.activePrefab != null && _prefabSystem.TryGetEntity(_toolSystem.activePrefab, out var entity))
			{
				var infoView = ToolBaseSystemPatch.GetInfoViewPrefab(entity);

				if (infoView != null)
				{
					_toolSystem.infoview = infoView;
				}
			}
		}
	}
}
