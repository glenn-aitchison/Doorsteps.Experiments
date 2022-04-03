/***************************************************************************************************
 * 
 * Filename:    JsonDatasourceDao.cs 
 * Description: Implemenation - Allows experiments to be added or retrieved from a JSON data file.
 * 
 * Author:      Glenn Aitchison
 * Date:        29-Mar-2022
 * 
 ***************************************************************************************************/

// Include NewtonSoft JSON libraries
using Newtonsoft.Json;

// Include custom code we have built
using Doorsteps.Experiments.Models;

namespace Doorsteps.Experiments.Dao
{
     /// <summary>
     ///    Allows experiments to be added or retrieved from a JSON data file.
     /// </summary>
     public class JsonDatasourceDao : IDatasourceDao
     {
          #region Setup and Initialization

               // Setup variables
               private String _jsonDataFilename     = "Data\\fileData.json",                       // JSON data filename
                              _jsonResponseFilename = "Data\\userResponses.json";                  // Response filename

          #endregion

          #region Accessors 

               /// <summary>
               ///    The name of the file that contains the experiments (in JSON format).
               /// </summary>
               public String DataFilename { get => _jsonDataFilename; set { _jsonDataFilename = value; } }

               /// <summary>
               ///    The name of the file that holds responses that have been submitted (in JSON format).
               /// </summary>
               public String ResponseFilename { get => _jsonResponseFilename; set { _jsonResponseFilename = value; } }

          #endregion

          #region Methods

               /// <summary>
               ///    Get a list of experiments from the data source. 
               /// </summary>
               /// <returns>A list of experiments.</returns>
               public List<Experiment> GetExperiments() 
               {
                    // Setup variables 
                    List<Experiment>? listOfExperiments = null;       // List of experiments                                                                                          
                    JsonSerializer jsonSerializer =                   // JSON serializer - converts data from JSON to objects
                                      JsonSerializer.Create(new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });        

                    // Load experiments from JSON data file
                    using (StreamReader fileStreamReader = new StreamReader(_jsonDataFilename))
                    using (JsonTextReader jsonFileStream = new JsonTextReader(fileStreamReader)) { listOfExperiments = jsonSerializer.Deserialize<List<Experiment>>(jsonFileStream); }

                    // Return list of experiments to user - if none found return a blank list
                    return listOfExperiments ?? new List<Experiment>();
               }  // End GetExperiments

               /// <summary>
               ///    Get a list of responses from the data source. 
               /// </summary>
               /// <returns>A list of experiments and their responses.</returns>
               public List<Experiment> GetResponses() 
               {
                    // Setup variables 
                    List<Experiment>? listOfExperiments = null;      // List of experiments                                                                                          
                    JsonSerializer jsonSerializer =                   // JSON serializer - converts data from JSON to objects
                                      JsonSerializer.Create(new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });        

                    // Load experiments from JSON data file
                    using (StreamReader fileStreamReader = new StreamReader(_jsonResponseFilename))
                    using (JsonTextReader jsonFileStream = new JsonTextReader(fileStreamReader)) { listOfExperiments = jsonSerializer.Deserialize<List<Experiment>>(jsonFileStream); }

                    // Return list of experiments to user - if none found return a blank list
                    return listOfExperiments ?? new List<Experiment>();
               }  // End GetResponses

               /// <summary>
               ///    Add an experiment to the JSON data file.
               /// </summary>
               /// <param name="experimentToAdd">Experiment to add.</param>
               public void AddExperiment(Experiment experimentToAdd) 
               {
                    // Setup variables
                    List<Experiment>? myExperiments  = GetExperiments();                                                                                     // Get a list of all experiments from JSON data file
                    JsonSerializer    jsonSerializer =                                                                                                       // JSON serializer - converts data from JSON to objects
                                         JsonSerializer.Create(new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All, Formatting = Formatting.Indented });    

                    // Add experiment to end of list
                    myExperiments = myExperiments ?? new List<Experiment>();                                                                                 // If no experiments found then create a blank list of them
                    myExperiments.Add(experimentToAdd);                                                                                                      // Add experiment to end of list

                    // Save experiments to JSON data file
                    if (File.Exists(_jsonDataFilename)) File.Delete(_jsonDataFilename);                                                                       // Delete existing JSON data file
                    using (StreamWriter jsonFileStream = new StreamWriter(_jsonDataFilename)) { jsonSerializer.Serialize(jsonFileStream, myExperiments); }    // Save experiments to JSON data file
               }  // End AddExperiment

               /// <summary>
               ///    Update an experiment in the JSON data file.
               /// </summary>
               /// <param name="experimentToUpdate">Experiment to update.</param>
               public void UpdateExperiment(Experiment experimentToUpdate)
               {
                    // Setup variables
                    bool             foundExperiment      = false;                                                                                              // We haven't found experiment yet
                    List<Experiment> myExperiments        = GetExperiments();                                                                                   // Get a list of all experiments from JSON data file
                    Experiment       myExperimentToUpdate = new Experiment();                                                                                   // Get a list of all experiments from JSON data file
                    JsonSerializer   jsonSerializer =                                                                                                           // JSON serializer - converts data from JSON to objects              
                                        JsonSerializer.Create(new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All, Formatting = Formatting.Indented });        

                    // Find out if experiment exists in JSON data file
                    foundExperiment = myExperiments.Any(experiment => experiment.Name.Equals(experimentToUpdate.Name));                                          // Does the experiment exist?                                                                     

                    // If we have found experiment then update it 
                    if (foundExperiment) 
                    {
                         myExperiments = myExperiments.Where(experiment => !experiment.Name.Equals(experimentToUpdate.Name)).ToList();                           // Get all experiements except one we want to update
                         myExperiments.Add(experimentToUpdate);                                                                                                  // Re-add updated experiment to end of the list of experiments
                    }  // End if 

                    // Save updated experiments to JSON data file
                    if (File.Exists(_jsonDataFilename)) File.Delete(_jsonDataFilename);                                                                          // First delete the JSON data file
                    using (StreamWriter jsonFileStream = new StreamWriter(_jsonDataFilename)) { jsonSerializer.Serialize(jsonFileStream, myExperiments); }
               }  // End UpdateExperiment

               /// <summary>
               ///    Submit responses to experiments and save in results JSON data file.
               /// </summary>
               /// <param name="experimentToSubmit">An experiment to submit responses for.</param>
               public void SubmitResponses(Experiment experimentToSubmit)
               {
                    // Setup variables
                    List<Experiment>? myExperiments  = GetResponses();                                                                                           // Get a list of all experiments from JSON data file
                    JsonSerializer    jsonSerializer =                                                                                                           // JSON serializer - converts data from JSON to objects
                                         JsonSerializer.Create(new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All, Formatting = Formatting.Indented });

                    // Add experiment to end of list 
                    myExperiments = myExperiments ?? new List<Experiment>();                                                                                      // If no experiments found then create a blank list of them
                    myExperiments.Add(experimentToSubmit);                                                                                                        // Add experiment to end of list

                    // Save experiments to JSON data file
                    if (File.Exists(_jsonResponseFilename)) File.Delete(_jsonResponseFilename);                                                                   // Delete existing JSON response file
                    using (StreamWriter jsonFileStream = new StreamWriter(_jsonResponseFilename)) { jsonSerializer.Serialize(jsonFileStream, myExperiments); }    // Save experiments to JSON response file
               }  // End SubmitResponses

          #endregion 
     }  // End class
}  // End namespace
