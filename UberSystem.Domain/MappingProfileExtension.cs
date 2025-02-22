﻿using AutoMapper;
using System.Numerics;
using UberSystem.Domain.Entities;
using UberSystem.Dto.Requests;
using UberSystem.Dto.Responses;
using UberSytem.Dto;
using UberSytem.Dto.Requests;
using UberSytem.Dto.Responses;

namespace UberSytem.Domain
{
    public class MappingProfileExtension : Profile
    {
        /// <summary>
        /// Mapping
        /// </summary>
        public MappingProfileExtension()
        {
            CreateMap<User, Customer>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(x => Helper.GenerateRandomLong()));
            CreateMap<User, Driver>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(x => Helper.GenerateRandomLong()));

            CreateMap<User, UserResponseModel>();
            CreateMap<SignupModel, User>()
                // Default value: False
                .ForMember(dest => dest.EmailVerified, opt => opt.MapFrom(src => false));
			CreateMap<UpdateModel, User>()
	            .ForMember(dest => dest.UserName, opt =>
		            opt.Condition(src => !string.IsNullOrEmpty(src.UserName)))
	            .ForMember(dest => dest.Password, opt =>
		            opt.Condition(src => !string.IsNullOrEmpty(src.Password)));
            CreateMap<Driver, DriverResponse>();
			CreateMap<Trip, TripResponse>().ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId.ToString()))
											.ForMember(dest => dest.DriverId, opt => opt.MapFrom(src => src.DriverId.ToString())) 
                                            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));
			CreateMap<RatingRequest, Rating>();
            CreateMap<Rating, RatingResponse>()
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating1));
		}
	}
}
