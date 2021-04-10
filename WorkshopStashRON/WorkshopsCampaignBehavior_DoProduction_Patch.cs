using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace WorkshopStashRON
{
    [HarmonyPatch(typeof(WorkshopsCampaignBehavior), "DoProduction")]
    public static class WorkshopsCampaignBehavior_DoProduction_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructionsIn)
        {
            var sufficientInputsMethod = typeof(WorkshopsCampaignBehavior).GetMethod("DetermineTownHasSufficientInputs", BindingFlags.Static | BindingFlags.NonPublic);

            foreach (var instruction in instructionsIn)
            {
                if (instruction.Calls(sufficientInputsMethod))
                {
                    instruction.operand = typeof(WorkshopsCampaignBehavior_DoProduction_Patch).GetMethod("DetermineTownHasSufficientInputsReplacement", BindingFlags.Static | BindingFlags.Public);
                }
                yield return instruction;
            }
        }

        public static bool DetermineTownHasSufficientInputsReplacement(WorkshopType.Production production, Town town, out int inputMaterialCost)
        {
            ItemRoster stashRoster = null;
            for (int i = 0; i < town.Workshops.Length; i++)
            {
                if (town.Workshops[i].Owner == Hero.MainHero)
                {
                    bool didWeFindIt = CampaignChanger.Current.QuickAccess.TryGetValue(town, out var stash);

                    if (didWeFindIt && stash.InputTrue)
                    {
                        stashRoster = stash.Stash;
                    }
                }
            }

            inputMaterialCost = 0;

            using (IEnumerator<ValueTuple<ItemCategory, int>> enumerator = production.Inputs.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    ValueTuple<ItemCategory, int> current = enumerator.Current;
                    ItemCategory item1 = current.Item1;
                    int item2 = current.Item2;
                    ItemRoster itemRoster = town.Owner.ItemRoster;
                    int num1 = 0;

                    if (stashRoster != null)
                    {
                        for (int a = 0; a < stashRoster.Count; a++)
                        {
                            ItemObject itemAtIndex1 = stashRoster.GetItemAtIndex(a);
                            if (itemAtIndex1.ItemCategory == item1)
                            {
                                num1 = stashRoster.GetElementNumber(a);
                            }
                        }
                    }

                    for (int i = 0; i < itemRoster.Count; i++)
                    {
                        ItemObject itemAtIndex = itemRoster.GetItemAtIndex(i);
                        if (itemAtIndex.ItemCategory == item1)
                        {
                            int num = Math.Min(item2, itemRoster.GetElementNumber(i));
                            item2 = item2 + num1 - num;
                            inputMaterialCost = inputMaterialCost + town.GetItemPrice(itemAtIndex, null, false) * num;
                        }
                    }
                    if (item2 >= 0)
                    {
                        continue;
                    }
                    return false;
                }
                return true;
            }
        }
    }
}