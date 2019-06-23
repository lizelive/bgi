using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEditor;

public class AudioMan : MonoBehaviour
{
    Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();


    struct SoundNotification
    {
        public AudioClip clip;
        Vector3 pos;
    }

    public Transform viewAs;
    public static AudioMan I;
    AudioMan()
    {
        I = this;
    }

    private void Start()
    {
        /*
        var sounds = AssetDatabase.FindAssets("t:AudioClip", new string[] { "audio" });
        print(sounds.Length);
        foreach (var path in sounds)
        {
            var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);

            print($"{path} has {clip}");
            clips.Add(path, clip);
        }
        */
    }

    public void Play(string clip, Vector3 pos)
    {
        var localPos = viewAs.InverseTransformPoint(pos);
        localPos = localPos.xz().normalized;


        var angle = Mathf.Atan2(localPos.y, localPos.x)/Mathf.PI*180;

        print($"/playaudio {clip} {angle}");

    }
}
