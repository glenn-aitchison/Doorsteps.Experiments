/***************************************************************************************************
 * 
 * Filename:    SingleLineQuestion.cs 
 * Description: Implementation - A single line question to ask.
 * 
 * Author:      Glenn Aitchison
 * Date:        29-Mar-2022
 * 
 ***************************************************************************************************/

namespace Doorsteps.Experiments.Models
{
     /// <summary>
     ///    A single line question to ask.
     /// </summary>
     [Serializable]
     public class SingleLineQuestion : Question, IQuestion
     {
          #region Setup and Initialization

               /// <summary>
               ///    Create a new single line question and answer. 
               /// </summary>
               /// <param name="promptText">The question to ask.</param>
               /// <param name="answerText">The answer to the question.</param>
               public SingleLineQuestion(String promptText = "", String answerText = "") : base(promptText, answerText) { }

          #endregion 
     }  // End class
}  // End namespace
