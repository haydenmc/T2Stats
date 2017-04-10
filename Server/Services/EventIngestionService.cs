using System;
using System.Collections.Generic;
using T2Stats.Models;

namespace T2Stats.Services
{
    /// <summary>
    /// The EventIngestionService acts as a buffer between incoming events in the database.
    /// It correlates incoming events and flattens them before committing them.
    /// </summary>
    public class EventIngestionService
    {
        /// <summary>
        /// Transient class that serves to store pending event information.
        /// </summary>
        private class PendingEvent
        {
            public Event Ev;
            public List<(string reporterGuid, string reporterName)> Reporters;
        }

        /// <summary>
        /// Timeout before a pending event is committed.
        /// </summary>
        private const int EventCommitTimeoutSeconds = 5;

        /// <summary>
        /// List of events pending ingestion.
        /// </summary>
        private List<PendingEvent> pendingEvents = new List<PendingEvent>();

        /// <summary>
        /// Reference to the database.
        /// </summary>
        private ApplicationDbContext db;

        /// <summary>
        /// Instantiates EventIngestionService with reference to database.
        /// </summary>
        /// <param name="db"></param>
        public EventIngestionService(ApplicationDbContext db)
        {
            this.db = db;
        }

        /// <summary>
        /// Records a new event
        /// </summary>
        /// <param name="ev"></param>
        /// <param name="reporterGuid"></param>
        /// <param name="reporterName"></param>
        public void RecordEvent(Event ev, string reporterGuid, string reporterName)
        {
            // See if there's a metching event in our pending store.
            for (var i = 0; i < pendingEvents.Count; i++)
            {
                if (ev.EventCompareTo(pendingEvents[i].Ev))
                {
                    pendingEvents[i].Reporters.Add((reporterGuid: reporterGuid, reporterName: reporterName));
                    return;
                }
            }
            // Otherwise add a new one and set the clock
            var pendingEv = new PendingEvent()
            {
                Ev = ev,
                Reporters = new List<(string reporterGuid, string reporterName)>()
                {
                    (reporterGuid: reporterGuid, reporterName: reporterName)
                }
            };
            pendingEvents.Add(pendingEv);
        }
    }
},