using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BallBounceSound : MonoBehaviour
{
    [SerializeField] float minImpactVelocity = 1.5f;
    [SerializeField] float maxVolume = 1f;

    private AudioSource audioSource;
    private Rigidbody rb;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Ignore tiny contacts
        float impactStrength = collision.relativeVelocity.magnitude;

        if (impactStrength < minImpactVelocity)
            return;

        // Volume based on bounce strength
        float volume = Mathf.Clamp01(impactStrength / 10f) * maxVolume;

        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(audioSource.clip, volume );
    }
}