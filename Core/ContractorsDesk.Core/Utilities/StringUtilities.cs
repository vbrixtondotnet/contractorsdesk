using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Utilities
{
	public static class StringUtilities
	{
		public static string NumberToWords(decimal num)
		{
			if (num == 0) return "Zero";

			string[] ones = { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };
			string[] teens = { "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
			string[] tens = { "", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
			string[] thousands = { "", "Thousand", "Million", "Billion", "Trillion" };

			string words = "";

			int chunkIndex = 0;

			while (num > 0)
			{
				int chunk = (int)(num % 1000);
				if (chunk > 0)
				{
					words = ConvertChunk(chunk, ones, teens, tens) + " " + thousands[chunkIndex] + " " + words;
				}
				num /= 1000;
				chunkIndex++;
			}

			return words.Trim();
		}
		public static string FormatWithCommas(decimal? number)
		{
			if (number == null || number == 0) return "0.00";

			return string.Format("{0:N0}", number);
		}
		private static string ConvertChunk(int num, string[] ones, string[] teens, string[] tens)
		{
			string chunkWords = "";

			if (num >= 100)
			{
				chunkWords += ones[num / 100] + " Hundred ";
				num %= 100;
			}

			if (num >= 11 && num <= 19)
			{
				chunkWords += teens[num - 11] + " ";
			}
			else
			{
				if (num >= 10)
				{
					chunkWords += tens[num / 10] + " ";
					num %= 10;
				}
				if (num >= 1 && num <= 9)
				{
					chunkWords += ones[num] + " ";
				}
			}

			return chunkWords.Trim();
		}
	}
}
