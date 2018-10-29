using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FinderApp.API.Dtos;
using FinderApp.API.Helpers;
using FinderApp.API.Model;
using FinderApp.API.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FinderApp.API.Controllers
{
    [Authorize]
    [Route("api/user/{userId}/photos")]
    public class PhotosController : Controller
    {
        private readonly IFinderRepository repository;
        private readonly IMapper mapper;
        private readonly IOptions<CloudinarySettings> cloudinaryConfig;
        private readonly Cloudinary _cloudinary;

        public PhotosController(IFinderRepository repository, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            this.cloudinaryConfig = cloudinaryConfig;
            this.mapper = mapper;
            this.repository = repository;

            Account acc = new Account(
            cloudinaryConfig.Value.CloudName,
            cloudinaryConfig.Value.ApiKey,
            cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);

        }

        [HttpGet("{id}", Name = "GetPhoto")]

        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await repository.GetPhoto(id);

            var photo = mapper.Map<PhotoReturnDto>(photoFromRepo);
            return Ok(photo);
        }



        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, PhotoCreationDto photoDto)
        {   
           var user = await repository.GetUser(userId);
           if (user == null)
           return BadRequest("Could Not find User");
           var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
           if (currentUserId != user.Id)
           return Unauthorized();

           var file = photoDto.File;

           //create a variable of type imageuploadresult to store all the result from cloudinary when it sends back its response
           var uploadResult = new ImageUploadResult();
           if(file.Length > 0)
           {
               //to read the file 
               using (var stream = file.OpenReadStream())
               {
                   //specify the upload parameters
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        //apply transformation so that when we upload it automatically crops the photo and provides an image that centers around the face
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face") 
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
               }
           }
           //if upload successful map ur model properties to cloudinary response
           photoDto.Url = uploadResult.Uri.ToString();
           photoDto.PublicId = uploadResult.PublicId;

            // do mapping
           var photo = mapper.Map<Photo>(photoDto);
           photo.User = user;

           //check to see if the photo is not the user IsMain photo
           if(!user.Photos.Any(m => m.IsMain))
           //set Ismain to true
           photo.IsMain = true;


            //add the photo
           user.Photos.Add(photo);


            //check if the photos was successfully saved
           if(await repository.CompleteAsync())
           {
               var photoToreturn = mapper.Map<PhotoReturnDto>(photo);
               return CreatedAtRoute("GetPhoto", new {id = photo.Id}, photoToreturn);
           }
           
           return BadRequest("could not add the photo");

        }

        [HttpPost("{id}/setMain")]

        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
             if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var photoFromRepo = await repository.GetPhoto(id);
            if(photoFromRepo == null)
            return NotFound();

            if(photoFromRepo.IsMain)
            return BadRequest("This is already the main photo");

            var currentMainPhoto = await repository.GetIsMainPhotoForUser(userId);
            if(currentMainPhoto != null)
            currentMainPhoto.IsMain = false;

            photoFromRepo.IsMain = true;
            if(await repository.CompleteAsync())
            return NoContent();

            return BadRequest("Could not set photo to main");


        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
             if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var photoFromRepo = await repository.GetPhoto(id);
            if(photoFromRepo == null)
            return NotFound();

            if(photoFromRepo.IsMain)
            return BadRequest("You cannot delete the main photo");

            if(photoFromRepo.PublicId != null)
            {
                
            var deleteParams = new DeletionParams(photoFromRepo.PublicId);

            var result = _cloudinary.Destroy(deleteParams);

            if (result.Result == "ok")
                repository.Delete(photoFromRepo);
            }

            if(photoFromRepo.PublicId == null)
            {
                repository.Delete(photoFromRepo);
            }
            
            if (await repository.CompleteAsync())
             return Ok();


             return BadRequest("Error while deleting photo");

                
        }

    }
}