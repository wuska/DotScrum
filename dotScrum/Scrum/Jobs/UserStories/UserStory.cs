using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using dotScrum.Scrum.DoD;
using dotScrum.Scrum.Jobs.Tasks;

namespace dotScrum.Scrum.Jobs
{
    public class UserStory : Job
    {
        private USDefinitionOfDone dod;
        private IList<Task> tasks;

        public UserStory()
        {
            dod = new USDefinitionOfDone();
            tasks = new List<Task>();
        }

        public void addTask(Task task)
        {
            this.tasks.Add(task);
        }

        public void deleteTask(Task task)
        {
            this.tasks.Remove(task);
        }

        public void clearTasks()
        {
            this.tasks.Clear();
        }

        public IList<Task> getTasks()
        {
            return this.tasks;
        }

        public bool Equals(UserStory userStory)
        {
            if (this.value.Equals(userStory.getValue()) &&
                this.state.Equals(userStory.getState()) &&
                this.finishDate.Equals(userStory.getFinishDate()) &&
                this.title.Equals(userStory.getTitle()) &&
                this.description.Equals(userStory.getDescription()) &&
                this.tasks.Equals(userStory.getTasks()))
            {
                return true;
            }
                
            return false;
        }
    }
}