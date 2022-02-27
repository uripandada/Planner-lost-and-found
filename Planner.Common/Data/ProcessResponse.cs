using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planner.Common.Data
{
	public class ProcessResponse
	{
		public bool IsSuccess { get; set; }
		public bool HasError { get; set; }
		public string Message { get; set; }

		public List<ErrorData> ModelErrors { get; set; } = new List<ErrorData>();
		public void AddError(string key, string validatorKey)
		{
			var lowercaseKey = key.IsNull() ? "" : char.ToLower(key[0]) + key.Substring(1);
			var error = this.ModelErrors.FirstOrDefault(e => e.Key == lowercaseKey);
			if (error == null)
			{
				error = new ErrorData() { Key = lowercaseKey, ValidatorKey = validatorKey };
				this.ModelErrors.Add(error);
			}
			else
			{
				error.ValidatorKey += $", { validatorKey}";
			}

			this.IsSuccess = false;
			this.HasError = true;
		}
	}
	public class ErrorData
	{
		public string Key { get; set; }
		public string ValidatorKey { get; set; }
	}

	public class ProcessResponse<TResponse> : ProcessResponse
	{
		public TResponse Data { get; set; }
	}

	public class DeleteProcessResponse: ProcessResponse
	{
		public bool HasWarning { get; set; }
	}
	public class SimpleProcessResponse
	{
		public bool Success { get; set; }
	}

}
