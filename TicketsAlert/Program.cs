using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TicketsAlert
{
    enum MovieDay
    {
        Friday,
        Saturday
    }

    class Program
    {
        static string cinemaName = "Prasads";
        static string ticketsFoundMessage = "Tickets in {0} are available on {1}";

        static string movieName = "avengers-infinity-war-3d";
        static string movieCode = "ET00053419";
        static string cityFullName = "hyderabad";
        static string cityCode = "hyd";

        //static string movieName = "rampage";
        //static string movieCode = "ET00073752";
        //static string cityFullName = "hyderabad";
        //static string cityCode = "hyd";

        //static string movieName = "ready-player-one-3d";
        //static string movieCode = "ET00060190";
        //static string cityFullName = "hyderabad";
        //static string cityCode = "hyd";

        static string baseURL = string.Format("https://in.bookmyshow.com/buytickets/{0}-{1}/movie-{2}-{3}-MT/{4}", movieName, cityFullName, cityCode, movieCode, "{0}");
        static Dictionary<MovieDay, string> days = new Dictionary<MovieDay, string>() { { MovieDay.Friday, "20180427" } };
        static NotifyIcon trayIcon;

        static void Main(string[] args)
        {
            trayIcon = initializeAndGetTrayIcon();
            long round = 0;

            while (true)
            {
                foreach (var day in days)
                {
                    var pageContent = sendRequest(day.Key);
                    if (doesContain(pageContent, cinemaName))
                    {
                        notify(day.Key);
                        Console.WriteLine(string.Format(ticketsFoundMessage, cinemaName, day.Key.ToString()));
                        Console.ReadKey();
                        trayIcon.Dispose();
                        return;
                    }
                }

                round++;
                Thread.Sleep(8000);
            }
        }

        static NotifyIcon initializeAndGetTrayIcon()
        {
            NotifyIcon notifyIcon = new NotifyIcon();
            notifyIcon.Text = "Checking for tickets is in progress...";
            notifyIcon.Icon = new Icon(SystemIcons.Information, 100, 100);

            ContextMenu trayMenu = new ContextMenu();

            notifyIcon.ContextMenu = trayMenu;
            notifyIcon.Visible = true;
            notifyIcon.BalloonTipTitle = "TICKETS AVAILABLE!";

            return notifyIcon;
        }

        static void notify(MovieDay day)
        {
            trayIcon.BalloonTipText = string.Format(ticketsFoundMessage, cinemaName, day.ToString());
            trayIcon.ShowBalloonTip(100000000);
        }

        static string sendRequest(MovieDay day)
        {
            string html = string.Empty;
            var url = string.Format(baseURL, days[day]);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            //Console.WriteLine(html);

            return html;
        }

        static bool doesContain(string source, string searchKey)
        {
            return source.IndexOf(searchKey, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
