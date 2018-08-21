using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using RazorPagesMovie.Models;

namespace RazorPagesMovie.Pages.Movies
{
    public class CreateModel : PageModel
    {
        private readonly RazorPagesMovie.Models.RazorPagesMovieContext _context;

        public CreateModel(RazorPagesMovie.Models.RazorPagesMovieContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Movie Movie { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var str = getMovieInfo(Movie.Title);
            JObject json = JObject.Parse(str);

            foreach (JProperty prop in (JToken)(json))
            {
                var key = prop.Name;
                var jvalue = prop.Value;

                if (key == "Ratings")
                {
                    //Create a list with all ratings
                    List<double> avgRating = new List<double>();

                    foreach (JObject parsedObject in jvalue.Children<JObject>())
                    {
                        var rating = parsedObject["Value"].ToString();
                        //convert all the ratings to 1 to 10 scale

                        //convert percentage based rating to 1 to 10 scale (example : 88% ==> 8.8)
                        if (rating.Contains("%"))
                        {
                            avgRating.Add(fromPercentageRatingToDouble(rating));
                        }
                        else if (rating.Contains("/"))
                        {
                            var tempRating = rating.Remove(rating.IndexOf("/"));

                            //7.5/10 ==> 7.5
                            if (tempRating.Contains("."))
                            {
                                avgRating.Add(double.Parse(tempRating));
                            }
                            else
                            {
                                // 75/100 ==> 7.5
                                avgRating.Add(double.Parse(tempRating) / 10);
                            }
                        }
                        else
                        {
                            try
                            {
                                avgRating.Add(double.Parse(rating));
                            }
                            catch (Exception ex)
                            {
                                continue;
                            }
                        }
                    }

                    //assign the average rating
                    Movie.Ratings = "Avarage rating (1 to 10 Scale) : " + String.Format("{0:0.00}", avgRating.Average());
                }
                else if (key == "Poster" && !string.IsNullOrWhiteSpace(jvalue.ToString()))
                {
                    Movie.PosterArt = jvalue.ToString();
                }
            }


            _context.Movie.Add(Movie);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private static double fromPercentageRatingToDouble(string value)
        {
            return double.Parse(value.Substring(0, value.Length - 1)) / 10;
        }

        public string getMovieInfo(string title)
        {

            var url = "https://www.omdbapi.com/?apikey=cc2c053b&t=" + title;

            //var url = "http://api.rest7.com/v1/movie_info.php?title=" + title;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    // log errorText
                }
                throw;
            }
        }
    }
}