using GameOfThrones.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GameOfThrones.Services
{
    /// <summary>
    /// This class is needed to reach the Song of Ice and Fire API
    /// </summary>
    public class GoTService
    {
        /// <summary>
        /// Root Uri for the web service
        /// </summary>
        private readonly Uri root = new Uri("https://www.anapioficeandfire.com");

        /// <summary>
        /// Gets the characters from the API
        /// </summary>
        /// <param name="pageNumber">Which page to request</param>
        /// <returns>The list of characters</returns>
        public async Task<List<Character>> GetCharactersAsync(int pageNumber)
        {
            List<Character> ret = new List<Character>();
            List<Character> temp = new List<Character>();
            temp = await GetAsync<List<Character>>(new Uri(root, $"api/characters?page={pageNumber}&pageSize=50"));
            foreach (var item in temp)
            {
                if (item.Name != "")
                {
                    ret.Add(item);
                }
            }

            return ret;
        }

        /// <summary>
        /// Gets the books from the API
        /// </summary>
        /// <returns>The list of books</returns>
        public async Task<List<Book>> GetBooksAsync()
        {
            var books = await GetAsync<List<Book>>(new Uri(root, "api/books?page=1&pageSize=50"));
            return books;
        }

        /// <summary>
        /// Gets the houses from the API
        /// </summary>
        /// <param name="pageNumber">Which page to request</param>
        /// <returns>The list of houses</returns>
        public async Task<List<House>> GetHousesAsync(int pageNumber)
        {
            var houses = await GetAsync<List<House>>(new Uri(root, $"api/houses?page={pageNumber}&pageSize=50"));
            return houses;
        }

        /// <summary>
        /// Gets a characters with a specified name
        /// </summary>
        /// <param name="name">The name to search for</param>
        /// <returns>The list of characters that have the given name</returns>
        public async Task<List<Character>> GetCharactersByNameAsync(string name)
        {
            var characters = await GetAsync<List<Character>>(new Uri(root, $"api/characters?name={name}"));
            return characters;
        }

        /// <summary>
        /// Gets a character with a given url
        /// </summary>
        /// <param name="url">The given url</param>
        /// <returns>The character with the specified url</returns>
        public async Task<Character> GetCharacterByUrlAsync(string url)
        {
            Character ret = await GetAsync<Character>(new Uri(url));
            return ret;
        }

        /// <summary>
        /// Gets a book with a given url
        /// </summary>
        /// <param name="url">The given url</param>
        /// <returns>The book with the specified url</returns>
        public async Task<Book> GetBookByUrlAsync(string url)
        {
            Book ret = await GetAsync<Book>(new Uri(url));
            return ret;
        }

        /// <summary>
        /// Gets a house with a given url
        /// </summary>
        /// <param name="url">The given url</param>
        /// <returns>The house with the specified url</returns>
        public async Task<House> GetHouseByUrlAsync(string url)
        {
            House ret = await GetAsync<House>(new Uri(url));
            return ret;
        }

        /// <summary>
        /// Generic function that makes the call to the Web API
        /// </summary>
        /// <typeparam name="T">The requested type</typeparam>
        /// <param name="uri">The given URI which is called by the service</param>
        /// <returns>THe requested type</returns>
        private async Task<T> GetAsync<T>(Uri uri)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(uri);
                var json = await response.Content.ReadAsStringAsync();
                T result = JsonConvert.DeserializeObject<T>(json);
                return result;
            }
        }
    }
}
