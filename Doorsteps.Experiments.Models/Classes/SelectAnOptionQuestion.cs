/***************************************************************************************************
 * 
 * Filename:    SelectAnOptionQuestion.cs 
 * Description: Implementation - A select an option question to ask.
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
     ///    A select an option question to ask.
     /// </summary>
     [Serializable]
     public class SelectAnOptionQuestion : Question, IQuestion
     {
          #region Setup and Initialization

               /// <summary>
               ///    Create a new select an option question and answer. 
               /// </summary>
               public SelectAnOptionQuestion() { Prompt = ""; Answer = ""; PossibleAnswers = new List<String>(); Type = (int) Question.TypeOfQuestion.SelectAnOption; }

               /// <summary>
               ///    Create a new select an option question and answer and possible answers to select from. 
               /// </summary>
               /// <param name="promptText">The question to ask.</param>
               /// <param name="answerText">The answer to the question.</param>
               public SelectAnOptionQuestion(String promptText = "", String answerText = "", List<String>? possibleAnswers = null) : base(promptText, answerText, (int) Question.TypeOfQuestion.SelectAnOption) 
               { PossibleAnswers = possibleAnswers ?? new List<String>(); }

          #endregion 

          #region Accessors 

               /// <summary>
               ///    A list of possible answers to select from.
               /// </summary>
               [JsonProperty(PropertyName = "possible_answers", Order = 1)]
               public List<String> PossibleAnswers { get; set; }

          #endregion 

     }  // End class
}  // End namespace
