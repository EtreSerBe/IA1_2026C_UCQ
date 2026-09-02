// Añada la Library/Biblioteca de UnityEngine que nos permite usar las funciones, clases, y demás, que están dentro de ella.
using UnityEngine;
// esto me permite, entre otras cosas, utilizar la clase Monobehaviour.

// Public declara que otros archivos pueden usar dicha clase.
// Declara la clase "MoverALaDerecha" que hereda de monobehaviour
// los dos puntos ':' significa que Hereda de la clase MonoBehaviour
// ¿qué significa heredar? La clase Padre le hereda TODAS sus propiedades y funcionalidades a la(s) clase(s) hija(s).
public class MoverALaDerecha : MonoBehaviour
{ // Lo que está dentro de estas llaves {} es la IMPLEMENTACIÓN de la clase "MoverALaDerecha"

    public int MyNumber;
    public bool ImprimirHolaMundo = true;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (ImprimirHolaMundo)
        {
            Debug.Log("Hola mundo", gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
