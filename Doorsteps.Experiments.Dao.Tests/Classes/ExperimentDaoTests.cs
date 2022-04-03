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
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Include Spring libraries we need 
using Spring.Context;
using Spring.Context.Support;

// Include custom code we have built
using Doorsteps.Experiments.Models;

namespace Doorsteps.Experiments.Dao.Tests
{
     /// <summary>
     ///    Test the Experiment DAOs (Data Access Objects) to verify they are working correctly. 
     /// </summary>
     [TestClass]
     public class ExperimentDaoTests
     {
          #region Setup and Initialization

               // Setup variables
               private IApplicationContext _experimentTestApp = new XmlApplicationContext("Config/spring.xml");    // Test application that will interact with experiments DAO (Data Access Object)
               private IDatasourceDao?     _myTestDataSource  = null;                                              // We want to get experiments from Json data file

               /// <summary>
               ///    Get ready to test. 
               /// </summary>
               [TestInitialize]
               public void Setup()
               {
                    // Register Spring configuration file with application and use that to inject dependencies
                    ContextRegistry.Clear(); ContextRegistry.RegisterContext(new XmlApplicationContext("Config/spring.xml"));

                    // Get application context from spring - basically the application once all dependencies have been injected
                    _experimentTestApp = ContextRegistry.GetContext();

                    // Get JSON data file as a datasource
                    _myTestDataSource = (IDatasourceDao) _experimentTestApp.GetObject("myJsonDatasource");
               }  // End Setup

               /// <summary>
               ///    Cleanup after testing.
               /// </summary>
               [TestCleanup]
               public void Cleanup()
               {}  // End Cleanup

          #endregion

          #region Get Experiments Tests

               /// <summary>
               ///    Try to get a list of experiments from the JSON data file.
               /// </summary>
               [TestMethod]
               public void Get_Existing_Experiments_From_JsonFile()
               {
                    // Arrange
                    List<Experiment> myExperiments = new List<Experiment>();                                       // A list of experiments found
                    if (_myTestDataSource == null) throw new Exception("Aborting test - no datasource found!");    // If no datasource is provided to test with then fail test

                    // Act 
                    myExperiments = _myTestDataSource.GetExperiments();                                            // Try to get a list of experiments

                    // Assert 
                    Assert.AreEqual(true, myExperiments != null && myExperiments.Count > 0);                       // We must have at least one experiment loaded from JSON data file 
               }  // End Get_Existing_Experiments_From_JsonFile

          #endregion

          #region Get Experiments Tests

               /// <summary>
               ///    Try to get a list of responses from the JSON data file.
               /// </summary>
               [TestMethod]
               public void Get_Existing_Responses_From_JsonFile()
               {
                    // Arrange
                    List<Experiment> myExperiments = new List<Experiment>();                                       // A list of experiments found
                    if (_myTestDataSource == null) throw new Exception("Aborting test - no datasource found!");    // If no datasource is provided to test with then fail test

                    // Act 
                    myExperiments = _myTestDataSource.GetResponses();                                              // Try to get a list of experiments

                    // Assert 
                    Assert.AreEqual(true, myExperiments != null && myExperiments.Count > 0);                       // We must have at least one experiment loaded from JSON data file 
               }  // End Get_Existing_Responses_From_JsonFile

          #endregion

          #region Add Experiment Tests

