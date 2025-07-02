using System.Text;
using MailKit.Net.Smtp;
using MimeKit;
using WebScraper_RyanW84.Models;

namespace WebScraper_RyanW84.Service;

internal class Email
{
    internal async Task SendEmail(BasketballResults results)
    {
        var email = new MimeMessage();

        email.From.Add(new MailboxAddress("Ryan Weavers", "ryanweavers@gmail.com"));
        email.To.Add(new MailboxAddress("Bob Jones", "ryanweavers@gmail.com"));

        email.Subject = $"{results.EmailTitle} {DateTime.Now}";

        // Build the HTML body
        var sb = new StringBuilder();
        sb.AppendLine("<html><body>");
        sb.AppendLine("<p>Hello Bob,</p>");
        sb.AppendLine(
            $"<p>Here are the basketball results collected on <b>{DateTime.Now:dddd, MMMM dd, yyyy HH:mm}</b>:</p>"
        );
        sb.AppendLine($"<h2>{results.EmailTitle}</h2>");
        sb.AppendLine("<table border='1' cellpadding='5' cellspacing='0'>");

        // Table Headings
        sb.AppendLine("<tr>");
        foreach (var heading in results.EmailTableHeadings)
        {
            sb.AppendLine($"<th>{System.Net.WebUtility.HtmlEncode(heading)}</th>");
        }
        sb.AppendLine("</tr>");

        // Table Rows
        if (results.EmailTableRows != null)
        {
            foreach (var row in results.EmailTableRows)
            {
                sb.AppendLine("<tr>");
                foreach (var cell in row)
                {
                    sb.AppendLine($"<td>{System.Net.WebUtility.HtmlEncode(cell)}</td>");
                }
                sb.AppendLine("</tr>");
            }
        }

        sb.AppendLine("</table>");
        sb.AppendLine("<p>Best regards,<br/>WebScraper</p>");
        sb.AppendLine("</body></html>");

        email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = sb.ToString() };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync("smtp.gmail.com", 587, false);
        await smtp.AuthenticateAsync("smtp_username", "smtp_password");
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}
