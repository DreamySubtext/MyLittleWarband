using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace MyLittleWarband
{
	// Token: 0x0200000A RID: 10
	[HarmonyPatch(typeof(RecruitmentCampaignBehavior), "OnTroopRecruited")]
	internal class ClanRecruitPatch2
	{
		// Token: 0x06000038 RID: 56 RVA: 0x00003B9C File Offset: 0x00001D9C
		private static bool Prefix(Hero recruiter, Settlement settlement, Hero recruitmentSource, CharacterObject troop, int count)
		{
			bool flag = !SubModule.ClanRecruitCustomTroop || recruiter == null || recruiter.Clan == null || recruiter.Clan != Hero.MainHero.Clan || recruiter.PartyBelongedTo == null;
			bool flag2;
			if (flag)
			{
				flag2 = true;
			}
			else
			{
				bool flag3 = RecruitPatch2.isEliteLine(troop);
				if (flag3)
				{
					CharacterObject characterObject = RecruitPatch2.tryToLevel(Game.Current.ObjectManager.GetObject<CharacterObject>("_elite_root"), troop.Tier);
					bool flag4 = characterObject != null && recruiter.PartyBelongedTo.MemberRoster.Contains(troop);
					if (flag4)
					{
						recruiter.PartyBelongedTo.MemberRoster.AddToCounts(troop, -1, false, 0, 0, true, -1);
						recruiter.PartyBelongedTo.MemberRoster.AddToCounts(characterObject, 1, false, 0, 0, true, -1);
					}
				}
				else
				{
					CharacterObject characterObject2 = RecruitPatch2.tryToLevel(Game.Current.ObjectManager.GetObject<CharacterObject>("_basic_root"), troop.Tier);
					bool flag5 = characterObject2 != null && recruiter.PartyBelongedTo.MemberRoster.Contains(troop);
					if (flag5)
					{
						recruiter.PartyBelongedTo.MemberRoster.AddToCounts(troop, -1, false, 0, 0, true, -1);
						recruiter.PartyBelongedTo.MemberRoster.AddToCounts(characterObject2, 1, false, 0, 0, true, -1);
					}
				}
				flag2 = true;
			}
			return flag2;
		}
	}
}
