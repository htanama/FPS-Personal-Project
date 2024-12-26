using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class gunStats : ScriptableObject
{
    public GameObject model;
    
    public int shootDamage;
    public float shootRate;
    public int reloadRate;
    public int shootDist;
    public int ammoCur, ammoMax;

    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    public float shootSoundVol;
    public AudioClip reloadSound;
    public float reloadSoundVol;
    public Sprite weaponSprite;
}
