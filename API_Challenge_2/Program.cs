using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using Accord.Statistics.Testing;

namespace API_Challenge_2
{
    class Item
    {
        private string item_name;
        private string id;
        private int count = 0;

        public Item(string n, int i)
        {
            item_name = n;
            id = i.ToString();
        }

        public string getName()
        {
            return item_name;
        }

        public int getCount()
        {
            return count;
        }

        public string getId()
        {
            return id;
        }

        public void Add()
        {
            count++;
        }

        public float getAverage(int num_games)
        {
            return (float)count / (float)num_games;
        }
    }

    class Items
    {
        public List<Item> item_list = new List<Item>();

        public Items()
        {
            Item Rylai = new Item("Rylai's Crystal Scepter", 3116);
            Item Rabadon = new Item("Rabadon's Deathcap", 3089);
            Item Void_Staff = new Item("Void Staff", 3135);
            Item Liandry = new Item("Liandry's Torment", 3151);
            Item Zhonya = new Item("Zhonya's Hourglass", 3157);
            Item Morello = new Item("Morellonomicon", 3165);
            Item Athene = new Item("Athene's Unholy Grail", 3174);
            Item Luden = new Item("Luden's Echo", 3285);
            Item Archangel = new Item("Argchangel's Staff", 3003);
            Item Seraph = new Item("Seraph's Embrace", 3048);
            Item RoA = new Item("Rod of Ages", 3027);
            Item Nashor = new Item("Nashor's Tooth", 3115);
            Item WotA = new Item("Will of the Ancients", 3152);

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
        }

        // add to count
        public void Add(string item_id)
        {
            foreach(Item i in item_list)
            {
                if (i.getId() == item_id)
                {
                    i.Add();
                }
            }
        }

        // print stats
        public void print(int n)
        {
            foreach (Item i in item_list)
            {
                Console.WriteLine("{0} has a total count of {1}", i.getName(), i.getCount());
                Console.WriteLine("{0} has an average of {1} per game.", i.getName(), i.getAverage(n));
            }
        }

        public Item at(int i)
        {
            if (i >= item_list.Count())
            {
                return null;
            }
            return item_list[i];
        }
    }

    class RequestBlock
    {
        private int req;                            // number of requests per time
        private int req_per_secs;                   // time for req
        private int max;                            // maximum number of requests per time
        private int max_per_secs;                   // time for max

        private int count;                          // count of requests within req_per_secs
        private System.Timers.Timer count_time;     // how much of req_per_secs has passed
        private int total;                          // count of max requests within max_per_secs
        private System.Timers.Timer total_time;     // how much of max_per_secs has passed

        public RequestBlock(int r, int rps, int m, int mps)
        {
            req = r;
            req_per_secs = rps * 1000;
            max = m;
            max_per_secs = mps * 1000;

            count = 0;
            count_time = new System.Timers.Timer(req_per_secs);
            count_time.Elapsed += reset_req;
            count_time.AutoReset = true;
            count_time.Enabled = true;

            total = 0;
            total_time = new System.Timers.Timer(max_per_secs);
            total_time.Elapsed += reset_max;
            total_time.AutoReset = true;
            total_time.Enabled = true;
        }

        private void reset_req(Object source, System.Timers.ElapsedEventArgs e)
        {
            count = 0;
        }

        private void reset_max(Object source, System.Timers.ElapsedEventArgs e)
        {
            total = 0;
        }

        public void block()
        {
            count++;
            total++;
            while (((count >= req) && count_time.Enabled) || ((total >= max) && total_time.Enabled))
            {
                Thread.Sleep(251);
            }
        }
    }

    class Program
    {
        private const string base_match_url = "https://na.api.pvp.net/api/lol/na/v2.2/match/";
        private const string api_key        = "?api_key=72ed6f93-1e5d-47b3-ae92-8c4657887887";
        private static string[] types = new string[] { "NORMAL_5X5", "RANKED_SOLO" };
        private static string[] regions = { "BR", "EUNE", "EUW", "KR", "LAN", "LAS", "NA", "OCE", "RU", "TR" };
        private const string pre = "5.11";
        private const string post = "5.14";

        private const int item_count        = 13;
        private const int sample_size       = 100;

        // download data or load from cache
        // returns file name (unfortunately)
        static string cache(string cache_dir, string match_id, RequestBlock blocker)
        {
            // if data is already cached, just return
            string cache_file = cache_dir + "/" + match_id;
            if (File.Exists(cache_file))
            {
                return cache_file;
            }

            // block request if rate exceeded
            blocker.block();

            // if file doesn't exist, download it

            // FORM URL
            string full_url = base_match_url + match_id + api_key;
            //Console.WriteLine("URL: {0}", full_url);

            // CALL API WITH FORMED URL
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

            // check that the match_id provided is valid
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            // match_id is valid

            // read downloaded data
            Stream dataStream = response.GetResponseStream();

            // /////////////////////////////////////////
            // have to check if data is valid here
            // but dataStream cannot have its position set
            // /////////////////////////////////////////

            File.Create(cache_file).Close();
            using (FileStream fs = File.Open(cache_file, FileMode.Open, FileAccess.Write))
            {
                dataStream.CopyTo(fs);
            }

            // parse data
            return cache(cache_dir, match_id, blocker);
        }

