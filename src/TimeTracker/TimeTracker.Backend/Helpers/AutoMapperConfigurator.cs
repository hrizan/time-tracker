using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TimeTracker.Backend.Models;
using TimeTracker.Models;

namespace TimeTracker.Backend.Helpers
{
    public class AutoMapperConfigurator
    {
        public static void CreateMappings()
        {
            Mapper.CreateMap<Activity, ActivityUpdateDto>();
            Mapper.CreateMap<ActivityUpdateDto, Activity>();
            //====================SAMPLE MAPPINGS

            //Items mapping

            //mapper.CreateMap<Item, ItemDetailsViewModel>();
            ////.ForMember(dest => dest.Tracks, opt => opt.MapFrom(src => src.TrackedAsLocationCollection));

            //mapper.CreateMap<Item, Item>();

            //mapper.CreateMap<Item, ItemListDto>()
            //    .ForMember(dest => dest.ObjectDetailId, opt => opt.MapFrom(src => src.ObjectDetailId))
            //    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            //    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            //    .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.GroupId))
            //    .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.Name))
            //    .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.ImagePath))
            //    .ForMember(dest => dest.ImageThumbnailPath, opt => opt.MapFrom(src => src.ImageThumbnailPath));

            //mapper.CreateMap<Track, TrackDto>();

            //mapper.CreateMap<Thipper.Models.Item, ItemCreateViewModel>()
            //    .ForMember(dest => dest.ObjectDetailId, opt => opt.MapFrom(src => src.ObjectDetailId))
            //    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            //    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            //    .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.GroupId))
            //    .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.Name));

            //mapper.CreateMap<ItemCreateViewModel, Thipper.Models.Item>()
            //    .ForMember(dest => dest.ObjectDetailId, opt => opt.MapFrom(src => src.ObjectDetailId))
            //    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            //    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            //    .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.GroupId));

            ////api
            //mapper.CreateMap<Thipper.Models.Item, Thipper.Areas.Api.Models.ItemDetailsDto>()
            //    .ForMember(dest => dest.ObjectDetailId, opt => opt.MapFrom(src => src.ObjectDetailId))
            //    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            //    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            //    .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.GroupId))
            //    .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.Name));

            ////Contacts
            //mapper.CreateMap<ContactDetailsViewModel, Thipper.Models.Contact>();
            //mapper.CreateMap<Thipper.Models.Contact, ContactDetailsViewModel>();

            //mapper.CreateMap<Contact, ContactListDto>();
            //mapper.CreateMap<ContactListDto, Contact>();

            ////Locations
            //mapper.CreateMap<LocationDetailsViewModel, Thipper.Models.Location>();
            //mapper.CreateMap<Thipper.Models.Location, LocationDetailsViewModel>();

            //mapper.CreateMap<Location, LocationListDto>();
            //mapper.CreateMap<LocationListDto, Location>();
        }
    }
}