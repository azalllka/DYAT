using DYAT.Domain.Entities;

namespace DYAT.Application.Interfaces.Repositories;

public interface INftRepository
{
    Task<IEnumerable<Nft>> GetAllNftsAsync();
    Task<Nft?> GetNftByIdAsync(int id);
    Task<IEnumerable<Nft>> GetNftsByCreatorAsync(string creatorId);
    Task<IEnumerable<Nft>> GetNftsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task<IEnumerable<Nft>> GetPopularNftsAsync(int count = 10);
    Task<IEnumerable<Nft>> GetNewNftsAsync(int count = 10);
    Task<Nft> CreateNftAsync(Nft nft);
    Task<Nft> UpdateNftAsync(Nft nft);
    Task<bool> DeleteNftAsync(int id);
    Task<bool> UpdateNftPriceAsync(int id, decimal newPrice);
    Task<bool> UpdateNftRatingAsync(int id, int newRating);
} 