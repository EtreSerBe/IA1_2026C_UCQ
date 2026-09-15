using System;
using System.Collections.Generic;
using UnityEngine;

public class DeteccionPorProximidadPorCollider : MonoBehaviour
{
    // int, float, bool, void
    
    // El programa "Analizador Léxico" es el que analiza el texto como caracteres para encontrar tokens.
    // Token es una cadena de caracteres que tiene un significado para el lenguaje de programación
    // El programa "Analizador Sintáctico" analiza la Sintáxis
    // Analiza cuáles secuencias de tokens son válidas.
    // int int // Por sintáxis, un token "int" no puede ir seguido de otro token "int"
    // int float 
    // Forma de Backus-Naur: A -> B | * | [A-Z]*[a-z]
    // MyClass -> "MyClass" + nombreDeVariable |
    // "MyClass" + nombreDeFunción+(+parametros+)

    // Si el objeto a detectar está a una distancia mayor que este rango, entonces no lo detecto.
    public float rangoDeDeteccion = 5.0f;

    private SphereCollider _colliderDeDeteccion;
    
    public GameObject gameObjectEjemplo;

    public bool imprimirMensajesDeDebug = true;

    [SerializeField]
    private List<GameObject> objetosConocidos = new List<GameObject>();
    // public GameObject[] knownObjects = new GameObject[5]; // por flexibilidad y facilidad de uso,
    // es mejor el List (array de tamaño dinámico) que un array normalito
    
    // List de Unity es en realidad un Array dinámico (de tamaño dinámico), que por ejemplo, en C++ se le conoce como "vector"
    
    // Para usar una Lista ligada en unity (Linked List), la clase que tienen que usar es: LinkedList<>
    
    
    // ¿En código, cómo podemos obtener o usar un componente de nuestro gameObject?
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        objetosConocidos.Add(gameObject);
        foreach (var objeto in objetosConocidos)
        {
            Debug.Log(objeto.name + " está en los objetos conocidos");
        }
        
        // Función GetComponent obtiene un componente del tipo T del actor/GameObject que la mande a llamar.
        // Específicamente, me está dando el componente SphereCollider del dueño de este script.
        _colliderDeDeteccion = GetComponent<SphereCollider>();
        if (_colliderDeDeteccion != null)
        {
            // sobreescribimos el radio que tuviera el collider en el editor por el que está en este script.
            _colliderDeDeteccion.radius = rangoDeDeteccion;
            
            if(imprimirMensajesDeDebug)
                Debug.Log("radio del collider propio es: " + _colliderDeDeteccion.radius, gameObject);
        }
        

        
        // Acá, me va a dar el SphereCollider que le pertenece a gameObjectEjemplo, porque él es quien está
        // mandando a llamar GetComponent.
        
        // 1) Cómo se previene el MissingComponentException?
        // Hay que checar que sí esté el componente.
        SphereCollider tempSphereCollider = gameObjectEjemplo.GetComponent<SphereCollider>();
        if (tempSphereCollider != null)
        {
            // entonces, es sí se encontró el componente SphereCollider que le pertenece a gameObjectEjemplo
            if(imprimirMensajesDeDebug)
                Debug.Log("radio del collider del gameObjectEjemplo es: " + 
                      tempSphereCollider.radius, gameObject);
        }
        else
        {
            Debug.LogWarning("el gameObject: " + gameObjectEjemplo.name + " no trae un SphereCollider", gameObject);
        }

        // 2) Cómo se soluciona?
        // Asegurándose de que el gameObject que está mandando a llamar GetComponent tiene un componente de dicho tipo asignado.

        
        // Unassigned Reference Exception es el título/categoría del error
        // Unassigned es "sin asignar"
        // Reference es Referencia
        // Exception es una "excepción" que se refiere a un tipo de error en programación, que se reconoce como un error
        // pero se manejó de manera adecuada para que no tronara todo el programa.
        // En conjunto, quiere decir que ocurrió un error en el que una referencia no se asignó Y se trató de usar
        // ejemplos de uso: (acceder a sus variables, llamar sus funciones, etc.)
        // Entonces, si el error es que no está asignado algo, la solución sería que sí esté asignado lo que está marcando error.
        // Usen el "find usages" de su IDE para encontrar todos los usos de dicha variable y encontrar dónde está sucediendo el problema.
        
        // Missing Component Exception
        // Missing es Falta o Faltante
        // Component es Componente
        // Exception es que hubo un error
        // En conjunto dice: "Error, falta un componente"
        // Entonces, si el error es que falta un componente, la solución sería que sí se tenga dicho componente.
        // Si el gameObject que mandó a llamar la función NO tiene al menos un componente de dicho tipo, entonces sale este error.
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Función heredada de Monobehaviour, que se manda a llamar automáticamente cuando el gameObject que es dueño
    // de este script, detecta una colisión con su collider. Y, requiere que al menos uno de los dos que colisionó
    // tenga un componente RigidBody. 
    // other toda la información que tiene quer ver con la colisión, pero en la mayoría de los casos, lo que
    // nos importa es saber con quién (o qué tipo de objeto) estoy haciendo colisión
    private void OnCollisionEnter(Collision other) 
    {
        // OnCollision son "con quién choqué y Cómo?"
        
        // Cosas que aprendimos con solo tener este debug.log:
        // 1) al menos uno de los dos objetos que queremos que colisionen debe de tener un rigidBody
        // 2) el RigidBody NO debe de ser Kinemático
        // 3) Para que se detecte bien bien bien la colisión, el que se debe de mover es el que trae el componente rigidBody.
        if(imprimirMensajesDeDebug)
            Debug.Log("On collision enter contra: " + other.gameObject.name, gameObject);
    }
    
    // se manda a llamar cuando deja de estar en contacto con un objeto con el que ya estaba en contacto.
    private void OnCollisionExit(Collision other) 
    {
        if(imprimirMensajesDeDebug)
            Debug.Log("On collision exit contra: " + other.gameObject.name, gameObject);
    }
    
    
    // La clase que viene vamos a hablar sobre OnTriggerEnter/Exit, y por qué no usamos los Stay de estas funciones.
    // OnTrigger son "con quién choqué?"
    
    // Nuestro sentido de visión no tendría por qué empujar a los objetos que quiere detectar, entonces usamos 
    // OnTrigger Enter/Exit para ello, ya que los trigger no tienen respuesta física/simulada.
    private void OnTriggerEnter(Collider other)
    {
        if(imprimirMensajesDeDebug)
            Debug.Log("On Trigger enter contra: " + other.gameObject.name, gameObject);
    }
    
    private void OnTriggerExit(Collider other)
    {
        if(imprimirMensajesDeDebug)
            Debug.Log("On Trigger Exit contra: " + other.gameObject.name, gameObject);
    }
}
