import json
import os
import urllib2

base_match_url = "https://na.api.pvp.net/api/lol/na/v2.2/match/"
api_key = "?api_key=72ed6f93-1e5d-47b3-ae92-8c4657887887"

class Item:
    def __init__(self, item_name, id, pre_work):
        self.__item_name = item_name
        self.__id = id
        self.__pre_rework = pre_work
        self.__count = 0
        self.__average_per_game = 0.0

    def getName(self):
        return self.__item_name

    def Add(self):
        self.__count += 1

    def getCount(self):
        return self.__count

    def getId(self):
        return self.__id

    def calculateAverage(self, num_games):
        self.__average_per_game = self.__count / num_games

    def getAverage(self):
        return self.__average_per_game

    def __str__(self):
        return "{0}: {1} {2}".format(self.__id, self.__item_name, self.__count)

def printAllItemCount(items):
    for id in sorted(items):
        print("{0} has a count of {1}".format(items[id].getName(), items[id].getCount()))

    # blank line to separate matches
    print("\n")

def main():
    # initialize data
    items = {   3116: Item("Rylai's Crystal Scepter",   3116, True),
                3089: Item("Rabadon's Deathcap",        3089, True),
                3135: Item("Void Staff",                3135, True),
                3135: Item("Liandry's Torment",         3151, True),
                3157: Item("Zhonya's Hourglass",        3157, True),
                3165: Item("Morellonomicon",            3165, True),
                3174: Item("Athene's Unholy Grail",     3174, True),
                3285: Item("Luden's Echo",              3285, True),
                3003: Item("Argchangel's Staff",        3003, True),
                3048: Item("Seraph's Embrace",          3048, True),
                3027: Item("Rod of Ages",               3027, True),
                3115: Item("Nashor's Tooth",            3115, True),
                3152: Item("Will of the Ancients",      3152, True),
            }

    game_count = 0
    while (True):
        match_id = raw_input("Please give a match id or type 'exit' to exit.")
        if (match_id == "exit"):
            break

        print("match_id number is {0}".format(match_id))

        # FORM URL
        full_url = base_match_url + match_id + api_key
        print("URL: {0}\n".format(full_url))

        # CALL API WITH FORMED URL
        response = ""
        try:
            response = urllib2.urlopen(full_url)
        except urllib2.URLError as e:
            print "{0} is an invalid match id, please enter a valid match id\n".format(match_id)
            return

        data = json.load(response)

        for player in data["participants"]:
            stats = player["stats"]
            for item in range(7):
                id = int(stats["item{0}".format(item)])
                if id in items:
                    items[id].Add()

        printAllItemCount(items)

    print("Goodbye.")

if __name__=='__main__':
    main()