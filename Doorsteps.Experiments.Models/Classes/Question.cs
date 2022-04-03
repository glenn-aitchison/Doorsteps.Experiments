/***************************************************************************************************
 * 
 * Filename:    Question.cs 
 * Description: Implementation - A question to ask.
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
     ///    A question to ask.
     /// </summary>
     [Serializable]
     public class Question : IQuestion
     {
          #region Setup and Initialization

               /// <summary>
               ///    Create a new question and answer - default to single line question. 
               /// </summary>
               public Question() { Prompt = ""; Answer = ""; Type = (int) Question.TypeOfQuestion.SingleLine; }

               /// <summary>
               ///    Create a new question and answer - default to single line question.  
               /// </summary>
               /// <param name="promptText">The question to ask.</param>
               /// <param name="answerText">The answer to the question.</param>
               public Question(String promptText = "", String answerText = "", int questionType = (int) TypeOfQuestion.SingleLine)
               { Prompt = promptText; Answer = answerText; Type = questionType; }

               /// <summary>
               ///    The type of question to ask - default to single line if not specified. 
               /// </summary>
               public enum TypeOfQuestion { SingleLine = 1, MultiLine = 2, SelectAnOption = 3 }

          #endregion 

          #region Accessors 

               /// <summary>
               ///    The question prompt that is shown to the user.
               /// </summary>
               [JsonProperty(PropertyName = "prompt", Order = 1)]
               public String Prompt { get; set; }

               /// <summary>
               ///    An answer to the question.
               /// </summary>
               [JsonProperty(PropertyName = "answer", Order = 2)]
               public String Answer { get; set; }

               /// <summary>
               ///    The type of question to ask user. 
               /// </summary>
               [JsonProperty(PropertyName = "type", Order = 3)]
               public int Type { get; set; }

          #endregion
     }  // End class
}  // End namespace
