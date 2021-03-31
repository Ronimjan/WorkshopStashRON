using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace WorkshopStashRON
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            if (!(game.GameType is Campaign)) { return; }

            MBObjectManager.Instance.RegisterType<WorkshopStash>("TownWorkshopStash", "TownWorkshopStashes", 0306000);

            if (gameStarterObject is CampaignGameStarter starter)
            {
                starter.AddBehavior(new CampaignChanger());
            }

        }
    }
}