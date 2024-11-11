﻿

using Discount.Grpc;

namespace Basket.api.Basket.UpdateBasket
{
    public record StoreBasketCommand(ShoppingCart Cart) : ICommand<StoreBasketResult>;
    public record StoreBasketResult(string UserName);

    public class StoreBasketValidator : AbstractValidator<StoreBasketCommand>
    {
        public StoreBasketValidator()
        {
            RuleFor(x => x.Cart).NotNull().WithMessage("Cart can not be null");
            RuleFor(x => x.Cart.UserName).NotEmpty().WithMessage("UserName is required");
        }
    }
    public class StoreBasketCommandHandler(IBasketRepository repository,DiscountProtoService.DiscountProtoServiceClient discountProto)
        : ICommandHandler<StoreBasketCommand, StoreBasketResult>
    {
        public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
        {
            await DeductDiscount(command.Cart,cancellationToken);

            await repository.StoreBasketAsync(command.Cart, cancellationToken);

            return new StoreBasketResult(command.Cart.UserName);
        }


        private async Task DeductDiscount(ShoppingCart Cart, CancellationToken cancellationToken)
        {
            foreach (var item in Cart.Items)
            {
                var coupon = await discountProto.GetDiscountAsync(new GetDiscountRequest() { ProductName = item.ProductName },cancellationToken:cancellationToken);
                item.Price -= coupon.Amount;
            }
        }
    }

}
