using System;
using TaleWorlds.CampaignSystem;

namespace MyLittleWarband
{
	// Token: 0x0200003E RID: 62
	internal class TroopsBehavior : CampaignBehaviorBase
	{
		// Token: 0x060001DA RID: 474 RVA: 0x000021FE File Offset: 0x000003FE
		public override void RegisterEvents()
		{
		}

		// Token: 0x060001DB RID: 475 RVA: 0x000021FE File Offset: 0x000003FE
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x040000E2 RID: 226
		public static CharacterObject[,] recruits;
	}
}
