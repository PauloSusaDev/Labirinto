using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(MeshCollider))]
public class openBau : MonoBehaviour
{
    [Header("Velocidade")]
    [SerializeField] private float OpenSpeed = 5f;

    [Header("Prefab")]
    public Transform Tampa;
    Quaternion TampaFechada;
    Quaternion TampaAberta;
    private bool abrindo = false;
    private bool fechando = false;

    void Start()
    {
        TampaFechada = Tampa.localRotation;
        TampaAberta = TampaFechada * Quaternion.Euler(-90f, 0f, 0f);
    }
    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            abrindo = true;
            fechando = false;
        }
        if (abrindo)
        {
            Open();
        }
        if (Keyboard.current.eKey.wasPressedThisFrame && Tampa.localRotation == TampaAberta)
        {
           fechando = true;
           abrindo = false;
        }
        if (fechando)
        {
            Close();
        }

    }
    void Open()
    {
         Tampa.localRotation = Quaternion.Slerp(Tampa.localRotation, TampaAberta, Time.deltaTime * OpenSpeed);
    }
    void Close()
    {
        Tampa.localRotation = Quaternion.Slerp(Tampa.localRotation, TampaFechada, Time.deltaTime * OpenSpeed);
    }
}
