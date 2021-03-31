using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.SaveSystem;
using TaleWorlds.ObjectSystem;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;

namespace WorkshopStashRON
{
    [SaveableClass(030600)]
    class WorkshopStash : MBObjectBase
    {
        public WorkshopStash()
        {
            Stash = new ItemRoster();
        }

        [SaveableProperty(030600)]
        public Boolean InputTrue { get; set; }

        [SaveableProperty(030601)]
        public Boolean OutputTrue { get; set; }

        [SaveableProperty(030602)]
        public ItemRoster Stash { get; set; }

        [SaveableProperty(030603)]
        public Town Town { get; set; }
    }
}
