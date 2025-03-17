using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Recruitment;

namespace MyLittleWarband
{
	// Token: 0x0200003B RID: 59
	[HarmonyPatch(typeof(RecruitmentVM), "Deactivate")]
	internal class RecruitPatch1
	{
		// Token: 0x060001CC RID: 460 RVA: 0x0000D608 File Offset: 0x0000B808
		private static void Postfix()
		{
			bool flag = !SubModule.ReplaceAllForPlayer;
			if (!flag)
			{
				int num = 0;
				foreach (Hero hero in Settlement.CurrentSettlement.Notables)
				{
					for (int i = 0; i < 6; i++)
					{
						bool flag2 = hero.VolunteerTypes[i] != null && TroopsBehavior.recruits != null && TroopsBehavior.recruits[i, num] != null;
						if (flag2)
						{
							hero.VolunteerTypes[i] = TroopsBehavior.recruits[i, num];
						}
					}
					num++;
				}
			}
		}
	}
}
