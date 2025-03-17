using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace MyLittleWarband
{
	// Token: 0x0200000B RID: 11
	internal class XMLExporter : CampaignBehaviorBase
	{
		// Token: 0x0600003A RID: 58 RVA: 0x000021E3 File Offset: 0x000003E3
		public override void RegisterEvents()
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.MenuItems));
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00003CDC File Offset: 0x00001EDC
		private void MenuItems(CampaignGameStarter campaignGameStarter)
		{
			campaignGameStarter.AddGameMenuOption("town_backstreet", "export_basic_tree", "Export Custom Troop Tree To XML File", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Escape;
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				this.Name();
			}, false, 1, false, null);
			campaignGameStarter.AddGameMenuOption("town_backstreet", "import_basic_tree", "Import Custom Troop Tree", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Escape;
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				this.Import();
			}, false, 1, false, null);
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00003D74 File Offset: 0x00001F74
		private void Import()
		{
			int num = 0;
			foreach (CharacterObject characterObject in Campaign.Current.Characters)
			{
				bool flag = characterObject.StringId.StartsWith("_basic_root") || characterObject.StringId.StartsWith("_elite_root");
				if (flag)
				{
					CharacterObject @object = Game.Current.ObjectManager.GetObject<CharacterObject>("copy" + characterObject.StringId);
					bool flag2 = @object != null;
					if (flag2)
					{
						num++;
						typeof(BasicCharacterObject).GetMethod("SetName", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(characterObject, new object[] { @object.Name });
						bool flag3 = characterObject.StringId != "_basic_root" && characterObject.StringId != "_elite_root";
						if (flag3)
						{
							CharacterObject object2 = Game.Current.ObjectManager.GetObject<CharacterObject>(characterObject.StringId.Substring(0, characterObject.StringId.Length - 1));
							List<CharacterObject> list = new List<CharacterObject>();
							list.Add(characterObject);
							bool flag4 = object2.UpgradeTargets != null && object2.UpgradeTargets.Length != 0;
							if (flag4)
							{
								list.Add(object2.UpgradeTargets[0]);
							}
							typeof(CharacterObject).GetProperty("UpgradeTargets").SetValue(object2, list.ToArray(), null);
							CustomUnitsBehavior.update(object2.StringId, false);
						}
						CustomUnitsBehavior.CopyCharacter(@object, characterObject);
						CustomUnitsBehavior.update(characterObject.StringId, false);
					}
				}
			}
			bool flag5 = num == 0;
			if (flag5)
			{
				InformationManager.DisplayMessage(new InformationMessage("Failed to load any units.  Check if exported troop tree module is turned on"));
			}
			else
			{
				InformationManager.DisplayMessage(new InformationMessage("Load " + num.ToString() + " units"));
			}
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00003F90 File Offset: 0x00002190
		private void Name()
		{
			InformationManager.ShowTextInquiry(new TextInquiryData("Save As", "Enter name", true, true, "save", "Cancel", delegate(string s)
			{
				XMLExporter.SaveFileName = s;
				string text = Path.Combine(BasePath.Name, "Modules/" + string.Concat<char>(XMLExporter.SaveFileName.Where((char c) => !char.IsWhiteSpace(c))));
				string text2 = Path.Combine(BasePath.Name, "Modules/" + string.Concat<char>(XMLExporter.SaveFileName.Where((char c) => !char.IsWhiteSpace(c))) + "/ModuleData");
				Directory.CreateDirectory(text);
				Directory.CreateDirectory(text2);
				this.exportTree();
				this.createSubModuleXML();
			}, null, false, null, "", ""), false, false);
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00003FDC File Offset: 0x000021DC
		private void createSubModuleXML()
		{
			string text = Path.Combine(BasePath.Name, "Modules/" + string.Concat<char>(XMLExporter.SaveFileName.Where((char c) => !char.IsWhiteSpace(c))) + "/SubModule.xml");
			string text2 = "";
			text2 += "<Module>\n";
			text2 = text2 + "\t<Name value=\"" + XMLExporter.SaveFileName + "\"/>\n";
			text2 = text2 + "\t<Id value=\"" + string.Concat<char>(XMLExporter.SaveFileName.Where((char c) => !char.IsWhiteSpace(c))) + "\"/>\n";
			text2 += "\t<Version value=\"v1.6.0\"/>\n";
			text2 += "\t<SingleplayerModule value=\"true\"/>\n";
			text2 += "\t<MultiplayerModule value=\"false\"/>\n";
			text2 += "\t<DependedModules>\n";
			text2 += "\t\t<DependedModule Id=\"Native\"/>\n";
			text2 += "\t\t<DependedModule Id=\"SandBoxCore\"/>\n";
			text2 += "\t\t<DependedModule Id=\"Sandbox\"/>\n";
			text2 += "\t\t<DependedModule Id=\"CustomBattle\"/>\n";
			text2 += "\t\t<DependedModule Id =\"StoryMode\"/>\n";
			text2 += "\t</DependedModules>\n";
			text2 += "\t<SubModules>\n";
			text2 += "\t</SubModules>\n";
			text2 += "\t<Xmls>\n";
			text2 += "\t\t<XmlNode>\n";
			text2 += "\t\t\t<XmlName id=\"NPCCharacters\" path=\"troops\"/>\n";
			text2 += "\t\t\t<IncludedGameTypes>\n";
			text2 += "\t\t\t\t<GameType value=\"Campaign\"/>\n";
			text2 += "\t\t\t\t<GameType value=\"CampaignStoryMode\"/>\n";
			text2 += "\t\t\t\t<GameType value=\"CustomGame\"/>\n";
			text2 += "\t\t\t\t<GameType value =\"EditorGame\"/>\n";
			text2 += "\t\t\t</IncludedGameTypes>\n";
			text2 += "\t\t</XmlNode>\n";
			text2 += "\t</Xmls>\n";
			text2 += "</Module>\n";
			File.WriteAllText(text, text2);
		}

		// Token: 0x0600003F RID: 63 RVA: 0x000041C4 File Offset: 0x000023C4
		private void exportTree()
		{
			string text = Path.Combine(BasePath.Name, "Modules/" + string.Concat<char>(XMLExporter.SaveFileName.Where((char c) => !char.IsWhiteSpace(c))) + "/ModuleData/troops.xml");
			InformationManager.DisplayMessage(new InformationMessage("Troop tree exported to " + text));
			string text2 = "";
			text2 += "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n";
			text2 += "<NPCCharacters>\n";
			foreach (CharacterObject characterObject in XMLExporter.getAllCharacters())
			{
				this.exportCharacter(characterObject, ref text2);
			}
			text2 += "</NPCCharacters>\n";
			File.WriteAllText(text, text2);
		}

		// Token: 0x06000040 RID: 64 RVA: 0x000042B0 File Offset: 0x000024B0
		public static List<CharacterObject> getAllCharacters()
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
			return list;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x000043D0 File Offset: 0x000025D0
		private void exportCharacter(CharacterObject character, ref string s)
		{
			s = s + "\t<NPCCharacter id=\"copy" + character.StringId + "\"\n";
			s = s + "\t\tdefault_group=\"" + character.DefaultFormationClass.ToString() + "\"\n";
			s = s + "\t\tlevel=\"" + character.Level.ToString() + "\"\n";
			s = string.Concat(new string[]
			{
				s,
				"\t\tname=\"{=",
				character.Name.ToString(),
				"}",
				character.Name.ToString(),
				"\"\n"
			});
			bool flag = character.UpgradeRequiresItemFromCategory != null;
			if (flag)
			{
				s = s + "\t\tupgrade_requires=\"ItemCategory." + character.UpgradeRequiresItemFromCategory.StringId + "\"\n";
			}
			s = s + "\t\toccupation=\"" + character.Occupation.ToString() + "\"\n";
			s = s + "\t\tculture=\"Culture." + character.Culture.StringId + "\">\n";
			s += "\t\t<face>\n";
			s = s + "\t\t\t<face_key_template value=\"BodyProperty.villager_" + character.Culture.StringId + "\"/>\n";
			s += "\t\t</face>\n";
			s += "\t\t<skills >\n";
			s = s + "\t\t\t<skill id=\"Athletics\" value=\"" + character.GetSkillValue(DefaultSkills.Athletics).ToString() + "\"/>\n";
			s = s + "\t\t\t<skill id=\"Riding\" value=\"" + character.GetSkillValue(DefaultSkills.Riding).ToString() + "\"/>\n";
			s = s + "\t\t\t<skill id=\"OneHanded\" value=\"" + character.GetSkillValue(DefaultSkills.OneHanded).ToString() + "\"/>\n";
			s = s + "\t\t\t<skill id=\"TwoHanded\" value=\"" + character.GetSkillValue(DefaultSkills.TwoHanded).ToString() + "\"/>\n";
			s = s + "\t\t\t<skill id=\"Polearm\" value=\"" + character.GetSkillValue(DefaultSkills.Polearm).ToString() + "\"/>\n";
			s = s + "\t\t\t<skill id=\"Bow\" value=\"" + character.GetSkillValue(DefaultSkills.Bow).ToString() + "\"/>\n";
			s = s + "\t\t\t<skill id=\"Crossbow\" value=\"" + character.GetSkillValue(DefaultSkills.Crossbow).ToString() + "\"/>\n";
			s = s + "\t\t\t<skill id=\"Throwing\" value=\"" + character.GetSkillValue(DefaultSkills.Throwing).ToString() + "\"/>\n";
			s += "\t\t</skills>\n";
			bool flag2 = character.UpgradeTargets != null && character.UpgradeTargets.Length != 0;
			if (flag2)
			{
				s += "\t\t<upgrade_targets>\n";
				s = s + "\t\t\t<upgrade_target id=\"NPCCharacter.copy" + character.UpgradeTargets[0].StringId + "\"/>\n";
				bool flag3 = character.UpgradeTargets.Length > 1;
				if (flag3)
				{
					s = s + "\t\t\t<upgrade_target id=\"NPCCharacter.copy" + character.UpgradeTargets[1].StringId + "\"/>\n";
				}
				s += "\t\t</upgrade_targets>\n";
			}
			s += "\t\t<Equipments>\n";
			List<Equipment> list = character.AllEquipments.Where((Equipment x) => !x.IsCivilian).ToList<Equipment>();
			List<Equipment> list2 = character.AllEquipments.Where((Equipment x) => x.IsCivilian).ToList<Equipment>();
			this.exportEquipmentRoaster(list[0], ref s, false);
			this.exportEquipmentRoaster(list[1], ref s, false);
			this.exportEquipmentRoaster(list[2], ref s, false);
			this.exportEquipmentRoaster(list[3], ref s, false);
			this.exportEquipmentRoaster(list2[0], ref s, true);
			s += "\t\t</Equipments>\n";
			s += "\t</NPCCharacter>\n";
		}

		// Token: 0x06000042 RID: 66 RVA: 0x000047E8 File Offset: 0x000029E8
		private void exportEquipmentRoaster(Equipment equipment, ref string s, bool isCivilian)
		{
			if (isCivilian)
			{
				s += "\t\t\t<EquipmentRoster civilian=\"true\">\n";
			}
			else
			{
				s += "\t\t\t<EquipmentRoster>\n";
			}
			bool flag = equipment[EquipmentIndex.WeaponItemBeginSlot].Item != null;
			if (flag)
			{
				s = s + "\t\t\t\t<equipment slot=\"Item0\" id=\"Item." + equipment[EquipmentIndex.WeaponItemBeginSlot].Item.StringId + "\"/>\n";
			}
			bool flag2 = equipment[EquipmentIndex.Weapon1].Item != null;
			if (flag2)
			{
				s = s + "\t\t\t\t<equipment slot=\"Item1\" id=\"Item." + equipment[EquipmentIndex.Weapon1].Item.StringId + "\"/>\n";
			}
			bool flag3 = equipment[EquipmentIndex.Weapon2].Item != null;
			if (flag3)
			{
				s = s + "\t\t\t\t<equipment slot=\"Item2\" id=\"Item." + equipment[EquipmentIndex.Weapon2].Item.StringId + "\"/>\n";
			}
			bool flag4 = equipment[EquipmentIndex.Weapon3].Item != null;
			if (flag4)
			{
				s = s + "\t\t\t\t<equipment slot=\"Item3\" id=\"Item." + equipment[EquipmentIndex.Weapon3].Item.StringId + "\"/>\n";
			}
			bool flag5 = equipment[EquipmentIndex.NumAllWeaponSlots].Item != null;
			if (flag5)
			{
				s = s + "\t\t\t\t<equipment slot=\"Head\" id=\"Item." + equipment[EquipmentIndex.NumAllWeaponSlots].Item.StringId + "\"/>\n";
			}
			bool flag6 = equipment[EquipmentIndex.Cape].Item != null;
			if (flag6)
			{
				s = s + "\t\t\t\t<equipment slot=\"Cape\" id=\"Item." + equipment[EquipmentIndex.Cape].Item.StringId + "\"/>\n";
			}
			bool flag7 = equipment[EquipmentIndex.Body].Item != null;
			if (flag7)
			{
				s = s + "\t\t\t\t<equipment slot=\"Body\" id=\"Item." + equipment[EquipmentIndex.Body].Item.StringId + "\"/>\n";
			}
			bool flag8 = equipment[EquipmentIndex.Gloves].Item != null;
			if (flag8)
			{
				s = s + "\t\t\t\t<equipment slot=\"Gloves\" id=\"Item." + equipment[EquipmentIndex.Gloves].Item.StringId + "\"/>\n";
			}
			bool flag9 = equipment[EquipmentIndex.Leg].Item != null;
			if (flag9)
			{
				s = s + "\t\t\t\t<equipment slot=\"Leg\" id=\"Item." + equipment[EquipmentIndex.Leg].Item.StringId + "\"/>\n";
			}
			bool flag10 = equipment[EquipmentIndex.ArmorItemEndSlot].Item != null;
			if (flag10)
			{
				s = s + "\t\t\t\t<equipment slot=\"Horse\" id=\"Item." + equipment[EquipmentIndex.ArmorItemEndSlot].Item.StringId + "\"/>\n";
			}
			bool flag11 = equipment[EquipmentIndex.HorseHarness].Item != null;
			if (flag11)
			{
				s = s + "\t\t\t\t<equipment slot=\"HorseHarness\" id=\"Item." + equipment[EquipmentIndex.HorseHarness].Item.StringId + "\"/>\n";
			}
			s += "\t\t\t</EquipmentRoster>\n";
		}

		// Token: 0x06000043 RID: 67 RVA: 0x000021FE File Offset: 0x000003FE
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x04000014 RID: 20
		private static string SaveFileName = "";
	}
}
