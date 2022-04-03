/***************************************************************************************************
 * 
 * Filename:    ExperimentController.cs 
 * Description: Implementation - Provides a website to use for interacting with Experiments.
 * 
 * Author:      Glenn Aitchison
 * Date:        29-Mar-2022
 * 
 ***************************************************************************************************/

// Include Microsoft libraries we need
using System.Text;
using Microsoft.AspNetCore.Mvc;

// Include NewtonSOFT JSON libraries
using Newtonsoft.Json;

// Include RestSharp libraries we need to include
using RestSharp;

// Include Spring libraries we need 
using Spring.Context;
using Spring.Context.Support;

// Include custom code we need
using Doorsteps.Experiments.Models;

namespace Doorsteps.Experiments.Web
{
     /// <summary>
     ///    Allows user to interact with Experiments.
     /// </summary>
     [Route("/")]
     public class ExperimentsController : Controller
     {
          #region Setup and Initialization

               // Setup variables
               private IApplicationContext _experimentsWebApp;     // Application that will interact to get experiments from JSON data files
               private RestClient          _experimentsApi;        // RESTful APIs to interact with

               /// <summary>
               ///    Create a website to allow user to interact with Experiments.
               /// </summary>
               public ExperimentsController()
               {
                    // Get application context from spring - basically the application once all dependencies have been injected
                    _experimentsWebApp = ContextRegistry.GetContext();

                    // Get experiments API
                    _experimentsApi = (RestClient) _experimentsWebApp.GetObject("myExperimentsApi");
               }  // End ExperimentsController

          #endregion

          #region Methods 

               #region Common Functionality 

                    /// <summary>
                    ///    Deserialize JSON data to objects.
                    /// </summary>
                    /// <param name="restfulResponse">A RESTful response received from APIs.</param>
                    /// <returns>Objects generated from JSON data.</returns>
                    private T? DeserializeData<T>(RestResponse restfulResponse)
                    {
                         // Setup variables                                                                                       
                         JsonSerializer jsonSerializer =              // Create a JSON serializer - converts JSON data to objects
                                           JsonSerializer.Create(new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });

                         // Convert JSON data to .Net Objects
                         using (StringReader jsonTextReader = new StringReader(restfulResponse.Content ?? ""))
                         using (JsonTextReader jsonStream = new JsonTextReader(jsonTextReader)) { return jsonSerializer.Deserialize<T>(jsonStream); }
                    }  // End DeserializeData

                    /// <summary>
                    ///    Execute RESTful request.
                    /// </summary>
                    /// <param name="restfulRequest">A RESTful request.</param>
                    /// <returns></returns>
                    private RestResponse ExecuteRestfulRequest<T>(RestRequest restfulRequest, T? jsonObject)
                    {
                         // Setup variables
                         StringBuilder  jsonData       = new StringBuilder();    // JSON data
                         JsonSerializer jsonSerializer =                         // JSON serializer - converts data from JSON to objects - make sure we send thro types to make inheritance work
                                           JsonSerializer.Create(new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });   

                         // If we want to pass an object (as JSON) to web service then do so!
                         if (jsonObject != null)
                         {
                              using (StringWriter jsonDataStream = new StringWriter(jsonData)) 
                              { jsonSerializer.Serialize(jsonDataStream, jsonObject); restfulRequest.AddStringBody(jsonData.ToString(), "application/json"); }  
                         }  // End if 

