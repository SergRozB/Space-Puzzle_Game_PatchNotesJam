using UnityEngine;

public class MainMenuEffects : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip hoveringClip;
    [SerializeField] private AudioClip selectedClip;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void PlayHoveringClip() 
    {
        audioSource.PlayOneShot(hoveringClip);
        Debug.Log("Playing hovering clip");
    }

    public void PlaySelectedClip() 
    {
        audioSource.PlayOneShot(selectedClip);
        Debug.Log("Playing selected clip");
    }
}
