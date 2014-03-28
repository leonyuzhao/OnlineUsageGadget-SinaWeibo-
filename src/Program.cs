using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Configuration;

namespace OnlineUsageLogger
{
    class Program
    {
        enum Mode
        {
            Tracker,
            Logger
        }

        static void Main(string[] args)
        {
            Mode m = Mode.Tracker;
            logPath = ConfigurationSettings.AppSettings["logpath"];
            string modeStr = ConfigurationSettings.AppSettings["mode"];
            switch (modeStr.ToLower())
            {
                case "t":
                case "tracker":
                    m = Mode.Tracker;
                    break;
                case "l":
                case "logger":
                    m = Mode.Logger;
                    break;
            }

            // Tracker mode runs every 5 mins, track down if user is online or not
            if (m == Mode.Tracker)
            {
                username = ConfigurationSettings.AppSettings["username"];
                password = ConfigurationSettings.AppSettings["password"];
                url = ConfigurationSettings.AppSettings["statusurl"];
                appKey = ConfigurationSettings.AppSettings["appkey"];
                isOnlineRegex = ConfigurationSettings.AppSettings["isonlineregex"];
                targetUID = ConfigurationSettings.AppSettings["targetuid"];
                toAddress = ConfigurationSettings.AppSettings["toaddress"];
                ccAddress = ConfigurationSettings.AppSettings["ccaddress"];
                int.TryParse(ConfigurationSettings.AppSettings["dailyquota"], out dailyQuota);
                hasSentLogPath = ConfigurationSettings.AppSettings["hassentlogpath"];

                if (string.IsNullOrEmpty(url)) { url = cStatusURL; }
                if (string.IsNullOrEmpty(isOnlineRegex)) { isOnlineRegex = cIsOnlineRegex; }

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) ||
                    string.IsNullOrEmpty(appKey) || string.IsNullOrEmpty(targetUID) ||
                    string.IsNullOrEmpty(logPath) || string.IsNullOrEmpty(hasSentLogPath))
                {
                    return;
                }

                string logFileName = string.Format("{0}{1}{2}{3}.log", logPath, DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString());

                bool currentStatus = IsUserOnline(targetUID);
                if (currentStatus)
                {
                    Utility.IO.File.WriteContent(logFileName, string.Format("{0} - Tracked activity on Sina Weibo", DateTime.Now.ToString()), true);

                    bool hasSent = false;
                    string hasSentStr = Utility.IO.File.ReadContent(hasSentLogPath).Trim();
                    string tempStr = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                    hasSent = hasSentStr.Equals(tempStr);

                    List<DateTime> trackedTimeList = GetTrackedTimeList(logFileName);
                    Dictionary<int, int> processedTimeList = GetProcessedTimeList(trackedTimeList);
                    int totalTime = 0;
                    foreach (int key in processedTimeList.Keys)
                    {
                        totalTime += processedTimeList[key];
                    }
                    if (totalTime > dailyQuota)
                    {
                        if (!hasSent)
                        {
                            if (!(string.IsNullOrEmpty(toAddress) && string.IsNullOrEmpty(ccAddress)))
                            { 
                                string content = Utility.IO.File.ReadContent(logFileName);
                                Utility.Web.GMail mail = new Utility.Web.GMail(username, password);
                                if (!string.IsNullOrEmpty(toAddress)) { mail.ToAddress = toAddress; }
                                if (!string.IsNullOrEmpty(ccAddress)) { mail.CCAddress = ccAddress; }
                                mail.Subject = "ALERT!STOP USING MOBILE PHONE!DAILY QUOTA REACHED!";
                                mail.Content = string.Format("Here's the detail log information:{0}{1}", Environment.NewLine, content);
                                mail.SendMail();
                            }
                            Utility.IO.File.WriteContent(hasSentLogPath, string.Format("{0}{1}{2}", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString()), true);
                        }
                    }
                }
            }

