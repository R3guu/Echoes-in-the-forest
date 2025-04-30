using UnityEngine;

public class BearAudioAttenuation : MonoBehaviour
{
    [SerializeField] private Transform player;              // Referencia al jugador
    [SerializeField] private AudioSource bearAudioSource;   // Fuente de audio del oso
    [SerializeField] private float maxAudibleDistance = 20f; // Distancia máxima a la que se oye el oso

    private void Update()
    {
        if (player == null || bearAudioSource == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Volumen proporcional inverso: 1 cuando está cerca, 0 cuando está lejos
        float volume = Mathf.Clamp01(1f - (distance / maxAudibleDistance));
        bearAudioSource.volume = volume;
    }
}
