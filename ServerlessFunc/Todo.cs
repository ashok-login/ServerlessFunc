using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessFunc
{
    //We use Todo class for serialization to JSON in responses to our API calls,
    //In this class, we don't want the end user to call any of the properties except TaskDescription.
    //Hence, created one another class TodoCreateModel
    public class Todo
    {
        //This is how to initialize a property.
        public string Id { get; set; } = Guid.NewGuid().ToString("n");
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public string TaskDescription { get; set; }
        public bool IsCompleted { get; set; }
    }
}
