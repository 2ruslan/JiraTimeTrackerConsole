using JiraTimeTracker.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JiraTimeTracker
{
    class Repository
    {
        private string UrlIssuesFormat = "/rest/api/2/search?jql= worklogAuthor = currentUser() and worklogDate >= \"{0}\" and worklogDate <= \"{1}\"";
        private string UrlWorklogsFormat = "/rest/api/2/issue/{0}/worklog";

        private JiraClient _jiraClient;
        private string _user;
        private string _pass;


        public Repository(string baseUrl, string user, string pass)
        {
            UrlIssuesFormat = baseUrl + UrlIssuesFormat;
            UrlWorklogsFormat = baseUrl + UrlWorklogsFormat;
            _user = user;
            _pass = pass;
        }

        public List<Issue> Issues { get; private set; }

        public List<Worklog> Worklogs { get; private set; } = new List<Worklog>();

        public void LoadData(DateTime dateFrom, DateTime dateTo)
        {
            Worklogs.Clear();
            Issues?.Clear();

            _jiraClient = new JiraClient(_user, _pass);
            LoadIssues(dateFrom, dateTo);

            foreach(var issue in Issues)
                LoadWorklogs(_user, issue.id);
        }


        private void LoadIssues(DateTime dateFrom, DateTime dateTo)
        {
            var url = string.Format(UrlIssuesFormat, dateFrom.ToString("yyyy-MM-dd"), dateTo.ToString("yyyy-MM-dd"));
            var json = _jiraClient.RunQuery(url);
            var root = JsonConvert.DeserializeObject<RootIssue>(json);

            Issues = root.issues;
        }

        private void LoadWorklogs(string name, string issueId)
        {
            if (Worklogs.Any(x => x.issueId == issueId))
                return;

            var url = string.Format(UrlWorklogsFormat, issueId);
            var json = _jiraClient.RunQuery(url);
            var root = JsonConvert.DeserializeObject<RootWorklog>(json);

            Worklogs.AddRange(root.worklogs.Where(x => x.author?.name == name));
        }

    }
}
