using System;
using UnityEngine;

public class HitboxTrigger : MonoBehaviour
{
    [SerializeField] private EnemigoEspadachin parent;
    
    // Jugador, obstáculos destructibles, proyectiles del player, etc.
    [SerializeField] private LayerMask capasALasQueDanianLosAtaques;
    
    private void OnTriggerEnter(Collider other)
    {
                
        if ((capasALasQueDanianLosAtaques.value & (1 << other.gameObject.layer)) != 0)
        {
            // Quiere decir que sí está en las capas que nos interesan.
            Debug.Log("Este hitbox trigger chocó contra: " + other.gameObject.name, gameObject);
            
            // que el parent resuelva qué hacer, pero ya vas a estar seguro que se filtraron solo las capas necesarias.
            parent.OnChildTriggerEnter(other);
        }
        
        // si quisieras que tu espada rebote al chocar contra una pared, aquí lo podrías hacer.
        
        
    }
}
