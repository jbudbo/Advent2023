using cs;

long start = Stopwatch.GetTimestamp();

//int d1p1 = Day1.Calibrate(Day1.P1_EXAMPLE);
//Debug.Assert(d1p1 == Day1.P1_ANSWER);

int d1p1 = Day1.Calibrate(File.ReadAllText("Day1.data"));
Debug.WriteLine(d1p1);

//int d1p2 = Day1.ReCalibrate(Day1.P2_EXAMPLE);
//Debug.Assert(d1p2 == Day1.P2_ANSWER);


//int d1p2 = Day1.ReCalibrate(File.ReadAllText("Day1.data"));
//Debug.WriteLine(d1p2);

Debug.WriteLine($"Run time {Stopwatch.GetElapsedTime(start)}");