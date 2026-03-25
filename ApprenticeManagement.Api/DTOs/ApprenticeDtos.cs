namespace ApprenticeManagement.Api.DTOs;

public record CreateApprenticeRequest(
    string FirstName,
    string LastName,
    string Email,
    string Program,
    DateOnly StartDate,
    DateOnly? EndDate);

public record UpdateApprenticeRequest(
    string FirstName,
    string LastName,
    string Email,
    string Program,
    DateOnly StartDate,
    DateOnly? EndDate);

public record ApprenticeResponse(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string Program,
    DateOnly StartDate,
    DateOnly? EndDate);
