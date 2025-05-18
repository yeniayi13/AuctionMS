using MediatR;
using AuctionMS.Core.Repository;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Application.Auction.Commands;
using AuctionMS.Infrastructure.Repositories;
using AuctionMS.Common.Dtos.Auction.Response;

namespace AuctionMS.Application.Auction.Handlers.Commands
{
    public class DeleteAuctionCommandHandler : IRequestHandler<DeleteAuctionCommand, Guid>
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly IEventBus<GetAuctionDto> _eventBus;
        private readonly IMapper _mapper;

        public DeleteAuctionCommandHandler(IAuctionRepository auctionRepository, IEventBus<GetAuctionDto> eventBus, IMapper mapper)
        {
           _auctionRepository = auctionRepository ?? throw new ArgumentNullException(nameof(auctionRepository)); //*Valido que estas inyecciones sean exitosas
            _eventBus = eventBus;
            _mapper = mapper;//*Valido que estas inyecciones sean exitosas
        }

        public async Task<Guid> Handle(DeleteAuctionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var auctionId = AuctionId.Create(request.AuctionId);
                var userId = AuctiontUserId.Create(request.UserId);
                var auction = await _auctionRepository.GetByIdAsync(auctionId, userId);
                await _auctionRepository.DeleteAsync(auctionId);
                var auctionDto = _mapper.Map<GetAuctionDto>(auction);
                await _eventBus.PublishMessageAsync(auctionDto, "productQueue", "PRODUCT_DELETED");
                return auctionId.Value;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
