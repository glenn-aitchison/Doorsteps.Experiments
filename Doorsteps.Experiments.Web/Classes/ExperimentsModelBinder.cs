/***************************************************************************************************
 * 
 * Filename:    ExperimentModelBinder.cs 
 * Description: Implementation - Ensures we bind to standard and custom questions correctly.
 * 
 * Author:      Glenn Aitchison
 * Date:        29-Mar-2022
 * 
 ***************************************************************************************************/

// Include Microsoft libraries we need
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;

// Include custom code we have built
using Doorsteps.Experiments.Models;

namespace Doorsteps.Experiments.Web
{ 
     /// <summary>
     ///    Experiments Model Binder - Ensures we bind to standard and custom questions correctly.
     /// </summary>
     public class ExperimentsModelBinder : IModelBinder
     {
          /// <summary>
          ///    Bind experiments model to data.
          /// </summary>
          public Task BindModelAsync(ModelBindingContext bindingContext)
          {
               // Setup variables 
               int                 questionToProcess           = 0,                             // Question we are processing
                                   possibleAnswerToProcess     = 0;                             // Possible answer we are processing
               String              questionId                  = "",                            // The identifier of the question
                                   questionTypeToProcess       = "";                            // The type of questions we are processing
               ValueProviderResult questionType,                                                // The type of question
                                   possibleAnswer;                                              // A possible answer
               HttpRequest         httpRequest;                                                 // The HTTP request
               PropertyInfo[]      experimentProperties;                                        // Experiment's properties             
               Experiment          experimentToBind            = new Experiment();              // An experiment to bind to 
               List<Question>      questionsToAsk              = new List<Question>();          // A list of questions to ask
               List<String>        possibleAnswers             = new List<String>();            // A list of possible answers
               bool                isDoneWithQuestions         = false,                         // Are we finished processing standard questions?
                                   isDoneWithPossibleAnswers   = false;                         // Are we finished processing possible answers?

               // Make sure context we are binding to exists
               if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

               // Get the HTTP request from user
               httpRequest = bindingContext.ActionContext.HttpContext.Request;

               // Get all properties of an Experiment
               experimentProperties = typeof(Experiment).GetProperties();

               // Bind each property of Experiment so it's available to user
               foreach (PropertyInfo experimentProperty in experimentProperties)
               {
                    // If we have standard or custom questions then get them all correctly
                    if (experimentProperty.Name.Equals("StandardQuestionsToAsk") || experimentProperty.Name.Equals("CustomQuestionsToAsk"))
                    {
                         // Figure out what type of question we have so we can start processing the questions
                         questionTypeToProcess = experimentProperty.Name;                 // Get the question type we are processing - either standard or custom questions
                         questionToProcess = 0; isDoneWithQuestions = false;              // Process all questions
                         questionsToAsk = new List<Question>();

                         // Correctly bind each type of question to the experiment
                         while (!isDoneWithQuestions)
                         {
                              // Get the identifier of the question (in ASP.NET MVC Model Binding) and the type of question we are processing
                              questionId = questionTypeToProcess + "[" + questionToProcess + "]";
                              questionType = bindingContext.ValueProvider.GetValue(questionId + ".Type");

                              // If there are no more questions to process then we are done
                              if (questionType.Length == 0) { isDoneWithQuestions = true; break; }

                              // If we have a single line question then get it
                              if ((questionType.FirstValue ?? "").Equals("1"))
                              {
                                   // Create the question
                                   SingleLineQuestion newQuestion = new SingleLineQuestion()
                                   {
                                        Prompt = bindingContext.ValueProvider.GetValue(questionId + ".Prompt").FirstValue ?? "",
                                        Answer = bindingContext.ValueProvider.GetValue(questionId + ".Answer").FirstValue ?? "",
                                        Type = int.Parse(questionType.FirstValue ?? "")
                                   };

                                   // Add it to list
                                   questionsToAsk.Add(newQuestion);
                              }  // End if

                              // If we have a multi line question then get it
                              if ((questionType.FirstValue ?? "").Equals("2"))
                              {
                                   // Create the question
                                   MultiLineQuestion newQuestion = new MultiLineQuestion()
                                   {
                                        Prompt = bindingContext.ValueProvider.GetValue(questionId + ".Prompt").FirstValue ?? "",
                                        Answer = bindingContext.ValueProvider.GetValue(questionId + ".Answer").FirstValue ?? "",
                                        Type = int.Parse(questionType.FirstValue ?? "")
                                   };

                                   // Add question to list
                                   questionsToAsk.Add(newQuestion);
                              }  // End if

                              // If we have a select an option question then get it
                              if ((questionType.FirstValue ?? "").Equals("3"))
                              {
                                   // Process first possible answer to start with
                                   possibleAnswerToProcess = 0; isDoneWithPossibleAnswers = false; possibleAnswers.Clear();

                                   // Get all possible answers
                                   while (!isDoneWithPossibleAnswers)
                                   {
                                        // Get the possible answer
                                        possibleAnswer = bindingContext.ValueProvider.GetValue(questionId + ".PossibleAnswers[" + possibleAnswerToProcess + "]");

                                        // If we have a possible answer that isnt blank then add it to list of possible answers
                                        // Otherwise we are done getting possible answers
                                        if (possibleAnswer.Length > 0) possibleAnswers.Add(possibleAnswer.FirstValue ?? "");
                                        else if (possibleAnswer.Length == 0) isDoneWithPossibleAnswers = true;

                                        // Go to next possible answer
                                        possibleAnswerToProcess++;
                                   }  // End while

                                   // Create the select an option question with possible answers
                                   SelectAnOptionQuestion newQuestion = new SelectAnOptionQuestion()
                                   {
                                        Prompt = bindingContext.ValueProvider.GetValue(questionId + ".Prompt").FirstValue ?? "",
                                        Answer = bindingContext.ValueProvider.GetValue(questionId + ".Answer").FirstValue ?? "",
                                        Type = int.Parse(questionType.FirstValue ?? ""),
                                        PossibleAnswers = possibleAnswers
                                   };

                                   // Add question to list
                                   questionsToAsk.Add(newQuestion);
                              }  // End if 

                              // Process next question
                              questionToProcess++;
                         }  // End while
                         
                         // Set the list of questions
                         experimentProperty.SetValue(experimentToBind, questionsToAsk);
                    }  // End if

                    // If we are not processing a question then by default set it's value
                    else { experimentProperty.SetValue(experimentToBind, bindingContext.ValueProvider.GetValue(experimentProperty.Name).FirstValue); }
               }  // End foreach

               // Return bound Experiment to user
               bindingContext.Result = ModelBindingResult.Success(experimentToBind); return Task.CompletedTask;
          }  // End BindModelAsync
     }  // End class
}  // End class
 