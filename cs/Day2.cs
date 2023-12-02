namespace cs;

internal static partial class Day2
{
    const StringSplitOptions DefaultSplitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
    public const string P1_EXAMPLE = """
    Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
    Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
    Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
    Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
    Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green
    """;
    public const int P1_ANSWER = 8;

    public static int Max(string set)
    {
        int sumOfPower = 0;

        foreach (var line in set.Split(Environment.NewLine)){
            Console.WriteLine(line);

            string[] playData = line.Split(':', DefaultSplitOptions);

            int maxBlue = 0, maxGreen = 0, maxRed = 0;
            foreach (var play in playData[1].Split(';', DefaultSplitOptions))
            {
                foreach (var cube in play.Split(',', DefaultSplitOptions))
                {
                    string[] data = cube.Split(' ', DefaultSplitOptions);

                    int amount = int.Parse(data[0]);

                    switch (data[1].ToLower()){
                        case "blue" when amount > maxBlue:
                            maxBlue = amount;
                            continue;
                            
                        case "green" when amount > maxGreen:
                            maxGreen = amount;
                            continue;

                        case "red" when amount > maxRed:
                            maxRed = amount;
                            continue;
                    }
                }
            }

            sumOfPower += maxBlue * maxGreen * maxRed;
        }

        return sumOfPower;
    }

    public static int Aggregate(string set)
    {
        const int maxRed = 12, maxGreen = 13, maxBlue = 14;

        int sumOfGameIds = 0;

        foreach (var line in set.Split(Environment.NewLine)){
            Console.WriteLine(line);

            string[] playData = line.Split(':', DefaultSplitOptions);

            bool gameFailed = false;
            foreach (var play in playData[1].Split(';', DefaultSplitOptions))
            {
                if (gameFailed) break;
                foreach (var cube in play.Split(',', DefaultSplitOptions))
                {
                    if (gameFailed) break;

                    string[] data = cube.Split(' ', DefaultSplitOptions);

                    int amount = int.Parse(data[0]);

                    switch (data[1].ToLower()){
                        case "blue" when amount > maxBlue:
                            gameFailed = true;
                            continue;
                            
                        case "green" when amount > maxGreen:
                            gameFailed = true;
                            continue;

                        case "red" when amount > maxRed:
                            gameFailed = true;
                            continue;
                    }
                }
            }

            if (!gameFailed)
            {
                Console.WriteLine("PASS");
                sumOfGameIds += int.Parse(playData[0][5..]);
            }
            else{
                Console.WriteLine("FAIL");
            }
        }

        return sumOfGameIds;
    }
}