            // Logger mode runs every day, process past log files
            if (m == Mode.Logger)
            {
                const string cTableCreationQuery = " CREATE TABLE [Activity] ( " +
                                                    " [ActivityID] INT IDENTITY(1,1) PRIMARY KEY, " +
                                                    " [UserID] INT, " +
                                                    " [Raw Text] VARCHAR(13) NOT NULL, " +
                                                    " [Activity Date] DATETIME NOT NULL, " +
                                                    " [Year] INT, " +
                                                    " [Month] INT, " +
                                                    " [Day] INT, " +
                                                    " [Time Slot] INT, " +
                                                    " [Total Mins] INT NOT NULL, " +
                                                    " [Create Date] DATETIME NOT NULL, " +
                                                    " [Modified Date] DATETIME NOT NULL " +
                                                    " ) ";
                string connectStr = "";

                archivePath = ConfigurationSettings.AppSettings["archivepath"];
                dataFile = ConfigurationSettings.AppSettings["mdbfile"];
                connectStr = ConfigurationSettings.AppSettings["connectstr"];

                if (string.IsNullOrEmpty(logPath) || string.IsNullOrEmpty(archivePath) ||
                    string.IsNullOrEmpty(dataFile) || string.IsNullOrEmpty(connectStr))
                {
                    return;
                }

                EnsureDatabase(false);

                Utility.Database.AccessDB db = new Utility.Database.AccessDB(connectStr);
                db.EnsureTable("Activity", cTableCreationQuery, false);

                string currentFile = string.Format("{0}{1}{2}.log", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString());

                foreach (string fileName in System.IO.Directory.GetFiles(logPath))
                {
                    // Exclude 7z and hasSent.log files
                    if (!fileName.EndsWith(".log") || fileName.Contains("hasSent.log") || fileName.Contains(currentFile)) { continue; }

                    List<DateTime> trackedTimeList = GetTrackedTimeList(fileName);
                    if (trackedTimeList.Count > 0)
                    {
                        string newPath = string.Format(@"{0}{1}\{2}\", archivePath, trackedTimeList[0].Year.ToString(), trackedTimeList[0].Month.ToString("00"));
                        Dictionary<int, int> processedTimeList = GetProcessedTimeList(trackedTimeList);
                        foreach (int key in processedTimeList.Keys)
                        {
                            string rawText = string.Format("{0}{1}|{2}", trackedTimeList[0].ToString("yyyyMMdd"), key.ToString("00"), processedTimeList[key].ToString());
                            string activityText = string.Format("{0}{1}", trackedTimeList[0].ToString("yyyyMMdd"), key.ToString("00"));
                            int totalNumber = Convert.ToInt32(processedTimeList[key].ToString());
                            int year = Convert.ToInt32(activityText.Substring(0, 4));
                            int month = Convert.ToInt32(activityText.Substring(4, 2));
                            int day = Convert.ToInt32(activityText.Substring(6, 2));
                            int timeslot = Convert.ToInt32(activityText.Substring(8, 2));
                            DateTime activityDate = new DateTime(year, month, day);

                            string insertRecordQuery = string.Format("INSERT INTO [Activity] ([Raw Text], [Activity Date], [Year], [Month], [Day], [Time Slot], [Total Mins], [Create Date], [Modified Date]) VALUES ('{0}', #{1}#, {2}, {3}, {4}, {5}, {6}, #{7}#, #{8}#)" , rawText, activityDate.ToString("MM/dd/yyyy"), year.ToString(), month.ToString() , day.ToString() , timeslot.ToString() , totalNumber.ToString() , DateTime.Now.ToString("MM/dd/yyyy") , DateTime.Now.ToString("MM/dd/yyyy"));
                            string updateRecordQuery = string.Format("UPDATE [Activity] SET [Total Mins] = {0}, [Modified Date] = #{1}# WHERE [Activity Date] = #{2}# AND [Time Slot] = {3}", totalNumber.ToString(), DateTime.Now.ToString("MM/dd/yyyy"), activityDate.ToString("MM/dd/yyyy"), timeslot.ToString());

                            string checkExistingRecordQuery = string.Format(" SELECT [ActivityID] FROM [Activity] WHERE [Activity Date] = #{0}# AND [Time Slot] = {1}",activityDate.ToString("MM/dd/yyyy"), timeslot.ToString());
                            if (db.GetData(checkExistingRecordQuery).Rows.Count > 0)
                            {
                                db.ExecuteQuery(updateRecordQuery);
                            }
                            else
                            {
                                db.ExecuteQuery(insertRecordQuery);
                            }

                            // Archive the file
                            if (!System.IO.Directory.Exists(newPath)) { System.IO.Directory.CreateDirectory(newPath); }
                            System.IO.File.Move(fileName, newPath + System.IO.Path.GetFileName(fileName));
                        }
                    }
                }
            }
        }

        #region Tracker Mode Helpers

