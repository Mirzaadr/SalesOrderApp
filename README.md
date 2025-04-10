# Sales Order App
A Simple app that shows Sales order and the items associated with it. This web-based application allows users to create, edit, and manage customer sales orders. Built with ASP.NET Core MVC and Bootstrap, the system supports dynamic item entry within orders, inline editing with locking, and manual pagination for browsing records. Key features include customer selection, real-time calculation of totals, confirmation modals for critical actions, and clean separation of logic using services.
[Prototype](https://www.figma.com/proto/e9mRWZeWvsMgM0y6EWbf5Q/Test-Developer?node-id=16-1153&amp%3Bnode-type=CANVAS&amp%3Bt=8FOLfvYSI01NvHGn-1&amp%3Bscaling=scale-down&amp%3Bcontent-scaling=fixed&amp%3Bpage-id=0%3A1&amp%3Bstarting-point-node-id=1%3A4)

## Getting Started
- Use database schema ```So Schema.Sql``` to setup the database
- Update the connection string inside the ```appsettings.json```
  json```
  {
  ...
    "ConnectionStrings": {
      "SalesDB": "Data Source=<YOUR_DATA_SOURCE>;Database=<DATABASE_NAME>;uid=<YOUR_USER>;Pwd=<PASSWORD>;TrustServerCertificate=True;"
    }
  }
  ```
- ```Ctrl+F5``` to run the app in Visual Studio, or run ```dotnet run``` from command line

The published app can be found [here](https://github.com/Mirzaadr/SalesOrderApp/releases/tag/dotnet).
To run the published version simply update the connection string inside ```appsettings.json``` and publish the files to IIS.

## Tech
- ASP.NET Core 6.0
- Bootstrap
