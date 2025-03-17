using System;
using TaleWorlds.Library;

namespace MyLittleWarband
{
	// Token: 0x02000003 RID: 3
	public class EquipmentCardRowVM : ViewModel
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600000E RID: 14 RVA: 0x000020FA File Offset: 0x000002FA
		// (set) Token: 0x0600000F RID: 15 RVA: 0x00002102 File Offset: 0x00000302
		public MBBindingList<EquipmentCardVM> Cards { get; set; }

		// Token: 0x06000010 RID: 16 RVA: 0x0000210B File Offset: 0x0000030B
		public EquipmentCardRowVM(MBBindingList<EquipmentCardVM> cards)
		{
			this.Cards = cards;
		}
	}
}
