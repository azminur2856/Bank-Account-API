# Bank-Account-API

Bank Account API using ASP.NET

### Technologies Used

The following key technologies and packages are used in this project:

- **ASP.NET Web API**: Provides the framework for building the HTTP services. The API version is 5.2.9.
- **Newtonsoft.Json**: A popular high-performance JSON framework for .NET. The version used is 13.0.3.
- **Microsoft.CodeDom.Providers.DotNetCompilerPlatform**: A package that provides runtime C# and Visual Basic compilers. The version used is 2.0.1.

### Project Structure

The solution is organized into three main projects:

- **API**: The main project that exposes the Web API endpoints.
- **BLL** (Business Logic Layer): Contains the business logic of the application.
- **DAL** (Data Access Layer): Handles the data access logic.

### API Endpoints

The API is configured with a default route template of `api/{controller}/{id}`. The `id` parameter is optional. Specific endpoints will be implemented within the `Controllers` folder of the `API` project.

### Getting Started

To get the project running, you will need to:

1.  Restore the NuGet packages using the `packages.config` file.
2.  Build the solution.
3.  Run the project. The application is configured to run locally on IIS Express with SSL at `https://localhost:44364/`.
