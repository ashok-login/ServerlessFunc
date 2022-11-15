using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace ServerlessFunc
{
    public static class TodoApi_InMemory
    {
        static List<Todo> items = new List<Todo>();

        /* Here, we can use HttpTrigger attribute to customize the HttpTigger for this function.
         * Here, AuthorizationLevel was set to Anonymous. Allowing just single post method. */
        [FunctionName("CreateTodo")]/* ← Azure function name */
        public static async Task<IActionResult> CreateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, /*"get",*/ "post", Route = "todo")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Creating a new todo list item");
            /* Read the body of the incoming HTTP request into a string. */
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            /* Use Newtonsoft Json to deserialize requestBody into an instance of our TodoCreateModel class. */
            var input = JsonConvert.DeserializeObject<TodoCreateModel>(requestBody);

            var todo = new Todo() { TaskDescription = input.TaskDescription };
            items.Add(todo);
            /* Use the OkObjectResult to return the entire todo item which will be serialized to JSON for us. */
            return new OkObjectResult(todo);
        }

        [FunctionName("GetTodos")]
        public static IActionResult GetTodos(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo")]HttpRequest req, ILogger log)
        {
            log.LogInformation("Getting todo list items");
            return new OkObjectResult(items);
        }

        [FunctionName("GetTodoById")]
        public static IActionResult GetTodoById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo/{id}")]HttpRequest req, ILogger log, string id)
        {
            var todo = items.FirstOrDefault(x => x.Id == id);
            if(todo == null)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(todo);
        }

        [FunctionName("UpdateTodo")]
        public static async Task<IActionResult> UpdateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo/{id}")]HttpRequest req, ILogger log, string id)
        {
            var todo = items.FirstOrDefault(x => x.Id == id);
            if(todo == null)
            {
                return new NotFoundResult();
            }
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updated = JsonConvert.DeserializeObject<TodoUpdateModel>(requestBody);

            todo.IsCompleted = updated.IsCompleted;
            if(!string.IsNullOrEmpty(updated.TaskDescription))
            {
                todo.TaskDescription = updated.TaskDescription;
            }
            return new OkObjectResult(todo);
        }

        [FunctionName("DeleteTodo")]
        public static IActionResult DeleteTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todo/{id}")]HttpRequest req, ILogger log, string id)
        {
            var todo = items.FirstOrDefault(x => x.Id == id);
            if(todo == null)
            {
                return new NotFoundResult();
            }
            items.Remove(todo);
            return new OkResult();/* Notice that, this isn't OkObjectResult like all other APIs and that's because
                                   the body of this response is empty. We've got nothing to return other than 200-OK
                                    indicating that we've deleted this todo item. */
        }
    }
}
