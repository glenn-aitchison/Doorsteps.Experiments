/***************************************************************************************************
 * 
 * Filename:    IDatasourceDao.cs 
 * Description: Interface - Defines what a datasource must do.
 * 
 * Author:      Glenn Aitchison
 * Date:        29-Mar-2022
 * 
 ***************************************************************************************************/

// Include custom code we have built
using Doorsteps.Experiments.Models;

namespace Doorsteps.Experiments.Dao
{
     /// <summary>
     ///    Defines what a datasource must do.
     /// </summary>
     public interface IDatasourceDao
     {
          #region Methods

               /// <summary>
               ///    Get a list of experiments from the datasource. 
               /// </summary>
               /// <returns>A list of experiments.</returns>
               List<Experiment> GetExperiments();

               /// <summary>
               ///    Get a list of responses from the data source. 
               /// </summary>
               /// <returns>A list of experiments and their responses.</returns>
               List<Experiment> GetResponses();

               /// <summary>
               ///    Add an experiment to the data source.
               /// </summary>
               /// <param name="experimentToAdd">Experiment to add.</param>
               void AddExperiment(Experiment experimentToAdd);

               /// <summary>
               ///    Update an experiment in the data source.
               /// </summary>
               /// <param name="experimentToUpdate">Experiment to update.</param>
               void UpdateExperiment(Experiment experimentToUpdate);

               /// <summary>
               ///    Submit responses to experiments to the data source.
               /// </summary>
               /// <param name="experimentToSubmit">An experiment to submit responses for.</param>
               void SubmitResponses(Experiment experimentsToSubmit);

          #endregion
     }  // End interface
}  // End namespace
