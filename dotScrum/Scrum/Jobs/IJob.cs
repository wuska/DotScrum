using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotScrum.Scrum.Jobs
{
    public interface IJob
    {
        void setTitle(String title);
        String getTitle();
        void setDescription(String description);
        String getDescription();
        void setValue(int value);
        int getValue();
        void setFinishDate(DateTime finishDate);
        DateTime getFinishDate();
        void setState(JobState state);
        JobState getState();
    }
}
