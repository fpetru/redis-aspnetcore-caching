using Microsoft.AspNetCore.Mvc;
using System.Text;
using WebCache.Models;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System;
using System.Text.RegularExpressions;

namespace WebCache.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var watch = Stopwatch.StartNew();
            string jSONText = RetrieveOrUpdateRedis();
            watch.Stop();

            TempData["DataLoadTime"] = watch.ElapsedMilliseconds;
            var itemsFromjSON = JsonConvert.DeserializeObject<IEnumerable<TwitterItem>>(jSONText);
            return View(itemsFromjSON);
        }

        private string RetrieveOrUpdateRedis()
        {
            var valueFromRedis = default(byte[]);
            string valueToReturn = string.Empty;
            if (HttpContext.Session.TryGetValue("TwitterDataset", out valueFromRedis))
            {
                // Retrieve from Redis
                valueToReturn = Encoding.UTF8.GetString(valueFromRedis);
                TempData["DataLoadType"] = "From Redis";
            }
            else
            {
                // read the file and update the URLs
                var jSONText = System.IO.File.ReadAllText("twitter.json");
                valueToReturn = GetUpdatedFileContent(jSONText);
                Thread.Sleep(20000);

                // store values in Redis
                var valueToStoreInRedis = Encoding.UTF8.GetBytes(valueToReturn);
                HttpContext.Session.Set("TwitterDataset", valueToStoreInRedis);
                TempData["DataLoadType"] = "From file";
            }

            return valueToReturn;
        }

        private string GetUpdatedFileContent(string jSONText)
        {   
            var itemsFromjSON = JsonConvert.DeserializeObject<IEnumerable<TwitterItem>>(jSONText);
            foreach (var item in itemsFromjSON)
            {
                Regex r = new Regex(@"(https?://[^\s]+)");
                item.Text = r.Replace(item.Text, "<a href=\"$1\">$1</a>");
            }

            return JsonConvert.SerializeObject(itemsFromjSON);
        }

        private void SimpleTest()
        {
            var valueToStoreInRedis = Encoding.UTF8.GetBytes("This is a cached value from Redis");
            HttpContext.Session.Set("TestProperty", valueToStoreInRedis);

            string stringValueFromRedis = string.Empty;
            var valueFromRedis = default(byte[]);
            if (HttpContext.Session.TryGetValue("TestProperty", out valueFromRedis))
                stringValueFromRedis = Encoding.UTF8.GetString(valueFromRedis);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
