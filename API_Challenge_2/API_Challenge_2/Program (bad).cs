using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace API_Challenge_2
{
    public class Item
    {
        private string item_name;
        private int id;
        private int count = 0;
        private float average_per_game = 0.0f;

        bool pre_rework;

        public Item(string n, int i, bool p)
        {
            item_name = n;
            id = i;
            pre_rework = p;
        }

        public string getName()
        {
            return item_name;
        }

        public void Add()
        {
            count++;
        }

        public int getCount()
        {
            return count;
        }

        public int getId()
        {
            return id;
        }

        public void calculateAverage(int num_games)
        {
            average_per_game = count / num_games;
        }

        public float getAverage()
        {
            return average_per_game;
        }
    }

    class Program
    {
        static void printAllItemCount(Dictionary <int, Item> items)
        {
            foreach (int i in items.Keys)
                Console.WriteLine("{0} has a count of {1}", items[i].getName(), items[i].getCount());

            //blank line to separate matches
            Console.WriteLine("\n");
        }

        static void Main(string[] args)
        {
            //set up the item class objects for the "fully built" AP items
            Dictionary <int, Item> items = new Dictionary <int, Item>();

            items[3116] = new Item("Rylai's Crystal Scepter", 3116, true);
            items[3089] = new Item("Rabadon's Deathcap", 3089, true);
            items[3135] = new Item("Void Staff", 3135, true);
            items[3135] = new Item("Liandry's Torment", 3151, true);
            items[3157] = new Item("Zhonya's Hourglass", 3157, true);
            items[3165] = new Item("Morellonomicon", 3165, true);
            items[3174] = new Item("Athene's Unholy Grail", 3174, true);
            items[3285] = new Item("Luden's Echo", 3285, true);
            items[3003] = new Item("Argchangel's Staff", 3003, true);
            items[3048] = new Item("Seraph's Embrace", 3048, true);
            items[3027] = new Item("Rod of Ages", 3027, true);
            items[3115] = new Item("Nashor's Tooth", 3115, true);
            items[3152] = new Item("Will of the Ancients", 3152, true);

            int game_count = 0;

            string match_id;

            string base_match_url = "https://na.api.pvp.net/api/lol/na/v2.2/match/";
            string api_key = "?api_key=72ed6f93-1e5d-47b3-ae92-8c4657887887";

            //infinitely loop to allow for testing on any number of items
            while (true)
            {
                Console.WriteLine("Please give a match id or type 'exit' to exit.");
                match_id = Console.ReadLine().ToString();
                if (match_id == "exit")
                    break;
                Console.WriteLine("\nmatch_id number is {0}", match_id);

                //FORM URL
                string full_url = base_match_url + match_id + api_key;
                Console.WriteLine("URL: {0}\n", full_url);

                //CALL API WITH FORMED URL
                WebRequest request = WebRequest.Create(full_url);
                HttpWebResponse response;
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                }
                catch (WebException ex)
                {
                    response = ex.Response as HttpWebResponse;
                }

                //check that the match_id provided is valid
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine("{0} is an invalid match id, please enter a valid match id\n", match_id);
                }
                else
                {
                    // Parse data
                    //JObject record = JObject.Parse(response);

                    // get players
                    //JArray participants = (JObject) record["participants"];

                    // go through each player
                    //foreach (JObject player in participants.Children<JObject>())
                    // {
                        // JObject stats = (JObject) participants ["stats"];
                        // items[(int)stats["item0"]].add();
                        // items[(int)stats["item1"]].add();
                        // items[(int)stats["item2"]].add();
                        // items[(int)stats["item3"]].add();
                        // items[(int)stats["item4"]].add();
                        // items[(int)stats["item5"]].add();
                        // items[(int)stats["item6"]].add();
                    // }

                    //simple output to show the # of items in the game
                    printAllItemCount(items);
                }
                //end of infinite loop
            }
            Console.WriteLine("Goodbye.");
        }
    }
}
