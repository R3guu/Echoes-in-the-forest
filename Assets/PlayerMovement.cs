
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
    public GameObject cameraScript; // Referencia al script de la cámara/fotografía

    void Start()
    {
        controller = GetComponent<CharacterController>();
        gameOverCanvas.SetActive(false); // Asegurar que el Canvas esté desactivado al iniciar
        Cursor.lockState = CursorLockMode.Locked; // Ocultar el cursor al inicio
        Cursor.visible = false; // Asegurar que el cursor no se vea
    }

    void Update()
    {
        if (muerto) return; // Si está muerto, no puede moverse

        // Comprobar si está tocando el suelo
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
    }

    public void MatarJugador()
    {
        if (muerto) return; // Evitar que se ejecute varias veces

        muerto = true;
        gameOverCanvas.SetActive(true); // Activar pantalla de Game Over
        Debug.Log("¡Has muerto! Game Over.");

        // Pausar el tiempo
        Time.timeScale = 0f;

        // Mostrar el cursor para que el jugador pueda hacer clic
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Desactivar el script de fotografía (si tienes uno)
        if (cameraScript != null)
        {
            cameraScript.SetActive(false);
        }
    }

    public void ReiniciarPartida()
    {
        Time.timeScale = 1f; // Reactivar el tiempo
        Cursor.lockState = CursorLockMode.Locked; // Bloquear el cursor nuevamente
        Cursor.visible = false; // Ocultar el cursor

        muerto = false; // Restablecer el estado del jugador

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Recargar la escena
    }

}


