using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class EnemigoBase : MonoBehaviour
{
    [SerializeField]
    private float maxSpeed = 5.0f;

    private Vector3 posicionInicial;
    private float tiempoTranscurrido = 0;

    private SentidoDeVision _sentidoDeVision;
    private GameObject _objetivo;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        posicionInicial = transform.position;

        _sentidoDeVision = GetComponent<SentidoDeVision>();
        if (_sentidoDeVision == null)
        {
            Debug.LogError("No hay componente SentidoDeVision asignado", gameObject);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        // Le pedimos al sentido de visión los objetos que conoce, y de esos nosotros vamos a perseguir a uno, si es que hay
        List<GameObject> objetosConocidos = _sentidoDeVision.GetObjetosConocidos();
        // si la lista no está vacía, es que hay al menos un conocido, y entonces yo lo puedo poner como mi objetivo.
        if (objetosConocidos.Count > 0)
        {
            _objetivo = objetosConocidos[0];
        }
            
        // punta menos cola desde mí hacia el objetivo
        Vector3 puntaMenosCola = _objetivo.transform.position - transform.position;
        
        // Para obtener la pura dirección de una flecha, usamos la Normalización
        Vector3 puntMenosColaNormalizado = puntaMenosCola.normalized;
        
        // Solo para demostrar que este y lo de abajo son prácticamente lo mismo.
        // tiempoTranscurrido += Time.deltaTime;
        // Vector3 posicionFinal = posicionInicial + puntMenosColaNormalizado * (tiempoTranscurrido * maxSpeed);
        // Debug.Log("La posicionInicial + la velocidad por todo el tiempo transcurrido es: " + posicionFinal); 
        
        transform.position += puntMenosColaNormalizado * (Time.deltaTime * maxSpeed);
        Debug.Log("El transform.position que va aumentando cada cuadro es: " + transform.position); 

    }
}
