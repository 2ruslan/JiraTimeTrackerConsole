using Alba.CsConsoleFormat;
using JiraTimeTracker.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JiraTimeTracker
{
    class Presenter
    {
        private long NoShowInterval = 5 * 60; // 5min
        private TimeSpan ControlTime = new TimeSpan(8, 0, 0);

        LineThickness headerThickness = new LineThickness(LineWidth.None, LineWidth.None);
        LineThickness rowThickness = new LineThickness(LineWidth.Single, LineWidth.Single);

        public void ShowData(Repository repo, DateTime dateFrom, DateTime dateTo)
        {
            var colData = new List<IEnumerator<Cell>>();
            var colTotal = new List<TimeSpan>();
            var dates = new List<DateTime>();

            var onDate = dateFrom;
            while (onDate <= dateTo)
            {
                var col = repo.Worklogs.Where(q => q.started.Date == onDate).OrderBy(q => q.started).ToList();
                colTotal.Add(new TimeSpan(0, 0, col.Sum(x => x.timeSpentSeconds)));

                colData.Add(GetColumnData(col, repo.Issues).GetEnumerator());

                dates.Add(onDate);
                onDate = onDate.AddDays(1);
            }
            
            

            var grid = new Grid
            {
                Color = ConsoleColor.Gray,
                Columns = {dates.Select(itm =>  GridLength.Star(1))},
                Children = {dates.Select(itm => new Cell(itm.ToString("dd.MM.yyyy ddd")) { Stroke = headerThickness, Align = Align.Center, Color = ConsoleColor.Cyan})}

            };

            // total row
            foreach (var col in colTotal)
            {
                if (col.Hours > 0 || col.Minutes > 0)
                {
                    var delta = ControlTime - col;
                    var deltaTxt = delta.Hours > 0 || delta.Minutes > 0 ?  $" ({delta.Hours.ToString("D2")}:{delta.Minutes.ToString("D2")})" : "";
                    grid.Children.Add(new Cell($"{col.Hours.ToString("D2")}:{col.Minutes.ToString("D2")}{deltaTxt}") { Stroke = rowThickness, Color = col < ControlTime ? ConsoleColor.Red : ConsoleColor.Green, Align = Align.Center});
                }
                else
                {
                    grid.Children.Add(new Cell() { Stroke = rowThickness, Color = ConsoleColor.Red, Align = Align.Center });
                }
            }

            


            // create table
            bool exist;
            do
            {
                exist = false;
                var row = new List<Cell>();
                foreach (var cell in colData)
                {
                    if (cell.MoveNext())
                    {
                        row.Add(cell.Current);
                        exist = true;
                    }
                    else
                        row.Add(new Cell() { Stroke = rowThickness });
                }

                if (exist)
                    row.ForEach(r => grid.Children.Add(r));

            } while (exist);


            var doc = new Document(
                new Cell($"{dateFrom.ToString("dd.MM.yyyy")} - {dateTo.ToString("dd.MM.yyyy")}") { Stroke = rowThickness, Color = ConsoleColor.Yellow, Align = Align.Center },
                grid);
            
             ConsoleRenderer.RenderDocument(doc);
        }

        private List<Cell> GetColumnData(List<Worklog> worklog, List<Issue> issues)
        {
            var data = new List<Cell>();

            if (worklog.Count == 0)
                return data;

            var prevTime = worklog[0].started;

            foreach (var w in worklog)
            {
                if (w.started.Ticks - prevTime.Ticks > NoShowInterval)
                {
                    var timeSpent = (w.started - prevTime);
                    var timeSpentStr = (timeSpent.Hours > 0 ? $"{timeSpent.Hours}h " : "") + $"{timeSpent.Minutes}m";
                    data.Add(new Cell($"{FormatTime(prevTime)} - {FormatTime(w.started)}\n{timeSpentStr}\nFREE TIME") { Stroke = rowThickness, Color = ConsoleColor.Green });
                }

                var endTime = w.started.AddSeconds(w.timeSpentSeconds);

                var issue = issues.Where(i => i.id == w.issueId).First();

                data.Add(new Cell($"{FormatTime(w.started)} - {FormatTime(endTime)}\n{w.timeSpent}\n{issue.key}\n{issue.fields.summary}") { Stroke = rowThickness, Color = ConsoleColor.Yellow });

                prevTime = endTime;

            }

            return data;
        }

        private string FormatTime(DateTime dt)
        {
            return dt.ToString("HH:mm");
        }
    }
}
