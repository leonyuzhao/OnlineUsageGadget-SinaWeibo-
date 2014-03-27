<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SinaWeiboUsageReport.aspx.cs" Inherits="_SinaWeiboUsageReport" %>

<%@ Import Namespace="System.Data" %>
<%@ Assembly Src="Chart.cs" %>
<%@ Assembly Src="Database.cs" %>
<!DOCTYPE html>

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
                <h3 class="text-muted">Sina Weibo Usage Report</h3>
                <hr />
            </div>
            <div class="row">
                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title">Report Filter</h3>
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
                    <div class="jumbotron" id="chartContent" runat="server" visible="false">
                        <asp:PlaceHolder ID="chartHolder" runat="server"></asp:PlaceHolder>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-12" id="detailContent" runat="server" visible="false">
                    <asp:Repeater ID="detailGrid" runat="server">
                        <HeaderTemplate>
                            <table class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>Date
                                        </th>
                                        <th>Minutes
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%#FormatDate(((System.Data.DataRowView)Container.DataItem)) %></td>
                                <td><%#((System.Data.DataRowView)Container.DataItem)[0] %></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate></tbody></table></FooterTemplate>
                    </asp:Repeater>
                </div>
                <div id="noDataContent" runat="server" class="col-lg-12">
                    <i>There's no data available.</i>
                </div>
            </div>
            <div class="footer">
                <hr />
                <p>
                    &copy; Leon Yu Zhao 2014
                </p>
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

    $(function () {
        $("#startDateTxt").datepicker({ dateFormat: 'dd/mm/yy' });
        $("#endDateTxt").datepicker({ dateFormat: 'dd/mm/yy' });
    });

    $("#btnGenerate").click(function () {
        document.forms[0].submit();
        return false;
    });

    $("#btnClear").click(function () {
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
        document.forms[0].submit();
        return false;
    });

// ]]>
</script>

