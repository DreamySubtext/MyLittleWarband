using System;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace MyLittleWarband
{
	// Token: 0x02000004 RID: 4
	public class EquipmentCardVM : ViewModel
	{
		// Token: 0x06000011 RID: 17 RVA: 0x00002908 File Offset: 0x00000B08
		public EquipmentCardVM(ItemObject item, CharacterObject troop, string equipmentType)
		{
			this.ItemName = new MBBindingList<BindingListStringItem>();
			bool flag = item != null;
			if (flag)
			{
				this.ItemName.Add(new BindingListStringItem(item.Name.ToString()));
				this.Image = new ImageIdentifierVM(item, "");
			}
			else
			{
				this.ItemName.Add(new BindingListStringItem("Empty"));
			}
			this.ItemFlagList = new MBBindingList<ItemFlagVM>();
			this.ItemProperties = new MBBindingList<ItemMenuTooltipPropertyVM>();
			this._item = item;
			this._troop = troop;
			this._equipmentType = equipmentType;
			this._getItemUsageSetFlags = new Func<WeaponComponentData, ItemObject.ItemUsageSetFlags>(this.GetItemUsageSetFlag);
			bool flag2 = item != null && item.WeaponComponent != null && item.WeaponComponent.Item != null;
			if (flag2)
			{
				this.AddWeaponItemFlags(this.ItemFlagList, item.WeaponComponent.Item.GetWeaponWithUsageIndex(0));
			}
			bool flag3 = item != null;
			if (flag3)
			{
				this.AddGeneralItemFlags(this.ItemFlagList, item);
			}
			bool flag4 = item != null && item.HorseComponent != null;
			if (flag4)
			{
				this.AddHorseItemFlags(this.ItemFlagList, item);
			}
			bool flag5 = item != null;
			if (flag5)
			{
				this.CreateColoredProperty(this.ItemProperties, "", item.Value.ToString() + "<img src=\"General\\Icons\\Coin@2x\" extend=\"8\"/>", UIColors.Gold, 1, null, TooltipProperty.TooltipPropertyFlags.Cost);
			}
			bool flag6 = item != null && item.Culture != null && item.Culture.Name != null;
			if (flag6)
			{
				this.CreateColoredProperty(this.ItemProperties, "Culture: ", item.Culture.Name.ToString(), Color.FromUint(item.Culture.Color), 0, null, TooltipProperty.TooltipPropertyFlags.None);
			}
			else
			{
				bool flag7 = item != null;
				if (flag7)
				{
					this.CreateColoredProperty(this.ItemProperties, "Culture: ", "No Culture", UIColors.Gold, 0, null, TooltipProperty.TooltipPropertyFlags.None);
				}
			}
			bool flag8 = item != null && item.RelevantSkill != null && item.Difficulty > 0;
			if (flag8)
			{
				this.AddSkillRequirement(item, this.ItemProperties, false);
			}
			bool flag9 = item != null && item.HorseComponent != null;
			if (flag9)
			{
				MBBindingList<ItemMenuTooltipPropertyVM> itemProperties = this.ItemProperties;
				string text = new TextObject("{=08abd5af7774d311cadc3ed900b47754}Type: ", null).ToString();
				string text2 = "str_inventory_type_";
				int num = (int)item.Type;
				this.CreateProperty(itemProperties, text, GameTexts.FindText(text2 + num.ToString(), null).ToString(), 0, null);
				this.AddIntProperty(new TextObject("{=mountTier}Mount Tier: ", null), (int)(item.Tier + 1));
				this.AddIntProperty(new TextObject("{=c7638a0869219ae845de0f660fd57a9d}Charge Damage: ", null), new EquipmentElement(item, null, null, false).GetModifiedMountCharge(in EquipmentElement.Invalid));
				this.AddIntProperty(new TextObject("{=c7638a0869219ae845de0f660fd57a9d}Charge Damage: ", null), new EquipmentElement(item, null, null, false).GetModifiedMountSpeed(in EquipmentElement.Invalid));
				this.AddIntProperty(new TextObject("{=3025020b83b218707499f0de3135ed0a}Maneuver: ", null), new EquipmentElement(item, null, null, false).GetModifiedMountManeuver(in EquipmentElement.Invalid));
				this.AddIntProperty(GameTexts.FindText("str_hit_points", null), new EquipmentElement(item, null, null, false).GetModifiedMountHitPoints());
				bool flag10 = item.HasHorseComponent && item.HorseComponent.IsMount;
				if (flag10)
				{
					this.CreateProperty(this.ItemProperties, new TextObject("{=9sxECG6e}Mount Type: ", null).ToString(), item.ItemCategory.GetName().ToString(), 0, null);
				}
			}
			bool flag11 = item != null && item.WeaponComponent != null;
			if (flag11)
			{
				WeaponComponentData weaponWithUsageIndex = item.WeaponComponent.Item.GetWeaponWithUsageIndex(0);
				this.CreateProperty(this.ItemProperties, new TextObject("{=8cad4a279770f269c4bb0dc7a357ee1e}Class: ", null).ToString(), GameTexts.FindText("str_inventory_weapon", ((int)weaponWithUsageIndex.WeaponClass).ToString()).ToString(), 0, null);
				bool flag12 = item.BannerComponent == null;
				if (flag12)
				{
					this.AddIntProperty(new TextObject("{=weaponTier}Weapon Tier: ", null), (int)(item.Tier + 1));
				}
				ItemObject.ItemTypeEnum itemTypeFromWeaponClass = WeaponComponentData.GetItemTypeFromWeaponClass(weaponWithUsageIndex.WeaponClass);
				bool flag13 = itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.OneHandedWeapon || itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.TwoHandedWeapon || itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.Polearm;
				if (flag13)
				{
					bool flag14 = weaponWithUsageIndex.SwingDamageType != DamageTypes.Invalid;
					if (flag14)
					{
						this.AddIntProperty(new TextObject("{=345a87fcc69f626ae3916939ef2fc135}Swing Speed: ", null), new EquipmentElement(item, null, null, false).GetModifiedSwingSpeedForUsage(0));
						this.CreateProperty(this.ItemProperties, GameTexts.FindText("str_swing_damage", null).ToString(), ItemHelper.GetSwingDamageText(weaponWithUsageIndex, new EquipmentElement(item, null, null, false).ItemModifier).ToString(), 0, null);
					}
					bool flag15 = weaponWithUsageIndex.ThrustDamageType != DamageTypes.Invalid;
					if (flag15)
					{
						this.AddIntProperty(GameTexts.FindText("str_thrust_speed", null), new EquipmentElement(item, null, null, false).GetModifiedThrustSpeedForUsage(0));
						this.CreateProperty(this.ItemProperties, GameTexts.FindText("str_thrust_damage", null).ToString(), ItemHelper.GetThrustDamageText(weaponWithUsageIndex, new EquipmentElement(item, null, null, false).ItemModifier).ToString(), 0, null);
					}
					this.AddIntProperty(new TextObject("{=c6e4c8588ca9e42f6e1b47b11f0f367b}Length: ", null), weaponWithUsageIndex.WeaponLength);
					this.AddIntProperty(new TextObject("{=ca8b1e8956057b831dfc665f54bae4b0}Handling: ", null), new EquipmentElement(item, null, null, false).GetModifiedHandlingForUsage(0));
				}
				bool flag16 = itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.Thrown;
				if (flag16)
				{
					this.AddIntProperty(new TextObject("{=5fa36d2798479803b4518a64beb4d732}Weapon Length: ", null), weaponWithUsageIndex.WeaponLength);
					this.CreateProperty(this.ItemProperties, new TextObject("{=c9c5dfed2ca6bcb7a73d905004c97b23}Damage: ", null).ToString(), ItemHelper.GetMissileDamageText(weaponWithUsageIndex, new EquipmentElement(item, null, null, false).ItemModifier).ToString(), 0, null);
					this.AddIntProperty(GameTexts.FindText("str_missile_speed", null), new EquipmentElement(item, null, null, false).GetModifiedMissileSpeedForUsage(0));
					this.AddIntProperty(new TextObject("{=5dec16fa0be433ade3c4cb0074ef366d}Accuracy: ", null), weaponWithUsageIndex.Accuracy);
					this.AddIntProperty(new TextObject("{=05fdfc6e238429753ef282f2ce97c1f8}Stack Amount: ", null), (int)new EquipmentElement(item, null, null, false).GetModifiedStackCountForUsage(0));
				}
				bool flag17 = itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.Shield;
				if (flag17)
				{
					this.AddIntProperty(new TextObject("{=74dc1908cb0b990e80fb977b5a0ef10d}Speed: ", null), new EquipmentElement(item, null, null, false).GetModifiedSwingSpeedForUsage(0));
					this.AddIntProperty(GameTexts.FindText("str_hit_points", null), (int)new EquipmentElement(item, null, null, false).GetModifiedMaximumHitPointsForUsage(0));
				}
				bool flag18 = itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.Bow || itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.Crossbow;
				if (flag18)
				{
					this.AddIntProperty(new TextObject("{=74dc1908cb0b990e80fb977b5a0ef10d}Speed: ", null), new EquipmentElement(item, null, null, false).GetModifiedSwingSpeedForUsage(0));
					this.CreateProperty(this.ItemProperties, new TextObject("{=c9c5dfed2ca6bcb7a73d905004c97b23}Damage: ", null).ToString(), ItemHelper.GetThrustDamageText(weaponWithUsageIndex, new EquipmentElement(item, null, null, false).ItemModifier).ToString(), 0, null);
					this.AddIntProperty(new TextObject("{=5dec16fa0be433ade3c4cb0074ef366d}Accuracy: ", null), weaponWithUsageIndex.Accuracy);
					this.AddIntProperty(GameTexts.FindText("str_missile_speed", null), new EquipmentElement(item, null, null, false).GetModifiedMissileSpeedForUsage(0));
					bool flag19 = itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.Crossbow;
					if (flag19)
					{
						this.AddIntProperty(new TextObject("{=6adabc1f82216992571c3e22abc164d7}Ammo Limit: ", null), (int)weaponWithUsageIndex.MaxDataValue);
					}
				}
				bool isAmmo = weaponWithUsageIndex.IsAmmo;
				if (isAmmo)
				{
					bool flag20 = itemTypeFromWeaponClass != ItemObject.ItemTypeEnum.Arrows && itemTypeFromWeaponClass != ItemObject.ItemTypeEnum.Bolts;
					if (flag20)
					{
						this.AddIntProperty(new TextObject("{=5dec16fa0be433ade3c4cb0074ef366d}Accuracy: ", null), weaponWithUsageIndex.Accuracy);
					}
					this.CreateProperty(this.ItemProperties, new TextObject("{=c9c5dfed2ca6bcb7a73d905004c97b23}Damage: ", null).ToString(), ItemHelper.GetThrustDamageText(weaponWithUsageIndex, new EquipmentElement(item, null, null, false).ItemModifier).ToString(), 0, null);
					this.AddIntProperty(new TextObject("{=05fdfc6e238429753ef282f2ce97c1f8}Stack Amount: ", null), (int)new EquipmentElement(item, null, null, false).GetModifiedStackCountForUsage(0));
				}
			}
			bool flag21 = item != null && item.ArmorComponent != null;
			if (flag21)
			{
				this.AddIntProperty(new TextObject("{=armorTier}Armor Tier: ", null), (int)(item.Tier + 1));
				MBBindingList<ItemMenuTooltipPropertyVM> itemProperties2 = this.ItemProperties;
				string text3 = new TextObject("{=08abd5af7774d311cadc3ed900b47754}Type: ", null).ToString();
				string text4 = "str_inventory_type_";
				int num = (int)item.Type;
				this.CreateProperty(itemProperties2, text3, GameTexts.FindText(text4 + num.ToString(), null).ToString(), 0, null);
				bool flag22 = new EquipmentElement(item, null, null, false).GetModifiedHeadArmor() != 0;
				if (flag22)
				{
					this.CreateProperty(this.ItemProperties, GameTexts.FindText("str_head_armor", null).ToString(), new EquipmentElement(item, null, null, false).GetModifiedHeadArmor().ToString(), 0, null);
				}
				bool flag23 = item.ArmorComponent.BodyArmor != 0;
				if (flag23)
				{
					bool flag24 = this.GetItemTypeWithItemObject(item) == EquipmentIndex.HorseHarness;
					if (flag24)
					{
						this.CreateProperty(this.ItemProperties, new TextObject("{=305cf7f98458b22e9af72b60a131714f}Horse Armor: ", null).ToString(), new EquipmentElement(item, null, null, false).GetModifiedMountBodyArmor().ToString(), 0, null);
					}
					else
					{
						this.CreateProperty(this.ItemProperties, GameTexts.FindText("str_body_armor", null).ToString(), new EquipmentElement(item, null, null, false).GetModifiedBodyArmor().ToString(), 0, null);
					}
				}
				bool flag25 = new EquipmentElement(item, null, null, false).GetModifiedLegArmor() != 0;
				if (flag25)
				{
					this.CreateProperty(this.ItemProperties, GameTexts.FindText("str_leg_armor", null).ToString(), new EquipmentElement(item, null, null, false).GetModifiedLegArmor().ToString(), 0, null);
				}
				bool flag26 = new EquipmentElement(item, null, null, false).GetModifiedArmArmor() != 0;
				if (flag26)
				{
					this.CreateProperty(this.ItemProperties, new TextObject("{=cf61cce254c7dca65be9bebac7fb9bf5}Arm Armor: ", null).ToString(), new EquipmentElement(item, null, null, false).GetModifiedArmArmor().ToString(), 0, null);
				}
			}
		}

		// Token: 0x06000012 RID: 18 RVA: 0x0000211D File Offset: 0x0000031D
		public void Apply()
		{
			CustomUnitsBehavior.UpgradeGearFinalize(CustomUnitsBehavior.ToEquipmentSlot(this._equipmentType), this._item, this._troop.StringId);
			EquipmentSelectorBehavior.DeleteVMLayer();
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00003344 File Offset: 0x00001544
		public EquipmentIndex GetItemTypeWithItemObject(ItemObject item)
		{
			bool flag = item == null;
			EquipmentIndex equipmentIndex;
			if (flag)
			{
				equipmentIndex = EquipmentIndex.None;
			}
			else
			{
				ItemObject.ItemTypeEnum type = item.Type;
				switch (type)
				{
				case ItemObject.ItemTypeEnum.Horse:
					return EquipmentIndex.ArmorItemEndSlot;
				case ItemObject.ItemTypeEnum.OneHandedWeapon:
				case ItemObject.ItemTypeEnum.TwoHandedWeapon:
				case ItemObject.ItemTypeEnum.Polearm:
				case ItemObject.ItemTypeEnum.Bow:
				case ItemObject.ItemTypeEnum.Crossbow:
				case ItemObject.ItemTypeEnum.Thrown:
				case ItemObject.ItemTypeEnum.Goods:
					break;
				case ItemObject.ItemTypeEnum.Arrows:
					return EquipmentIndex.WeaponItemBeginSlot;
				case ItemObject.ItemTypeEnum.Bolts:
					return EquipmentIndex.WeaponItemBeginSlot;
				case ItemObject.ItemTypeEnum.Shield:
					return EquipmentIndex.WeaponItemBeginSlot;
				case ItemObject.ItemTypeEnum.HeadArmor:
					return EquipmentIndex.NumAllWeaponSlots;
				case ItemObject.ItemTypeEnum.BodyArmor:
					return EquipmentIndex.Body;
				case ItemObject.ItemTypeEnum.LegArmor:
					return EquipmentIndex.Leg;
				case ItemObject.ItemTypeEnum.HandArmor:
					return EquipmentIndex.Gloves;
				default:
					switch (type)
					{
					case ItemObject.ItemTypeEnum.Cape:
						return EquipmentIndex.Cape;
					case ItemObject.ItemTypeEnum.HorseHarness:
						return EquipmentIndex.HorseHarness;
					case ItemObject.ItemTypeEnum.Banner:
						return EquipmentIndex.ExtraWeaponSlot;
					}
					break;
				}
				bool flag2 = item.WeaponComponent != null;
				if (flag2)
				{
					equipmentIndex = EquipmentIndex.WeaponItemBeginSlot;
				}
				else
				{
					equipmentIndex = EquipmentIndex.None;
				}
			}
			return equipmentIndex;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002148 File Offset: 0x00000348
		public void Click()
		{
			InformationManager.DisplayMessage(new InformationMessage(this._item.Name.ToString()));
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00003420 File Offset: 0x00001620
		private void AddIntProperty(TextObject description, int Value)
		{
			string text = Value.ToString();
			this.CreateColoredProperty(this.ItemProperties, description.ToString(), text, Colors.White, 0, null, TooltipProperty.TooltipPropertyFlags.None);
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00003454 File Offset: 0x00001654
		private void AddSkillRequirement(ItemObject item, MBBindingList<ItemMenuTooltipPropertyVM> itemProperties, bool isComparison)
		{
			string text = "";
			bool flag = item.Difficulty > 0;
			if (flag)
			{
				text = item.RelevantSkill.Name.ToString();
				text += " ";
				text += item.Difficulty.ToString();
			}
			string text2 = new TextObject("{=154a34f8caccfc833238cc89d38861e8}Requires: ", null).ToString();
			this.CreateColoredProperty(itemProperties, text2, text, (this._troop.GetSkillValue(item.RelevantSkill) >= item.Difficulty) ? UIColors.PositiveIndicator : UIColors.NegativeIndicator, 0, null, TooltipProperty.TooltipPropertyFlags.None);
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000034EC File Offset: 0x000016EC
		private ItemMenuTooltipPropertyVM CreateColoredProperty(MBBindingList<ItemMenuTooltipPropertyVM> targetList, string definition, string value, Color color, int textHeight = 0, HintViewModel hint = null, TooltipProperty.TooltipPropertyFlags propertyFlags = TooltipProperty.TooltipPropertyFlags.None)
		{
			bool flag = color == Colors.Black;
			ItemMenuTooltipPropertyVM itemMenuTooltipPropertyVM;
			if (flag)
			{
				this.CreateProperty(targetList, definition, value, textHeight, hint);
				itemMenuTooltipPropertyVM = null;
			}
			else
			{
				ItemMenuTooltipPropertyVM itemMenuTooltipPropertyVM2 = new ItemMenuTooltipPropertyVM(definition, value, textHeight, color, false, hint, propertyFlags);
				targetList.Add(itemMenuTooltipPropertyVM2);
				itemMenuTooltipPropertyVM = itemMenuTooltipPropertyVM2;
			}
			return itemMenuTooltipPropertyVM;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x0000353C File Offset: 0x0000173C
		private ItemMenuTooltipPropertyVM CreateProperty(MBBindingList<ItemMenuTooltipPropertyVM> targetList, string definition, string value, int textHeight = 0, HintViewModel hint = null)
		{
			ItemMenuTooltipPropertyVM itemMenuTooltipPropertyVM = new ItemMenuTooltipPropertyVM(definition, value, textHeight, false, hint);
			targetList.Add(itemMenuTooltipPropertyVM);
			return itemMenuTooltipPropertyVM;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00003564 File Offset: 0x00001764
		private void AddWeaponItemFlags(MBBindingList<ItemFlagVM> list, WeaponComponentData weapon)
		{
			bool flag = weapon == null;
			if (!flag)
			{
				ItemObject.ItemUsageSetFlags itemUsageSetFlags = this._getItemUsageSetFlags(weapon);
				foreach (ValueTuple<string, TextObject> valueTuple in CampaignUIHelper.GetFlagDetailsForWeapon(weapon, itemUsageSetFlags, null))
				{
					list.Add(new ItemFlagVM(valueTuple.Item1, valueTuple.Item2));
				}
			}
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000035E8 File Offset: 0x000017E8
		private void AddGeneralItemFlags(MBBindingList<ItemFlagVM> list, ItemObject item)
		{
			bool isUniqueItem = item.IsUniqueItem;
			if (isUniqueItem)
			{
				list.Add(new ItemFlagVM("GeneralFlagIcons\\unique", GameTexts.FindText("str_inventory_flag_unique", null)));
			}
			bool isCivilian = item.IsCivilian;
			if (isCivilian)
			{
				list.Add(new ItemFlagVM("GeneralFlagIcons\\civillian", GameTexts.FindText("str_inventory_flag_civillian", null)));
			}
			bool flag = item.ItemFlags.HasAnyFlag(ItemFlags.NotUsableByFemale);
			if (flag)
			{
				list.Add(new ItemFlagVM("GeneralFlagIcons\\male_only", GameTexts.FindText("str_inventory_flag_male_only", null)));
			}
			bool flag2 = item.ItemFlags.HasAnyFlag(ItemFlags.NotUsableByMale);
			if (flag2)
			{
				list.Add(new ItemFlagVM("GeneralFlagIcons\\female_only", GameTexts.FindText("str_inventory_flag_female_only", null)));
			}
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000036AC File Offset: 0x000018AC
		private void AddHorseItemFlags(MBBindingList<ItemFlagVM> list, ItemObject item)
		{
			bool flag = !item.HorseComponent.IsLiveStock;
			if (flag)
			{
				bool flag2 = item.ItemCategory == DefaultItemCategories.PackAnimal;
				if (flag2)
				{
					list.Add(new ItemFlagVM("MountFlagIcons\\weight_carrying_mount", GameTexts.FindText("str_inventory_flag_carrying_mount", null)));
				}
				else
				{
					list.Add(new ItemFlagVM("MountFlagIcons\\speed_mount", GameTexts.FindText("str_inventory_flag_speed_mount", null)));
				}
			}
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00003720 File Offset: 0x00001920
		private ItemObject.ItemUsageSetFlags GetItemUsageSetFlag(WeaponComponentData item)
		{
			bool flag = !string.IsNullOrEmpty(item.ItemUsage);
			ItemObject.ItemUsageSetFlags itemUsageSetFlags;
			if (flag)
			{
				itemUsageSetFlags = MBItem.GetItemUsageSetFlags(item.ItemUsage);
			}
			else
			{
				itemUsageSetFlags = (ItemObject.ItemUsageSetFlags)0;
			}
			return itemUsageSetFlags;
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600001D RID: 29 RVA: 0x00003754 File Offset: 0x00001954
		// (set) Token: 0x0600001E RID: 30 RVA: 0x0000376C File Offset: 0x0000196C
		public MBBindingList<ItemMenuTooltipPropertyVM> ItemProperties
		{
			get
			{
				return this._itemProperties;
			}
			set
			{
				bool flag = value != this._itemProperties;
				if (flag)
				{
					this._itemProperties = value;
					base.OnPropertyChangedWithValue<MBBindingList<ItemMenuTooltipPropertyVM>>(value, "ItemProperties");
				}
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600001F RID: 31 RVA: 0x00002166 File Offset: 0x00000366
		// (set) Token: 0x06000020 RID: 32 RVA: 0x000037A0 File Offset: 0x000019A0
		[DataSourceProperty]
		public MBBindingList<BindingListStringItem> ItemName
		{
			get
			{
				return this._nameText;
			}
			set
			{
				bool flag = value == this._nameText;
				if (!flag)
				{
					this._nameText = value;
					base.OnPropertyChanged("ItemName");
				}
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000021 RID: 33 RVA: 0x0000216E File Offset: 0x0000036E
		// (set) Token: 0x06000022 RID: 34 RVA: 0x000037D0 File Offset: 0x000019D0
		[DataSourceProperty]
		public ImageIdentifierVM Image
		{
			get
			{
				return this._image;
			}
			set
			{
				bool flag = value == this._image;
				if (!flag)
				{
					this._image = value;
					base.OnPropertyChangedWithValue<object>(value, "Image");
				}
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000023 RID: 35 RVA: 0x00003804 File Offset: 0x00001A04
		// (set) Token: 0x06000024 RID: 36 RVA: 0x0000381C File Offset: 0x00001A1C
		[DataSourceProperty]
		public MBBindingList<ItemFlagVM> ItemFlagList
		{
			get
			{
				return this._ItemFlagList;
			}
			set
			{
				bool flag = value != this._ItemFlagList;
				if (flag)
				{
					this._ItemFlagList = value;
					base.OnPropertyChangedWithValue<MBBindingList<ItemFlagVM>>(value, "ItemFlagList");
				}
			}
		}

		// Token: 0x04000006 RID: 6
		private MBBindingList<BindingListStringItem> _nameText;

		// Token: 0x04000007 RID: 7
		private MBBindingList<ItemFlagVM> _ItemFlagList;

		// Token: 0x04000008 RID: 8
		private MBBindingList<ItemMenuTooltipPropertyVM> _itemProperties;

		// Token: 0x04000009 RID: 9
		private ImageIdentifierVM _image;

		// Token: 0x0400000A RID: 10
		public ItemObject _item;

		// Token: 0x0400000B RID: 11
		private Func<WeaponComponentData, ItemObject.ItemUsageSetFlags> _getItemUsageSetFlags;

		// Token: 0x0400000C RID: 12
		private CharacterObject _troop;

		// Token: 0x0400000D RID: 13
		private string _equipmentType;
	}
}
