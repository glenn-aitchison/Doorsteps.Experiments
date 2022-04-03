/***************************************************************************************************
 * 
 * Filename:    IQuestion.cs 
 * Description: Interface - Defines what a question must do.
 * 
 * Author:      Glenn Aitchison
 * Date:        29-Mar-2022
 * 
 ***************************************************************************************************/

namespace Doorsteps.Experiments.Models
{
     /// <summary>
     ///    Defines what a question must do. 
     /// </summary>
     public interface IQuestion
     {
          #region Accessors 

               /// <summary>
               ///    The question prompt that is shown to the user.
               /// </summary>
               String Prompt { get; set; }

               /// <summary>
               ///    An answer to the question.
               /// </summary>
               String Answer { get; set; }

               /// <summary>
               ///    The type of question to ask user. 
               /// </summary>
               int Type { get; set; }

          #endregion
     }  // End interface
}  // End namespace
