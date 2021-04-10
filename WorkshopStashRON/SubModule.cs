using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;
using HarmonyLib;

namespace WorkshopStashRON
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            if (!(game.GameType is Campaign)) { return; }

            MBObjectManager.Instance.RegisterType<WorkshopStash>("workshop_Stash", "workshop_stash", 21630002);

            if (gameStarterObject is CampaignGameStarter starter)
            {
                starter.AddBehavior(new CampaignChanger());
            }

            new HarmonyLib.Harmony("WorkshopStashRON.patcher").PatchAll();
        }
    }
}