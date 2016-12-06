using System;

namespace SubstituteProxy
{
    public class CatPicturesGenerator
    {
        private readonly Random _random;

        private const int PicturesNumber = 5;

        public CatPicturesGenerator()
        {
            _random = new Random();
        }

        public virtual string GetNextCatPicture(string picturesFolder)
        {
            if (picturesFolder == null) throw new ArgumentNullException(nameof(picturesFolder));

            return $"{picturesFolder}/{_random.Next(PicturesNumber)}.jpg";
        }
    }
}
