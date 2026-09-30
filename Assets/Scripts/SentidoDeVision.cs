using System;
using System.Collections.Generic;
using UnityEngine;

// Forza a que en el gameObject que tiene este script se tenga un SphereCollider component
[RequireComponent(typeof(SphereCollider))]
public class SentidoDeVision : MonoBehaviour
{
    [SerializeField]
    private float radioDeColliderDeDeteccion = 5.0f;

    public float GetRadioDeColliderDeDeteccion()
    {
        return radioDeColliderDeDeteccion;
    }

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
    
    
    private List<GameObject> obstaculosConocidos = new List<GameObject>();
    public List<GameObject> GetObstaculosConocidos()
    {
        return obstaculosConocidos;
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

        if (other.gameObject.layer == LayerMask.NameToLayer("Obstaculo"))
        {
            obstaculosConocidos.Add(other.gameObject);
            return; // como ya sé que fue un Obstáculo, entonces no puede ser ni un Jugador ni un Enemigo.
        }


        // Primero que nada, vamos a checar que sea de una Layer que sí nos importa.
        // Si no es un jugador Y no es un enemigo, entonces no nos interesa y nos salimos de esta función
        if (other.gameObject.layer != LayerMask.NameToLayer("Jugador") &&
            other.gameObject.layer != LayerMask.NameToLayer("Enemigo"))
        {
            return; // salimos de la función, no hay nada que hacer.
        }

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
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstaculo"))
        {
            obstaculosConocidos.Remove(other.gameObject);
            return; // como ya sé que fue un Obstáculo, entonces no puede ser ni un Jugador ni un Enemigo.
        }
        
        // Primero que nada, vamos a checar que sea de una Layer que sí nos importa.
        // Si no es un jugador Y no es un enemigo, entonces no nos interesa y nos salimos de esta función
        if (other.gameObject.layer != LayerMask.NameToLayer("Jugador") &&
            other.gameObject.layer != LayerMask.NameToLayer("Enemigo"))
        {
            return; // salimos de la función, no hay nada que hacer.
        }
        
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
