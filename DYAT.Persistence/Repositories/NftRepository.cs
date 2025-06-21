using Microsoft.EntityFrameworkCore;
using DYAT.Application.Interfaces.Repositories;
using DYAT.Domain.Entities;
using DYAT.Persistence.Context;

namespace DYAT.Persistence.Repositories
{
    public class NftRepository : INftRepository
    {
        private readonly ApplicationDbContext _context;

        public NftRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Nft>> GetAllNftsAsync()
        {
            return await _context.Nfts
                .Include(n => n.Creator)
                .Where(n => n.IsActive)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<Nft?> GetNftByIdAsync(int id)
        {
            return await _context.Nfts
                .Include(n => n.Creator)
                .FirstOrDefaultAsync(n => n.Id == id && n.IsActive);
        }

        public async Task<IEnumerable<Nft>> GetNftsByCreatorAsync(string creatorId)
        {
            return await _context.Nfts
                .Include(n => n.Creator)
                .Where(n => n.CreatedBy == creatorId && n.IsActive)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Nft>> GetNftsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _context.Nfts
                .Include(n => n.Creator)
                .Where(n => n.Price >= minPrice && n.Price <= maxPrice && n.IsActive)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Nft>> GetPopularNftsAsync(int count = 10)
        {
            // Здесь можно добавить логику определения популярных NFT
            // Например, по количеству просмотров или ставок
            return await _context.Nfts
                .Include(n => n.Creator)
                .Where(n => n.IsActive)
                .OrderByDescending(n => n.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Nft>> GetNewNftsAsync(int count = 10)
        {
            return await _context.Nfts
                .Include(n => n.Creator)
                .Where(n => n.IsActive)
                .OrderByDescending(n => n.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Nft> CreateNftAsync(Nft nft)
        {
            _context.Nfts.Add(nft);
            await _context.SaveChangesAsync();
            return nft;
        }

        public async Task<Nft> UpdateNftAsync(Nft nft)
        {
            _context.Entry(nft).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return nft;
        }

        public async Task<bool> DeleteNftAsync(int id)
        {
            var nft = await _context.Nfts.FindAsync(id);
            if (nft == null)
                return false;

            nft.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateNftPriceAsync(int id, decimal newPrice)
        {
            var nft = await _context.Nfts.FindAsync(id);
            if (nft == null)
                return false;

            nft.Price = newPrice;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateNftRatingAsync(int id, int newRating)
        {
            // Здесь можно добавить логику обновления рейтинга
            // Например, если у вас есть отдельная таблица для рейтингов
            return await Task.FromResult(true);
        }
    }
} 