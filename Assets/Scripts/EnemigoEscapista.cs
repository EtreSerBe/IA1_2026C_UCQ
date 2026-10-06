using System.Collections.Generic;
using UnityEngine;

public class EnemigoEscapista : EnemigoBase
{
    [SerializeField] private float tiempoDeCansancio = 3.0f;
    [SerializeField] private float tiempoDeActividad = 6.0f;

    // 1 float = 4 Bytes
    // 2 flotantes * 1000 enemigo = 8000 bytes
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        // Le pedimos al sentido de visión los objetos que conoce, y de esos nosotros vamos a perseguir a uno, si es que hay
        List<GameObject> objetosConocidos = SentidoDeVision.GetObjetosConocidos();
        // si la lista no está vacía, es que hay al menos un conocido, y entonces yo lo puedo poner como mi objetivo.
        if (objetosConocidos.Count > 0)
        {
            Objetivo = objetosConocidos[0];
            
            // Documento original de los steering behaviors: https://www.red3d.com/cwr/papers/1999/gdc99steer.pdf

            // Hacer flee o descansar según el estado del EnemigoEscapista
            Vector3 steeringForce = Flee();
            
            // Ahora le va a sumar a esa steering force la fuerza del ObstacleAvoidance de todos los obstáculos que conozca
            List<GameObject> obstaculosConocidos = SentidoDeVision.GetObstaculosConocidos();
            foreach (var obstaculo in obstaculosConocidos)
            {
                // Multiplica esa fuerza del flee por qué tan lejos está este enemigo de dicho obstáculo en proporción
                // al tamaño del rango de detección.
                // Si la distancia entre el obstáculo y este enemigo es mayor o igual que el rango de detección,
                // entonces ese flee se multiplica por 0.
                // Esto es un clásico caso de Interpolación Lineal
                float porcentaje = Mathf.Lerp(0, 1.0f,
                    1.0f - Vector3.Distance(transform.position,
                        obstaculo.transform.position) / SentidoDeVision.GetRadioDeColliderDeDeteccion());
                steeringForce += Flee(obstaculo.transform.position) * porcentaje;
            }
            // NOTA: para obstacle avoidance, prefieran los obstáculos redondeados, como cápsulas, cilindros o esferas, 
            // para que se vea más suave y se atore menos.
            
            ActualizarAceleracionVelocidadYPosicion(steeringForce);
            
            // Solo para demostrar que este y lo de abajo son prácticamente lo mismo.
            // tiempoTranscurrido += Time.deltaTime;
            // Vector3 posicionFinal = posicionInicial + puntMenosColaNormalizado * (tiempoTranscurrido * maxSpeed);
            // Debug.Log("La posicionInicial + la velocidad por todo el tiempo transcurrido es: " + posicionFinal); 
        
            // transform.position += direccionDeseada * (Time.deltaTime * maximaVelocidad);
            // Debug.Log("El transform.position que va aumentando cada cuadro es: " + transform.position); 
        }
        else // Nos faltaba el otro lado de la moneda: cuando ya no ves a nadie, entonces ya no tienes objetivo!
        {
            Objetivo = null;
        }
        
        
        
    }
}
