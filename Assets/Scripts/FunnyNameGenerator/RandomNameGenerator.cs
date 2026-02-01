using System;

public static class RandomNameGenerator
{
    private static string? name;

    static readonly string[] Adjectives =
    {
    "Sunny","Breezy","Cozy","Sparkly","Shiny","Glowing","Twinkly","Neon","Vivid","Mellow",
    "Jolly","Cheerful","Happy","Playful","Bubbly","Chirpy","Peppy","Witty","Silly","Goofy",
    "Quirky","Zany","Funky","Groovy","Snazzy","Nifty","Dandy","Dazzling","Dreamy","Zippy",
    "Swift","Nimble","Bouncy","Wiggly","Wobbly","Jumpy","Floaty","Swooshy","Whizzy","Zoomy",
    "Curious","Clever","Bright","Brave","Kind","Gentle","Friendly","Helpful","Lucky","Calm",
    "Sleepy","Tiny","Mini","Mighty","Giant","Chubby","Fluffy","Fuzzy","Icy","Frosty",
    "Toasty","Warm","Cool","Fresh","Crispy","Buttery","Sugary","Spicy","Zesty","Minty",
    "Cosmic","Galactic","Lunar","Solar","Stellar","Magic","Mystic","Pixel","Digital","Retro"
    };

    static readonly string[] Nouns =
    {
    "Panda","Koala","Otter","Penguin","Dolphin","Turtle","Bunny","Kitten","Puppy","Hamster",
    "Hedgehog","Raccoon","Fox","Llama","Alpaca","Giraffe","Zebra","Squirrel","Chipmunk","Badger",
    "Parrot","Robin","Falcon","Owl","Swan","Seagull","Butterfly","Bumblebee","Ladybug","Firefly",
    "Frog","Toad","Snail","Crab","Octopus","Seahorse","Starfish","Jellyfish","Goldfish","Whale",
    "Unicorn","Phoenix","Dragonfly","Mermaid","Elf","Sprite","Gnome","Wizard","Astronaut","Explorer",
    "Rocket","Comet","Meteor","Planet","Galaxy","Star","Moonbeam","Sunbeam","Rainbow","Cloud",
    "Pancake","Waffle","Cupcake","Muffin","Donut","Cookie","Brownie","Marshmallow","Popcorn","Noodle",
    "Dumpling","Sushi","Taco","Burrito","Pickle","Peach","Mango","Banana","Coconut","Blueberry",
    "Toaster","Teapot","Kettle","Backpack","Helmet","Sneaker","Skateboard","Bicycle","Kite","Balloon",
    "Guitar","Piano","Trumpet","Drum","Camera","Compass","Notebook","Crayon","Paintbrush","Sticker"
    };

    public static string GetName() => name == null ? GenerateName() : name;

    private static string GenerateName()
    {
        var rnd = new Random();

        string adjective = Adjectives[rnd.Next(Adjectives.Length)];
        string noun = Nouns[rnd.Next(Nouns.Length)];

        int number = rnd.Next(1, 100); // 1–99

        name = $"{adjective}{noun}{number}";
        return name;
    }

    public static string SingleTimeUseNickname()
    {
        var rnd = new Random();

        string adjective = Adjectives[rnd.Next(Adjectives.Length)];
        string noun = Nouns[rnd.Next(Nouns.Length)];

        int number = rnd.Next(1, 100); // 1–99

        return $"{adjective}{noun}{number}";
    }
}
