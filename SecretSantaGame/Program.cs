using SecretSantaGame;
using SecretSantaGame.Exceptions;

try
{
    Console.WriteLine("🎅 Welcome to the Secret Santa Game! 🎄");
    Console.WriteLine();

    // Parse command line arguments or use default values
    var employeeFile = args.Length > 0 ? args[0] : "employees.csv";
    var outputFile = args.Length > 1 ? args[1] : "secret_santa_assignments.csv";
    var previousAssignmentsFile = args.Length > 2 ? args[2] : "previous_assignments.csv";

    // Display configuration
    Console.WriteLine("Configuration:");
    Console.WriteLine($"  Employee File: {employeeFile}");
    Console.WriteLine($"  Output File: {outputFile}");
    Console.WriteLine($"  Previous Assignments: {previousAssignmentsFile}");
    Console.WriteLine();

    // Run the Secret Santa game
    var gameManager = new SecretSantaGameManager();
    gameManager.RunSecretSantaGame(employeeFile, outputFile, previousAssignmentsFile);

    Console.WriteLine();
    Console.WriteLine("###Secret Santa Game completed successfully!");
}
catch (SecretSantaException ex)
{
    Console.WriteLine($"\n{ex.Message}");
    Environment.Exit(1);
}
catch (Exception ex)
{
    Console.WriteLine($"\nAn unexpected error occurred: {ex.Message}");
    Environment.Exit(1);
}

if (args.Length == 0)
{
    Console.WriteLine("\nPress any key to exit...");
    Console.ReadKey();
}
