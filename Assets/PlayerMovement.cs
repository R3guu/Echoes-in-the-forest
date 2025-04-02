
using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para reiniciar la escena

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f; // Velocidad normal
    public float sprintSpeed = 8f; // Velocidad al esprintar
    public float gravity = -9.81f; // Gravedad
    public float jumpHeight = 2f; // Altura del salto

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool muerto = false;

    public GameObject gameOverCanvas; // Asigna el Canvas GameOver en el Inspector

    void Start()
    {
        controller = GetComponent<CharacterController>();
        gameOverCanvas.SetActive(false); // Asegurarse de que el Canvas est� desactivado al iniciar
    }

    void Update()
    {
        if (muerto) return; // Si est� muerto, no puede moverse

        // Comprobar si est� tocando el suelo
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Movimiento con WASD
        float moveX = 0f;
        float moveZ = 0f;

        if (Input.GetKey(KeyCode.A)) moveX = -1f;
        if (Input.GetKey(KeyCode.D)) moveX = 1f;
        if (Input.GetKey(KeyCode.W)) moveZ = 1f;
        if (Input.GetKey(KeyCode.S)) moveZ = -1f;

        // Determinar velocidad (sprint o caminar)
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : speed;

        // Calcular el movimiento
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // Aplicar movimiento
        controller.Move(move.normalized * currentSpeed * Time.deltaTime);

        // Salto
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Aplicar gravedad
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        gameOverCanvas.SetActive(false); // Asegurar que GameOver est� oculto al iniciar
    }

    public void MatarJugador()
    {
        if (muerto) return; // Evitar que se ejecute varias veces

        muerto = true;
        gameOverCanvas.SetActive(true); // Activar pantalla de Game Over
        Debug.Log("�Has muerto! Game Over.");
        gameOverCanvas.SetActive(true); // Mostrar Game Over
        Time.timeScale = 0f; // Pausar el juego
    }
    public void ReiniciarPartida()
    {
        Time.timeScale = 1f; // Reactivar el tiempo
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Recargar la escena actual
    }
}