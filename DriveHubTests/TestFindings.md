# Booking Test Findings

## 1. **Booking Conflict Logic**

At the moment, the **BookingsController** is handling booking conflicts correctly in terms of logic, but there’s a slight issue with how it responds to those conflicts. Here’s the conflict check:

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

### What’s Happening:
- The conflict is identified fine, but it doesn’t seem to return a **ViewResult** when a conflict occurs. Instead, it’s adding an error to **ModelState**, checking if **ModelState** is valid, and proceeding accordingly. If it’s invalid, the form reloads, but otherwise, the booking goes through.

### Why the Test Fails:
- The test **Create_ShouldFail_WhenBookingConflictsWithExistingBooking** expects a **ViewResult** to reload the form and show the error. But it looks like the controller is possibly redirecting (to something like **Index**), which causes the test to fail since the expectation is to stay on the **Create** view.

### Action Plan:
- We need to make sure the controller returns a **ViewResult** when there’s a conflict so that the user stays on the form and sees the error. Something like this would work:

```csharp
if (conflictingBookings)
{
    ModelState.AddModelError("VehicleId", "The selected vehicle is already booked during this time range.");
    return View(bookingDto); // Reload the Create view with the error
}
```

- If we don’t want to change the controller, another option would be to adjust the test to expect a redirect. But honestly, it’s better to reload the form with the error message for a smoother user experience.

---

## 2. **Aligning Tests with Controller Behavior**

We need to make sure our tests reflect what the controller is actually doing.

### The Issue:
- The test **Create_ShouldFail_WhenBookingConflictsWithExistingBooking** expects a **ViewResult** when there’s a conflict, but the controller might be returning a **RedirectToActionResult** (redirecting to **Index** or some other view).

### Why This Matters:
- If the controller is redirecting, the test needs to reflect that. But in cases like a booking conflict, it’s usually better UX to stay on the form and show the error so the user can fix the issue.

### Two Solutions:
1. **Update the Test**: If we’re happy with the redirect behavior, we can change the test to expect a redirect instead of a view reload:

    ```csharp
    var result = await bookingTestFixtures.Controller.Create(bookingDto);
    var redirectResult = Assert.IsType<RedirectToActionResult>(result);
    Assert.Equal("Index", redirectResult.ActionName); // Adjust if it's redirecting elsewhere
    ```

2. **Update the Controller Logic**: If we prefer keeping the user on the form, we can modify the controller to return a **ViewResult** when there’s a conflict. This keeps the user on the page and displays the error:

    ```csharp
    if (!ModelState.IsValid)
    {
        return View(bookingDto); // Keep the user on the form and show validation errors
    }
    ```

### Conclusion:
- Keeping the user on the form to fix the conflict error feels like the smoother option from a UX perspective, and it aligns better with our tests.

---

## Successful Test Findings

### 1. **Details_ShouldReturnNotFound_WhenBookingDoesNotExist**
- **Result**: Passed  
- **Purpose**: This test checks that when a user tries to view a non-existent booking, the controller correctly returns a 404 (NotFound).
- **What This Means**: The controller correctly handles invalid booking IDs and returns a 404 when necessary. It’s doing what we’d expect: not showing data for bookings that don’t exist.

### 2. **Details_ShouldReturnBooking_WhenExists**
- **Result**: Passed  
- **Purpose**: This test verifies that when a valid booking ID is provided, the controller returns the right view with the booking details.
- **What This Means**: The logic for retrieving and displaying booking details is solid. The controller properly returns the booking when it exists, and the details are displayed correctly.

### 3. **Search_ShouldReturnAvailableVehicles**
- **Result**: Passed  
- **Purpose**: This test checks that the **Search** action returns a list of available vehicles for booking.
- **What This Means**: The search functionality is working as expected. The controller is correctly pulling available vehicles from the database and returning them in the search view. It’s solid with the current dataset.

### 4. **Search_ShouldReturnEmptyList_WhenNoVehiclesAreAvailable**
- **Result**: Passed  
- **Purpose**: This test checks that when no vehicles are available, the search result returns an empty list.
- **What This Means**: The logic handles the edge case where no vehicles are available correctly. It doesn’t break or throw an error; instead, it returns an empty list, which is exactly what we want.

---

### Summary:
The successful tests show that the **Details** and **Search** functionalities are working well. The 404 error handling is solid, and both the search results and edge cases (like no vehicles being available) are handled correctly. The core functionality for retrieving and displaying bookings, as well as searching for vehicles, seems reliable.
