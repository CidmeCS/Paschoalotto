using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestePachoalotto
{
    internal class ListaDePokemons
    {
        static List<string> listPokemons = new List<string>() { "Bulbasaur", "Ivysaur", "Venusaur", "Charmander", "Charmeleon", "Charizard", "Squirtle", "Wartortle", "Blastoise", "Caterpie", "Metapod", "Butterfree", "Weedle", "Kakuna", "Beedrill", "Pidgey", "Pidgeotto", "Pidgeot", "Rattata", "Raticate" };

        internal static List<string> Listar()
        {
            var newsPokemons = listPokemons.ConvertAll(d => d.ToLower());
            newsPokemons.Sort();

            return newsPokemons;
        }
    }
}
