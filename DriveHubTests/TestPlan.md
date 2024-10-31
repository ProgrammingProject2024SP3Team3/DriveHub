# Incremental Plan

## 1. Review Existing Code
- Review the following components to understand the overall booking process:
  - **BookingsController**
    - Focus on actions related to booking: Search, Create, Details, Edit, and Delete.
  - **BookingDto**: Understand the data structure for bookings.
  - **View Files**: Examine the views for searching, creating, and viewing bookings.
  - **ApplicationDbContext**: Understand the database context and how bookings, vehicles, and pods are managed.

## 2. Identify Dependencies
- Identify all the components involved in the booking process:
  - **BookingsController**: Responsible for handling booking requests.
  - **ViewModel** (BookingDto): Carries data for the booking operations.
  - **Database Context** (ApplicationDbContext): Handles data interactions.
  - **Validation Attributes**: Ensure proper validation on models.
  - **Services or Utilities**: Any external services or utilities used in booking logic (e.g., for payment processing or notifications).

## 3. Plan Test Structure
- Create a test class specifically for testing the booking functionality.
- The test class should cover:
  - Valid scenarios (successful booking).
  - Invalid scenarios (validation errors, conflicts).
  - Integration tests that simulate the entire booking process.

## 4. Test Coverage
- Ensure tests cover the following areas:
  - **Search Functionality**: Verify available vehicles and their details.
  - **Create Booking**: Test successful booking and various failure cases (validation errors, overlapping bookings).
  - **View Details**: Check if booking details are retrieved correctly.
  - **Extend Reservation**: Validate the editing process and ensure changes are saved.
  - **Cancel Reservation**: Ensure reservations can be cancelled correctly.

## 5. Test Data Handling
- Implement a test data builder to create test data efficiently without hardcoding values.
- Utilize the **InMemoryDatabase** for unit tests, ensuring that the test data resembles production data.

## 6. Create data sets
- Create 7x white data sets that represent different states for a booking
- The sets are:
    1. User A has reserved Iron Stallion
    2. User A has collected Iron Stallion
    3. User A has unpaid booking for Iron Stallion
    4. User A has a complete booking for Iron Stallion
    5. User A has a cancelled booking for Iron Stallion
    6. User A has an expired reservation for Iron Stallion
    7. User A has an extended reservation for Iron Stallion

## 7. Write Tests
- Incrementally write tests based on the scenarios identified.
- Use **xUnit** for testing, and **Moq** for mocking any dependencies.