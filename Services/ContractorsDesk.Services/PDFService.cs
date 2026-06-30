using ContractorsDesk.Services.Interfaces;
using DinkToPdf;
using DinkToPdf.Contracts;
using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Utilities;


namespace ContractorsDesk.Services
{
	public class PDFService : IPDFService
	{
		private readonly IConverter pdfConverter;

		public PDFService(IConverter pdfConverter)
		{
			this.pdfConverter = pdfConverter;
		}

		#region Public
		public byte[] GetCostPlusPDF(ClientContractDetails clientContractDetails)
		{
			var htmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "PDF", $"Templates/ClientContract/cost-plus.html");
			var styleSheetFilePath = Path.Combine(Directory.GetCurrentDirectory(), "PDF", $"Templates/ClientContract/style.css");

			var htmlContent = GetTemplateContent(htmlFilePath, styleSheetFilePath);

			htmlContent = htmlContent.Replace("{{PROJECT_ADDRESS}}", clientContractDetails.ClientAddress);
			htmlContent = htmlContent.Replace("{{EST_STARTDATE}}", clientContractDetails.EstStartDate);
			htmlContent = htmlContent.Replace("{{EST_COMPLETIONDATE}}", clientContractDetails.EstCompletionDate);
			htmlContent = htmlContent.Replace("{{GENCONTRACTORSFEEPERCENTAGE}}", clientContractDetails.GenContractorsFeePercentage.ToString());
			htmlContent = htmlContent.Replace("{{PROJECTCOST}}", StringUtilities.FormatWithCommas(clientContractDetails.ProjectCost));
			htmlContent = htmlContent.Replace("{{GENCONTRACTORSFEEAMOUNT}}", StringUtilities.FormatWithCommas(clientContractDetails.GenContractorsFeeAmount));
			htmlContent = htmlContent.Replace("{{PROJECTCOSTTEXT}}", StringUtilities.NumberToWords(clientContractDetails.ProjectCost));
			htmlContent = htmlContent.Replace("{{INITIALDEPOSITPERCENTAGE}}", clientContractDetails.InitialDepositPercentage.ToString());
			htmlContent = htmlContent.Replace("{{CONTRACTOR}}", clientContractDetails.Contractor);
			htmlContent = htmlContent.Replace("{{CLIENTCITY}}", clientContractDetails.ClientCity);
			htmlContent = htmlContent.Replace("{{CLIENTSTATE}}", clientContractDetails.ClientState);

			return GeneratePDFByteArray(htmlContent);
		}

		public async Task<Stream> ConvertHtmlToPdfFromUrl(string url, Orientation orientation = Orientation.Portrait)
		{
			//using var httpClient = new HttpClient();

			////// Fetch the HTML content from the URL
			//var htmlContent = await httpClient.GetStringAsync(url);

			//var pdfBytes = GeneratePDFByteArray(htmlContent);
			//return new MemoryStream(pdfBytes);

			//var pdf = Pdf.From(htmlContent).Content();

			//return new MemoryStream(pdf);

			return await Task.Run(() =>
			{
				var document = new HtmlToPdfDocument
				{
					GlobalSettings = new GlobalSettings
					{
						ColorMode = ColorMode.Color,
						Orientation = orientation,
						PaperSize = PaperKind.Letter,
					}
				};

				var objectSettings = new ObjectSettings
				{
					Page = url
				};

				document.Objects.Add(objectSettings);

				var pdf = this.pdfConverter.Convert(document);

				return new MemoryStream(pdf);
			});
		}
		#endregion

		#region Private

		private byte[] GeneratePDFByteArray(string htmlContent)
		{
			var globalSettings = new GlobalSettings
			{
				ColorMode = ColorMode.Color,
				Orientation = Orientation.Portrait,
				PaperSize = PaperKind.Legal
			};

			var objectSettings = new ObjectSettings
			{
				PagesCount = true,
				HtmlContent = htmlContent,
				WebSettings = { DefaultEncoding = "utf-8" },
				FooterSettings = { Center = "Page [page] of [toPage]", FontSize = 9 }
			};

			var pdfDocument = new HtmlToPdfDocument
			{
				GlobalSettings = globalSettings,
				Objects = { objectSettings }
			};

			return pdfConverter.Convert(pdfDocument);
		}

		private string GetTemplateContent(string htmlFilePath, string? styleSheetFilePath = null)
		{
			if (!File.Exists(htmlFilePath))
			{
				throw new Exception($"The file {htmlFilePath} was not found.");
			}

			var htmlContent = File.ReadAllText(htmlFilePath);

			if (!string.IsNullOrEmpty(styleSheetFilePath) && File.Exists(styleSheetFilePath))
			{
				var stylesheetContent = File.ReadAllText(styleSheetFilePath);
				htmlContent = htmlContent.Replace("{{stylesheet}}", stylesheetContent);
			}

			return htmlContent;
		}

		#endregion

	}
}
