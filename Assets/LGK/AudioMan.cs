using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEditor;
using System.IO;

public class AudioMan : MonoBehaviour
{

	public AudioClip[] clips;


    Dictionary<string, AudioClip> nameToClip = new Dictionary<string, AudioClip>();


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
        foreach (var clip in clips)
        {
#if UNITY_EDITOR
			var path = AssetDatabase.GetAssetPath(clip);
            //print($"{path} has {clip}");
			var name = Path.GetFileNameWithoutExtension(path);

			var start = path.IndexOf("audio") + 6;
			var end = path.LastIndexOf('.');
			var dir = path.Substring(start, end - start);

			//print($"{path} has {clip} {name} {dir}");
			nameToClip.Add(dir, clip);
#endif
		}
    }

    public void Play(string clip, Vector3 pos)
    {
        var localPos = viewAs.InverseTransformPoint(pos);
        localPos = localPos.xz().normalized;


        var angle = Mathf.Atan2(localPos.y, localPos.x)/Mathf.PI*180;

        print($"/playaudio {clip} {angle}");

    }
}
