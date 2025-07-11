using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionMS.Common.Dtos.Auction.Request;
using AuctionMS.Domain.Entities.Auction;
using AutoMapper;

namespace AuctionMS.Common.AutoMapper
{
        [ExcludeFromCodeCoverage]
        public class ClaimProfile : Profile
        {
            public ClaimProfile()
            {
                CreateMap<ClaimPrizeAuction, ClaimPrizeDto>()
                    .ForMember(dest => dest.AuctionId, opt => opt.MapFrom(src => src.AuctionId))
                    .ForMember(dest => dest.BidId, opt => opt.MapFrom(src => src.BidId))
                    .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                    .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => src.ShippingAddress))
                    .ForMember(dest => dest.DeliveryMethod, opt => opt.MapFrom(src => src.DeliveryMethod))
                    .ForMember(dest => dest.ContactPhone, opt => opt.MapFrom(src => src.ContactPhone))
                    .ForMember(dest => dest.AdditionalNotes, opt => opt.MapFrom(src => src.AdditionalNotes))
                    .ReverseMap();
            }
        }
    }


