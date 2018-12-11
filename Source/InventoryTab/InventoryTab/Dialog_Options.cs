using System;
using System.Collections.Generic;

using UnityEngine;

using Verse;

using InventoryTab.Helpers;

namespace InventoryTab
{
    public class Dialog_Options : Window {

        public override Vector2 InitialSize {
            get {
                return new Vector2(512f, (float)UI.screenHeight / 2);
            }
        }

        private OptionsHelper _options;

        public Dialog_Options(OptionsHelper options) {
            this._options = options;

            this.doCloseButton = true;
            this.doCloseX = true;
        }

        public override void DoWindowContents(Rect inRect) {
            TextAnchor anchor = Text.Anchor;
            GameFont font = Text.Font;
            //Header
            Rect titleRect = new Rect(0, 0, inRect.width, 25);
            Text.Anchor = TextAnchor.MiddleCenter;
            Text.Font = GameFont.Medium;

            Widgets.Label(titleRect, "InventoryOptions".Translate());

            Text.Anchor = TextAnchor.MiddleLeft;
            Text.Font = GameFont.Small;
            //Draw all the options
            Rect optionsRects = new Rect(inRect.x, inRect.y, inRect.width, inRect.height - 25);
            _options.DrawOptions(optionsRects);

            Text.Anchor = anchor;
            Text.Font = font;
        }
    }
}
