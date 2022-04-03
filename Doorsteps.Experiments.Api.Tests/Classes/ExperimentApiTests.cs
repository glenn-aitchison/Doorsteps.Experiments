/***************************************************************************************************
 * 
 * Filename:    ExperimentApiTests.cs 
 * Description: Implementation - Test the Experiment APIs to verify they are working correctly. 
 * 
 * Author:      Glenn Aitchison
 * Date:        29-Mar-2022
 * 
 ***************************************************************************************************/

// Include Microsoft libraries we need
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Include Spring libraries we need 
using Spring.Context;
using Spring.Context.Support;

// Include custom code we have built
using Doorsteps.Experiments.Models;

namespace Doorsteps.Experiments.Api.Tests
{
     /// <summary>
     ///    Test the Experiment APIs to verify they are working correctly. 
     /// </summary>
     [TestClass]
     public class ExperimentApiTests
     {
          #region Setup and Initialization

               // Setup variables
               private IApplicationContext?      _experimentTestApp        = null;   // Test application that will interact with experiments APIs
               private ExperimentsApiController? _experimentsApiController = null;   // Experiments Api

               /// <summary>
               ///    Get ready to test. 
               /// </summary>
               [TestInitialize]
               public void Setup()
               { }  // End Setup

               /// <summary>
               ///    Cleanup after testing.
               /// </summary>
               [TestCleanup]
               public void Cleanup()
               { }  // End Cleanup

          #endregion

          #region Accessors

               /// <summary>
               ///    The datasource that contains experiments.
               /// </summary>
               public ExperimentsApiController ExperimentsApi
               {
                    get
                    {
                         // Register Spring configuration file with application and use that to inject dependencies
                         ContextRegistry.Clear(); ContextRegistry.RegisterContext(new XmlApplicationContext("Config/spring.xml"));

                         // Get application context from spring - basically the application once all dependencies have been injected
                         _experimentTestApp = ContextRegistry.GetContext();

                         // Get experiment web apis
                         if (_experimentsApiController == null) _experimentsApiController = (ExperimentsApiController) _experimentTestApp.GetObject("myExperimentsApi");

                         // Return data source
                         return _experimentsApiController;
                    }  // End get
                    set { _experimentsApiController = value; }
               }  // End Datasource

          #endregion

          #region Get Experiments Tests

               /// <summary>
               ///    Try to get a list of experiments  using Experiments APIs.
               /// </summary>
               [TestMethod]
               public void Get_Existing_Experiments_Using_WebApi()
               {
                    // Arrange
                    List<Experiment> myExperiments = new List<Experiment>();             // A list of experiments                                                                                       // A list of experiments found
                    
                    // Act 
                    myExperiments = ExperimentsApi.GetExperiments();                                                                                                           // Try to get list of experiments from APIs

                    // Assert 
                    Assert.AreEqual(true, myExperiments != null && myExperiments.Count > 0);                                                                                   // We must have at least one experiment 
               }  // End Get_Existing_Experiments_Using_WebApi

          #endregion

          #region Get Experiments Tests

               /// <summary>
               ///    Try to get a list of experiments  using Experiments APIs.
               /// </summary>
               [TestMethod]
               public void Get_Existing_Responses_Using_WebApi()
               {
                    // Arrange
                    List<Experiment> myExperiments = new List<Experiment>();             // A list of experiments                                                                                       // A list of experiments found
                    
                    // Act 
                    myExperiments = ExperimentsApi.GetResponses();                                                                                                           // Try to get list of experiments from APIs

                    // Assert 
                    Assert.AreEqual(true, myExperiments != null && myExperiments.Count > 0);                                                                                   // We must have at least one experiment 
               }  // End Get_Existing_Responses_Using_WebApi

          #endregion

          #region Add Experiment Tests

