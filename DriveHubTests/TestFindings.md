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

## 2. Aligning Tests with Controller Behavior

We need to ensure that our tests match the current behavior of the controller. Let's take a closer look at why this mismatch is happening:

### Current Issue:
- The test **Create_ShouldFail_WhenBookingConflictsWithExistingBooking** expects a **ViewResult** when there is a booking conflict, but the controller might be returning a **RedirectToActionResult** (i.e., redirecting to another view, like **Index**).

### Why It Matters:
- If the controller is redirecting, the test needs to reflect that behavior. But typically, when a validation error like a booking conflict occurs, it's better to return the form view with the error message so that the user can correct the input. This is the more common pattern for good user experience.

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
- It's usually better for the user to remain on the form when there's a conflict, so they can immediately correct the error rather than being redirected. This behavior also aligns better with the expectations in our tests.
