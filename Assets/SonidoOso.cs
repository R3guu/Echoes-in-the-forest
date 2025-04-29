using UnityEngine;

public class BearAudioAttenuation : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private AudioSource bearAudioSource;
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private float muteDistance = 20f;

    private void Update()
    {
        if (player == null || bearAudioSource == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= muteDistance)
        {
            bearAudioSource.volume = 0f;
        }
        else if (distance >= maxDistance)
        {
            bearAudioSource.volume = 1f;
        }
        else
        {
            // Volume decreases linearly from maxDistance to muteDistance
            float t = (distance - muteDistance) / (maxDistance - muteDistance);
            bearAudioSource.volume = Mathf.Clamp01(t);
        }
    }
}
