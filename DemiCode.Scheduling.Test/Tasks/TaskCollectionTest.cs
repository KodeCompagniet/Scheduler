using System;
using System.Collections;
using System.Collections.Generic;
using DemiCode.Scheduling.Schedules;
using DemiCode.Scheduling.Test;
using DemiCode.Scheduling.Test.Mocks;
using NUnit.Framework;

namespace DemiCode.Scheduling.Tasks.Test
{
    /// <summary>
    ///This is a test class for DemiCode.Scheduling.Tasks.TaskCollection and is intended
    ///to contain all DemiCode.Scheduling.Tasks.TaskCollection Unit Tests
    ///</summary>
    [TestFixture]
    public class TaskCollectionTest
    {

        private static Scheduler CreateSchedulerMock(out TaskCollection taskCollection)
        {
            Scheduler scheduler = new Scheduler();

            try
            {
                scheduler.Add(new NamedOneShotSchedule("scheduletest"));
                TaskCollection target = new TaskCollection(scheduler);

                ITask task = new TaskMock();
                IDictionary parameters = new Dictionary<string, string>();
                string scheduleName = "scheduletest";

                target.Add(task, parameters, scheduleName);

                taskCollection = target;
                return scheduler;
            }
            catch
            {
                scheduler.Dispose();
                throw;
            }
        }

        [Test]
        public void DisposeTest()
        {
            DisposableTask.ResetTrackerFields();

            using (Scheduler scheduler = new Scheduler())
            {
                using (TaskCollection col = new TaskCollection(scheduler))
                {
                    DisposableTask task = new DisposableTask();
                    col.Add(task);
                }
            }

            Assert.IsTrue(DisposableTask.DisposeWasCalled);
        }

        /// <summary>
        ///A test for Add (ITask)
        ///</summary>
        [Test]
        public void AddTest()
        {
            using (Scheduler scheduler = new Scheduler())
            {
                TaskCollection target = DemiCode_Scheduling_Tasks_TaskCollectionAccessor.CreatePrivate(scheduler);
                ITask task = new TaskMock();
                target.Add(task);

                Assert.AreEqual(1, target.Count);
            }
        }

        /// <summary>
        ///A test for Add (ITask)
        ///</summary>
        [Test]
        public void AddRange()
        {
            using (Scheduler scheduler = new Scheduler())
            {
                var taskCollection = DemiCode_Scheduling_Tasks_TaskCollectionAccessor.CreatePrivate(scheduler);
                var tasks = new[]{ new TaskMock(), new TaskMock() };
                taskCollection.AddRange(tasks);

                Assert.AreEqual(2, taskCollection.Count);
            }
        }


        /// <summary>
        ///A test for Add (ITask)
        ///</summary>
        [Test]
        public void AddRangeToNamedSchedules()
        {
            using (Scheduler scheduler = new Scheduler())
            {
                scheduler.Add(new IntervalSchedule("s1", DateTime.Now, TimeSpan.FromSeconds(5)));
                scheduler.Add(new IntervalSchedule("s2", DateTime.Now, TimeSpan.FromSeconds(10)));

                var taskCollection = DemiCode_Scheduling_Tasks_TaskCollectionAccessor.CreatePrivate(scheduler);
                var tasks = new[] { new TaskMock(), new TaskMock() };
                taskCollection.AddRange(tasks, "s1");
                taskCollection.AddRange(tasks, "s2");

                Assert.AreEqual(4, taskCollection.Count);
            }
        }

        /// <summary>
        ///A test for Add (ITask, IDictionary)
        ///</summary>
        [Test]
        public void AddTest1()
        {
            using (Scheduler scheduler = new Scheduler())
            {
                TaskCollection target = DemiCode_Scheduling_Tasks_TaskCollectionAccessor.CreatePrivate(scheduler);

                ITask task = new TaskMock();
                IDictionary parameters = new Dictionary<string, string>();

                target.Add(task, parameters);

                Assert.AreEqual(1, target.Count);
            }
        }

