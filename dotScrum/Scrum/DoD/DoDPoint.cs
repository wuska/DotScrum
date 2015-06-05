using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dotScrum.Scrum.DoD
{
    public class DoDPoint
    {
        private bool fulfilled;
        private String title;
        private String description;

        DoDPoint() { }
        DoDPoint(bool fulfilled, String title, String description)
        {
            this.fulfilled = fulfilled;
            this.title = title;
            this.description = description;
        }

        public void setFulfilled(bool fulfilled)
        {
            this.fulfilled = fulfilled;
        }

        public void setTitle(String title)
        {
            this.title = title;
        }

        public void setDescription(String description)
        {
            this.description = description;
        }

        public bool getFulfilled()
        {
            return this.fulfilled;
        }

        public String getTitle()
        {
            return this.title;
        }

        public String getDescription()
        {
            return this.description;
        }

        public DoDPoint getCopy()
        {
            return new DoDPoint(this.fulfilled, this.title, this.description);
        }

        public bool Equals(DoDPoint dodPoint)
        {
            if (this.fulfilled == dodPoint.getFulfilled() &&
                this.title.Equals(dodPoint.getTitle()) &&
                this.description.Equals(dodPoint.getDescription()))
            {
                return true;
            }

            return false;
        }
    }
}