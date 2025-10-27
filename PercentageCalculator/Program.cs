// See https://aka.ms/new-console-template for more information

static int? ReadNullableInt(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();
        // Treat empty input as null (unknown)
        if (string.IsNullOrWhiteSpace(input))
            return null;

        if (int.TryParse(input.Trim(), out int value))
        {
            if (value < 0)
            {
                Console.WriteLine("Please enter a non-negative integer.");
                continue;
            }
            return value;
        }

        Console.WriteLine("Invalid integer. Try again.");
    }
}

static bool TwoOrMoreZerosOrNulls(params int?[] values)
{
    int zeroCount = 0;
    foreach (var v in values)
    {
        if (v == null || v == 0) zeroCount++;
        if (zeroCount >= 2) return true;
    }
    return false;
}

static bool IsUnknown(int? v) => v == null || v == 0;

int? RightAnswers = 0;
int? FalseAnswers = 0;
int? NotAnswered = 0;
int? TotalQuestions = 0;
float Percentage = 0;

Console.WriteLine("Wellcome to the percentage calculator!");

while (true)
{
    RightAnswers = ReadNullableInt("Please enter the number of right answers: ");
    FalseAnswers = ReadNullableInt("Please enter the number of false answers: ");
    NotAnswered = ReadNullableInt("Please enter the number of not answered questions: ");
    TotalQuestions = ReadNullableInt("Please enter the total number of questions: ");

    if (TwoOrMoreZerosOrNulls(RightAnswers, FalseAnswers, NotAnswered, TotalQuestions))
    {
        Console.WriteLine("Error: two or more values are zero or blank. Provide at least three values (only one value may be unknown/blank). Let's try again.\n");
        continue;
    }

    // If exactly one is unknown (null or 0), deduce it by treating null as 0 in arithmetic
    if (IsUnknown(RightAnswers))
    {
        RightAnswers = (TotalQuestions ?? 0) - (FalseAnswers ?? 0) - (NotAnswered ?? 0);
    }
    else if (IsUnknown(FalseAnswers))
    {
        FalseAnswers = (TotalQuestions ?? 0) - (RightAnswers ?? 0) - (NotAnswered ?? 0);
    }
    else if (IsUnknown(NotAnswered))
    {
        NotAnswered = (TotalQuestions ?? 0) - (RightAnswers ?? 0) - (FalseAnswers ?? 0);
    }
    else if (IsUnknown(TotalQuestions))
    {
        TotalQuestions = (RightAnswers ?? 0) + (FalseAnswers ?? 0) + (NotAnswered ?? 0);
    }

    // Validate deduced/provided values
    if ((RightAnswers ?? 0) < 0 || (FalseAnswers ?? 0) < 0 || (NotAnswered ?? 0) < 0 || (TotalQuestions ?? 0) < 0)
    {
        Console.WriteLine("Error: one or more deduced values are negative — inputs are inconsistent. Please re-enter.\n");
        continue;
    }

    if ((RightAnswers ?? 0) + (FalseAnswers ?? 0) + (NotAnswered ?? 0) != (TotalQuestions ?? 0))
    {
        Console.WriteLine("Error: values are inconsistent (Right + False + Not = Total). Please check and re-enter.\n");
        continue;
    }

    // All good
    break;
}

int r = RightAnswers ?? 0;
int f = FalseAnswers ?? 0;
int n = NotAnswered ?? 0;
int t = TotalQuestions ?? 0;

Console.WriteLine($"Right: {r}, False: {f}, Not: {n}, Total: {t}");

if (t == 0)
{
    Console.WriteLine("Total questions is 0 — cannot compute percentage.");
}
else
{
    int score = (r * 3) - f;
    int maxScore = t * 3;

    // Cast to float to avoid integer division
    Percentage = (float)score / maxScore * 100f;

    //Console.WriteLine($"Score: {score} / {maxScore}");
    Console.WriteLine($"Percentage: {Percentage:F2}%");
}