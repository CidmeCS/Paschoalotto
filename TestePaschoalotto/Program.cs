using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TestePachoalotto;

namespace TestePaschoalotto
{
    internal class Program
    {
        static int k = 1;
        static List<PokemonCustom> listaPokemonsCustom;
        static HttpClient client = ClientHTTP();
        static void Main(string[] args)
        {
            Console.WriteLine("Teste Paschoalotto - Desafio PokeApi");
            Console.WriteLine("01 - Consumir API de Pokemons (Seriada)");
            Console.WriteLine($"**************************************\n\n");
            var lista = ListaDePokemons.Listar();
            ConsumirApiPokemons01Seriada(lista);
            Console.ReadKey();

            Console.Clear();
            Console.WriteLine("Teste Paschoalotto - Desafio PokeApi");
            Console.WriteLine("02 - Consumir API de Pokemons (Paralela)");
            Console.WriteLine($"**************************************\n\n");
            ConsumirApiPokemons02Paralela(lista);
            Console.ReadKey();


            Console.Clear();
            Console.WriteLine("Teste Paschoalotto - Desafio PokeApi");
            Console.WriteLine("03 - [Extra] Download de imagem Pokemon)");
            Console.WriteLine($"**************************************\n\n");
            DownloadImagensAsync();
            Console.ReadKey();
        }

        static async void ConsumirApiPokemons01Seriada(List<string> listaDePokemons)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var pokemonItem in listaDePokemons)
            {
                HttpResponseMessage response = await client.GetAsync($"api/v2/pokemon/{pokemonItem}");
                if (response.IsSuccessStatusCode)
                {
                    Pokemon pokemon = await response.Content.ReadAsAsync<Pokemon>();

                    string tipos = string.Empty;
                    foreach (var item in pokemon.types.Select(p => p.type.name).ToList())
                    {
                        tipos += $"{item}, ";
                    }
                    string abilities = string.Empty;
                    foreach (var item in pokemon.abilities.Select(p => p.ability.name).ToList())
                    {
                        abilities += $"{item}, ";
                    }

                    sb.Append(
                        $"Nome: {pokemon.name}\n" +
                        $"Tipos: {tipos.Remove(tipos.Length - 2, 2)}\n" +
                        $"Abilidades: {abilities.Substring(0, abilities.Length - 2)}\n" +
                        $"Weight {pokemon.weight}\n" +
                        $"Hight {pokemon.height}\n" +
                        $"Sprites_FrontDefault {pokemon.sprites.front_default}" +
                        $"\n\n");
                }
                Console.WriteLine(k++ + $": {pokemonItem}");
            }
            string file = "01-ConsumirApiDePokemons_Seriada.txt";
            Console.WriteLine($"Lista de Pokemons sava em:\n{Environment.CurrentDirectory}\\{file}\n\n");
            File.WriteAllText(file, sb.ToString());
            Console.WriteLine("Pressione qualquer tecla para avancar");
        }

        private static HttpClient ClientHTTP()
        {
            HttpClient client = new HttpClient();
            string url = $"https://pokeapi.co/";
            client.BaseAddress = new System.Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        static void ConsumirApiPokemons02Paralela(List<string> lista)
        {
            listaPokemonsCustom = new List<PokemonCustom>();
            Parallel.ForEach(lista, pokemonL =>
            {
                k++;
                var pokemonT = ConsumirApiPokemons02Paralela02(pokemonL, client);

                var z = (new PokemonCustom
                {
                    Name = pokemonT.Result.name,
                    Tipos = pokemonT.Result.types.Select(p => p.type.name).ToList(),
                    Abilities = pokemonT.Result.abilities.Select(p => p.ability.name).ToList(),
                    Weight = (int)pokemonT.Result.weight,
                    Height = pokemonT.Result.height,
                    Spriets = pokemonT.Result.sprites.front_default
                });
                listaPokemonsCustom.Add(z);

            });

            string file = "02-ConsumirApiDePokemons_Paralela.json";
            string jsonmText = JsonConvert.SerializeObject(listaPokemonsCustom);
            File.WriteAllText(file, jsonmText);
            Console.WriteLine($"Lista de Pokemons sava em:\n{Environment.CurrentDirectory}\\{file}\n\n");
            Console.WriteLine("Pressione qualquer tecla para avancar");
        }

        public static async Task<Pokemon> ConsumirApiPokemons02Paralela02(string pokemon, HttpClient client)
        {
            HttpResponseMessage response = await client.GetAsync($"api/v2/pokemon/{pokemon}");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(k + $": {pokemon}");
                Pokemon p = await response.Content.ReadAsAsync<Pokemon>();
                return p;
            }
            return null;
        }

        public static async Task DownloadImagensAsync()
        {
            var FileName = $@"img\Ditto.png";
            var uri = new Uri("https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/132.png");
            Directory.CreateDirectory(@"img");
            using (var s = await client.GetStreamAsync(uri))
            {
                using (var fs = new FileStream(FileName, FileMode.Create))
                {
                    await s.CopyToAsync(fs);
                }
            }
            Console.WriteLine($"A imagem do Pokemon foi salva em:\n{Environment.CurrentDirectory}\\{FileName}\n\n");
            Console.WriteLine("Pressione qualquer tecla para finalizar");
        }
    }
}

