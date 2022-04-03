/***************************************************************************************************
 * 
 * Filename:    IExperiment.cs 
 * Description: Interface - Defines what an experiment must do. 
 * 
 * Author:      Glenn Aitchison
 * Date:        29-Mar-2022
 * 
 ***************************************************************************************************/

namespace Doorsteps.Experiments.Models
{
     /// <summary>
     ///    Defines what an experiment must do. 
     /// </summary>
     public interface IExperiment
     {
          #region Accessors 

               /// <summary>
               ///    The name of the experiment.
               /// </summary>
               String Name { get; set; }

               /// <summary>
               ///    The description of the experiment.
               /// </summary>
               String Description { get; set; }

               /// <summary>
               ///    Is the experiment disabled?
               /// </summary>
               bool IsDisabled { get; set; }

               /// <summary>
               ///    A list of standard questions to ask the user. 
               /// </summary>
               List<Question> StandardQuestionsToAsk { get; set; }

               /// <summary>
               ///    A list of custom questions to ask the user. 
               /// </summary>
               List<Question> CustomQuestionsToAsk { get; set; }

          #endregion
     }  // End interface
}  // End namespace
