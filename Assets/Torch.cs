using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour
{
    private void Start()
    {
        GameAudioManager.PlaySound(GameAudioManager.Sound.TorchSound, transform.position, true);

        Debug.Log("Playing Torch sound");
    }
}
