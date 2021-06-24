using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace WorkshopStashRON
{
    public class WorkshopSaveSystem : SaveableTypeDefiner
    {
        public WorkshopSaveSystem() : base(21630001) { }

        protected override void DefineClassTypes()
        {
            AddClassDefinition(typeof(WorkshopStash), 01);
        }

        protected override void DefineContainerDefinitions()
        {
            ConstructContainerDefinition(typeof(List<WorkshopStash>));
            ConstructContainerDefinition(typeof(Dictionary<Town, WorkshopStash>));
        }
    }
}