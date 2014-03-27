Online Usage Gadget [Sina Weibo]
================================
1. **Logger**

2. **Report**

**Overview**

![ScreenShot](https://raw.githubusercontent.com/leonyuzhao/OnlineUsageGadget-SinaWeibo-/master/screenshots/ReportOverview.jpg)

**Detail Information**

![ScreenShot](https://raw.githubusercontent.com/leonyuzhao/OnlineUsageGadget-SinaWeibo-/master/screenshots/ReportWithDetail.jpg)

**No Data**

![ScreenShot](https://raw.githubusercontent.com/leonyuzhao/OnlineUsageGadget-SinaWeibo-/master/screenshots/ReportWithNoData.jpg)


The report aspx page helps you to visualise your online usage data based on Logger console's generation.

Setup & Installation
====================
1. **Logger**

2. **Report**

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
