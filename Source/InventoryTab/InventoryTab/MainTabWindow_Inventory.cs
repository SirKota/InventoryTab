using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

using UnityEngine;

using RimWorld;
using Verse;

using InventoryTab.Helpers;

namespace InventoryTab{
    public class MainTabWindow_Inventory : MainTabWindow {

        //This is used for convenience sake
        public enum Tabs {
            All,
            Foods,
            Manufactured,
            RawResources,
            Items,
            Weapons,
            Apperal,
            Building,
            Chunks,
            Corpses
        }
        //Sets the size of the window
        public override Vector2 RequestedTabSize
        {
            get
            {
                return new Vector2(600f, 750f);
            }
        }

        //Used to define the height of the slot for the items
        private const float _slotHeight = 32;
        //Hold the position of the scroll
        private Vector2 _scrollPosition;
        //What tab is currently being viewed
        private Tabs _currentTab = Tabs.All;

        //Used for searching items
        private string _searchFor;

        //Chached list of all the corpses found
        private List<Corpse> _corpses = new List<Corpse>();

        private List<Thing> _things;
        private List<Slot> _slots;

        private float _timer;

        private bool _dirty = false;

        //A holder for all the options
        private OptionsHelper _options;

        public MainTabWindow_Inventory() {
            this._options = new OptionsHelper(this);
        }

        public override void PostOpen() {
            base.PostOpen();
            //Reset this to zero, because it retains it;s position form the
            //previous time it was opened
            this._scrollPosition = Vector2.zero;

            //Set it so it's in the map view, no point seeing the items you have in world view
            //plus the selector might not work right in planet view(untested)
            Find.World.renderer.wantedMode = RimWorld.Planet.WorldRenderMode.None;
            
            UpdateThings();
        }

        public override void DoWindowContents(Rect inRect) {
            base.DoWindowContents(inRect);
            _timer -= Time.deltaTime;

            //Cache the font and anchor before changing it, so 
            //later we can set it back to what it was before.
            GameFont fontBefore = Text.Font;
            TextAnchor anchorBefore = Text.Anchor;

            //Clear the cached corpses
            _corpses.Clear();
            
            if (_options.AutoUpdate == true && _timer < 0) {
                //Cache all items based on options
                _dirty = true;
                _timer = _options.AutoUpdateTimeInterval;
            }

            if (_dirty == true) { UpdateThings(); }

            //Draw the header; options, search and how many items were found
            DrawHeader(inRect, _things.Count);
            //Draws the tabs
            DrawTabs(inRect);
            //Draw all the items based on tabs and options
            DrawMainRect(inRect, _slots);

            //Reset the font and anchor after we are done drawing all our stuff
            Text.Font = fontBefore;
            Text.Anchor = anchorBefore;
        }

        public override void PostClose(){
            if (Find.WindowStack.IsOpen<Dialog_Options>() == true){
                Find.WindowStack.TryRemove(typeof(Dialog_Options));
            }

            base.PostClose();
        }

        public void UpdateThings() {
            //Do an inital find of all the items on the map based on the options
            _things = ItemFinderHelper.GetAllMapItems(Find.CurrentMap, _options);
            _slots = SortSlotsWithCategory(CombineThings(_things.ToArray()), _currentTab);
            _dirty = false;
        }

        public void Dirty() {
            _dirty = true;
        }

        private void DrawHeader(Rect inRect, int itemCount) {
            //Draw a label for all the items found
            Rect label = new Rect(0, 0, 256, 128);
            Text.Font = GameFont.Small;
            Widgets.Label(label, "TotalFound".Translate() + ": " + itemCount);
            //Draw the search bar
            Rect searchOptions = new Rect(0, 25, 200, 25);
            _searchFor = Widgets.TextArea(searchOptions, _searchFor);

            Rect optionsRect = new Rect(inRect.width - 25, 0, 25, 25);
            TooltipHandler.TipRegion(optionsRect, new TipSignal("Options".Translate()));
            if (Widgets.ButtonImage(optionsRect, ContentFinder<Texture2D>.Get("UI/settings", true)) == true) {
                if (Find.WindowStack.IsOpen<Dialog_Options>() == true) {
                    Find.WindowStack.TryRemove(typeof(Dialog_Options));
                }
                else {
                    Find.WindowStack.Add(new Dialog_Options(_options));
                }
            }

            Rect searchRect = new Rect(optionsRect.x - 35, 0, 25, 25);
            TooltipHandler.TipRegion(searchRect, new TipSignal("Search".Translate()));
            if (Widgets.ButtonImage(searchRect, ContentFinder<Texture2D>.Get("UI/search", true)) == true) {
                _dirty = true;
            }
            
        }

