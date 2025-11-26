using Dunder_Store.Interfaces.IServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Dunder_Store.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public EmailService(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _env = env;
        }

        public async Task SendWelcomeAsync(Dunder_Store.Entities.Cliente cliente)
        {
            var section = _config.GetSection("Email");
            var host = section.GetValue<string>("SmtpHost");
            var from = section.GetValue<string>("From");
            var user = section.GetValue<string>("User");
            var pass = section.GetValue<string>("Password");
            var port = section.GetValue<int?>("Port") ?? 587;
            var ssl = section.GetValue<bool?>("EnableSsl") ?? true;
            var baseUrl = section.GetValue<string>("BaseUrl") ?? "http://localhost:8000";
            var brand = section.GetValue<string>("BrandName") ?? "Dunder Store";
            var logo = section.GetValue<string>("BrandLogoUrl") ?? "https://img.icons8.com/fluency/48/online-store.png";

            var htmlWelcome = BuildWelcomeHtml(cliente, brand, logo, baseUrl);
            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
            {
                await SavePreviewAsync($"welcome-{cliente.Id}.html", htmlWelcome);
                System.Console.WriteLine($"[Email] Preview salvo em /emails/welcome-{cliente.Id}.html");
                return;
            }

            using var smtp = new SmtpClient(host, port)
            {
                EnableSsl = ssl,
                Credentials = new System.Net.NetworkCredential(user, pass)
            };
            using var mail = new MailMessage(from, cliente.Email)
            {
                Subject = "Bem-vindo à Dunder Store",
                Body = htmlWelcome,
                IsBodyHtml = true
            };
            await smtp.SendMailAsync(mail);
            await SavePreviewAsync($"welcome-{cliente.Id}.html", htmlWelcome);
        }

        public async Task SendOrderReceiptAsync(Dunder_Store.Entities.Pedido pedido)
        {
            var section = _config.GetSection("Email");
            var host = section.GetValue<string>("SmtpHost");
            var from = section.GetValue<string>("From");
            var user = section.GetValue<string>("User");
            var pass = section.GetValue<string>("Password");
            var port = section.GetValue<int?>("Port") ?? 587;
            var ssl = section.GetValue<bool?>("EnableSsl") ?? true;

            var to = pedido.Cliente.Email;
            var baseUrl = section.GetValue<string>("BaseUrl") ?? "http://localhost:8000";
            var brand = section.GetValue<string>("BrandName") ?? "Dunder Store";
            var logo = section.GetValue<string>("BrandLogoUrl") ?? "https://img.icons8.com/fluency/48/purchase-order.png";

            var htmlReceipt = BuildReceiptHtml(pedido, brand, logo, baseUrl);
            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
            {
                await SavePreviewAsync($"receipt-{pedido.Id}.html", htmlReceipt);
                System.Console.WriteLine($"[Email] Preview salvo em /emails/receipt-{pedido.Id}.html");
                return;
            }

            using var smtp = new SmtpClient(host, port)
            {
                EnableSsl = ssl,
                Credentials = new System.Net.NetworkCredential(user, pass)
            };
            using var mail = new MailMessage(from, to)
            {
                Subject = "Seu pedido foi finalizado",
                Body = htmlReceipt,
                IsBodyHtml = true
            };
            await smtp.SendMailAsync(mail);
            await SavePreviewAsync($"receipt-{pedido.Id}.html", htmlReceipt);
        }

        private async Task SavePreviewAsync(string fileName, string html)
        {
            var webRoot = string.IsNullOrWhiteSpace(_env.WebRootPath) ? Path.Combine(_env.ContentRootPath, "wwwroot") : _env.WebRootPath;
            var outDir = Path.Combine(webRoot, "emails");
            Directory.CreateDirectory(outDir);
            var path = Path.Combine(outDir, fileName);
            await File.WriteAllTextAsync(path, html, Encoding.UTF8);
        }

        private string BuildWelcomeHtml(Dunder_Store.Entities.Cliente cliente, string brand, string logo, string baseUrl)
        {
            var primary = "#0B1739";
            var accent = "#E5EBFF";
            var site = baseUrl.EndsWith("/") ? baseUrl : (baseUrl + "/");
            var sb = new StringBuilder();
            sb.Append("<html><body style=\"margin:0;padding:0;background:#f5f7fb;\">");
            sb.Append($"<div style=\"max-width:640px;margin:24px auto;background:#ffffff;border-radius:12px;box-shadow:0 6px 16px rgba(0,0,0,0.06);overflow:hidden;\">");
            sb.Append($"<div style=\"background:{primary};color:#ffffff;padding:20px 24px;\"><table style=\"width:100%\"><tr><td style=\"font-size:20px;font-weight:700;\">{brand}</td><td style=\"text-align:right;\"><img alt=\"Logo\" src=\"{logo}\" width=\"32\" height=\"32\" style=\"vertical-align:middle\"/></td></tr></table></div>");
            sb.Append("<div style=\"padding:24px 24px 8px 24px;color:#1f2937;\">");
            sb.Append($"<h1 style=\"margin:0 0 12px;font-size:22px;color:{primary};\">Bem‑vindo, {cliente.Nome}!</h1>");
            sb.Append("<p style=\"margin:0 0 16px;line-height:1.6\">Seu cadastro foi concluído com sucesso. Agora você pode aproveitar ofertas, novidades e acompanhar seus pedidos com facilidade.</p>");
            sb.Append("</div>");
            sb.Append($"<div style=\"padding:0 24px 24px;\"><a href=\"{site}\" style=\"display:inline-block;background:{primary};color:#ffffff;text-decoration:none;padding:12px 18px;border-radius:8px;font-weight:600\">Acessar a loja</a></div>");
            sb.Append($"<div style=\"padding:16px 24px;background:{accent};color:#4b5563;font-size:12px\">Se você não solicitou este cadastro, desconsidere este e‑mail.</div>");
            sb.Append("</div>");
            sb.Append("<div style=\"text-align:center;color:#9CA3AF;font-size:12px;margin:8px 0\">© " + DateTime.Now.Year + " Dunder Store</div>");
            sb.Append("</body></html>");
            return sb.ToString();
        }

        private string BuildReceiptHtml(Dunder_Store.Entities.Pedido pedido, string brand, string logo, string baseUrl)
        {
            var primary = "#0B1739";
            var accent = "#E5EBFF";
            var sb = new StringBuilder();
            var nome = pedido.Cliente.Nome;
            var total = pedido.ValorTotal;
            var data = pedido.DataPedido.ToString("dd/MM/yyyy HH:mm");
            sb.Append("<html><body style=\"margin:0;padding:0;background:#f5f7fb;\">");
            sb.Append($"<div style=\"max-width:700px;margin:24px auto;background:#ffffff;border-radius:12px;box-shadow:0 6px 16px rgba(0,0,0,0.06);overflow:hidden;\">");
            sb.Append($"<div style=\"background:{primary};color:#ffffff;padding:20px 24px;\"><table style=\"width:100%\"><tr><td style=\"font-size:20px;font-weight:700;\">{brand}</td><td style=\"text-align:right;\"><img alt=\"Logo\" src=\"{logo}\" width=\"32\" height=\"32\" style=\"vertical-align:middle\"/></td></tr></table></div>");
            sb.Append("<div style=\"padding:24px;color:#1f2937;\">");
            sb.Append($"<h1 style=\"margin:0 0 8px;font-size:22px;color:{primary};\">Recibo do pedido</h1>");
            sb.Append($"<p style=\"margin:0 0 16px;line-height:1.6\">Olá {nome}, seu pedido <strong>{pedido.Id}</strong> foi finalizado em {data}.</p>");
            sb.Append("<table style=\"width:100%;border-collapse:collapse;margin-top:8px\">");
            sb.Append("<thead><tr>");
            sb.Append("<th style=\"text-align:left;padding:10px;background:#f9fafb;border-bottom:1px solid #e5e7eb\">Item</th>");
            sb.Append("<th style=\"text-align:left;padding:10px;background:#f9fafb;border-bottom:1px solid #e5e7eb\">Produto</th>");
            sb.Append("<th style=\"text-align:right;padding:10px;background:#f9fafb;border-bottom:1px solid #e5e7eb\">Qtd</th>");
            sb.Append("<th style=\"text-align:right;padding:10px;background:#f9fafb;border-bottom:1px solid #e5e7eb\">Unitário</th>");
            sb.Append("<th style=\"text-align:right;padding:10px;background:#f9fafb;border-bottom:1px solid #e5e7eb\">Subtotal</th>");
            sb.Append("</tr></thead><tbody>");
            foreach (var pp in pedido.PedidoProdutos ?? new List<Dunder_Store.Entities.PedidoProduto>())
            {
                var nm = pp.Produto?.Nome ?? pp.ProdutoNome ?? "Produto";
                var qt = pp.Quantidade;
                var pr = pp.PrecoUnitario;
                var sub = pr * qt;
                var rawImg = pp.Produto?.ImagemURL;
                var img = BuildAbsoluteImageUrl(rawImg, baseUrl);
                var imgTag = !string.IsNullOrWhiteSpace(img) ? $"<img src=\"{img}\" alt=\"{nm}\" width=\"56\" height=\"56\" style=\"border-radius:8px;object-fit:cover\"/>" : "";
                sb.Append($"<tr><td style=\"padding:10px;border-bottom:1px solid #f0f2f5\">{imgTag}</td><td style=\"padding:10px;border-bottom:1px solid #f0f2f5\">{nm}</td><td style=\"padding:10px;border-bottom:1px solid #f0f2f5;text-align:right\">{qt}</td><td style=\"padding:10px;border-bottom:1px solid #f0f2f5;text-align:right\">R$ {pr:F2}</td><td style=\"padding:10px;border-bottom:1px solid #f0f2f5;text-align:right\">R$ {sub:F2}</td></tr>");
            }
            sb.Append("</tbody></table>");
            sb.Append($"<div style=\"margin-top:16px;padding:12px;border-top:2px solid {accent};display:flex;justify-content:flex-end\"><div style=\"font-size:16px;font-weight:700;color:{primary}\">Total: R$ {total:F2}</div></div>");
            sb.Append("<p style=\"margin:16px 0 0;color:#4b5563;font-size:12px\">Este e‑mail é o seu comprovante. Guarde para referência futura.</p>");
            sb.Append("</div>");
            sb.Append($"<div style=\"padding:16px 24px;background:{accent};color:#4b5563;font-size:12px\">Dúvidas? Responda este e‑mail para falar com o suporte.</div>");
            sb.Append("</div>");
            sb.Append("<div style=\"text-align:center;color:#9CA3AF;font-size:12px;margin:8px 0\">© " + DateTime.Now.Year + " Dunder Store</div>");
            sb.Append("</body></html>");
            return sb.ToString();
        }

        private string? BuildAbsoluteImageUrl(string? storedUrl, string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(storedUrl)) return null;
            if (Uri.TryCreate(storedUrl, UriKind.Absolute, out var uri))
            {
                return uri.ToString();
            }
            var rel = storedUrl.TrimStart('/');
            var prefix = baseUrl.EndsWith("/") ? baseUrl : (baseUrl + "/");
            if (rel.StartsWith("produtos")) return prefix + rel;
            var file = System.IO.Path.GetFileName(rel);
            if (!string.IsNullOrEmpty(file)) return prefix + "produtos/" + file;
            return prefix + rel;
        }
    }
}