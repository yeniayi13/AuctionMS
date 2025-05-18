using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Common.Dtos.Auction.Response;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AuctionMS.Common.AutoMapper
{
    public class AuctionProfile : Profile
    {
        public AuctionProfile()
        {
            CreateMap<AuctionEntity, GetAuctionDto>()
            .ForMember(dest => dest.AuctionId, opt => opt.MapFrom(src => src.AuctionId.Value))
            .ForMember(dest => dest.AuctionName, opt => opt.MapFrom(src => src.AuctionName.Value))
            .ForMember(dest => dest.AuctionImage, opt => opt.MapFrom(src => src.AuctionImage.Url))
            .ForMember(dest => dest.AuctionPriceBase, opt => opt.MapFrom(src => src.AuctionPriceBase.Value))
            .ForMember(dest => dest.AuctionPriceReserva, opt => opt.MapFrom(src => src.AuctionPriceReserva.Value))
            .ForMember(dest => dest.AuctionDescription, opt => opt.MapFrom(src => src.AuctionDescription.Value))
            .ForMember(dest => dest.AuctionDuracion, opt => opt.MapFrom(src => src.AuctionDuracion.Value))
            .ForMember(dest => dest.AuctionIncremento, opt => opt.MapFrom(src => src.AuctionIncremento.Value))
            .ForMember(dest => dest.AuctionCondiciones, opt => opt.MapFrom(src => src.AuctionCondiciones.Value))

            .ForMember(dest => dest.AuctionUserId, opt => opt.MapFrom(src => src.AuctionUserId.Value))
   
            
            .ReverseMap();
        }
    }
}
