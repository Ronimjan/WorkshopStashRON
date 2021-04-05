﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace WorkshopStashRON
{
    [HarmonyPatch(typeof(WorkshopsCampaignBehavior), "ProductionOutput")]
    public static class WorkshopsCampaignBehavior_ProductionOutput_Patch
    {
        public static bool Prefix(ItemCategory outputItem, Town town, Workshop workshop, int count, bool doNotEffectCapital)
        {
            if (workshop.Owner != Hero.MainHero) { return true; }

            var stash = MBObjectManager.Instance.GetObject<WorkshopStash>(x => x.Town == town);

            var index = stash.Stash.FindIndex(x => x.ItemCategory == outputItem);

            if (stash != null && stash.OutputTrue)
            {
                stash.Stash.AddToCounts(stash.Stash.GetItemAtIndex(index), count);
                return false;
            }
            return true;
        }
    }
}