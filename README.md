# Build an App with Asp.net core and Angular
## Section 2 Creating Skeleton

### 6 Creating the .net api using cli
* Create Directory
    ```
    mkdir DatingApp
    ```

* Create solution
    ```
    dotnet new sln
    ```

* Create Api project

    ```
    dotnet new webapi -o Api
    ```

* Add Api project into Solution

    ```
    dotnet sln add Api
    ```

* Add .gitignore file

    ```
    dotnet new gitignore
    ```

* Trust development certificates

    ```
    dotnet dev-certs https --trust
    ```

    - To remove existing https dev certs

        ```
        dotnet dev-certs https clean
        ```
* Run dotnet project 
    ```
    dotnet run
    ```

    - using specific profile
        ```
        dotnet run -lp <profile-name>
        ```

    - using watch mode
        It has some issues so doesn't always work properly.
        ```
        dotnet run watch
        ```

* Restore dotnet packages

    Run this command after adding or removing packages from dotnet projects
    ```
    dotnet restore
    ```

* Install dotnet-ef tool for migrations
    for install command search `nuget dotnet-ef`

    ```
    dotnet tool install --global dotnet-ef --version 7.0.2
    ```

    - To update existing tools 
    ```
    dotnet tool update --global dotnet-ef
    ```

    - To List installed tools
    ```
    dotnet tool list -g
    ```

* Run EF tool
    ```
    dotnet ef
    ```

### 7 Setting VSCode for C#

1. Extensions

    * Install `C#` extension

    * Generate Assets 
        use `cmd + shift + P` and type asset and execute `.Net Build Assets for build and debug` command

    * Install `C# Extensions` extension

2. Preferences
    * Exclude
        - `**/bin`
        - `**/obj`

### 8 Getting to know API project files

1. `Controllers` (folder): This folder contains api endpoint files. This files handles http requests and sends appropriate response back.

2.  `Properties/launchSettings.json`: This file contains profile settings for the application. e.g. http profile sets applicationUrl. While running the dotnet application we can specify the profile to use. By default dotnet run will use first profile (http in our case) in launchSettings.json. 
    `dotnet run -lp <profile-name>` can be used to specify profile. eg. `dotnet run -lp https`

    Changed `launchSettings.json` to following
    ```
    {
        "$schema": "https://json.schemastore.org/launchsettings.json",
        "profiles": {
            "Api": {
                "commandName": "Project",
                "dotnetRunMessages": true,
                "launchBrowser": true,
                "launchUrl": "swagger",
                "applicationUrl": "https://localhost:5001;http://localhost:5000",
                "environmentVariables": {
                    "ASPNETCORE_ENVIRONMENT": "Development"
                }
            }
        }
    }
    ```

3. `Api.csproj`: 
    
    Contains properties such as target framework and package references etc...

    Removed `ItemGroup` node with all package references.

4. `appsettings.Development.json` , `appsettings.json`: specify application settings. In development version changed value for `Microsoft.AspNetCore` to `Information` from `Warning` to get more information in logs.

5. `Program.cs`: 

This file is entry point to the application. 

It contains services and middleware calls.

Generally lines before `var app = builder.Build();` are services related calls and after are middleware.

Since this course is not using swagger calls to add swagger service and middleware in development environment are removed.

Also the author is mainly using https only so middleware call to `app.UseHttpsRedirection();` is also removed.

Middleware `app.UseAuthorization();` is related to authentication. Its removed for now and will be added when doing authentication.

<!-- Copied files provided in resources to api project.

Main difference in classic hosting model is that 
in Program.cs it doesn't Main class

    - Api.csproj
        * ImplicitUsings : some common usings are automatically generated and stored in Api.GlobalUsings.g.cs
        * Nullable: To mark types to be Nullable.
    - launchSettings.json
        profiles > Api (name of project) > applicationUrl > Updated port number for http and https instead of it being random. -->


<!-- ### 9
   
Explained how api project starts. LaunchSettings.json, StartUp.cs and little bit about Route. -->

### 9 Creating Our First Entity

1. Created a folder `Entities` in Api folder
2. Using extension created new C# -> Class `AppUser`. 

- Deleted un used usings. Added two prop 
    
    * Id (int)
    * UserName (string)


- `Api.csproj`

    **Nullable Strings**

    Since traditionally strings are always Nullable in C# above property shows wiggly line to indicate that the property is nullable and to use Nullable operator (`?`). Another option is to disable Nullable from `Api.csproj` file. This option is more preferred since it makes strings non nullable and we can always check if string has a value or not.

    **Implicit Usings**

    From global usings in obj -> Debug folder usings are implicitly imported. This makes it possible to not include usings in every file. This setting is controlled by `ImplicitUsings` property in csproj file.

### 10 Introduction to Entity Framework
- What is it?
An Object Relational Mapper (ORM). Translates code in SQL commands that update our tables in the db.

