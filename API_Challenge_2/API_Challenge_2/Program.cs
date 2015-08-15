using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace API_Challenge_2
{
    class Item
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
        static void printAllItemCount(List<Item> item_list)
        {
            foreach (Item i in item_list)
                Console.WriteLine("{0} has a count of {1}", i.getName(), i.getCount());
        }

        static void Main(string[] args)
        {
            //set up the item class objects for the "fully built" AP items 
            //**************(POST-REWORK)************
            Item Rylai = new Item("Rylai's Crystal Scepter", 3116, true);
            Item Rabadon = new Item("Rabadon's Deathcap", 3089, true);
            Item Void_Staff = new Item("Void Staff", 3135, true);
            Item Liandry = new Item("Liandry's Torment", 3151, true);
            Item Zhonya = new Item("Zhonya's Hourglass", 3157, true);
            Item Morello = new Item("Morellonomicon", 3165, true);
            Item Athene = new Item("Athene's Unholy Grail", 3174, true);
            Item Luden = new Item("Luden's Echo", 3285, true);
            Item Archangel = new Item("Argchangel's Staff", 3003, true);
            Item Seraph = new Item("Seraph's Embrace", 3048, true);
            Item RoA = new Item("Rod of Ages", 3027, true);
            Item Nashor = new Item("Nashor's Tooth", 3115, true);
            Item WotA = new Item("Will of the Ancients", 3152, true);

            List<Item> item_list = new List<Item>();
            item_list.Add(Rylai);
            item_list.Add(Rabadon);
            item_list.Add(Void_Staff);
            item_list.Add(Liandry);
            item_list.Add(Zhonya);
            item_list.Add(Morello);
            item_list.Add(Athene);
            item_list.Add(Luden);
            item_list.Add(Archangel);
            item_list.Add(Seraph);
            item_list.Add(RoA);
            item_list.Add(Nashor);
            item_list.Add(WotA);


            List<int> all_ids = new List<int>();
            //BEAR IN MIND THIS IS USING THE LOCATION OF THE JSON FILE(S) ON KARL'S COMPUTER
            using (StreamReader file = File.OpenText(@"C:\Users\Karl Olsen\Desktop\API_Challenge\AP_ITEM_DATASET\5.11\NORMAL_5X5\NA.json"))
            {
                JsonSerializer ser = new JsonSerializer();
                all_ids = (List<int>)ser.Deserialize(file, typeof(List<int>));
            }

            foreach (int x in all_ids)
            {
                Console.WriteLine(x);
            }


            int game_count = 0;

            string match_id;

            string base_match_url = "https://na.api.pvp.net/api/lol/na/v2.2/match/";
            string api_key = "?api_key=72ed6f93-1e5d-47b3-ae92-8c4657887887";
            string full_url;
            string full_match_information;


            //characters to "ignore" while reading the item's information
            char[] delimiters = { '}', '{', ':', '[', ']', ',', '\"', ';', '(', ')', '<', '>' };
            //the words that are "read" in the item's information
            string[] word_array;
            //"cleaner" version of word_array
            List<string> words = new List<string>();

            //infinitely loop to allow for testing on any number of items
            while (true)
            {
                Console.WriteLine("Please give a match id or type 'exit' to exit.");
                match_id = Console.ReadLine().ToString();
                if (match_id == "exit")
                    break;
                Console.WriteLine("\nmatch_id number is {0}", match_id);

                //FORM URL 
                full_url = base_match_url + match_id + api_key;
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
                    //COLLECT RELEVANT DATA FROM THE API
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    full_match_information = reader.ReadToEnd();

                    word_array = full_match_information.Split(delimiters);

                    //cleaning up the blank spots and unwanted strings that tokenizing the original string created
                    for (int i = 0; i < word_array.Length; i++)
                    {
                        if (word_array[i] != "")
                            words.Add(word_array[i]);
                    }


                    Console.WriteLine("*********************************");
                    //search through the match's items for each character to find any of the relevant items' ids
                    for (int i = 0; i < words.Count(); i++)
                    {
                        //extract the item's name
                        if (words[i] == "item0" || words[i] == "item1" || words[i] == "item2" || words[i] == "item3" || words[i] == "item4" || words[i] == "item5")
                        {
                            if (words[i + 1] == Rylai.getId().ToString())
                                Rylai.Add();
                            if (words[i + 1] == Rabadon.getId().ToString())
                                Rabadon.Add();
                            if (words[i + 1] == Void_Staff.getId().ToString())
                                Void_Staff.Add();
                            if (words[i + 1] == Liandry.getId().ToString())
                                Liandry.Add();
                            if (words[i + 1] == Zhonya.getId().ToString())
                                Zhonya.Add();
                            if (words[i + 1] == Morello.getId().ToString())
                                Morello.Add();
                            if (words[i + 1] == Athene.getId().ToString())
                                Athene.Add();
                            if (words[i + 1] == Luden.getId().ToString())
                                Luden.Add();
                            if (words[i + 1] == Archangel.getId().ToString())
                                Archangel.Add();
                            if (words[i + 1] == Seraph.getId().ToString())
                                Seraph.Add();
                            if (words[i + 1] == RoA.getId().ToString())
                                RoA.Add();
                            if (words[i + 1] == Nashor.getId().ToString())
                                Nashor.Add();
                            if (words[i + 1] == WotA.getId().ToString())
                                WotA.Add();
                        }


                    }

                    //simple output to show the # of items in the game
                    printAllItemCount(item_list);

                    //clear the information variables for the next test
                    match_id = "";
                    words.Clear();

                    //blank line to separate matches
                    Console.WriteLine("\n");
                }
                //end of infinite loop
            }

            Console.WriteLine("Goodbye.");

        }
    }
}
