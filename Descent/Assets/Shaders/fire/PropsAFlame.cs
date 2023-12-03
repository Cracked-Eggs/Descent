using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;

[ExecuteInEditMode]
public class PropsAFlame : MonoBehaviour
{
    public Transform emptyBall;
    public Material material;

    public float noisePower = .5f, noiseSize = 1.0f;

    VisualEffect effect;

    void Start()
    {
        effect = GetComponent<VisualEffect>();
    }

    void Update()
    {
        if (emptyBall && material)
        {
            effect.SetVector3("Ball Pos", emptyBall.position);
            effect.SetFloat("Ball Size", emptyBall.localScale.x);
            effect.SetFloat("Noise Size", noiseSize);
            effect.SetFloat("Noise Power", noisePower);

            material.SetVector("_Ball_Pos", emptyBall.position);
            material.SetFloat("_Ball_Size", emptyBall.localScale.x);
            material.SetFloat("_Noise_Size", noiseSize);
            material.SetFloat("_Noise_Power", noisePower);

            //transform.position = emptyBall.position;
        }
    }

}