               /// <summary>
               ///    Test if we have been able to successfully add an experiment using Experiments APIs.
               /// </summary>
               [TestMethod]
               public void Add_New_Experiment_Using_WebApi()
               {
                    // Arrange
                    List<Experiment> myExperiments             = ExperimentsApi.GetExperiments();       // A list of experiements before adding one                                                             // Current list of experiments
                    int              numberOfExperimentsBefore = myExperiments.Count;                   // Number of experiments before adding one                                                                    // Get number of experiments before adding an additional one

                    // Create a new experiment with some questions
                    Experiment myExperimentToAdd = new Experiment()                                                                         
                    {
                         Name = String.Format("Test Experiment - {0}", System.DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss.fff")),                                               // Name of experiment includes data and time
                         Description = "This is a test.",
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
                    ExperimentsApi.AddExperiment(myExperimentToAdd);                                                                                                           // Try adding an experiment using the APIs

                    // Assert 
                    myExperiments = ExperimentsApi.GetExperiments();                                                                                                           // Reload experiments using the APIs
                    Assert.AreEqual(true, myExperiments != null && myExperiments.Count > numberOfExperimentsBefore);                                                           // Check we have successfully added a new experiment using APIs
               }  // End Add_New_Experiment_Using_WebApi

          #endregion

          #region Update Experiment Tests

               /// <summary>
               ///    Test if we can update an experiment.
               /// </summary>
               [TestMethod]
               public void Update_New_Experiment_Using_WebApi()
               {
                    // Arrange
                    int              numberOfExperimentsBefore = 0;                                       // Number of experiments before testing                                                                                             // The number of experiments before we begin test
                    List<Experiment> myExperiments             = new List<Experiment>();                  // List of experiments                                                                             // A list of experiences from JSON data file
                    bool             isExperimentDisabled      = false;                                   // Is experiment disabled?                                                                                        // We have not yet found experiment and havent disabled experiment yet

                    // Create a new experiment
                    Experiment myExperimentToUpdate = new Experiment()                                                              
                    {
                         Name = String.Format("Test Experiment - {0}", System.DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss.fff")),
                         Description = "This is a test.",
                         IsDisabled = false,
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

                    // Make sure we have added a new experiment so we can update it later on - verify it exists first before proceeding
                    ExperimentsApi.AddExperiment(myExperimentToUpdate); myExperiments = ExperimentsApi.GetExperiments();                                                       // Add experiment and then reload it from datasource
                    if (!myExperiments.Any(experiment => experiment.Name.Equals(myExperimentToUpdate.Name)))                                                                   // If experiment not found then return error to user
                              throw new Exception("Test failed - unable to update an experiment that doesnt exist!");

                    // Get number of experiments before updating the one we have recently added and check if experiment is disabled
                    numberOfExperimentsBefore = myExperiments.Count; isExperimentDisabled = myExperimentToUpdate.IsDisabled;

                    // Make sure new added experiment is now disabled
                    myExperimentToUpdate.IsDisabled = true;

                    // Act 
                    ExperimentsApi.UpdateExperiment(myExperimentToUpdate);                                                                                                     // Try to update experiments

                    // Assert 
                    myExperiments = ExperimentsApi.GetExperiments();                                                                                                           // Reload experiments from JSON data file
                    Assert.AreEqual(true, myExperiments.Count == numberOfExperimentsBefore);                                                                                   // Check we have the same number of experiments after update
                    Assert.AreEqual(true, !isExperimentDisabled && myExperiments.Where(experiment => experiment.Name.Equals(myExperimentToUpdate.Name)).First().IsDisabled);   // Make sure experiment is now disabled
               }  // End Update_New_Experiment_Using_WebApi

          #endregion

          #region Submit Experiment Responses Tests

               /// <summary>
               ///    Test we have been able to successfully submit a response to an experiment.
               /// </summary>
               [TestMethod]
               public void Submit_Experiment_Responses_Using_WebApi()
               {
                    // Arrange
                    // Do nothing
               
                    // Create a new experiment with some questions
                    Experiment myExperimentToAdd = new Experiment()                                                              
                    {
                         Name                   = String.Format("Test Experiment - {0}", System.DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss.fff")),              // Include data and time in name of experiment
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
                    ExperimentsApi.SubmitResponses(myExperimentToAdd);                                                                                          // Try adding an experiment to the JSON data file

                    // Assert 
                    // Nothing to check - if there was a problem then an exception would be returned via APIs
               }  // End Submit_Experiment_Responses_Using_WebApi

          #endregion
     }  // End class
}  // End namespace