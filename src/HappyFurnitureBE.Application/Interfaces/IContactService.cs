using HappyFurnitureBE.Application.DTOs.Contact;

namespace HappyFurnitureBE.Application.Interfaces;

public interface IContactService
{
    Task<ContactResponse> CreateContactAsync(CreateContactRequest request);
    Task<IEnumerable<ContactResponse>> GetAllContactsAsync(bool? isRead);
    Task<ContactResponse?> GetContactByIdAsync(int id);
    Task MarkAsReadAsync(int id);
    Task DeleteContactAsync(int id);
}
