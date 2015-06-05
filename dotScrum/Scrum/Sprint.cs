using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using dotScrum.Scrum.Jobs;

namespace dotScrum.Scrum
{
    public class Sprint
    {
        private int duration;
        private int sprintNo;
        private IList<UserStory> userStories;

        public Sprint()
        {
            this.userStories = new List<UserStory>();
        }

        public int Duration
        {
            get { return duration; }
            set { duration = value; }
        }

        public int SprintNo
        {
            get { return sprintNo; }
            set { sprintNo = value; }
        }

        public IList<UserStory> getUserStories()
        {
            return this.userStories;
        }
    }
}