using MediatR;
using AutoMapper;
using AuctionMS.Core.Repository;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Application.Auction.Commands;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Core.RabbitMQ;

namespace AuctionMS.Application.Auction.Handlers.Commands
{
    public class DeleteAuctionCommandHandler : IRequestHandler<DeleteAuctionCommand, Guid>
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly IAuctionRepositoryMongo _auctionRepositoryMongo;
        private readonly IEventBus<GetAuctionDto> _eventBus;
        private readonly IMapper _mapper;

        public DeleteAuctionCommandHandler(IAuctionRepositoryMongo auctionRepositoryMongo, IAuctionRepository auctionRepository, IEventBus<GetAuctionDto> eventBus, IMapper mapper)
        {
           _auctionRepository = auctionRepository ?? throw new ArgumentNullException(nameof(auctionRepository)); //*Valido que estas inyecciones sean exitosas
            _eventBus = eventBus;
            _mapper = mapper;//*Valido que estas inyecciones sean exitosas
            _auctionRepositoryMongo = auctionRepositoryMongo;
        }

        public async Task<Guid> Handle(DeleteAuctionCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Request cannot be null.");
            }


            var auctionId = AuctionId.Create(request.AuctionId);
            var userId = AuctionUserId.Create(request.UserId);
            var productId = AuctionProductId.Create(request.ProductId);

            var auction = await _auctionRepositoryMongo.GetByIdAsync(auctionId, userId, productId);
            if (auction == null)
            {
                throw new Exception("Product not found."); // Esta excepción debe existir
            }

            await _auctionRepository.DeleteAsync(auctionId);

            var auctionDto = _mapper.Map<GetAuctionDto>(auction);
            await _eventBus.PublishMessageAsync(auctionDto, "auctionQueue", "AUCTION_DELETED");

            return auctionId.Value;

        }
    }
}
