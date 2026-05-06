using System.Globalization;
using EventManager.Domain.Entities;

namespace EventManager.Application.Email;

public static class EmailTemplates
{
    private static readonly CultureInfo Es = new("es-CO");

    private static string Fecha(DateTime dt) =>
        dt.ToString("dddd, d 'de' MMMM 'de' yyyy 'a las' HH:mm 'UTC'", Es);

    private static string Layout(string title, string accentColor, string body) => $@"<!DOCTYPE html>
<html lang=""es"">
<head>
  <meta charset=""UTF-8"" />
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
  <title>{title}</title>
</head>
<body style=""margin:0;padding:0;background:#f4f6f8;font-family:Arial,Helvetica,sans-serif;"">
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
    <tr><td align=""center"" style=""padding:40px 16px;"">
      <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background:#ffffff;border-radius:10px;overflow:hidden;box-shadow:0 4px 16px rgba(0,0,0,0.08);"">
        <!-- Header -->
        <tr>
          <td style=""background:{accentColor};padding:24px 40px;"">
            <h1 style=""margin:0;color:#ffffff;font-size:20px;letter-spacing:0.5px;"">EventManager</h1>
          </td>
        </tr>
        <!-- Body -->
        <tr>
          <td style=""padding:36px 40px;color:#333333;font-size:15px;line-height:1.6;"">
            {body}
          </td>
        </tr>
        <!-- Footer -->
        <tr>
          <td style=""background:#f8f9fa;padding:16px 40px;border-top:1px solid #e9ecef;"">
            <p style=""margin:0;color:#9ca3af;font-size:12px;text-align:center;"">
              Este mensaje es generado automáticamente. Por favor no respondas este correo.
            </p>
          </td>
        </tr>
      </table>
    </td></tr>
  </table>
</body>
</html>";

    private static string FilaTabla(string label, string value, bool last = false)
    {
        var border = last ? "" : "border-bottom:1px solid #e9ecef;";
        return $@"<tr>
            <td style=""padding:12px 16px;background:#f8f9fa;{border}font-weight:bold;color:#555;width:140px;"">{label}</td>
            <td style=""padding:12px 16px;{border}"">{value}</td>
          </tr>";
    }

    // ──────────────────────────────────────────────────────────────────────────
    // 1. Confirmación de inscripción
    // ──────────────────────────────────────────────────────────────────────────
    public static string RegistrationConfirmation(Registration registration)
    {
        var ev = registration.Event;
        var user = registration.User;

        var filas = FilaTabla("Evento", ev.Name)
                  + FilaTabla("Fecha de inicio", Fecha(ev.StartDate))
                  + (ev.EndDate.HasValue ? FilaTabla("Fecha de fin", Fecha(ev.EndDate.Value)) : "")
                  + FilaTabla("Lugar", ev.Location ?? "Por confirmar")
                  + FilaTabla("Inscrito el", Fecha(registration.RegisteredAt), last: true);

        var body = $@"<h2 style=""color:#1a56db;margin-top:0;"">¡Inscripción confirmada!</h2>
            <p>Hola <strong>{user.FirstName} {user.LastName}</strong>,</p>
            <p>Tu inscripción ha sido registrada exitosamente. Aquí están los detalles:</p>
            <table width=""100%"" cellpadding=""0"" cellspacing=""0""
                   style=""margin:24px 0;border:1px solid #e9ecef;border-radius:6px;border-collapse:collapse;"">
              {filas}
            </table>
            <p>
              Se adjunta tu <strong>código QR</strong> de acceso. Preséntalo el día del evento
              para registrar tu asistencia.
            </p>
            <p style=""color:#6b7280;font-size:14px;"">¡Te esperamos!</p>";

        return Layout("Confirmación de inscripción", "#1a56db", body);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // 2. Recordatorio 24 horas antes
    // ──────────────────────────────────────────────────────────────────────────
    public static string EventReminder(Registration registration)
    {
        var ev = registration.Event;
        var user = registration.User;

        var filas = FilaTabla("Evento", ev.Name)
                  + FilaTabla("Fecha", Fecha(ev.StartDate))
                  + FilaTabla("Lugar", ev.Location ?? "Por confirmar", last: true);

        var body = $@"<h2 style=""color:#1a56db;margin-top:0;"">Recordatorio de evento</h2>
            <p>Hola <strong>{user.FirstName} {user.LastName}</strong>,</p>
            <p>Te recordamos que <strong>mañana</strong> tienes un evento al que estás inscrito:</p>
            <table width=""100%"" cellpadding=""0"" cellspacing=""0""
                   style=""margin:24px 0;border:1px solid #e9ecef;border-radius:6px;border-collapse:collapse;"">
              {filas}
            </table>
            <p>Recuerda llevar tu <strong>código QR</strong> de acceso. ¡Te esperamos!</p>";

        return Layout("Recordatorio de evento", "#1a56db", body);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // 3. Notificación de cambio en el evento
    // ──────────────────────────────────────────────────────────────────────────
    public static string EventChanged(User user, Event ev)
    {
        var filas = FilaTabla("Evento", ev.Name)
                  + FilaTabla("Nueva fecha de inicio", Fecha(ev.StartDate))
                  + (ev.EndDate.HasValue ? FilaTabla("Nueva fecha de fin", Fecha(ev.EndDate.Value)) : "")
                  + FilaTabla("Nuevo lugar", ev.Location ?? "Por confirmar", last: true);

        var body = $@"<h2 style=""color:#e85d04;margin-top:0;"">Actualización de evento</h2>
            <p>Hola <strong>{user.FirstName} {user.LastName}</strong>,</p>
            <p>
              El organizador ha realizado cambios en un evento al que estás inscrito.
              A continuación encontrarás la información actualizada:
            </p>
            <table width=""100%"" cellpadding=""0"" cellspacing=""0""
                   style=""margin:24px 0;border:1px solid #e9ecef;border-radius:6px;border-collapse:collapse;"">
              {filas}
            </table>
            <p>Si tienes dudas, comunícate con el organizador del evento.</p>
            <p style=""color:#6b7280;font-size:14px;"">Disculpa las molestias ocasionadas.</p>";

        return Layout("Actualización de evento", "#e85d04", body);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // 4. Confirmación de check-in
    // ──────────────────────────────────────────────────────────────────────────
    public static string CheckInConfirmation(Registration registration)
    {
        var ev = registration.Event;
        var user = registration.User;

        var filas = FilaTabla("Evento", ev.Name)
                  + FilaTabla("Fecha del evento", Fecha(ev.StartDate))
                  + FilaTabla("Check-in realizado", Fecha(DateTime.UtcNow), last: true);

        var body = $@"<h2 style=""color:#0e9f6e;margin-top:0;"">¡Asistencia confirmada!</h2>
            <p>Hola <strong>{user.FirstName} {user.LastName}</strong>,</p>
            <p>Tu asistencia al siguiente evento ha sido registrada exitosamente:</p>
            <table width=""100%"" cellpadding=""0"" cellspacing=""0""
                   style=""margin:24px 0;border:1px solid #e9ecef;border-radius:6px;border-collapse:collapse;"">
              {filas}
            </table>
            <p>¡Gracias por asistir! Esperamos que disfrutes el evento.</p>";

        return Layout("Asistencia confirmada", "#0e9f6e", body);
    }
}
