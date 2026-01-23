using UnityEngine;

public class MainMenuEffects : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip hoveringClip;
    [SerializeField] private AudioClip selectedClip;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.ignoreListenerPause = true;
    }
    public void PlayHoveringClip() 
    {
        audioSource.PlayOneShot(hoveringClip);
    }

    public void PlaySelectedClip() 
    {
        audioSource.PlayOneShot(selectedClip);
    }
}
