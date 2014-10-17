using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace TimeTracker.Backend.Helpers
{
    public class ImageStorage
    {
        const string WEB_IMAGE_STORE_PATH = "~/App_Data/FileStorage";
        public static string GetWebImageStorePath(string path)
        {
            string fullPath = path.Trim();
            if (path.StartsWith("~/"))
            {
                fullPath = HttpContext.Current.Server.MapPath(path);
            }

            return fullPath;
        }

        public ImageStorage() :
            this(ImageStorage.GetWebImageStorePath(WEB_IMAGE_STORE_PATH))
        {

        }

        public ImageStorage(string storagePath)
        {
            _storagePath = storagePath;
        }
        string _storagePath;

        /// <summary>
        /// Saves image by given relative image name.
        /// Parent path is retrieved form configuration.
        /// </summary>
        /// <param name="image">Image to be saved</param>
        /// <param name="imageName">Relative image name</param>
        public void SaveImage(WebImage image, string imageName)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image must not be null!");
            }

            if (string.IsNullOrEmpty(imageName) || string.IsNullOrWhiteSpace(imageName))
            {
                throw new ArgumentNullException("imageName must not be null!");
            }

            string imageStorePath = _storagePath;
            if (string.IsNullOrEmpty(imageStorePath))
            {
                throw new NullReferenceException("image store is not found");
            }

            string fileToSavePath = Path.Combine(imageStorePath, imageName);
            image.Save(fileToSavePath);

        }

        /// <summary>
        /// Gets an WebImage from given image name and altImage name.
        /// Gets the image from a ImageStore configured for the application
        /// </summary>
        /// <param name="imageName">Relative image name without path. Parent path is retrieved form configuration</param>
        /// <param name="altImageName">Relative image name without path. Parent path is retrieved form configuration</param>
        /// <returns></returns>
        public WebImage GetImage(string imageName, string altImageName)
        {
            string imageStorePath = _storagePath;
            if (string.IsNullOrEmpty(imageStorePath))
            {
                throw new NullReferenceException("image store is not found");
            }

            string altImagePath = Path.Combine(imageStorePath, altImageName);
            if (string.IsNullOrEmpty(imageName))
            {
                altImagePath = Path.Combine(imageStorePath, altImageName);
                bool isAltImageExists = System.IO.File.Exists(altImagePath);
                if (isAltImageExists)
                {
                    var altImage = new WebImage(altImagePath);
                    return altImage;
                }
                return null;
            }

            string imagePath = Path.Combine(imageStorePath, imageName);

            try
            {
                bool isImageExists = System.IO.File.Exists(imagePath);
                if (isImageExists)
                {
                    var image = new WebImage(imagePath);
                    return image;
                }

                var altImage = new WebImage(altImagePath);
                return altImage;
            }//must not throw exception
            catch (Exception)
            {
                var altImage = new WebImage(altImagePath);
                return altImage;
            }
            return null;
        }

        public WebImage GetImage(string imageName)
        {
            string imageStorePath = _storagePath;
            if (string.IsNullOrEmpty(imageStorePath))
            {
                throw new NullReferenceException("image store is not found");
            }

            if (string.IsNullOrEmpty(imageName))
            {
                return null;
            }

            string imagePath = Path.Combine(imageStorePath, imageName);

            try
            {
                bool isImageExists = System.IO.File.Exists(imagePath);
                if (isImageExists)
                {
                    var image = new WebImage(imagePath);
                    return image;
                }

            }//must not throw exception
            catch (Exception)
            {
                return null;
            }
            return null;
        }

        /// <summary>
        /// Tries to save image from uploaded file and a given image name with extension
        /// Parent path is retrieved form configuration.
        /// </summary>
        /// <param name="postedFile"></param>
        /// <param name="imageName">relative image name</param>
        /// <returns>True if save is successfull</returns>
        public bool TrySaveImage(HttpPostedFileBase postedFile, string imageName)
        {
            try
            {
                SaveImage(postedFile, imageName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void SaveImage(HttpPostedFileBase postedFile, string imageName)
        {

            if (postedFile == null)
            {
                throw new ArgumentNullException("postedFile must not be null!");
            }

            if (string.IsNullOrEmpty(imageName) || string.IsNullOrWhiteSpace(imageName))
            {
                throw new ArgumentNullException("imageName must not be null!");
            }

            string imageStorePath = _storagePath;
            if (string.IsNullOrEmpty(imageStorePath))
            {
                throw new NullReferenceException("image store is not found");
            }

            try
            {
                string fileToSave = Path.Combine(imageStorePath, imageName);
                postedFile.SaveAs(fileToSave);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}