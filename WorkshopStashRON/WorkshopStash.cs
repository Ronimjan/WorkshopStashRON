using System;
using TaleWorlds.SaveSystem;
using TaleWorlds.ObjectSystem;
using TaleWorlds.CampaignSystem;
namespace WorkshopStashRON
{
    [SaveableRootClass(21630000)]
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