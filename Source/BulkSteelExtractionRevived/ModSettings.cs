using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace BulkSteelExtractionRevived
{
    [StaticConstructorOnStartup]
    internal static class BSER_Initializer {
        static BSER_Initializer() {
            LongEventHandler.QueueLongEvent(Setup, "LibraryStartup", false, null);
        }

        public static void Setup() {
            ThingDef thingDef;
            thingDef = ThingDef.Named("Steel");
            var RecipeDefs = DefDatabase<RecipeDef>.AllDefsListForReading;
            var foundExtractMetalFromSlag = false;
            var foundBulkExtractMetalFromSlag = false;
            var foundExtractMetalFromDebris = false;
            var foundBulkExtractMetalFromDebris = false;

            for (var i = 0; (!foundExtractMetalFromSlag || !foundBulkExtractMetalFromSlag || !foundExtractMetalFromDebris || !foundBulkExtractMetalFromDebris) && i < RecipeDefs.Count; i++) {
                // extract steel from slag
                if (!foundExtractMetalFromSlag && RecipeDefs[i].defName == "ExtractMetalFromSlag") {
                    RecipeDefs[i].workAmount = Controller.Settings.workAmountFromSlag;
                    RecipeDefs[i].products.Clear();
                    RecipeDefs[i].products.Add(new ThingDefCountClass(thingDef, Controller.Settings.steelAmountFromSlag));
                    if (Controller.Settings.componentFromSlag.Equals(true)) {
                        RecipeDefs[i].products.Add(new ThingDefCountClass(ThingDefOf.ComponentIndustrial, 1));
                    }
                    foundExtractMetalFromSlag = true;
                } else if (!foundBulkExtractMetalFromSlag && RecipeDefs[i].defName == "BulkExtractMetalFromSlag")
                {
                    RecipeDefs[i].workAmount = (int)(Controller.Settings.workAmountFromSlag * 3 * 0.75);
                    RecipeDefs[i].products.Clear();
                    RecipeDefs[i].products.Add(new ThingDefCountClass(thingDef, Controller.Settings.steelAmountFromSlag * 3));
                    if (Controller.Settings.componentFromSlag.Equals(true)) {
                        RecipeDefs[i].products.Add(new ThingDefCountClass(ThingDefOf.ComponentIndustrial, 3));
                    }
                    foundBulkExtractMetalFromSlag = true;
                }

                // extract steel from debris
                else if (!foundExtractMetalFromDebris && RecipeDefs[i].defName == "ExtractMetalFromDebris")
                {
                    RecipeDefs[i].workAmount = Controller.Settings.workAmountFromDebris;
                    RecipeDefs[i].products.Clear();
                    RecipeDefs[i].products.Add(new ThingDefCountClass(thingDef, Controller.Settings.steelAmountFromDebris));
                    if (Controller.Settings.componentFromDebris.Equals(true)) {
                        RecipeDefs[i].products.Add(new ThingDefCountClass(ThingDefOf.ComponentIndustrial, 1));
                    }
                    foundExtractMetalFromDebris = true;
                } else if (!foundBulkExtractMetalFromDebris && RecipeDefs[i].defName == "BulkExtractMetalFromDebris")
                {
                    RecipeDefs[i].workAmount = (int)(Controller.Settings.workAmountFromDebris * 3 * 0.75);
                    RecipeDefs[i].products.Clear();
                    RecipeDefs[i].products.Add(new ThingDefCountClass(thingDef, Controller.Settings.steelAmountFromDebris * 3));
                    if (Controller.Settings.componentFromDebris.Equals(true)) {
                        RecipeDefs[i].products.Add(new ThingDefCountClass(ThingDefOf.ComponentIndustrial, 3));
                    }
                    foundBulkExtractMetalFromDebris = true;
                }
            }
        }
    }

    public class Controller : Mod {
        public static Settings Settings;
        public override string SettingsCategory() { return "BSER.Name".Translate(); }
        public override void DoSettingsWindowContents(Rect canvas) { Settings.DoWindowContents(canvas); }
        public Controller(ModContentPack content) : base(content) {
            Settings = GetSettings<Settings>();
        }
    }

    public class Settings : ModSettings {
        public int steelAmountFromSlag = 15;
        public int workAmountFromSlag = 400;
        public bool componentFromSlag = false;

        public int steelAmountFromDebris = 10;
        public int workAmountFromDebris = 400;
        public bool componentFromDebris = false;

        public void DoWindowContents(Rect canvas) {
            Listing_Standard list = new Listing_Standard();
            list.ColumnWidth = canvas.width;
            list.Begin(canvas);
            list.Gap();
            list.Label("BSER.Description".Translate());
            list.Gap();

            list.Label("BSER.SteelAmountFromSlag".Translate() + "  " + steelAmountFromSlag);
            steelAmountFromSlag = (int) list.Slider(steelAmountFromSlag, 1, 200);
            Text.Font = GameFont.Tiny;
            list.Label("          "+"BSER.SteelAmountTipFromSlag".Translate());
            Text.Font = GameFont.Small;
            list.Gap();

            list.Label("BSER.WorkAmountFromSlag".Translate() + "  " + Math.Round((decimal)workAmountFromSlag/60, MidpointRounding.AwayFromZero));
            workAmountFromSlag = (int) list.Slider(workAmountFromSlag, 60, 2000);
            Text.Font = GameFont.Tiny;
            list.Label("          "+"BSER.WorkAmountTipFromSlag".Translate());
            Text.Font = GameFont.Small;
            list.Gap();

            list.CheckboxLabeled( "BSER.ComponentFromSlag".Translate(), ref componentFromSlag);
            list.Gap();

            list.Label("BSER.SteelAmountFromDebris".Translate() + "  " + steelAmountFromDebris);
            steelAmountFromDebris = (int) list.Slider(steelAmountFromDebris, 1, 200);
            Text.Font = GameFont.Tiny;
            list.Label("          "+"BSER.SteelAmountTipFromDebris".Translate());
            Text.Font = GameFont.Small;
            list.Gap();

            list.Label("BSER.WorkAmountFromDebris".Translate() + "  " + Math.Round((decimal)workAmountFromDebris/60, MidpointRounding.AwayFromZero));
            workAmountFromDebris = (int) list.Slider(workAmountFromDebris, 60, 2000);
            Text.Font = GameFont.Tiny;
            list.Label("          "+"BSER.WorkAmountTipFromDebris".Translate());
            Text.Font = GameFont.Small;
            list.Gap();

            list.CheckboxLabeled( "BSER.ComponentFromDebris".Translate(), ref componentFromDebris);
            list.Gap();

            list.End();
        }
        public override void ExposeData() {
            base.ExposeData();
            Scribe_Values.Look(ref steelAmountFromSlag, "steelAmountFromSlag", 15);
            Scribe_Values.Look(ref workAmountFromSlag, "workAmountFromSlag", 400);
            Scribe_Values.Look(ref componentFromSlag, "componentFromSlag", false);

            Scribe_Values.Look(ref steelAmountFromDebris, "steelAmountFromDebris", 10);
            Scribe_Values.Look(ref workAmountFromDebris, "workAmountFromDebris", 400);
            Scribe_Values.Look(ref componentFromDebris, "componentFromDebris", false);
        }
    }
}
