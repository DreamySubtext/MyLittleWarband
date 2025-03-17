using System;
using HarmonyLib;
using TaleWorlds.Core;

namespace MyLittleWarband
{
	// Token: 0x02000008 RID: 8
	[HarmonyPatch(typeof(BasicCharacterObject), "IsMounted", MethodType.Getter)]
	internal class IsMountedPatch
	{
		// Token: 0x06000034 RID: 52 RVA: 0x00003A6C File Offset: 0x00001C6C
		private static bool Prefix(BasicCharacterObject __instance, ref bool __result)
		{
			__result = __instance.DefaultFormationClass == FormationClass.Cavalry || __instance.DefaultFormationClass == FormationClass.HorseArcher;
			return false;
		}
	}
}
