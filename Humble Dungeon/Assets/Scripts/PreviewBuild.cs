using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewBuild : MonoBehaviour
{
    public List<Collider> colliders = new List<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ground") && !other.CompareTag("Wall"))
        {
            colliders.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        colliders.Remove(other);
    }
}
