
using System;
using System.Collections.Generic;
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
            string resutl = "Implementar";
            return resutl;
        }


        public String Consulta2(ArbolGeneral<DatoDistancia> arbol)
        {
            string result = "Implementar";

            return result;
        }



        public String Consulta3(ArbolGeneral<DatoDistancia> arbol)
        {
            string result = "Implementar";

            return result;
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