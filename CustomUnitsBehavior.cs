using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.ScreenSystem;

namespace MyLittleWarband
{
	// Token: 0x02000012 RID: 18
	internal class CustomUnitsBehavior : CampaignBehaviorBase
	{
		// Token: 0x060000FB RID: 251 RVA: 0x00009BF8 File Offset: 0x00007DF8
		public static void CreateVMLayer(string unitId)
		{
			bool flag = CustomUnitsBehavior.layer != null;
			if (!flag)
			{
				CustomUnitsBehavior.layer = new GauntletLayer(1000, "GauntletLayer", false);
				bool flag2 = CustomUnitsBehavior.customUnitVM == null;
				if (flag2)
				{
					CustomUnitsBehavior.customUnitVM = new CustomUnitsVM(unitId);
				}
				CustomUnitsBehavior.customUnitVM.RefreshValues();
				CustomUnitsBehavior.gauntletMovie = (GauntletMovie)CustomUnitsBehavior.layer.LoadMovie("CustomUnits", CustomUnitsBehavior.customUnitVM);
				CustomUnitsBehavior.layer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
				ScreenManager.TopScreen.AddLayer(CustomUnitsBehavior.layer);
				CustomUnitsBehavior.layer.IsFocusLayer = true;
				ScreenManager.TrySetFocus(CustomUnitsBehavior.layer);
			}
		}

		// Token: 0x060000FC RID: 252 RVA: 0x00009CA8 File Offset: 0x00007EA8
		public static void DeleteVMLayer()
		{
			ScreenBase topScreen = ScreenManager.TopScreen;
			bool flag = CustomUnitsBehavior.layer != null;
			if (flag)
			{
				CustomUnitsBehavior.layer.InputRestrictions.ResetInputRestrictions();
				CustomUnitsBehavior.layer.IsFocusLayer = false;
				bool flag2 = CustomUnitsBehavior.gauntletMovie != null;
				if (flag2)
				{
					CustomUnitsBehavior.layer.ReleaseMovie(CustomUnitsBehavior.gauntletMovie);
				}
				topScreen.RemoveLayer(CustomUnitsBehavior.layer);
			}
			CustomUnitsBehavior.layer = null;
			CustomUnitsBehavior.gauntletMovie = null;
			CustomUnitsBehavior.customUnitVM = null;
		}

		// Token: 0x060000FD RID: 253 RVA: 0x00009D24 File Offset: 0x00007F24
		public static void UpgradeGear(string equipmentType, string unitId)
		{
			List<InquiryElement> list = new List<InquiryElement>();
			List<Equipment> list2 = Game.Current.ObjectManager.GetObject<CharacterObject>(unitId).AllEquipments.Where((Equipment x) => !x.IsCivilian).ToList<Equipment>();
			for (int i = 0; i < list2.Count; i++)
			{
				list.Add(new InquiryElement(i, (i + 1).ToString() + " : " + ((list2[i][CustomUnitsBehavior.ToEquipmentSlot(equipmentType)].Item == null) ? "Empty" : list2[i][CustomUnitsBehavior.ToEquipmentSlot(equipmentType)].Item.Name.ToString()), (list2[i][CustomUnitsBehavior.ToEquipmentSlot(equipmentType)].Item == null) ? new ImageIdentifier(ImageIdentifierType.Null) : new ImageIdentifier(list2[i][CustomUnitsBehavior.ToEquipmentSlot(equipmentType)].Item, "")));
			}
			MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData("Select variartions sets to change", "", list, true, 0, list2.Count, "Continue", null, delegate(List<InquiryElement> args)
			{
				if (args == null || args.Any<InquiryElement>())
				{
					InformationManager.HideInquiry();
					CustomUnitsBehavior.updateSlots = new List<int>();
					foreach (InquiryElement inquiryElement in args)
					{
						CustomUnitsBehavior.updateSlots.Add((int)inquiryElement.Identifier);
					}
					SubModule.ExecuteActionOnNextTick(new Action(base.<UpgradeGear>g__ExecuteUpgrade|1));
				}
			}, null, "", false), false, false);
		}

