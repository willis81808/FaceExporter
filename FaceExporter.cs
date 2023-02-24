using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BepInEx;
using HarmonyLib;

[BepInDependency("com.willis.rounds.unbound")]
[BepInPlugin(ModId, ModName, ModVersion)]
[BepInProcess("Rounds.exe")]
public class FaceExporter : BaseUnityPlugin
{
    private const string ModId = "com.willis.rounds.face.exporter";
    private const string ModName = "Face Exporter";
    private const string ModVersion = "1.0.0";
    private const string CompatabilityModName = "FaceExporter";

    private void Start()
    {
        var harmony = new Harmony(ModId);
        harmony.PatchAll();
    }
}
