using Nwazet.Commerce.Models;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.MediaLibrary.Services;
using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.IO;
using Orchard.ContentManagement;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.AttributeExtensions")]
    public class ImageProductAttributeExtensionProvider : IProductAttributeExtensionProvider {
        private const string folder = "ImageAttributeExtension";
        private readonly string[] allowedFileExtensions = new string[] {".jpg", ".jpeg", ".png"};

        private readonly dynamic _shapeFactory;
        private readonly IMediaLibraryService _mediaService;
        private readonly IContentManager _contentManager;

        public ImageProductAttributeExtensionProvider(
            IShapeFactory shapeFactory,
            IMediaLibraryService mediaService,
            IContentManager contentManager) {

            _shapeFactory = shapeFactory;
            _mediaService = mediaService;
            _contentManager = contentManager;
        }

        public string Name {
            get { return "ImageProductAttributeExtension"; }
        }

        public string DisplayName {
            get { return "Image Field"; }
        }

        public string Serialize(string value, Dictionary<string, string> form, HttpFileCollectionBase files) {
            // Transform the values posted by the user into a string value that 
            // we'll store as part of the shopping cart and order

            // Save image from form post to media library
            string name = "no image";
            foreach (string fileName in files) {
                HttpPostedFileBase file = files[fileName];
                if (!_mediaService.GetMediaFolders(null).Any(f => f.Name == folder)) {
                    _mediaService.CreateFolder(null, folder);
                }
                if (file.ContentLength > 0 && allowedFileExtensions.Contains(Path.GetExtension(file.FileName))) {
                    var mediaPart = _mediaService.ImportMedia(file.InputStream, folder, file.FileName);
                    _contentManager.Create(mediaPart);
                    name = mediaPart.FileName;
                }
            }

            return name;
        }

        public dynamic BuildInputShape(ProductAttributePart part) {
            // Builds the shape used to prompt users for the extended attribute info.
            // Data entered into this shape will be passed to the Serialize method.

            return _shapeFactory.ImageProductAttributeExtensionInput(
                ExtensionName: Name,
                Part: part);
        }

        public string DisplayString(string value) {
            // Builds the shape used to display the extended attribute info on front-end
            
            return string.Format("[File: {0}]", value);
        }

        public dynamic BuildAdminShape(string value) {
            // Builds the shape used to display the extended attribute info on back-end
            var src = string.Empty;
            if (value != "no image") {
                src = _mediaService.GetMediaPublicUrl(folder, value);
            }
            return _shapeFactory.ImageProductAttributeExtensionAdmin(
                    ImageUrl: src,
                    FileName: value);
        }        
    }
}