		// Token: 0x060000FE RID: 254 RVA: 0x00009EAC File Offset: 0x000080AC
		internal static void CopyTemplate(string unitId)
		{
			List<InquiryElement> list = new List<InquiryElement>();
			CharacterObject editedUnit = Game.Current.ObjectManager.GetObject<CharacterObject>(unitId);
			foreach (CharacterObject characterObject in Campaign.Current.Characters)
			{
				if (characterObject != null && !characterObject.IsTemplate && !characterObject.HiddenInEncylopedia && characterObject.HeroObject == null && (characterObject.Tier <= editedUnit.Tier || CustomUnitsBehavior._disable_gear_restriction) && (characterObject.Occupation == Occupation.Soldier || characterObject.Occupation == Occupation.Mercenary || characterObject.Occupation == Occupation.Bandit || characterObject.Occupation == Occupation.Gangster || characterObject.Occupation == Occupation.CaravanGuard))
				{
					list.Add(new InquiryElement(characterObject, characterObject.Name.ToString(), new ImageIdentifier(CharacterCode.CreateFrom(characterObject)), true, CustomUnitsBehavior.GetHint(characterObject)));
				}
			}
			list.Sort(new Comparison<InquiryElement>(CustomUnitsBehavior.Compare2));
			MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData("Select template to copy from", "", list, true, 0, 1, "Continue", null, delegate(List<InquiryElement> args)
			{
				if (args == null || args.Any<InquiryElement>())
				{
					InformationManager.HideInquiry();
					CharacterObject template = args.Select((InquiryElement element) => element.Identifier as CharacterObject).First<CharacterObject>();
					SubModule.ExecuteActionOnNextTick(delegate
					{
						CustomUnitsBehavior.CopyCharacter(template, editedUnit);
						CustomUnitsBehavior.update(unitId, true);
					});
				}
			}, null, "", true), false, false);
		}

		// Token: 0x060000FF RID: 255 RVA: 0x0000A00C File Offset: 0x0000820C
		private static string GetHint(CharacterObject character)
		{
			string text = "";
			text = text + "level : " + character.Level.ToString() + "\n";
			text = text + "tier : " + character.Tier.ToString() + "\n";
			text = text + "culture : " + character.Culture.ToString() + "\n";
			text = text + "one handed : " + character.GetSkillValue(DefaultSkills.OneHanded).ToString() + "\n";
			text = text + "two handed : " + character.GetSkillValue(DefaultSkills.TwoHanded).ToString() + "\n";
			text = text + "polearm : " + character.GetSkillValue(DefaultSkills.Polearm).ToString() + "\n";
			text = text + "bow : " + character.GetSkillValue(DefaultSkills.Bow).ToString() + "\n";
			text = text + "crossbow : " + character.GetSkillValue(DefaultSkills.Crossbow).ToString() + "\n";
			text = text + "throwing : " + character.GetSkillValue(DefaultSkills.Throwing).ToString() + "\n";
			text = text + "riding : " + character.GetSkillValue(DefaultSkills.Riding).ToString() + "\n";
			return text + "athletics : " + character.GetSkillValue(DefaultSkills.Athletics).ToString() + "\n";
		}

		// Token: 0x06000100 RID: 256 RVA: 0x0000A1A0 File Offset: 0x000083A0
		public static void UpgradeGear2(string equipmentType, string unitId)
		{
			List<ItemObject> list = new List<ItemObject>();
			CharacterObject @object = Game.Current.ObjectManager.GetObject<CharacterObject>(unitId);
			foreach (ItemObject itemObject in MBObjectManager.Instance.GetObjectTypeList<ItemObject>())
			{
				bool flag = CustomUnitsBehavior.getRequiredItemType(CustomUnitsBehavior.ToEquipmentSlot(equipmentType)).Contains(itemObject.Type) && (itemObject.Tier <= (ItemObject.ItemTiers)@object.Tier || CustomUnitsBehavior._disable_gear_restriction) && CustomUnitsBehavior.MatchesFilter(itemObject);
				if (flag)
				{
					list.Add(itemObject);
				}
			}
			EquipmentSelectorBehavior.CreateVMLayer(list, @object, equipmentType);
		}

		// Token: 0x06000101 RID: 257 RVA: 0x0000A25C File Offset: 0x0000845C
		private static bool MatchesFilter(ItemObject item)
		{
			bool flag = CustomUnitsBehavior._filter_tiers.Contains((int)(item.Tier + 1));
			bool flag2 = CustomUnitsBehavior._filter_Culture.IsEmpty<CultureObject>();
			bool flag3 = flag2 || (item.Culture != null && CustomUnitsBehavior._filter_Culture.Contains(item.Culture));
			bool flag4 = CustomUnitsBehavior._filter_weapon_types.IsEmpty<ItemObject.ItemTypeEnum>() || (item != null && !CustomUnitsBehavior.isWeaponType(item.ItemType));
			bool flag5 = flag4 || CustomUnitsBehavior._filter_weapon_types.Contains(item.ItemType);
			bool flag6 = CustomUnitsBehavior._filter_armour_types.IsEmpty<ArmorComponent.ArmorMaterialTypes>() || item == null || item.ArmorComponent == null;
			bool flag7 = flag6 || CustomUnitsBehavior._filter_armour_types.Contains(item.ArmorComponent.MaterialType);
			return flag && flag3 && flag5 && flag7;
		}

		// Token: 0x06000102 RID: 258 RVA: 0x0000A340 File Offset: 0x00008540
		internal static void FilterWeapons()
		{
			List<InquiryElement> list = new List<InquiryElement>();
			list.Add(new InquiryElement("Arrows", "Arrows", null));
			list.Add(new InquiryElement("Bolts", "Bolts", null));
			list.Add(new InquiryElement("Bow", "Bow", null));
			list.Add(new InquiryElement("Bullets", "Bullets", null));
			list.Add(new InquiryElement("Crossbow", "Crossbow", null));
			list.Add(new InquiryElement("Musket", "Musket", null));
			list.Add(new InquiryElement("One Handed Weapon", "One Handed Weapon", null));
			list.Add(new InquiryElement("Pistol", "Pistol", null));
			list.Add(new InquiryElement("Polearm", "Polearm", null));
			list.Add(new InquiryElement("Shield", "Shield", null));
			list.Add(new InquiryElement("Thrown", "Thrown", null));
			list.Add(new InquiryElement("Two Handed Weapon", "Two Handed Weapon", null));
			MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData("Filter by weapon type", "", list, true, 0, list.Count, "Continue", null, delegate(List<InquiryElement> args)
			{
				if (args == null || args.Any<InquiryElement>())
				{
					InformationManager.HideInquiry();
					IEnumerable<string> enumerable = args.Select((InquiryElement element) => element.Identifier as string);
					CustomUnitsBehavior._filter_weapon_types.Clear();
					foreach (string text in enumerable)
					{
						CustomUnitsBehavior._filter_weapon_types.Add(CustomUnitsBehavior.stringToWeaponType(text));
					}
					CustomUnitsBehavior.customUnitVM.RefreshValues();
				}
			}, null, "", true), false, false);
		}

		// Token: 0x06000103 RID: 259 RVA: 0x0000A4A8 File Offset: 0x000086A8
		internal static void FilterArmour()
		{
			List<InquiryElement> list = new List<InquiryElement>();
			list.Add(new InquiryElement("Chainmail", "Chainmail", null));
			list.Add(new InquiryElement("Cloth", "Cloth", null));
			list.Add(new InquiryElement("Leather", "Leather", null));
			list.Add(new InquiryElement("Plate", "Plate", null));
			MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData("Filter by armour type", "", list, true, 0, list.Count, "Continue", null, delegate(List<InquiryElement> args)
			{
				if (args == null || args.Any<InquiryElement>())
				{
					InformationManager.HideInquiry();
					IEnumerable<string> enumerable = args.Select((InquiryElement element) => element.Identifier as string);
					CustomUnitsBehavior._filter_armour_types.Clear();
					foreach (string text in enumerable)
					{
						CustomUnitsBehavior._filter_armour_types.Add(CustomUnitsBehavior.stringToArmourType(text));
					}
					CustomUnitsBehavior.customUnitVM.RefreshValues();
				}
			}, null, "", true), false, false);
		}

		// Token: 0x06000104 RID: 260 RVA: 0x0000A560 File Offset: 0x00008760
		private static ArmorComponent.ArmorMaterialTypes stringToArmourType(string s)
		{
			ArmorComponent.ArmorMaterialTypes armorMaterialTypes;
			if (!(s == "Chainmail"))
			{
				if (!(s == "Cloth"))
				{
					if (!(s == "Leather"))
					{
						if (!(s == "Plate"))
						{
							armorMaterialTypes = ArmorComponent.ArmorMaterialTypes.None;
						}
						else
						{
							armorMaterialTypes = ArmorComponent.ArmorMaterialTypes.Plate;
						}
					}
					else
					{
						armorMaterialTypes = ArmorComponent.ArmorMaterialTypes.Leather;
					}
				}
				else
				{
					armorMaterialTypes = ArmorComponent.ArmorMaterialTypes.Cloth;
				}
			}
			else
			{
				armorMaterialTypes = ArmorComponent.ArmorMaterialTypes.Chainmail;
			}
			return armorMaterialTypes;
		}

		// Token: 0x06000105 RID: 261 RVA: 0x0000A5C0 File Offset: 0x000087C0
		private static ItemObject.ItemTypeEnum stringToWeaponType(string s)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(s);
			if (num <= 2996768862U)
			{
				if (num <= 1048100111U)
				{
					if (num != 163970187U)
					{
						if (num != 731742070U)
						{
							if (num == 1048100111U)
							{
								if (s == "Bolts")
								{
									return ItemObject.ItemTypeEnum.Bolts;
								}
							}
						}
						else if (s == "Pistol")
						{
							return ItemObject.ItemTypeEnum.Pistol;
						}
					}
					else if (s == "One Handed Weapon")
					{
						return ItemObject.ItemTypeEnum.OneHandedWeapon;
					}
				}
				else if (num != 1387635315U)
				{
					if (num != 2039097040U)
					{
						if (num == 2996768862U)
						{
							if (s == "Bullets")
							{
								return ItemObject.ItemTypeEnum.Bullets;
							}
						}
					}
					else if (s == "Shield")
					{
						return ItemObject.ItemTypeEnum.Shield;
					}
				}
				else if (s == "Thrown")
				{
					return ItemObject.ItemTypeEnum.Thrown;
				}
			}
			else if (num <= 3565557811U)
			{
				if (num != 3083591375U)
				{
					if (num != 3331724581U)
					{
						if (num == 3565557811U)
						{
							if (s == "Polearm")
							{
								return ItemObject.ItemTypeEnum.Polearm;
							}
						}
					}
					else if (s == "Two Handed Weapon")
					{
						return ItemObject.ItemTypeEnum.TwoHandedWeapon;
					}
				}
				else if (s == "Arrows")
				{
					return ItemObject.ItemTypeEnum.Arrows;
				}
			}
			else if (num != 3618788796U)
			{
				if (num != 3637216139U)
				{
					if (num == 4282369777U)
					{
						if (s == "Crossbow")
						{
							return ItemObject.ItemTypeEnum.Crossbow;
						}
					}
				}
				else if (s == "Bow")
				{
					return ItemObject.ItemTypeEnum.Bow;
				}
			}
			else if (s == "Musket")
			{
				return ItemObject.ItemTypeEnum.Musket;
			}
			return ItemObject.ItemTypeEnum.Invalid;
		}

		// Token: 0x06000106 RID: 262 RVA: 0x0000A7A4 File Offset: 0x000089A4
		internal static void FilterCultures()
		{
			List<CultureObject> list = new List<CultureObject>();
			List<InquiryElement> list2 = new List<InquiryElement>();
			foreach (Kingdom kingdom in Campaign.Current.Kingdoms)
			{
				if (kingdom != null && !list.Contains(kingdom.Culture))
				{
					list.Add(kingdom.Culture);
					list2.Add(new InquiryElement(kingdom.Culture, kingdom.Culture.Name.ToString(), new ImageIdentifier(kingdom.Banner)));
				}
			}
			MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData("Filter by equipment culture", "", list2, true, 0, list2.Count, "Continue", null, delegate(List<InquiryElement> args)
			{
				if (args == null || args.Any<InquiryElement>())
				{
					InformationManager.HideInquiry();
					IEnumerable<CultureObject> enumerable = args.Select((InquiryElement element) => element.Identifier as CultureObject);
					CustomUnitsBehavior._filter_Culture.Clear();
					foreach (CultureObject cultureObject in enumerable)
					{
						CustomUnitsBehavior._filter_Culture.Add(cultureObject);
					}
					CustomUnitsBehavior.customUnitVM.RefreshValues();
				}
			}, null, "", true), false, false);
		}

		// Token: 0x06000107 RID: 263 RVA: 0x0000A894 File Offset: 0x00008A94
		internal static void FilterTiers()
		{
			List<InquiryElement> list = new List<InquiryElement>();
			list.Add(new InquiryElement("0", "0", null));
			list.Add(new InquiryElement("1", "1", null));
			list.Add(new InquiryElement("2", "2", null));
			list.Add(new InquiryElement("3", "3", null));
			list.Add(new InquiryElement("4", "4", null));
			list.Add(new InquiryElement("5", "5", null));
			list.Add(new InquiryElement("6", "6", null));
			MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData("Filter by equipment tier", "", list, true, 0, 6, "Continue", null, delegate(List<InquiryElement> args)
			{
				if (args == null || args.Any<InquiryElement>())
				{
					InformationManager.HideInquiry();
					IEnumerable<string> enumerable = args.Select((InquiryElement element) => element.Identifier as string);
					CustomUnitsBehavior._filter_tiers.Clear();
					foreach (string text in enumerable)
					{
						CustomUnitsBehavior._filter_tiers.Add(int.Parse(text));
					}
					CustomUnitsBehavior.customUnitVM.RefreshValues();
				}
			}, null, "", false), false, false);
		}

		// Token: 0x06000108 RID: 264 RVA: 0x0000A988 File Offset: 0x00008B88
		private static bool isWeaponType(ItemObject.ItemTypeEnum type)
		{
			return type - ItemObject.ItemTypeEnum.OneHandedWeapon <= 8 || type - ItemObject.ItemTypeEnum.Pistol <= 2;
		}

		// Token: 0x06000109 RID: 265 RVA: 0x0000A9B4 File Offset: 0x00008BB4
		public static void UpgradeGearFinalize(EquipmentIndex equipmentIndex, ItemObject item, string unitId)
		{
			foreach (int num in CustomUnitsBehavior.updateSlots)
			{
				CustomUnitsBehavior.ChangeUnitEquipment((int)equipmentIndex, Game.Current.ObjectManager.GetObject<CharacterObject>(unitId), item, num, false);
			}
			CustomUnitsBehavior.update(unitId, true);
		}

		// Token: 0x0600010A RID: 266 RVA: 0x0000AA28 File Offset: 0x00008C28
		public static void update(string unitId, bool refresh = true)
		{
			CustomUnitsBehavior.SetDefaultGroup(unitId);
			if (refresh)
			{
				CustomUnitsBehavior.customUnitVM.RefreshValues();
			}
			CustomUnit customUnit;
			bool flag = CustomUnitsBehavior.CustomUnits.TryGetValue(unitId, out customUnit);
			if (flag)
			{
				customUnit.SaveUpdate();
			}
			else
			{
				CustomUnitsBehavior.CustomUnits.Add(unitId, new CustomUnit(unitId));
			}
			CustomUnit customUnit2;
			bool flag2 = CustomUnitsBehavior.CustomUnits.TryGetValue(unitId + "0", out customUnit2);
			if (flag2)
			{
				customUnit2.checkHorseNeeded();
			}
			CustomUnit customUnit3;
			bool flag3 = CustomUnitsBehavior.CustomUnits.TryGetValue(unitId + "1", out customUnit3);
			if (flag3)
			{
				customUnit3.checkHorseNeeded();
			}
		}

		// Token: 0x0600010B RID: 267 RVA: 0x0000AAD0 File Offset: 0x00008CD0
		private static void ChangeUnitEquipment(int slot, CharacterObject character, ItemObject item, int set, bool isCivilian = false)
		{
			List<Equipment> list = character.AllEquipments.Where((Equipment x) => x.IsCivilian).ToList<Equipment>();
			List<Equipment> list2 = character.AllEquipments.Where((Equipment x) => !x.IsCivilian).ToList<Equipment>();
			EquipmentElement equipmentElement = ((item == null) ? default(EquipmentElement) : new EquipmentElement(item, null, null, false));
			if (isCivilian)
			{
				list[set][slot] = equipmentElement;
			}
			else
			{
				list2[set][slot] = equipmentElement;
			}
			list2.AddRange(list);
			CustomUnitsBehavior.UpdateSelectedUnitEquipment(character, list2);
		}

		// Token: 0x0600010C RID: 268 RVA: 0x0000AB90 File Offset: 0x00008D90
		private static void UpdateSelectedUnitEquipment(CharacterObject character, List<Equipment> equipments)
		{
			MBEquipmentRoster mbequipmentRoster = new MBEquipmentRoster();
			((FieldInfo)CustomUnitsBehavior.GetInstanceField<MBEquipmentRoster>(mbequipmentRoster, "_equipments")).SetValue(mbequipmentRoster, new MBList<Equipment>(equipments));
			((FieldInfo)CustomUnitsBehavior.GetInstanceField<BasicCharacterObject>(character, "_equipmentRoster")).SetValue(character, mbequipmentRoster);
			character.InitializeEquipmentsOnLoad(character);
		}

		// Token: 0x0600010D RID: 269 RVA: 0x0000ABE4 File Offset: 0x00008DE4
		internal static object GetInstanceField<T>(T instance, string fieldName)
		{
			BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			return typeof(T).GetField(fieldName, bindingFlags);
		}

		// Token: 0x0600010E RID: 270 RVA: 0x0000AC0C File Offset: 0x00008E0C
		private static int Compare2(InquiryElement x, InquiryElement y)
		{
			bool flag = x == null || x.Identifier == null;
			int num;
			if (flag)
			{
				num = -1;
			}
			else
			{
				bool flag2 = y == null || y.Identifier == null;
				if (flag2)
				{
					num = 1;
				}
				else
				{
					num = string.Compare(((CharacterObject)x.Identifier).Name.ToString(), ((CharacterObject)y.Identifier).Name.ToString());
				}
			}
			return num;
		}

		// Token: 0x0600010F RID: 271 RVA: 0x0000AC80 File Offset: 0x00008E80
		public static void UpdateSkill(SkillObject skill, int amount, string unitId)
		{
			CharacterObject @object = Game.Current.ObjectManager.GetObject<CharacterObject>(unitId);
			MBCharacterSkills mbcharacterSkills = MBObjectManager.Instance.CreateObject<MBCharacterSkills>(@object.StringId);
			mbcharacterSkills.Skills.SetPropertyValue(DefaultSkills.Crossbow, @object.GetSkillValue(DefaultSkills.Crossbow));
			mbcharacterSkills.Skills.SetPropertyValue(DefaultSkills.Bow, @object.GetSkillValue(DefaultSkills.Bow));
			mbcharacterSkills.Skills.SetPropertyValue(DefaultSkills.Throwing, @object.GetSkillValue(DefaultSkills.Throwing));
			mbcharacterSkills.Skills.SetPropertyValue(DefaultSkills.OneHanded, @object.GetSkillValue(DefaultSkills.OneHanded));
			mbcharacterSkills.Skills.SetPropertyValue(DefaultSkills.TwoHanded, @object.GetSkillValue(DefaultSkills.TwoHanded));
			mbcharacterSkills.Skills.SetPropertyValue(DefaultSkills.Polearm, @object.GetSkillValue(DefaultSkills.Polearm));
			mbcharacterSkills.Skills.SetPropertyValue(DefaultSkills.Athletics, @object.GetSkillValue(DefaultSkills.Athletics));
			mbcharacterSkills.Skills.SetPropertyValue(DefaultSkills.Riding, @object.GetSkillValue(DefaultSkills.Riding));
			mbcharacterSkills.Skills.SetPropertyValue(skill, Math.Min(CustomUnitsBehavior.skillCap(unitId), (@object.GetSkillValue(skill) + amount > 0) ? (@object.GetSkillValue(skill) + amount) : 0));
			FieldInfo field = @object.GetType().GetField("DefaultCharacterSkills", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (null != field)
			{
				field.SetValue(@object, mbcharacterSkills);
			}
			CustomUnitsBehavior.update(unitId, true);
		}

		// Token: 0x06000110 RID: 272 RVA: 0x0000ADE4 File Offset: 0x00008FE4
		public static int skillCap(string unitId)
		{
			bool disable_skill_cap_restriction = CustomUnitsBehavior._disable_skill_cap_restriction;
			int num;
			if (disable_skill_cap_restriction)
			{
				num = 1023;
			}
			else
			{
				CharacterObject @object = Game.Current.ObjectManager.GetObject<CharacterObject>(unitId);
				switch (@object.Tier)
				{
				case 0:
				case 1:
					num = 25;
					break;
				case 2:
					num = 50;
					break;
				case 3:
					num = 90;
					break;
				case 4:
					num = 120;
					break;
				case 5:
					num = 170;
					break;
				case 6:
					num = 260;
					break;
				default:
					num = 330;
					break;
				}
			}
			return num;
		}

		// Token: 0x06000111 RID: 273 RVA: 0x0000AE78 File Offset: 0x00009078
		public static void rename(string unitId)
		{
			InformationManager.ShowTextInquiry(new TextInquiryData("Rename", "Enter new name", true, true, "Procede", "Cancel", delegate(string s)
			{
				CustomUnitsBehavior.newName(s, unitId);
			}, null, false, null, "", ""), false, false);
		}

		// Token: 0x06000112 RID: 274 RVA: 0x0000AED0 File Offset: 0x000090D0
		private static void newName(string s, string unitId)
		{
			CharacterObject @object = Game.Current.ObjectManager.GetObject<CharacterObject>(unitId);
			typeof(BasicCharacterObject).GetMethod("SetName", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(@object, new object[]
			{
				new TextObject(s, null)
			});
			CustomUnitsBehavior.update(unitId, true);
		}

		// Token: 0x06000113 RID: 275 RVA: 0x0000AF24 File Offset: 0x00009124
		public static void ChangeCulture(string unitId)
		{
			List<CultureObject> list = new List<CultureObject>();
			List<InquiryElement> list2 = new List<InquiryElement>();
			foreach (Kingdom kingdom in Campaign.Current.Kingdoms)
			{
				if (kingdom != null && !list.Contains(kingdom.Culture))
				{
					list.Add(kingdom.Culture);
					list2.Add(new InquiryElement(kingdom.Culture, kingdom.Culture.Name.ToString(), new ImageIdentifier(kingdom.Banner)));
				}
			}
			MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData("Select Culture", "", list2, true, 1, 1, "Continue", null, delegate(List<InquiryElement> args)
			{
				if (args == null || args.Any<InquiryElement>())
				{
					InformationManager.HideInquiry();
					CultureObject culture = args.Select((InquiryElement element) => element.Identifier as CultureObject).First<CultureObject>();
					SubModule.ExecuteActionOnNextTick(delegate
					{
						CustomUnitsBehavior.SetCulture(culture, unitId);
					});
				}
			}, null, "", true), false, false);
		}

		// Token: 0x06000114 RID: 276 RVA: 0x0000B010 File Offset: 0x00009210
		private static void SetCulture(CultureObject culture, string unitId)
		{
			CharacterObject @object = Game.Current.ObjectManager.GetObject<CharacterObject>(unitId);
			@object.Culture = culture;
			CustomUnitsBehavior.updateAppearance(@object);
			CustomUnitsBehavior.update(unitId, true);
		}

		// Token: 0x06000115 RID: 277 RVA: 0x0000B048 File Offset: 0x00009248
		public static void ChangeGender(string unitId)
		{
			CharacterObject @object = Game.Current.ObjectManager.GetObject<CharacterObject>(unitId);
			@object.IsFemale = !@object.IsFemale;
			CustomUnitsBehavior.updateAppearance(@object);
			CustomUnitsBehavior.update(unitId, true);
		}

		// Token: 0x06000116 RID: 278 RVA: 0x0000B088 File Offset: 0x00009288
		public static void AddUpgrade(string unitId)
		{
			CustomUnitsBehavior.update(unitId, true);
			CharacterObject @object = Game.Current.ObjectManager.GetObject<CharacterObject>(unitId);
			List<CharacterObject> list = new List<CharacterObject>();
			if (@object.UpgradeTargets != null)
			{
				for (int i = 0; i < @object.UpgradeTargets.Length; i++)
				{
					list.Add(@object.UpgradeTargets[i]);
				}
			}
			if (list.Count <= 1)
			{
				if (Game.Current.ObjectManager.GetObject<CharacterObject>(unitId + "0") != null && !list.Contains(Game.Current.ObjectManager.GetObject<CharacterObject>(unitId + "0")))
				{
					CharacterObject object2 = Game.Current.ObjectManager.GetObject<CharacterObject>(unitId + "0");
					CustomUnitsBehavior.CopyCharacter(@object, object2);
					list.Add(object2);
					CustomUnitsBehavior.update(unitId + "0", true);
				}
				else
				{
					if (Game.Current.ObjectManager.GetObject<CharacterObject>(unitId + "1") == null || list.Contains(Game.Current.ObjectManager.GetObject<CharacterObject>(unitId + "1")))
					{
						InformationManager.DisplayMessage(new InformationMessage("This unit is max tier"));
						return;
					}
					CharacterObject object3 = Game.Current.ObjectManager.GetObject<CharacterObject>(unitId + "1");
					CustomUnitsBehavior.CopyCharacter(@object, object3);
					list.Add(object3);
					CustomUnitsBehavior.update(unitId + "1", true);
				}
				typeof(CharacterObject).GetProperty("UpgradeTargets").SetValue(@object, list.ToArray(), null);
				CustomUnitsBehavior.update(unitId, true);
			}
		}

		// Token: 0x06000117 RID: 279 RVA: 0x0000B218 File Offset: 0x00009418
		public static void RemoveUpgrade(string unitId, CharacterObject upgrade)
		{
			CharacterObject @object = Game.Current.ObjectManager.GetObject<CharacterObject>(unitId);
			List<CharacterObject> list = new List<CharacterObject>();
			for (int i = 0; i < @object.UpgradeTargets.Length; i++)
			{
				list.Add(@object.UpgradeTargets[i]);
			}
			list.Remove(upgrade);
			typeof(CharacterObject).GetProperty("UpgradeTargets").SetValue(@object, (list.Count > 0) ? list.ToArray() : new CharacterObject[0], null);
			CustomUnitsBehavior.update(unitId, true);
		}

		// Token: 0x06000118 RID: 280 RVA: 0x0000B2A8 File Offset: 0x000094A8
		public static void SetDefaultGroup(string unitId)
		{
			CharacterObject @object = Game.Current.ObjectManager.GetObject<CharacterObject>(unitId);
			bool flag = CustomUnitsBehavior.isRanged(@object) && @object.Equipment[10].Item != null;
			if (flag)
			{
				typeof(CharacterObject).GetProperty("DefaultFormationClass").SetValue(@object, FormationClass.HorseArcher, null);
				@object.DefaultFormationGroup = 3;
			}
			else
			{
				bool flag2 = @object.Equipment[10].Item != null;
				if (flag2)
				{
					typeof(CharacterObject).GetProperty("DefaultFormationClass").SetValue(@object, FormationClass.Cavalry, null);
					@object.DefaultFormationGroup = 2;
				}
				else
				{
					bool flag3 = CustomUnitsBehavior.isRanged(@object);
					if (flag3)
					{
						typeof(CharacterObject).GetProperty("DefaultFormationClass").SetValue(@object, FormationClass.Ranged, null);
						@object.DefaultFormationGroup = 1;
					}
					else
					{
						typeof(CharacterObject).GetProperty("DefaultFormationClass").SetValue(@object, FormationClass.Infantry, null);
						@object.DefaultFormationGroup = 0;
					}
				}
			}
		}

		// Token: 0x06000119 RID: 281 RVA: 0x0000B3D0 File Offset: 0x000095D0
		private static bool isRanged(CharacterObject character)
		{
			for (int i = 0; i < 4; i++)
			{
				ItemObject item = character.Equipment[i].Item;
				bool flag = item != null && (item.ItemType == ItemObject.ItemTypeEnum.Bow || item.ItemType == ItemObject.ItemTypeEnum.Crossbow);
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600011A RID: 282 RVA: 0x0000B434 File Offset: 0x00009634
		public static void CopyCharacter(CharacterObject orginalCharacter, CharacterObject newCharacter)
		{
			EquipmentIndex[] array = new EquipmentIndex[]
			{
				EquipmentIndex.WeaponItemBeginSlot,
				EquipmentIndex.Weapon1,
				EquipmentIndex.Weapon2,
				EquipmentIndex.Weapon3,
				EquipmentIndex.NumAllWeaponSlots,
				EquipmentIndex.Cape,
				EquipmentIndex.Body,
				EquipmentIndex.Gloves,
				EquipmentIndex.Leg,
				EquipmentIndex.ArmorItemEndSlot,
				EquipmentIndex.HorseHarness
			};
			CustomUnit customUnit;
			if (CustomUnitsBehavior.CustomUnits.TryGetValue(orginalCharacter.StringId, out customUnit))
			{
				foreach (EquipmentIndex equipmentIndex in array)
				{
					for (int j = 0; j < customUnit.Equipment.Length; j++)
					{
						if (customUnit.Equipment[j] != null)
						{
							if (customUnit.Equipment[j].GetEquipmentFromSlot(equipmentIndex).Item != null && customUnit.Equipment[j].GetEquipmentFromSlot(equipmentIndex).Item.Name != null)
							{
								CustomUnitsBehavior.ChangeUnitEquipment((int)equipmentIndex, newCharacter, customUnit.Equipment[j].GetEquipmentFromSlot(equipmentIndex).Item, j, false);
							}
							else
							{
								CustomUnitsBehavior.ChangeUnitEquipment((int)equipmentIndex, newCharacter, null, j, false);
							}
						}
					}
				}
				typeof(CharacterObject).GetProperty("UpgradeTargets").SetValue(newCharacter, new CharacterObject[0], null);
			}
			else
			{
				customUnit = new CustomUnit(orginalCharacter.StringId);
				foreach (EquipmentIndex equipmentIndex2 in array)
				{
					int num = 0;
					for (;;)
					{
						if (num >= newCharacter.AllEquipments.Where((Equipment x) => !x.IsCivilian).ToList<Equipment>().Count)
						{
							break;
						}
						if (customUnit.Equipment[num % customUnit.Equipment.Length] != null)
						{
							if (customUnit.Equipment[num % customUnit.Equipment.Length].GetEquipmentFromSlot(equipmentIndex2).Item != null && customUnit.Equipment[num % customUnit.Equipment.Length].GetEquipmentFromSlot(equipmentIndex2).Item.Name != null)
							{
								CustomUnitsBehavior.ChangeUnitEquipment((int)equipmentIndex2, newCharacter, customUnit.Equipment[num % customUnit.Equipment.Length].GetEquipmentFromSlot(equipmentIndex2).Item, num, false);
							}
							else
							{
								CustomUnitsBehavior.ChangeUnitEquipment((int)equipmentIndex2, newCharacter, null, num, false);
							}
						}
						num++;
					}
				}
			}
			MBCharacterSkills mbcharacterSkills = MBObjectManager.Instance.CreateObject<MBCharacterSkills>(newCharacter.StringId);
			mbcharacterSkills.Skills.SetPropertyValue(DefaultSkills.Crossbow, customUnit.Crossbow);
			mbcharacterSkills.Skills.SetPropertyValue(DefaultSkills.Bow, customUnit.Bow);
			mbcharacterSkills.Skills.SetPropertyValue(DefaultSkills.Throwing, customUnit.Throwing);
			mbcharacterSkills.Skills.SetPropertyValue(DefaultSkills.OneHanded, customUnit.OneHand);
			mbcharacterSkills.Skills.SetPropertyValue(DefaultSkills.TwoHanded, customUnit.TwoHand);
			mbcharacterSkills.Skills.SetPropertyValue(DefaultSkills.Polearm, customUnit.Polearm);
			mbcharacterSkills.Skills.SetPropertyValue(DefaultSkills.Athletics, customUnit.Athletics);
			mbcharacterSkills.Skills.SetPropertyValue(DefaultSkills.Riding, customUnit.Riding);
			FieldInfo field = newCharacter.GetType().GetField("DefaultCharacterSkills", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (null != field)
			{
				field.SetValue(newCharacter, mbcharacterSkills);
			}
			newCharacter.IsFemale = orginalCharacter.IsFemale;
			newCharacter.Culture = orginalCharacter.Culture;
			AccessTools.Property(typeof(CharacterObject), "BodyPropertyRange").SetValue(newCharacter, orginalCharacter.BodyPropertyRange, null);
		}

		// Token: 0x0600011B RID: 283 RVA: 0x0000B7B8 File Offset: 0x000099B8
		private static void updateAppearance(CharacterObject character)
		{
			bool flag = (character.IsFemale ? character.Culture.Townswoman : character.Culture.Townsman) != null;
			if (flag)
			{
				MBBodyProperty mbbodyProperty = (character.IsFemale ? character.Culture.FemaleDancer.BodyPropertyRange : character.Culture.BasicTroop.BodyPropertyRange);
				AccessTools.Property(typeof(CharacterObject), "BodyPropertyRange").SetValue(character, mbbodyProperty, null);
			}
			else
			{
				MBBodyProperty mbbodyProperty2 = (character.IsFemale ? Game.Current.ObjectManager.GetObject<CharacterObject>("female_dancer_empire").BodyPropertyRange : Game.Current.ObjectManager.GetObject<CharacterObject>("imperial_recruit").BodyPropertyRange);
				AccessTools.Property(typeof(CharacterObject), "BodyPropertyRange").SetValue(character, mbbodyProperty2, null);
			}
		}

		// Token: 0x0600011C RID: 284 RVA: 0x0000B898 File Offset: 0x00009A98
		public static EquipmentIndex ToEquipmentSlot(string equipment)
		{
			bool flag = equipment == "Wep0";
			EquipmentIndex equipmentIndex;
			if (flag)
			{
				equipmentIndex = EquipmentIndex.WeaponItemBeginSlot;
			}
			else
			{
				bool flag2 = equipment == "Wep1";
				if (flag2)
				{
					equipmentIndex = EquipmentIndex.Weapon1;
				}
				else
				{
					bool flag3 = equipment == "Wep2";
					if (flag3)
					{
						equipmentIndex = EquipmentIndex.Weapon2;
					}
					else
					{
						bool flag4 = equipment == "Wep3";
						if (flag4)
						{
							equipmentIndex = EquipmentIndex.Weapon3;
						}
						else
						{
							bool flag5 = equipment == "Head";
							if (flag5)
							{
								equipmentIndex = EquipmentIndex.NumAllWeaponSlots;
							}
							else
							{
								bool flag6 = equipment == "Cape";
								if (flag6)
								{
									equipmentIndex = EquipmentIndex.Cape;
								}
								else
								{
									bool flag7 = equipment == "Body";
									if (flag7)
									{
										equipmentIndex = EquipmentIndex.Body;
									}
									else
									{
										bool flag8 = equipment == "Gloves";
										if (flag8)
										{
											equipmentIndex = EquipmentIndex.Gloves;
										}
										else
										{
											bool flag9 = equipment == "Leg";
											if (flag9)
											{
												equipmentIndex = EquipmentIndex.Leg;
											}
											else
											{
												bool flag10 = equipment == "Horse";
												if (flag10)
												{
													equipmentIndex = EquipmentIndex.ArmorItemEndSlot;
												}
												else
												{
													bool flag11 = equipment == "Harness";
													if (flag11)
													{
														equipmentIndex = EquipmentIndex.HorseHarness;
													}
													else
													{
														equipmentIndex = EquipmentIndex.None;
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return equipmentIndex;
		}

		// Token: 0x0600011D RID: 285 RVA: 0x0000B9AC File Offset: 0x00009BAC
		private static List<ItemObject.ItemTypeEnum> getRequiredItemType(EquipmentIndex equipmentIndex)
		{
			List<ItemObject.ItemTypeEnum> list = new List<ItemObject.ItemTypeEnum>();
			bool flag = equipmentIndex == EquipmentIndex.WeaponItemBeginSlot || equipmentIndex == EquipmentIndex.Weapon1 || equipmentIndex == EquipmentIndex.Weapon2 || equipmentIndex == EquipmentIndex.Weapon3;
			if (flag)
			{
				list.Add(ItemObject.ItemTypeEnum.Arrows);
				list.Add(ItemObject.ItemTypeEnum.Bolts);
				list.Add(ItemObject.ItemTypeEnum.Bow);
				list.Add(ItemObject.ItemTypeEnum.Bullets);
				list.Add(ItemObject.ItemTypeEnum.Crossbow);
				list.Add(ItemObject.ItemTypeEnum.Musket);
				list.Add(ItemObject.ItemTypeEnum.OneHandedWeapon);
				list.Add(ItemObject.ItemTypeEnum.Pistol);
				list.Add(ItemObject.ItemTypeEnum.Polearm);
				list.Add(ItemObject.ItemTypeEnum.Shield);
				list.Add(ItemObject.ItemTypeEnum.Thrown);
				list.Add(ItemObject.ItemTypeEnum.TwoHandedWeapon);
			}
			else
			{
				bool flag2 = equipmentIndex == EquipmentIndex.NumAllWeaponSlots;
				if (flag2)
				{
					list.Add(ItemObject.ItemTypeEnum.HeadArmor);
				}
				else
				{
					bool flag3 = equipmentIndex == EquipmentIndex.Cape;
					if (flag3)
					{
						list.Add(ItemObject.ItemTypeEnum.Cape);
					}
					else
					{
						bool flag4 = equipmentIndex == EquipmentIndex.Body;
						if (flag4)
						{
							list.Add(ItemObject.ItemTypeEnum.BodyArmor);
						}
						else
						{
							bool flag5 = equipmentIndex == EquipmentIndex.Gloves;
							if (flag5)
							{
								list.Add(ItemObject.ItemTypeEnum.HandArmor);
							}
							else
							{
								bool flag6 = equipmentIndex == EquipmentIndex.Leg;
								if (flag6)
								{
									list.Add(ItemObject.ItemTypeEnum.LegArmor);
								}
								else
								{
									bool flag7 = equipmentIndex == EquipmentIndex.ArmorItemEndSlot;
									if (flag7)
									{
										list.Add(ItemObject.ItemTypeEnum.Horse);
									}
									else
									{
										bool flag8 = equipmentIndex == EquipmentIndex.HorseHarness;
										if (flag8)
										{
											list.Add(ItemObject.ItemTypeEnum.HorseHarness);
										}
									}
								}
							}
						}
					}
				}
			}
			return list;
		}

		// Token: 0x0600011E RID: 286 RVA: 0x0000250F File Offset: 0x0000070F
		public override void RegisterEvents()
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.GameStart));
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.MenuItems));
		}

		// Token: 0x0600011F RID: 287 RVA: 0x0000BAEC File Offset: 0x00009CEC
		private void MenuItems(CampaignGameStarter campaignGameSystemStarter)
		{
			campaignGameSystemStarter.AddGameMenuOption("town", "recruit_custom_volunteers", "Switch recruitment type", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				bool replaceAllForPlayer = SubModule.ReplaceAllForPlayer;
				if (replaceAllForPlayer)
				{
					InformationManager.DisplayMessage(new InformationMessage("Recruitment type set to native troops"));
				}
				else
				{
					InformationManager.DisplayMessage(new InformationMessage("Recruitment type set to custom troops"));
				}
				SubModule.ReplaceAllForPlayer = !SubModule.ReplaceAllForPlayer;
			}, false, 5, false, null);
			campaignGameSystemStarter.AddGameMenuOption("village", "recruit_custom_volunteers", "Switch recruitment type", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				bool replaceAllForPlayer2 = SubModule.ReplaceAllForPlayer;
				if (replaceAllForPlayer2)
				{
					InformationManager.DisplayMessage(new InformationMessage("Recruitment type set to native troops"));
				}
				else
				{
					InformationManager.DisplayMessage(new InformationMessage("Recruitment type set to custom troops"));
				}
				SubModule.ReplaceAllForPlayer = !SubModule.ReplaceAllForPlayer;
			}, false, 2, false, null);
		}

		// Token: 0x06000120 RID: 288 RVA: 0x0000BBAC File Offset: 0x00009DAC
		private void GameStart(CampaignGameStarter campaignStarter)
		{
			bool flag = CustomUnitsBehavior.CustomUnits == null;
			if (flag)
			{
				CustomUnitsBehavior.CustomUnits = new Dictionary<string, CustomUnit>();
			}
			foreach (KeyValuePair<string, CustomUnit> keyValuePair in CustomUnitsBehavior.CustomUnits)
			{
				bool flag2 = keyValuePair.Value.Id.Contains("_basic_root") || keyValuePair.Value.Id.Contains("_elite_root") || this.instance.FullUnitEditor;
				if (flag2)
				{
					keyValuePair.Value.Update();
				}
			}
			this.InitializeEquipmentFilterSettings();
		}

		// Token: 0x06000121 RID: 289 RVA: 0x0000BC6C File Offset: 0x00009E6C
		private void InitializeEquipmentFilterSettings()
		{
			CustomUnitsBehavior._filter_tiers.Clear();
			CustomUnitsBehavior._filter_tiers.Add(0);
			CustomUnitsBehavior._filter_tiers.Add(1);
			CustomUnitsBehavior._filter_tiers.Add(2);
			CustomUnitsBehavior._filter_tiers.Add(3);
			CustomUnitsBehavior._filter_tiers.Add(4);
			CustomUnitsBehavior._filter_tiers.Add(5);
			CustomUnitsBehavior._filter_tiers.Add(6);
		}

		// Token: 0x06000122 RID: 290 RVA: 0x0000BCDC File Offset: 0x00009EDC
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<string, CustomUnit>>("_custom_units", ref CustomUnitsBehavior.CustomUnits);
			dataStore.SyncData<bool>("_gear_restriction", ref CustomUnitsBehavior._disable_gear_restriction);
			dataStore.SyncData<bool>("_skill_total_restriction", ref CustomUnitsBehavior._disable_skill_total_restriction);
			dataStore.SyncData<bool>("_skill_cap_restriction", ref CustomUnitsBehavior._disable_skill_cap_restriction);
		}

		// Token: 0x0400005D RID: 93
		public static GauntletLayer layer;

		// Token: 0x0400005E RID: 94
		public static GauntletMovie gauntletMovie;

		// Token: 0x0400005F RID: 95
		public static CustomUnitsVM customUnitVM;

		// Token: 0x04000060 RID: 96
		public static Dictionary<string, CustomUnit> CustomUnits = new Dictionary<string, CustomUnit>();

		// Token: 0x04000061 RID: 97
		public static List<int> updateSlots;

		// Token: 0x04000062 RID: 98
		public static List<int> _filter_tiers = new List<int>();

		// Token: 0x04000063 RID: 99
		public static List<CultureObject> _filter_Culture = new List<CultureObject>();

		// Token: 0x04000064 RID: 100
		public static List<ItemObject.ItemTypeEnum> _filter_weapon_types = new List<ItemObject.ItemTypeEnum>();

		// Token: 0x04000065 RID: 101
		public static List<ArmorComponent.ArmorMaterialTypes> _filter_armour_types = new List<ArmorComponent.ArmorMaterialTypes>();

		// Token: 0x04000066 RID: 102
		private MyLittleWarbandSetting instance = GlobalSettings<MyLittleWarbandSetting>.Instance;

		// Token: 0x04000067 RID: 103
		public static bool _disable_gear_restriction;

		// Token: 0x04000068 RID: 104
		public static bool _disable_skill_total_restriction;

		// Token: 0x04000069 RID: 105
		public static bool _disable_skill_cap_restriction;
	}
}
