using Colossal.Serialization.Entities;
using Colossal.UI.Binding;

using Game;
using Game.Prefabs;
using Game.Tools;
using Game.UI;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;

namespace ToggleableOverlays
{
	internal partial class ToggleableOverlaysUISystem : UISystemBase
	{
		private ToolSystem toolSystem;
		private PrefabSystem prefabSystem;

		protected override void OnCreate()
		{
			base.OnCreate();

			toolSystem = World.GetOrCreateSystemManaged<ToolSystem>();
			prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();

			AddBinding(new TriggerBinding("ToggleableOverlays", "InfoViewOpened", OpenInfoView));
		}

		private void OpenInfoView()
		{
			if (Mod.Settings.AutomaticallyOpenInfoView && toolSystem.activePrefab != null && prefabSystem.TryGetEntity(toolSystem.activePrefab, out var entity))
			{
				var infoView = ToolBaseSystemPatch.GetInfoViewPrefab(entity);

				if (infoView != null)
				{
					toolSystem.infoview = infoView;
				}
			}
		}
	}
}
