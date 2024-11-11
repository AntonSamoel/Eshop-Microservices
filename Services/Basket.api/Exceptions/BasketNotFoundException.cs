using BuildingBlocks.Exeptions;

namespace Basket.api.Exceptions
{
    public class BasketNotFoundException : NotFoundException
    {
        public BasketNotFoundException(string userName) : base("basket",userName)
        {
                
        }
    }
}
