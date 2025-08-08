using System.Diagnostics;
using System.Text;

namespace PosSystem.Services
{
    public interface IHtmlToPdfService
    {
        Task<byte[]> ConvertHtmlToPdfAsync(string html, string? outputPath = null);
        Task<Stream> ConvertHtmlToPdfStreamAsync(string html);
    }

    public class HtmlToPdfService : IHtmlToPdfService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<HtmlToPdfService> _logger;

        public HtmlToPdfService(IConfiguration configuration, ILogger<HtmlToPdfService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<byte[]> ConvertHtmlToPdfAsync(string html, string? outputPath = null)
        {
            var tempDir = _configuration["StoragePath"] ?? "./temp";
            Directory.CreateDirectory(tempDir);

            var htmlFile = Path.Combine(tempDir, $"temp_{Guid.NewGuid()}.html");
            var pdfFile = outputPath ?? Path.Combine(tempDir, $"temp_{Guid.NewGuid()}.pdf");

            try
            {
                // Write HTML to temporary file
                await File.WriteAllTextAsync(htmlFile, html, Encoding.UTF8);

                // Try different PDF conversion methods
                bool success = false;

                // Method 1: Try wkhtmltopdf (most common)
                if (!success)
                {
                    success = await TryWkhtmltopdf(htmlFile, pdfFile);
                }

                // Method 2: Try Chrome/Chromium headless
                if (!success)
                {
                    success = await TryChromePdf(htmlFile, pdfFile);
                }

                // Method 3: Try Puppeteer (if available)
                if (!success)
                {
                    success = await TryPuppeteerPdf(htmlFile, pdfFile);
                }

                // Method 4: Fallback to basic HTML rendering (limited)
                if (!success)
                {
                    _logger.LogWarning("All PDF conversion methods failed, using fallback method");
                    success = await CreateFallbackPdf(html, pdfFile);
                }

                if (!success)
                {
                    throw new InvalidOperationException("Failed to convert HTML to PDF using all available methods");
                }

                // Read the generated PDF
                var pdfBytes = await File.ReadAllBytesAsync(pdfFile);
                return pdfBytes;
            }
            finally
            {
                // Clean up temporary files
                try
                {
                    if (File.Exists(htmlFile))
                        File.Delete(htmlFile);
                    if (File.Exists(pdfFile) && outputPath == null)
                        File.Delete(pdfFile);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to clean up temporary files");
                }
            }
        }

        public async Task<Stream> ConvertHtmlToPdfStreamAsync(string html)
        {
            var pdfBytes = await ConvertHtmlToPdfAsync(html);
            return new MemoryStream(pdfBytes);
        }

        private async Task<bool> TryWkhtmltopdf(string htmlFile, string pdfFile)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "wkhtmltopdf",
                    Arguments = $"--page-size A4 --margin-top 0.75in --margin-right 0.75in --margin-bottom 0.75in --margin-left 0.75in \"{htmlFile}\" \"{pdfFile}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var process = Process.Start(startInfo);
                if (process != null)
                {
                    await process.WaitForExitAsync();
                    return process.ExitCode == 0 && File.Exists(pdfFile);
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "wkhtmltopdf conversion failed");
            }

            return false;
        }

        private async Task<bool> TryChromePdf(string htmlFile, string pdfFile)
        {
            try
            {
                var chromeCommands = new[]
                {
                    "google-chrome",
                    "chromium-browser",
                    "chromium",
                    "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome"
                };

                foreach (var chrome in chromeCommands)
                {
                    try
                    {
                        var startInfo = new ProcessStartInfo
                        {
                            FileName = chrome,
                            Arguments = $"--headless --disable-gpu --print-to-pdf=\"{pdfFile}\" \"file://{Path.GetFullPath(htmlFile)}\"",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true
                        };

                        using var process = Process.Start(startInfo);
                        if (process != null)
                        {
                            await process.WaitForExitAsync();
                            if (process.ExitCode == 0 && File.Exists(pdfFile))
                            {
                                return true;
                            }
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Chrome PDF conversion failed");
            }

            return false;
        }

        private async Task<bool> TryPuppeteerPdf(string htmlFile, string pdfFile)
        {
            try
            {
                // This would require a Node.js script with Puppeteer
                // For now, we'll skip this implementation
                await Task.CompletedTask;
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Puppeteer PDF conversion failed");
                return false;
            }
        }

        private async Task<bool> CreateFallbackPdf(string html, string pdfFile)
        {
            try
            {
                // This is a very basic fallback that creates a simple PDF-like structure
                // In a real implementation, you might want to use a library like iTextSharp
                // For now, we'll create a simple text-based PDF
                
                var pdfContent = CreateBasicPdfContent(html);
                await File.WriteAllBytesAsync(pdfFile, pdfContent);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fallback PDF creation failed");
                return false;
            }
        }

        private byte[] CreateBasicPdfContent(string html)
        {
            // This is a very simplified PDF creation
            // In production, you should use a proper PDF library
            var pdfHeader = "%PDF-1.4\n";
            var cleanText = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]*>", "");
            
            var pdfBodyContent = $@"1 0 obj
<<
/Type /Catalog
/Pages 2 0 R
>>
endobj

2 0 obj
<<
/Type /Pages
/Kids [3 0 R]
/Count 1
>>
endobj

3 0 obj
<<
/Type /Page
/Parent 2 0 R
/MediaBox [0 0 612 792]
/Contents 4 0 R
>>
endobj

4 0 obj
<<
/Length {cleanText.Length + 50}
>>
stream
BT
/F1 12 Tf
50 750 Td
({cleanText}) Tj
ET
endstream
endobj

xref
0 5
0000000000 65535 f 
0000000010 00000 n 
0000000053 00000 n 
0000000125 00000 n 
0000000185 00000 n 
trailer
<<
/Size 5
/Root 1 0 R
>>
startxref
500
%%EOF";

            return Encoding.UTF8.GetBytes(pdfHeader + pdfBodyContent);
        }
    }
}