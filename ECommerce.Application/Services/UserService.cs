using ECommerce.Application.DTOs.Order;
using ECommerce.Application.DTOs.Product;
using ECommerce.Application.DTOs.User;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace ECommerce.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        // --- MAPPING HELPER ---
        private UserDTO MapToDto(UserEntity user)
        {
            return new UserDTO
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Address = user.Address,
                Enabled = user.Enabled,
                IsSellerApproved = user.IsSellerApproved,
                HasPendingSellerRequest = user.HasPendingSellerRequest,
                IsRejected = user.IsRejected,
                RoleId = user.RoleId,
                RoleName = user.Role?.Name,
                CreatedAt = user.CreatedAt,

                // --- SİPARİŞLERİ MAPLE ---
                Orders = user.Orders?.Select(o => new OrderDTO
                {
                    Id = o.Id,
                    OrderCode = o.OrderCode,
                    CreatedAt = o.CreatedAt,
                    TotalPrice = o.TotalPrice,
                    PaymentMethod = o.PaymentMethod,
                    // Teslimat bilgilerini Entity'den DTO'ya aktar
                    DeliveryAddress = o.DeliveryAddress,
                    DeliveryFullName = o.DeliveryFullName,
                    DeliveryPhone = o.DeliveryPhone,
                    UserId = o.UserId,
                    UserFullName = $"{user.FirstName} {user.LastName}",

                    Items = o.OrderItems?.Select(oi => new OrderItemDTO
                    {
                        ProductId = oi.ProductId, // oi.Id değil, ProductId olmalı
                        ProductName = oi.Product?.Name ?? "Unknown Product",
                        UnitPrice = oi.UnitPrice,
                        Quantity = oi.Quantity,
                        // Görsel bilgisini al (Eğer Product Include edilmişse)
                        ImageUrl = oi.Product?.Images?.FirstOrDefault(x => x.IsMain)?.Url
                                   ?? oi.Product?.Images?.FirstOrDefault()?.Url
                                   ?? "/img/product/default.jpg"
                    }).ToList() ?? new List<OrderItemDTO>()
                }).ToList() ?? new List<OrderDTO>(),

                // --- ÜRÜNLERİ MAPLE (Burada 'Items' değil 'Products' olmalı) ---
                Products = user.Products?.Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    OldPrice = p.OldPrice,
                    StockAmount = p.StockAmount,
                    Enabled = p.Enabled,
                    IsFeatured = p.IsFeatured,
                    MainImageUrl = p.Images?.FirstOrDefault(x => x.IsMain)?.Url
                                   ?? p.Images?.FirstOrDefault()?.Url
                                   ?? "/img/product/default.jpg"
                }).ToList() ?? new List<ProductDTO>()
            };
            }

        // Bütün kullanıcıları al
        public async Task<IEnumerable<UserDTO>> GetAllAsync()
        {
            var users = await _userRepository.GetAllWithRolesAsync();
            return users.Select(MapToDto);
        }

        // ID ile kullanıcıyı al
        public async Task<UserDTO?> GetAsync(int id, bool includeOrders = false, bool includeProducts = false)
        {
            var user = await _userRepository.GetByIdWithIncludesAsync(id, includeOrders, includeProducts);
            return user != null ? MapToDto(user) : null;
        }

        // Kullanıcıyı ekle 
        //Aslında şu an registerla kayıt oluyor ama ileride admine ekleme yetkisi verilirse kullanılabilir
        //Aksi halde register'a da yönlendirilebilir.
        public async Task AddAsync(UserDTO dto)
        {
            var user = new UserEntity
            {
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Password = dto.Password,
                RoleId = dto.RoleId, // Admin hangi rolü seçtiyse o (Örn: Seller)
                Enabled = dto.Enabled,
                CreatedAt = DateTime.Now,
                Phone = dto.Phone,
                Address = dto.Address
            };
            await _userRepository.AddAsync(user);
            await _userRepository.SaveAsync();
        }

        // Kullanıcıyı güncelle 
        public async Task UpdateAsync(UserDTO dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.Id);
            if (user != null)
            {
                user.FirstName = dto.FirstName;
                user.LastName = dto.LastName;
                user.Phone = dto.Phone;
                user.Address = dto.Address;
                
                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveAsync();
            }
        }

        // Kullanıcıyı sil 
        public async Task DeleteAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user != null)
            {
                await _userRepository.DeleteAsync(user);
                await _userRepository.SaveAsync();
            }
        }

        // Seller onayla
        public async Task ApproveSellerAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user != null)
            {
                user.IsSellerApproved = true;
                user.HasPendingSellerRequest = false;

                var sellerRole = await _roleRepository.GetByNameAsync("Seller");
                if (sellerRole != null)
                {
                    user.RoleId = sellerRole.Id;
                }

                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveAsync();
            }
        }


        // Email ile kullanıcı getirme (async)
        public async Task<UserDTO?> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user != null ? MapToDto(user) : null;
        }


        // Kullanıcı kaydetme (async)
        public async Task<UserDTO> RegisterAsync(UserDTO dto)
        {
            var user = new UserEntity
            {
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Password = dto.Password, // İleride Hash eklenebilir
                RoleId = 3, // Default: Buyer
                Enabled = true,
                CreatedAt = DateTime.Now,
                Phone = dto.Phone,
                Address = dto.Address
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveAsync();

            dto.Id = user.Id;
            return dto;
        }



        // Authenticate
        public async Task<UserDTO?> AuthenticateAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user != null && user.Password == password)
            {
                return MapToDto(user);
            }
            return null;
        }



        // ------------------- REQUEST SELLER STATUS -------------------
        public async Task RequestSellerStatusAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null && !user.IsSellerApproved)
            {
                user.HasPendingSellerRequest = true;
                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveAsync();
            }
        }

        //--------------TOGGLE-------------------
        public async Task ToggleEnabledAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null)
            {
                user.Enabled = !user.Enabled;
                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveAsync();
            }
        }

        public async Task RejectSellerAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user != null)
            {
                user.HasPendingSellerRequest = false; // Bekleyen isteği kaldır
                user.IsRejected = true;                // Reddedildi olarak işaretle
                user.IsSellerApproved = false;         // Onaylı değil

                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveAsync();
            }
        }
    }
}