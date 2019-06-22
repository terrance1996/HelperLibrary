using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;

namespace HelperLibrary.NetCore
{

    public class Post
    {

        public class locResult
        {
            public bool found { get; set; }
            public List<string> results { get; set; }
        }

        private class State
        {
            public string Name { get; set; }
            public string Code { get; set; }
            public string Capital { get; set; }
            public string CapitalPostcode { get; set; }
        }

        public static bool ValidatePostcode(string state, string postcode)
        {
            switch (state)
            {
                case "NSW":
                    if (int.Parse(postcode) >= 200 && int.Parse(postcode) <= 299)
                    { return true; }
                    else if (int.Parse(postcode) >= 1000 && int.Parse(postcode) <= 2999)
                    { return true; }
                    else
                    { return false; }
                case "ACT":
                    if (int.Parse(postcode) >= 200 && int.Parse(postcode) <= 299)
                    { return true; }
                    else if (int.Parse(postcode) >= 1000 && int.Parse(postcode) <= 2999)
                    { return true; }
                    else
                    { return false; }

                case "NT":
                    if (int.Parse(postcode) >= 800 && int.Parse(postcode) <= 999)
                    { return true; }
                    else
                    { return false; }

                case "QLD":
                    if (int.Parse(postcode) >= 4000 && int.Parse(postcode) <= 4899)
                    { return true; }
                    else if (int.Parse(postcode) >= 9000 && int.Parse(postcode) <= 9299)
                    { return true; }
                    else if (int.Parse(postcode) >= 9400 && int.Parse(postcode) <= 9599)
                    { return true; }
                    else if (int.Parse(postcode) >= 9700 && int.Parse(postcode) <= 9999)
                    { return true; }
                    else
                    { return false; }

                case "SA":
                    if (int.Parse(postcode) >= 5000 && int.Parse(postcode) <= 5749)
                    { return true; }
                    else if (int.Parse(postcode) >= 5800 && int.Parse(postcode) <= 5999)
                    { return true; }
                    else
                    { return false; }

                case "TAS":
                    if (int.Parse(postcode) >= 7000 && int.Parse(postcode) <= 7999)
                    { return true; }
                    else
                    { return false; }

                case "VIC":
                    if (int.Parse(postcode) >= 3000 && int.Parse(postcode) <= 3999)
                    { return true; }
                    else if (int.Parse(postcode) >= 8000 && int.Parse(postcode) <= 8999)
                    { return true; }
                    else
                    { return false; }

                case "WA":
                    if (int.Parse(postcode) >= 6000 && int.Parse(postcode) <= 6999)
                    { return true; }
                    else
                    { return false; }

                default:
                    return false;
            }



        }

        static List<State> statesList = new List<State>
                                                         {
                                                            new State { Name="WESTERN AUSTRALIA",Code="WA",Capital="PERTH",CapitalPostcode="6000"},
                                                            new State { Name="VICTORIA",Code="VIC",Capital="MELBOURNE",CapitalPostcode="3000"},
                                                            new State { Name="NEW SOUTH WALES",Code="NSW",Capital="SYDNEY",CapitalPostcode="2000"},
                                                            new State { Name="SOUTH AUSTRALIA",Code="SA",Capital="ADELAIDE",CapitalPostcode="5000"},
                                                            new State { Name="QEENSLAND",Code="QLD",Capital="BRISBANE",CapitalPostcode="4000"},
                                                            new State { Name="TASMANIA",Code="TAS",Capital="HOBART",CapitalPostcode="7000"},
                                                            new State { Name="NORTHERN TERITORY",Code="NT",Capital="DARWIN",CapitalPostcode="800"},
                                                            new State { Name="AUSTRALIAN CAPITAL TERITORY",Code="ACT",Capital="CANBERRA",CapitalPostcode="2600"}
                                                                                                                                                                     };


