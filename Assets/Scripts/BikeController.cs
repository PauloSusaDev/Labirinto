using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class BikeController : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float aceleracao = 12f;
    [SerializeField] private float velocidadeMaxima = 15f;
    [SerializeField] private float desaceleracao = 4f;

    [Header("Descida e Subida")]
    [SerializeField] private float aceleracaoDescida = 8f;
    [SerializeField] private float perdaSubida = 5f;

    [Header("Direção")]
    [SerializeField] private float velocidadeCurva = 80f;

    [Header("Freio")]
    [SerializeField] private float forcaFreio = 15f;

    [Header("Rodas")]
    [SerializeField] private Transform rodaDianteira;
    [SerializeField] private Transform rodaTraseira;
    [SerializeField] private float raioRoda = 0.35f;

    [Header("Input")]
    [SerializeField] private InputActionReference movimento;
    [SerializeField] private InputActionReference freio;

    private Rigidbody rb;

    private float acelerador;
    private float direcao;
    private bool freando;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        movimento.action.Enable();
        freio.action.Enable();
    }

    private void OnDisable()
    {
        movimento.action.Disable();
        freio.action.Disable();
    }

    private void Update()
    {
        Vector2 input = movimento.action.ReadValue<Vector2>();

        acelerador = input.y;
        direcao = input.x;

        freando = freio.action.IsPressed();
    }

    private void FixedUpdate()
    {
        Mover();
        Inclinação();
        LimitarVelocidade();
        GirarRodas();
    }

    private void Mover()
    {
        // Velocidade atual na direção da bicicleta
        float velocidadeAtual = Vector3.Dot(rb.linearVelocity, transform.forward);

        // Aceleração normal
        if (acelerador > 0)
        {
            rb.AddForce(
                transform.forward * acelerador * aceleracao,
                ForceMode.Acceleration
            );
        }

        // Desaceleração quando solta o acelerador
        if (acelerador <= 0 && !freando)
        {
            Vector3 velocidadeHorizontal =
                Vector3.ProjectOnPlane(rb.linearVelocity, Vector3.up);

            Vector3 resistencia =
                -velocidadeHorizontal.normalized * desaceleracao;

            if (velocidadeHorizontal.magnitude > 0.1f)
            {
                rb.AddForce(resistencia, ForceMode.Acceleration);
            }
        }

        // Freio
        if (freando)
        {
            Vector3 velocidadeHorizontal =
                Vector3.ProjectOnPlane(rb.linearVelocity, Vector3.up);

            if (velocidadeHorizontal.magnitude > 0.1f)
            {
                rb.AddForce(
                    -velocidadeHorizontal.normalized * forcaFreio,
                    ForceMode.Acceleration
                );
            }
        }

        // Direção
        if (Mathf.Abs(velocidadeAtual) > 0.5f)
        {
            float curva = direcao * velocidadeCurva * Time.fixedDeltaTime;

            rb.MoveRotation(
                rb.rotation * Quaternion.Euler(0f, curva, 0f)
            );
        }
    }

    private void Inclinação()
    {
        // Detecta a inclinação do terreno
        float inclinacao = Vector3.Dot(
            transform.forward,
            Vector3.down
        );

        // DESCIDA
        if (inclinacao > 0.05f)
        {
            rb.AddForce(
                transform.forward *
                inclinacao *
                aceleracaoDescida,
                ForceMode.Acceleration
            );
        }

        // SUBIDA
        if (inclinacao < -0.05f)
        {
            rb.AddForce(
                transform.forward *
                inclinacao *
                perdaSubida,
                ForceMode.Acceleration
            );
        }
    }

    private void LimitarVelocidade()
    {
        Vector3 velocidade = rb.linearVelocity;

        Vector3 velocidadeHorizontal =
            Vector3.ProjectOnPlane(velocidade, Vector3.up);

        if (velocidadeHorizontal.magnitude > velocidadeMaxima)
        {
            Vector3 velocidadeLimitada =
                velocidadeHorizontal.normalized * velocidadeMaxima;

            rb.linearVelocity = new Vector3(
                velocidadeLimitada.x,
                velocidade.y,
                velocidadeLimitada.z
            );
        }
    }

    private void GirarRodas()
    {
        float velocidade =
            Vector3.Dot(rb.linearVelocity, transform.forward);

        float distanciaPercorrida =
            velocidade * Time.fixedDeltaTime;

        float graus =
            (distanciaPercorrida / (2f * Mathf.PI * raioRoda)) * 360f;

        if (rodaDianteira != null)
        {
            rodaDianteira.Rotate(
                graus,
                0f,
                0f,
                Space.Self
            );
        }

        if (rodaTraseira != null)
        {
            rodaTraseira.Rotate(
                graus,
                0f,
                0f,
                Space.Self
            );
        }
    }
}