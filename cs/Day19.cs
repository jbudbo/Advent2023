using System.Linq.Expressions;
using System.Reflection;

internal sealed partial class Day19 : AdventBase<Day19.MachinePart>
{
    protected override string Day => nameof(Day19);

    //private readonly Dictionary<string, ConditionalExpression> instructionSet = [];
    private readonly List<Expression> expressions = [];
    private Expression? inExpression;

    private int _headers = int.MaxValue;

    protected override int HeaderLines => _headers;

    private delegate string FollowPartDelegate(MachinePart part);
    
    protected override void ParseHeader(DataLine<string?> header, bool PartTwo)
    {
        var (l, (valid, data)) = header;

        if (!valid)
        {
            _headers = l;
            return;
        }

        Span<Range> buffer = stackalloc Range[3];
        ReadOnlySpan<char> source = data;
        
        source.SplitAny(buffer, "{}", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        string name = data[buffer[0]];

        string path = data[buffer[1]];

        var conditionPath = MakeCondition(path);

        var z = Expression.Lambda<Func<MachinePart, string>>(conditionPath, name, true, new[] { parameterExpression });

        if (name is "in")
            inExpression = z;
        else
            expressions.Add(z);
    }

    protected override Uof<MachinePart> ParseData(DataLine<string?> row, bool PartTwo)
    {
        var (_, (valid, data)) = row;

        if (!valid)
            return Union.Fail<MachinePart>();

        ReadOnlySpan<char> source = data;
        Span<Range> buffer = stackalloc Range[4];
        source.Split(buffer, ',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        int x = int.Parse(source[buffer[0]][3..]);
        int m = int.Parse(source[buffer[1]][2..]);
        int a = int.Parse(source[buffer[2]][2..]);
        int s = int.Parse(source[buffer[3]][2..^1]);

        return Union.Pass(new MachinePart
        {
            X = x,
            M = m,
            A = a,
            S = s
        });
    }

    protected override async ValueTask PartOne(IAsyncEnumerable<DataLine<MachinePart>> source, CancellationToken cancellationToken = default)
    {
        List<MachinePart> parts = [];

        await foreach(var (_,(valid,data)) in  source)
        {
            if (!valid) continue;

            parts.Add(data);
        }

        var root = Expression.Invoke(inExpression, parameterExpression);
        var b = Expression.Block(expressions.Prepend(root));
        var l = Expression.Lambda(b, parameterExpression);
        var x = l.Compile();

        var z = x.DynamicInvoke(parts[0]);

        //var program = Expression.Block(expressions);
        
        //var y = Expression.Lambda<Func<MachinePart, bool>>(program, parameterExpression);
        
        //Func<MachinePart, bool> test = y.Compile();

    }

    protected override ValueTask PartTwo(IAsyncEnumerable<DataLine<MachinePart>> source, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }


    internal readonly struct PartCondition
    {
        public int Value { get; init; }
        public char Field { get; init; }
        public string Continuation { get; init; }
        public char Op { get; init; }

        internal bool Evaluate(MachinePart p) => Field switch
        {
            'a' when Op is '<' => p.A < Value,
            'a' when Op is '>' => p.A > Value,
            'm' when Op is '<' => p.M < Value,
            'm' when Op is '>' => p.M > Value,
            'x' when Op is '<' => p.X < Value,
            'x' when Op is '>' => p.X > Value,
            's' when Op is '<' => p.S < Value,
            's' when Op is '>' => p.S > Value,
            _ => throw new UnreachableException()
        };
    }

    internal readonly struct MachinePart
    {
        public int X { get; init; }
        public int M { get; init; }
        public int A { get; init; }
        public int S { get; init; }
    }

    private static Expression MakeNode(ReadOnlySpan<char> source)
    {
        if (source.Contains(':'))
        {
            return MakeCondition(source);
        }
        
        return Expression.Constant(new string(source), typeof(string));
    }

    private static ConditionalExpression MakeCondition(ReadOnlySpan<char> source)
    {
        var member = MakeMember(source[0]);

        ExpressionType test = source[1] switch
        {
            '>' => ExpressionType.GreaterThan,
            '<' => ExpressionType.LessThan,
            _ => throw new UnreachableException()
        };

        Span<Range> buffer = stackalloc Range[3];
        source.SplitAny(buffer, ":,", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        int value = int.Parse(source[buffer[0]][2..]);
        ConstantExpression check = Expression.Constant(value, typeof(int));

        var conditionExpression = Expression.MakeBinary(test, member, check);

        Expression leftExpression = MakeNode(source[buffer[1]]);
        Expression rightExpression = MakeNode(source[buffer[2]]);

        return Expression.Condition(conditionExpression, leftExpression, rightExpression, typeof(string));
    }

    

    private static MemberExpression MakeMember(in char c)
        => Expression.MakeMemberAccess(parameterExpression, members[c]);

    private static ConstantExpression MakeConstant<T>(T val)
        => Expression.Constant(val, typeof(T));

    private static readonly ParameterExpression parameterExpression = Expression.Parameter(typeof(MachinePart), nameof(MachinePart));
    private static readonly IReadOnlyDictionary<char, MemberInfo> members = new Dictionary<char, MemberInfo>(4)
    {
        ['x'] = typeof(MachinePart).GetMember("X")[0],
        ['m'] = typeof(MachinePart).GetMember("M")[0],
        ['a'] = typeof(MachinePart).GetMember("A")[0],
        ['s'] = typeof(MachinePart).GetMember("S")[0],
    };
}
