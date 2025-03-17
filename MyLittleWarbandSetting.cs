using System;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v1;
using MCM.Abstractions.Base.Global;

namespace MyLittleWarband
{
	// Token: 0x02000002 RID: 2
	internal class MyLittleWarbandSetting : AttributeGlobalSettings<MyLittleWarbandSetting>
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x0000207C File Offset: 0x0000027C
		public override string Id
		{
			get
			{
				return "MyLittleWarbandSetting";
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000002 RID: 2 RVA: 0x00002083 File Offset: 0x00000283
		public override string DisplayName
		{
			get
			{
				return "My Little Warband Setting";
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000003 RID: 3 RVA: 0x0000207C File Offset: 0x0000027C
		public override string FolderName
		{
			get
			{
				return "MyLittleWarbandSetting";
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000004 RID: 4 RVA: 0x0000208A File Offset: 0x0000028A
		public override string FormatType
		{
			get
			{
				return "json";
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000005 RID: 5 RVA: 0x00002091 File Offset: 0x00000291
		// (set) Token: 0x06000006 RID: 6 RVA: 0x00002099 File Offset: 0x00000299
		[SettingProperty("Spawn At Player Settlement", HintText = "Do custom troops spawn at settlements owned by the player clan?  AI lords visiting player settlements can hire these troops", RequireRestart = false)]
		[SettingPropertyGroup("Setting")]
		public bool SpawnAtPlayerSettlement { get; set; } = false;

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000007 RID: 7 RVA: 0x000020A2 File Offset: 0x000002A2
		// (set) Token: 0x06000008 RID: 8 RVA: 0x000020AA File Offset: 0x000002AA
		[SettingProperty("Spawn At Player Kingdom", HintText = "Do custom troops spawn at settlements in the kingdom lead by the player clan?  AI lords visiting settlements in player lead kingdom can hire these troops", RequireRestart = false)]
		[SettingPropertyGroup("Setting")]
		public bool SpawnAtPlayerKingdom { get; set; } = false;

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000009 RID: 9 RVA: 0x000020B3 File Offset: 0x000002B3
		// (set) Token: 0x0600000A RID: 10 RVA: 0x000020BB File Offset: 0x000002BB
		[SettingProperty("Full Unit Editor", HintText = "Can non-custom (vanilla, added by other mods) troops be edited?", RequireRestart = false)]
		[SettingPropertyGroup("Setting")]
		public bool FullUnitEditor { get; set; } = false;

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600000B RID: 11 RVA: 0x000020C4 File Offset: 0x000002C4
		// (set) Token: 0x0600000C RID: 12 RVA: 0x000020CC File Offset: 0x000002CC
		[SettingProperty("Clan Recruit Custom Troop", HintText = "Do parties part of the player clan recruit custom troops at settlements instead of the default troops?", RequireRestart = false)]
		[SettingPropertyGroup("Setting")]
		public bool ClanRecruitCustomTroop { get; set; } = false;
	}
}
