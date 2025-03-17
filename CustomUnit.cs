using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace MyLittleWarband
{
	// Token: 0x02000038 RID: 56
	internal class CustomUnit
	{
		// Token: 0x0600019F RID: 415 RVA: 0x00002805 File Offset: 0x00000A05
		public CustomUnit(string id)
		{
			this._id = id;
			this.SaveUpdate();
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x0000CACC File Offset: 0x0000ACCC
		public void SaveUpdate()
		{
			CharacterObject @object = Game.Current.ObjectManager.GetObject<CharacterObject>(this._id);
			List<Equipment> list = @object.AllEquipments.Where((Equipment x) => !x.IsCivilian).ToList<Equipment>();
			this._equimpent = new Equipment[list.Count];
			int num = 0;
			foreach (Equipment equipment in list)
			{
				this._equimpent[num] = equipment;
				num++;
			}
			this._name = @object.Name.ToString();
			this._level = @object.Level;
			this._athletics = @object.GetSkillValue(DefaultSkills.Athletics);
			this._riding = @object.GetSkillValue(DefaultSkills.Riding);
			this._one_hand = @object.GetSkillValue(DefaultSkills.OneHanded);
			this._two_hand = @object.GetSkillValue(DefaultSkills.TwoHanded);
			this._polearm = @object.GetSkillValue(DefaultSkills.Polearm);
			this._bow = @object.GetSkillValue(DefaultSkills.Bow);
			this._throwing = @object.GetSkillValue(DefaultSkills.Throwing);
			this._crossbow = @object.GetSkillValue(DefaultSkills.Crossbow);
			this._culture = @object.Culture;
			this._is_female = @object.IsFemale;
			bool flag = @object.UpgradeTargets != null;
			if (flag)
			{
				bool flag2 = @object.UpgradeTargets.Length != 0;
				if (flag2)
				{
					this._upgrade_target_1 = @object.UpgradeTargets[0];
				}
				else
				{
					this._upgrade_target_1 = null;
				}
				bool flag3 = @object.UpgradeTargets.Length > 1;
				if (flag3)
				{
					this._upgrade_target_2 = @object.UpgradeTargets[1];
				}
				else
				{
					this._upgrade_target_2 = null;
				}
			}
			this.checkHorseNeeded();
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060001A1 RID: 417 RVA: 0x0000CCAC File Offset: 0x0000AEAC
		// (set) Token: 0x060001A2 RID: 418 RVA: 0x0000281D File Offset: 0x00000A1D
		[SaveableProperty(1)]
		public string Id
		{
			get
			{
				return this._id;
			}
			set
			{
				this._id = value;
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060001A3 RID: 419 RVA: 0x0000CCC4 File Offset: 0x0000AEC4
		// (set) Token: 0x060001A4 RID: 420 RVA: 0x00002827 File Offset: 0x00000A27
		[SaveableProperty(2)]
		public int Level
		{
			get
			{
				return this._level;
			}
			set
			{
				this._level = value;
			}
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060001A5 RID: 421 RVA: 0x0000CCDC File Offset: 0x0000AEDC
		// (set) Token: 0x060001A6 RID: 422 RVA: 0x00002831 File Offset: 0x00000A31
		[SaveableProperty(3)]
		public Equipment[] Equipment
		{
			get
			{
				return this._equimpent;
			}
			set
			{
				this._equimpent = value;
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060001A7 RID: 423 RVA: 0x0000CCF4 File Offset: 0x0000AEF4
		// (set) Token: 0x060001A8 RID: 424 RVA: 0x0000283B File Offset: 0x00000A3B
		[SaveableProperty(4)]
		public int Athletics
		{
			get
			{
				return this._athletics;
			}
			set
			{
				this._athletics = value;
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060001A9 RID: 425 RVA: 0x0000CD0C File Offset: 0x0000AF0C
		// (set) Token: 0x060001AA RID: 426 RVA: 0x00002845 File Offset: 0x00000A45
		[SaveableProperty(5)]
		public int Riding
		{
			get
			{
				return this._riding;
			}
			set
			{
				this._riding = value;
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060001AB RID: 427 RVA: 0x0000CD24 File Offset: 0x0000AF24
		// (set) Token: 0x060001AC RID: 428 RVA: 0x0000284F File Offset: 0x00000A4F
		[SaveableProperty(6)]
		public int OneHand
		{
			get
			{
				return this._one_hand;
			}
			set
			{
				this._one_hand = value;
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060001AD RID: 429 RVA: 0x0000CD3C File Offset: 0x0000AF3C
		// (set) Token: 0x060001AE RID: 430 RVA: 0x00002859 File Offset: 0x00000A59
		[SaveableProperty(7)]
		public int TwoHand
		{
			get
			{
				return this._two_hand;
			}
			set
			{
				this._two_hand = value;
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060001AF RID: 431 RVA: 0x0000CD54 File Offset: 0x0000AF54
		// (set) Token: 0x060001B0 RID: 432 RVA: 0x00002863 File Offset: 0x00000A63
		[SaveableProperty(8)]
		public int Polearm
		{
			get
			{
				return this._polearm;
			}
			set
			{
				this._polearm = value;
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060001B1 RID: 433 RVA: 0x0000CD6C File Offset: 0x0000AF6C
		// (set) Token: 0x060001B2 RID: 434 RVA: 0x0000286D File Offset: 0x00000A6D
		[SaveableProperty(9)]
		public int Bow
		{
			get
			{
				return this._bow;
			}
			set
			{
				this._bow = value;
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060001B3 RID: 435 RVA: 0x0000CD84 File Offset: 0x0000AF84
		// (set) Token: 0x060001B4 RID: 436 RVA: 0x00002877 File Offset: 0x00000A77
		[SaveableProperty(10)]
		public int Throwing
		{
			get
			{
				return this._throwing;
			}
			set
			{
				this._throwing = value;
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060001B5 RID: 437 RVA: 0x0000CD9C File Offset: 0x0000AF9C
		// (set) Token: 0x060001B6 RID: 438 RVA: 0x00002881 File Offset: 0x00000A81
		[SaveableProperty(11)]
		public int Crossbow
		{
			get
			{
				return this._crossbow;
			}
			set
			{
				this._crossbow = value;
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060001B7 RID: 439 RVA: 0x0000CDB4 File Offset: 0x0000AFB4
		// (set) Token: 0x060001B8 RID: 440 RVA: 0x0000288B File Offset: 0x00000A8B
		[SaveableProperty(12)]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060001B9 RID: 441 RVA: 0x0000CDCC File Offset: 0x0000AFCC
		// (set) Token: 0x060001BA RID: 442 RVA: 0x00002895 File Offset: 0x00000A95
		[SaveableProperty(16)]
		public CultureObject Culture
		{
			get
			{
				return this._culture;
			}
			set
			{
				this._culture = value;
			}
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060001BB RID: 443 RVA: 0x0000CDE4 File Offset: 0x0000AFE4
		// (set) Token: 0x060001BC RID: 444 RVA: 0x0000289F File Offset: 0x00000A9F
		[SaveableProperty(17)]
		public bool IsFemale
		{
			get
			{
				return this._is_female;
			}
			set
			{
				this._is_female = value;
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060001BD RID: 445 RVA: 0x0000CDFC File Offset: 0x0000AFFC
		// (set) Token: 0x060001BE RID: 446 RVA: 0x000028A9 File Offset: 0x00000AA9
		[SaveableProperty(18)]
		public CharacterObject UpgradeTarget1
		{
			get
			{
				return this._upgrade_target_1;
			}
			set
			{
				this._upgrade_target_1 = value;
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060001BF RID: 447 RVA: 0x0000CE14 File Offset: 0x0000B014
		// (set) Token: 0x060001C0 RID: 448 RVA: 0x000028B3 File Offset: 0x00000AB3
		[SaveableProperty(19)]
		public CharacterObject UpgradeTarget2
		{
			get
			{
				return this._upgrade_target_2;
			}
			set
			{
				this._upgrade_target_2 = value;
			}
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x0000CE2C File Offset: 0x0000B02C
		public void Update()
		{
			CharacterObject @object = Game.Current.ObjectManager.GetObject<CharacterObject>(this._id);
			foreach (EquipmentIndex equipmentIndex in new EquipmentIndex[]
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
			})
			{
				for (int j = 0; j < this._equimpent.Length; j++)
				{
					if (this._equimpent[j] != null)
					{
						if (this._equimpent[j].GetEquipmentFromSlot(equipmentIndex).Item != null && this._equimpent[j].GetEquipmentFromSlot(equipmentIndex).Item.Name != null)
						{
							this.ChangeUnitEquipment((int)equipmentIndex, @object, this._equimpent[j].GetEquipmentFromSlot(equipmentIndex).Item, j);
						}
						else
						{
							this.ChangeUnitEquipment((int)equipmentIndex, @object, null, j);
						}
					}
				}
			}
			MBCharacterSkills mbcharacterSkills = MBObjectManager.Instance.CreateObject<MBCharacterSkills>(@object.StringId);
			mbcharacterSkills.Skills.SetPropertyValue(DefaultSkills.Crossbow, this.Crossbow);
			mbcharacterSkills.Skills.SetPropertyValue(DefaultSkills.Bow, this.Bow);
			mbcharacterSkills.Skills.SetPropertyValue(DefaultSkills.Throwing, this.Throwing);
			mbcharacterSkills.Skills.SetPropertyValue(DefaultSkills.OneHanded, this.OneHand);
			mbcharacterSkills.Skills.SetPropertyValue(DefaultSkills.TwoHanded, this.TwoHand);
			mbcharacterSkills.Skills.SetPropertyValue(DefaultSkills.Polearm, this.Polearm);
			mbcharacterSkills.Skills.SetPropertyValue(DefaultSkills.Athletics, this.Athletics);
			mbcharacterSkills.Skills.SetPropertyValue(DefaultSkills.Riding, this.Riding);
			FieldInfo field = @object.GetType().GetField("DefaultCharacterSkills", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (null != field)
			{
				field.SetValue(@object, mbcharacterSkills);
			}
			typeof(BasicCharacterObject).GetMethod("SetName", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(@object, new object[]
			{
				new TextObject(this._name, null)
			});
			@object.Level = this.Level;
			@object.IsFemale = this.IsFemale;
			if (this.Culture != null)
			{
				@object.Culture = this.Culture;
			}
			else
			{
				this.Culture = @object.Culture;
			}
			if ((this.IsFemale ? this.Culture.Townswoman : this.Culture.Townsman) != null)
			{
				AccessTools.Property(typeof(CharacterObject), "BodyPropertyRange").SetValue(@object, this.IsFemale ? this.Culture.Townswoman.BodyPropertyRange : this.Culture.Townsman.BodyPropertyRange, null);
			}
			else
			{
				AccessTools.Property(typeof(CharacterObject), "BodyPropertyRange").SetValue(@object, this.IsFemale ? Game.Current.ObjectManager.GetObject<CharacterObject>("townswoman_empire").BodyPropertyRange : Game.Current.ObjectManager.GetObject<CharacterObject>("townsman_empire").BodyPropertyRange, null);
			}
			int num = 0;
			if (this._upgrade_target_1 != null)
			{
				num++;
			}
			if (this._upgrade_target_2 != null)
			{
				num++;
			}
			if (num > 0)
			{
				int num2 = 0;
				CharacterObject[] array2 = new CharacterObject[num];
				if (this._upgrade_target_1 != null)
				{
					array2[num2] = this._upgrade_target_1;
					num2++;
				}
				if (this._upgrade_target_2 != null)
				{
					array2[num2] = this._upgrade_target_2;
				}
				typeof(CharacterObject).GetProperty("UpgradeTargets").SetValue(@object, array2, null);
			}
			CustomUnitsBehavior.SetDefaultGroup(this._id);
			this.checkHorseNeeded();
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x0000D1AC File Offset: 0x0000B3AC
		public void checkHorseNeeded()
		{
			CharacterObject @object = Game.Current.ObjectManager.GetObject<CharacterObject>(this._id);
			bool flag = EnyclopediaEditUnitPatch.isCustomTroop(@object);
			if (flag)
			{
				CharacterObject object2 = Game.Current.ObjectManager.GetObject<CharacterObject>(@object.StringId.Substring(0, @object.StringId.Length - 1));
				bool flag2 = object2 != null && @object.Equipment[EquipmentIndex.ArmorItemEndSlot].Item != null && @object.Equipment[EquipmentIndex.ArmorItemEndSlot].Item.ItemCategory == DefaultItemCategories.WarHorse && (object2.Equipment[EquipmentIndex.ArmorItemEndSlot].Item == null || object2.Equipment[EquipmentIndex.ArmorItemEndSlot].Item.ItemCategory != DefaultItemCategories.WarHorse);
				if (flag2)
				{
					typeof(CharacterObject).GetProperty("UpgradeRequiresItemFromCategory").SetValue(@object, DefaultItemCategories.WarHorse, null);
				}
				else
				{
					bool flag3 = object2 != null && @object.Equipment[EquipmentIndex.ArmorItemEndSlot].Item != null && @object.Equipment[EquipmentIndex.ArmorItemEndSlot].Item.ItemCategory == DefaultItemCategories.Horse && object2.Equipment[EquipmentIndex.ArmorItemEndSlot].Item == null;
					if (flag3)
					{
						typeof(CharacterObject).GetProperty("UpgradeRequiresItemFromCategory").SetValue(@object, DefaultItemCategories.Horse, null);
					}
					else
					{
						typeof(CharacterObject).GetProperty("UpgradeRequiresItemFromCategory").SetValue(@object, null, null);
					}
				}
			}
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x0000D358 File Offset: 0x0000B558
		private void ChangeUnitEquipment(int slot, CharacterObject character, ItemObject item, int set)
		{
			List<Equipment> list = null;
			bool flag = character != null && character.CivilianEquipments != null;
			if (flag)
			{
				list = character.CivilianEquipments.ToList<Equipment>();
			}
			List<Equipment> list2 = null;
			bool flag2 = character != null && character.BattleEquipments != null;
			if (flag2)
			{
				list2 = character.BattleEquipments.ToList<Equipment>();
			}
			bool flag3 = list2 == null || set >= list2.Count;
			if (!flag3)
			{
				EquipmentElement equipmentElement = ((item == null) ? default(EquipmentElement) : new EquipmentElement(item, null, null, false));
				bool flag4 = list2 != null;
				if (flag4)
				{
					list2[set][slot] = equipmentElement;
					bool flag5 = list != null;
					if (flag5)
					{
						list2.AddRange(list);
					}
				}
				this.UpdateSelectedUnitEquipment(character, list2);
			}
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x0000D41C File Offset: 0x0000B61C
		private void UpdateSelectedUnitEquipment(CharacterObject character, List<Equipment> equipments)
		{
			MBEquipmentRoster mbequipmentRoster = new MBEquipmentRoster();
			((FieldInfo)CustomUnit.GetInstanceField<MBEquipmentRoster>(mbequipmentRoster, "_equipments")).SetValue(mbequipmentRoster, new MBList<Equipment>(equipments));
			((FieldInfo)CustomUnit.GetInstanceField<BasicCharacterObject>(character, "_equipmentRoster")).SetValue(character, mbequipmentRoster);
			character.InitializeEquipmentsOnLoad(character);
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x0000ABE4 File Offset: 0x00008DE4
		internal static object GetInstanceField<T>(T instance, string fieldName)
		{
			BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			return typeof(T).GetField(fieldName, bindingFlags);
		}

		// Token: 0x040000C9 RID: 201
		private string _id;

		// Token: 0x040000CA RID: 202
		private Equipment[] _equimpent;

		// Token: 0x040000CB RID: 203
		private int _level;

		// Token: 0x040000CC RID: 204
		private int _athletics;

		// Token: 0x040000CD RID: 205
		private int _riding;

		// Token: 0x040000CE RID: 206
		private int _one_hand;

		// Token: 0x040000CF RID: 207
		private int _two_hand;

		// Token: 0x040000D0 RID: 208
		private int _polearm;

		// Token: 0x040000D1 RID: 209
		private int _bow;

		// Token: 0x040000D2 RID: 210
		private int _throwing;

		// Token: 0x040000D3 RID: 211
		private int _crossbow;

		// Token: 0x040000D4 RID: 212
		private string _name;

		// Token: 0x040000D5 RID: 213
		private CultureObject _culture;

		// Token: 0x040000D6 RID: 214
		private bool _is_female;

		// Token: 0x040000D7 RID: 215
		private CharacterObject _upgrade_target_1;

		// Token: 0x040000D8 RID: 216
		private CharacterObject _upgrade_target_2;
	}
}
