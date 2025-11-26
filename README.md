# dotnet-interview / TodoApi

[![Open in Coder](https://dev.crunchloop.io/open-in-coder.svg)](https://dev.crunchloop.io/templates/fly-containers/workspace?param.Git%20Repository=git@github.com:crunchloop/dotnet-interview.git)

This is a simple Todo List API built in .NET 8. This project is currently being used for .NET full-stack candidates.

## Database

The project comes with a devcontainer that provisions a SQL Server database. If you are not going to use the devcontainer, make sure to provision a SQL Server database and
update the connection string.

To use your own Database, update the ConnectionStrings:TodoContext in appsettings.Development.json file.

## Migrations 

Commands:
- Run migrations: dotnet ef migrations add {migrationName} --project TodoApi.DataAccess --startup-project TodoApi
- Update database: dotnet ef database update --project TodoApi.DataAccess --startup-project TodoApi

## Build

To build the application:

`dotnet build`

## Run the API

To run the TodoApi in your local environment:

`dotnet run --project TodoApi`

## Test

To run tests:

`dotnet test`

Check integration tests at: (https://github.com/crunchloop/interview-tests)

## Contact

- Martín Fernández (mfernandez@crunchloop.io)

## About Crunchloop

![crunchloop](https://crunchloop.io/logo-blue.png)

We strongly believe in giving back :rocket:. Let's work together [`Get in touch`](https://crunchloop.io/contact).


## Developer notes

- To conect a frontend application to this API, you need to update the AllowedOrigins in appsettings.Development.json file to include the URL where your frontend application is running.

- The route to connect with the SignalR hub is {apiUrl}/hubs/todos.

- To use your own Database, update the ConnectionStrings:TodoContext in appsettings.Development.json file.

## The requested functionalities were implemented:
- CRUD for Todos and TodoLists

## The following tools and principles were used:
- BackgroundServices, implemented in the Infrastructure package
- SignalR, implemented in the Hubs and Infrastructure packages
- SOLID principles, with emphasis on SRP, OCP, ISP (internal interfaces), and DIP.
- GRASP principles
- Repository pattern
