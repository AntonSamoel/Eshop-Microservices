
namespace Catalog.Api.Products.CreateProduct
{
    #region models
    public record CreateProductRequest(string Name, List<string> Category, string Description, string ImageFile, decimal Price);
    public record CreateProductresponse(Guid Id);
    #endregion

    public class CreateProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/products", async(CreateProductRequest request,ISender sender)=>{

                var command = request.Adapt<CreateProductCommand>();
                
                var result = await sender.Send(command);

                var response = result.Adapt<CreateProductresponse>();

                return Results.Created($"/products/{response.Id}", response);
            })
        .WithName("CreateProduct")
        .Produces<CreateProductresponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Create Product")
        .WithDescription("Create Product");
        }
    }
}
