/*** 20.12 20:22
 * 
 * Программа основанная на игре "Города"
 * Вычисляет саммую длинную партию, которую теоретически можно сыграть.
 * Идеи:
 * Предусмотреть случай, когда последние символы города - не буквы? (Алкино-2)
 * ! Что можем сделать с последними?
 */

using System.Collections;
using System.Collections.Generic;

namespace Games_Cities
{
    public class Game
    {
        static string cities_list_path = "C:\\Users\\Ermol\\source\\repos\\Games_Cities\\Cities_Ru.txt";
        static List<string> cities = new List<string>();
        static List<string> cities_copy = new List<string>();
        static List<string> longest_part_game = new List<string>(); // буферный список для сохранения текущей партии
        static float percent_max;
        static int default_list_size;
        static Dictionary<char,int> alphabet_weight = new Dictionary <char,int>(); // словарь, содержащий все буквы, на которые начинаются города и их веса

        // Главная функция
        static void Main(string[] args)
        {
            int i = 0;
            
            Console.WriteLine("Ну что, поиграем?");
            fill_cities_array(cities_list_path);//заполняем список городами из txt
            Console.WriteLine("Общее количество городов в игре: " + cities.Count);
            calculation_the_weight_of_the_alphabet();
            while (i!=100) //Никакого рандома, кроме выбора первой жертвы нет.
            {
                determine_the_longest_batch(); // непосредственно процесс
                cities = new List<string>(); // обнуляем список с которым работали
                cities.AddRange(cities_copy); // и заполняем его снова всеми городами из txt 
                i = i + 1;
            }
            Console.WriteLine($"Максимальная длинна партии: {longest_part_game.Count()} \nПроцент использования словаря: {percent_max:0.00}");
            //Console.WriteLine(string.Join("\t", longest_part_game));
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
            if (buffer_of_matches.Count > longest_part_game.Count)
            {
                longest_part_game = new List<string>();
                longest_part_game.AddRange(buffer_of_matches);
                percent_max = (float) buffer_of_matches.Count / cities_copy.Count*100;
            } 
            return buffer_of_matches.Count;
        }

        // Возвращает город из списка, на основании последней буквы аргумента
        static string? find_next_city(string city)
        {
            char last_sym = last_symbol(city);
            string? n_city = Find_val(last_sym);    //string? n_city = cities.Find(n_city => n_city.StartsWith(last_sym));     // !!!!! подряд перебирает все элементы списка и ищет следующий город. Нужно оптимизировать...
            if (n_city != null) //Если город найден
            {
                cities.Remove(n_city); // Удаляем из списка
                return n_city;  // Вернем город
            }
            else return null;
        }

        // ищет все города из списка на букву "*" и выбирает из всех рандомный
        // совершает одно действие - отдает город из списка
        static string? Find_val(char last_sym)
        {
            List<string> cities_starting_with_a_letter = new List<string>(); // в этот список кладет все города на букву "*". Почему бы не сохранить его и не использовать в будущем?
            foreach (string city in cities)
            {
                if (Char.Parse(city.Substring(0, 1)) == last_sym)
                {
                    cities_starting_with_a_letter.Add(city); // формируем список городов, начинающихся на "*"
                }
            }
            if (cities_starting_with_a_letter.Count == 1) return cities_starting_with_a_letter[0];
            if (cities_starting_with_a_letter.Count > 0) // если в массиве что-то есть
            {
                foreach (var letter in alphabet_weight) // логика в том, чтобы последняя буква возвращаемого слова оканчивалась на самую распространенную в данный момент.
                {
                    foreach (string city in cities)
                    {
                        if (Char.Parse(city.Substring(city.Length - 1, 1)) == letter.Key)
                        {
                            alphabet_weight[letter.Key] = alphabet_weight[letter.Key] - 1;
                            return city;
                        }
                    }
                }
            }
            else return null;

            return null; // компилятор без неё ругается, но мне видится что сюда никогда не зайдет.
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

        // определяет вес каждой буквы-начала городов из списка и заполняет соответствуюший словарь
        static void calculation_the_weight_of_the_alphabet()
        {
            foreach(string city in cities)
            {
                int default_weight = 1;
                char first_symbol = Char.Parse(city.Substring(0, 1)); //Определяем первую букву
                if (alphabet_weight.ContainsKey(first_symbol) == false) // Если её ещё нет в словаре - добавляем и присваиваем вес default_weight
                {
                    alphabet_weight.Add(first_symbol, default_weight);
                }
                else // А если есть - увеличиваем вес на default_weight
                {
                    alphabet_weight[first_symbol]= alphabet_weight[first_symbol]+default_weight;
                }
            }
            alphabet_weight = alphabet_weight.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        }

        // Читает файл с городами и записывает все их в список
        static void fill_cities_array(string filepath)
        {
            using (StreamReader reader = new StreamReader(filepath)) //убрал async - программа не успевала посчитать, а уже выводила результат. КОроче тут ни к чему.
            {
                string? line; // переменная может принимать значение null
                while ((line = reader.ReadLine()) != null)
                {
                    cities.Add(line.ToLower());
                }
            }
            cities_copy.AddRange(cities); // Сразу делаем копию исходного списка
            default_list_size = cities.Count; // и записываем в переменную его размер
        }

    }
}



//foreach (string city in cities)
//{
//    Console.WriteLine(city);
//}