                         // Run the request against the APIs
                         return _experimentsApi.ExecuteAsync(restfulRequest).Result;
                    }  // End ExecuteRestfulRequest

                    /// <summary>
                    ///    Get a list of all experiments.
                    /// </summary>
                    /// <returns>A list of all experiments.</returns>
                    private List<Experiment> GetAllExperiments()
                    {
                         // Setup variables
                         List<Experiment>? myExperiments;                  // List of experiments found
                         RestResponse      restfulResponse;                // RESTful API response

                         // Get all experiments
                         restfulResponse = ExecuteRestfulRequest<IExperiment>(new RestRequest("/all", Method.Get), null);

                         // Show thank you page or error dependant if everything went okay
                         if (!restfulResponse.IsSuccessful) return new List<Experiment>();

                         // Get a list of current experiments  
                         myExperiments = DeserializeData<List<Experiment>>(restfulResponse) ?? new List<Experiment>();                                                       

                         // Return list of experiments
                         return myExperiments;
                    }  // End GetAllExperiments

                    /// <summary>
                    ///    Get a list of all experiments.
                    /// </summary>
                    /// <returns>A list of all experiments.</returns>
                    private List<Experiment> GetAllResponses()
                    {
                         // Setup variables
                         List<Experiment>? myExperiments;                  // List of experiments found
                         RestResponse      restfulResponse;                // RESTful API response

                         // Get all responses from experiments
                         restfulResponse = ExecuteRestfulRequest<IExperiment>(new RestRequest("/responses", Method.Get), null);

                         // Show thank you page or error dependant if everything went okay
                         if (!restfulResponse.IsSuccessful) return new List<Experiment>();

                         // Get a list of current experiments  
                         myExperiments = DeserializeData<List<Experiment>>(restfulResponse) ?? new List<Experiment>();                                                       

                         // Return list of responses from experiments
                         return myExperiments;
                    }  // End GetAllResponses

               #endregion

               #region Routes

                    /// <summary>
                    ///    Add an experiment.
                    /// </summary>
                    /// <param name="myExperiment">The experiment you wish to add questions to.</param>
                    /// <param name="questionCategory">The category of the question (standard or custom)/</param>
                    /// <param name="typeOfQuestion">The type of the question - i.e. SingeLineQuestion, MultiLineQuesiton, or SelectAnOptionQuest.</param>
                    /// <param name="operationMode">The mode of operation.</param>
                    [HttpPost]
                    [HttpGet]
                    [Route("AddExperiment")]
                    public IActionResult AddExperiment([ModelBinder(BinderType = typeof(ExperimentsModelBinder))] Experiment myExperiment, String questionCategory, String typeOfQuestion, String operationMode = "")
                    {
                         // Setup variables
                         RestResponse restfulResponse;                     // Response from RESTful APIs             
                         Experiment?  templateExperiment;                  // Template experiment

                         // Get experiments from template
                         if (operationMode.Equals(""))
                         { 
                              templateExperiment = GetAllExperiments().Where(experiment => experiment.Name.Equals("[Template Experiment]", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                              if (templateExperiment != null) { myExperiment = templateExperiment; myExperiment.Name = ""; }
                         }  // End if 

                         // Remove TEMPLATE answer when when are not adding an answer - otherwise it will be included
                         if (!operationMode.Equals("Add Answer"))
                         {
                              // Remove template answer from any select an option questions that are standard ones
                              foreach (IQuestion question in myExperiment.StandardQuestionsToAsk)
                              {
                                   if (question.GetType() == typeof(SelectAnOptionQuestion))
                                   { SelectAnOptionQuestion myQuestion = (SelectAnOptionQuestion) question; myQuestion.PossibleAnswers.RemoveAt(myQuestion.PossibleAnswers.Count - 1); }
                              }  // End foreach

                              // Remove template answer from any select an option questionsthat are standard ones
                              foreach (IQuestion question in myExperiment.CustomQuestionsToAsk)
                              {
                                   if (question.GetType() == typeof(SelectAnOptionQuestion))
                                   { SelectAnOptionQuestion myQuestion = (SelectAnOptionQuestion) question; myQuestion.PossibleAnswers.RemoveAt(myQuestion.PossibleAnswers.Count - 1); }
                              }  // End foreach
                         }  // End if 

                         // Add a question if user has clicked button
                         if (operationMode.Equals("Add Question"))
                         {
                              // Add a single line question if requested
                              if (myExperiment != null && questionCategory.Equals("Standard") && typeOfQuestion.Equals("Single Line"))
                                   myExperiment.StandardQuestionsToAsk.Add(new SingleLineQuestion());

                              // Add a multi line question if requested
                              if (myExperiment != null && questionCategory.Equals("Standard") && typeOfQuestion.Equals("Multi Line"))
                                   myExperiment.StandardQuestionsToAsk.Add(new MultiLineQuestion());

                              // Add a single line question if requested
                              if (myExperiment != null && questionCategory.Equals("Standard") && typeOfQuestion.Equals("Select An Option"))
                                   myExperiment.StandardQuestionsToAsk.Add(new SelectAnOptionQuestion());

                              // Add a single line question if requested
                              if (myExperiment != null && questionCategory.Equals("Custom") && typeOfQuestion.Equals("Single Line"))
                                   myExperiment.CustomQuestionsToAsk.Add(new SingleLineQuestion());

                              // Add a multi line question if requested
                              if (myExperiment != null && questionCategory.Equals("Custom") && typeOfQuestion.Equals("Multi Line"))
                                   myExperiment.CustomQuestionsToAsk.Add(new MultiLineQuestion());

                              // Add a single line question if requested
                              if (myExperiment != null && questionCategory.Equals("Custom") && typeOfQuestion.Equals("Select An Option"))
                                   myExperiment.CustomQuestionsToAsk.Add(new SelectAnOptionQuestion());
                         }  // End if

                         // Add experiment if user has pressed the button
                         if (operationMode.Equals("Add Experiment"))
                         {
                              // Make sure experiment name has been provided
                              if (String.IsNullOrEmpty((myExperiment ?? new Experiment()).Name))
                              { ViewBag.Title = "An error has occurred"; return View("NoExperimentName"); }

                              // Make sure experiment doesnt already exist
                              if (GetAllExperiments().Any(experiment => experiment.Name.Equals((myExperiment ?? new Experiment()).Name, StringComparison.InvariantCultureIgnoreCase)))
                              { ViewBag.Title = "An error has occurred"; return View("ExperimentAlreadyExists"); }

                              // Make sure at least one standard question exists
                              if ((myExperiment ?? new Experiment()).StandardQuestionsToAsk.Count == 0)
                              { ViewBag.Title = "An error has occurred"; return View("NoQuestionsToAsk"); }

                              // Make sure we have completed questions
                              if ((myExperiment ?? new Experiment()).StandardQuestionsToAsk.Any(question => String.IsNullOrEmpty(question.Prompt)) ||
                                  (myExperiment ?? new Experiment()).CustomQuestionsToAsk.Any(question => String.IsNullOrEmpty(question.Prompt)))
                              { ViewBag.Title = "An error has occurred"; return View("IncompleteQuestions"); }

                              // Add experiment
                              restfulResponse = ExecuteRestfulRequest<IExperiment>(new RestRequest("/add", Method.Post), myExperiment);

                              // Show list of experiments
                              return RedirectToAction("");
                         }  // End if 

                         // Show add experiment page
                         return View("AddExperiment", myExperiment);
                    }  // End ShowQuestionaire

                    /// <summary>
                    ///    Display name and description of experiment along with questions to ask user.
                    /// </summary>
                    [HttpGet]
                    [Route("{experimentName}")]
                    public IActionResult ShowQuestionaire(String experimentName = "") 
                    {
                         // Setup variables
                         Experiment        myExperiment;                   // Experiment 
                         List<Experiment>? myExperiments;                  // List of experiments found

                         // Replace -'s with spaces
                         experimentName = experimentName.Replace("-", " ");

                         // Display Experiment Questionaire as title
                         ViewBag.Title = "Experiment Questionaire";        

                         // Get all experiments
                         myExperiments = GetAllExperiments();

                         // Make sure experiment we want to view exists
                         myExperiment = myExperiments.Where(experiment => experiment.Name.Equals(experimentName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault() ?? new Experiment();        // Find selected experiment
                         if (myExperiment == null) { ViewBag.Title = "An error has occurred"; return View("NoSuchExperiment"); }                                                                            // Show error if experiment not found
               
                         // Show error if experiment is disabled
                         if (myExperiment.IsDisabled) { ViewBag.Title = "An error has occurred"; return View("ExperimentDisabled"); }

                         // Show questionaire for experiment
                         return View("ShowQuestionaire", myExperiment); 
                    }  // End ShowQuestionaire

                    /// <summary>
                    ///    Get a list of all experiments so they can be either enabled or disabled.
                    /// </summary>
                    [HttpGet]
                    [Route("")]
                    public IActionResult ListExperiments()
                    {
                         ViewBag.Title = "Welcome";                        // Display Welcome as title on front page
                         return View(GetAllExperiments());                 // Display list of experiments
                    }  // End ListExperiments

                    /// <summary>
                    ///    Toggle what experiments are enabled or disabled.
                    /// </summary>
                    [HttpPost]
                    [Route("ToggleExperiments")]
                    public IActionResult ToggleExperiments(List<Experiment> myExperiments)
                    {
                         // Setup variables
                         Experiment?      myExperiment;                    // The experiment to toggle
                         List<Experiment> myExperimentsFromApis;           // A list of experiments retrieved from APIs
                         RestResponse     restfulResponse;                 // RESTful API response

                         // Get all experiments
                         myExperimentsFromApis = GetAllExperiments();

                         // Update experiments in system
                         foreach (IExperiment experimentToUpdate in myExperimentsFromApis)
                         {
                              // Enable / Disable Experiment
                              myExperiment = myExperiments.Where(experiment => experiment.Name.Equals(experimentToUpdate.Name)).FirstOrDefault();    // Find experiment to toggle
                              if (myExperiment != null) experimentToUpdate.IsDisabled = myExperiment.IsDisabled;                                     // If experiment is found then toggle whether or not its disabled         

                              // Update experiment 
                              restfulResponse = ExecuteRestfulRequest<IExperiment>(new RestRequest("/update", Method.Post), experimentToUpdate);
                         }  // End foreach

                         // Show list of experiments
                         return View("ExperimentsUpdated");
                    }  // End ToggleExperiments
                                        
                    /// <summary>
                    ///    Submit responses to questions.
                    /// </summary>
                    [HttpPost]
                    [Route("SubmitResponses")]
                    public IActionResult SubmitResponses(Experiment myExperiment) 
                    {
                         // Setup variables
                         List<Experiment>? myExperiments;                       // A list of experiments from APIs
                         IQuestion?        questionAndAnswerProvidedByUser;     // Question and answer provided by user
                         Experiment?       experimentFound;                     // The experiment to submit responses to

                         // Find experiment 
                         myExperiments = GetAllExperiments();

                         // Find experiment
                         experimentFound = myExperiments.Where(experiment => experiment.Name.Equals(myExperiment.Name)).FirstOrDefault();        // Find experiment to submit responses for
                         if (experimentFound == null) { ViewBag.Title = "An error has occurred"; return View("NoSuchExperiment"); }              // Display error if experiment not found

                         // If experiment is disabled then display error to user 
                         if (myExperiment.IsDisabled) { ViewBag.Title = "An error has occurred"; return View("ExperimentDisabled"); }

                         // Update standard questions and answers 
                         foreach(IQuestion questionToAsk in experimentFound.StandardQuestionsToAsk)
                         {
                              // Get question and answer provided by user that matches question we are asking 
                              questionAndAnswerProvidedByUser = myExperiment.StandardQuestionsToAsk.Where(question => question.Prompt.Equals(questionToAsk.Prompt)).FirstOrDefault();

                              // If user has provided answer to question asked then submit it as a response
                              if (questionAndAnswerProvidedByUser != null) questionToAsk.Answer = questionAndAnswerProvidedByUser.Answer??""; 
                         }  // End foreach

                         // Update custom questions and answers 
                         foreach (IQuestion questionToAsk in experimentFound.CustomQuestionsToAsk)
                         {
                              // Get question and answer provided by user that matches question we are asking 
                              questionAndAnswerProvidedByUser = myExperiment.CustomQuestionsToAsk.Where(question => question.Prompt.Equals(questionToAsk.Prompt)).FirstOrDefault();

                              // If user has provided answer to question asked then submit it as a response
                              if (questionAndAnswerProvidedByUser != null) questionToAsk.Answer = questionAndAnswerProvidedByUser.Answer??"";
                         }  // End foreach

                         // Submit responses to questions
                         RestResponse restfulResponse = ExecuteRestfulRequest<IExperiment>(new RestRequest("/submit", Method.Post), experimentFound);

                         // Show thank you page or error dependant if everything went okay
                         if (restfulResponse.IsSuccessful) { ViewBag.Title = "Experiment Submitted"; return View("ThankYou"); } else { ViewBag.Title = "An error has occurred"; return View("SomethingWentWrong"); }
                    }  // End SubmitResponses

               #endregion

          #endregion
     }  // End class
}  // End namespace