using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using dotScrum.Scrum;
using dotScrum.Scrum.DoD;
using dotScrum.Scrum.Jobs;

namespace dotScrum.Project
{
    public class Project
    {
        private DefinitionOfDone dod;
        private IList<UserStory> backlog;
        private IList<Sprint> sprints;

        public Project()
        {
            dod = new DefinitionOfDone();
            backlog = new List<UserStory>();
            sprints = new List<Sprint>();
        }
    }
}