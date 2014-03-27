<%@ Page Language="C#" %>

<!DOCTYPE html>

<script runat="server">

  protected void Page_Load(object sender, EventArgs e)
  {
    InitialControls();
  }

  protected void Page_PreRender(object sender, EventArgs e)
  {
    //Response.Write("Type: " + typeSelection.Items[typeSelection.SelectedIndex].Value + "<br />");
    //Response.Write("Start Date: " + startDateTxt.Value + "<br />");
    //Response.Write("End Date: " + endDateTxt.Value + "<br />");
    //Response.Write("Year: " + yearSelection.Items[yearSelection.SelectedIndex].Value + "<br />");
    //Response.Write("Month: " + monthSelection.Items[monthSelection.SelectedIndex].Value + "<br />");
    //Response.Write("Day: " + daySelection.Items[daySelection.SelectedIndex].Value + "<br />");
    //Response.Write("TimeSlot: " + timeSelection.Items[timeSelection.SelectedIndex].Value + "<br />");
    //Response.Write("Min: " + minValTxt.Value + "<br />");
    //Response.Write("Max: " + maxValTxt.Value + "<br />");
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
  
</script>

<html>
<head>
  <title>Sina Weibo Usage Report</title>
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <link rel="stylesheet" href="http://code.jquery.com/ui/1.10.3/themes/smoothness/jquery-ui.css" />
  <!-- Latest compiled and minified CSS -->
  <link rel="stylesheet" href="//netdna.bootstrapcdn.com/bootstrap/3.1.1/css/bootstrap.min.css">
  <!-- Optional theme -->
  <link rel="stylesheet" href="//netdna.bootstrapcdn.com/bootstrap/3.1.1/css/bootstrap-theme.min.css">
</head>
<body>
  <form runat="server">
  <div class="container">
    <div class="header">
      <h3 class="text-muted">
        Sina Weibo Usage Report</h3>
      <hr />
    </div>
    <div class="row">
      <div class="col-lg-12">
        <div class="panel panel-primary">
          <div class="panel-heading">
            <h3 class="panel-title">
              Report Filter</h3>
          </div>
          <div class="panel-body">
            <div class="row">
              <div class="col-lg-12">
                <div class="input-group">
                  <span class="input-group-addon">Report Type</span>
                  <select class="form-control" id="typeSelection" runat="server">
                  </select>
                </div>
              </div>
            </div>
            <br />
            <div class="row">
              <div class="col-lg-6">
                <div class="input-group">
                  <span class="input-group-addon">Start Date</span>
                  <input type="text" class="form-control" placeholder="Start Date" id="startDateTxt"
                    runat="server">
                </div>
              </div>
              <div class="col-lg-6">
                <div class="input-group">
                  <span class="input-group-addon">End Date</span>
                  <input type="text" class="form-control" placeholder="End Date" id="endDateTxt" runat="server">
                </div>
              </div>
            </div>
            <br />
            <div class="row">
              <div class="col-lg-3">
                <div class="input-group">
                  <span class="input-group-addon">Year</span>
                  <select class="form-control" id="yearSelection" runat="server">
                  </select>
                </div>
              </div>
              <div class="col-lg-3">
                <div class="input-group">
                  <span class="input-group-addon">Month</span>
                  <select class="form-control" id="monthSelection" runat="server">
                  </select>
                </div>
              </div>
              <div class="col-lg-3">
                <div class="input-group">
                  <span class="input-group-addon">Day</span>
                  <select class="form-control" id="daySelection" runat="server">
                  </select>
                </div>
              </div>
              <div class="col-lg-3">
                <div class="input-group">
                  <span class="input-group-addon">Time Slot</span>
                  <select class="form-control" id="timeSelection" runat="server">
                  </select>
                </div>
              </div>
            </div>
            <br />
            <div class="row">
              <div class="col-lg-6">
                <div class="input-group">
                  <span class="input-group-addon">Min Val</span>
                  <input type="text" class="form-control" placeholder="Min Val" id="minValTxt" runat="server">
                </div>
              </div>
              <div class="col-lg-6">
                <div class="input-group">
                  <span class="input-group-addon">Max Val</span>
                  <input type="text" class="form-control" placeholder="Max Val" id="maxValTxt" runat="server">
                </div>
              </div>
            </div>
            <hr />
            <button type="button" class="btn btn-primary btn-lg" id="btnGenerate" runat="server">
              Generate</button>
            <button type="button" class="btn btn-primary btn-lg" id="btnClear" runat="server">
              Clear</button>
          </div>
        </div>
      </div>
    </div>
    <div class="row">
      <div class="col-lg-12">
        <div class="jumbotron">
          <h1>
            Genated Graph<br />
            To Be Added Here</h1>
        </div>
      </div>
    </div>
    <div class="row">
      <div class="col-lg-12">
        <table class="table table-striped">
          <thead>
            <tr>
              <th>
                Date
              </th>
              <th>
                Minutes
              </th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>
                27/03/2014
              </td>
              <td>
                14
              </td>
            </tr>
            <tr>
              <td>
                26/03/2014
              </td>
              <td>
                13
              </td>
            </tr>
            <tr>
              <td>
                25/03/2014
              </td>
              <td>
                44
              </td>
            </tr>
            <tr>
              <td>
                24/03/2014
              </td>
              <td>
                21
              </td>
            </tr>
            <tr>
              <td>
                23/03/2014
              </td>
              <td>
                54
              </td>
            </tr>
            <tr>
              <td>
                22/03/2014
              </td>
              <td>
                231
              </td>
            </tr>
            <tr>
              <td>
                21/03/2014
              </td>
              <td>
                11
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
    <div class="footer">
      <hr />
      <p>
        &copy; Leon Yu Zhao 2014</p>
    </div>
  </div>
  <!-- Placed at the end of the document so the pages load faster -->

  <script src="http://cdn.bootcss.com/jquery/1.10.2/jquery.min.js"></script>

  <script src="http://code.jquery.com/ui/1.10.3/jquery-ui.js"></script>

  <!-- Latest compiled and minified JavaScript -->

  <script src="//netdna.bootstrapcdn.com/bootstrap/3.1.1/js/bootstrap.min.js"></script>

  </form>
</body>
</html>

<script language="javascript" type="text/javascript">
// <!CDATA[

  $(function() {
    $("#startDateTxt").datepicker();
    $("#endDateTxt").datepicker();
  });

  $("#btnGenerate").click(function() {
    document.forms[0].submit();
    return false;
  });

  $("#btnClear").click(function() {
    var type = document.getElementById("<%=typeSelection.ClientID %>");
    if (type) { type.selectedIndex = 0; }
    var startDate = document.getElementById("<%=startDateTxt.ClientID %>");
    if (startDate) { startDate.value = ""; }
    var endDate = document.getElementById("<%=endDateTxt.ClientID %>");
    if (endDate) { endDate.value = ""; }
    var year = document.getElementById("<%=yearSelection.ClientID %>");
    if (year) { year.selectedIndex = 0; }
    var month = document.getElementById("<%=monthSelection.ClientID %>");
    if (month) { month.selectedIndex = 0; }
    var day = document.getElementById("<%=daySelection.ClientID %>");
    if (day) { day.selectedIndex = 0; }
    var time = document.getElementById("<%=timeSelection.ClientID %>");
    if (time) { time.selectedIndex = 0; }
    var min = document.getElementById("<%=minValTxt.ClientID %>");
    if (min) { min.value = ""; }
    var max = document.getElementById("<%=maxValTxt.ClientID %>");
    if (max) { max.value = ""; }
    return false;
  });
  
// ]]>
</script>

