using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets _i;

    private void Start()
    {
        GameAudioManager.Initialize();
    }

    public static GameAssets i
    {
        get
        {
            if (_i == null) _i = Instantiate(Resources.Load<GameAssets>("GameAssets"));
            return _i;
        }
    }

    public SoundAudioClip[] soundAudioClipArray;

    [System.Serializable]
    public class SoundAudioClip
    {
        public GameAudioManager.Sound sound;
        public AudioClip audioClip;
        [Range(0f, 1f)]
        public float volume;
    }
}
