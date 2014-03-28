Online Usage Gadget [Sina Weibo]
================================

* **Logger**

Logger engine contains two parts.

Part I: Daily activity tracker

This mode (Tracker) runs every 5 mins. It detects whether user is online or offline. If online status, the tracker will record the current time into a log file. If total recorded time is greater than defined quota (15 mins by default), an automated email will send to pre-defined address for notification purpose.

Part II: Activity summary

Everyday morning, the engine will run under (Logger) mode to summary and archive previous activities into database which is used for report purpose.


* **Report**

**Overview**

![ScreenShot](https://raw.githubusercontent.com/leonyuzhao/OnlineUsageGadget-SinaWeibo-/master/screenshots/ReportOverview.jpg)

**Detail Information**

![ScreenShot](https://raw.githubusercontent.com/leonyuzhao/OnlineUsageGadget-SinaWeibo-/master/screenshots/ReportWithDetail.jpg)

**No Data**

![ScreenShot](https://raw.githubusercontent.com/leonyuzhao/OnlineUsageGadget-SinaWeibo-/master/screenshots/ReportWithNoData.jpg)


The report aspx page helps you to visualise your online usage data based on Logger console's generation.

Setup & Installation
====================

* **Logger**

Add COM references of following items:

  ```
  Microsoft ActiveX Data Object 2.8 Library
  Microsoft ADO Ext. 2.8 for DDL and Security
  ```

Modify web.config add following app setting record:
  
  ```
  <add key="mode" value="#Tracker|Logger#" />
  <add key="logpath" value="#LOGFILESAVEPATH#" />
    
  <add key="username" value="#EMAIL|WEIBOUSERNAME#" />
  <add key="password" value="#EMAIL|WEIBOPASSWORD#" />
  <add key="appkey" value="#WEIBODEVELOPERAPPKEY#" />
  <add key="targetuid" value="#WEIBOUSERUID#" />
  <add key="toaddress" value="#NOTIFYADDRESS#" />
  <add key="ccaddress" value="#CCADDRESS#" />
  <add key="hassentlogpath" value="#HASEMAILSENTLOGPATH#" />

  <add key="archivepath" value="#LOGFILEARCHIVEPATH#"/>
  <add key="mdbfile" value="#PROCESSEDDBPATH#"/>
  <add key="connectstr" value="#CONNECTIONSTRING#"/>
  ```
  
Make sure add following <a href="https://github.com/leonyuzhao/Utility-CodeSnippet-" target="_blank">references</a> into your project before compile. 

  ```
  IO.cs
  Web.cs
  Database.cs
  ```
  
* **Report**

Modify web.config add following app setting record:
  
  ```
  <add key="connectstr" value="#CONNTECTIONSTRING#"/>
  ```

Sample Connection String:

  ```
  "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=#DATABASELOCATION(mdb)#;"
  ```
  
Make sure add following <a href="https://github.com/leonyuzhao/Utility-CodeSnippet-" target="_blank">references</a> into your project before compile. 

Chart.cs/Database.cs utility file should be put within the same folder as main aspx page. Or modify the follow link to refect the change:

  ```
  <%@ Assembly Src="Chart.cs" %>
  <%@ Assembly Src="Database.cs" %>
  ```
  
Author
======
Leon Yu Zhao
