// Añada la Library/Biblioteca de UnityEngine que nos permite usar las funciones, clases, y demás, que están dentro de ella.
using UnityEngine;
using UnityEngine.InputSystem;

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
    // ¿Qué es un frame?
    // en películas, animación el estándar es 24 FPS
    // en juegos, suele 60 el mínimo; pero hay juegos más "cinemáticos" que nos basta con tener 30 FPS
    // Update significa Actualizar, que es que algo esté en su versión o estado más reciente
    void Update()
    {

        transform.position = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
        Debug.Log("Update, la posición ahora es: " + transform.position.x);
        
        // El maravilloso Time.deltaTime
        // Time es tiempo
        // delta que significa cambio
        // Entonces, delta Time significaría "cambio de tiempo".
        // Si eso es el cambio de tiempo, y nosotros lo llamamos cada cuadro,
        // entonces nos da el cambio de tiempo entre cuadro n y cuadro n+1
        transform.position = new Vector3(transform.position.x, transform.position.y + 1 * Time.deltaTime, transform.position.z);
        Debug.Log("Update, la posición en Y ahora es: " + transform.position.y);
        
        // Las cosas que no son con base al tiempo, es muy raro ponerlas dentro de un update.
        // Por ejemplo: spawnear proyectiles, iniciar habilidades 
        // O van en una función dentro del update y que después no se vuelve a ejecutar hasta que X cosa la vuelve a habilitar.
        //if(Input.GetKeyDown("left")
        
        // cosas que sí van: movimiento de personajes, de proyectiles, de animaciones*
    }
    
    // Otras funciones del Ciclo de vida del monobehaviour son:
    // Destroy() // lo contrario de Start
    // Awake() // va antes del Start de todos los scripts en escena
    // FixedUpdate() // como update pero para físicas.
}
