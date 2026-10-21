using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Nodo
{
    public Nodo(string id, int x, int y, bool esCaminable)
    {
        _id = id;
        _x = x;
        _y = y;
        _esCaminable = esCaminable;
        _parent = null;
    }
    
    // Identificador
    private string _id;
    public string ID => _id; // esta es de solo lectura.
    
    // como es para una cuadrícula, yo le quiero poner su coordenada X y Y
    private int _x;
    public int X => _x;
    private int _y;
    public int Y => _y;

    private bool _esCaminable;
    public bool EsCaminable => _esCaminable;

    public void HacerCaminable()
    {
        _esCaminable = true;
    }

    // Propiedades del NodoDelÁrbol
    private Nodo _parent;

    public void AsignarParent(Nodo parent)
    {
        _parent = parent;
    }
    public Nodo Parent => _parent;
}

public class TileGrid : MonoBehaviour
{
    [Header("Configuración de cuadricula")]
    public int ancho = 5;
    public int alto = 5;
    public float probabilidadDeNoCaminable = 0.2f;
    public bool permitirDiagonales = false;
    
    [Header("Configuración del pathfinding")]
    [Min(0)]
    public int origenX = 0;
    [Min(0)]
    public int origenY = 0;
    
    [Min(0)]
    public int destinoX = 2;
    [Min(0)]
    public int destinoY = 2;
    
    
    public Nodo[,] Grid;

    // hace que en el editor salga un slider para este valor, el cual únicamente se va a poder mover del 0.1f al 1.0f.
    // OJO: esto no impide que el valor de dicha variable se pueda cambiar a fuera de ese rango mediante código.
    [Header("Control de Gizmos")]
    [Range(0.1f, 1.0f)]
    public float tamanioPorCuadroDelGizmo = 0.8f;
    
    public Color colorCaminable = Color.white;
    public Color colorNoCaminable = Color.black;
    public Color colorOrigen = Color.darkCyan;
    public Color colorDestino = Color.gray5;
    public Color colorLineaParent = Color.red;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // checamos que las coordenadas del origen y destino no se salgan de la cuadrícula
        if(origenX >= ancho || origenY >= alto ||
        destinoX >= ancho || destinoY >= alto)
        {
            Debug.LogError("Alguno de las coordenadas de Origen o destino está fuera del rango de la cuadrícula," +
                           "el programa no inicializará la cuadrícula");
            return;
        }
        
        // For bidimensional
        // en el primer for vamos a crear las filas, pero todavía no están rellenas
        // en el segundo for vamos a crear cada uno de los Nodos de la fila actual para que estén rellenas.
        Grid = new Nodo[alto, ancho];
        for (int y = 0; y < alto; y++)
        {
            // Grid[y] = new Node[ancho];
            for (int x = 0; x < ancho; x++)
            {
                float rand = Random.Range(0.0f, 1.0f);
                
                // // If clásico que es el más didáctico pero el más extenso
                // bool esCaminable = true;
                //
                // if (rand < probabilidadDeNoCaminable)
                // {
                //     esCaminable = false;
                // }
                
                // Estructura de un If de una sola línea. Creo que se le llama Operador Ternario.
                // (condición a checar) + ? + (lo que pasa si sí se cumplió la condición) + : + (lo que pasa si no se cumplió);
                bool esCaminable = rand < probabilidadDeNoCaminable ? false : true; // este de acá es para lo que sea.
                
                // bool esCaminableDirecto = rand < probabilidadDeNoCaminable; // exclusivo para booleanos
                
                Grid[y, x] = new Nodo($"{x}_{y}", x, y, esCaminable);
                // por que "[y, x]" y no "[x, y]"?
            }
        }

        Nodo origen = Grid[origenY, origenX];
        origen.AsignarParent(origen); // recuerden, el nodo raíz siempre es su propio parent.
        origen.HacerCaminable(); // lo hacemos caminable para siempre poder ejecutar al menos un paso del pathfinding
        
        Nodo destino = Grid[destinoY, destinoX];
        destino.HacerCaminable(); // lo hacemos caminable para siempre poder entrar al nodo destino si hubo un camino.
        
