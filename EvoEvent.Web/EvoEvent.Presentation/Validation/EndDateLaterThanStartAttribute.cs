using System.ComponentModel.DataAnnotations;

namespace EvoEvent.Web.Validation
{
	public class EndDateLaterThanStartAttribute : ValidationAttribute
	{
		protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
		{
			var startAtProp = validationContext.ObjectType.GetProperty("StartAt");
			var endAtProp = validationContext.ObjectType.GetProperty("EndAt");

			if (startAtProp is null || endAtProp is null)
				return ValidationResult.Success;

			var startAt = startAtProp.GetValue(validationContext.ObjectInstance) as DateTime?;
			var endAt = endAtProp.GetValue(validationContext.ObjectInstance) as DateTime?;

			if (!startAt.HasValue || !endAt.HasValue)
				return ValidationResult.Success;

			if (endAt.Value <= startAt.Value)
			{
				return new ValidationResult("Дата окончания должна быть позже Даты начала");
			}

			return ValidationResult.Success;
		}
	}
}
