﻿using System;
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
        private string id;
        private int count = 0;

        bool pre_rework;

        public Item(string n, int i, bool p)
        {
            item_name = n;
            id = i.ToString();
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

        public string getId()
        {
            return id;
        }

        public float getAverage(int num_games)
        {
            return (float)count / (float)num_games;
        }

    }

    class Program
    {
        static void printAllItemCount(List<Item> item_list, int n)
        {
            foreach (Item i in item_list)
            {
                Console.WriteLine("{0} has a total count of {1}", i.getName(), i.getCount());
                Console.WriteLine("{0} has an average of {1} per game.", i.getName(), i.getAverage(n));
            }
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

            using (StreamReader file = File.OpenText(@"..\..\..\..\AP_ITEM_DATASET\5.11\NORMAL_5X5\NA.json"))
            {
                JsonSerializer ser = new JsonSerializer();
                all_ids = (List<int>)ser.Deserialize(file, typeof(List<int>));
            }
            
            //index for traversing the "all_ids" list
            int match_index = 0;
            int game_count = 0;

            const string base_match_url = "https://na.api.pvp.net/api/lol/na/v2.2/match/";
            const string api_key = "?api_key=72ed6f93-1e5d-47b3-ae92-8c4657887887";
            string full_url;

            // loop on all data
            while (match_index < all_ids.Count())
            {
                string match_id = all_ids[match_index].ToString();
                //Console.WriteLine("\nmatch_id number is {0}", match_id);

                //FORM URL
                full_url = base_match_url + match_id + api_key;
                //Console.WriteLine("URL: {0}\n", full_url);

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
                    JsonTextReader reader = new JsonTextReader(new StreamReader(dataStream));
                    while (reader.Read())
                    {
                        if (reader != null)
                        {
                            // find all item fields
                            string name = (reader.Value ?? "").ToString();
                            if (name.Length == 5)
                            {
                                if (name.Substring(0, 4) == "item")
                                {
                                    // get item id
                                    reader.Read();
                                    if (reader == null)
                                    {
                                        Console.WriteLine("Could not get item ID. Skipping.");
                                        continue;
                                    }

                                    string item_id = (reader.Value ?? "").ToString();

                                    // add item to stats
                                    if (item_id == Rylai.getId())
                                        Rylai.Add();
                                    else if (item_id == Rabadon.getId())
                                        Rabadon.Add();
                                    else if (item_id == Void_Staff.getId())
                                        Void_Staff.Add();
                                    else if (item_id == Liandry.getId())
                                        Liandry.Add();
                                    else if (item_id == Zhonya.getId())
                                        Zhonya.Add();
                                    else if (item_id == Morello.getId())
                                        Morello.Add();
                                    else if (item_id == Athene.getId())
                                        Athene.Add();
                                    else if (item_id == Luden.getId())
                                        Luden.Add();
                                    else if (item_id == Archangel.getId())
                                        Archangel.Add();
                                    else if (item_id == Seraph.getId())
                                        Seraph.Add();
                                    else if (item_id == RoA.getId())
                                        RoA.Add();
                                    else if (item_id == Nashor.getId())
                                        Nashor.Add();
                                    else if (item_id == WotA.getId())
                                        WotA.Add();
                                }
                            }
                        }
                    }

                    //simple output to show the # of items in the game
                    Console.WriteLine("Match id: {0}", all_ids[match_index]);
                    printAllItemCount(item_list, match_index+1);
                    Console.WriteLine("Number of games left to check: {0}\n", all_ids.Count() - match_index);
                    
                    match_index++;
                    game_count++;

                }
            }
            Console.WriteLine("Goodbye.");
        }
    }
}
