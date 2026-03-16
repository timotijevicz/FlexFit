using MediatR;

namespace FlexFit.Application.Handlers
{
    // Komanda: Šta šaljemo sistemu
    public record ProcessEntryCommand(string ScannedCardNumber, int FitnessObjectId) : IRequest<EntryResponse>;

    // Odgovor: Šta sistem vraća kontroleru
    public record EntryResponse(bool IsSuccess, string Message);
}