        private void DrawTabs(Rect rect) {
            Rect tabRect = new Rect(rect);
            //Need to give it a minY or they get drawn one pixel tall
            tabRect.yMin += 120f;

            List<TabRecord> tabs = new List<TabRecord>();

            //Creating all the tabs, we have to reCreate all these at runtime because they don't update
            TabRecord tabRec_All = new TabRecord("TabAll".Translate(), delegate ()                     { TabClick(Tabs.All); }, _currentTab == Tabs.All);
            TabRecord tabRec_Foods = new TabRecord("TabFoods".Translate(), delegate ()                 { TabClick(Tabs.Foods); }, _currentTab == Tabs.Foods);
            TabRecord tabRec_Manufactured = new TabRecord("TabManufactured".Translate(), delegate ()    {  TabClick(Tabs.Manufactured); }, _currentTab == Tabs.Manufactured);
            TabRecord tabRec_RawResources = new TabRecord("TabRawResources".Translate(), delegate ()  {  TabClick(Tabs.RawResources); }, _currentTab == Tabs.RawResources);
            TabRecord tabRec_Items = new TabRecord("TabItems".Translate(), delegate ()                 {  TabClick(Tabs.Items); }, _currentTab == Tabs.Items);

            TabRecord tabRec_Weapon = new TabRecord("TabWeapons".Translate(), delegate ()               {  TabClick(Tabs.Weapons); }, _currentTab == Tabs.Weapons);
            TabRecord tabRec_Apperal = new TabRecord("TabApparel".Translate(), delegate ()             {  TabClick(Tabs.Apperal); }, _currentTab == Tabs.Apperal);
            TabRecord tabRec_Buildings = new TabRecord("TabBuildings".Translate(), delegate ()         {  TabClick(Tabs.Building); }, _currentTab == Tabs.Building);
            TabRecord tabRec_Chunks = new TabRecord("TabChunks".Translate(), delegate ()               { TabClick(Tabs.Chunks); }, _currentTab == Tabs.Chunks);
            TabRecord tabRec_Corpses = new TabRecord("TabCorpses".Translate(), delegate ()             { TabClick(Tabs.Corpses); }, _currentTab == Tabs.Corpses);

            //Add them to the list
            tabs.Add(tabRec_All);
            tabs.Add(tabRec_Foods);
            tabs.Add(tabRec_Manufactured);
            tabs.Add(tabRec_RawResources);
            tabs.Add(tabRec_Items);

            tabs.Add(tabRec_Weapon);
            tabs.Add(tabRec_Apperal);
            tabs.Add(tabRec_Buildings);
            tabs.Add(tabRec_Chunks);
            tabs.Add(tabRec_Corpses);

            //Draw the tabs, the last argument is how many rows you want
            TabDrawer.DrawTabs(tabRect, tabs, 2);
        }

        private void DrawMainRect(Rect inRect, List<Slot> slots) {
            Rect mainRect = new Rect(inRect.x, inRect.y + 37f + (_slotHeight * 3), inRect.width, inRect.height - 37f);
            //Creats slots for all the items; combines, sorts into catergorys and checks for searches all in one line 
            List<Slot> categorizedSlots = GetSearchForList(slots);
            //Sort based on market value
            categorizedSlots.Sort();

            //This is for the scrolling
            Rect viewRect = new Rect(0, 0, mainRect.width - 16f, categorizedSlots.Count * _slotHeight + 6f + (_slotHeight * 3));
            Widgets.BeginScrollView(mainRect, ref _scrollPosition, viewRect);
            {
                for (int i = 0; i < categorizedSlots.Count; i++) {
                    Rect slotRect = new Rect(0, i * _slotHeight, viewRect.width, _slotHeight);

                    //For every second slot hightlight it to make it a bit easier to see
                    if (i % 2 == 1) Widgets.DrawLightHighlight(slotRect);

                    Widgets.DrawHighlightIfMouseover(slotRect);
                    
                    //Draw the slot
                    DrawThingSlot(categorizedSlots[i], slotRect);
                }
            }
            Widgets.EndScrollView();
        }

