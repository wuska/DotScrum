using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dotScrum.Scrum.DoD
{
    public class DefinitionOfDone
    {
        private static ISet<DoDPoint> points;

        public DefinitionOfDone()
        {
            points = new HashSet<DoDPoint>();
        }

        public void addPoint(DoDPoint dodPoint)
        {
            points.Add(dodPoint);
        }

        public bool deletePoint(DoDPoint dodPoint)
        {
            return points.Remove(dodPoint);
        }

        public void clearPoints()
        {
            points.Clear();
        }

        public ISet<DoDPoint> getPoints()
        {
            return points;
        }
    }
}