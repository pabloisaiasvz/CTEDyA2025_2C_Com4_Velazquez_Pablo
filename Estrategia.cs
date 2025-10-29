
using System;
using System.Collections.Generic;
using System.Drawing;
using tp1;

namespace tpfinal
{

    public class Estrategia
    {
        private int CalcularDistancia(string str1, string str2)
        {
            // using the method
            String[] strlist1 = str1.ToLower().Split(' ');
            String[] strlist2 = str2.ToLower().Split(' ');
            int distance = 1000;
            foreach (String s1 in strlist1)
            {
                foreach (String s2 in strlist2)
                {
                    distance = Math.Min(distance, Utils.calculateLevenshteinDistance(s1, s2));
                }
            }

            return distance;
        }

        public String Consulta1(ArbolGeneral<DatoDistancia> arbol)
        {
            string resutl = "";
            if (arbol.esHoja()) // Si fuera hoja retorno su texto, el cual seria agregado a resutl
            {
                return arbol.getDatoRaiz().texto + "\n"; // La forma "\n" indica un salto de linea
            }
            else
            {
                foreach (ArbolGeneral<DatoDistancia> listaHijos in arbol.getHijos())
                {
                    resutl += Consulta1(listaHijos); // Se agrega a resutl los textos de los hijos
                }
            }
            return resutl;
        }


        public string Consulta2(ArbolGeneral<DatoDistancia> arbol)
        {
            List<string> caminos = new List<string>(); // Lista donde se almacenarán todos los caminos posibles
            ExploradorCaminos(arbol, "", caminos); // Llena la lista "caminos" con los caminos
            string resultado = "";
            foreach (string camino in caminos)
            { // Cada "camino" es el conjunto de recorridos hasta una hoja 
                resultado += camino + "\n"; // Agrega el camino al resultado y hace un salto de línea
            }
            return resultado; // Devuelve el resultado final
        }

        private void ExploradorCaminos(ArbolGeneral<DatoDistancia> arbol, string caminoActual, List<string> caminos)
        {
            caminoActual += " --> " + arbol.getDatoRaiz().ToString(); // Agrega el texto del nodo actual al camino actual
            if (arbol.esHoja()) // Si el nodo actual es una hoja, se ha alcanzado el final del camino
            {
                caminos.Add(caminoActual); // Agrega el camino actual a la lista de caminos
                return; // Termina y vuelve a la recursión anterior
            }
            foreach (ArbolGeneral<DatoDistancia> hijo in arbol.getHijos()) // Itera sobre los hijos del nodo actual
            {
                ExploradorCaminos(hijo, caminoActual, caminos); // Llama recursivamente al explorador para cada hijo
            }
        }



        public String Consulta3(ArbolGeneral<DatoDistancia> arbol)
        {
            // Creamos un diccionario donde la clave es el número de nivel (int) 
            // y el valor es una lista de strings con los textos de todos los nodos de ese nivel
            Dictionary<int, List<string>> nodosPorNivel = new Dictionary<int, List<string>>();

            // Llamamos al método que recorre el árbol y llena el diccionario
            // Empezamos desde el nivel 0 (la raíz del árbol)
            RecorrerPorNiveles(arbol, 0, nodosPorNivel);

            // Variable para almacenar el resultado final que vamos a retornar
            string resultado = "";

            // Recorremos cada nivel almacenado en el diccionario
            foreach (int nivel in nodosPorNivel.Keys)
            {
                // Para cada nivel, agregamos al resultado una línea con formato:
                // "Nivel X: nodo1, nodo2, nodo3"
                // string.Join une todos los elementos de la lista separándolos con ", "
                resultado += "Nivel " + nivel + ": " + string.Join(", ", nodosPorNivel[nivel]) + "\n";
            }

            return resultado;
        }

        private void RecorrerPorNiveles(ArbolGeneral<DatoDistancia> nodo, int nivelActual, Dictionary<int, List<string>> nodosPorNivel)
        {
            if (nodo == null) return;

            // Verificamos si ya existe una lista para este nivel en el diccionario, si no existe, la creamos
            if (!nodosPorNivel.ContainsKey(nivelActual))
            {
                nodosPorNivel[nivelActual] = new List<string>(); // Creamos una lista vacía para este nivel
            }

            // Agregamos el texto del nodo actual a la lista de su nivel correspondiente
            // Usamos ToString() para obtener la representación en texto del dato
            nodosPorNivel[nivelActual].Add(nodo.getDatoRaiz().ToString());

            // Recorremos todos los hijos del nodo actual
            foreach (var hijo in nodo.getHijos())
            {
                // Llamamos recursivamente al método para cada hijo
                // Oncrementamos el nivel en 1 porque los hijos están un nivel más abajo
                RecorrerPorNiveles(hijo, nivelActual + 1, nodosPorNivel);
            }
        }

        public void AgregarDato(ArbolGeneral<DatoDistancia> arbol, DatoDistancia dato)
        {
            if (arbol == null || dato == null)
            {
                return;
            }

            DatoDistancia datoArbol = arbol.getDatoRaiz();

            if (datoArbol == null)
            {
                return;
            }

            int distancia = CalcularDistancia(datoArbol.texto, dato.texto);

            if (distancia == 0)
            {
                return;
            }

            bool existeMismaDistancia = false;

            foreach (var hijo in arbol.getHijos())
            {
                DatoDistancia datoHijo = hijo.getDatoRaiz();
                int distanciaHijo = datoHijo.distancia;

                if (distancia == distanciaHijo)
                {
                    existeMismaDistancia = true;
                    AgregarDato(hijo, dato);
                    break;
                }
            }

            if (!existeMismaDistancia)
            {
                DatoDistancia nuevoDato = new DatoDistancia(distancia, dato.texto, dato.descripcion);
                ArbolGeneral<DatoDistancia> nuevoHijo = new ArbolGeneral<DatoDistancia>(nuevoDato);
                arbol.agregarHijo(nuevoHijo);
            }
        }

        public void Buscar(ArbolGeneral<DatoDistancia> arbol, string elementoABuscar, int umbral, List<DatoDistancia> collected)
        {
            if (arbol == null || elementoABuscar == null || collected == null)
            {
                return;
            }

            DatoDistancia datoArbol = arbol.getDatoRaiz();

            if (datoArbol == null)
            {
                return;
            }

            int distanciaArbolElemento = CalcularDistancia(datoArbol.texto, elementoABuscar);

            if (distanciaArbolElemento <= umbral)
            {
                collected.Add(datoArbol);
            }

            if (!arbol.esHoja())
            {
                foreach (var hijo in arbol.getHijos())
                {
                    DatoDistancia datoHijo = hijo.getDatoRaiz();
                    if (datoHijo != null)
                    {
                        int distanciaHijo = datoHijo.distancia;

                        int diferencia = Math.Abs(distanciaArbolElemento - distanciaHijo);

                        if (diferencia <= umbral)
                        {
                            Buscar(hijo, elementoABuscar, umbral, collected);
                        }
                    }
                }
            }
        }
    }
}