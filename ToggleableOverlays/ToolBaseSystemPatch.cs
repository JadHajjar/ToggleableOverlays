using Colossal.Entities;

using Game.Prefabs;
using Game.Tools;

using HarmonyLib;

using Unity.Entities;

namespace ToggleableOverlays
{
	[HarmonyPatch]
	internal class ToolBaseSystemPatch
	{
		private static readonly ToolSystem _toolSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<ToolSystem>();
		private static readonly PrefabSystem _prefabSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PrefabSystem>();

		[HarmonyPrefix, HarmonyPatch(typeof(ToolBaseSystem), "UpdateInfoview")]
		public static bool UpdateInfoview(Entity prefab)
		{
			if (Mod.Settings.CloseInfoViewOnAssetChange)
			{
				var infoView = GetInfoViewPrefab(prefab);

				if (_toolSystem.infoview != null && infoView != null && _toolSystem.infoview != infoView)
				{
					_toolSystem.infoview = null;
				}

				return false;
			}
			else if (Mod.Settings.AutomaticallySwitchInfoViewIfOpen && _toolSystem.infoview != null)
			{
				return true;
			}

			return Mod.Settings.OpenInfoViewWhenSelectingAsset;
		}

		public static InfoviewPrefab GetInfoViewPrefab(Entity prefab)
		{
			if (_toolSystem.EntityManager.HasComponent<NetData>(prefab) && _toolSystem.EntityManager.TryGetBuffer(prefab, isReadOnly: true, out DynamicBuffer<SubObject> buffer))
			{
				for (var i = 0; i < buffer.Length; i++)
				{
					var subObject = buffer[i];
					if ((subObject.m_Flags & SubObjectFlags.MakeOwner) != 0)
					{
						prefab = subObject.m_Prefab;
						break;
					}
				}
			}

			if (_toolSystem.EntityManager.TryGetBuffer(prefab, isReadOnly: true, out DynamicBuffer<PlaceableInfoviewItem> buffer2) && buffer2.Length != 0)
			{
				return _prefabSystem.GetPrefab<InfoviewPrefab>(buffer2[0].m_Item);
			}

			return null;
		}
	}
}
