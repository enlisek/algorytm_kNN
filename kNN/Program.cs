using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleApp1
{
    class Dane
    {
        //static void shuffle(double[][] tab) //tasuje wiersze macierzy
        //{
        //    Random rnd = new Random();
        //    double[] tmp = new double[tab[0].Length];
        //    for (int i = tab.Length - 1; i > 0; i--)
        //    {

        //        int k = rnd.Next(0, tab.Length);
        //        tmp = tab[i];
        //        tab[i] = tab[k];
        //        tab[k] = tmp;

        //    }
        //}

        static double[][] pobierz(string addr) //pobiera lokalizacje pliku i zamienia dane na macierz 
        {
            string[] lines = File.ReadAllLines(addr);
            double[][] data = new double[lines.Length][];

            for (int i = 0; i < lines.Length; i++)
            {
                string[] tmp = lines[i].Split(','); //dane rozdzielone są przecinkiem. kropka oznacza część niecałkowitą
                data[i] = new double[tmp.Length + 2]; //o 2 wiekszy, bo dodajemy 3-el ciąg 0/1 opisujący gatunek
                for (int j = 0; j < tmp.Length - 1; j++) // wszystkie elementy oprócz nazwy 
                {
                    data[i][j] = Convert.ToDouble(tmp[j].Replace(".", ",")); //odczytuje liczby z przecinkiem, więc trzeba zmienić
                }
                // iris setosa: 001, iris versicolor: 010, Iris-virginica: 100
                if (tmp[4] == "Iris-setosa")
                {
                    data[i][4] = 0;
                    data[i][5] = 0;
                    data[i][6] = 1;

                }
                else if (tmp[4] == "Iris-versicolor")
                {
                    data[i][4] = 0;
                    data[i][5] = 1;
                    data[i][6] = 0;
                }
                else
                {
                    data[i][4] = 1;
                    data[i][5] = 0;
                    data[i][6] = 0;
                }
            }

            return data;
        }

        static double[] Min(double[][] data)
        {
            double[] min = new double[4]; //chcemy min/max 4 pierwszych kolumn
            for (int i = 0; i < min.Length; i++)
            {
                min[i] = data[0][i];
            }

            //szukamy min dla kazdej kolumny
            for (int i = 0; i < data.Length; i++) // i wiersz, j kolumna 
            {
                for (int j = 0; j < 4; j++)
                {
                    if (data[i][j] < min[j])
                        min[j] = data[i][j];
                }


            }

            return min;
        }
        static double[] Max(double[][] data)
        {
           
            double[] max = new double[4];
            for (int i = 0; i < max.Length; i++)
            {
                max[i] = data[0][i];
            }

            //szukamy max dla kazdej kolumny
            for (int i = 0; i < data.Length; i++) // i wiersz, j kolumna 
            {
                for (int j = 0; j < 4; j++)
                {
                    if (data[i][j] > max[j])
                        max[j] = data[i][j];
                }
            }

            return max;
        }
        static double[][] normalizuj(double[][] data)
        {
            //double[] min = new double[4]; //chcemy min/max 4 pierwszych kolumn
            //double[] max = new double[4];
            //for (int i = 0; i < min.Length; i++)
            //{
            //    min[i] = data[0][i];
            //    max[i] = data[0][i];
            //}

            ////szukamy max min dla kazdej kolumny
            //for (int i = 0; i < data.Length; i++) // i wiersz, j kolumna 
            //{
            //    for (int j = 0; j < 4; j++)
            //    {
            //        if (data[i][j] > max[j])
            //            max[j] = data[i][j];
            //        if (data[i][j] < min[j])
            //            min[j] = data[i][j];
            //    }
            //}
            double[] min = Min(data);
            double[] max = Max(data);

            
            //normalizacja
            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    data[i][j] = ((data[i][j] - min[j]) / (max[j] - min[j])) * (1 - 0) + 0;
                }
            }
            return data;
        }


        static double[] D(double[] unknown, double[][] data)
        {
            double[] d = new double[data.Length];
            
            for (int i = 0; i < data.Length; i++)
            {
                d[i] = Math.Sqrt(Math.Pow((unknown[0] - data[i][0]), 2) +
                                 Math.Pow((unknown[1] - data[i][1]), 2) +
                                 Math.Pow((unknown[2] - data[i][2]), 2) +
                                 Math.Pow((unknown[3] - data[i][3]), 2) );
            }

            return d;
        }          

        static void Knn(int k, double[] unknown, double[][] data)
        {
            int[] ind = new int[k]; // indeksy k elementów z najmniejszą odległością
            double[] d = D(unknown, data); //tablica odległości

            double max = d[0]; // szukam max, żeby znalezione najmniejsze indeksy zastąpić maxem
            for (int i = 0; i < d.Length; i++)
            {
                if (d[i]>max)
                {
                    max = d[i];
                }
            }

            int indMin;
            int[] sum = new int[] { 0, 0, 0 };
            for (int i = 0; i < k; i++) // szuka k sąsiadów
            {
                indMin = 0;
                for (int j = 0; j < d.Length; j++) //szuka indeks sąsiada z najmniejszą odegłością
                {
                    if (d[j]<d[indMin])
                    {
                        indMin = j;                        
                    }
                }
                ind[i] = indMin; //i-ty element tablicy to i-ty najbliższy sąsiad
                d[indMin] = max; //zastepuje najmniejszy el maxem

                for (int j = 0; j < 3; j++)
                {
                    sum[j] += Convert.ToInt32(data[indMin][j + 4]);
                }


            }

            if ((sum[0] > sum[1] && sum[0] > sum[2]) || (sum[0] == sum[1] && sum[0] > sum[2]) || (sum[0] == sum[2] && sum[0] > sum[1])) // nawet, gdy wartosci sumy 1 i 2 będą takie, jak sumy 0, to przyjmujemy iris virginica
            {
                Console.WriteLine("Iris-virginica");
            }
            else if ((sum[1] > sum[0] && sum[1] > sum[2]) || (sum[1] == sum[2] && sum[1] > sum[0]))
            {
                Console.WriteLine("iris-versicolor");
            }
            else
            {
                Console.WriteLine("iris-setosa");
            }


        }

        static void Main(string[] args)
        {


            string addr = @"C:\Users\Dell\OneDrive\Pulpit\Systemy sztucznej inteligencji\dane.txt";
            double[][] data = normalizuj(pobierz(addr));


            Console.WriteLine("Podaj k: ");
            int k = Convert.ToInt32(Console.ReadLine());

            double[] unknown = new double[] { 5.8, 4.0, 4, 0.2 };

            for (int i = 0; i < unknown.Length; i++) // normalizujemy nieznane wartości wg max/min danych z tablicy 
            {
                unknown[i] = ((unknown[i] - Min(pobierz(addr))[i]) / (Max(pobierz(addr))[i] - Min(pobierz(addr))[i])) * (1 - 0) + 0;
            }

            Knn(k,unknown,data);


            Console.ReadKey();
        }
    }
}
