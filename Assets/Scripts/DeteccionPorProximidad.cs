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

        // Antes de normalizar un vector, generalmente se hace una línea como esta:
        // Vector3 direction;
        // checar que la magnitud (o magnitud cuadrada, cualquiera de las dos sirve) no sea muy chiquita.
        // if (direction.sqrMagnitude < 0.001f) 
        // EN RESUMEN: Para no dividir entre 0 (lo cual es error fatal) y
        // para que no haya valores exagerados que hagan que el programa se porte raro
        
        // EN EXPLICACIÓN A DETALLE:
        // Normalizar nos da una flecha con magnitud 1, qué tiene de malo normalizar una flecha chiquita (de magnitud chiquita)?
        // 1) problema matemático
        // Normalizar es: x/mag(Vec), y/mag(Vec), z/mag(Vec)
        // La única división que es inválida, que no se puede hacer jamás, es entre 0
        // Magnitud es: x*x + y*y + z*z
        // Si tú tienes un vector(0, 0, 0), pues su normalizado sería 0/mag(Vec), 0/mag(Vec), 0/mag(Vec)
        // La magnitud de dicho vector es 0*0 + 0*0 + 0*0
        // Entonces su normalizado sería (0/0, 0/0, 0/0), claramente causando una división sobre 0, lo cual es un error fatal.
        // 2) problema programático
        // Los números de punto flotante (float) no son precisos
        // Las variables tienen un tamaño, es decir una cantidad limitada de bits.
        // Un bool tiene 1 byte, que son 8 bits, entonces puede representar 2^8 valores, que son 256, por ejemplo del 0 al 255 (porque el 0 también cuenta)
        // Un short tiene 2 byts, que son 16 bits, y puede representar 2^16 valores, 65536, por ejemplo del 0 al 65535 (porque el 0 también cuenta)
        // Los int tienen 4 bytes, son 32 bits, entonces 2^32,
        // Los float también tienen 4 bytes, entonces también pueden representar 2^32 valores... por qué no son precisos?
        // ¿cuántos valores hay entre el 0 y 1?
        // pues está el 0, 0.1, 0.01, 0.001, 0.00001, .... 0.inf1
        // La cantidad de valores entre 0 y 1 es infinita. E infinito es más grande que 2^32.
        // Es menos preciso cuando tiene números enteros muy grandes,
        // y tampoco puede tener demasiados números decimales de precisión (por ejemplo, 8 decimales de precisión),
        // si fuera un número muuuuy chiquito, se "redondearía" a 0, y causaría una división entre 0 que es error fatal,
        // o casi 0, que al dividir algo, nos daría un valor extremadamente grande, por ejemplo 2/0.00000001, nos daría
        // 200,000,000 que está muy por encima del 8,388,608 (sacado de internet) y entonces perdemos precisión decimal.
        
        
        
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
