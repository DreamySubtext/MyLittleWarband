using System;
using System.Collections.Generic;
using TaleWorlds.SaveSystem;

namespace MyLittleWarband
{
	// Token: 0x02000037 RID: 55
	public class SaveDefiner : SaveableTypeDefiner
	{
		// Token: 0x0600019C RID: 412 RVA: 0x000027CC File Offset: 0x000009CC
		public SaveDefiner()
			: base(1436500005)
		{
		}

		// Token: 0x0600019D RID: 413 RVA: 0x000027DB File Offset: 0x000009DB
		protected override void DefineClassTypes()
		{
			base.AddClassDefinition(typeof(CustomUnit), 1, null);
		}

		// Token: 0x0600019E RID: 414 RVA: 0x000027F1 File Offset: 0x000009F1
		protected override void DefineContainerDefinitions()
		{
			base.ConstructContainerDefinition(typeof(Dictionary<string, CustomUnit>));
		}
	}
}