        private void DrawThingSlot(Slot slot, Rect slotRect) {
            Thing thing = slot.thingInSlot;

            //Draw the image of the thing
            Rect imageRect = new Rect(0f, slotRect.y, 32f, 32f);
            Widgets.ThingIcon(imageRect, thing);

            Widgets.InfoCardButton(slotRect.x + imageRect.width + 5, slotRect.y, thing.def);

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;

            Rect labelRect = new Rect(slotRect);
            labelRect.x += imageRect.width + 35;

            //Set the label for the thing, we use custom stacksize so we have to set it here
            string thingLabel = thing.LabelCapNoCount + " (x" + slot.stackSize + ")";
            //If item is a humanlike corpse we want to display their name
            if (slot.tab == Tabs.Corpses && (thing as Corpse) != null && (thing as Corpse).InnerPawn.def.race.Humanlike == true){
                thingLabel = thing.Label;
            }

            if (Widgets.ButtonInvisible(labelRect) == true){
                //Handles clicking of the slot, this was a bitch to get working correctly
                HandleClick(slot.groupedThings);
            }

            Widgets.Label(labelRect, thingLabel);
        }

        private void TabClick(Tabs tab) {
            _currentTab = tab;
            _scrollPosition = Vector2.zero;
            _dirty = true;
        }

        //Disclaimer i hate how i had to handle the corpses in this method
        private void HandleClick(List<Thing> things) {
            
            Find.Selector.ClearSelection();
            //Set this so when we are looping we only jump to one thing
            CameraJumperHelper.alreadyJumpedThisLoop = false;

            for (int i = 0; i < things.Count; i++){
                Corpse corpse = null;
                Pawn pawn = null;
                
                //Checks the thing to find out if its in a pawn inventory
                if ((things[i].ParentHolder as Pawn_EquipmentTracker) != null) {
                    pawn = (things[i].ParentHolder as Pawn_EquipmentTracker).pawn;
                    //we need to check if the pawn is dead beacuase we can't selected a dead pawn
                    //we need to select it's corpse otherwise just select the pawn
                    if (CheckForCorpse(pawn, out corpse) == true) {
                        Find.Selector.Select(corpse);
                    } else Find.Selector.Select(pawn);
                }
                else if ((things[i].ParentHolder as Pawn_ApparelTracker) != null) {
                    pawn = (things[i].ParentHolder as Pawn_ApparelTracker).pawn;
                    if (CheckForCorpse(pawn, out corpse) == true){
                        Find.Selector.Select(corpse);
                    } else Find.Selector.Select(pawn);
                }
                else if ((things[i].ParentHolder as Pawn_InventoryTracker) != null) {
                    pawn = (things[i].ParentHolder as Pawn_InventoryTracker).pawn;
                    if (CheckForCorpse(pawn, out corpse) == true){
                        Find.Selector.Select(corpse);
                    } else Find.Selector.Select(pawn);
                } else {
                    //And if it's not attach to a pawn or a pawn's corpse it's most likly just a thing
                    Find.Selector.Select(things[i]);
                }

                //Try to jump to the thing
                CameraJumperHelper.Jump(things.ToArray());
                //Flag it so we don't jump after this
                CameraJumperHelper.alreadyJumpedThisLoop = true;
            }
        }

        //This is for checking a pawn against a corpse to see if it belongs to it
        private bool CheckForCorpse(Pawn pawn, out Corpse corpse) {
            if (pawn.Dead == false) {
                corpse = null;
                return false;
            }

            for (int i = 0; i < _corpses.Count; i++) {
                if (pawn == _corpses[i].InnerPawn) {
                    corpse = _corpses[i];
                    return true;
                }
            }
            corpse = null;
            return false;
        }

