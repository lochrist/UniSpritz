using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniMini
{
    public class SoundbankAsset : ScriptableObject
    {
        public Soundbank bank;
    }

    [Serializable]
    public class Soundbank
    {
        public AudioClip[] clips;
        public static Dictionary<string, Soundbank> s_SoundbankFromFolder = new Dictionary<string, Soundbank>();
        public static Dictionary<string, Soundbank> s_SoundbankAssets = new Dictionary<string, Soundbank>();

        public static Soundbank LoadFromFolder(string folder)
        {
            if (s_SoundbankFromFolder.TryGetValue(folder, out var sb))
                return sb;
            var clips = Resources.LoadAll<AudioClip>(folder);
            if (clips == null)
                return null;
            sb = new Soundbank()
            {
                clips = clips
            };
            s_SoundbankFromFolder.Add(folder, sb);
            return sb;
        }

        public static Soundbank LoadFromFile(string filePath)
        {
            if (s_SoundbankAssets.TryGetValue(filePath, out var sb))
                return sb;
            var soundbankAsset = Resources.Load<SoundbankAsset>(filePath);
            if (soundbankAsset == null)
                return null;
            s_SoundbankFromFolder.Add(filePath, soundbankAsset.bank);
            return sb;
        }

        public AudioClip GetClip(AudioClipId id)
        {
            for (var i = 0; i < clips.Length; ++i)
                if (id.value == clips[i].name.GetHashCode())
                    return clips[i];
            return null;
        }

        public AudioClipId[] GetAudioClips()
        {
            return clips.Select(clip => new AudioClipId(clip.name)).ToArray();
        }
    }
}