using UnityEngine;

public class OsoSonido : MonoBehaviour
{
    public GameObject jugador; // Asigna el objeto del jugador
    private AudioSource audioSource;
    public float distanciaMaxima = 20f; // Distancia máxima a la cual el sonido es audible
    public float volumenMaximo = 1f; // Volumen máximo
    public float volumenMinimo = 0.1f; // Volumen mínimo cuando está lejos

    void Start()
    {
        // Obtener el AudioSource del oso
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.Play(); // Inicia el sonido desde el principio
    }

    void Update()
    {
        if (jugador != null)
        {
            // Calcula la distancia entre el jugador y el oso
            float distancia = Vector3.Distance(transform.position, jugador.transform.position);

            // Ajusta el volumen en función de la distancia
            if (distancia < distanciaMaxima)
            {
                // Normaliza la distancia (volumen decreciente a medida que te alejas)
                float nuevoVolumen = Mathf.Lerp(volumenMaximo, volumenMinimo, distancia / distanciaMaxima);
                audioSource.volume = nuevoVolumen; // Establece el nuevo volumen
            }
            else
            {
                // Si está más allá de la distancia máxima, el sonido no se escucha
                audioSource.volume = volumenMinimo;
            }
        }
    }
}
