using DriveHubModel;

public class TestDataBuilder
{
    public Vehicle CreateValidVehicle()
    {
        return new Vehicle
        {
            VehicleId = "example-id",
            Make = "Tesla",
            Model = "Model Y",
            // Other properties...
        };
    }

    public BookingDto CreateValidBookingDto()
    {
        return new BookingDto
        {
            // Set properties for a valid booking...
        };
    }
}
