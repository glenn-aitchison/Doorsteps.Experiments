/***************************************************************************************************
 * 
 * Filename:    Program.cs 
 * Description: Implemenation - Starts the Experiments Website.
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
builder.Services.AddControllersWithViews();

// Build Experiments Web Application
myWebApp = builder.Build();                  // Build application
myWebApp.UseHttpsRedirection();              // Use HTTPS
myWebApp.UseStaticFiles();                   // Allow static files to be used
myWebApp.UseRouting();                       // Use routing
myWebApp.MapControllers();                   // Map Web API Controllers

myWebApp.MapControllerRoute(
    name: "default",
    pattern: "{controller=Experiments}/{action=ListExperiments}");


// Run application
myWebApp.Run();
