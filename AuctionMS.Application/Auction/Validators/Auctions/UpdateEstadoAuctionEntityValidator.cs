using FluentValidation;
using AuctionMS.Common.Dtos.Auction.Request;

namespace AuctionMS.Application.Auction.Validators.Auctions
{
    public class UpdateEstadoAuctionEntityValidator : ValidatorBase<UpdateEstadoAuctionDto>
    {
        public UpdateEstadoAuctionEntityValidator()
        {
            RuleFor(x => x.CorrelationId).NotEmpty();
            RuleFor(x => x.CurrentState)
                .NotEmpty()
                .Must(s => new[] { "Pending", "Active", "Ended", "Canceled", "Completed" }.Contains(s))
                .WithMessage("Estado inválido.");
        }
    }
}
