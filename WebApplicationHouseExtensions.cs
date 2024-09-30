using Microsoft.AspNetCore.Mvc;
using MiniValidation;

public static class WebApplicationHouseExtensions
{
    public static void MapHouseEndpoints(this WebApplication app)
    {
       
//Endpoint for getting all houses
app.MapGet("/houses", (IHouseRepository repo) => repo.GetAll())
.Produces<HouseDto[]>(StatusCodes.Status200OK);

//Endpoint for getting details for one house
app.MapGet("/house/{houseId:int}", async (int houseId, IHouseRepository repo) =>
{
    var house = await repo.Get(houseId);
    if (house == null)
        return Results.Problem($"House with Id {houseId} not found.",
        statusCode: 404);
    return Results.Ok(house);
}).ProducesProblem(404).Produces<HouseDetailDto>(StatusCodes.Status200OK);

//Endpoint for creating houses
app.MapPost("/houses", async ([FromBody] HouseDetailDto dto,
IHouseRepository repo) =>
{
    if (!MiniValidator.TryValidate(dto, out var errors))
    return Results.ValidationProblem(errors);
    var newHouse = await repo.Add(dto);
    return Results.Created($"/house/{newHouse.Id}", newHouse);
}).Produces<HouseDetailDto>(StatusCodes.Status201Created)
.ProducesValidationProblem();

//Endpoint for updating houses
app.MapPut("/houses", async ([FromBody] HouseDetailDto dto,
IHouseRepository repo) =>
{
    if (!MiniValidator.TryValidate(dto, out var errors))
    return Results.ValidationProblem(errors);
    if (await repo.Get(dto.Id) == null)
        return Results.Problem($"House {dto.Id} not found",
        statusCode: 404);
    var updatedHouse = await repo.Update(dto);
    return Results.Ok(updatedHouse);
}).ProducesValidationProblem().ProducesProblem(404).Produces<HouseDetailDto>(StatusCodes.Status200OK);

//Endpoint for deleting houses
app.MapDelete("/houses/{houseId:int}", async (int houseId,
IHouseRepository repo) =>
{
    if (await repo.Get(houseId) == null)
        return Results.Problem($"house {houseId} not found",
        statusCode: 404);
    await repo.Delete(houseId);
    return Results.Ok();
}).ProducesProblem(404).Produces(StatusCodes.Status200OK);

    }
}