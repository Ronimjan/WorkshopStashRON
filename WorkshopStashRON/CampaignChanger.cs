using Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace WorkshopStashRON
{
    class CampaignChanger : CampaignBehaviorBase
    {
        public static CampaignChanger Current { get; private set; }

        public Dictionary<Town, WorkshopStash> QuickAccess => _workshopStashSaveDictionary;

        private Dictionary<Town, WorkshopStash> _workshopStashSaveDictionary = new Dictionary<Town, WorkshopStash>();

        public CampaignChanger()
        {
            Current = this;
        }

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
            starter.AddGameMenuOption("workshop_manage", "Workshop_Toggle_Input", "Use materials from stash: {STASH_INPUT}", new GameMenuOption.OnConditionDelegate(ProductionCondition), new GameMenuOption.OnConsequenceDelegate(ToggleInput), false, -1, false);
            starter.AddGameMenuOption("workshop_manage", "Workshop_Toggle_Output", "Put produced goods into stash: {STASH_OUTPUT}", new GameMenuOption.OnConditionDelegate(ProductionCondition), new GameMenuOption.OnConsequenceDelegate(ToggleOutput), false, -1, false);
            starter.AddGameMenuOption("workshop_manage", "Workshop_Leave", "Back to town center", new GameMenuOption.OnConditionDelegate(BackCondition), new GameMenuOption.OnConsequenceDelegate(_ => GameMenu.SwitchToMenu("town")), true, -1, false);
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
            bool didWeFindIt = CampaignChanger.Current.QuickAccess.TryGetValue(town, out var stash);
            if (!didWeFindIt)
            {
                stash = new WorkshopStash();
                stash.Town = town;
                CampaignChanger.Current.QuickAccess.Add(town, stash);
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
            Dictionary<string, (bool, bool, string)> workshopStashSaveDictionaryString = new Dictionary<string, (bool, bool, string)>();
            Dictionary<string, int> temp;

            if (dataStore.IsSaving)
            {
                foreach (Settlement town in Campaign.Current.Settlements)
                {
                    bool registered = _workshopStashSaveDictionary.TryGetValue(town.Town, out WorkshopStash stash);
                    if (registered)
                    {
                        temp = new Dictionary<string, int>();
                        for (int i = 0; i < stash.Stash.Count; i++)
                        {
                            temp.Add(stash.Stash.GetItemAtIndex(i).ToString(), stash.Stash.GetElementNumber(i));
                        }

                        string jsonList = JsonConvert.SerializeObject(temp);

                        workshopStashSaveDictionaryString.Add(town.Town.ToString(), (stash.InputTrue, stash.OutputTrue, jsonList));
                    }
                }

                string jsonString = JsonConvert.SerializeObject(workshopStashSaveDictionaryString);
                dataStore.SyncData("workshopStashSaveDictionaryString", ref jsonString);

                jsonString = JsonConvert.SerializeObject(SubModule.modVersion);
                dataStore.SyncData("modVersionWorkshopStashRON", ref jsonString);
            }
            if (dataStore.IsLoading)
            {
                string jsonString = "";

                if (dataStore.SyncData("modVersionWorkshopStashRON", ref jsonString) && !string.IsNullOrEmpty(jsonString))
                {
                    SubModule.modVersion = JsonConvert.DeserializeObject<float?>(jsonString);

                }

                if (!SubModule.modVersion.HasValue)
                {
                    dataStore.SyncData("workshopStashSaveDictionaryString", ref _workshopStashSaveDictionary);
                    SubModule.modVersion = 1.1f;
                }
                else if(SubModule.modVersion == 1.1f)
                {
                    if (dataStore.SyncData("modVersionWorkshopStashRON", ref jsonString) && !string.IsNullOrEmpty(jsonString))
                    {
                        workshopStashSaveDictionaryString = JsonConvert.DeserializeObject<Dictionary<string, (bool, bool, string)>>(jsonString);

                        foreach(Settlement town in Campaign.Current.Settlements)
                        {
                            bool registered = workshopStashSaveDictionaryString.TryGetValue(town.Town.ToString(), out (bool, bool, string) value);
                            if (registered)
                            {
                                temp = JsonConvert.DeserializeObject<Dictionary<string, int>>(value.Item3);
                                WorkshopStash tempWorkshopStash = new WorkshopStash();

                                tempWorkshopStash.InputTrue = value.Item1;
                                tempWorkshopStash.OutputTrue = value.Item2;
                                tempWorkshopStash.Town = town.Town;

                                foreach(ItemObject item in Campaign.Current.Items)
                                {
                                    bool registeredItem = temp.TryGetValue(item.ToString(), out int num);
                                    if (registeredItem)
                                    {
                                        tempWorkshopStash.Stash.AddToCounts(item, num);
                                    }
                                }

                                _workshopStashSaveDictionary.Add(town.Town, tempWorkshopStash);
                            }
                        }

                    }
                }

            }
        }
    }
}