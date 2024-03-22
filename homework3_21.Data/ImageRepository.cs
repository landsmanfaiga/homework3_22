using System;

namespace homework3_21.Data
{
    public class ImageRepository
    {
        private readonly string _connectionString;
        public ImageRepository(string connectionString)
        {
             _connectionString = connectionString;
        }

        public List<Image> GetAll()
        {
            using var context = new ImageDataContext(_connectionString);
            return context.Images.ToList();
        }

        public void Add(string title, string imagePath)
        {
            using var context = new ImageDataContext(_connectionString);
            Image image = new()
            {
                Title = title,
                ImagePath = imagePath,
                Date = DateTime.Now,
                Likes = 0,
            };

            context.Images.Add(image);
            context.SaveChanges();
        }

        public Image GetById(int id)
        {
            using var context = new ImageDataContext(_connectionString);
            return context.Images.FirstOrDefault(i => i.Id == id);
        }

        public void IncrementLikes(int id)
        {
            using var context = new ImageDataContext(_connectionString);
            Image image =  context.Images.FirstOrDefault(i => i.Id == id);
            image.Likes++;
            context.SaveChanges();
        }

        public int GetLikes(int id)
        {
            using var context = new ImageDataContext(_connectionString);
            Image image = context.Images.FirstOrDefault(i => i.Id == id);
            return image.Likes;
        }
    }
}