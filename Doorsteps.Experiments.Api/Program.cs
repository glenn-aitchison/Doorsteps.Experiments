/***************************************************************************************************
 * 
 * Filename:    Program.cs 
 * Description: Implemenation - Starts the Experiments APIs.
 * 
 * Author:      Glenn Aitchison
 * Date:        29-Mar-2022
 * 
 ***************************************************************************************************/

// Include Spring libraries we need 
using Spring.Context.Support;

// Setup variables 
WebApplication        myWebApp;              // Web Application
WebApplicationBuilder builder;               // Build an application

// Register Spring configuration file with application and use that to inject dependancies
ContextRegistry.RegisterContext(new XmlApplicationContext("Config/spring.xml"));

// Get ready to create web application
builder = WebApplication.CreateBuilder(args);

// Add controller and views
builder.Services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Build Experiments Web Application
myWebApp = builder.Build();                  // Build application
//myWebApp.UseHttpsRedirection();            // Use HTTPS
myWebApp.MapControllers();                   // Map Web API Controllers

// Use swagger to test apis
myWebApp.UseSwagger(); myWebApp.UseSwaggerUI();

// Run application
myWebApp.Run();
