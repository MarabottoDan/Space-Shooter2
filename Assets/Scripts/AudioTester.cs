using UnityEngine;

public class AudioTester : MonoBehaviour
{
    [SerializeField] private AudioClip testClip;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Trying to play test clip...");

            AudioSource camAudio = Camera.main.GetComponent<AudioSource>();

            if (camAudio != null && testClip != null)
            {
                camAudio.PlayOneShot(testClip, 1.5f); // Full volume, 2D sound
                Debug.Log("Sound played through camera AudioSource");
            }
            else
            {
                Debug.LogWarning("Missing AudioSource on camera or clip not assigned");
            }
        }
    }
}
