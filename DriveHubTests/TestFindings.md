# Booking Test Findings

## 1. Booking Conflict Logic

Currently, the **BookingsController** checks for conflicts in bookings before allowing new bookings to proceed. Here's the existing logic:

```csharp
var conflictingBookings = _context.Bookings
    .Where(b => b.VehicleId == bookingDto.VehicleId &&
                ((b.StartTime <= bookingDto.EndTime && b.StartTime >= bookingDto.StartTime) ||
                 (b.EndTime <= bookingDto.EndTime && b.EndTime >= bookingDto.StartTime)))
    .Any();

if (conflictingBookings)
{
    ModelState.AddModelError("VehicleId", "The selected vehicle is already booked during this time range.");
}
```

### Issues Identified:
- The logic correctly identifies conflicts, but it doesn't necessarily return a **ViewResult** when a conflict is found. The current behavior is that the controller adds a **ModelState** error and proceeds to check the validity of **ModelState**. If invalid, the form is reloaded, but otherwise, the booking proceeds.

### Test Failure Explanation:
- In the test **Create_ShouldFail_WhenBookingConflictsWithExistingBooking**, we expect a **ViewResult** to be returned when a conflict arises so that the form reloads with the error message displayed. However, the controller might be redirecting to another view (like **Index**), causing the test to fail since it's expecting to stay on the **Create** view.

### Action Plan:
- The controller should ensure it returns a **ViewResult** in the case of a booking conflict to keep the user on the form and display the error message. The fix could look something like this:

```csharp
if (conflictingBookings)
{
    ModelState.AddModelError("VehicleId", "The selected vehicle is already booked during this time range.");
    return View(bookingDto); // Reload the Create view and display the error
}
```

- If you don't want to modify the controller, we could instead modify the test to expect a redirect to another view. However, reloading the form with the error message would be a better user experience.

---

## 2. Aligning Tests with Controller Behavior

We need to ensure that our tests match the current behavior of the controller.

### Current Issue:
- The test **Create_ShouldFail_WhenBookingConflictsWithExistingBooking** expects a **ViewResult** when there is a booking conflict, but the controller might be returning a **RedirectToActionResult** (i.e., redirecting to another view, like **Index**).

### Why It Matters:
- If the controller is redirecting, the test needs to reflect that behavior. But typically, when a validation error like a booking conflict occurs, it's better to return the form view with the error message so that the user can correct the input.

### Solutions:
1. **Update the Test**: If we want to maintain the controller's current behavior of redirecting, we can adjust the test to expect a redirect:

```csharp
var result = await bookingTestFixtures.Controller.Create(bookingDto);
var redirectResult = Assert.IsType<RedirectToActionResult>(result);
Assert.Equal("Index", redirectResult.ActionName); // Change "Index" if redirecting elsewhere
```

2. **Update the Controller Logic**: Alternatively, we can change the controller to return a **ViewResult** (as suggested in point 1), ensuring it stays on the form when there's a conflict, showing the validation errors:

```csharp
if (!ModelState.IsValid)
{
    return View(bookingDto); // Stay on the Create view and display errors
}
```

### Conclusion:
- It's usually better for the user to remain on the form when there's a conflict, so they can immediately correct the error rather than being redirected.

---

## Test Findings for Successful Tests

### 1. **Details_ShouldReturnNotFound_WhenBookingDoesNotExist**
- **Outcome**: Passed
- **Purpose**: This test ensures that when a user requests the details of a non-existent booking, the **BookingsController** returns a `NotFoundResult` (HTTP 404).
- **Findings**: The controller correctly returns a 404 when the specified booking does not exist. This means the logic for checking whether a booking exists is functioning as expected, ensuring users aren’t shown a booking that doesn't exist.

### 2. **Details_ShouldReturnBooking_WhenExists**
- **Outcome**: Passed
- **Purpose**: This test verifies that when a valid booking ID is provided, the **BookingsController** returns the correct `ViewResult` containing the booking details.
- **Findings**: The controller properly returns the `ViewResult` when a booking exists in the database, and the test confirms that the booking details are displayed correctly. The booking retrieval logic is solid and working correctly.

### 3. **Search_ShouldReturnAvailableVehicles**
- **Outcome**: Passed
- **Purpose**: This test ensures that the **Search** action in the **BookingsController** returns a list of available vehicles for booking.
- **Findings**: The controller successfully returns a `ViewResult` with a populated `BookingSearchVM` model, indicating that vehicles are correctly retrieved from the database and made available for the search functionality. This test passing confirms that the search functionality works as expected with the current dataset.

### 4. **Search_ShouldReturnEmptyList_WhenNoVehiclesAreAvailable**
- **Outcome**: Passed
- **Purpose**: This test verifies that when no vehicles are available, the search result returns an empty list.
- **Findings**: The controller appropriately returns an empty list of vehicles when there are none available. This test passing ensures that the search logic can handle edge cases where no vehicles are present in the database, leading to a smoother user experience.

---

### Overall Summary:
The successful tests confirm that the `Details` and `Search` functionalities in the **BookingsController** are working correctly. When there are no vehicles available, the **Search** action correctly returns an empty list, and when invalid booking IDs are used, the **Details** view returns a 404 as expected. The core logic for viewing booking details and searching for available vehicles is functioning properly, ensuring that these user-facing features are reliable.
