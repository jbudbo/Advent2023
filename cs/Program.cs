using cs;

long start = Stopwatch.GetTimestamp();

//int d4p1 = Day4.P2(Day4.P1_EXAMPLE);
int d4p1 = Day4.P2(File.ReadAllText("Day4.data"));
Console.WriteLine($"Answer {d4p1}");

//int d3p1 = Day3.P2(File.ReadAllText("Day3.data"));
//Console.WriteLine($"Answer {d3p1}");

//int d2p1 = Day2.Aggregate(Day2.P1_EXAMPLE);
//Console.WriteLine($"Answer {d2p1}; Target {Day2.P1_ANSWER}");

//int d2p2 = Day2.Max(File.ReadAllText("Day2.data"));
//Console.WriteLine($"Answer {d2p2}");

//int d1p1 = Day1.Calibrate(Day1.P1_EXAMPLE);
//Debug.Assert(d1p1 == Day1.P1_ANSWER);

//int d1p1 = Day1.Calibrate(File.ReadAllText("Day1.data"));
//Debug.WriteLine(d1p1);

//int d1p2 = Day1.ReCalibrate(Day1.P2_EXAMPLE);
//Debug.Assert(d1p2 == Day1.P2_ANSWER);


//int d1p2 = Day1.ReCalibrate(File.ReadAllText("Day1.data"));
//Debug.WriteLine(d1p2);

Console.WriteLine($"Run time {Stopwatch.GetElapsedTime(start)}");
Console.ReadKey();