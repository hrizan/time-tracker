using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.IO;
using System.Text;

namespace TimeTracker.Backend.Helpers
{
	public struct Size
	{
		public int Width;
		public int Height;

		public Size(int width, int height)
		{
			Width = width;
			Height = height;
		}

		public bool IsEmpty
		{
			get
			{
				bool isEmpty = (Width == 0) || (Height == 0);
				return isEmpty;
			}
		}

	}

	public static class ImageHelper
	{
		#region Settings
		//image dimensions

		public static Size DefaultImageSize { get; set; }
		public static Size ThumbnailImageSize { get; set; }

		public static int MaxFileSizeInBytes { get; set; }

		private static readonly Dictionary<string, string> _supportedFileTypes = new Dictionary<string, string>();
		public static Dictionary<string, string> SupportedFileTypes
		{
			get { return ImageHelper._supportedFileTypes; }
		}

		private static string _imageStore;
		public static string ImageStore
		{
			get { return _imageStore; }
			set { _imageStore = value; }
		}


		public static string GetImageStore()
		{
			string imageStore = ConfigHelper.GetAppSettingsValue("ImageStore");
			//if (imageStore[0] == "~")
			//{
			//    imageStore = Server
			//}

			return imageStore;
		}

		#endregion

		/// <summary>
		/// Generates image name by a specific naming strategy.
		/// Strategy: Guid with N formatting
		/// </summary>
		/// <returns></returns>
		public static string GenerateImageName()
		{
			string imageName = Guid.NewGuid().ToString("N");
			return imageName;
		}

		/// <summary>
		/// Renturns thumb image name by a given main image by a specific naming strategy
		/// Strategy: imageName+ "_thumb"
		/// </summary>
		/// <param name="imageName"></param>
		/// <returns></returns>
		public static string GenerateImageThumbName(string imageName)
		{
			string imageThumbName = string.Format("{0}{1}", imageName, "_thumb");
			return imageThumbName;
		}

		/// <summary>
		/// Gets an WebImage from given image name and altImage name.
		/// Gets the image from a ImageStore configured for the application
		/// </summary>
		/// <param name="imageName">Relative image name without path. Parent path is retrieved form configuration</param>
		/// <param name="altImageName">Relative image name without path. Parent path is retrieved form configuration</param>
		/// <returns></returns>
		public static WebImage GetImage(string imageName, string altImageName)
		{
			string imageStorePath = GetImageStore();
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

		/// <summary>
		/// Tries to save image from uploaded file and a given image name with extension
		/// Parent path is retrieved form configuration.
		/// </summary>
		/// <param name="postedFile"></param>
		/// <param name="imageName">relative image name</param>
		/// <returns>True if save is successfull</returns>
		public static bool TrySaveImage(HttpPostedFileBase postedFile, string imageName)
		{
			if (postedFile == null)
			{
				throw new ArgumentNullException("postedFile must not be null!");
			}

			if (string.IsNullOrEmpty(imageName) || string.IsNullOrWhiteSpace(imageName))
			{
				throw new ArgumentNullException("imageName must not be null!");
			}

			string imageStorePath = GetImageStore();
			if (string.IsNullOrEmpty(imageStorePath))
			{
				throw new NullReferenceException("image store is not found");
			}

			try
			{
				string fileToSave = Path.Combine(imageStorePath, imageName);
				postedFile.SaveAs(fileToSave);

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		/// <summary>
		/// Checks if the given file content type is in current app supported file types
		/// </summary>
		/// <param name="contentType"></param>
		/// <returns></returns>
		public static bool IsFileTypeSupported(string contentType)
		{
			bool isFileTypeSupported = SupportedFileTypes.ContainsKey(contentType);
			return isFileTypeSupported;
		}

		/// <summary>
		/// Gets image extension for a given content type. Returns only supported extensions
		/// </summary>
		/// <param name="contentType"></param>
		/// <returns></returns>
		public static string GetSupportedImageExtension(string contentType)
		{
			string imageExtension = string.Empty;
			bool doesExtExists = SupportedFileTypes.TryGetValue(contentType, out imageExtension);

			return imageExtension;
		}

		/// <summary>
		/// Tries to get supported file extension by a given file content type
		/// </summary>
		/// <param name="contentType"></param>
		/// <param name="extension"></param>
		/// <returns></returns>
		public static bool TryGetSupportedFileExtension(string contentType, out string extension)
		{
			extension = string.Empty;
			//try
			//{
				if (IsFileTypeSupported(contentType))
				{
					extension = GetSupportedImageExtension(contentType);
					return true;
				}
				else
				{
					return false;
				}
			//}
			//catch (UnsupportedException ue)
			//{
			//    extension = string.Empty;
			//    return false;
			//}
		}

		public static string GetSupportedFileExtensionsListAsTextToDisplay()
		{
			var supportedFileExtensions = ImageHelper.SupportedFileTypes.Values.Distinct();

			StringBuilder sb = new StringBuilder();
			foreach (var ext in supportedFileExtensions)
			{
				sb.AppendFormat(".{0}, ", ext);
			}

			sb.Remove(sb.Length - 2, 1);
			string extensionsList = sb.ToString();
			return extensionsList;
		}

		public static WebImage ConvertToWebImage(HttpPostedFileBase file)
		{
			if (file == null)
			{
				throw new ArgumentNullException("file must not be null");
			}

			MemoryStream imageStream = new MemoryStream();
			file.InputStream.CopyTo(imageStream);
			byte[] data = imageStream.ToArray();

			var webImage = new WebImage(data);

			return webImage;
		}

		/// <summary>
		/// Resizes image by given size
		/// </summary>
		/// <param name="image"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		private static WebImage GetImageResized(WebImage image, Size size)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image must not be null");
			}

			if (size.IsEmpty)
			{
				throw new ArgumentNullException("size must not be empty");
			}
			var newImage = image.Resize(size.Width, size.Height);

			return newImage;
		}

		/// <summary>
		/// Gets image with DefaultImageSize form givern image
		/// </summary>
		/// <param name="image"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		public static WebImage GetDefaultImage(WebImage image)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image must not be null");
			}