        if (DepthFirstSearchRecursiva(origen, destino))
        {
            Debug.Log($"Sí hubo camino de: {origen.ID} a {destino.ID}");
        }
        else
        {
            Debug.Log($"No hubo camino de: {origen.ID} a {destino.ID}");
        }

    }

    bool DepthFirstSearchRecursiva(Nodo origen, Nodo destino)
    {
        // Condición de parada
        if (origen.ID == destino.ID)
        {
            Debug.Log($"ya llegamos a la meta");
            return true; // ya llegamos al destino.
        }

        for (int i = 0; i < 8; i++)
        {
            // si no se permiten diagonales, solo checar 0, 2, 4, y 6, que son arriba, derecha, abajo e izquierda, respectivamente.
            if (!permitirDiagonales && i % 2 == 1)
            {
                continue; 
            }

            if (ChecarVecino(origen, destino, i))
            {
                return true;
            }        
        }

        // Nodo vecinoArribaDerecha = Grid[origen.Y - 1, origen.X + 1]; // diagonal hacia arriba y hacia la derecha

        Debug.Log($"el nodo {origen.ID} NO fue parte del camino");
        return false;
    }

    bool EstaDentroDeLaCuadricula(int x, int y)
    {
        if (x >= ancho || x < 0 ||
            y >= alto || y < 0)
            return false;
        Debug.Log($"{x}, {y} está dentro de la cuadricula");
        return true;
    }

    Nodo ObtenerVecino(Nodo actual, int direccion)
    {
        int xOffset = 0;
        int yOffset = 0;
        switch (direccion)
        {
            case 0: // arriba
                yOffset = -1;
                break;
            case 1: // arriba+derecha
                yOffset = -1;
                xOffset = 1;
                break;
            case 2: // derecha
                xOffset = 1;
                break;
            case 3: // derecha abajo
                xOffset = 1;
                yOffset = 1;
                break;
            case 4: // abajo
                yOffset = 1;
                break;
            case 5: // abajo izquierda
                xOffset = -1;
                yOffset = 1;
                break;
            case 6: // izquierda
                xOffset = -1;
                break;
            case 7: // izquierda arriba
                xOffset = -1;
                yOffset = -1;
                break;
        }
        
        if (EstaDentroDeLaCuadricula(actual.X + xOffset, actual.Y + yOffset))
            return Grid[actual.Y + yOffset, actual.X + xOffset];
        
        return null;
    }

    bool IntentarRecursion(Nodo origen, Nodo destino, Nodo vecino)
    {
        if (vecino.Parent == null && vecino.EsCaminable)
        {
            // se asigna el nodo actual (origen) como parent de su vecino
            vecino.AsignarParent(origen);

            // sí podemos mandar a llamar la recursión a partir de él
            if (DepthFirstSearchRecursiva(vecino, destino))
            {
                Debug.Log($"el nodo {origen.ID} fue parte del camino");
                return true;
            }
        }

        return false;
    }

    bool ChecarVecino(Nodo origen, Nodo destino, int direccion)
    {
        Nodo vecino = ObtenerVecino(origen, direccion);
        if (vecino != null) // Checamos que no nos salgamos de la matriz por la izquierda.
        {
            if (IntentarRecursion(origen, destino, vecino))
            {
                return true;
            } // y si no, pues nada
        }

        return false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            // Dibujamos la cuadrícula, y lo hacemos cuadrito por cuadrito
            for (int y = 0; y < alto; y++)
            {
                // Grid[y] = new Node[ancho];
                for (int x = 0; x < ancho; x++)
                {
                    Gizmos.color = Grid[y, x].EsCaminable ? colorCaminable : colorNoCaminable;

                    Gizmos.DrawCube(new Vector3(x, -y, 0.0f), Vector3.one * tamanioPorCuadroDelGizmo);
                    
                    // si tiene asignado un parent del pathfinding
                    Nodo parent = Grid[y, x].Parent;
                    if (parent != null)
                    {
                        Gizmos.color = colorLineaParent;
                        // entonces dibujamos una línea que los conecte
                        Gizmos.DrawLine(new Vector3(x, -y, 0.0f), new Vector3(parent.X, -parent.Y, 0.0f));
                    }
                }
            }

            Gizmos.color = colorOrigen;
            Gizmos.DrawCube(new Vector3(origenX, -origenY, 0.0f), Vector3.one * tamanioPorCuadroDelGizmo);
            
            Gizmos.color = colorDestino;
            Gizmos.DrawCube(new Vector3(destinoX, -destinoY, 0.0f), Vector3.one * tamanioPorCuadroDelGizmo);
        }
    }
}
