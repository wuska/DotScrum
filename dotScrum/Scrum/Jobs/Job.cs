using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dotScrum.Scrum.Jobs
{
    public class Job : IJob
    {
        protected String title;
        protected String description;
        protected int value;
        protected DateTime finishDate;
        protected JobState state;

        public void setTitle(String title)
        {
            this.title = title;
        }

        public String getTitle()
        {
            return this.title;
        }

        public void setDescription(String description)
        {
            this.description = description;
        }

        public String getDescription()
        {
            return this.description;
        }

        public void setValue(int value)
        {
            this.value = value;
        }

        public int getValue()
        {
            return this.value;
        }

        public void setFinishDate(DateTime finishDate)
        {
            this.finishDate = finishDate;
        }

        public DateTime getFinishDate()
        {
            return this.finishDate;
        }

        public void setState(JobState state)
        {
            this.state = state;
        }

        public JobState getState()
        {
            return this.state;
        }
    }
}