using System;
using System.Collections.Generic;
using HarmonyLib;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace MyLittleWarband
{
	// Token: 0x0200003D RID: 61
	public class SubModule : MBSubModuleBase
	{
		// Token: 0x060001D2 RID: 466 RVA: 0x000028C9 File Offset: 0x00000AC9
		protected override void OnBeforeInitialModuleScreenSetAsRoot()
		{
			base.OnBeforeInitialModuleScreenSetAsRoot();
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x0000D978 File Offset: 0x0000BB78
		protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
		{
			bool flag = !(game.GameType is Campaign);
			if (!flag)
			{
				((CampaignGameStarter)gameStarterObject).AddBehavior(new TroopsBehavior());
				((CampaignGameStarter)gameStarterObject).AddBehavior(new CustomUnitsBehavior());
				((CampaignGameStarter)gameStarterObject).AddBehavior(new XMLExporter());
				this.instance = GlobalSettings<MyLittleWarbandSetting>.Instance;
				SubModule.ClanRecruitCustomTroop = this.instance.ClanRecruitCustomTroop;
				SubModule.SpawnAtPlayerSettlement = this.instance.SpawnAtPlayerSettlement;
				SubModule.SpawnAtPlayerKingdom = this.instance.SpawnAtPlayerKingdom;
				SubModule.FullUnitEditor = this.instance.FullUnitEditor;
			}
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x0000DA1C File Offset: 0x0000BC1C
		public static void ExecuteActionOnNextTick(Action action)
		{
			bool flag = action == null;
			if (!flag)
			{
				SubModule.ActionsToExecuteNextTick.Add(action);
			}
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x0000DA44 File Offset: 0x0000BC44
		protected override void OnApplicationTick(float dt)
		{
			base.OnApplicationTick(dt);
			SubModule.CheckKeyPressed();
			foreach (Action action in SubModule.ActionsToExecuteNextTick)
			{
				action();
			}
			SubModule.ActionsToExecuteNextTick.Clear();
			this.instance = GlobalSettings<MyLittleWarbandSetting>.Instance;
			bool flag = this.instance != null;
			if (flag)
			{
				SubModule.ClanRecruitCustomTroop = this.instance.ClanRecruitCustomTroop;
				SubModule.SpawnAtPlayerSettlement = this.instance.SpawnAtPlayerSettlement;
				SubModule.SpawnAtPlayerKingdom = this.instance.SpawnAtPlayerKingdom;
				SubModule.FullUnitEditor = this.instance.FullUnitEditor;
			}
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x0000DB0C File Offset: 0x0000BD0C
		public static void CheckKeyPressed()
		{
			/*
An exception occurred when decompiling this method (060001D6)

ICSharpCode.Decompiler.DecompilerException: Error decompiling System.Void MyLittleWarband.SubModule::CheckKeyPressed()

 ---> System.NullReferenceException: Object reference not set to an instance of an object.
   at ICSharpCode.Decompiler.ILAst.TypeAnalysis.SubstituteTypeArgs(TypeSig type, TypeSig typeContext, IMethod method) in D:\a\dnSpy\dnSpy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\ILAst\TypeAnalysis.cs:line 1026
   at ICSharpCode.Decompiler.ILAst.TypeAnalysis.GetFieldType(IField field) in D:\a\dnSpy\dnSpy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\ILAst\TypeAnalysis.cs:line 1018
   at ICSharpCode.Decompiler.ILAst.TypeAnalysis.DoInferTypeForExpression(ILExpression expr, TypeSig expectedType, Boolean forceInferChildren) in D:\a\dnSpy\dnSpy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\ILAst\TypeAnalysis.cs:line 457
   at ICSharpCode.Decompiler.ILAst.TypeAnalysis.InferBinaryArguments(ILExpression left, ILExpression right, TypeSig expectedType, Boolean forceInferChildren, TypeSig leftPreferred, TypeSig rightPreferred) in D:\a\dnSpy\dnSpy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\ILAst\TypeAnalysis.cs:line 1130
   at ICSharpCode.Decompiler.ILAst.TypeAnalysis.InferArgumentsInBinaryOperator(ILExpression expr, Nullable`1 isSigned, TypeSig expectedType) in D:\a\dnSpy\dnSpy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\ILAst\TypeAnalysis.cs:line 1062
   at ICSharpCode.Decompiler.ILAst.TypeAnalysis.DoInferTypeForExpression(ILExpression expr, TypeSig expectedType, Boolean forceInferChildren) in D:\a\dnSpy\dnSpy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\ILAst\TypeAnalysis.cs:line 889
   at ICSharpCode.Decompiler.ILAst.TypeAnalysis.InferTypeForExpression(ILExpression expr, TypeSig expectedType, Boolean forceInferChildren) in D:\a\dnSpy\dnSpy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\ILAst\TypeAnalysis.cs:line 309
   at ICSharpCode.Decompiler.ILAst.TypeAnalysis.DoInferTypeForExpression(ILExpression expr, TypeSig expectedType, Boolean forceInferChildren) in D:\a\dnSpy\dnSpy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\ILAst\TypeAnalysis.cs:line 348
   at ICSharpCode.Decompiler.ILAst.TypeAnalysis.InferTypeForExpression(ILExpression expr, TypeSig expectedType, Boolean forceInferChildren) in D:\a\dnSpy\dnSpy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\ILAst\TypeAnalysis.cs:line 309
   at ICSharpCode.Decompiler.ILAst.TypeAnalysis.RunInference(ILExpression expr) in D:\a\dnSpy\dnSpy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\ILAst\TypeAnalysis.cs:line 284
   at ICSharpCode.Decompiler.ILAst.TypeAnalysis.RunInference() in D:\a\dnSpy\dnSpy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\ILAst\TypeAnalysis.cs:line 220
   at ICSharpCode.Decompiler.ILAst.TypeAnalysis.Run(DecompilerContext context, ILBlock method) in D:\a\dnSpy\dnSpy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\ILAst\TypeAnalysis.cs:line 49
   at ICSharpCode.Decompiler.ILAst.ILAstOptimizer.Optimize(DecompilerContext context, ILBlock method, AutoPropertyProvider autoPropertyProvider, StateMachineKind& stateMachineKind, MethodDef& inlinedMethod, AsyncMethodDebugInfo& asyncInfo, ILAstOptimizationStep abortBeforeStep) in D:\a\dnSpy\dnSpy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\ILAst\ILAstOptimizer.cs:line 264
   at ICSharpCode.Decompiler.Ast.AstMethodBodyBuilder.CreateMethodBody(IEnumerable`1 parameters, MethodDebugInfoBuilder& builder) in D:\a\dnSpy\dnSpy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\Ast\AstMethodBodyBuilder.cs:line 123
   at ICSharpCode.Decompiler.Ast.AstMethodBodyBuilder.CreateMethodBody(MethodDef methodDef, DecompilerContext context, AutoPropertyProvider autoPropertyProvider, IEnumerable`1 parameters, Boolean valueParameterIsKeyword, StringBuilder sb, MethodDebugInfoBuilder& stmtsBuilder) in D:\a\dnSpy\dnSpy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\Ast\AstMethodBodyBuilder.cs:line 88
   --- End of inner exception stack trace ---
   at ICSharpCode.Decompiler.Ast.AstMethodBodyBuilder.CreateMethodBody(MethodDef methodDef, DecompilerContext context, AutoPropertyProvider autoPropertyProvider, IEnumerable`1 parameters, Boolean valueParameterIsKeyword, StringBuilder sb, MethodDebugInfoBuilder& stmtsBuilder) in D:\a\dnSpy\dnSpy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\Ast\AstMethodBodyBuilder.cs:line 92
   at ICSharpCode.Decompiler.Ast.AstBuilder.AddMethodBody(EntityDeclaration methodNode, EntityDeclaration& updatedNode, MethodDef method, IEnumerable`1 parameters, Boolean valueParameterIsKeyword, MethodKind methodKind) in D:\a\dnSpy\dnSpy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\Ast\AstBuilder.cs:line 1686
*/;
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x000028D3 File Offset: 0x00000AD3
		protected override void OnSubModuleLoad()
		{
			base.OnSubModuleLoad();
			new Harmony("MyLittleWarband").PatchAll();
		}

		// Token: 0x040000DB RID: 219
		private static readonly List<Action> ActionsToExecuteNextTick = new List<Action>();

		// Token: 0x040000DC RID: 220
		public static bool ReplaceAllForPlayer = true;

		// Token: 0x040000DD RID: 221
		private MyLittleWarbandSetting instance;

		// Token: 0x040000DE RID: 222
		public static bool ClanRecruitCustomTroop;

		// Token: 0x040000DF RID: 223
		public static bool SpawnAtPlayerSettlement;

		// Token: 0x040000E0 RID: 224
		public static bool SpawnAtPlayerKingdom;

		// Token: 0x040000E1 RID: 225
		public static bool FullUnitEditor;
	}
}