- EF Features
    * Querying: 
        Allows to query db using Linq queries.
    * Change Tracking:
        Keeps track of changes occurring in entities. 
    * Saving: 
        Executes CRUD operations using DBContext class.
    * Concurrency: 
        Uses optimistic concurrency by default to protect over writing changes made by another user since data was fetched from db.
    * Transactions: 
        Provides automatic transaction management while querying or saving data.
    * Caching:
        Includes first level caching out of the box so repeated querying would return data out of the cache instead of hitting the db.
    * Built-in Conventions:
        Provides set of default rules to configure EF schema.
    * Configurations:
        Provides way to configure the entities to override the conventions.
    * Migrations:
        Provides ability to create db schema to automatically create tables in db server and make changes to schema based on changes in the Entity. This is called code first db creation.

### 11 Add EF to Project
- NuGet Gallery: install NuGet Gallery extension in vscode.
- command + shift + p open NuGet Gallery and install package. Match version to you dotnet framework version. 
    * `Microsoft.EntityFrameworkCore.Sqlite`
    * `Microsoft.EntityFrameworkCore.Design`

### 12 Adding DbContext
- create Data folder under api project
- Create DataContext class overriding DbContext and add public prop of type DbSet<AppUser> Users for creating users table in database. Data > DataContext.cs looks like below

    ```
    using Api.Entities;
    using Microsoft.EntityFrameworkCore;

    namespace Api.Data
    {
        public class DataContext : DbContext
        {
            public DataContext(DbContextOptions options) : base(options)
            {
            }

            public DbSet<AppUser> Users { get; set; }
        }
    }
    ```
- In Program.cs added DbContext of type DataContext using connection string from app settings.
Added following code before `var app = builder.Build();` 

    ```
    builder.Services.AddDbContext<DataContext>(opt =>
    {
        opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    });
    ```

### 13 Connection String
- Added connection string in appsettings.Development.json
following lines were added after `Logging` node

```
  "ConnectionStrings": {
    "DefaultConnection": "Data source=datingApp.db"
  }
```
### 14 Creating Code First EF Migrations

- Installed dotnet-ef using command 

    ```dotnet tool install --global dotnet-ef --version 7.0.2```

- Created initial migration with command 

    ```dotnet ef migrations add InitialCreate -o Data/Migrations```

- Created new database with 

    ```dotnet ef database update```

- Installed SQLite VScode Extension
- Inserted data into Users table using the SQLite extension.

### 16 Add new controller

- Added Users controller at route `api/Users` & `api/Users/{id}`
    * controllers are always pluralized.
    * It's a convention to use `Controller` as suffix for each controller class name. eg. `UsersController`
    * Controller class should derive from `ControllerBase`
- Attribute `[ApiController]` on top of class declares the class as controller class
- Attribute `[Route("api/[controller]")]` specifies the route starts with api and then followed by name of the Controller class. In our case users.

### 17 Making code asynchronous

- It is important to make database calls asynchronously that would help dotnet to reuse the thread to serve other requests while current operation is waiting for data from the database. This is makes the app scalable when there are a lot of concurrent users and the operations are taking some time getting data from db.

- added `async` to function definition
- Changed return type to `Task<return type>`
- added `await` front of async call
- used async function for database calls eg. 
    ```
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
            return await _context.Users.FindAsync(id);
        }
    ```

## Section 3 Creating Skeleton
### 20 Creating Angular App
    - install angular cli using 

        ```
        yarn global add @angular/cli@14
        ```

    - created new angular app called client using 

        ```
        ng new client
        ```
    
    - Selected yes for routing, css for styles
### 23 Making Http requests in Angular
    - Added HttpClientModule from @angular/common/http in imports list of App Module
    - App component created constructor method and declared private http object of type HttpClient in params of constructor. This created a private prop in the class 

    ```
      constructor(private http: HttpClient){}
    ```
    
    - declared prop users of type any
    - Updated App class to implement OnInit interface from @angular/core
    - Overridden ngOnInit method to make request to users list from users api endpoint. Subscribed to returned observable and on success store the resp in users list, on error log the error on console and at completion of request log message request complete

    ```
    ngOnInit(): void {
        this.http.get(`https://localhost:6001/api/users`).subscribe({
        next: (resp) => this.users = resp,
        error: (err) => console.log({getUserErr: err}),
        complete: () => console.log('Get users request completed.')
        })
    }  
    ```

### 23 Making Http requests in Angular
    - fixed CORS errors by trusting angular client url in app

    - Added following CORS policy 
        ```
        var app = builder.Build();

        // CORS policy
        app.UseCors(corsPolicyBuilder => corsPolicyBuilder.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200"));

        // Configure the HTTP request pipeline.
        app.MapControllers();
        ```