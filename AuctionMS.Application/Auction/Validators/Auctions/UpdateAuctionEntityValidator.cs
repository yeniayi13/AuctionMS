using FluentValidation;
using AuctionMS.Common.Dtos.Auction.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionMS.Application.Auction.Validators;

namespace AuctionMS.Application.Auction.Validators.Auctions
{
    public class UpdateAuctionEntityValidator : ValidatorBase<UpdateAuctionDto>
    {
        public UpdateAuctionEntityValidator()
        {
            RuleFor(x => x.AuctionName)
               .NotEmpty().WithMessage("El campo AuctionName no puede estar vacío.")
               .MaximumLength(100).WithMessage("El campo AuctionName no puede tener más de 100 caracteres.")
               .Matches("^[^0-9]*$").WithMessage("El campo AuctionName no puede contener números.");

            RuleFor(x => x.AuctionPriceBase)
                .NotEmpty().WithMessage("El precio base de la subasta es obligatorio.")
                .ExclusiveBetween(0.01m, 100000m).WithMessage("El precio debe estar entre 0.01 y 100000.");

            RuleFor(x => x.AuctionPriceReserva)
               .NotEmpty().WithMessage("El precio de reserva de la subasta es obligatorio.")
               .ExclusiveBetween(0.01m, 100000m).WithMessage("El precio debe estar entre 0.01 y 100000.");

            RuleFor(x => x.AuctionIncremento)
               .NotEmpty().WithMessage("El campo incremento de la subasta no puede estar vacío.")
                .ExclusiveBetween(0.01m, 100000m).WithMessage("El incremento debe estar entre 0.01 y 100000.");


            RuleFor(x => x.AuctionUserId)
                .NotNull().WithMessage("Debe asignar el producto a un usuario válido.");

            RuleFor(x => x.AuctionProductId)
                .NotNull().WithMessage("Debe especificar un id de producto válido.");
        }
    }

}
