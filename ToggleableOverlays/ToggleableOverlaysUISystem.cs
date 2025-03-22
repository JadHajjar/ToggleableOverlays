using Game.Input;
using Game.Prefabs;
using Game.Tools;

using UnityEngine;

namespace ToggleableOverlays
{
	internal partial class ToggleableOverlaysUISystem : ExtendedUISystemBase
	{
		private ToolSystem toolSystem;
		private PrefabSystem prefabSystem;
		private ProxyAction toggleKeyBinding;

		protected override void OnCreate()
		{
			base.OnCreate();

			toolSystem = World.GetOrCreateSystemManaged<ToolSystem>();
			prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();

			toggleKeyBinding = Mod.Settings.GetAction(nameof(Setting.ToggleShaderKeyBinding));
			toggleKeyBinding.shouldBeEnabled = true;

			CreateBinding("InfoViewsEnabled", () => Shader.GetGlobalInt("colossal_InfoviewOn") == 1);
			CreateBinding("HideUIToggle", () => Mod.Settings.HideUIToggle);
			CreateTrigger<bool>("SetInfoViewsEnabled", SetInfoViewsToggle);
			CreateTrigger("InfoViewOpened", OpenInfoView);
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();

			if (toggleKeyBinding.WasPerformedThisFrame() && toolSystem.activeInfoview)
			{
				SetInfoViewsToggle(Shader.GetGlobalInt("colossal_InfoviewOn") != 1);
			}
		}

		private void SetInfoViewsToggle(bool obj)
		{
			Shader.SetGlobalInt("colossal_InfoviewOn", obj ? 1 : 0);
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
