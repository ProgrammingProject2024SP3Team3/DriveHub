# Incremental Plan

## 1. Review Existing Code
- Start by reviewing the current implementation in the **BookingsController** to understand the booking logic, validation, and any relevant dependencies.
- Look at the view files related to booking to understand how data is being submitted.

## 2. Identify Dependencies
- Identify all the components involved in the booking process, including:
  - **BookingsController**
  - **ViewModel** (BookingDto)
  - **View** (Create Booking View)
  - **Database Context** (ApplicationDbContext)
  - Any services or utilities that are being called during the booking process.

## 3. Plan Test Structure
- Create a test class specifically for testing the booking functionality.
- The test class should cover:
  - Valid scenarios (successful booking).
  - Invalid scenarios (validation errors, conflicts).
  - Integration tests that simulate the entire booking process.

## 4. Test Data Handling
- Create a test data builder to construct test data efficiently without hardcoding values.
- Utilize the **InMemoryDatabase** for unit tests, ensuring the test data resembles production data.

## 5. Write Tests
- Incrementally write tests based on the scenarios identified.
- Use **xUnit** for testing, and **Moq** for mocking any dependencies.
