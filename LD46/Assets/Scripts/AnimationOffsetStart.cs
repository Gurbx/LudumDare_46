using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOffsetStart : MonoBehaviour
{
    [SerializeField] bool useTimeMultiplier;
    [SerializeField] float timeMultiplier;

    private void Awake()
    {
        GetComponent<Animator>().SetFloat("TimeOffset", Random.Range(0, 1f));
        if (useTimeMultiplier) GetComponent<Animator>().SetFloat("TimeMultiplier", (1 + Random.Range(-timeMultiplier, timeMultiplier)));
    }
}
