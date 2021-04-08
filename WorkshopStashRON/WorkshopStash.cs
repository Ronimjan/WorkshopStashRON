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

        [SaveableProperty(04)]
        public Boolean InputTrue { get; set; } = true;

        [SaveableProperty(05)]
        public Boolean OutputTrue { get; set; } = false;

        [SaveableProperty(06)]
        public ItemRoster Stash { get; set; }

        [SaveableProperty(07)]
        public Town Town { get; set; }
    }
}
