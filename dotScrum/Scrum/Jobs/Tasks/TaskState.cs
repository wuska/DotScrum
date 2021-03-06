﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dotScrum.Scrum.Jobs.UserStories.Tasks
{
    public class TaskState : JobState
    {
        private static ISet<String> possibleStates = new HashSet<String>();

        public override void addState(String state)
        {
            possibleStates.Add(state);
        }

        public override void deleteState(String state)
        {
            possibleStates.Remove(state);
        }

        public override void clearStates()
        {
            possibleStates.Clear();
        }

        public override ISet<String> getPossibleStates()
        {
            return possibleStates;
        }

        public override void setState(String state)
        {
            if (possibleStates.Contains(state))
            {
                this.state = state;
            }
        }

        public override string getState()
        {
            return this.state;
        }

        public bool Equals(TaskState taskState)
        {
            if (this.state.Equals(taskState.getState()))
            {
                return true;
            }

            return false;
        }
    }
}