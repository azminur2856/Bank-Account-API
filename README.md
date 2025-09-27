# Bank-Account-API

A comprehensive Bank Account API built using ASP.NET Web API for managing user accounts, transactions, and audit logging.

### Project Structure

The solution is organized into three main projects:

- **API**: The main project that exposes the Web API endpoints.
- **BLL** (Business Logic Layer): Contains the business logic of the application, services, and DTOs.
- **DAL** (Data Access Layer): Handles the data access logic using Entity Framework.

### Technologies Used

The following key technologies and NuGet packages are used in this project:

- **Framework**: ASP.NET Web API 5.2.9
- **ORM**: Entity Framework 6.5.1
- **JSON Handling**: Newtonsoft.Json 13.0.3
- **Hashing**: BCrypt.Net-Next 4.0.3
- **Mapping**: AutoMapper 10.0.0
- **Configuration**: DotNetEnv 3.1.1 (for loading `.env` configuration files)
- **Domain Verification**: DnsClient 1.8.0
- **Compiler**: Microsoft.CodeDom.Providers.DotNetCompilerPlatform 2.0.1

### Data Access

- **Database Context**: Uses `BANKContext` in the Data Access Layer (DAL).
- **Connection String**: The API is configured to connect to a SQL Server database. The default connection string in `Web.config` is:
    ```xml
    <add name="BANKContext" connectionString="server=AZMINUR-RAHMAN\SQLEXPRESS; initial catalog=Bank_Account_API_DB; integrated security=true;" providerName="System.Data.SqlClient" />
    ```

### Configuration

The application requires a `.env` file in the API project root for critical configuration.

Key environment variables to set include:
| Variable | Purpose |
|---|---|
| `SMTP_HOST`, `SMTP_PORT`, `SMTP_USER`, `SMTP_APP_PASSWORD` | Email service configuration |
| `SMS_API_URL`, `SMS_API_KEY`, `SMS_SENDER_ID` | SMS service configuration (for phone OTP) |
| `API_BASE_URL` | The base URL for generating verification links (e.g., `https://localhost:44364`) |
| `BANK_MASTER_ACCOUNT_NUMBER` | The account number of the bank's central master account |
| `TRANSFER_FEE` | The fee charged for a user-to-user fund transfer |
| `SEMI_ANNUAL_MAINTENANCE_FEE` | The fee charged for semi-annual account maintenance |

### API Endpoints

The base route template is `api/{controller}/{id}`.

| Controller/Route | Method | Description | Roles |
|---|---|---|---|
| **Auth** (`api/auth`) | | | |
| `/login` | `POST` | Authenticate and get an access token | No Auth |
| `/logout` | `GET` | Expire the current session token | `Logged` |
| `/requestpasswordreset` | `POST` | Send a password reset OTP to email or phone | No Auth |
| `/resetpassword` | `PUT` | Reset password using the OTP | No Auth |
| **User** (`api/user`) | | | |
| `/create` | `POST` | Register a new user | No Auth |
| `/verifyaccount/{token}` | `GET` | Activate account via email verification link (for registration) | No Auth |
| `/verifyemail/{token}` | `GET` | Verify user's email address | No Auth |
| `/changepassword` | `PUT` | Change user's password | `Logged` |
| `/sendphoneverificationotp` | `POST` | Send OTP for phone number verification | `Logged` |
| `/verifyphone` | `POST` | Verify phone number using OTP | `Logged` |
| `/address/add` | `POST` | Add a new address (Present or Permanent) | `Logged` |
| `/address/update/present` | `PUT` | Update the user's present address | `Logged` |
| `/address/all` | `GET` | Get all addresses for the current user | `Logged` |
| `/address/verify/{addressId}` | `POST` | Verify a user's address | `Admin, Employee` |
| `/change-role` | `PUT` | Change a user's role | `Admin` |
| `/change-status` | `PUT` | Activate or deactivate a user account | `Admin, Employee` |
| `/account/all` | `GET` | Get all accounts for the current user | `Logged` |
| `/profile` | `GET` | Get the current user's profile details, addresses, and accounts | `Logged` |
| **Account** (`api/account`) | | | |
| `/create` | `POST` | Create a new bank account for a user | `Admin, Employee, Customer` |
| `/activate/{accountNumber}` | `POST` | Activate a customer's inactive account | `Employee` |
| `/all` | `GET` | Get a list of all bank accounts in the system | `Admin, Employee` |
| `/collect-semi-annual-fee`| `POST` | Manually trigger the semi-annual account maintenance fee collection | `Admin` |
| **Transaction** (`api/transaction`) | | | |
| `/deposit` | `POST` | Deposit funds into a customer account (from master account) | `Employee` |
| `/withdrawal` | `POST` | Withdraw funds from an account (to master account) | `Employee, Customer` |
| `/transfer` | `POST` | Transfer funds between accounts (user-to-user) | `Employee, Customer` |
| `/system/credit` | `POST` | Credit the bank master account (system level credit) | `Admin` |
| `/statement` | `POST` | Generate an account statement for a date range | `Logged` |
| **Audit Logs** (`api/auditlogs`) | | | |
| `/all` | `GET` | Get all audit logs (with pagination/sorting) | `Admin, Employee` |
| `/bytype/{type}` | `GET` | Get logs filtered by `AuditLogType` enum | `Admin, Employee` |
| `/bydaterange/{startDate}/{endDate}` | `GET` | Get logs for a specified date range | `Admin, Employee` |
| `/byuseremail` | `POST` | Get logs for a specific user email | `Admin, Employee` |

***

### Getting Started

To get the project running, you will need to:

1.  **Configure Environment**: Create a `.env` file in the `API` project root and populate it with the required configuration variables listed above.
2.  **Restore Packages**: Restore the NuGet packages using the `packages.config` files.
3.  **Build**: Build the solution.
4.  **Database**: Ensure your database is up and running and accessible via the connection string in `API/Web.config`. Run any necessary Entity Framework migrations.
5.  **Run**: Run the project. The application is configured to run locally on IIS Express with SSL at `https://localhost:44364/`.

### License

This project is released under the **MIT License**.

Copyright (c) 2025 AZMINUR RAHMAN