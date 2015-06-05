using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dotScrum.Scrum.Jobs
{
    public abstract class JobState
    {
        protected String state;

        public abstract void addState(String state);
        public abstract void deleteState(String state);
        public abstract void clearStates();
        public abstract ISet<String> getPossibleStates();
        public abstract void setState(String state);
        public abstract String getState();
    }
}