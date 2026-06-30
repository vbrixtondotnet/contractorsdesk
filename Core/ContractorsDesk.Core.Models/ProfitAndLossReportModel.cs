using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace ContractorsDesk.Core.Models
{
	using System;
	using System.Collections.Generic;
	using Newtonsoft.Json; // Or System.Text.Json

	// --- Top-level container ---
	public class ProfitAndLossReportModel
	{
		[JsonProperty("Header")]
		public ReportHeader Header { get; set; }

		[JsonProperty("Columns")]
		public ReportColumns Columns { get; set; }

		[JsonProperty("Rows")]
		public RowsContainer Rows { get; set; } // Container for the list of top-level rows
	}

	// --- Header Structure ---
	public class ReportHeader
	{
		// Using DateTime for Time and Date properties (Json.NET handles ISO 8601)
		[JsonProperty("Time")]
		public DateTime Time { get; set; }

		[JsonProperty("ReportName")]
		public string ReportName { get; set; }

		[JsonProperty("DateMacro")]
		public string DateMacro { get; set; }

		[JsonProperty("ReportBasis")]
		public string ReportBasis { get; set; }

		[JsonProperty("StartPeriod")]
		public DateTime StartPeriod { get; set; }

		[JsonProperty("EndPeriod")]
		public DateTime EndPeriod { get; set; }

		[JsonProperty("SummarizeColumnsBy")]
		public string SummarizeColumnsBy { get; set; }

		[JsonProperty("Currency")]
		public string Currency { get; set; }

		// 'Class' seems to be a string of comma-separated values
		[JsonProperty("Class")]
		public string Class { get; set; }

		[JsonProperty("Option")]
		public List<HeaderOption> Option { get; set; }
	}

	public class HeaderOption
	{
		[JsonProperty("Name")]
		public string Name { get; set; }

		[JsonProperty("Value")]
		public string Value { get; set; }
	}

	// --- Columns Structure ---
	public class ReportColumns
	{
		[JsonProperty("Column")]
		public List<ColumnDefinition> Column { get; set; }
	}

	public class ColumnDefinition
	{
		[JsonProperty("ColTitle")]
		public string ColTitle { get; set; }

		[JsonProperty("ColType")]
		public string ColType { get; set; }

		[JsonProperty("MetaData")]
		public List<ColumnMetaData> MetaData { get; set; }
	}

	public class ColumnMetaData
	{
		[JsonProperty("Name")]
		public string Name { get; set; }

		[JsonProperty("Value")]
		public string Value { get; set; }
	}

	// --- Rows Structure (Recursive) ---

	// Container for the array of Row objects within the "Rows" property
	public class RowsContainer
	{
		[JsonProperty("Row")]
		public List<Row> Row { get; set; } // This list contains the actual row objects
	}

	// Represents a single row object. This class must accommodate all possible properties
	// found in any type of row ("Data", "Section").
	public class Row
	{
		// Properties for "Section" type rows:
		// Recursive: This is the list of nested rows within this section
		[JsonProperty("Rows")]
		public RowsContainer Rows { get; set; }

		// Header for a section
		[JsonProperty("Header")]
		public RowHeader Header { get; set; }

		// Summary for a section or a standalone summary row
		[JsonProperty("Summary")]
		public RowSummary Summary { get; set; }

		// Properties for "Data" type rows:
		// The actual cell data for a data row
		[JsonProperty("ColData")]
		public List<ColDataObject> ColData { get; set; }

		// Shared properties:
		// Indicates the type of row ("Data", "Section")
		[JsonProperty("type")] // JSON property name is lowercase
		public string Type { get; set; }

		// Group identifier for a section (optional)
		[JsonProperty("group")] // JSON property name is lowercase
		public string Group { get; set; }

		// ID for data rows (optional, seems tied to the first ColData item)
		[JsonProperty("id")] // JSON property name is lowercase
		public string Id { get; set; }
	}

	// Structure for the Header within a Row object
	public class RowHeader
	{
		// ColData is the array of values in the header row
		[JsonProperty("ColData")]
		public List<ColDataObject> ColData { get; set; }

		// Note: A RowHeader sometimes has an 'id' in the JSON based on your data.
		// However, looking at the JSON, the 'id' seems to be within the *first* ColDataObject
		// inside this ColData list for the header, not on the Header object itself.
		// So we don't need an 'id' property directly on RowHeader.
	}

	// Structure for the Summary within a Row object
	public class RowSummary
	{
		// ColData is the array of values in the summary row
		[JsonProperty("ColData")]
		public List<ColDataObject> ColData { get; set; }
	}

	// Represents an object within the 'ColData' arrays found in Rows, RowHeader, or RowSummary
	public class ColDataObject
	{
		// The main value (string, number, etc.)
		[JsonProperty("value")] // JSON property name is lowercase
		public object Value { get; set; } // Use 'object' as it can be string or number

		// The optional ID, only present on the first item of ColData for Data rows
		[JsonProperty("id")] // JSON property name is lowercase
		public string Id { get; set; }
	}
}