        const string cStatusURL = @"https://api.weibo.com/2/users/show.json";
        const string cIsOnlineRegex = @"""online_status"":(\d)";

        static string username = string.Empty;
        static string password = string.Empty;
        static string url = string.Empty;
        static string appKey = string.Empty;
        static string isOnlineRegex = string.Empty;
        static string targetUID = string.Empty;
        static string toAddress = string.Empty;
        static string ccAddress = string.Empty;
        static int dailyQuota = 15;
        static string hasSentLogPath = string.Empty;

        private static bool IsUserOnline(string UID)
        {
            Utility.Web.HttpContent webContentManager = new Utility.Web.HttpContent(username, password);
            string responseContent = webContentManager.GetContent(string.Format("{0}?source={1}&uid={2}", url, appKey, UID));

            Regex reg = new Regex(isOnlineRegex);
            Match match = reg.Match(responseContent);
            if (match.Groups.Count == 2)
            {
                return (Convert.ToInt32(match.Groups[1].Value) == 1);
            }
            return false;
        }

        #endregion

        #region Logger Mode Helpers

        static string archivePath = string.Empty;
        static string dataFile = string.Empty;
        static string connectStr = string.Empty;

        private static void EnsureDatabase(bool overwrite)
        {
            if (overwrite)
            {
                Utility.IO.File.DeleteFile(dataFile);
            }
            if (System.IO.File.Exists(dataFile)) { return; }
            string directoryPath = System.IO.Path.GetDirectoryName(dataFile);
            if (!System.IO.Directory.Exists(directoryPath)) { System.IO.Directory.CreateDirectory(directoryPath); }
                    
            ADOX.Catalog cat = new ADOX.Catalog();
            try
            {
                cat.Create("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dataFile + "; Jet OLEDB:Engine Type=5"); 
            }
            catch (Exception ex)
            { 
                throw ex; 
            }
            finally
            {
                ADODB.Connection con = cat.ActiveConnection;
                if (con != null) 
                { 
                    con.Close(); 
                }
            }
        }

        #endregion

        static string logPath = string.Empty;

        private static List<DateTime> GetTrackedTimeList(string filePath)
        {
            List<DateTime> trackedTimeList = new List<DateTime>();
            string fileContent = Utility.IO.File.ReadContent(filePath);
            foreach (string item in fileContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                DateTime tempDate;
                if (DateTime.TryParse(item.Split('-')[0], out tempDate))
                {
                    trackedTimeList.Add(tempDate);
                }
            }
            return trackedTimeList;
        }

        private static Dictionary<int, int> GetProcessedTimeList(List<DateTime> trackedTimeList)
        {
            /// <Calculation Formula>
            /// 1. Calculate minutes used per hour
            /// 2. Check records in pairs (Console running time span is 5 minutes)
            ///    if TimeSpan > 4"50' but < 5"10 minutes, treat this timespan as a 'SOLID' one, record 5 mins
            ///    otherwise, treat this as 'WEAK', only record 1 min. (This situation can be user just opened app, and close it immediately.)
            /// <Calculation Formula>
            const int cSolidEvidence = 5;
            const int cWeakEvidence = 1;

            Dictionary<int, int> processedList = new Dictionary<int, int>();
            DateTime lastItem = DateTime.MinValue;
            TimeSpan span = TimeSpan.Zero;
            for (int i = 0; i < trackedTimeList.Count - 2; i++)
            {
                if (!processedList.ContainsKey(trackedTimeList[i].Hour)) { processedList.Add(trackedTimeList[i].Hour, 0); }

                span = trackedTimeList[i + 1] - trackedTimeList[i];
                if (span > new TimeSpan(0, 4, 50) && span < new TimeSpan(0, 5, 10))
                {
                    lastItem = trackedTimeList[i + 1];
                    processedList[trackedTimeList[i].Hour] += cSolidEvidence;
                }
                else if (trackedTimeList[i] != lastItem)
                {
                    processedList[trackedTimeList[i].Hour] += cWeakEvidence;
                }
            }
            if (trackedTimeList[trackedTimeList.Count - 1] != lastItem)
            {
                if (!processedList.ContainsKey(trackedTimeList[trackedTimeList.Count - 1].Hour)) { processedList.Add(trackedTimeList[trackedTimeList.Count - 1].Hour, 0); }
                processedList[trackedTimeList[trackedTimeList.Count - 1].Hour] += cWeakEvidence;
            }
            return processedList;
        }
    }
}
