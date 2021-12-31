//وسام سعد عبدالله المجمعي
// محمد خلف محل البوصكر
// كروه : 5

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
namespace LoremBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Run().Wait();
        }
        static async Task Run()
        {
            var bot = new Api("5098932169:AAEe0fOe0OXyBNOm3n6BWVCiJe1yT35iTT8");
            var weatherApiKey = "27fc2444d573596be6704996fafed90c";
            var me = await bot.GetMe();
             var offset = 0;
            while (true)
            {
                var updates = await bot.GetUpdates(offset);
                foreach (var update in updates)
                {
                    if (update.Message.Type == MessageType.TextMessage)
                    {
                        var inputMessage = update.Message.Text;
                        if (inputMessage.StartsWith("/weather"))
                        {
                            var messageParts = inputMessage.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            var city = messageParts.Length == 1 ? "Minsk" : messageParts.Skip(1).First();
                            WebUtility.UrlEncode(city);
                            string url = string.Format("http://api.openweathermap.org/data/2.5/weather?q={0}&APPID={1}&units=metric", city, weatherApiKey);
                            WebRequest request = WebRequest.Create(url);
                            WebResponse response = request.GetResponse();
                            using (var streamReader = new StreamReader(response.GetResponseStream()))
                            {
                                string responseString = streamReader.ReadToEnd();
                                Console.WriteLine(responseString);
                                JObject joResponse = JObject.Parse(responseString);
                                JObject main = (JObject)joResponse["main"];
                                double temp = (double)main["temp"];
                                JObject weather = (JObject)joResponse["weather"][0];
                                string description = (string)weather["description"];
                                string cityName = (string)joResponse["name"];
                                Console.WriteLine(string.Format("temp is: {0}", temp));
                                var message = "In " + cityName + " " + description + " and the temperature is " + temp.ToString("+#;-#") + "°C";
                                var t = await bot.SendTextMessage(update.Message.Chat.Id, message);
                                Console.WriteLine("Echo Message: {0}", message);
                            }
                        }
                        else
                        {
                            await bot.SendChatAction(update.Message.Chat.Id, ChatAction.Typing);
                            await Task.Delay(2000);
                            var t = await bot.SendTextMessage(update.Message.Chat.Id, update.Message.Text);
                            Console.WriteLine("Echo Message: {0}", update.Message.Text);
                        }
                    }

                    offset = update.Id + 1;
                }

                await Task.Delay(1000);
            }
        }
    }
}
