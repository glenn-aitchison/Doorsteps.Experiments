/***************************************************************************************************
 * 
 * Filename:    ExperimentApiController.cs 
 * Description: Implementation - Provides an API to use for interacting with Experiments.
 * 
 * Author:      Glenn Aitchison
 * Date:        29-Mar-2022
 * 
 ***************************************************************************************************/

// Include Microsoft libraries we need
using Microsoft.AspNetCore.Mvc;

// Include Spring libraries we need 
using Spring.Context;
using Spring.Context.Support;

// Include custom code we have builit
using Doorsteps.Experiments.Dao;
using Doorsteps.Experiments.Models;

namespace Doorsteps.Experiments.Api
{
     /// <summary>
     ///    Provides an API to use for interacting with Experiments.
     /// </summary>
     [Route("api/experiments")]
     [ApiController]
     public class ExperimentsApiController : ControllerBase
     {
          #region Setup and Initialization

               // Setup variables
               private IApplicationContext? _experimentApiApp;     // Application that will interact to get experiments from JSON data files
               private IDatasourceDao?      _dataSource;           // Data source to get data from and store data in
          
               /// <summary>
               ///    Create an Experiments API to use.
               /// </summary>
               public ExperimentsApiController()
               { } 

          #endregion

          #region Accessors

               /// <summary>
               ///    The datasource that contains experiments.
               /// </summary>
               public IDatasourceDao Datasource 
               { 
                    get 
                    { 
                         // Get application context from spring - basically the application once all dependencies have been injected
                         _experimentApiApp = ContextRegistry.GetContext();

                         // Get datasource from spring.xml if datasource isnt set
                         if (_dataSource == null) _dataSource = (IDatasourceDao) _experimentApiApp.GetObject("myJsonDatasource"); 

                         // Return data source
                         return _dataSource; 
                    }  // End get
                    set { _dataSource = value; } 
               }  // End Datasource

          #endregion

          #region Methods

               /// <summary>
               ///    Get list of experiments and their questions.
               /// </summary>
               /// <returns>A list of experiments.</returns>
               [HttpGet]
               [Route("all")]
               public List<Experiment> GetExperiments()
               { return Datasource.GetExperiments(); }

               /// <summary>
               ///    Get list of responses for experiments.
               /// </summary>
               /// <returns>A list of responses.</returns>
               [HttpGet]
               [Route("responses")]
               public List<Experiment> GetResponses()
               { return Datasource.GetResponses(); }

               /// <summary>
               ///    Add experiment to datasource.
               /// </summary>
               /// <param name="myExperimentToAdd">Experiment to add.</param>
               [HttpPost]
               [Route("add")]
               public void AddExperiment(Experiment myExperimentToAdd)
               { Datasource.AddExperiment(myExperimentToAdd); }

               /// <summary>
               ///    Update experiment in datasource.
               /// </summary>
               /// <param name="myExperimentToUpdate">Experiment to update.</param>
               [HttpPost]
               [Route("update")]
               public void UpdateExperiment(Experiment myExperimentToUpdate)
               { Datasource.UpdateExperiment(myExperimentToUpdate); }

               /// <summary>
               ///    Submit experiment as a response to datasource.
               /// </summary>
               /// <param name="myExperimentToSubmit">Experiment to submit as response.</param>
               [HttpPost]
               [Route("submit")]
               public void SubmitResponses(Experiment myExperimentToSubmit)
               { Datasource.SubmitResponses(myExperimentToSubmit); }

          #endregion
     }  // End class
}  // End namespace