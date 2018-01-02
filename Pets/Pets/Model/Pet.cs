using Newtonsoft.Json;
using Pets.Helpers;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pets.Model
{
    public class Pet : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("breed", NullValueHandling = NullValueHandling.Ignore)]
        public string Breed { get; set; }

        [JsonProperty("weight", NullValueHandling = NullValueHandling.Ignore)]
        public double Weight { get; set; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("latitude", NullValueHandling = NullValueHandling.Ignore)]
        public double Latitude { get; set; }

        [JsonProperty("longitude", NullValueHandling = NullValueHandling.Ignore)]
        public double Longitude { get; set; }

        public string Image { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public async static Task<List<Pet>> GetPets()
        {
            List<Pet> pets = new List<Pet>();

            try
            {
                var url = Constants.LIST_PETS;

                using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
                {
                    var response = await client.GetAsync(url);

                    string json;

                    if (response.IsSuccessStatusCode)
                    {
                        json = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        //Json mock
                        json = "[{\"id\":1,\"name\":\"Bob\",\"breed\":\"Mixed Breed\",\"weight\":6.6,\"description\":\"Bob is a young blackcat.\",\"image\":\"https://cdn.pixabay.com/photo/2016/11/19/22/55/cat-1841561_960_720.jpg\",\"latitude\":-3.7834678,\"longitude\":-38.5634031},{\"id\":2,\"name\":\"Milo\",\"breed\":\"American Bobtail\",\"weight\":6.6,\"description\":\"Milo is a cat.\",\"image\":\"https://cdn.pixabay.com/photo/2015/12/14/10/26/cat-1092371_960_720.jpg\",\"latitude\":-3.777754,\"longitude\":-38.562245}]";
                    }

                    pets = JsonConvert.DeserializeObject<List<Pet>>(json);

                }

            } catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
            return pets;
        }


        public async static Task<List<Pet>> ListPets()
        {
            List<Pet> pets = new List<Pet>();

            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                {
                    conn.CreateTable<Pet>();
                    pets = conn.Table<Pet>().ToList();

                    //Populate local database
                    if (pets.Count < 1)
                    {
                        pets = await GetPets();

                        foreach (var pet in pets)
                        {
                            if (pets.Any(p => p.Id == pet.Id))
                            {
                                if (conn.Insert(pet) == 0)
                                    Debug.WriteLine("Pet failed to be inserted");
                            }
                        }
                    }

                }

            } catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return pets;
        }


        public static bool Update(Pet pet)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                if (conn.Update(pet) == 0)
                {
                    Debug.WriteLine("Pet failed to be inserted");
                    return false;
                }
            }
            return true;
        }

    }
}
