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
        public List<Thing> groupedThings = new List<Thing>();
        public int stackSize;
        
        public Slot(Thing thing, MainTabWindow_Inventory.Tabs tab) {
            this.thingInSlot = thing;
            this.tab = tab;

            groupedThings.Add(thing);
            this.stackSize = thing.stackCount;
        }

        //Used for when List<T>.Sort is called
        //1 means the object is greater then what it's being compared to
        //-1 means the object is less then what it's being compared to
        // 0 means they are equal
        public int CompareTo(Slot other) {
            if (groupedThings[0].MarketValue > other.groupedThings[0].MarketValue) {
                return 1;
            } else if (groupedThings[0].MarketValue < other.groupedThings[0].MarketValue) {
                return -1;
            }

            return 0;
        }
    }
}
