// See https://aka.ms/new-console-template for more information
//git tag -a v1.2.0 -m "Release v1.2.0" then git push origin v1.2.0

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
            if (value > 1000) // reasonable upper limit
            {
                Console.WriteLine("Value seems too large. Are you sure? (y/n)");
                if (Console.ReadLine()?.ToLower() != "y")
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

//int? RightAnswers = 0;
//int? FalseAnswers = 0;
//int? NotAnswered = 0;
//int? TotalQuestions = 0;
float Percentage = 0;
float OptimistPercentage = 0;

Console.WriteLine("Welcome to the percentage calculator!");
Console.WriteLine("Type 'exit' at any prompt to quit the program.\n");

while (true)
{
    try
    {
        int? rightAnswers = null, falseAnswers = null,
            notAnswered = null, totalQuestions = null;

        while (true)
        {
            rightAnswers = ReadNullableInt("Please enter the number of right answers: ");
            falseAnswers = ReadNullableInt("Please enter the number of false answers: ");
            notAnswered = ReadNullableInt("Please enter the number of not answered questions: ");
            totalQuestions = ReadNullableInt("Please enter the total number of questions: ");

            if (TwoOrMoreZerosOrNulls(rightAnswers, falseAnswers, notAnswered, totalQuestions))
            {
                if (TwoOrMoreZerosOrNulls(rightAnswers, notAnswered, totalQuestions))
                {
                    Console.WriteLine("Error: two or more values are zero or blank. Provide at least three values (only one value may be unknown/blank). Let's try again.\n");
                    continue;
                }
                Console.WriteLine("Assuming false answers as 0");
            }

            // If exactly one is unknown (null or 0), deduce it by treating null as 0 in arithmetic
            if (IsUnknown(rightAnswers))
                rightAnswers = (totalQuestions ?? 0) - (falseAnswers ?? 0) - (notAnswered ?? 0);
            
            else if (IsUnknown(notAnswered))
                notAnswered = (totalQuestions ?? 0) - (rightAnswers ?? 0) - (falseAnswers ?? 0);
            
            else if (IsUnknown(totalQuestions))
                totalQuestions = (rightAnswers ?? 0) + (falseAnswers ?? 0) + (notAnswered ?? 0);
            
            else if (IsUnknown(falseAnswers))
            {
                if (falseAnswers == 0)
                    break;
                falseAnswers = (totalQuestions ?? 0) - (rightAnswers ?? 0) - (notAnswered ?? 0);
            }

            // Validate deduced/provided values
            if ((rightAnswers ?? 0) < 0 || (falseAnswers ?? 0) < 0 || (notAnswered ?? 0) < 0 || (totalQuestions ?? 0) < 0)
            {
                Console.WriteLine("Error: one or more deduced values are negative — inputs are inconsistent. Please re-enter.\n");
                continue;
            }

            if ((rightAnswers ?? 0) + (falseAnswers ?? 0) + (notAnswered ?? 0) != (totalQuestions ?? 0))
            {
                Console.WriteLine("Error: values are inconsistent (Right + False + Not = Total). Please check and re-enter.\n");
                continue;
            }

            // All good
            break;
        }

        int r = rightAnswers ?? 0;
        int f = falseAnswers ?? 0;
        int n = notAnswered ?? 0;
        int t = totalQuestions ?? 0;

        Console.WriteLine($"Right: {r}, False: {f}, Not: {n}, Total: {t}");

        if (t == 0)
        {
            Console.WriteLine("Total questions is 0 — cannot compute percentage.");
            continue;
        }

        int score = (r * 3) - f;
        int maxScore = t * 3;

        // Cast to float to avoid integer division
        Percentage = (float)score / maxScore * 100f;

        //Console.WriteLine($"Score: {score} / {maxScore}");
        Console.WriteLine($"Percentage: {Percentage:F2}%");
        if (f > 0)
        {
            OptimistPercentage = (float)r / t * 100;
            Console.WriteLine($"You could've gotten {OptimistPercentage:F2}% if you left your false answered questions unanswered!");
        }
        Console.WriteLine("\nPress Enter to calculate another or type 'exit' to quit.");
        if (Console.ReadLine()?.ToLower() == "exit")
            break;

    }
    catch (OverflowException)
    {
        Console.WriteLine("Error: Numbers are too large to process. Please try again with smaller values.\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An unexpected error occurred: {ex.Message}\n");
    }
}