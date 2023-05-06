using EGNLogic;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

var menuStr = $"1. Generate EGN {Environment.NewLine}2. Check EGN {Environment.NewLine}3. Describe EGN {Environment.NewLine}0. Exit {Environment.NewLine}Awaiting input: ";

while (true)
{
    Console.Write(menuStr);
    int.TryParse(Console.ReadLine(), out var menuInput);
    switch (menuInput)
    {
        case 1: Generate();
            Wait();
            break;
        case 2: Check();
            Wait();
            break;
        case 3: Parse();
            Wait();
            break;
    }
    if (menuInput == 0)
    {
        break;
    }
}

void Parse()
{
    Console.Clear();
    Console.Write("Insert EGN: ");
    var egn = Console.ReadLine();
    EGNGenerator.TryParse(egn, out var output);
    Console.WriteLine(output.ToString());
}

void Generate()
{
    Console.Clear();
    Console.Write("Insert date of birth (mm/dd/yyyy): ");
    DateOnly.TryParse(Console.ReadLine(), out var date);
    Console.Write("Choose gender: ");
    int.TryParse(Console.ReadLine(), out var gender);
    Console.Write("Choose region: ");
    int.TryParse(Console.ReadLine(), out var region);
    var generator = new EGNGenerator(date, (Gender)gender, (Region)region);
    Console.WriteLine(generator.GenerateEGN());
}

void Check()
{
    Console.Clear();
    Console.Write("Insert EGN: ");
    var egn = Console.ReadLine();
    Console.WriteLine(EGNGenerator.CheckValidity(egn) == true ? "EGN is valid" : "EGN is invalid");
}

void Wait()
{
    Console.WriteLine("Press any key to continue...");
    Console.ReadKey();
    Console.Clear();
}
