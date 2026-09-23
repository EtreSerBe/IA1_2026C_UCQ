using System.Collections.Generic;
using UnityEngine;

// La clases hijas no tienen acceso a ninguna propiedad ni método marcado como Private.
// Solo a las que estén como Protected o Public.
public class EnemigoPesado : EnemigoBase
{
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /* EJEMPLO
        // función de enojado
        // le suben la velocidad y la aceleración o algo así
        // y después usamos tal cual el update de EnemigoBase
        // base.Update(); // NOTA: el Update del EnemigoBase tiene que ser public o protected para poder mandarlo a llamar aquí.
        */
        
        // Le pedimos al sentido de visión los objetos que conoce, y de esos nosotros vamos a perseguir a uno, si es que hay
        List<GameObject> objetosConocidos = SentidoDeVision.GetObjetosConocidos();
        // si la lista no está vacía, es que hay al menos un conocido, y entonces yo lo puedo poner como mi objetivo.
        if (objetosConocidos.Count > 0)
        {
            // El Enemigo Pesado no tiene que alternar entre Seek y Flee, entonces lo quito del update, para que nunca alterne entre ellos.
            // AlternarSeekYFlee();
            
            Objetivo = objetosConocidos[0];
            
            // Documento original de los steering behaviors: https://www.red3d.com/cwr/papers/1999/gdc99steer.pdf

            Vector3 steeringForce = Seek();

            ActualizarAceleracionVelocidadYPosicion(steeringForce);
            
            // Solo para demostrar que este y lo de abajo son prácticamente lo mismo.
            // tiempoTranscurrido += Time.deltaTime;
            // Vector3 posicionFinal = posicionInicial + puntMenosColaNormalizado * (tiempoTranscurrido * maxSpeed);
            // Debug.Log("La posicionInicial + la velocidad por todo el tiempo transcurrido es: " + posicionFinal); 
        
            // transform.position += direccionDeseada * (Time.deltaTime * maximaVelocidad);
            // Debug.Log("El transform.position que va aumentando cada cuadro es: " + transform.position); 
        }    }
}
