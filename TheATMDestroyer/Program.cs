using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text; 
using System.Threading.Tasks;
using System.IO;

namespace TheATMDestroyer
{
    class Program
    {
        static void Main(string[] args)
        {

            var t1 = Task.Run(CreateTask("steve1", "steve", 1000));
            var t2 = Task.Run(CreateTask("steve", "steve1", 2000));

            var t3= Task.Run(CreateTask("steve3", "steve4", 1000));
            var t4 = Task.Run(CreateTask("steve3", "steve1", 2000));

            Task.WaitAll(t1, t2, t3, t4);
        }


        public static Action CreateTask(string from, string to, int amount)
        {
            return new Action(() =>
            {
                Console.WriteLine("start:" + DateTime.Now + "-" + from + "-" + to + "-" + amount);
                var request = (HttpWebRequest)WebRequest.Create(String.Format("http://localhost:9112/api/Account?accountFrom={0}&accountTo={1}&amount={2}", from, to, amount));
                request.Method = "PUT";
                request.ContentLength = 0;
                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    string returnString = response.StatusCode.ToString();
                    Console.WriteLine("end:" + DateTime.Now + "-" + from + "-" + to + "-" + amount);
                    Console.WriteLine(returnString);
                }
                catch (WebException ex)
                {
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        Console.WriteLine("end:" + DateTime.Now + "-" + from + "-" + to + "-" + amount);
                        Console.WriteLine(reader.ReadToEnd());
                    }
                }
            });
        }
    }
}
