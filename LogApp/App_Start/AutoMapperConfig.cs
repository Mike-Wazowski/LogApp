using AutoMapper;
using LogApp.Database.Models;
using LogApp.Helpers;
using LogApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogApp.App_Start
{
    public class AutoMapperConfig
    {
        public static void InitializeAutoMapper()
        {
            Mapper.Reset();
            Mapper.CreateMap<LogType, LogTypeViewModel>()
                .IgnoreAllUnmapped()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Headers, opt => opt.MapFrom(src => src.GetHeaders()));
            Mapper.AssertConfigurationIsValid();
        }
    }
}