        //Combines all the things into easier to manage slots
        private List<Slot> CombineThings(Thing[] things) {
            Dictionary<string, Slot> slotMap = new Dictionary<string, Slot>();

            for (int i = 0; i < things.Length; i++) {
                string tId = things[i].LabelNoCount;

                //If a thing is a corpse then we need to added it to the _corpse
                //so later we can check it against pawn
                if (things[i].def.IsWithinCategory(ThingCategoryDefOf.Corpses)) {
                    Corpse cor = things[i] as Corpse;

                    //Had to do a check to make sure the corpse is actually a corpse,
                    //Beacause mods like "Thanks for all the Fish" have things that are 
                    //catergorized as corpses but arent actually corpsese...
                    if (cor != null && cor.InnerPawn.def.race.Humanlike == true){
                        
                        _corpses.Add(things[i] as Corpse);
                    }
                }

                //If a slot already exists for the thing then add it to it
                if (slotMap.ContainsKey(tId) == true) {
                    slotMap[tId].groupedThings.Add(things[i]);
                    slotMap[tId].stackSize += things[i].stackCount;

                    continue;
                }

                if (slotMap.ContainsKey(tId)) {
                    Log.ErrorOnce("Some how we are attempting to add " + tId + " again...", 5);
                    break;
                }

                //Create a new slot
                Slot s = new Slot(things[i], AssignTab(things[i]));
                slotMap.Add(tId, s);
            }

            //Create and fill the return value
            List<Slot> result = new List<Slot>();
            foreach (Slot s in slotMap.Values) {
                result.Add(s);
            }

            return result;
        }

        //Sorts slots by catergory
        private List<Slot> SortSlotsWithCategory(List<Slot> slots, Tabs tab) {
            if(tab == Tabs.All) { return slots; }

            List<Slot> result = new List<Slot>();

            for (int i = 0; i < slots.Count; i++) {
                if (slots[i].tab == tab) {
                    result.Add(slots[i]);
                }
            }
            
            return result;
        }

        //Get a list of slots based on the _searchOf string
        private List<Slot> GetSearchForList(List<Slot> slots) {

            if (string.IsNullOrEmpty(_searchFor) == true) { return slots; }

            List<Slot> res = new List<Slot>();

            for (int i = 0; i < slots.Count; i++) {
                string searchText = slots[i].thingInSlot.Label + " " + slots[i].thingInSlot.LabelNoCount;
                if (Regex.IsMatch(searchText, _searchFor, RegexOptions.IgnoreCase) == true) {
                    res.Add(slots[i]);
                }
            }
            return res;
        }
        
        private Tabs AssignTab(Thing thing) {

            //For some reason a thing that's been minifide dosen't have a thing categories
            //but as far as i know it;s the only thing that dosen't so just add it to the builing tab
            if (thing.def.thingCategories == null) {
                //Log.ErrorOnce("For some reason the thing we are trying to assing a tab to its thingCatergories is null. thing: " + thing.Label, 2);
                return Tabs.Building;
            }

            List<ThingCategoryDef> catDefs = thing.def.thingCategories;
            for (int i = 0; i < catDefs.Count; i++) {
                
                if(thing.def.IsWithinCategory(ThingCategoryDefOf.Foods)){
                    return Tabs.Foods;
                }

                if (thing.def.IsWithinCategory(ThingCategoryDefOf.Manufactured)){
                    return Tabs.Manufactured;
                }

                if (thing.def.IsWithinCategory(ThingCategoryDefOf.ResourcesRaw)){
                    return Tabs.RawResources;
                }

                if (thing.def.IsWithinCategory(ThingCategoryDefOf.Items)){
                    return Tabs.Items;
                }

                if (thing.def.IsWithinCategory(ThingCategoryDefOf.Weapons)){
                    return Tabs.Weapons;
                }
                
                if (thing.def.IsWithinCategory(ThingCategoryDefOf.Apparel)) {
                    return Tabs.Apperal;
                }

                if (thing.def.IsWithinCategory(ThingCategoryDefOf.Buildings)){
                    return Tabs.Building;
                }

                if (thing.def.IsWithinCategory(ThingCategoryDefOf.Chunks)){
                    return Tabs.Chunks;
                }

                if (thing.def.IsWithinCategory(ThingCategoryDefOf.Corpses)){
                    return Tabs.Corpses;
                }

            }
            
            return Tabs.All;
        }


    }
}
