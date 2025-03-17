using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace MyLittleWarband
{
	// Token: 0x02000009 RID: 9
	[HarmonyPatch(typeof(PartyCharacterVM), "InitializeUpgrades")]
	internal class PartyUnitUpgradeLengthPatch
	{
		// Token: 0x06000036 RID: 54 RVA: 0x00003A98 File Offset: 0x00001C98
		private static bool Prefix(PartyCharacterVM __instance)
		{
			bool flag = __instance.Character.UpgradeTargets.Length > __instance.Upgrades.Count;
			if (flag)
			{
				__instance.Upgrades = new MBBindingList<UpgradeTargetVM>();
				for (int i = 0; i < __instance.Character.UpgradeTargets.Length; i++)
				{
					CharacterCode characterCode = (CharacterCode)typeof(PartyCharacterVM).GetMethod("GetCharacterCode", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(__instance, new object[]
					{
						__instance.Character.UpgradeTargets[i],
						__instance.Type,
						__instance.Side
					});
					__instance.Upgrades.Add(new UpgradeTargetVM(i, __instance.Character, characterCode, new Action<int, int>(__instance.Upgrade), new Action<UpgradeTargetVM>(__instance.FocusUpgrade)));
				}
				__instance.HasMoreThanTwoUpgrades = __instance.Upgrades.Count > 2;
			}
			return true;
		}
	}
}
