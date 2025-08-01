# Secret Santa Game

A C# application for automating Secret Santa assignments in corporate environments. This application ensures fair distribution while respecting constraints from previous years' assignments.

## Solution Overview

The Secret Santa Game is a modular, object-oriented application that automates the process of assigning secret children to employees for holiday gift exchanges. The system prevents employees from being assigned to themselves or to the same person they were assigned to in the previous year.

### Key Features

- **Automated Assignment**: Intelligent assignment algorithm ensuring each employee gets exactly one secret child
- **Previous Year Constraints**: Prevents employees from being assigned to the same person as the previous year
- **Duplicate Handling**: Automatically removes duplicate employee entries
- **Comprehensive Error Handling**: Robust error handling for file operations and data validation
- **Modular Architecture**: Clean, extensible codebase following SOLID principles
- **CSV Support**: Reads from and writes to CSV files for easy data management

### Architecture

The application follows object-oriented programming principles with a modular design:

- **Models**: `Employee` and `SecretSantaAssignment` classes represent the core data structures
- **Services**: `CsvFileProcessor` handles file operations, `SecretSantaAssigner` contains assignment logic
- **Interfaces**: `IFileProcessor` and `ISecretSantaAssigner` provide abstraction for extensibility
- **Exception Handling**: Custom `SecretSantaException` for application-specific errors
- **Game Manager**: Orchestrates the entire workflow

## Installation

### Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022 or VS Code with C# extension
- Git for version control

### Setup Instructions

1. Clone the repository:
```bash
git clone https://github.com/gowdhamnedunhezhiyan/secretsantagame.git
cd secret-santa-game
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Build the solution:
```bash
dotnet build
```

4. Run tests to verify installation:
```bash
dotnet test
```

## Running the Program

### Command Line Usage

Basic usage with default file names:
```bash
dotnet run
```

Specify custom file paths:
```bash
dotnet run -- [employee_file] [output_file] [previous_assignments_file]
```

### Parameters

1. **Employee File** (default: `employees.csv`): CSV file containing employee information
2. **Output File** (default: `secret_santa_assignments.csv`): Generated CSV file with assignments
3. **Previous Assignments File** (default: `previous_assignments.csv`): Optional file with last year's assignments

### Input File Format

#### Employee File (employees.csv)
```csv
Employee_Name,Employee_EmailID
John Doe,john.doe@acme.com
Jane Smith,jane.smith@acme.com
Bob Johnson,bob.johnson@acme.com
Alice Brown,alice.brown@acme.com
```

#### Previous Assignments File (previous_assignments.csv)
```csv
Employee_Name,Employee_EmailID,Secret_Child_Name,Secret_Child_EmailID
John Doe,john.doe@acme.com,Jane Smith,jane.smith@acme.com
Jane Smith,jane.smith@acme.com,Bob Johnson,bob.johnson@acme.com
```

### Output File Format

The application generates a CSV file with the following structure:
```csv
Employee_Name,Employee_EmailID,Secret_Child_Name,Secret_Child_EmailID
John Doe,john.doe@acme.com,Alice Brown,alice.brown@acme.com
Jane Smith,jane.smith@acme.com,John Doe,john.doe@acme.com
Bob Johnson,bob.johnson@acme.com,Jane Smith,jane.smith@acme.com
Alice Brown,alice.brown@acme.com,Bob Johnson,bob.johnson@acme.com
```

## Understanding the Solution

### Assignment Algorithm

The application uses an intelligent assignment algorithm that:

1. **Validates Input**: Ensures minimum 2 unique employees are present
2. **Maps Constraints**: Creates lookup for previous year assignments to avoid repetition
3. **Randomizes Order**: Shuffles employee list to ensure fair, random assignments
4. **Assigns Secret Children**: Iteratively assigns while respecting all constraints
5. **Retry Logic**: Attempts up to 1000 times to find valid assignments if constraints are tight

### Error Handling

The application provides comprehensive error handling for common scenarios:

- **File Not Found**: Clear error messages with file paths
- **Invalid Data Format**: Detailed validation errors with line numbers
- **Insufficient Employees**: Minimum employee count validation
- **Assignment Constraints**: Graceful handling when valid assignments cannot be created

### Extensibility

The modular design allows for easy extension:

- **Custom Assignment Logic**: Implement `ISecretSantaAssigner` interface
- **Different File Formats**: Implement `IFileProcessor` interface
- **Additional Constraints**: Extend existing classes with custom rules

## Testing

The solution includes comprehensive unit and integration tests:

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

Test coverage includes:
- Model validation and behavior
- File processing operations
- Assignment algorithm logic
- Error handling scenarios
- End-to-end integration tests

## Additional Information

### Requirements Met

1. **Modularity and Extensibility**: Object-oriented design with clear separation of concerns
2. **Tests**: Comprehensive test suite with unit and integration tests
3. **Error Handling**: Robust exception handling throughout the application
4. **Documentation**: Complete README with installation and usage instructions
5. **Version Control**: Designed for Git-based version control systems

### Project Structure

```
SecretSantaGame/
├── SecretSantaGame/
│   ├── Models/
│   ├── Interfaces/
│   ├── Services/
│   ├── Exceptions/
│   ├── SecretSantaGameManager.cs
│   └── Program.cs
├── SecretSantaGame.Tests/
└── README.md
```

### Performance

The application is optimized for performance with efficient algorithms and minimal memory usage, suitable for organizations with thousands of employees.

## Error Handling

The application provides comprehensive error handling for common scenarios:

- **File Not Found**: Clear error message with file path
- **Invalid Data Format**: Detailed validation errors with line numbers
- **Insufficient Employees**: Minimum employee count validation
- **Assignment Constraints**: Graceful handling when valid assignments cannot be created
- **File Permission Issues**: Clear error messages for file access problems

### Common Error Messages

```
❌ Secret Santa Error: At least 2 employees are required for Secret Santa.
❌ Secret Santa Error: Employee file not found: employees.csv
❌ Secret Santa Error: Invalid data format at line 3: John Doe
❌ Secret Santa Error: Unable to create valid Secret Santa assignments after 1000 attempts.
```

## 🎯 Examples

### Example 1: Basic Usage

```bash
# Create employee file
echo "Employee_Name,Employee_EmailID
John Doe,john.doe@acme.com
Jane Smith,jane.smith@acme.com
Bob Johnson,bob.johnson@acme.com" > employees.csv

