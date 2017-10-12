using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace RealEstateSystem.Helpers
{
    public static class Helper
    {
        public static bool Scrape(string url)
        {
            //bool scrape = true;
            //return scrape;

            var allOffers = GetAllOffers();

            return true;
        }

        public static List<string> GetAllIds()
        {
            List<string> ids = new List<string>();
            for (int i = 1; i < 50; i++)
            {
                string urlAddress = $"http://www.imoti.bg/bg/adv/type:/oblast:sofiq/city:grad-sofiq/offer_id:ID/budget:Цена%20до/currency:EUR/action:rent/page:{i}";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = null;

                    if (response.CharacterSet == null)
                    {
                        readStream = new StreamReader(receiveStream);
                    }
                    else
                    {
                        readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                    }


                    HtmlDocument htmlDoc = new HtmlDocument();

                    // There are various options, set as needed
                    htmlDoc.OptionFixNestedTags = true;

                    // filePath is a path to a file containing the html
                    htmlDoc.Load(readStream);

                    // Use:  htmlDoc.LoadHtml(xmlString);  to load from a string (was htmlDoc.LoadXML(xmlString)

                    // ParseErrors is an ArrayList containing any errors from the Load statement
                    if (htmlDoc.DocumentNode != null)
                    {
                        HtmlNodeCollection nodes = htmlDoc.DocumentNode.SelectNodes("/html/body/div[3]/div/div[3]/div[2]/div[2]/div[3]/div/div/h4/span");
                        if (nodes == null)
                        {
                            break;
                        }
                        foreach (var node in nodes)
                        {
                            if (node != null)
                            {
                                var id = node.InnerText;
                                var result = id.Replace("ID: ", "");
                                ids.Add(result);
                            }
                        }
                    }
                    //   string data = readStream.ReadToEnd();

                    response.Close();
                    readStream.Close();
                }
            }

            return ids;
        }

        public static List<Offer> GetAllOffers()
        {
            //var ids = GetAllIds();
            var ids = new List<string>() { "207624" };
            List<Offer> offers = new List<Offer>();
            foreach (var id in ids)
            {
                string urlAddress = $"http://www.imoti.bg/bg/adv/view:{id}";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = null;

                    if (response.CharacterSet == null)
                    {
                        readStream = new StreamReader(receiveStream);
                    }
                    else
                    {
                        readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                    }
                    HtmlDocument htmlDoc = new HtmlDocument();
                    htmlDoc.OptionFixNestedTags = true;
                    htmlDoc.Load(readStream);
                    var offer = new Offer();
                    if (htmlDoc.DocumentNode != null)
                    {
                        HtmlNode title = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[3]/div/div[3]/div[2]/div[2]/div[2]/div/div[2]/h2[1]");
                        if (title != null)
                        {
                            offer.Title = title.InnerText;
                        }

                        HtmlNode price = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[3]/div/div[3]/div[2]/div[2]/div[2]/div/div[2]/h2[2]");
                        if (price != null)
                        {
                            offer.Price = price.InnerText;
                        }

                        HtmlNode table = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[3]/div/div[3]/div[2]/div[2]/div[2]/div/div[2]/table");
                        var counter = 0;

                        foreach (HtmlNode row in table.SelectNodes("tr"))
                        {
                            foreach (HtmlNode cell in row.SelectNodes("th|td"))
                            {
                                if (counter == 1)
                                {
                                    offer.Area = cell.InnerText;
                                }
                                else if (counter == 3)
                                {
                                    offer.Specs = cell.InnerText;
                                }
                                else if (counter == 5)
                                {
                                    offer.Floor = cell.InnerText;
                                }
                                else if (counter == 7)
                                {
                                    offer.Added = cell.InnerText;
                                }
                                else if (counter == 9)
                                {
                                    offer.Visit = cell.InnerText;
                                }
                                counter++;
                            }
                        }

                        offers.Add(offer);
                    }

                    response.Close();
                    readStream.Close();
                }
            }

            return offers;
        }

        public class Offer
        {
            public string Title { get; set; }

            public string Price { get; set; }

            public string Area { get; set; }

            public string Specs { get; set; }

            public string Floor { get; set; }

            public string Added { get; set; }

            public string Visit { get; set; }

            public string Info { get; set; }

            public string Person { get; set; }

            public string Location { get; set; }

            public string Phone { get; set; }
        }
    }
}