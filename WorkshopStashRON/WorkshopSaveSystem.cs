using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            ConstructContainerDefinition(typeof(Dictionary<string, List<WorkshopStash>>));
            ConstructContainerDefinition(typeof(Dictionary<string, Dictionary<string, List<WorkshopStash>>>));
        }
    }
}