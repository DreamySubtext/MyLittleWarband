using System;
using System.Collections.Generic;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace MyLittleWarband
{
	// Token: 0x0200003C RID: 60
	[HarmonyPatch(typeof(PlayerTownVisitCampaignBehavior), "game_menu_recruit_volunteers_on_consequence")]
	internal class RecruitPatch2
	{
		// Token: 0x060001CE RID: 462 RVA: 0x0000D6D0 File Offset: 0x0000B8D0
		private static bool Prefix(MenuCallbackArgs args)
		{
			bool flag = !SubModule.ReplaceAllForPlayer;
			bool flag2;
			if (flag)
			{
				flag2 = true;
			}
			else
			{
				TroopsBehavior.recruits = new CharacterObject[6, Settlement.CurrentSettlement.Notables.Count];
				int num = 0;
				foreach (Hero hero in Settlement.CurrentSettlement.Notables)
				{
					for (int i = 0; i < 6; i++)
					{
						TroopsBehavior.recruits[i, num] = hero.VolunteerTypes[i];
						bool flag3 = hero.VolunteerTypes[i] != null && !EnyclopediaEditUnitPatch.isCustomTroop(hero.VolunteerTypes[i]);
						if (flag3)
						{
							bool flag4 = RecruitPatch2.isEliteLine(hero.VolunteerTypes[i]);
							if (flag4)
							{
								hero.VolunteerTypes[i] = RecruitPatch2.tryToLevel(Game.Current.ObjectManager.GetObject<CharacterObject>("_elite_root"), hero.VolunteerTypes[i].Tier);
							}
							else
							{
								hero.VolunteerTypes[i] = RecruitPatch2.tryToLevel(Game.Current.ObjectManager.GetObject<CharacterObject>("_basic_root"), hero.VolunteerTypes[i].Tier);
							}
						}
					}
					num++;
				}
				flag2 = false;
			}
			return flag2;
		}

		// Token: 0x060001CF RID: 463 RVA: 0x0000D854 File Offset: 0x0000BA54
		public static CharacterObject tryToLevel(CharacterObject root, int tier)
		{
			CharacterObject characterObject = root;
			while (characterObject.Tier < tier && characterObject.UpgradeTargets != null && characterObject.UpgradeTargets.Length != 0)
			{
				characterObject = characterObject.UpgradeTargets.GetRandomElement<CharacterObject>();
			}
			return characterObject;
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x0000D89C File Offset: 0x0000BA9C
		public static bool isEliteLine(CharacterObject unit)
		{
			List<CharacterObject> list = new List<CharacterObject>();
			Stack<CharacterObject> stack = new Stack<CharacterObject>();
			stack.Push(unit.Culture.EliteBasicTroop);
			list.Add(unit.Culture.EliteBasicTroop);
			while (!stack.IsEmpty<CharacterObject>())
			{
				CharacterObject characterObject = stack.Pop();
				bool flag = characterObject.UpgradeTargets != null && characterObject.UpgradeTargets.Length != 0;
				if (flag)
				{
					for (int i = 0; i < characterObject.UpgradeTargets.Length; i++)
					{
						bool flag2 = !list.Contains(characterObject.UpgradeTargets[i]);
						if (flag2)
						{
							list.Add(characterObject.UpgradeTargets[i]);
							stack.Push(characterObject.UpgradeTargets[i]);
						}
					}
				}
			}
			return list.Contains(unit);
		}
	}
}
