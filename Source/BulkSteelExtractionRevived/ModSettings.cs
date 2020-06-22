﻿using RimWorld;
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
                    RecipeDefs[i].workAmount = (int)Controller.Settings.workAmountFromSlag;
                    RecipeDefs[i].products.Clear();
                    RecipeDefs[i].products.Add(new ThingDefCountClass(thingDef, (int)Controller.Settings.steelAmountFromSlag));
                    if (Controller.Settings.componentFromSlag.Equals(true)) {
                        RecipeDefs[i].products.Add(new ThingDefCountClass(ThingDefOf.ComponentIndustrial, 1));
                    }
                    foundExtractMetalFromSlag = true;
                } else if (!foundBulkExtractMetalFromSlag && RecipeDefs[i].defName == "BulkExtractMetalFromSlag")
                {
                    RecipeDefs[i].workAmount = (int)(Controller.Settings.workAmountFromSlag * 3 * 0.75);
                    RecipeDefs[i].products.Clear();
                    RecipeDefs[i].products.Add(new ThingDefCountClass(thingDef, (int)Controller.Settings.steelAmountFromSlag * 3));
                    if (Controller.Settings.componentFromSlag.Equals(true)) {
                        RecipeDefs[i].products.Add(new ThingDefCountClass(ThingDefOf.ComponentIndustrial, 3));
                    }
                    foundBulkExtractMetalFromSlag = true;
                }

                // extract steel from debris
                else if (!foundExtractMetalFromDebris && RecipeDefs[i].defName == "ExtractMetalFromDebris")
                {
                    RecipeDefs[i].workAmount = (int)Controller.Settings.workAmountFromDebris;
                    RecipeDefs[i].products.Clear();
                    RecipeDefs[i].products.Add(new ThingDefCountClass(thingDef, (int)Controller.Settings.steelAmountFromDebris));
                    if (Controller.Settings.componentFromDebris.Equals(true)) {
                        RecipeDefs[i].products.Add(new ThingDefCountClass(ThingDefOf.ComponentIndustrial, 1));
                    }
                    foundExtractMetalFromDebris = true;
                } else if (!foundBulkExtractMetalFromDebris && RecipeDefs[i].defName == "BulkExtractMetalFromDebris")
                {
                    RecipeDefs[i].workAmount = (int)(Controller.Settings.workAmountFromDebris * 3 * 0.75);
                    RecipeDefs[i].products.Clear();
                    RecipeDefs[i].products.Add(new ThingDefCountClass(thingDef, (int)(Controller.Settings.steelAmountFromDebris * 3)));
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
        public float steelAmountFromSlag = 20.0f;
        public float workAmountFromSlag = 1600.0f;
        public bool componentFromSlag = false;

        public float steelAmountFromDebris = 10.0f;
        public float workAmountFromDebris = 1600.0f;
        public bool componentFromDebris = false;

        public void DoWindowContents(Rect canvas) {
            Listing_Standard list = new Listing_Standard();
            list.ColumnWidth = canvas.width;
            list.Begin(canvas);
            list.Gap();

            list.Label("BSER.SteelAmountFromSlag".Translate()+"  "+(int)steelAmountFromSlag);
            steelAmountFromSlag = list.Slider(steelAmountFromSlag, 1f, 300.99f);
            Text.Font = GameFont.Tiny;
            list.Label("          "+"BSER.SteelAmountTipFromSlag".Translate());
            Text.Font = GameFont.Small;
            list.Gap();

            list.Label("BSER.WorkAmountFromSlag".Translate()+"  "+(int)((workAmountFromSlag/60)+.5));
            workAmountFromSlag = list.Slider(workAmountFromSlag, 575f, 2400f);
            Text.Font = GameFont.Tiny;
            list.Label("          "+"BSER.WorkAmountTipFromSlag".Translate());
            Text.Font = GameFont.Small;
            list.Gap();

            list.CheckboxLabeled( "BSER.ComponentFromSlag".Translate(), ref componentFromSlag);
            list.Gap();

            list.Label("BSER.SteelAmountFromDebris".Translate()+"  "+(int)steelAmountFromDebris);
            steelAmountFromDebris = list.Slider(steelAmountFromDebris, 1f, 300.99f);
            Text.Font = GameFont.Tiny;
            list.Label("          "+"BSER.SteelAmountTipFromDebris".Translate());
            Text.Font = GameFont.Small;
            list.Gap();

            list.Label("BSER.WorkAmountFromDebris".Translate()+"  "+(int)((workAmountFromDebris/60)+.5));
            workAmountFromDebris = list.Slider(workAmountFromDebris, 575f, 2400f);
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
            Scribe_Values.Look(ref steelAmountFromSlag, "steelAmountFromSlag", 20.0f);
            Scribe_Values.Look(ref workAmountFromSlag, "workAmountFromSlag", 1600.0f);
            Scribe_Values.Look(ref componentFromSlag, "componentFromSlag", false);

            Scribe_Values.Look(ref steelAmountFromDebris, "steelAmountFromDebris", 10.0f);
            Scribe_Values.Look(ref workAmountFromDebris, "workAmountFromDebris", 1600.0f);
            Scribe_Values.Look(ref componentFromDebris, "componentFromDebris", false);
        }
    }
}
