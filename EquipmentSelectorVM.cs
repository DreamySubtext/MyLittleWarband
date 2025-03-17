using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace MyLittleWarband
{
	// Token: 0x02000005 RID: 5
	public class EquipmentSelectorVM : ViewModel
	{
		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000025 RID: 37 RVA: 0x00002176 File Offset: 0x00000376
		// (set) Token: 0x06000026 RID: 38 RVA: 0x0000217E File Offset: 0x0000037E
		public MBBindingList<EquipmentCardRowVM> Rows { get; set; }

		// Token: 0x06000027 RID: 39 RVA: 0x00003850 File Offset: 0x00001A50
		public EquipmentSelectorVM(List<ItemObject> items, CharacterObject troop, string equipmentType)
		{
			this.Cards = new MBBindingList<EquipmentCardVM>();
			this.Rows = new MBBindingList<EquipmentCardRowVM>();
			this.AddInitialEquipmentCard(troop, equipmentType);
			foreach (ItemObject itemObject in items)
			{
				this.Cards.Add(new EquipmentCardVM(itemObject, troop, equipmentType));
				if (this.Cards.Count == 5)
				{
					this.AddRowAndClearCards();
				}
			}
			if (this.Cards.Count > 0)
			{
				this.AddRowAndClearCards();
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x000038F8 File Offset: 0x00001AF8
		public void Close()
		{
			try
			{
				EquipmentSelectorBehavior.DeleteVMLayer();
			}
			catch (Exception ex)
			{
				InformationManager.DisplayMessage(new InformationMessage("Error closing Equipment Selector: " + ex.Message));
			}
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002187 File Offset: 0x00000387
		private void AddInitialEquipmentCard(CharacterObject troop, string equipmentType)
		{
			this.Cards.Add(new EquipmentCardVM(null, troop, equipmentType));
		}

		// Token: 0x0600002A RID: 42 RVA: 0x0000219C File Offset: 0x0000039C
		private void AddRowAndClearCards()
		{
			this.Rows.Add(new EquipmentCardRowVM(this.Cards));
			this.Cards = new MBBindingList<EquipmentCardVM>();
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600002B RID: 43 RVA: 0x000021BF File Offset: 0x000003BF
		// (set) Token: 0x0600002C RID: 44 RVA: 0x000021C7 File Offset: 0x000003C7
		public MBBindingList<EquipmentCardVM> Cards { get; private set; }

		// Token: 0x0400000F RID: 15
		private const int MaxCardsPerRow = 5;
	}
}
