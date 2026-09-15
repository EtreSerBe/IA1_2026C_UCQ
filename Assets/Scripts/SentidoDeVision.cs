using System;
using System.Collections.Generic;
using UnityEngine;

public class SentidoDeVision : MonoBehaviour
{
    [SerializeField]
    private float radioDeColliderDeDeteccion = 5.0f;

    // Referencia al propio sphere collider que se usa para simular el rango de visión
    private SphereCollider _colliderDeDeteccion;
    
    // Cuáles objetos conoce este script a través del sentido de visión.
    // Cuando alguien entre a nuestro radio de visión lo conocemos,
    // Cuando alguien sale de nuestro radio de visión, lo dejamos de conocer.
    [SerializeField]
    private List<GameObject> objetosConocidos = new List<GameObject>();

    public List<GameObject> GetObjetosConocidos()
    {
        return objetosConocidos;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _colliderDeDeteccion = GetComponent<SphereCollider>();
        if (_colliderDeDeteccion == null)
        {
            Debug.LogError("No hay un sphere collider asignado a este gameObject", gameObject);
            return; // me salgo del Start porque algo salió mal, y si siguiera ejecutando el código de abajo me daría error
        }

        _colliderDeDeteccion.radius = radioDeColliderDeDeteccion;
        


        // Otras líneas...
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("On trigger Enter contra: " + other.gameObject.name, gameObject);

        // ejemplo
        // ya conozco a Cristian y a Samuel,
        // en other llega Rogelio
        
        // Cuando alguien entre a nuestro radio de visión lo conocemos,
        // Por pura seguridad, checamos que no haya repetidos.
        foreach (var conocido in objetosConocidos)
        {
            if (conocido == other.gameObject) // si esto se cumple, quiere decir que ya lo conozco
            {
                Debug.LogWarning("On trigger enter con el objeto " + other.gameObject.name + " pero ya lo conocía", gameObject);
                return;
            }
        }
        
        // si se acabó ese foreach, quiere decir que other no está en mi lista de conocidos
        // y entonces lo añadimos, para ahora sí conocerlo.
        objetosConocidos.Add(other.gameObject);
        
        foreach (var conocido in objetosConocidos)
        {
            Debug.Log("el objeto " + conocido + " está en los objetos conocidos", gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("On trigger exit contra: " + other.gameObject.name, gameObject);
        objetosConocidos.Remove(other.gameObject);

        if (objetosConocidos.Count == 0)
        {
            Debug.Log("ya no se conoce ningún objeto", gameObject);
        }
        else
        {
            foreach (var conocido in objetosConocidos)
            {
                Debug.Log("el objeto " + conocido + " está en los objetos conocidos", gameObject);
            }
        }
    }
}