          /// <summary>
          ///    Test if we have been able to successfully add an experiment to the JSON Data file.
          /// </summary>
          [TestMethod]
               public void Add_New_Experiment_To_JsonFile()
               {
                    // Arrange
                    if (_myTestDataSource == null) throw new Exception("Aborting test - no datasource found!");   // If no datasource is provided to test with then fail test
                    List<Experiment>  myExperiments             = _myTestDataSource.GetExperiments();             // Current list of experiments
                    int               numberOfExperimentsBefore = myExperiments.Count;                            // Get number of experiments before adding an additional one

                    // Create a new experiment with some questions
                    Experiment myExperimentToAdd = new Experiment()                                                                         
                    {
                         Name                   = String.Format("Test Experiment - {0}", System.DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss.fff")),
                         Description            = "This is a test.",
                         StandardQuestionsToAsk =
                              new List<Question>()
                              {
                                   new SingleLineQuestion()     { Prompt = "What is your name?",            Answer = "Joe Bloggs"                                                      },
                                   new MultiLineQuestion()      { Prompt = "Please tell me about yourself", Answer = "I am a cool person and\nlive in some place."                     },
                                   new SelectAnOptionQuestion() { Prompt = "What is gender?",               Answer = "Male", PossibleAnswers = new List<String>() { "Male", "Female" } },
                              },
                         CustomQuestionsToAsk =
                              new List<Question>() { new SingleLineQuestion() { Prompt = "What are your hobbies?", Answer = "Rugby, Netball, Soccer" } }
                    };

                    // Act 
                    _myTestDataSource.AddExperiment(myExperimentToAdd);                                            // Try adding an experiment to the JSON data file

                    // Assert 
                    myExperiments = _myTestDataSource.GetExperiments();                                            // Reload experiments from JSON data file
                    Assert.AreEqual(true, myExperiments.Count > numberOfExperimentsBefore);                        // Check we have successfully added a new experiment to file
               }  // End Add_New_Experiment_To_JsonFile

          #endregion

          #region Update Experiment Tests

               /// <summary>
               ///    Test if we can update an experiment.
               /// </summary>
               [TestMethod]
               public void Update_New_Experiment_In_JsonFile()
               {
                    // Arrange
                    if (_myTestDataSource == null) throw new Exception("Aborting test - no datasource found!");    // If no datasource is provided to test with then fail test
                    int              numberOfExperimentsBefore = 0;                                                // The number of experiments before we begin
                    bool             isExperimentDisabled      = false;                                            // We havent disabled experiment yet
                    List<Experiment> myExperiments             = new List<Experiment>();                           // A list of experiences from JSON data file

                    // Create a new experiment to add then update
                    Experiment myExperimentToUpdate = new Experiment()                                        
                    {
                         Name                   = String.Format("Test Experiment - {0}", System.DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss.fff")),
                         Description            = "This is a test.",
                         IsDisabled             = false,
                         StandardQuestionsToAsk =
                              new List<Question>()
                              {
                                   new SingleLineQuestion()     { Prompt = "What is your name?",            Answer = "Joe Bloggs"                                                      },
                                   new MultiLineQuestion()      { Prompt = "Please tell me about yourself", Answer = "I am a cool person and\nlive in some place."                     },
                                   new SelectAnOptionQuestion() { Prompt = "What is gender?",               Answer = "Male", PossibleAnswers = new List<String>() { "Male", "Female" } },
                              },
                         CustomQuestionsToAsk =
                              new List<Question>() { new SingleLineQuestion() { Prompt = "What are your hobbies?", Answer = "Rugby, Netball, Soccer" } }
                    };

                    // Make sure we have added a new experiment so we can update it later on - verify it exists first before proceeding otherwise fail the test
                    _myTestDataSource.AddExperiment(myExperimentToUpdate); myExperiments = _myTestDataSource.GetExperiments();                                                                          // Add experiment then reload it from datasource
                    if (!myExperiments.Any(experiment => experiment.Name.Equals(myExperimentToUpdate.Name))) throw new Exception("Test failed - unable to update an experiment that doesnt exist!");    // Return error to user if we cannot find experiment

                    // Get number of experiments and if experiment is disabled before we update it
                    numberOfExperimentsBefore = myExperiments.Count; isExperimentDisabled = myExperimentToUpdate.IsDisabled;

                    // Make sure new added experiment is now disabled
                    myExperimentToUpdate.IsDisabled = true;

                    // Act 
                    _myTestDataSource.UpdateExperiment(myExperimentToUpdate);                                                                                                                           // Update experiment in datasource

                    // Assert 
                    myExperiments = _myTestDataSource.GetExperiments();                                                                                                                                 // Reload experiments from datasource
                    Assert.AreEqual(true, myExperiments.Count == numberOfExperimentsBefore);                                                                                                            // Check we have the same number of experiments after update
                    Assert.AreEqual(true, !isExperimentDisabled && myExperiments.Where(experiment => experiment.Name.Equals(myExperimentToUpdate.Name)).First().IsDisabled);                            // Make sure experiment is now disabled
               }  // End Update_New_Experiment_In_JsonFile

          #endregion

          #region Submit Experiment Responses Tests

               /// <summary>
               ///    Test we have been able to successfully submit the responses to an experiment and save them into a JSON responses file.
               /// </summary>
               [TestMethod]
               public void Submit_Experiment_Responses()
               {
                    // Arrange
                    if (_myTestDataSource == null) throw new Exception("Aborting test - no datasource found!");                                     // If we don't have a datasource then return error to user
                    Experiment myExperimentToAdd = new Experiment()                                                                                 // Create a new experiment with some questions
                    {
                         Name                   = String.Format("Test Experiment - {0}", System.DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss.fff")),  // Name the experiment with data and time 
                         Description            = "This is a test.",
                         StandardQuestionsToAsk =
                              new List<Question>()
                              {
                                   new SingleLineQuestion()     { Prompt = "What is your name?",            Answer = "Joe Bloggs"                                                      },
                                   new MultiLineQuestion()      { Prompt = "Please tell me about yourself", Answer = "I am a cool person and\nlive in some place."                     },
                                   new SelectAnOptionQuestion() { Prompt = "What is gender?",               Answer = "Male", PossibleAnswers = new List<String>() { "Male", "Female" } },
                              },
                         CustomQuestionsToAsk =
                              new List<Question>() { new SingleLineQuestion() { Prompt = "What are your hobbies?", Answer = "Rugby, Netball, Soccer" } }
                    };

                    // Act 
                    _myTestDataSource.SubmitResponses(myExperimentToAdd);                                                                         // Try submitting an experiment and its responses

                    // Assert 
                    // We have to manually check the Responses submitted in the file
                    Assert.AreEqual(true, File.Exists(((JsonDatasourceDao)_myTestDataSource).ResponseFilename));
               }  // End Submit_Experiment_Responses

          #endregion
     }  // End class
}  // End namespace