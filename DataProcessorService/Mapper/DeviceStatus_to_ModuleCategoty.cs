
using AutoMapper;


namespace api_fact_weather_by_city.Mapper
{
    public class DeviceStatus_to_ModuleCategoty : Profile
    {
        public DeviceStatus_to_ModuleCategoty()
        {
            CreateMap<DeviceStatus,ModuleCategory>()
            .ForMember(mod => mod.ModuleCategoryID, opt => opt.MapFrom(dev => dev.ModuleCategoryID))
            .ForMember(mod => mod.ModuleState, opt => opt.MapFrom(dev => dev.RapidControl.ModuleState))
            .ReverseMap();         
        }
        
    }
}