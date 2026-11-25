using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EEstadosEspadachin
{
    Idle,
    MeleeBasico,
    MeleeArea,
    MeleeDash,
    MeleeUltimate
    
}

public class EnemigoEspadachin : EnemigoBaseFSM
{

    // este se asigna manualmente en el editor para no tener problemas con el get component
    // que vaya a tomar un collider distinto
    [SerializeField] private Collider colliderEspada;


    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
       base.Start();
       
       if (colliderEspada == null)
       {
           Debug.LogError("Error, no hay collider de la espada asignado", gameObject);
       }
       
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        // 
       
        // actualizamos el parámetro de Velocidad que tiene el animator de este agente, con la velocidad que trae su NavMeshAgent
        animator.SetFloat("Velocidad", _navMeshAgent.velocity.magnitude);
    }

    public void CambiarEstado(EEstadosEspadachin estado)
    {
        switch (estado)
        {
            case EEstadosEspadachin.Idle:
                _fsm.CambiarEstado<EspadachinEstadoIdle>();
                break;
            case EEstadosEspadachin.MeleeBasico:
                _fsm.CambiarEstado<EspadachinEstadoAtaqueMeleeBasico>();
                break;
            case EEstadosEspadachin.MeleeArea:
                _fsm.CambiarEstado<EspadachinEstadoAtaqueMeleeArea>();
                break;
            case EEstadosEspadachin.MeleeDash:
                _fsm.CambiarEstado<EspadachinEstadoAtaqueMeleeDash>();
                break;
            case EEstadosEspadachin.MeleeUltimate:
                _fsm.CambiarEstado<EspadachinEstadoAtaqueMeleeUltimate>();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(estado), estado, null);
        }
    }

    // No quiero que la espada tenga físicas tal cual, solo saber si intersectó o no.
    protected void OnTriggerEnter(Collider other)
    {

    }
    
    public void OnChildTriggerEnter(Collider other)
    {
        // El hitboxTrigger ya filtró y solo quedan las capas deseadas.
        // Al player le harían daño
        
        // al objeto destructible le harían daño.
        
        // a una pared podrían hacer que rebote el golpe y se ponga una animación adecuada, tipo darksouls

    }

    // Los eventos de animación NO permiten parámetros booleanos...
    public void ActivarColliderEspada(int activa)
    {
        Debug.Log("La espada está activa?: " + activa);
        colliderEspada.enabled = activa != 0; // cualquier cosa a excepción de 0, significa activa.
    }
    
}
