/***************************************************************************************************
 * 
 * Filename:    ExperimentDaoTests.cs 
 * Description: Implementation - Test the Experiment DAOs (Data Access Objects) to verify they 
 *                               are working correctly. 
 * 
 * Author:      Glenn Aitchison
 * Date:        29-Mar-2022
 * 
 ***************************************************************************************************/

// Include Microsoft libraries we need
using System;
using System.IO;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Include Selenium libraries we need
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Doorsteps.Experiments.Web.Tests
{
     /// <summary>
     ///    Test the website by automatically click on buttons, filling out forms, and navigating the website.
     ///    NOTE: Please make sure you run 'dotnet dev-certs https --trust' from a command prompt in 'Doorsteps.Experiments\Doorsteps.Experiments.Api' folder to trust unsigned DEV SSL certs
     /// </summary>
     [TestClass]
     public class ExperimentWebTests
     {
          #region Methods

               #region Setup and Initialization

                     // Setup variables
                    private IWebDriver?  _chromeWebBrowser = null;
                    private String       _hostName         = "https://localhost:44363";
                    private Process      _experimentsApi   = new Process(), _experimentsWebApp = new Process();

                    /// <summary>
                    ///    Setup testing.
                    /// </summary>
                    [TestInitialize]
                    public void Setup()
                    {
                         // Setup variables
                         DirectoryInfo? currentDir = Directory.GetParent(Environment.CurrentDirectory),            // Current directory
                                        parentDir1 = null,                                                         // Directory one level above current directory
                                        parentDir2 = null;                                                         // Directory two levels above current directory
                         
                         // Get parent directory up two levels 
                         if (currentDir != null) parentDir1 = currentDir.Parent;
                         if (parentDir1 != null) parentDir2 = parentDir1.Parent;

                         // Return error if we cannot load the chrome driver - used to test the website with Chrome web browser
                         if (parentDir2 == null) throw new Exception("Unable to determine where the chrome web driver is.");

                         // Run the Experiment Web App and APIs
                         RunWebAppAndApi();

                         // Connect testing up to the Chrome web browser
                         _chromeWebBrowser = new ChromeDriver(parentDir2.FullName + @"\drivers");           
                    }  // End Setup

                    /// <summary>
                    ///    Cleanup after testing.
                    /// </summary>
                    [TestCleanup]
                    public void Cleanup()
                    { StopWebAppAndApi(); }

               #endregion

               #region Common Functionality

                    /// <summary>
                    ///    Run web application and api.
                    /// </summary>
                    public void RunWebAppAndApi()
                    {
                         // Setup variables
                         Process experimentsApi = new Process(), experimentsWebApp = new Process();

                         // Start Experiments Api
                         _experimentsApi.StartInfo.FileName = "dotnet";
                         _experimentsApi.StartInfo.Arguments = "run";
                         _experimentsApi.StartInfo.WorkingDirectory = @"..\..\..\..\Doorsteps.Experiments.Api";
                         _experimentsApi.Start();

                         // Start Experiments Web App
                         _experimentsWebApp.StartInfo.FileName = "dotnet";
                         _experimentsWebApp.StartInfo.Arguments = "run";
                         _experimentsWebApp.StartInfo.WorkingDirectory = @"..\..\..\..\Doorsteps.Experiments.Web";
                         _experimentsWebApp.Start();

                         // Wait for web application and api to start
                         System.Threading.Thread.Sleep(25000);
                    }  // End RunWebAppAndApi

                    /// <summary>
                    ///    Stop web application and api.
                    /// </summary>
                    public void StopWebAppAndApi()
                    {
                         // Stop Experiments Api
                         _experimentsApi.Kill();

                         // Stop Experiments Api
                         _experimentsWebApp.Kill();

                         // Wait for web application and api to start
                         System.Threading.Thread.Sleep(5000);
                    }  // End StopWebAppAndApi

               #endregion

          #endregion

          #region Tests 

               /// <summary>
               ///    Test if Test Experiment 2 can be disabled/enabled.
               /// </summary>
               [TestMethod]
               public void Toggle_DisableEnable_Test_Experiment_2()
               {
                    // Arrange
                    if (_chromeWebBrowser == null) throw new Exception("Unable to determine where the chrome web driver is.");

                    // Act
                    _chromeWebBrowser.Navigate().GoToUrl(_hostName);                                // Open website
                    _chromeWebBrowser.FindElement(By.Id("z0__IsDisabled")).Click();                 // Tick Experiment 1 disabled box
                    _chromeWebBrowser.FindElement(By.Id("UpdateExperiments")).Click();              // Click on Update Experiments button

                    // Assert
                    Assert.AreEqual(true, _chromeWebBrowser.FindElement(By.TagName("div")).Text.IndexOf("Thank you for updating the status of the experiments") > -1);
               }  // End Toggle_DisableEnable_Test_Experiment_2

               
               /// <summary>
               ///    Test if responses can be submitted for Test Experiment 2.
               /// </summary>
               [TestMethod]
               public void Submit_Responses_To_Test_Experiment_2()
               {
                    // Arrange
                    if (_chromeWebBrowser == null) throw new Exception("Unable to determine where the chrome web driver is.");

                    // Act
                    _chromeWebBrowser.Navigate().GoToUrl(_hostName);                                                                   // Open website

                    _chromeWebBrowser.FindElement(By.Id("Test-Experiment-2")).Click();                                                 // Click on Test Experiment 2 

                    _chromeWebBrowser.FindElement(By.Id("StandardQuestionsToAsk_0__Answer")).SendKeys("Joe");                          // Fill out name field
                    _chromeWebBrowser.FindElement(By.Id("StandardQuestionsToAsk_1__Answer")).SendKeys("someone@example.com");          // Fill out email field
                    _chromeWebBrowser.FindElement(By.Id("StandardQuestionsToAsk_2__Answer")).SendKeys("1234");                         // Fill out phone field

                    (new SelectElement(_chromeWebBrowser.FindElement(By.Id("StandardQuestionsToAsk_3__Answer")))).SelectByText("No");  // Set do you play sports to 'No'

                    _chromeWebBrowser.FindElement(By.Id("CustomQuestionsToAsk_0__Answer")).SendKeys("Typing, and Computers");          // Set hobbies to Typing, and Computers
                    
                    _chromeWebBrowser.FindElement(By.Id("SubmitResponses")).Click();                                                   // Click on Update Experiments button

                    // Assert
                    Assert.AreEqual(true, _chromeWebBrowser.FindElement(By.TagName("div")).Text.IndexOf("Thank you for answering the questions. Your responses have been submitted.") > -1);
               }  // End Submit_Responses_To_Test_Experiment_2

               /// <summary>
               ///    Test if responses can be submitted for Test Experiment 2.
               /// </summary>
               [TestMethod]
               public void Add_Experiment_Test_Experiment_With_DateTime()
               {
                    // Arrange
                    if (_chromeWebBrowser == null) throw new Exception("Unable to determine where the chrome web driver is.");
                    String experimentName = "Test Experiment " + DateTime.Now.ToString("yyyyMMdd_HHmmss.fff");

                    // Act
                    _chromeWebBrowser.Navigate().GoToUrl(_hostName);                                                                                  // Open website

                    _chromeWebBrowser.FindElement(By.Id("AddExperiment")).Click();                                                                    // Click add experiment
                    
                    (new SelectElement(_chromeWebBrowser.FindElement(By.Name("questionCategory")))).SelectByText("Standard");                         // Select Standard Question
                    (new SelectElement(_chromeWebBrowser.FindElement(By.Name("typeOfQuestion")))).SelectByText("Select An Option");                   // Select Select An Option Question
                    _chromeWebBrowser.FindElement(By.Id("AddQuestion")).Click();                                                                      // Click Add Question

                    _chromeWebBrowser.FindElement(By.Id("AddAnswer")).Click();                                                                        // Click Add Answer
                    _chromeWebBrowser.FindElement(By.Id("AddAnswer")).Click();                                                                        // Click Add Answer
                    _chromeWebBrowser.FindElement(By.Name("StandardQuestionsToAsk[3].Prompt")).SendKeys("Do you have a dog?");                        // Enter question - 'Do you have a dog?'
                    _chromeWebBrowser.FindElement(By.Name("StandardQuestionsToAsk[3].PossibleAnswers[0]")).SendKeys("Yes");                           // Set possible answer to Yes
                    _chromeWebBrowser.FindElement(By.Name("StandardQuestionsToAsk[3].PossibleAnswers[1]")).SendKeys("No");                            // Set possible answer to No

                    (new SelectElement(_chromeWebBrowser.FindElement(By.Name("questionCategory")))).SelectByText("Custom");                           // Select Custom Question
                    (new SelectElement(_chromeWebBrowser.FindElement(By.Name("typeOfQuestion")))).SelectByText("Multi Line");                         // Select Multi Line
                    _chromeWebBrowser.FindElement(By.Id("AddQuestion")).Click();                                                                      // Click Add Question
                    _chromeWebBrowser.FindElement(By.Name("CustomQuestionsToAsk[0].Prompt")).SendKeys("Describe your hobbies");                       // Enter question - 'Describe your hobbies'
                    
                    _chromeWebBrowser.FindElement(By.Id("Name")).SendKeys(experimentName);                                                            // Enter experiment name

                    // Click Add Experiment button
                    _chromeWebBrowser.FindElement(By.Id("AddExperiment")).Click();

                    // Navigate to list of experiments
                    _chromeWebBrowser.Navigate().GoToUrl(_hostName);                                                                                  // Go to list of experiments

                    // Assert
                    Assert.AreEqual(true, _chromeWebBrowser.FindElement(By.Id(experimentName.Replace(" ", "-"))).Text.IndexOf(experimentName) > -1);  // Find experiment name on list
               }  // End Add_Experiment_Test_Experiment_With_DateTime

               /// <summary>
               ///    Test if Test Experiment 2 can be access directly via the URL https://server:port/Test-Experiment-2.
               /// </summary>
               [TestMethod]
               public void Access_Test_Experiment_2_On_Url()
               {
                    // Arrange
                    if (_chromeWebBrowser == null) throw new Exception("Unable to determine where the chrome web driver is.");

                    // Act
                    _chromeWebBrowser.Navigate().GoToUrl(_hostName + "/Test-Experiment-2");         // Open website

                    // Assert
                    Assert.AreEqual(true, _chromeWebBrowser.FindElement(By.Id("standard_questions")).Text.IndexOf("Standard Questions") > -1);
               }  // End Submit_Responses_To_Test_Experiment_1

               /// <summary>
               ///    Test if Test Experiment 2 can be access directly via the URL https://server:port/Test-Experiment-2.
               /// </summary>
               [TestMethod]
               public void Access_Disabled_Test_Experiment_1()
               {
                    // Arrange
                    if (_chromeWebBrowser == null) throw new Exception("Unable to determine where the chrome web driver is.");
                    _chromeWebBrowser.Navigate().GoToUrl(_hostName);                                // Open website
                    if (!_chromeWebBrowser.FindElement(By.Id("z0__IsDisabled")).Selected)           // If Test Experiment 1 is not disabled then tick the disabled box and save
                    { 
                         _chromeWebBrowser.FindElement(By.Id("z0__IsDisabled")).Click();            // Tick Experiment 1 disabled box
                         _chromeWebBrowser.FindElement(By.Id("UpdateExperiments")).Click();         // Click on Update Experiments button
                    }  // End if

                    // Act
                    _chromeWebBrowser.Navigate().GoToUrl(_hostName + "/Test-Experiment-1");         // Open website

                    // Assert
                    Assert.AreEqual(true, _chromeWebBrowser.FindElement(By.TagName("div")).Text.IndexOf("The experiment you are trying to access is disabled. Please re-enable it before proceeding.") > -1);
               }  // End Access_Disabled_Test_Experiment_1

          #endregion
     }  // End class
}  // End namespace