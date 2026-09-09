using Unity.Mathematics.Geometry;
using UnityEngine;

public class DeteccionPorProximidad : MonoBehaviour
{
    // cómo sabemos nosotros, como personas, si hay algo/alguien cerca?
    // encontrar el qué y dónde

    public GameObject ObjetoADetectar;

    // Si el objeto a detectar está a una distancia mayor que este rango, entonces no lo detecto.
    public float RangoDeDeteccion = 5.0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Qué tan lejos/cerca está de mí?
        // usamos, por ejemplo, metros, que son una medida de Distancia
        // Para obtener una distancia, necesitamos dos puntos en un espacio.
        
        // La resta de dos puntos en el espacio nos da la diferencia de Posición
        // El primero de esos puntos sería la posición del dueño de este script, que es "gameObject", así con 'g' en minúscula.
        Vector3 diferenciaDePosicion = gameObject.transform.position - ObjetoADetectar.transform.position;
        // [1, 0, 0] - [1, 1, 1] = [1-1, 0-1, 0-1] -> [0, -1, -1]
        
        // Ya que tenemos la diferencia de dichos puntos, podemos obtener la magnitud de dicho vector
        // La magnitud es simplemente la fórmula de la hipotenusa: Raíz cuadrada de la suma de los cuadrados.
        // La magnitud es UN solo número, que en este caso es la Distancia.
        float distanciaEntrePosiciones = Mathf.Sqrt(diferenciaDePosicion.x * diferenciaDePosicion.x +
                                         diferenciaDePosicion.y * diferenciaDePosicion.y +
                                         diferenciaDePosicion.z * diferenciaDePosicion.z);

        // Comparamos si la distanciaEntrePosiciones es mayor que el rango de detección:
        // Comparar, checar, verificar, etc. sugiere el uso de un "if()" en el código.
        if (distanciaEntrePosiciones > RangoDeDeteccion)
        {
            // si lo de arriba se cumple, entonces él está más allá de mi rango de detección, por lo tanto está fuera de él
            // y entonces no lo puedo detectar
            Debug.Log("El objeto a detectar está fuera de mi rango de detección, no lo puedo detectar", gameObject);
        }
        else
        {
            // si lo del if de arriba no se cumplió, entonces la distancia entre él y yo es menor que mi rango de detección
            // y por lo tanto sí lo puedo detectar
            Debug.Log("El objeto a detectar está dentro de mi rango de detección, ha sido detectado", gameObject);
        }
        
        // comandos muy útiles: Ctrl + k + c es comentar lo seleccionado
        // Ctrl + k + u descomentar lo seleccionado. Al parecer también sirve igual que el de + k + c
    }
}
