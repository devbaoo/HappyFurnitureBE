using HappyFurnitureBE.Application.DTOs.Contact;
using HappyFurnitureBE.Application.Interfaces;
using HappyFurnitureBE.Domain.Interfaces;
using HappyFurnitureBE.Domain.Entities;

namespace HappyFurnitureBE.Application.Services.Contact;

public class ContactService : IContactService
{
    private readonly IContactRepository _contactRepository;
    private readonly IRecaptchaService _recaptchaService;

    public ContactService(IContactRepository contactRepository, IRecaptchaService recaptchaService)
    {
        _contactRepository = contactRepository;
        _recaptchaService = recaptchaService;
    }

    public async Task<ContactResponse> CreateContactAsync(CreateContactRequest request)
    {
        var isValid = await _recaptchaService.VerifyTokenAsync(request.RecaptchaToken);
        if (!isValid)
            throw new InvalidOperationException("reCAPTCHA verification failed.");

        var contact = new Domain.Entities.Contact
        {
            Name = request.Name,
            Email = request.Email,
            Subject = request.Subject,
            Message = request.Message,
            PhoneNumber = request.PhoneNumber,
            Address = request.Address,
            IsRead = false
        };

        var created = await _contactRepository.AddAsync(contact);
        return MapToResponse(created);
    }

    public async Task<IEnumerable<ContactResponse>> GetAllContactsAsync(bool? isRead)
    {
        var contacts = await _contactRepository.GetAllWithFilterAsync(isRead);
        return contacts.Select(MapToResponse);
    }

    public async Task<ContactResponse?> GetContactByIdAsync(int id)
    {
        var contact = await _contactRepository.GetByIdAsync(id);
        return contact == null ? null : MapToResponse(contact);
    }

    public async Task MarkAsReadAsync(int id)
    {
        var contact = await _contactRepository.GetByIdAsync(id);
        if (contact == null)
            throw new KeyNotFoundException($"Contact with id {id} not found.");

        contact.IsRead = true;
        await _contactRepository.UpdateAsync(contact);
    }

    public async Task DeleteContactAsync(int id)
    {
        var contact = await _contactRepository.GetByIdAsync(id);
        if (contact == null)
            throw new KeyNotFoundException($"Contact with id {id} not found.");

        await _contactRepository.DeleteAsync(id);
    }

    private static ContactResponse MapToResponse(Domain.Entities.Contact contact) => new()
    {
        Id = contact.Id,
        Name = contact.Name,
        Email = contact.Email,
        Subject = contact.Subject,
        Message = contact.Message,
        PhoneNumber = contact.PhoneNumber,
        Address = contact.Address,
        IsRead = contact.IsRead,
        CreatedAt = contact.CreatedAt
    };
}
