using System;
using HarmonyLib;
using TaleWorlds.Core;

namespace MyLittleWarband
{
	// Token: 0x02000007 RID: 7
	[HarmonyPatch(typeof(BasicCharacterObject), "IsRanged", MethodType.Getter)]
	internal class IsRangedPatch
	{
		// Token: 0x06000032 RID: 50 RVA: 0x00003A40 File Offset: 0x00001C40
		private static bool Prefix(BasicCharacterObject __instance, ref bool __result)
		{
			__result = __instance.DefaultFormationClass == FormationClass.Ranged || __instance.DefaultFormationClass == FormationClass.HorseArcher;
			return false;
		}
	}
}
