using System.Collections.Generic;
using System.Web;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.IO;
using rest_api.Libary.JsonHelper;
using rest_api.Models;
using System.Web.Helpers;

namespace rest_api.Libary.Cloudinary
{

    public static class Cloudinary
    {
        private static string imageRootUrl = "https://res.cloudinary.com/www-menkule-com-tr/image/upload/";
        private static string noProfile = imageRootUrl + "no-profile_u5qcn9.png";
        private static Account CloudAccount = new Account("www-menkule-com-tr", "574816954465322", "38oob_T9cFYt7qChDAmRj3zqgpQ");

        //get and check image url
        private static object getParam(Dictionary<string, object> dict, string name)
        {
            if (dict.ContainsKey(name))
            {
                return dict[name];
            }
            return null;
        }

        public static Images upload(WebImage webImage, string folder = "")
        {
            try
            {
                CloudinaryDotNet.Cloudinary cloudinary = new CloudinaryDotNet.Cloudinary(CloudAccount);
                byte[] fileData = webImage.GetBytes();

                using (MemoryStream memoryStream = new MemoryStream(fileData))
                {
                    ImageUploadParams uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription("advert images", memoryStream),
                        Transformation = new Transformation().Crop("limit").Width(750).Height(750),
                        Tags = "advert_image",
                        Folder = folder

                    };
                    Dictionary<string, object> dic = JsonHelper.JsonHelper.ConvertJsonToDictionary(cloudinary.Upload(uploadParams).JsonObj.ToString());
                    Images image = new Images()
                    {
                        url = getParam(dic, "public_id") + "." + getParam(dic, "format")
                    };
                    return image;
                };
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }
    }
}