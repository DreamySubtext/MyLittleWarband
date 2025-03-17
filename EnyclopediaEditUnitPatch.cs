using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;

namespace MyLittleWarband
{
	// Token: 0x0200003A RID: 58
	[HarmonyPatch(typeof(EncyclopediaUnitVM), "ExecuteLink")]
	internal class EnyclopediaEditUnitPatch
	{
		// Token: 0x060001C9 RID: 457 RVA: 0x0000D470 File Offset: 0x0000B670
		private static bool Prefix(EncyclopediaUnitVM __instance)
		{
			BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			FieldInfo field = __instance.GetType().GetField("_character", bindingFlags);
			CharacterObject characterObject = (CharacterObject)field.GetValue(__instance);
			bool flag = (InputKey.LeftShift.IsDown() || InputKey.RightShift.IsDown()) && (EnyclopediaEditUnitPatch.isCustomTroop(characterObject) || SubModule.FullUnitEditor);
			bool flag2;
			if (flag)
			{
				CustomUnitsBehavior.CreateVMLayer(characterObject.StringId);
				flag2 = false;
			}
			else
			{
				flag2 = true;
			}
			return flag2;
		}

		// Token: 0x060001CA RID: 458 RVA: 0x0000D4E4 File Offset: 0x0000B6E4
		public static bool isCustomTroop(CharacterObject _character)
		{
			List<CharacterObject> list = new List<CharacterObject>();
			Stack<CharacterObject> stack = new Stack<CharacterObject>();
			stack.Push(Game.Current.ObjectManager.GetObject<CharacterObject>("_basic_root"));
			list.Add(Game.Current.ObjectManager.GetObject<CharacterObject>("_basic_root"));
			stack.Push(Game.Current.ObjectManager.GetObject<CharacterObject>("_elite_root"));
			list.Add(Game.Current.ObjectManager.GetObject<CharacterObject>("_elite_root"));
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
			return list.Contains(_character);
		}
	}
}
