using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.WebPortal.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using System.Text;

namespace ContractorsDesk.WebPortal.Helpers
{
	public static class HtmlHelperExtensions
	{
		private static IServiceProvider _serviceProvider;
		public static void Configure(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}
		private static string GetLatestVersion() => Guid.NewGuid().ToString();
		public static IHtmlContent RenderViewScripts(this IHtmlHelper htmlHelper)
		{
			var routeValues = htmlHelper.ViewContext.RouteData.Values.ToList();

			var controller = routeValues.FirstOrDefault(r => r.Key == "controller").Value;
			var view = routeValues.FirstOrDefault(r => r.Key == "action").Value;

			StringBuilder scriptTagBuilder = new();

			var viewJs = $"/Views/{controller}/{view}/{view}.view.js";
			var serviceJs = $"/Views/{controller}/{view}/services/{view}.service.js";
			var modelJs = $"/Views/{controller}/{view}/models/{view}.model.js";

			//if (!File.Exists(viewJs)) throw new ArgumentException("View script does not exist!", $"{view}.view.js");
			//if (!File.Exists(serviceJs)) throw new ArgumentException("View script does not exist!", $"{view}.service.js");
			//if (!File.Exists(viewJs)) throw new ArgumentException("View script does not exist!", $"{view}.model.js");

			// Generate the script tag
			scriptTagBuilder.AppendLine($"<script src=\"{htmlHelper.Encode(viewJs)}?v={GetLatestVersion()}\" defer></script>");
			scriptTagBuilder.AppendLine($"<script src=\"{htmlHelper.Encode(serviceJs)}?v={GetLatestVersion()}\" defer></script>");
			scriptTagBuilder.AppendLine($"<script src=\"{htmlHelper.Encode(modelJs)}?v={GetLatestVersion()}\" defer></script>");


			// Render Sub-Components Scripts
			var basePath = Directory.GetCurrentDirectory(); // root of your project
			var folderPath = Path.Combine(basePath, "Views", controller.ToString(), view.ToString(), "components");

			var subComponentScripts = GetSubComponentsJs(folderPath);
			if (subComponentScripts != null)
			{
				foreach (var script in subComponentScripts)
				{
					var scriptPath = $"/Views/{controller}/{view}/components/{script}";
					scriptTagBuilder.AppendLine($"<script src=\"{htmlHelper.Encode(scriptPath)}?v={GetLatestVersion()}\" defer></script>");
				}
			}

			// Return an HtmlString to render the script tag without encoding
			return new HtmlString(scriptTagBuilder.ToString());

		}
		public static IHtmlContent RenderViewStyles(this IHtmlHelper htmlHelper)
		{
			//"<link rel=\"stylesheet\" href=\"{htmlHelper.Encode(Path.Combine(directoryPath, fileInfo.Name))}\" />
			var routeValues = htmlHelper.ViewContext.RouteData.Values.ToList();

			var controller = routeValues.FirstOrDefault(r => r.Key == "controller").Value;
			var view = routeValues.FirstOrDefault(r => r.Key == "action").Value;

			StringBuilder scriptTagBuilder = new();

			var viewCss = $"/Views/{controller}/{view}/{view}.view.css";

			//if (!File.Exists(viewJs)) throw new ArgumentException("View script does not exist!", $"{view}.view.js");
			//if (!File.Exists(serviceJs)) throw new ArgumentException("View script does not exist!", $"{view}.service.js");
			//if (!File.Exists(viewJs)) throw new ArgumentException("View script does not exist!", $"{view}.model.js");

			// Generate the script tag
			//
			scriptTagBuilder.AppendLine($"<link rel=\"stylesheet\" href=\"/css/site.css?v={GetLatestVersion()}\"/>");
			scriptTagBuilder.AppendLine($"<link rel=\"stylesheet\" href=\"/css/viewports.css?v={GetLatestVersion()}\"/>");
			scriptTagBuilder.AppendLine($"<link rel=\"stylesheet\" href=\"{htmlHelper.Encode(viewCss)}?v={GetLatestVersion()}\"/>");

			// Render Sub-Components Styles
			var basePath = Directory.GetCurrentDirectory(); // root of your project
			var folderPath = Path.Combine(basePath, "Views", controller.ToString(), view.ToString(), "components");

			var subComponentCss = GetSubComponentsCss(folderPath);
			if (subComponentCss != null)
			{
				foreach (var css in subComponentCss)
				{
					var cssPath = $"/Views/{controller}/{view}/components/{css}";
					scriptTagBuilder.AppendLine($"<link rel=\"stylesheet\" href=\"{htmlHelper.Encode(cssPath)}?v={GetLatestVersion()}\"/>");
				}
			}

			// Return an HtmlString to render the script tag without encoding
			return new HtmlString(scriptTagBuilder.ToString());
		}
		public static IHtmlContent LoadDependencies(this IHtmlHelper htmlHelper, string[] paths)
		{
			///Views/Home/dashboard/models/dashboard.model.js
			var routeValues = htmlHelper.ViewContext.RouteData.Values.ToList();

			var controller = routeValues.FirstOrDefault(r => r.Key == "controller").Value;
			var view = routeValues.FirstOrDefault(r => r.Key == "action").Value;

			StringBuilder scriptTagBuilder = new StringBuilder();
			foreach(var path in paths)
			{
				if (string.IsNullOrWhiteSpace(path))
				{
					throw new ArgumentException("The script path cannot be null or whitespace.", nameof(path));
				}

				var scriptPath = $"/Views/{controller}/{view}/{path}";
				// Generate the script tag
				scriptTagBuilder.AppendLine($"<script src=\"{htmlHelper.Encode(scriptPath)}?v={GetLatestVersion()}\" defer></script>");
			}
			
			// Return an HtmlString to render the script tag without encoding
			return new HtmlString(scriptTagBuilder.ToString());
		}
		public static IHtmlContent LoadComponents(this IHtmlHelper htmlHelper, string[] components)
		{
			////Components/table/table.js?v=1.001
			var routeValues = htmlHelper.ViewContext.RouteData.Values.ToList();

			var controller = routeValues.FirstOrDefault(r => r.Key == "controller").Value;
			var view = routeValues.FirstOrDefault(r => r.Key == "action").Value;

			StringBuilder scriptTagBuilder = new StringBuilder();
			foreach (var component in components)
			{
				if (string.IsNullOrWhiteSpace(component))
				{
					throw new ArgumentException("Component cannot be null or whitespace.", nameof(component));
				}

				var componentPath = $"/Components/{component}/{component}.js";
				var componentStylePath = $"/Components/{component}/{component}.css";
				// Generate the script tag
				scriptTagBuilder.AppendLine($"<script src=\"{htmlHelper.Encode(componentPath)}?v={GetLatestVersion()}\" defer></script>");

				// Resolve IWebHostEnvironment from the service provider
				var webHostEnvironment = _serviceProvider.GetService<IWebHostEnvironment>();
				var physicalPath = Path.Combine(webHostEnvironment.ContentRootPath, $"Components/{component}/{component}.css");
				bool cssExists = File.Exists(physicalPath);
				if (cssExists)
				{
					scriptTagBuilder.AppendLine($"<link rel='stylesheet' href='{htmlHelper.Encode(componentStylePath)}?v=${GetLatestVersion()}' />");
				}

			}

			// Return an HtmlString to render the script tag without encoding
			return new HtmlString(scriptTagBuilder.ToString());
		}
		public static IHtmlContent RenderSharedComponentStyle(this IHtmlHelper htmlHelper, string sharedComponentName, string fileName)
		{
			var fullPath = $"/Views/Shared/Components/{sharedComponentName}/css/{fileName}";
			var linkTag = $"<link rel='stylesheet' href='{htmlHelper.Encode(fullPath)}?v=${GetLatestVersion()}' />";
			return new HtmlString(linkTag);
		}
		public static IHtmlContent InputGroupTextbox(this IHtmlHelper htmlHelper, InputGroupModel model)
		{
			var inputGroup = $@"<div class=""input-group input-group-sm mb-2 {model.CssClass}"">
									<span class=""input-group-text {model.IsRequired} {model.LabelCssClass}"">{model.Label}</span>
									<input type=""{model.Type}"" 
											class=""form-control {model.CustomInputClass}"" 
											autocomplete=""off""
											{model.GetId()}
											{model.GetName()}
											{model.GetDataType()}
											{model.GetDataModel()}  
											{model.GetBookmarkTitle()}
											{model.GetEvtInput()}
											{model.IsRequired}	
											{model.IsDisabled}
											{model.IsReadOnly}			
											>
								</div>";

			return new HtmlString(inputGroup);
		}
		public static IHtmlContent InputGroupDropdown(this IHtmlHelper htmlHelper, InputGroupModel model)
		{
			var isSelect2 = model.IsSelect2 ? "data-control=\"select2\" data-placeholder=\"Select an option\"" : string.Empty;
			var inputGroup = $@"<div class=""input-group input-group-sm mb-2 {model.CssClass}"">
									<span class=""input-group-text {model.IsRequired} {model.LabelCssClass}"">{model.Label}</span>
									<select class=""form-select {model.CustomInputClass}"" 
											{isSelect2}
											{model.GetId()} 
											{model.GetName()}
											{model.GetDataModel()}
											{model.GetEvtInput()}
											{model.GetEvtChange()}
											{model.IsReadOnly}
											{model.IsRequired}
											{model.IsMultiple}
											{model.IsDisabled}>";	
			
			foreach (var option in model.DropdownOptions)
			{
				var disabled = option.Disabled ? "disabled" : "";
				inputGroup += $@"<option value=""{option.Value}"" {(option.Selected ? "selected" : "")} {disabled}>{option.Text}</option>";
			}

			inputGroup += $@"</select>
								</div>";

			return new HtmlString(inputGroup);
		}
		public static IHtmlContent InputGroupTextArea(this IHtmlHelper htmlHelper, InputGroupModel model)
		{
			var inputGroup = $@"<div class=""input-group input-group-sm mb-2 {model.CssClass}"">
									<span class=""input-group-text {model.IsRequired} {model.LabelCssClass}"" style=""display:block;text-align:left;"">{model.Label}</span>
									<textarea class=""form-control {model.CustomInputClass}"" 
											{model.GetId()} 
											{model.GetName()}
											{model.GetDataModel()}
											{model.GetEvtInput()}
											{model.GetEvtChange()}
											{model.IsReadOnly}
											{model.IsRequired}
											{model.IsMultiple}
											{model.IsDisabled}
											rows=""{model.TextAreaRows}""
											style=""resize:none;""
											></textarea>
								</div>";

			return new HtmlString(inputGroup);
		}
		public static IHtmlContent StatesDropdown(this IHtmlHelper htmlHelper, HtmlElementModel model)
		{
			var required = model.Required ? "required" : "";
			var inputGroup = $@"
								<div class=""input-group input-group-sm mb-2"">
									<span class=""input-group-text mn-w-125px {model.LabelCssClass}"" id=""basic-addon1"">State</span>
									<div class=""flex-grow-1"">
										<select class=""form-select {model.Classes}"" data-model=""{model.DataModel}"" data-control=""select2"" data-placeholder=""Select a State"" id=""{model.Id}"">
											<option value="""">&nbsp;</option>
											<option value=""AL"">Alabama (AL)</option>
											<option value=""AK"">Alaska (AK)</option>
											<option value=""AZ"">Arizona (AZ)</option>
											<option value=""AR"">Arkansas (AR)</option>
											<option value=""CA"">California (CA)</option>
											<option value=""CO"">Colorado (CO)</option>
											<option value=""CT"">Connecticut (CT)</option>
											<option value=""DE"">Delaware (DE)</option>
											<option value=""FL"">Florida (FL)</option>
											<option value=""GA"">Georgia (GA)</option>
											<option value=""HI"">Hawaii (HI)</option>
											<option value=""ID"">Idaho (ID)</option>
											<option value=""IL"">Illinois (IL)</option>
											<option value=""IN"">Indiana (IN)</option>
											<option value=""IA"">Iowa (IA)</option>
											<option value=""KS"">Kansas (KS)</option>
											<option value=""KY"">Kentucky (KY)</option>
											<option value=""LA"">Louisiana (LA)</option>
											<option value=""ME"">Maine (ME)</option>
											<option value=""MD"">Maryland (MD)</option>
											<option value=""MA"">Massachusetts (MA)</option>
											<option value=""MI"">Michigan (MI)</option>
											<option value=""MN"">Minnesota (MN)</option>
											<option value=""MS"">Mississippi (MS)</option>
											<option value=""MO"">Missouri (MO)</option>
											<option value=""MT"">Montana (MT)</option>
											<option value=""NE"">Nebraska (NE)</option>
											<option value=""NV"">Nevada (NV)</option>
											<option value=""NH"">New Hampshire (NH)</option>
											<option value=""NJ"">New Jersey (NJ)</option>
											<option value=""NM"">New Mexico (NM)</option>
											<option value=""NY"">New York (NY)</option>
											<option value=""NC"">North Carolina (NC)</option>
											<option value=""ND"">North Dakota (ND)</option>
											<option value=""OH"">Ohio (OH)</option>
											<option value=""OK"">Oklahoma (OK)</option>
											<option value=""OR"">Oregon (OR)</option>
											<option value=""PA"">Pennsylvania (PA)</option>
											<option value=""RI"">Rhode Island (RI)</option>
											<option value=""SC"">South Carolina (SC)</option>
											<option value=""SD"">South Dakota (SD)</option>
											<option value=""TN"">Tennessee (TN)</option>
											<option value=""TX"">Texas (TX)</option>
											<option value=""UT"">Utah (UT)</option>
											<option value=""VT"">Vermont (VT)</option>
											<option value=""VA"">Virginia (VA)</option>
											<option value=""WA"">Washington (WA)</option>
											<option value=""WV"">West Virginia (WV)</option>
											<option value=""WI"">Wisconsin (WI)</option>
											<option value=""WY"">Wyoming (WY)</option>
										</select>
									</div>
								</div>";

			return new HtmlString(inputGroup);
		}
		public static IHtmlContent FormatAsMoney(this IHtmlHelper htmlHelper, decimal? value, int decimalPlaces = 2, string cultureCode = "en-US")
		{
			decimal decimalValue = value ?? 0;
			var roundedValue = Math.Round(decimalValue, decimalPlaces);
			var format = string.Format("0:C{0}",decimalPlaces);
				format = "{" + format + "}";
			// Format the value as currency based on the specified culture
			var formattedValue = string.Format(CultureInfo.CreateSpecificCulture(cultureCode), format, roundedValue);

			// Return the formatted value as an HTML string
			return new HtmlString(formattedValue);
		}

		private static IEnumerable<string>? GetSubComponentsJs(string absoluteFolderPath)
		{
			if (!Directory.Exists( absoluteFolderPath))
				return Enumerable.Empty<string>();

			return Directory.GetFiles(absoluteFolderPath, "*.js", SearchOption.AllDirectories)
				   .Select(path => Path.GetRelativePath(absoluteFolderPath, path)
									   .Replace("\\", "/")); // Normalize slashes

		}
		private static IEnumerable<string>? GetSubComponentsCss(string absoluteFolderPath)
		{
			if (!Directory.Exists(absoluteFolderPath))
				return Enumerable.Empty<string>();

			return Directory.GetFiles(absoluteFolderPath, "*.css", SearchOption.AllDirectories)
				   .Select(path => Path.GetRelativePath(absoluteFolderPath, path)
									   .Replace("\\", "/")); // Normalize slashes

		}


	}
}