        // read through single match and puts data into items
        static void read(JsonTextReader reader, Items items)
        {
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
                            items.Add(item_id);
                        }
                    }
                }
            }
        }

        // read each file and compile their stats
        static void aggregate(List<string> match_ids, Items items, string cache_dir)
        {
            List<int> used_matches = new List<int>();
            Random rng = new Random();

            int availible_match_ids = match_ids.Count();

            RequestBlock blocker = new RequestBlock(10, 10, 500, 600);

            // loop on data
            // while (match_index < match_ids.Count())
            for (int game_count = 0; game_count < sample_size; game_count++)
            {
                // get random unused match
                int match_index = rng.Next(0, availible_match_ids);
                int attempts;
                for(attempts = 0; (used_matches.Contains(match_index)) && (attempts < availible_match_ids); attempts++)
                {
                    match_index = rng.Next(0, availible_match_ids);
                }

                // if no more availible matches, stop
                if (attempts == availible_match_ids)
                {
                    return;
                }

                used_matches.Add(match_index);

                string match_id = match_ids[match_index];
                Console.WriteLine("game {0,-4}: index {1,4}, match {2}", game_count, match_index, match_id);

                string cache_file = cache(cache_dir, match_id, blocker);
                if (cache_file == null)
                {
                    game_count--;
                    continue;
                }

                using (StreamReader file = File.OpenText(cache_file))
                {
                    try
                    {
                        JsonTextReader data = new JsonTextReader(file);
                        if (data != null)
                        {
                            read(data, items);
                            //simple output to show the # of items in the game
                            // Console.WriteLine("Match id: {0}", all_ids[match_index]);
                            // items.print(game_count + 1);
                            // Console.WriteLine("Number of games left to check: {0}\n", sample_size - game_count + 1);
                        }
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        game_count--;
                        continue;
                    }
                }
            }
        }

        static int Main(string[] args)
        {
            if (args.Count() != 4)
            {
                Console.WriteLine("Syntax: {0} region game_type match_id_dir cache_dir", Process.GetCurrentProcess().ProcessName);
                return 1;
            }

            string region = args[0];
            string type = args[1];
            string match_id_dir = args[2];
            string cache_dir = args[3];

            // parse commandline arguments

            // make sure region is valid
            // (currently not useful)
            if (!regions.Contains(region))
            {
                Console.WriteLine("Invaid region: {0}", region);
                return 1;
            }

            // make sure game type is valid
            if (!types.Contains(type))
            {
                Console.WriteLine("Invalid game type: {0}", type);
                return 1;
            }

            // make sure match id files are valid
            string pre_match_ids_dir = match_id_dir + "/" + pre + "/" + type + "/" + region + ".json";
            if (!File.Exists(pre_match_ids_dir))
            {
                Console.WriteLine("File {0} does not exist", pre_match_ids_dir);
                return 1;
            }

            string post_match_ids_dir = match_id_dir + "/" + post + "/" + type + "/" + region + ".json";
            if (!File.Exists(post_match_ids_dir))
            {
                Console.WriteLine("File {0} does not exist", post_match_ids_dir);
                return 1;
            }

            // make sure cache directory is ready
            string pre_cache_dir = cache_dir + "/" + pre + "/" + type;
            if (!Directory.Exists(pre_cache_dir))
            {
                Directory.CreateDirectory(pre_cache_dir);
            }
            string post_cache_dir = cache_dir + "/" + post + "/" + type;
            if (!Directory.Exists(post_cache_dir))
            {
                Directory.CreateDirectory(post_cache_dir);
            }

            // read match id lists
            List<string> pre_match_ids = new List<string>();
            using (StreamReader file = File.OpenText(pre_match_ids_dir))
            {
                JsonSerializer ser = new JsonSerializer();
                pre_match_ids = (List<string>)ser.Deserialize(file, typeof(List<string>));
            }

            List<string> post_match_ids = new List<string>();
            using (StreamReader file = File.OpenText(post_match_ids_dir))
            {
                JsonSerializer ser = new JsonSerializer();
                post_match_ids = (List<string>)ser.Deserialize(file, typeof(List<string>));
            }

            // allocate memory for data
            Items pre_rework_data = new Items();
            Items post_rework_data = new Items();

            aggregate(pre_match_ids, pre_rework_data, cache_dir + "/" + pre + "/" + type);
            aggregate(post_match_ids, post_rework_data, cache_dir + "/" + post + "/" + type);

            //Console.WriteLine("\nPre-rework Stats");
            //pre_rework_data.print(sample_size);

            //Console.WriteLine("\nPost-rework Stats");
            //post_rework_data.print(sample_size);

            Console.WriteLine("Item count toals for Patch:|  {0}  |  {1}  |", pre, post);
            Console.WriteLine("-----------------------------------------------");

            // copy counts out to arrays
            double[] pre_counts = new double[item_count];
            double[] post_counts = new double[item_count];

            for (int i = 0; i < item_count; i++)
            {
                pre_counts[i] = pre_rework_data.at(i).getCount();
                post_counts[i] = post_rework_data.at(i).getCount();
                Console.WriteLine("{0, -25}  | {1, 6} | {2, 6} |", pre_rework_data.at(i).getName(), pre_counts[i], post_counts[i]);
            }

            ChiSquareTest test = new ChiSquareTest(pre_counts, post_counts, item_count - 1);

            Console.WriteLine("");
            Console.WriteLine("Pearson's Chi-Square Test for Independence:\n");
            Console.WriteLine("Sample Size: {0}", sample_size);
            Console.WriteLine("Degrees of Freedom: {0}", item_count - 1);
            Console.WriteLine("Signigicance Level: {0}", test.Size);
            Console.WriteLine("P-Value: {0}", test.PValue);
            Console.WriteLine("");
            Console.WriteLine("Difference Significant? {0}", test.Significant);

            Console.WriteLine("\nGoodbye.");



            return 0;
        }
    }
}