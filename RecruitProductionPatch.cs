using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace MyLittleWarband
{
	// Token: 0x0200000D RID: 13
	[HarmonyPatch(typeof(RecruitmentCampaignBehavior), "UpdateVolunteersOfNotablesInSettlement")]
	internal class RecruitProductionPatch
	{
		// Token: 0x06000054 RID: 84 RVA: 0x00002252 File Offset: 0x00000452
		private static void Postfix(Settlement settlement)
		{
			RecruitProductionPatch.fix(settlement);
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00004BD0 File Offset: 0x00002DD0
		public static void fix(Settlement settlement)
		{
			foreach (Hero hero in settlement.Notables)
			{
				bool flag = SubModule.SpawnAtPlayerSettlement && settlement.OwnerClan != null && settlement.OwnerClan == Hero.MainHero.Clan;
				bool flag2 = SubModule.SpawnAtPlayerKingdom && settlement.OwnerClan != null && settlement.OwnerClan.Kingdom != null && settlement.OwnerClan.Kingdom.Leader == Hero.MainHero;
				bool flag3 = hero.CanHaveRecruits && (flag || flag2);
				if (flag3)
				{
					for (int i = 0; i < 6; i++)
					{
						bool flag4 = hero.VolunteerTypes[i] != null && !EnyclopediaEditUnitPatch.isCustomTroop(hero.VolunteerTypes[i]);
						if (flag4)
						{
							bool flag5 = RecruitPatch2.isEliteLine(hero.VolunteerTypes[i]);
							if (flag5)
							{
								hero.VolunteerTypes[i] = RecruitPatch2.tryToLevel(Game.Current.ObjectManager.GetObject<CharacterObject>("_elite_root"), hero.VolunteerTypes[i].Tier);
							}
							else
							{
								hero.VolunteerTypes[i] = RecruitPatch2.tryToLevel(Game.Current.ObjectManager.GetObject<CharacterObject>("_basic_root"), hero.VolunteerTypes[i].Tier);
							}
						}
					}
				}
			}
		}
	}
}
