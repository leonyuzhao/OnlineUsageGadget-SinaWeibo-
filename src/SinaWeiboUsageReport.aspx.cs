using System;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Data;
using System.Text;

public partial class _SinaWeiboUsageReport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        InitialControls();
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        string connectStr = System.Web.Configuration.WebConfigurationManager.AppSettings["connectstr"];
        if (string.IsNullOrEmpty(connectStr)) { return; }

        Utility.Database.AccessDB db = new Utility.Database.AccessDB(connectStr);
        string query = GenerateSQL();
        DataTable dt = db.GetData(query);
        
        // Generate data for chart
        Dictionary<string, string> data = new Dictionary<string, string>();
        foreach (DataRow r in dt.Rows)
        {
            switch (typeSelection.Items[typeSelection.SelectedIndex].Value)
            {
                case "HOUR": data.Add(String.Format("{0} {1}:00", Convert.ToDateTime(r[1]).ToString("dd/MM/yyyy"), r[2].ToString()), r[0].ToString()); break;
                case "DAY": data.Add(Convert.ToDateTime(r[1]).ToString("dd/MM/yyyy"), r[0].ToString()); break;
                case "MONTH": data.Add(String.Format("{0}/{1}", r[1].ToString(), r[2].ToString()), r[0].ToString()); break;
                case "YEAR": data.Add(r[1].ToString(), r[0].ToString()); break;
                default: data.Add(Convert.ToDateTime(r[1]).ToString("dd/MM/yyyy"), r[0].ToString()); break;
            }
        }

        // Settings
        // Package List
        List<string> packageList = new List<string>();
        packageList.Add("corechart");

        Utility.Controls.GoogleChart chart = new Utility.Controls.GoogleChart();
        chart.PackageList = packageList;
        chart.Data = data;
        chart.xName = "Date";
        chart.yName = "Minutes";
        chart.Title = "Sina Weibo Usage Chart";
        chart.Width = 1024;
        chart.Height = 500;
        chart.Generate();
        chartHolder.Controls.Add(chart);

        // Detail
        DataView dv = new DataView(dt);
        switch (typeSelection.Items[typeSelection.SelectedIndex].Value)
        {
            case "HOUR": dv.Sort = "[Activity Date] DESC, [Time Slot]  DESC"; break;
            case "DAY": dv.Sort = "[Activity Date] DESC"; break;
            case "MONTH": dv.Sort = "[Year] DESC, [Month] DESC"; break;
            case "YEAR": dv.Sort = "[Year] DESC"; break;
            default: dv.Sort = "[Activity Date] DESC"; break;
        }
        detailGrid.DataSource = dv;
        detailGrid.DataBind();

        chartContent.Visible = (data.Count > 0);
        detailContent.Visible = (data.Count > 0);
        noDataContent.Visible = (data.Count == 0);
    }

    private void InitialControls()
    {
        if (typeSelection.Items.Count == 0)
        {
            typeSelection.Items.Add(new ListItem("By Day", "DAY"));
            typeSelection.Items.Add(new ListItem("By Month", "MONTH"));
            typeSelection.Items.Add(new ListItem("By Year", "YEAR"));
            typeSelection.Items.Add(new ListItem("By Hour", "HOUR"));
            typeSelection.SelectedIndex = 0;
        }
        if (yearSelection.Items.Count == 0)
        {
            yearSelection.Items.Add(new ListItem("Please Select", ""));
            int y = 2013;
            while (y <= DateTime.Now.Year)
            {
                yearSelection.Items.Add(new ListItem(y.ToString(), y.ToString()));
                y += 1;
            }
            yearSelection.SelectedIndex = 0;
        }
        if (monthSelection.Items.Count == 0)
        {
            monthSelection.Items.Add(new ListItem("Please Select", ""));
            for (int m = 1; m < 13; m++)
            {
                monthSelection.Items.Add(new ListItem(m.ToString(), m.ToString()));
            }
            monthSelection.SelectedIndex = 0;
        }
        if (daySelection.Items.Count == 0)
        {
            daySelection.Items.Add(new ListItem("Please Select", ""));
            for (int d = 1; d < 32; d++)
            {
                daySelection.Items.Add(new ListItem(d.ToString(), d.ToString()));
            }
            daySelection.SelectedIndex = 0;
        }
        if (timeSelection.Items.Count == 0)
        {
            timeSelection.Items.Add(new ListItem("Please Select", ""));
            for (int t = 0; t < 24; t++)
            {
                timeSelection.Items.Add(new ListItem(t.ToString(), t.ToString()));
            }
            timeSelection.SelectedIndex = 0;
        }
    }

    protected string FormatDate(DataRowView row)
    {
        switch (typeSelection.Items[typeSelection.SelectedIndex].Value)
        {
            case "HOUR": return String.Format("{0} {1}:00", Convert.ToDateTime(row[1]).ToString("dd/MM/yyyy"), row[2].ToString());
            case "DAY": return Convert.ToDateTime(row[1]).ToString("dd/MM/yyyy");
            case "MONTH": return String.Format("{0}/{1}", row[1].ToString(), row[2].ToString());
            case "YEAR": return row[1].ToString();
            default: return Convert.ToDateTime(row[1]).ToString("dd/MM/yyyy");
        }
    }

    private string GenerateSQL()
    {
        string subQuery = "";
        switch (typeSelection.Items[typeSelection.SelectedIndex].Value)
        {
            case "HOUR": subQuery = " [Activity Date], [Time Slot] "; break;
            case "DAY": subQuery = " [Activity Date] "; break;
            case "MONTH": subQuery = " [Year], [Month] "; break;
            case "YEAR": subQuery = " [Year] "; break;
            default: subQuery = " [Activity Date] "; break;
        }
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(" SELECT SUM([Total Mins]), ");
        sb.AppendLine(subQuery);
        sb.AppendLine(" FROM [Activity] ");
        sb.AppendLine(" WHERE 1 = 1 ");
        if (startDateTxt.Value != "") { sb.AppendLine(String.Format(" AND [Activity Date] >= #{0}# ", DateTime.Parse(startDateTxt.Value).ToString("MM/dd/yyyy"))); }
        if (endDateTxt.Value != "") { sb.AppendLine(String.Format(" AND [Activity Date] <= #{0}# ", DateTime.Parse(endDateTxt.Value).ToString("MM/dd/yyyy"))); }
        if (yearSelection.SelectedIndex > 0) { sb.AppendLine(String.Format(" AND [Year] = {0} ", yearSelection.Items[yearSelection.SelectedIndex].Value)); }
        if (monthSelection.SelectedIndex > 0) { sb.AppendLine(String.Format(" AND [Month] = {0} ", monthSelection.Items[monthSelection.SelectedIndex].Value)); }
        if (daySelection.SelectedIndex > 0) { sb.AppendLine(String.Format(" AND [Day] = {0} ", daySelection.Items[daySelection.SelectedIndex].Value)); }
        if (timeSelection.SelectedIndex > 0) { sb.AppendLine(String.Format(" AND [Time Slot] = {0} ", timeSelection.Items[timeSelection.SelectedIndex].Value)); }
        sb.AppendLine(" GROUP BY ");
        sb.AppendLine(subQuery);
        sb.AppendLine(" HAVING 1 = 1 ");
        if (minValTxt.Value != "") { sb.AppendLine(String.Format(" AND SUM([Total Mins]) >= {0} ", minValTxt.Value.ToString())); }
        if (maxValTxt.Value != "") { sb.AppendLine(String.Format(" AND SUM([Total Mins]) <= {0} ", maxValTxt.Value.ToString())); }
        sb.AppendLine(" ORDER BY ");
        sb.AppendLine(subQuery);
        return sb.ToString();
    }
}