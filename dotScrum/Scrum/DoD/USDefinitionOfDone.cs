using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dotScrum.Scrum.DoD
{
    public class USDefinitionOfDone : DefinitionOfDone
    {
        private ISet<DoDPoint> usPoints;

        public USDefinitionOfDone()
        {
            usPoints = new HashSet<DoDPoint>();

            ISet<DoDPoint> dodPoints = getPoints();
            foreach (DoDPoint point in dodPoints)
            {
                usPoints.Add(point.getCopy());
            }
        }
    }
}