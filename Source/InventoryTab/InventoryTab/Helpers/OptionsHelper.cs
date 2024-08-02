using System;
using System.Collections.Generic;

using UnityEngine;

using Verse;

namespace InventoryTab.Helpers
{
    public class OptionsHelper {

        public bool SearchWholeMap { get => _searchMap; }
        public bool SearchPawns { get => _searchPawns;}
        public bool SearchShipChunk { get => _searchShipChunks; }

        public bool AutoUpdate { get => _autoUpdate; }
        public float AutoUpdateTimeInterval { get => _autoUpdateTimeInterval; }

        private const float _optionsHeight = 32;
        private const float _indexJumper = 30;
        //Options, all hard coded because thats going to be the simplest
        private bool _searchMap = false;
        private bool _searchPawns = false;
        private bool _searchShipChunks = false;

        private bool _autoUpdate = true;
        private float _autoUpdateTimeInterval = 5;

        private MainTabWindow_Inventory _window;

        public OptionsHelper(MainTabWindow_Inventory window) {
            this._window = window;
        }

        public void DrawOptions(Rect inRect) {
            int index = 1;
            float optionsX = 200;

            //Draw the option for searching the whole map
            Text.Anchor = TextAnchor.MiddleLeft;
            Rect rectStockpile = new Rect(0, _indexJumper, 128, 32);
            Widgets.Label(rectStockpile, "SearchMap".Translate());
            //This rect is created for the checkbox so when you mouse over it, it tells you what it does
            Rect checkBoxRect = new Rect(rectStockpile.x + optionsX, rectStockpile.y, 24, 24);
            Widgets.Checkbox(checkBoxRect.x, checkBoxRect.y, ref _searchMap);
            //add a tooltip for the searchMap option
            TooltipHandler.TipRegion(checkBoxRect, new TipSignal("SearchMapToolTip".Translate()));
            index++;

            //Draw the option for searching the pawns
            Rect rectPawn = new Rect(0, index * _indexJumper, 128, 32);
            Widgets.Label(rectPawn, "SearchPawns".Translate());
            //This rect is created for the checkbox so when you mouse over it, it tells you what it does
            checkBoxRect = new Rect(rectPawn.x + optionsX, rectPawn.y, 24, 24);
            Widgets.Checkbox(checkBoxRect.x, checkBoxRect.y, ref _searchPawns);
            //add a tooltip for the searchMap option
            TooltipHandler.TipRegion(checkBoxRect, new TipSignal("SearchPawnsToolTip".Translate()));
            index++;
            
            Rect rectAutoUpdate = new Rect(0, index * _indexJumper, 128, 32);
            Widgets.Label(rectAutoUpdate, "AutoUpdate".Translate());
            checkBoxRect = new Rect(rectAutoUpdate.x + optionsX, rectAutoUpdate.y, 24, 24);
            Widgets.Checkbox(checkBoxRect.x, checkBoxRect.y, ref _autoUpdate);
            TooltipHandler.TipRegion(rectAutoUpdate, new TipSignal("AutoUpdateToolTip".Translate()));
            index++;

            Rect rectTimeInterval = new Rect(0, index * _indexJumper, 256, 32);
            Widgets.Label(rectTimeInterval, string.Format("AutoUpdateTimeInterval".Translate() + ": {0}", _autoUpdateTimeInterval));
            Rect floatRect = new Rect(rectTimeInterval.x + optionsX, rectTimeInterval.y + 10, 128, 32);
            _autoUpdateTimeInterval = Widgets.HorizontalSlider(floatRect, _autoUpdateTimeInterval, 0, 5);
            
            if (Widgets.ButtonInvisible(inRect) == true) {
                _window.Dirty();
            }
            
        }

    }
}
