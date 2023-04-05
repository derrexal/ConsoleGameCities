
/* 
 * Программа основанная на игре "Города"
 * Вычисляет саммую длинную партию, которую теоретически можно сыграть.
 */




using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace Games_Cities
{
    public class Game
    {
        static List<string> cities = new List<string>();
        static List<string> longest_part_game = new List<string>(); // буферный список для сохранения текущей партии
        static float percent_max; // Процент использования словаря
        static int default_list_size; // Общее количество городов в игре
        static Dictionary<char,int> alphabet_weight = new Dictionary <char,int>(); // словарь, содержащий все буквы, на которые начинаются города и их веса

        // Главная функция
        static void Main(string[] args)
        {
            Console.WriteLine("Ну что, поиграем?");
            fill_cities_array(cities_list_path);//заполняем список городами из txt
            Console.WriteLine("Общее количество городов в игре: " + default_list_size);
            calculation_the_weight_of_the_alphabet(); // определяет вес каждой первой буквы города
            
            determine_the_longest_batch(); // непосредственно процесс
            
            Console.WriteLine($"Максимальная длинна партии: {longest_part_game.Count()} \nПроцент использования словаря: {percent_max:0.00}");
            //Console.WriteLine(string.Join("\t", longest_part_game)); // выводит всю цепочку
        }

        // определение самой длинной партии
        static void determine_the_longest_batch()
        {
            string? last_city;
            string first_city; //первый город в цепочке
            char the_rarest_letter_in_the_dictionary = alphabet_weight.Keys.Last();
            first_city = Find_val(the_rarest_letter_in_the_dictionary); // параметр - самая нераспрастраненная буква.
            alphabet_weight[the_rarest_letter_in_the_dictionary] = alphabet_weight[the_rarest_letter_in_the_dictionary] - 1; // у этой буквы уменьшаем вес. 
            if (alphabet_weight[the_rarest_letter_in_the_dictionary] == 0) alphabet_weight.Remove(the_rarest_letter_in_the_dictionary);
            last_city = find_next_city(first_city);   //Сюда нужно всего лишь передать город, который начинается на самую нераспрастраненную первую букву

            while (last_city != null) // пока цепочка не закончится. Здесь происходит непосредственно процесс
            {
                last_city = find_next_city(last_city);
                if (last_city != null)
                {
                    longest_part_game.Add(last_city);
                }
            }
            percent_max = (float)longest_part_game.Count / default_list_size * 100; 
        }

        // Возвращает город из списка, на основании последней буквы аргумента
        static string? find_next_city(string city)
        {
            char last_sym = last_symbol(city);
            string? n_city = Find_val(last_sym);    //string? n_city = cities.Find(n_city => n_city.StartsWith(last_sym));     // !!!!! подряд перебирает все элементы списка и ищет следующий город. Нужно оптимизировать...
            if (n_city != null) //Если город найден
            {
                cities.Remove(n_city); // Удаляем из списка
                return n_city;  // И возвращаем его
            }
            else return null; // если город не найден - вернем нул - игра окончена
        }

        // определяет последний символ города
        static char last_symbol(string city, int step = 1)
        {
            char last_sym = Char.Parse(city.Substring(city.Length - step, 1));
            if (last_sym == 'ь' || last_sym == 'ъ' || last_sym == 'ы')// Если последний символ города - ъьы - возвращаем предпоследний и т.д.
            {
                step++;
                return last_symbol(city, step);
            }
            //if (last_sym == 'й') last_sym = 'и'; // а как сделать так, чтобы в первый раз он брал "йошкар-ола" а затем менял на "И"
            return last_sym;
        }


        // Возвращает город, который заканчивается на самую распространенную букву
        static string? Find_val(char last_sym)
        {
            List<string> cities_starting_with_a_specific_letter = create_list_of_cities_with_a_specific_letter(last_sym); // в этот список кладет все города на букву "*". Почему бы не сохранить его и не использовать в будущем?
            
            if (cities_starting_with_a_specific_letter.Count > 0) // если в массиве что-то есть
            {
                if (cities_starting_with_a_specific_letter.Count == 1) return cities_starting_with_a_specific_letter[0]; // если по итогу в списке оказался 1 город
                foreach (var letter in alphabet_weight.Keys) // спмсок самых распространенных первых букв городов по убыванию
                {
                    foreach (string city in cities_starting_with_a_specific_letter)
                    {
                        if (Char.Parse(city.Substring(city.Length - 1, 1)) == letter) // Если последняя буква слова оканчивается на самую распространенную в данный момент.
                        {
                            alphabet_weight[letter] = alphabet_weight[letter] - 1; // у этой буквы уменьшаем вес. 
                            if (alphabet_weight[letter] == 0) alphabet_weight.Remove(letter); // удаляет элемент из словаря, если он равен 0
                            return city;
                        }
                    }
                }
            }
            else return null;

            return null; // компилятор без неё ругается, но мне видится что сюда никогда не зайдет.
        }
        // ищет и возвращает списком все города на букву *
        static List<string> create_list_of_cities_with_a_specific_letter(char last_sym)
        {
            List<string> cities_starting_with_a_specific_letter = new List<string>(); // в этот список кладет все города на букву "*". Почему бы не сохранить его и не использовать в будущем?
            foreach (string city in cities) // перебираем все города
            {
                if (Char.Parse(city.Substring(0, 1)) == last_sym) //если первая буква города = *
                {
                    cities_starting_with_a_specific_letter.Add(city); // формируем список городов, начинающихся на "*"
                }
            }
            return cities_starting_with_a_specific_letter;
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
            default_list_size = cities.Count; // Количество всех городов
        }
    }
}
