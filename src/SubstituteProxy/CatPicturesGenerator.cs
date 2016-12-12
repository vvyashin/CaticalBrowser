using System;
using System.Net;

namespace SubstituteProxy
{
    public class CatPicturesGenerator
    {
        private readonly Random _random;

        public CatPicturesGenerator()
        {
            _random = new Random();
        }

        private const int DefaultWidth = 500;
        private const int DefaultHeight = 300;
        private const int MinNumber = 0;
        private const int MaxNumber = 10;
        
        public virtual string GetNextCatPicture(string url)
        {
            int width, height;
            try {
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = request.GetResponse()) {
                    var image = System.Drawing.Image.FromStream(response.GetResponseStream());

                    width = image.Width;
                    height = image.Height;
                }
            }
            catch (Exception) {
                width = DefaultWidth;
                height = DefaultHeight;
            }
            
            return new Uri($"http://lorempixel.com/{width}/{height}/cats/{_random.Next(MinNumber, MaxNumber)}").ToString();
        }
    }
}
