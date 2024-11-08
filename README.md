<pre>  _____       _           _    _       _     
 |  __ \     (_)         | |  | |     | |    
 | |  | |_ __ ___   _____| |__| |_   _| |__  
 | |  | | '__| \ \ / / _ \  __  | | | | '_ \ 
 | |__| | |  | |\ V /  __/ |  | | |_| | |_) |
 |_____/|_|  |_| \_/ \___|_|  |_|\__,_|_.__/ 
                                             
                                             </pre>
# DriveHub
DriveHub is a comprehensive vehicle reservation and management system built on a robust technological stack to ensure high performance, scalability, and security. This application enables users to book vehicles, manage reservations, handle payments, and much more.

## Table of Contents

- [Applications](#applications)
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Installation](#installation)
- [Configuration](#configuration)
- [Usage](#usage)
- [Running Tests](#running-tests)

## Applications

- DriveHub: The customer facing website hosted at https://drivehub.au
- Admin: The administrative portal hosted at https://portal.drivehub.au

## Features

- **Vehicle Booking**: Book and manage vehicle reservations seamlessly.
- **User Management**: Role-based access control for different user types.
- **Payment Processing**: Secure payment handling using Stripe.
- **Email Notifications**: Send email notifications using SendGrid.
- **PDF Generation**: Generate PDF documents for invoices and reports.
- **Responsive Design**: User-friendly interface using Bootstrap 5.3.3.
- **Error Handling**: Comprehensive error handling and logging.

## Technologies Used

- **C# & ASP.NET Core MVC**: For backend development.
- **Azure Cloud Services**: For hosting and cloud infrastructure.
- **Entity Framework**: For ORM and database interactions.
- **QuestPDF**: For generating PDF documents.
- **xUnit**: For unit testing.
- **Stripe.net**: For payment processing.
- **SendGrid**: For email services.
- **SQL Server 2022**: For data storage, integrated with ASP Identity.
- **GitHub Repository**: For version control and collaboration.
- **GitHub Actions**: For CI/CD pipelines.
- **Visual Studio 2022 & VS Code**: For development and coding.
- **Azure Data Studio**: For database management.
- **Azure Storage Explorer**: For managing Azure Storage.
- **Bootstrap 5.3.3**: For responsive front-end design.

## Installation

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/your-repo/drivehub.git
   cd drivehub
   ```
2. **Install Dependencies**: Ensure you have the required .NET SDK and other dependencies installed.

3. **Configure the Databases**: Update the appsettings.json with your SQL Server connection string.
    ```json
    "ConnectionStrings": {
        "DriveHub": "Server=your_server;Database=your_db;User Id=your_user;Password=your_password;"
    }
    ```

4. **Run Migrations**: Apply the database migrations to set up the database schema.

    ```bash
    dotnet ef database update
    ```

5. **Build and Run the Application(s)**:

    ```bash
    dotnet build
    dotnet run
    ```

## Configuration

**Azure Services**: Set up your Azure services (App Services, Storage, etc.) and update the configurations in appsettings.json.

**SendGrid**: Add your SendGrid API key to the configuration.

**Stripe**: Add your Stripe API keys to the configuration.

## Usage

**Register and Login**:

Users can register and log in to the system.

**Book a Vehicle**:

Navigate to the booking page to search and book vehicles.

**Manage Reservations**:

View and manage your reservations, extend or cancel bookings.

**Process Payments**:

Securely process payments using Stripe.

**Generate Reports**:

Generate and download PDF reports and invoices.

## Running Tests
To run the tests, use the following command:

```bash
dotnet test
```

This will execute all the unit tests using xUnit.

Made with ❤️ by the DriveHub Team
