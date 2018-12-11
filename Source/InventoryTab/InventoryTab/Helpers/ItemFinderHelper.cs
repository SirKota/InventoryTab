using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RimWorld;
using Verse;

namespace InventoryTab.Helpers
{
    public class ItemFinderHelper {

        //This is how we get all the items on the map
        public static List<Thing> GetAllMapItems(Map map, OptionsHelper options){
            List<Thing> results = new List<Thing>();
            List<Thing> allThings = map.listerThings.AllThings;

            for (int i = 0; i < allThings.Count; i++){
                Thing thing = allThings[i];

                //If the thing is a item and is not in the fog then continue to the next
                if (thing.def.category != ThingCategory.Item || (thing.Position.Fogged(thing.Map)) == true){ continue; }

                if (options.SearchWholeMap == false) {
                    //If it's not in a storage continue to the next
                    if (thing.IsInAnyStorage() == false) { continue; }
                    //Check for apparel and if it has some add it to the list
                    CorpseApparelHandler(thing, ref results, options.SearchPawns);
                    //add the thing to the list
                    results.Add(thing);
                    
                } else {
                    //Check for apparel and if it has some add it to the list
                    CorpseApparelHandler(thing, ref results, options.SearchPawns);
                    //add the thing to the list
                    results.Add(thing);
                }
                
            }

            //Handled all the searching all the pawns inventorys
            if (options.SearchPawns == true){
                //Get all the pawn. AllPawnsSpawned is the only list i could find that didn't
                //return null
                List<Pawn> pawns = Find.CurrentMap.mapPawns.AllPawnsSpawned as List<Pawn>;
                    for (int i = 0; i < pawns.Count; i++) {
                    //Check if pawn is not null and if not an animal and is apart of the colony as a colonist or a prisoner
                    if (pawns[i] != null 
                        && ((pawns[i].def.race.Animal == true || pawns[i].IsColonist) || pawns[i].IsPrisonerOfColony) == true) {

                        ThingOwner things;
                        //Add all the thing from the pawns inventory
                        if (pawns[i].inventory != null){
                            things = pawns[i].inventory.GetDirectlyHeldThings();
                            for (int j = 0; j < things.Count; j++) {
                                results.Add(things[j]);
                            }
                        }

                        //Add all the thing the pawn has equiped
                        if (pawns[i].equipment != null){
                            things = pawns[i].equipment.GetDirectlyHeldThings();
                            for (int j = 0; j < things.Count; j++){
                                results.Add(things[j]);
                            }
                        }
                        //Add all the thing the pawn is wearing Apperal
                        if (pawns[i].apparel != null){
                            things = pawns[i].apparel.GetDirectlyHeldThings();
                            for (int j = 0; j < things.Count; j++){
                                results.Add(things[j]);
                            }
                        }
                    }
                }
                
            }

            return results;
        }

        private static void CorpseApparelHandler(Thing thing, ref List<Thing> things, bool searchPawns) {
            if (searchPawns == false || thing.def.IsWithinCategory(ThingCategoryDefOf.Corpses) == false){
                return;
            }

            Corpse corpse = thing as Corpse;
            //if thing is not a corpse or an animal or an mechanoid then it dosen't wear apparel so skip it
            if (corpse == null || corpse.InnerPawn.def.race.Animal == true || corpse.InnerPawn.def.race.IsMechanoid == true) return;
            
            //Add pawns apparel to list
            ThingOwner pawnApparel = corpse.InnerPawn.apparel.GetDirectlyHeldThings();
            for (int i = 0; i < pawnApparel.Count; i++){
                things.Add(pawnApparel[i]);
            }    

        }

    }
}
