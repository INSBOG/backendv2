namespace WhotSiv.Validator;

public class DataValidator : IValidator
{
    public bool Validate(object input)
    {
        Console.WriteLine("Validating data...");
        return true;
    }
}