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
    [SaveableClass(0306001)]
    class WorkshopStash : MBObjectBase
    {
        public WorkshopStash()
        {
            Stash = new ItemRoster();
        }

        [SaveableProperty(4)]
        public Boolean InputTrue { get; set; } = true;

        [SaveableProperty(5)]
        public Boolean OutputTrue { get; set; } = false;

        [SaveableProperty(6)]
        public ItemRoster Stash { get; set; }

        [SaveableProperty(7)]
        public Town Town { get; set; }
    }
}
