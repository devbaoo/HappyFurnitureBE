namespace HappyFurnitureBE.Application.Interfaces;

public interface IRecaptchaService
{
    Task<bool> VerifyTokenAsync(string token);
}
