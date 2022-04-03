/***************************************************************************************************
 * 
 * Filename:    MultiLineQuestion.cs 
 * Description: Implementation - A multi line question to ask.
 * 
 * Author:      Glenn Aitchison
 * Date:        29-Mar-2022
 * 
 ***************************************************************************************************/

namespace Doorsteps.Experiments.Models
{
     /// <summary>
     ///    A multi line question to ask.
     /// </summary>
     [Serializable]
     public class MultiLineQuestion : Question, IQuestion
     {
          #region Setup and Initialization

               /// <summary>
               ///    Create a new multi line question and answer. 
               /// </summary>
               public MultiLineQuestion() { Type = (int) Question.TypeOfQuestion.MultiLine; }

               /// <summary>
               ///    Create a new multi line question and answer. 
               /// </summary>
               /// <param name="promptText">The question to ask.</param>
               /// <param name="answerText">The answer to the question.</param>
               public MultiLineQuestion(String promptText = "", String answerText = "") : base(promptText, answerText, (int) Question.TypeOfQuestion.MultiLine) { }

          #endregion 
     }  // End class
}  // End namespace
