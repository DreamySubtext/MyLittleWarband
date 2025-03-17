using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;

namespace MyLittleWarband
{
	// Token: 0x02000006 RID: 6
	public class EquipmentSelectorBehavior : CampaignBehaviorBase
	{
		// Token: 0x0600002D RID: 45 RVA: 0x0000393C File Offset: 0x00001B3C
		public static void CreateVMLayer(List<ItemObject> items, CharacterObject troop, string equipmentType)
		{
			if (EquipmentSelectorBehavior._layer != null)
			{
				return;
			}
			EquipmentSelectorBehavior._layer = new GauntletLayer(1001, "GauntletLayer", false);
			if (EquipmentSelectorBehavior._equipmentSelectorVM == null)
			{
				EquipmentSelectorBehavior._equipmentSelectorVM = new EquipmentSelectorVM(items, troop, equipmentType);
			}
			EquipmentSelectorBehavior._equipmentSelectorVM.RefreshValues();
			EquipmentSelectorBehavior._gauntletMovie = (GauntletMovie)EquipmentSelectorBehavior._layer.LoadMovie("EquipmentSelection", EquipmentSelectorBehavior._equipmentSelectorVM);
			EquipmentSelectorBehavior._layer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
			ScreenManager.TopScreen.AddLayer(EquipmentSelectorBehavior._layer);
			EquipmentSelectorBehavior._layer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(EquipmentSelectorBehavior._layer);
		}

		// Token: 0x0600002E RID: 46 RVA: 0x000039D8 File Offset: 0x00001BD8
		public static void DeleteVMLayer()
		{
			if (EquipmentSelectorBehavior._layer != null)
			{
				EquipmentSelectorBehavior._layer.InputRestrictions.ResetInputRestrictions();
				EquipmentSelectorBehavior._layer.IsFocusLayer = false;
				if (EquipmentSelectorBehavior._gauntletMovie != null)
				{
					EquipmentSelectorBehavior._layer.ReleaseMovie(EquipmentSelectorBehavior._gauntletMovie);
				}
				ScreenManager.TopScreen.RemoveLayer(EquipmentSelectorBehavior._layer);
			}
			EquipmentSelectorBehavior._layer = null;
			EquipmentSelectorBehavior._gauntletMovie = null;
			EquipmentSelectorBehavior._equipmentSelectorVM = null;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x000021D0 File Offset: 0x000003D0
		public override void RegisterEvents()
		{
		}

		// Token: 0x06000030 RID: 48 RVA: 0x000021D0 File Offset: 0x000003D0
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x04000011 RID: 17
		private static GauntletLayer _layer;

		// Token: 0x04000012 RID: 18
		private static GauntletMovie _gauntletMovie;

		// Token: 0x04000013 RID: 19
		private static EquipmentSelectorVM _equipmentSelectorVM;
	}
}
