using EventManager.Application.DTOs.CheckIn;

namespace EventManager.Application.Interfaces;

public interface ICheckInService
{
    Task<CheckInResponse> ValidateAndCheckInAsync(CheckInRequest request, CancellationToken cancellationToken = default);
    Task<AttendanceReportResponse> GetAttendanceReportAsync(Guid eventId, CancellationToken cancellationToken = default);
}
