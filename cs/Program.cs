CancellationTokenSource cts = new();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

long start = Stopwatch.GetTimestamp();

var Answer = await new Day14().PartTwo(cts.Token);

Console.WriteLine($"Answer {Answer}");
Console.WriteLine($"Run time {Stopwatch.GetElapsedTime(start)}");
Console.ReadKey();
