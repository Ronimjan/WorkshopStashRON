using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace WorkshopStashRON
{
    class  CampaignChanger : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener((object)this, new Action<CampaignGameStarter>(this.OnAfterNewGameCreated));
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener((object)this, new Action<CampaignGameStarter>(this.OnAfterNewGameCreated));
        }

        private void OnAfterNewGameCreated(CampaignGameStarter starter)
        {
            starter.AddGameMenu("workshop_manage", "You are visiting your workshops.", new OnInitDelegate(args => { args.MenuTitle = new TextObject("Workshops", null); }), GameOverlays.MenuOverlayType.SettlementWithBoth, GameMenu.MenuFlags.none, (object)null);
            starter.AddGameMenuOption("town", "Workshop_Stash", "Manage your workshops", new GameMenuOption.OnConditionDelegate(HasAnyWorkshops), new GameMenuOption.OnConsequenceDelegate(x => GameMenu.SwitchToMenu("workshop_manage")), false, 6, false);
            starter.AddGameMenuOption("workshop_manage", "Workshop_Stash_Browse", "Browse your Stash", new GameMenuOption.OnConditionDelegate(StashCondition), new GameMenuOption.OnConsequenceDelegate(StashConsequence), false, -1, false);
            starter.AddGameMenuOption("workshop_manage", "Workshop_Toggle_Input", "Use materials from stash: {INPUT_STASH}" ,new GameMenuOption.OnConditionDelegate(ProductionCondition), new GameMenuOption.OnConsequenceDelegate(ToggleInput), false, -1, false);
            starter.AddGameMenuOption("workshop_manage", "Workshop_Toggle_Output", "Place produced goods in stash: {OUTPUT_STASH}", new GameMenuOption.OnConditionDelegate(ProductionCondition), new GameMenuOption.OnConsequenceDelegate(ToggleOutput), false, -1, false);
            starter.AddGameMenuOption("workshop_manage", "Workshop_Leave", "Back to towncenter", new GameMenuOption.OnConditionDelegate(BackCondition), new GameMenuOption.OnConsequenceDelegate(_ => GameMenu.SwitchToMenu("town")), true, -1, false);
            
        }

        private static void StashConsequence(MenuCallbackArgs args)
        {
            var stash = GetCurrentSettlementStash();
            InventoryManager.OpenScreenAsStash(stash.Stash);
        }

        private static bool StashCondition(MenuCallbackArgs args)
        {
            bool disableOption;
            TextObject disabledText;
            bool canPlayerDo = Campaign.Current.Models.SettlementAccessModel.CanMainHeroDoSettlementAction(Settlement.CurrentSettlement, SettlementAccessModel.SettlementAction.Trade, out disableOption, out disabledText);
            args.optionLeaveType = GameMenuOption.LeaveType.Trade;
            return MenuHelper.SetOptionProperties(args, canPlayerDo, disableOption, disabledText);
        }

        private static bool HasAnyWorkshops(MenuCallbackArgs args)
        {
            args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
            return Settlement.CurrentSettlement.GetComponent<Town>().Workshops.Any(x => x.Owner == Hero.MainHero);
        }
        private static bool BackCondition(MenuCallbackArgs args)
        {
            args.optionLeaveType = GameMenuOption.LeaveType.Leave;
            return true;
        }

        private static WorkshopStash GetCurrentSettlementStash()
        {
            var town = Settlement.CurrentSettlement.GetComponent<Town>();
            var stash = MBObjectManager.Instance.GetObject<WorkshopStash>(x => x.Town == town);

            if (stash == null)
            {
                stash = MBObjectManager.Instance.CreateObject<WorkshopStash>();
                stash.Town = town;
            }

            return stash;
        }

        private static bool ProductionCondition(MenuCallbackArgs args)
        {
            var stash = GetCurrentSettlementStash();
            MBTextManager.SetTextVariable("STASH_INPUT", stash.InputTrue ? "Yes" : "No", true);
            MBTextManager.SetTextVariable("STASH_OUTPUT", stash.OutputTrue ? "Yes" : "No", true);

            args.optionLeaveType = GameMenuOption.LeaveType.Craft;
            return true;
        }

        private static void ToggleInput(MenuCallbackArgs args)
        {
            var stash = GetCurrentSettlementStash();
            stash.InputTrue = !stash.InputTrue;
            GameMenu.SwitchToMenu("workshop_manage");
        }

        private static void ToggleOutput(MenuCallbackArgs args)
        {
            var stash = GetCurrentSettlementStash();
            stash.OutputTrue = !stash.OutputTrue;
            GameMenu.SwitchToMenu("workshop_manage");
        }

        public override void SyncData(IDataStore dataStore)
        {
            return;
        }
    }
}
