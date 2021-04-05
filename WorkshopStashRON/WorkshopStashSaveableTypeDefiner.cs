using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace WorkshopStashRON
{
    public class WorkshopStashSaveableTypeDefiner : SaveableTypeDefiner
    {
        public WorkshopStashSaveableTypeDefiner() : base(03060000) { }
        protected override void DefineClassTypes()
        {
            this.AddClassDefinition(typeof(WorkshopStash), 1);
        }

        protected override void DefineGenericClassDefinitions()
        {
            this.ConstructGenericClassDefinition(typeof(MBObjectManager.ObjectTypeRecord<WorkshopStash>));
        }

        protected override void DefineContainerDefinitions()
        {
            this.ConstructContainerDefinition(typeof(List<WorkshopStash>));
            this.ConstructContainerDefinition(typeof(Dictionary<MBGUID, WorkshopStash>));
            this.ConstructContainerDefinition(typeof(Dictionary<string, WorkshopStash>));
        }
    }
}
