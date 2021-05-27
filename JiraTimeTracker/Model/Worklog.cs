using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraTimeTracker.Model
{

    public class Author
    {
        public string name;
        public string key;

    }
    public class Worklog
    {
        public string self;
        public string comment;
        public DateTime created;
        public DateTime updated;
        public DateTime started;
        public string timeSpent;
        public int timeSpentSeconds;
        public string id;
        public string issueId;
        public Author author;
        public Author updateAuthor;
    }

    public class RootWorklog
    {
        public string expand;
        public string id;
        public string self;
        public string key;
        public List<Worklog> worklogs;
    }
}
