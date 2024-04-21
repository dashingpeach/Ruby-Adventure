using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedCollectible : MonoBehaviour
{
    public AudioClip collectedSound;

   void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            controller.changeSpeedUp();
            controller.PlaySound(collectedSound);
            Destroy(gameObject);
        }
    }
}
