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
            var a = new WorkshopSaveSystem();
            if (!(game.GameType is Campaign)) { return; }

            MBObjectManager.Instance.RegisterType<WorkshopStash>("TownWorkshopStash", "TownWorkshopStashes", 21630002);

            if (gameStarterObject is CampaignGameStarter starter)
            {
                starter.AddBehavior(new CampaignChanger());
            }

            new HarmonyLib.Harmony("WorkshopStashRON.patcher").PatchAll();
        }
    }
}