        public static bool fixState(string stateVal, out string fixedState)
        {
            foreach (State s in statesList)
            {
                if (stateVal.Replace(".", "").ToUpper() == s.Name || stateVal.Replace(".", "").ToUpper() == s.Code)
                {
                    fixedState = s.Code;
                    return true;
                }
                else if (FuzzyMatching.LevenshteinDistance(stateVal.Replace(".", "").ToUpper(), s.Name) < 2)
                {
                    fixedState = s.Code;
                    return true;
                }

            }
            fixedState = stateVal;
            return false;
        }

        public static double getPostage(string fromPC, string toPC, string length, string width, string height, string weight, string serviceCode = "AUS_PARCEL_REGULAR")
        {
            try
            {
                string APIKey = "YjQ3NGQzMzctNzUzMy00ZDE5LWE3YjYtYmNhYjk2Mjc5MWFl";
                string jasonString = "";
                string url2 = $"https://digitalapi.auspost.com.au/postage/parcel/domestic/calculate.json?from_postcode={fromPC}&to_postcode={toPC}&length={length}&width={width}&height={height}&weight={weight}&service_code=AUS_PARCEL_REGULAR";
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url2);
                request.Headers.Add("auth-key", APIKey);

                using (var webResponse = (HttpWebResponse)request.GetResponse())
                {

                    using (var reader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        jasonString = reader.ReadToEnd();
                    }

                }


                List<string> propertyList = jasonString.Split(',').ToList();
                string cost = "";
                foreach (string s in propertyList)
                {
                    if (s.Contains("total_cost"))
                    {
                        cost = s.Split(':')[1].Replace("\"", "");
                    }
                }

                return Math.Round(double.Parse(cost) / 1.1, 2);

            }
            catch (Exception e)
            {
                Console.WriteLine($"\n\nError: {e.Message}");
                Console.Read();

            }

            return 0;
        }

        public static locResult checkLocStPc(string locality, string state, string postcode)
        {
            string url = $"https://digitalapi.auspost.com.au/shipping/v1/address?suburb={locality}&state={state}&postcode={postcode}";
            string content = "";
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

            string authInfo = "YThjNmM0YjgtMzg1Ni00MzdjLThhOGItNDc3NDkyNDA5MThlOng2N2I5OGYxYjdiNmZmYzk2YjU1";
            request.Headers["Authorization"] = "Basic " + authInfo;
            try
            {
                using (var webResponse = (HttpWebResponse)request.GetResponse())
                {

                    using (var reader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        content = reader.ReadToEnd();
                    }


                }
            }
            catch (Exception ex)
            {
                return new locResult { found = false, results = new List<string> { ex.Message } };
            }



            //  string[] arr = content.Split(',');
            //return arr[0].Split(':')[1].ToString();

            //  string jString = content.Substring(content.IndexOf(":") + 1);
            //  jString = jString.Substring(0, jString.Length - 1);
            locResult l = fastJSON.JSON.ToObject<locResult>(content);
            return l;
        }

        public static string checkPostCode(string query, string state = "")
        {
            string url = $"https://digitalapi.auspost.com.au/postcode/search.json?q={query}&state={state}";
            //string url = $"https://digitalapi.auspost.com.au/shipping/v1/address?suburb=cannington&state=vic&postcode=6107";
            string content = "";
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

            //string authInfo = "YThjNmM0YjgtMzg1Ni00MzdjLThhOGItNDc3NDkyNDA5MThlOng2N2I5OGYxYjdiNmZmYzk2YjU1";
            //request.Headers["Authorization"] = "Basic " + authInfo;
            string APIKey = "2adb3865-098c-4dac-98eb-4e78e23b2bde";
            request.Headers.Add("auth-key", APIKey);

            using (var webResponse = (HttpWebResponse)request.GetResponse())
            {
                using (var reader = new StreamReader(webResponse.GetResponseStream()))
                {
                    content = reader.ReadToEnd();
                }
            }

            string jString = content.Substring(content.IndexOf(":") + 1);
            jString = jString.Substring(0, jString.Length - 1);

            return content;
        }
    }
}


