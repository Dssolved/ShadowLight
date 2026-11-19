using UnityEngine;

[CreateAssetMenu(menuName = "ShadowLight/Sound Bank")]
public class SoundBank : ScriptableObject
{
    public AudioClip[] footsteps;
    public AudioClip[] heroHit;
    public AudioClip[] heroDeath;
    public AudioClip[] brazierFire;
    public AudioClip[] patrollerWindUp;
    public AudioClip[] shooterAim;
    public AudioClip[] shooterShot;
    public AudioClip[] chargerWindUp;
    public AudioClip[] uiClick;
    public AudioClip[] win;
    public AudioClip[] lose;
}
