/***************************************************************************************************
 * 
 * Filename:    Experiment.cs 
 * Description: Implementation - An experiment to perform.
 * 
 * Author:      Glenn Aitchison
 * Date:        29-Mar-2022
 * 
 ***************************************************************************************************/

// Include Newton JSON libaries
using Newtonsoft.Json;

namespace Doorsteps.Experiments.Models
{
     /// <summary>
     ///    An experiment to perform. 
     /// </summary>
     [Serializable]
     public class Experiment : IExperiment
     {
          #region Setup and Initialization

               // Setup variables
               private String         _name              = "",                     _description     = "";                          // Name and description of experiment
               private List<Question> _standardQuestions = new List<Question> { }, _customQuestions = new List<Question> { };      // Standard and custom questions for experiment

          #endregion

          #region Accessors 

               /// <summary>
               ///    The name of the experiment.
               /// </summary>
               [JsonProperty(PropertyName = "name", Order = 1)]
               public String Name { get => _name; set { _name = value; } }

               /// <summary>
               ///    The description of the experiment.
               /// </summary>
               [JsonProperty(PropertyName = "description", Order = 2)]
               public String Description { get => _description; set { _description = value; } }

               /// <summary>
               ///    Is the experiment disabled?
               /// </summary>
               [JsonProperty(PropertyName = "disabled", Order = 3)]
               public bool IsDisabled { get; set; }

               /// <summary>
               ///    A list of standard questions to ask the user. 
               /// </summary>
               [JsonProperty("standard_questions", Order = 4)]
               public List<Question> StandardQuestionsToAsk { get => _standardQuestions; set { _standardQuestions = value; } }

               /// <summary>
               ///    A list of custom questions to ask the user. 
               /// </summary>
               [JsonProperty(PropertyName = "custom_questions", Order = 5)]
               public List<Question> CustomQuestionsToAsk { get => _customQuestions; set { _customQuestions = value; } }

          #endregion
     }  // End class
}  // End namespace
