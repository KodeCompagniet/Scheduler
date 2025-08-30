//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Xml;

//namespace DemiCode.Scheduling.Schedules
//{
//    public class CompositeSchedule : ScheduleBase
//    {
//        #region static helper functions
//        private static DateTime GetFirstOccurrence(IEnumerable<ISchedule> schedules)
//        {
//            DateTime firstOccurrence = DateTime.MaxValue;

//            foreach (ISchedule schedule in schedules)
//            {
//                if (schedule.FirstOccurrence < firstOccurrence)
//                {
//                    firstOccurrence = schedule.FirstOccurrence;
//                }
//            }

//            return firstOccurrence;
//        }
//        #endregion

//        private List<ISchedule> _schedules = new List<ISchedule>();

//        #region ctors
//        public CompositeSchedule(IEnumerable<ISchedule> schedules, object additionalData)
//            : base(GetFirstOccurrence(schedules), additionalData)
//        {
//            _schedules.AddRange(schedules);
//        }

//        public CompositeSchedule(XmlNode config)
//            : base(config)
//        {
//            throw new NotImplementedException();
//        }
//        #endregion

//        public override bool MoveToFirstFrom(DateTime time)
//        {
//            for (int i = _schedules.Count - 1; i >= 0; i--)
//            {
//                if (!_schedules[i].MoveToFirstFrom(time))
//                {
//                    // Signal endtime ?
//                    _schedules.RemoveAt(i);
//                }
//            }

//            if (_schedules.Count > 0)
//            {
//                _schedules.Sort();

//                _currentOccurrence = _schedules[0].CurrentOccurrence;
//                return true;
//            }
//            else
//            {
//                return false;
//            }
//        }

		
//    }
//}