        /// <summary>
        ///A test for Add (ITask, IDictionary, string)
        ///</summary>
        [Test]
        public void AddTest2()
        {
            using (Scheduler scheduler = new Scheduler())
            {
                scheduler.Add(new NamedOneShotSchedule("scheduletest"));
                TaskCollection target = DemiCode_Scheduling_Tasks_TaskCollectionAccessor.CreatePrivate(scheduler);

                ITask task = new TaskMock();
                IDictionary parameters = new Dictionary<string, string>();
                string scheduleName = "scheduletest";

                target.Add(task, parameters, scheduleName);

                Assert.AreEqual(1, target.Count);
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void AddWithInvalidScheduleNameTest()
        {
            using (Scheduler scheduler = new Scheduler())
            {
                scheduler.Add(new NamedOneShotSchedule("scheduletest"));
                TaskCollection target = new TaskCollection(scheduler);

                ITask task = new TaskMock();
                IDictionary parameters = new Dictionary<string, string>();
                string scheduleName = "bogusschedulename";

                target.Add(task, parameters, scheduleName);
            }
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void ClearIsNITest()
        {
            using (Scheduler scheduler = new Scheduler())
            {
                ICollection<ITask> target = new TaskCollection(scheduler);
                target.Clear();
            }
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void RemoveIsNITest()
        {
            using (Scheduler scheduler = new Scheduler())
            {
                ICollection<ITask> target = new TaskCollection(scheduler);
                target.Remove(null);
            }
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void RemoveAtIsNITest()
        {
            using (Scheduler scheduler = new Scheduler())
            {
                IList<ITask> target = new TaskCollection(scheduler);
                target.RemoveAt(0);
            }
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void InsertIsNITest()
        {
            TaskCollection collection;
            using (CreateSchedulerMock(out collection))
            {
                ((IList<ITask>)collection).Insert(0, null);
            }
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void ItemSetIsNITest()
        {
            TaskCollection collection;
            using (CreateSchedulerMock(out collection))
            {
                collection[0] = new TaskMock();
            }
        }

        [Test]
        public void ContainsTest()
        {
            TaskCollection collection;
             using (CreateSchedulerMock(out collection))
             {
                 Assert.IsTrue(((ICollection<ITask>) collection).Contains(collection[0]));
                 ITask mock = new TaskMock();
                 Assert.IsFalse(((ICollection<ITask>)collection).Contains(mock));
             }
        }

        [Test]
        public void CopyToTest()
        {
            TaskCollection collection;
            using (CreateSchedulerMock(out collection))
            {
                ITask[] tasks = new ITask[collection.Count];

                ((ICollection<ITask>)collection).CopyTo(tasks, 0);
                foreach (ITask task in tasks)
                {
                    Assert.IsTrue(((ICollection<ITask>)collection).Contains(task));    
                }
            }
        }

        [Test]
        public void GetEnumeratorTest()
        {
            TaskCollection collection;
            using (CreateSchedulerMock(out collection))
            {
                IEnumerator<ITask> enumerator = ((IEnumerable<ITask>) collection).GetEnumerator();
                Assert.IsNotNull(enumerator);
            }
        }

        [Test]
        public void IndexOfTest()
        {
            TaskCollection collection;
            using (CreateSchedulerMock(out collection))
            {
                Assert.AreEqual(0, ((IList<ITask>) collection).IndexOf(collection[0]));
            }
        }

        /// <summary>
        ///A test for TaskCollection (Scheduler)
        ///</summary>
        
        [Test]
        public void ConstructorTest1()
        {
            using (Scheduler scheduler = new Scheduler())
            {
                TaskCollection target = DemiCode_Scheduling_Tasks_TaskCollectionAccessor.CreatePrivate(scheduler);
            }
        }

        /// <summary>
        ///A test for this[int index]
        ///</summary>
        [Test]
        public void ItemTest()
        {
            using (Scheduler scheduler = new Scheduler())
            {
                TaskCollection target = DemiCode_Scheduling_Tasks_TaskCollectionAccessor.CreatePrivate(scheduler);

                ITask val = new TaskMock();
                int index = 0; 

                target.Add(val);

                Assert.AreEqual(val, target[index],
                                "DemiCode.Scheduling.Tasks.TaskCollection.this was not set correctly.");
            }
        }

    }
}