			Size size = DefaultImageSize;
			var newImage = GetImageResized(image, size);

			return newImage;
		}

		/// <summary>
		/// Gets image with ThumbImageSize dimensions from a given image
		/// </summary>
		/// <param name="image"></param>
		/// <returns></returns>
		public static WebImage GetThumbnailImage(WebImage image)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image must not be null");
			}

			Size size = ThumbnailImageSize;
			var newImage = GetImageResized(image, size);

			return newImage;
		}

		/// <summary>
		/// Saves image by given relative image name.
		/// Parent path is retrieved form configuration.
		/// </summary>
		/// <param name="image">Image to be saved</param>
		/// <param name="imageName">Relative image name</param>
		public static void SaveImage(WebImage image, string imageName)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image must not be null!");
			}

			if (string.IsNullOrEmpty(imageName) || string.IsNullOrWhiteSpace(imageName))
			{
				throw new ArgumentNullException("imageName must not be null!");
			}

			string imageStorePath = GetImageStore();
			if (string.IsNullOrEmpty(imageStorePath))
			{
				throw new NullReferenceException("image store is not found");
			}

			string fileToSavePath = Path.Combine(imageStorePath, imageName);
			image.Save(fileToSavePath);

		}


		/// <summary>
		/// Tries to saves image by given relative image name.
		/// Parent path is retrieved form configuration.
		/// </summary>
		/// <param name="image">Image to be saved</param>
		/// <param name="imageName">Relative image name</param>
		/// <returns>True if image is saved</returns>
		public static bool TrySaveImage(WebImage image, string imageName)
		{
			//try
			//{
				SaveImage(image, imageName);
				return true;
			//}
			//catch (Exception)
			//{
			//    return false;
			//}
		}

		/// <summary>
		/// Deletes image from server
		/// </summary>
		/// <param name="imageName">Reative image name. Parent dir is retrieved from configuration</param>
		public static void DeleteImage(string imageName)
		{
			if (string.IsNullOrEmpty(imageName) || string.IsNullOrWhiteSpace(imageName))
			{
				throw new ArgumentNullException("imageName must not be null!");
			}

			string imageStorePath = GetImageStore();
			if (string.IsNullOrEmpty(imageStorePath))
			{
				throw new NullReferenceException("image store is not found");
			}

			string filePath = Path.Combine(imageStorePath, imageName);
			FileInfo file = new FileInfo(filePath);
			if (file.Exists)
			{
				file.Delete();
			}
			else
			{
				throw new FileNotFoundException("");
			}
		}

		/// <summary>
		/// Deletes image from server
		/// </summary>
		/// <param name="imageName">Reative image name. Parent dir is retrieved from configuration</param>
		public static bool TryDeleteImage(string imageName)
		{
			try
			{
				DeleteImage(imageName);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

	}
}