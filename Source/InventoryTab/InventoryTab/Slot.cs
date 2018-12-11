using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RimWorld;
using Verse;

namespace InventoryTab
{
    public class Slot : IComparable<Slot> {
        //This is the thing that is in this slot
        public Thing thingInSlot { get; private set; }
        public MainTabWindow_Inventory.Tabs tab { get; private set; }

        //This is all of the thing stack that was found
        public List<Thing> groupedThings;
        public int stackSize;
        
        public Slot(Thing thing, MainTabWindow_Inventory.Tabs tab) {
            this.thingInSlot = thing;
            this.tab = tab;

            this.groupedThings = new List<Thing>();
            groupedThings.Add(thing);
            this.stackSize = thing.stackCount;
        }

        //Used for when List<T>.Sort is called
        //1 means the object is greater then what it's being compared to
        //-1 means the object is less then what it's being compared to
        // 0 means they are equal
        public int CompareTo(Slot other) {
            if (thingInSlot.MarketValue > other.thingInSlot.MarketValue) {
                return 1;
            } else if (thingInSlot.MarketValue < other.thingInSlot.MarketValue) {
                return -1;
            }

            //If things have the same market value sort based on name
            if (thingInSlot.MarketValue == other.thingInSlot.MarketValue) {
                //More corpse bullshit
                if (thingInSlot.def.IsWithinCategory(ThingCategoryDefOf.Corpses) == true && other.thingInSlot.def.IsWithinCategory(ThingCategoryDefOf.Corpses) == true){
                    
                    Corpse a = thingInSlot as Corpse;
                    Corpse b = other.thingInSlot as Corpse;
                    
                    if (a != null && b != null && a.InnerPawn.def.race.Humanlike == true && b.InnerPawn.def.race.Humanlike == true) {
                        return string.Compare(a.InnerPawn.Label, b.InnerPawn.Label, StringComparison.CurrentCulture);
                    }

                }

                return string.Compare(thingInSlot.LabelNoCount, other.thingInSlot.LabelNoCount, StringComparison.CurrentCulture);
            }

            return 0;
        }
    }
}
