using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SC.DevChallenge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PricesController : ControllerBase
    {
        [HttpGet("average")]
        public string Average(string portfolio = "", string owner = "", string instrument = "", string datetime = "")
        {
            try
            {
                List<Values> values = System.IO.File.ReadAllLines(@"C:\Users\KDFX Team\Desktop\Battle\DevChallenge\SC.DevChallenge.Api\Input\data.csv")
                                           .Skip(1)
                                           .Select(v => Values.FromCsv(v))
                                           .ToList();

                List<Values> resValues = new List<Values>();
                double avaragePrice;

                int counter = 0;
                double sum = 0;

                foreach (Values value in values)
                {
                    if ((EqualsStrSimple(portfolio, value.Portfolio) || EqualsStrEncoded(portfolio, value.Portfolio))
                        && (EqualsStrSimple(owner, value.Owner) || EqualsStrEncoded(owner, value.Owner))
                        && (EqualsStrSimple(instrument, value.Instrument) || EqualsStrEncoded(instrument, value.Instrument)))
                    {
                        resValues.Add(value);
                        counter++;
                        sum += value.Price;
                    }
                }

                avaragePrice = sum / counter;
                
                if(Double.IsNaN(avaragePrice))
                {
                    throw new DivideByZeroException();
                }

                return $"Avarage Price: {avaragePrice}\n\n" + JsonConvert.SerializeObject(resValues, Formatting.Indented);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        private static bool EqualsStrSimple(string objFirst, string objSecond)
        {
            objFirst = objFirst.Replace(" ", "");
            objSecond = objSecond.Replace(" ", "");

            if (objFirst.ToLower() == objSecond.ToLower())
            {
                return true;
            }

            return false;
        }

        private static bool EqualsStrEncoded(string objFirst, string objSecond)
        {
            objFirst = objFirst.Replace(" ", "+");
            objSecond = objSecond.Replace(" ", "+");

            if (objFirst.ToLower() == objSecond.ToLower())
            {
                return true;
            }

            return false;
        }
    }

    class Values
    {
        public string Portfolio { get; set; }
        public string Owner { get; set; }
        public string Instrument { get; set; }
        public DateTime Date { get; set; }
        public double Price { get; set; }

        public static Values FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(',');
            Values dailyValues = new Values();

            dailyValues.Portfolio = values[0];
            dailyValues.Owner = values[1];
            dailyValues.Instrument = values[2];
            dailyValues.Date = Convert.ToDateTime(values[3]);
            dailyValues.Price = Convert.ToDouble(values[4].Replace('.', ','));

            return dailyValues;
        }
    }
}
