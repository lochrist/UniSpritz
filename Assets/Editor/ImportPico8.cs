using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using System;

public static class ImportPico8
{
    public static void Import(string path)
    {
        if (!File.Exists(path))
            throw new System.Exception($"Cannot import pico-8 file {path}");

        var allText = File.ReadAllText(path);
        var regex = new Regex("__lua__|__gfx__|__label__|__gff__|__map__|__sfx__|__music__");
        var cartDataSection = regex.Split(allText);

        var mapData = cartDataSection[5].Replace("\n", "");
        var gfxData = cartDataSection[2].Replace("\n", "");

        var mapArray = new int[64][];
        for (var i = 0; i < 64; ++i)
        {
            mapArray[i] = new int[128];
            Array.Fill(mapArray[i], 0);
        }


    }

    [MenuItem("Test/Import Pico8")]
    static void TestImportPico8()
    {
        Import("C:/Users/sebastien.phaneuf/AppData/Roaming/pico-8/carts/demos/jelpi.p8");
    }
}
