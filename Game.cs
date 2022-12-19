/*
 * 
 * Программа основанная на игре "Города"
 * Вычисляет саммую длинную партию, которую теоретически можно сыграть.
 * 
 */

using System.Collections.Generic;

namespace Games_Cities
{
    public class Game
    {
        static string cities_list_path = "C:\\Users\\Ermol\\source\\repos\\Games_Cities\\Cities_Ru.txt";
        static List<string> cities = new List<string>();
        static List<string> cities_copy = new List<string>();
        static List<string> longest_part_game = new List<string>(); // буферный список для сохранения длинной партии

        // Главная функция
        static void Main(string[] args)
        {
            int i = 0;
            
            Console.WriteLine("Ну что, поиграем?");
            fill_CitiesArray(cities_list_path);//заполняем список городами из txt
            Console.WriteLine("Общее количество городов в игре: " + cities.Count);
            while(i!=1000)
            {
                determine_the_longest_batch();
                cities = new List<string>();
                cities.AddRange(cities_copy);
                i = i + 1;
            }
            Console.WriteLine("Длинна max: " + longest_part_game.Count());
            Console.WriteLine(string.Join("\t", longest_part_game));

        }

        // Читает файл с городами и записывает все их в список
        static void fill_CitiesArray(string filepath)
        {
            using (StreamReader reader = new StreamReader(filepath)) //убрал async 
            {
                string? line; // переменная может принимать значение null
                while ((line = reader.ReadLine()) != null)
                {
                    cities.Add(line.ToLower());
                }
            }
            cities_copy.AddRange(cities);
        }
        // определение самой длинной партии
        static int determine_the_longest_batch()
        {
            Random random = new Random();
            int index = random.Next(0, cities.Count);
            string? last_city;
            List <string> buffer_of_matches = new List<string>();
            last_city = find_next_city(cities[index]);   //случайным образом выбираем первую жертву !!! 

            while (last_city != null) // пока цепочка не закончится. Здесь происходит непосредственно процесс
            {
                last_city = find_next_city(last_city);
                if (last_city != null)
                {
                    buffer_of_matches.Add(last_city);
                }
            }
            if (buffer_of_matches.Count > longest_part_game.Count) longest_part_game.AddRange(buffer_of_matches);
            return buffer_of_matches.Count;
        }

        // Возвращает город из списка, на основании последней буквы аргумента
        static string? find_next_city(string city)
        {
            //Console.WriteLine(city + "  ");
            char last_sym = last_symbol(city);
            string? n_city = find_val(last_sym);    //string? n_city = cities.Find(n_city => n_city.StartsWith(last_sym));     // !!!!! подряд перебирает все элементы списка и ищет следующий город. Нужно оптимизировать...
            if (n_city != null) //Если город найден
            {
                cities.Remove(n_city); // Удаляем из списка
                return n_city;  // Вернем город
            }
            else return null;
        }
        // ищет все города из списка на букву "*" и выбирает из всех рандомный
        static string? find_val(char last_sym)
        {
            Random random = new Random();
            List<string> cities_starting_with_a_letter = new List<string>();
            foreach (string city in cities)
            {
                if (Char.Parse(city.Substring(0, 1)) == last_sym)
                {
                    cities_starting_with_a_letter.Add(city);
                }
            }
            if (cities_starting_with_a_letter.Count > 0)
            {
                int index = random.Next(0, cities_starting_with_a_letter.Count);
                return cities_starting_with_a_letter[index];
            }
            else return null;
        }

        // определяет последний символ города
        static char last_symbol(string city,int step=1)
        {
            char last_sym = Char.Parse(city.Substring(city.Length - step, 1));
            if (last_sym == 'ь' || last_sym == 'ъ' || last_sym == 'ы')// Если последний символ города - ъьы - возвращаем предпоследний и т.д.
            {
                step++;
                return last_symbol(city, step);
            }
            if (last_sym == 'й') last_sym = 'и'; // а как сделать так, чтобы в первый раз он брал "йошкар-ола" а затем менял на "И"
            return last_sym;
        }   
    }
}



//foreach (string city in cities)
//{
//    Console.WriteLine(city);
//}