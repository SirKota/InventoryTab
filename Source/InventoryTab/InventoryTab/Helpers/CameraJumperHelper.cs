using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;

namespace InventoryTab.Helpers
{
    public static class CameraJumperHelper{

        //flag if we already looped through
        public static bool alreadyJumpedThisLoop = false;

        //Tell us the last thing we jumped to
        private static Thing _lastJumpedThing;
        //The index of the things we already jumped to
        private static int _jumpIndex;
        
        //This takes a whole list of things but will only jump to one at a time
        public static void Jump(Thing[] things) {
            if (alreadyJumpedThisLoop == true) { return; }

            //if the thing list only has one thing in it we don't need to
            // do the loop code
            if (things.Length == 1) {
                CameraJumper.TryJump(things[0]);
                //Resets the variable used for loop jumping
                ResetTracking(things[0]);
                return;
            }

            //if the thing currently looping is not the same as the last thing
            //then avoid the looping code and jump to the first in the list
            if (_lastJumpedThing != things[0]) {
                CameraJumper.TryJump(things[0]);
                //Resets the variable used for loop jumping
                ResetTracking(things[0]);
                return;
            }

            //If we made it this far then we need to do a loop jump
            _jumpIndex++;

            //this will keep it looping
            if (_jumpIndex > things.Length - 1) {
                _jumpIndex = 0;
            }

            CameraJumper.TryJump(things[_jumpIndex]);
        }

        private static void ResetTracking(Thing thing) {
            _lastJumpedThing = thing;
            _jumpIndex = 0;
        }

    }
}