# Run the application
dotnet run -- employees.csv assignments.csv

# Output: assignments.csv with valid Secret Santa assignments
```

### Example 2: With Previous Year Constraints

```bash
# Create previous assignments file
echo "Employee_Name,Employee_EmailID,Secret_Child_Name,Secret_Child_EmailID
John Doe,john.doe@acme.com,Jane Smith,jane.smith@acme.com" > previous.csv

# Run with constraints
dotnet run -- employees.csv new_assignments.csv previous.csv
```

### Example 3: Programmatic Usage

```csharp
using SecretSantaGame;
using SecretSantaGame.Models;
using SecretSantaGame.Services;

// Create employees
var employees = new List<Employee>
{
    new Employee("John Doe", "john.doe@acme.com"),
    new Employee("Jane Smith", "jane.smith@acme.com"),
    new Employee("Bob Johnson", "bob.johnson@acme.com")
};

// Create assigner and generate assignments
var assigner = new SecretSantaAssigner();
var assignments = assigner.AssignSecretChildren(employees);

// Display results
foreach (var assignment in assignments)
{
    Console.WriteLine($"{assignment.Employee.Name} → {assignment.SecretChild.Name}");
}
```

## 🧩 Extensibility

### Adding Custom Constraints

You can extend the assignment logic by creating custom constraint validators:

```csharp
public class DepartmentConstraintAssigner : SecretSantaAssigner
{
    private readonly Dictionary<string, string> _employeeDepartments;

    public DepartmentConstraintAssigner(Dictionary<string, string> departments)
    {
        _employeeDepartments = departments;
    }

    protected override bool IsValidAssignment(Employee employee, Employee potentialChild, 
        Dictionary<string, string> previousAssignmentLookup)
    {
        // Call base validation first
        if (!base.IsValidAssignment(employee, potentialChild, previousAssignmentLookup))
            return false;

        // Add department constraint (optional: avoid same department)
        var employeeDept = _employeeDepartments.GetValueOrDefault(employee.EmailId);
        var childDept = _employeeDepartments.GetValueOrDefault(potentialChild.EmailId);
        
        return employeeDept != childDept;
    }
}
```

### Adding Notification Features

```csharp
public class EmailNotificationGameManager : SecretSantaGameManager
{
    private readonly IEmailService _emailService;

    public EmailNotificationGameManager(IEmailService emailService, 
        IFileProcessor fileProcessor, ISecretSantaAssigner assigner) 
        : base(fileProcessor, assigner)
    {
        _emailService = emailService;
    }

    public override void RunSecretSantaGame(string employeeFilePath, 
        string outputFilePath, string previousAssignmentsFilePath = null)
    {
        base.RunSecretSantaGame(employeeFilePath, outputFilePath, previousAssignmentsFilePath);
        
        // Send notifications after assignments are created
        var assignments = ReadAssignmentsFromFile(outputFilePath);
        SendNotifications(assignments);
    }
}
```

## 🔒 Security Considerations

- **Data Privacy**: Employee information is only processed locally
- **File Access**: Validates file paths to prevent directory traversal
- **Input Validation**: Comprehensive validation of all input data
- **Error Messages**: Avoid exposing sensitive system information

### Development Setup

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/new-feature`
3. Make your changes and add tests
4. Ensure all tests pass: `dotnet test`
5. Run code formatting: `dotnet format`
6. Commit your changes: `git commit -am 'Add new feature'`
7. Push to the branch: `git push origin feature/new-feature`
8. Create a Pull Request

### Code Style Guidelines

- Follow [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions)
- Use meaningful variable and method names
- Add XML documentation for public APIs
- Maintain test coverage above 90%
- Follow SOLID principles

### Testing Guidelines

- Write unit tests for all public methods
- Include integration tests for end-to-end scenarios
- Use descriptive test method names following AAA pattern (Arrange, Act, Assert)
- Mock external dependencies
- Test both happy path and error scenarios

## 📝 Changelog

### Version 1.0.0 (2025-08-01)
- Initial release
- Core Secret Santa assignment functionality
- CSV file support
- Comprehensive error handling
- Full test suite
- Documentation
