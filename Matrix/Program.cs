using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Matrix {
    class Program {

        // Создаем рандом.
        static Random rm = new Random();

        static void Main(string[] args) 
        {
            #region Задание.
            // Написать программу перемножения матриц используя многопоточность
            // Например матрицы размера 1000 х 2000 * 2000 х 3000
            #endregion

            // Создаем таймер.
            Stopwatch sw = new Stopwatch();

            // Стартуем таймер.
            sw.Start();

            // Создаем матрицы.
            int lineMatrix_1 = 2000;
            int columnsMatrix_1 = 3000;
            int[,] matrix_1 = new int[lineMatrix_1, columnsMatrix_1];

            int lineMatrix_2 = 1000;
            int columnsMatrix_2 = 2000;
            int[,] matrix_2 = new int[lineMatrix_2, columnsMatrix_2];

            // Переменная для результата умножения двух матриц.
            int[,] resultMatrix = new int[lineMatrix_1, columnsMatrix_2];

            // Заполняем матрицы.
            matrix_1 = FillMatrix(matrix_1);
            matrix_2 = FillMatrix(matrix_2);

            // Запсываем кол-во потоков.
            int numThreads = (int)Environment.ProcessorCount;

            // Создаем массив потоков.
            Thread[] threads = new Thread[numThreads];

            // Получаем целое для дальнейшего деления на потоки.
            int chunk = GCD(lineMatrix_1, numThreads);
            
            // Переменная для определения диапазона в каждом потоке.
            int zend = (lineMatrix_1 - chunk + (chunk / numThreads)) - (chunk / numThreads);

            // Переменная для записи начала в каждом потоке.
            int start = 0;

            for (int x = 0; x < numThreads; x++) 
            {
                // Создаем переменные с диапазоном.
                int chunkStart = start;
                int chunkEnd = (x + 1) * (chunk / numThreads) + zend;

                // Присваиваем значение для следующей итерации.
                start = chunkEnd;

                // Создаем поток.
                threads[x] = new Thread(() => 
                {
                    // Производим действие над матрицей.
                    for (int i = chunkStart; i < chunkEnd; i++) 
                    {
                        for (int z = 0; z < matrix_2.GetLength(1); z++) 
                        {
                            int multiplication = 0;

                            for (int k = 0; k < matrix_2.GetLength(0); k++) 
                            {
                                multiplication += matrix_1[i, k] * matrix_2[k, z];
                            }

                            resultMatrix[i, z] = multiplication;
                        }
                    }
                });
                
                // Стартуем поток.
                threads[x].Start();
            }

            // Ожидаем завершения.
            foreach (Thread thread in threads) {
                thread.Join();
            }

            // Останавливаем таймер.
            sw.Stop();

            // Переводим время в удобный формат.
            TimeSpan ts = sw.Elapsed;
            string time = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);

            // Выводим на консоль.
            Console.WriteLine($"Время: {time}");
            Console.ReadLine();
        }

        /// <summary>
        /// Метод для заполнения матриц.
        /// </summary>
        /// <param name="matrix">Пустой двумерный массив</param>
        /// <returns>Двумерный массив со значеними</returns>
        private static int[,] FillMatrix(int[,] matrix) {

            // Заполняем матрицы.
            for (int i = 0; i < matrix.GetLength(0); i++) {
                for (int j = 0; j < matrix.GetLength(1); j++) {
                    
                    matrix[i, j] = rm.Next(0, 5);
                }
            }

            return matrix;
        }

        /// <summary>
        /// Находит наибольший делитель.
        /// </summary>
        /// <param name="a">Размер</param>
        /// <param name="b">Кол-во потоков.</param>
        /// <returns>Наибольший делитель.</returns>
        static int GCD(int a, int b)
        {
            while (a != 0)
            {
                var temp = a;
                a = a % b;

                if (a != 0)
                    a = temp - 1;
                
                if (a == 0)
                {
                    a = temp;
                    break;
                }
            }
            return a;
        }
    }
}
