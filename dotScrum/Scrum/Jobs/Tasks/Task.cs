using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using dotScrum.Scrum.Jobs.UserStories.Tasks;

namespace dotScrum.Scrum.Jobs.Tasks
{
    public class Task : Job
    {
        public DateTime emptyDate = new DateTime(0, 0, 0);

        Task()
        {
            this.state = new TaskState();
            this.finishDate = emptyDate;
        }

        Task(String title, String description, int value, DateTime finishDate, TaskState state)
        {
            this.title = title;
            this.description = description;
            this.value = value;
            this.finishDate = finishDate;
            this.state = state;
        }

        public bool Equals(Task task)
        {
            if (this.value.Equals(task.getValue()) &&
                this.state.Equals(task.getState()) &&
                this.title.Equals(task.getTitle()) &&
                this.description.Equals(task.getDescription()) &&
                this.finishDate.Equals(task.getFinishDate()))
            {
                return true;
            }

            return false;
        }
    }
}